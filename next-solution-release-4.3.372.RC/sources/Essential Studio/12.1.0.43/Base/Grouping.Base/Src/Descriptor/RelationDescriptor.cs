//-------------------------------------------------------------------------------------------------
// <copyright file="RelationDescriptor.cs" company="syncfusion">
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Design;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Various types of relations between two tables:
    /// </summary>
    public enum RelationKind
    {
        /// <summary>
        /// A Master-Details relation where matching keys in columns in the parent and child tables define
        /// a relationship between two tables. This a 1:n relation where each record in the child table
        /// can only belong to one parent record.
        /// </summary>
        RelatedMasterDetails,

        /// <summary>
        /// A foreign key relation where matching keys in columns in the parent and child table define
        /// a relationship between two tables. This a m:n relation. Field summaries of the related child table can
        /// be referenced using a '.' dot in the FieldDescriptor.MappingName of the main table.
        /// </summary>
        ForeignKeyKeyWords,

        // ForeignListItems

        /// <summary>
        /// Nested strong-typed collection inside a parent collection.
        /// </summary>
        UniformChildList,

        ////        /// <summary>
        //        /// Nested collection inside a parent collection, the collection can have different columns
        //        /// for different records of the parent collection.
        //        /// </summary>
        //        PolymorphChildList,

        /// <summary>
        /// A foreign-key relation for looking up values where an id column in the main table can be used
        /// to look up a record in a related table. This is an n:1 relation where multiple records in the parent
        /// table can reference the same record in the related table. Fields in the related table can
        /// be referenced using a '.' dot in the FieldDescriptor.MappingName of the main table.
        /// </summary>
        ForeignKeyReference,

        /// <summary>
        /// A object reference relation for looking up values from a strong typed collection.
        /// </summary>
        ListItemReference
    }

    #region RelationDescriptorCollection
    /// <summary>
    /// A collection of <see cref="RelationDescriptor"/> with constraints for a relation
    /// between two tables and schema information of child tables. An instance of this
    /// collection is returned by the <see cref="TableDescriptor.Relations"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [ListBindableAttribute(false)]
    [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class RelationDescriptorCollection : IDisposable, IList, ICloneable, IInsideCollectionEditorProperty
    {
        internal ArrayList _inner;
        internal SortedList _sorted;
        internal TableDescriptor _parenTableDescriptor;
        bool autoPopulated = false;
        int version = 0;
        int fieldsVersion = -1;
        int nestedCount = -1;
        bool modified = false;
        bool readOnly = false;
        bool isReset = true;
        bool inReset = false;
        int shadowedItemPropertiesVersion = -1;
        internal bool shouldPopulate = false;
        internal Table Table;

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

            foreach (RelationDescriptor rd in this._inner)
            {
                rd.ChildTableDescriptor.EnableOneTimePopulate();
            }
        }

        /// <summary>
        /// Occurs after a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changed;

        /// <summary>
        /// Occurs before a property in a nested element or the collection is changed.
        /// </summary>
        public event ListPropertyChangedEventHandler Changing;

        /// <summary>
        /// A Read-only and empty collection.
        /// </summary>
        public static RelationDescriptorCollection Empty = new RelationDescriptorCollection(null);

        internal bool insideCollectionEditor = false;

        /// <summary>Returns a string holding the current object. </summary>
        /// <returns>String representation of the current object. </returns>
        /// <override/>
        public override string ToString()
        {
            return String.Format("RelationDescriptorCollection: Count {0}, InsideColl {1}", Count, insideCollectionEditor);
        }

        Engine Engine
        {
            get
            {
                return this._parenTableDescriptor != null ? this._parenTableDescriptor.Engine : null;
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
                ////                TraceUtil.TraceCurrentMethodInfo(this);
                if (insideCollectionEditor != value)
                {
                    insideCollectionEditor = value;
                }
            }
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        public int NestedCount
        {
            get
            {
                if (nestedCount == -1)
                {
                    int count = 0;
                    foreach (RelationDescriptor rd in this)
                    {
                        if (rd.RelationKind == RelationKind.RelatedMasterDetails
                            || rd.RelationKind == RelationKind.UniformChildList)
                        {  
                            ////                            || rd.RelationKind == RelationKind.PolymorphChildList

                            count++;
                        }
                    }

                    nestedCount = count;
                    ////TraceUtil.TraceCurrentMethodInfo(nestedCount);
                }

                return this.nestedCount;
            }
        }

        /// <summary>
        /// The parent TableDescriptor this descriptor belongs to.
        /// </summary>
        public TableDescriptor ParentTableDescriptor
        {
            get
            {
                return _parenTableDescriptor;
            }
        }

        void IInsideCollectionEditorProperty.InitializeFrom(object other)
        {
            InitializeFrom((RelationDescriptorCollection)other);
        }

        /// <summary>
        /// Marks the collection as modified and avoids auto-population.
        /// </summary>
        public void Modify()
        {
            this.modified = true;
            this.RebuildSortedList();
        }

        internal bool inInitializeFrom = false;

        /// <summary>
        /// Copies settings from another collection and raises <see cref="Changing"/> and <see cref="Changed"/>
        /// events if differences to the other collection are detected.
        /// </summary>
        /// <param name="other">The source collection.</param>
        public void InitializeFrom(RelationDescriptorCollection other)
        {
            inInitializeFrom = true;
            int i;

            bool savedshouldPopulateThis = shouldPopulate;
            bool savedshouldPopulateOther = other.shouldPopulate;
            this.shouldPopulate = false;
            other.shouldPopulate = false;

            try
            {
                int count = Math.Min(Count, other.Count);

                this.RebuildSortedList();

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
                    Add(other[i].Clone());
                }

                this.RebuildSortedList();

                inInitializeFrom = false;
            }
            finally
            {
                inInitializeFrom = false;
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
            if (this._sorted == null)
            {
                _sorted = new SortedList();
                foreach (RelationDescriptor cd in _inner)
                {
                    _sorted.Add(cd.Name, cd);
                }

                nestedCount = -1;
            }
        }

        internal void RebuildSortedList()
        {
            _sorted = null;
            nestedCount = -1;
        }

        /// <summary>
        /// Initializes a new empty collection.
        /// </summary>
        public RelationDescriptorCollection()
            : this(null)
        {
        }

        internal RelationDescriptorCollection(TableDescriptor parentTableDescriptor)
        {
            this._inner = new ArrayList();
            _parenTableDescriptor = parentTableDescriptor;
        }

        internal RelationDescriptorCollection(TableDescriptor parentTableDescriptor, RelationDescriptor[] relationDescriptors)
            : this(parentTableDescriptor)
        {
            this.AddRange(relationDescriptors);
        }

        /// <summary>
        /// Resets the collection to its default state. If the collection is bound to a <see cref="TableDescriptor"/>,
        /// the collection will autopopulate itself the next time an item inside the collection is accessed.
        /// </summary>
        public void Reset()
        {
            bool b = this.modified || _inner.Count > 0;

            if (b)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            inReset = true;
            isReset = true;
            modified = false;
            version++;
            autoPopulated = false;
            _inner.Clear();
            this.RebuildSortedList();
            nestedCount = -1;
            autoPopulated = false;
            modified = false;
            isReset = true;

            if (b)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            inReset = false;
        }

        /// <summary>
        /// Resets the collection to its default state, autopopulates the collection, and marks the collection
        /// as modified. Call this method if you want to load the default items for the collection and then
        /// modify them (e.g. remove members from the auto-populated list).
        /// </summary>
        public void LoadDefault()
        {
            nestedCount = -1;
            if (this.IsModified)
            {
                Reset();
            }

            this.EnsureInitialized(true);
            this.modified = true;
        }
        
        bool inAddRange = false;

        /// <summary>
        /// Adds multiple elements at the end of the collection.
        /// </summary>
        /// <param name="relationDescriptors">The array whose elements should be added to the end of the collection.
        /// The array and its elements cannot be NULL references (Nothing in Visual Basic).
        /// </param>
        public void AddRange(RelationDescriptor[] relationDescriptors)
        {
            foreach (RelationDescriptor rd in relationDescriptors)
            {
                Add(rd);
            }
        }

        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnChanged(ListPropertyChangedEventArgs e)
        {
            if (e.Action != ListPropertyChangedType.ItemPropertyChanged)
            {   // TODO: e.Property != "ChildTableDescriptor.Fields" ???
                this.fieldsVersion++;
            }

            version++;
            shadowedItemPropertiesVersion = -1;
            nestedCount = -1;
            if (!this.inEnsureInitialized && !this.inReset)
            {
                modified = true;
            }

            if (!this.inEnsureInitialized && !this.insideCollectionEditor)
            {
                if (Changed != null)
                {
                    Changed(this, e);
                }
                //
                //                if (this._parenTableDescriptor != null && this._parenTableDescriptor.IsDesignTime())
                //                    this.EnsureSortedList();
            }
        }

        internal void RaisePropertyItemChanged(RelationDescriptor relation, DescriptorPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" || e.PropertyName == "MappingName" || e.PropertyName == "ChildTableName")
            {
                this.RebuildSortedList();
            }

            if (!this.InsideCollectionEditor)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, _inner.IndexOf(relation), relation, e.PropertyName, e));
                ////                TraceUtil.TraceCurrentMethodInfo(this, column.Name, e.PropertyName);
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

        internal void RaisePropertyItemChanging(RelationDescriptor relation, DescriptorPropertyChangedEventArgs e)
        {
            if (!this.InsideCollectionEditor)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemPropertyChanged, _inner.IndexOf(relation), relation, e.PropertyName, e));
            }
        }

        bool inEnsureInitialized = false;

        private bool hasSelfRelation;

        /// <summary>
        /// Determines if a self-relation was found when the collection was auto-populated.
        /// </summary>
        public bool HasSelfRelation
        {
            get
            {
                return this.hasSelfRelation;
            }
        }

        string ConstraintToString(Constraint constraint)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(constraint.GetType().Name);
            sb.Append(" {");
            sb.Append(constraint.ConstraintName);

            if (constraint is ForeignKeyConstraint)
            {
                ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
                sb.Append(", Table = " + foreignKeyConstraint.Table.TableName);
                sb.Append(", RelatedTable = " + foreignKeyConstraint.RelatedTable.TableName);
                sb.Append(" KeyPairs:");
                for (int n = 0; n < foreignKeyConstraint.Columns.Length; n++)
                {
                    sb.Append(" ");
                    sb.Append(foreignKeyConstraint.Columns[n].ColumnName);
                    sb.Append("->");
                    sb.Append(foreignKeyConstraint.RelatedColumns[n].ColumnName);
                }
            }

            if (constraint is UniqueConstraint)
            {
                UniqueConstraint uniqueConstraint = (UniqueConstraint)constraint;
                sb.Append(", Table = " + uniqueConstraint.Table.TableName);
                sb.Append(", IsPrimaryKey = " + uniqueConstraint.IsPrimaryKey);
                sb.Append(" Columns:");
                for (int n = 0; n < uniqueConstraint.Columns.Length; n++)
                {
                    sb.Append(" ");
                    sb.Append(uniqueConstraint.Columns[n].ColumnName);
                }
            }

            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// Debug Helper. Set this true if you want to have verbose output to Debug window for lazy population.
        /// </summary>
        public static bool VerboseEnsureInitialized = false;

        void EnsureInitialized(bool populate)
        {
            if (inEnsureInitialized || inAddRange || this.inInitializeFrom || !this.shouldPopulate)
            {
                return;
            }

            if (disableShouldPopulate)
            {
                shouldPopulate = false;
            }

            if (Table != null)
            {
                Table.RelatedTables.SynchronizeWithRelationDescriptor();
                return;
            }

            if (_parenTableDescriptor == null || _parenTableDescriptor.Engine == null || _parenTableDescriptor.Engine.InInitializeFrom)
            {
                return;
            }

            // Make sure fields are up to date - this will also initializa ItemProperties on demand.
            PropertyDescriptorCollection itemProperties = _parenTableDescriptor.ItemProperties;

            if (this._inner.Count == 0 && (itemProperties == null || itemProperties.Count == 0) && !_parenTableDescriptor.Engine.HasSourceList())
            {
                return;
            }

            bool _isReset = isReset;

            if (shadowedItemPropertiesVersion != this._parenTableDescriptor.ItemPropertiesVersion)
            {
                if (!modified)
                {
                    _isReset = true;
                    isReset = true;
                }
                else
                {
                    foreach (RelationDescriptor cd in this._inner)
                    {
                        cd.SetParentTableDescriptor(this._parenTableDescriptor);
                    }
                }

                shadowedItemPropertiesVersion = this._parenTableDescriptor.ItemPropertiesVersion;
            }

            if (!_isReset)
            {
                return;
            }

            if (!Engine.AutoPopulateRelations)
            {
                return;
            }

            int level = 0;
            TableDescriptor _p = _parenTableDescriptor.ParentTableDescriptor;
            while (_p != null)
            {
                level++;
                _p = _p.ParentTableDescriptor;
            }

            TableDescriptor _grandParentTableDescriptor = _parenTableDescriptor.ParentTableDescriptor;

            inEnsureInitialized = true;
            try
            {
                if (populate && _parenTableDescriptor != null && !this.modified)
                {
                    hasSelfRelation = false;

                    // protect when re-entering this method.
                    if (!inEnsureInitialized)
                    {
                        inEnsureInitialized = true;
                        _parenTableDescriptor.Engine.Table.EnsureInitialized(this);
                    }

                    inEnsureInitialized = true;

                    TraceUtil.TraceCalledFromIf(Switches.AutoPopulate.TraceVerbose, 10, version, _parenTableDescriptor);

                    PropertyDescriptorCollection pdc = _parenTableDescriptor.ItemProperties;
                    isReset = false;

                    if (VerboseEnsureInitialized)
                    {
                        TraceUtil.TraceCurrentMethodInfo(_parenTableDescriptor.Name);
                    }

                    RelationDescriptorCollection rdc = new RelationDescriptorCollection(null); // parentTableDescriptor will be null for RelationDescriptorCollection

                    object parenTableDescriptorList = _parenTableDescriptor.GetList();
                    DataTable dataTable = ListUtil.GetDataTable(parenTableDescriptorList);
                    string dataTableName;
                    if (dataTable != null)
                    {
                        dataTableName = dataTable.TableName;
                    }
                    else if (parenTableDescriptorList is ITypedList)
                    {
                        dataTableName = ListUtil.GetListName(parenTableDescriptorList);
                    }
                    else
                    {
                        dataTableName = "Not a DataTable";
                    }

                    if (VerboseEnsureInitialized)
                    {
                        Trace.WriteLine(_parenTableDescriptor.Name + ": DataTable.TableName = " + dataTableName);
                    }

                    if (pdc != null)
                    {
                        foreach (PropertyDescriptor pd in pdc)
                        {
                            // Only auto-populate DataRelation. For DataRelation, it is safe to assume
                            // that related tables should be displayed and that they actually are nested tables.
                            // With other relations (e.g. strong type collections) it is just a guessing game
                            // whether it is just a reference to a parent collection, related collection, or
                            // nested collection. For such scenarios, the user has to manually specify
                            // the relations.
                            DataTable dt = ListUtil.GetRelatedDataTable(pd);
                            string dtTableName = pd.PropertyType.Name;
                            if (dt != null)
                            {
                                dtTableName = dt.TableName;
                            }

                            // Change of mind for version 4.4: By default we also check now
                            // for nested collections. The user can simply set AutoPopulateRelations
                            // do prevent strong collections being picked up wrong if they are instead
                            // just references.
                            bool isNestedCollection = false;
                            // maybe later: if (Engine.AutoPopulateNestedCollections)
                            isNestedCollection = pd.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(pd.PropertyType)
                                && !(pd.PropertyType.IsArray && pd.PropertyType.GetElementType().IsPrimitive);

                            if (dt != null || isNestedCollection)
                            {
                                if (VerboseEnsureInitialized)
                                {
                                    Trace.WriteLine("Checking " + dataTableName + "[" + pd.Name + "].TableName = " + dtTableName);
                                }

                                if (level > Engine.MaxNestedCollectionRecurseLevel)
                                {
                                    //// Avoid StackOverflowException with self-related tables.
                                    //// User needs to manually add nested relations, e.g.
                                    ////            GridRelationDescriptor rd = new GridRelationDescriptor();
                                    ////            rd.MappingName = "OrderIDToParentID";
                                    ////            rd.ChildTableDescriptor.Relations.Add(new GridRelationDescriptor("TT"));
                                    ////            this.gridGroupingControl1.TableDescriptor.Relations.Add(rd);

                                    hasSelfRelation = true;
                                    if (VerboseEnsureInitialized)
                                    {
                                        Trace.WriteLine(dataTableName + "[" + pd.Name + "] is a SelfRelation");
                                    }
                                }

                                rdc.inEnsureInitialized = true;
                                rdc.modified = true;

                                if (!hasSelfRelation)
                                {
                                    RelationDescriptor relationDescriptor = this._parenTableDescriptor.CreateRelationDescriptor();
                                    relationDescriptor.MappingName = pd.Name;
                                    QueryAddRelationEventArgs e = new QueryAddRelationEventArgs(this._parenTableDescriptor, relationDescriptor);
                                    if (Engine != null)
                                    {
                                        Engine.RaiseQueryAddRelation(e);
                                    }

                                    if (!e.Cancel)
                                    {
                                        rdc.Add(relationDescriptor);
                                    }

                                    if (VerboseEnsureInitialized)
                                    {
                                        Trace.WriteLine("Add RelationDescriptor " + relationDescriptor.ToString() + " (RelatedMasterDetails)");
                                    }
                                }
                            }
                        }
                    }

                    if (dataTable != null)
                    {
                        bool hasPrimaryKeys = _parenTableDescriptor.ShouldSerializePrimaryKeyColumns();
                        foreach (Constraint constraint in dataTable.Constraints)
                        {
                            if (constraint is ForeignKeyConstraint)
                            {
                                ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;

                                if (VerboseEnsureInitialized)
                                {
                                    Trace.WriteLine("Checking " + ConstraintToString(constraint));
                                }

                                TableDescriptor grandParentTableDescriptor = _parenTableDescriptor.ParentTableDescriptor;

                                if (foreignKeyConstraint.RelatedTable.TableName == foreignKeyConstraint.Table.TableName)
                                {
                                    if (VerboseEnsureInitialized)
                                    {
                                        Trace.WriteLine("Not a ForeignKeyReference - possible SelfRelation");
                                    }

                                    foreignKeyConstraint = null;
                                }

                                while (foreignKeyConstraint != null && grandParentTableDescriptor != null)
                                {
                                    DataTable dt = ListUtil.GetDataTable(grandParentTableDescriptor.GetList());
                                    ////Console.WriteLine("Is " + foreignKeyConstraint.RelatedTable.TableName + " equal to " + (dt != null ? dt.TableName : null));
                                    if (dt != null && foreignKeyConstraint.RelatedTable.TableName == dt.TableName)
                                    {
                                        if (VerboseEnsureInitialized)
                                        {
                                            Trace.WriteLine(foreignKeyConstraint.ConstraintName + " ignored as ForeignKeyReference. This is a RelatedMasterDetails relation and " + dataTableName + " is the child table of " + dt.TableName);
                                        }

                                        // this is a master details constraint and this is the details table ...
                                        foreignKeyConstraint = null;
                                        break;
                                    }

                                    grandParentTableDescriptor = grandParentTableDescriptor.ParentTableDescriptor;
                                }

                                if (foreignKeyConstraint != null)
                                {
                                    if (ContainsNestedRelation(foreignKeyConstraint.RelatedTable, foreignKeyConstraint.ConstraintName))
                                    {
                                        if (VerboseEnsureInitialized)
                                        {
                                            Trace.WriteLine(foreignKeyConstraint.ConstraintName + " ignored as ForeignKeyReference. This is a RelatedMasterDetails relation and " + dataTableName + " is the child table");
                                        }

                                        foreignKeyConstraint = null;
                                    }

                                    ////                                    // Check if foreignKeyConstraint.RelatedTable has a relation to me
                                    ////                                    foreach (Constraint ct in foreignKeyConstraint.RelatedTable.Constraints)
                                    ////                                    {
                                    ////                                        Console.WriteLine(this.ConstraintToString(ct));
                                    ////                                        if (ct is UniqueConstraint)
                                    ////                                        {
                                    ////                                            UniqueConstraint uniqueConstraint = (UniqueConstraint) ct;
                                    ////                                            for (int n = 0; n < uniqueConstraint.Columns.Length; n++)
                                    ////                                            {
                                    ////                                                //if (uniqueConstraint.Columns[n] == foreignKeyConstraint.Columns[n])
                                    ////                                            }
                                    ////
                                    ////                                        }
                                    ////                                    }
                                }

                                if (foreignKeyConstraint != null)
                                {
                                    RelationDescriptor foreignKeyRelation = _parenTableDescriptor.CreateRelationDescriptor();
                                    foreignKeyRelation.MappingName = foreignKeyConstraint.ConstraintName;
                                    QueryAddRelationEventArgs e = new QueryAddRelationEventArgs(this._parenTableDescriptor, foreignKeyRelation);
                                    if (Engine != null)
                                    {
                                        Engine.RaiseQueryAddRelation(e);
                                    }

                                    if (!e.Cancel)
                                    {
                                        rdc.Add(foreignKeyRelation);
                                    }

                                    if (VerboseEnsureInitialized)
                                    {
                                        Trace.WriteLine("Add RelationDescriptor " + foreignKeyRelation.ToString() + " (ForeignKeyReference)");
                                    }
                                }
                            }
                        }
                    }

                    if (VerboseEnsureInitialized)
                    {
                        Trace.WriteLine(_parenTableDescriptor.Name + ": Found " + rdc.Count.ToString() + " relations.");
                    }

                    this.InitializeFrom(rdc);

                    autoPopulated = true;
                    modified = false;
                    if (Engine.HelpTracing)
                    {
                        TraceUtil.TraceCurrentMethodInfo("Reinitialize", this._parenTableDescriptor, pdc.Count);
                        TraceUtil.TraceCalledFrom();
                    }

                    shadowedItemPropertiesVersion = this._parenTableDescriptor.ItemPropertiesVersion;
                }
            }
            finally
            {
                inEnsureInitialized = false;
            }
        }

        /// <summary>
        /// Checks if the relatedTable has MasterDetailsRelation with MappingName equal to constraintName
        /// or if other related tables have a MasterDetailsRelation with MappingName equal to constraintName.
        /// </summary>
        /// <param name="relatedTable">The DataTable</param>
        /// <param name="constraintName">The constraint name</param>
        /// <returns>returns boolean value to check if relatedTable has MasterDetailsRelation with MappingName equal to constraintName</returns>
        bool ContainsNestedRelation(DataTable relatedTable, string constraintName)
        {
            PropertyDescriptorCollection relatedPdc = ListUtil.GetItemProperties(relatedTable);
            if (relatedPdc[constraintName] != null)
            {
                return true;
            }

            foreach (Constraint constraint in relatedTable.Constraints)
            {
                if (constraint is ForeignKeyConstraint)
                {
                    ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)constraint;
                    if (ContainsNestedRelation(foreignKeyConstraint.RelatedTable, constraintName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a copy of the collection and all its elements.
        /// </summary>
        /// <returns>A copy of the collection and all its elements.</returns>
        public RelationDescriptorCollection Clone()
        {
            int count = Count;
            bool b = this.insideCollectionEditor;
            this.insideCollectionEditor = true;
            RelationDescriptor[] relationDescriptors = new RelationDescriptor[count];
            for (int n = 0; n < count; n++)
            {
                relationDescriptors[n] = this[n].Clone();
            }

            RelationDescriptorCollection c = new RelationDescriptorCollection(this._parenTableDescriptor, relationDescriptors);
            c.modified = modified;
            c.autoPopulated = autoPopulated;
            c.insideCollectionEditor = true;
            this.insideCollectionEditor = b;
            c.RebuildSortedList();
            return c;
        }

        /// <summary>Determines if the specified object and current object are equal.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if both objects are equal; False otherwise.</returns>
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
            else if (!(obj is RelationDescriptorCollection))
            {
                return false;
            }

            return Equals((RelationDescriptorCollection)obj);
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
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
                this.EnsureInitialized(true);
                return version;
            }
        }

        ////        internal int FieldsVersion
        ////        {
        ////            get
        ////            {
        ////                return fieldsVersion;
        ////            }
        ////        }

        /// <summary>
        /// Determines if the collection was modified from its default state.
        /// </summary>
        public bool IsModified
        {
            get
            {
                return modified;
            }
        }
        
        bool Equals(RelationDescriptorCollection other)
        {
            int count = Count;
            if (other.modified != modified)
            {
                return false;
            }

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
        public RelationDescriptor this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return (RelationDescriptor)_inner[index];
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

                if (!modified)
                {
                    EnsureInitialized(false);
                }

                if (_inner[index] != value)
                {
                    OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.ItemChanged, index, value, null));
                    this.RebuildSortedList();
                    ////                    if (_sorted != null)
                    ////                        _sorted.Remove(this[index].Name);
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
        public RelationDescriptor this[string name]
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

                return (RelationDescriptor)_inner[index];
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

        internal int Find(string name)
        {
            this.EnsureSortedList();
            if (_sorted != null)
            {
                RelationDescriptor cd = (RelationDescriptor)_sorted[name];
                if (cd != null)
                {
                    return cd.index;
                }

                for (int n = 0; n < this._inner.Count; n++)
                {
                    if (this[n].MappingName == name || this[n].ChildTableName == name)
                    {
                        return n;
                    }
                }
            }

            return -1;
        }

        internal int Find(PropertyDescriptor pd)
        {
            return Find(pd.Name);
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
        /// </remarks>
        public bool Contains(RelationDescriptor value)
        {
            if (value == null)
            {
                return false;
            }

            if (!modified)
            {
                EnsureInitialized(true);
            }

            this.EnsureSortedList();
            return _sorted.Contains(value.Name);
        }

        /// <summary>
        /// Determines if an element with the specified name belongs to this collection.
        /// </summary>
        /// <param name="name">The name of the element to locate in the collection. </param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        public bool Contains(string name)
        {
            if (!modified)
            {
                EnsureInitialized(true);
            }

            this.EnsureSortedList();
            return _sorted.Contains(name);
        }

        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        public int IndexOf(RelationDescriptor value)
        {
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
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        public void CopyTo(RelationDescriptor[] array, int index)
        {
            int n = 0;
            foreach (RelationDescriptor item in this)
            {
                array[index + n] = item;
                n++;
            }
        }

        RelationDescriptorCollection SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns an enumerator for the entire collection
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection.
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public RelationDescriptorCollectionEnumerator GetEnumerator()
        {
            return new RelationDescriptorCollectionEnumerator(this);
        }

        /// <summary>
        /// Inserts a descriptor element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="value">The element to insert. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        public void Insert(int index, RelationDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Insert, index, value, null));
            _inner.Insert(index, value);
            this.RebuildSortedList();
            ////            if (_sorted != null)
            ////                _sorted.Add(value.Name, value);
            value.SetCollection(this);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }
        }

        /// <summary>
        /// Removes the specified descriptor element from the collection.
        /// </summary>
        /// <param name="value">The element to remove from the collection. If the value is NULL or the element is not contained
        /// in the collection the method will do nothing.</param>
        /// <remarks>
        /// The method
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the table or the TableDescriptor.
        /// </remarks>
        public void Remove(RelationDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is Read-only.");
            }

            if (value == null)
            {
                return;
            }

            if (!modified)
            {
                this.EnsureInitialized(true);
            }

            int index = IndexOf(value);
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            this.RebuildSortedList();
            ////            if (_sorted != null)
            ////                _sorted.Remove(value.Name);
            _inner.Remove(value);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }

            ////            if (this._parenTableDescriptor != null)
            //                value.Dispose();
            OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
        }

        /// <summary>
        /// Adds RelationDescriptor to the end of the collection.
        /// </summary>
        /// <param name="value">The element to be added to the end of the collection. The value must not be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based collection index at which the value has been added.</returns>
        public int Add(RelationDescriptor value)
        {
            if (readOnly)
            {
                throw new InvalidOperationException("Collection is read only");
            }

            if (value == null)
            {
                throw new ArgumentNullException();
            }

            // Changed Add behavior for 3.0.0.12 - user should call explicitly Clear()
            // if collection should be reset before adding fields.
            if (!this.inEnsureInitialized)
            {
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, -1, value, null));

                this.EnsureInitialized(true);
                if (!this.modified && this._inner.Count > 0)
                {
                    if (Contains(value.Name) && Engine.VersionInfo.CompareTo("3.0.0.12") <= 0)
                    {
                        FieldDescriptorCollection.ShowAddRangeChangedWarning("Relations");
                    }
                }
            }

            // If value is added with a name that already exists, replace 
            // the old value with the new descriptor.
            int index = -1;
            if (value.Name != null && value.Name.Length > 0)
            {
                EnsureSortedList();
                index = IndexOf(value.Name);
                if (index != -1)
                {
                    _inner[index] = value;
                    _sorted[value.Name] = value;
                }
            }
            else
            {
                SuggestName(value);
            }

            if (index == -1)
            {
                index = _inner.Add(value);
                if (_sorted != null)
                {
                    _sorted.Add(value.Name, value);
                }
            }

            value.index = index;
            value.SetCollection(this);

            if (!this.inEnsureInitialized)
            {
                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Add, index, value, null));
            }

            return index;
        }

        void SuggestName(RelationDescriptor value)
        {
            if (value.ChildTableName.Length == 0 && value.MappingName.Length == 0 && this._parenTableDescriptor != null)
            {
                PropertyDescriptorCollection pdc = this._parenTableDescriptor.ItemProperties;
                if (pdc.Count > this.Count)
                {
                    foreach (PropertyDescriptor pd in pdc)
                    {
                        if (IndexOf(pd.Name) == -1 && ListUtil.PropertyDescriptorIsARelation(pd))
                        {
                            value.MappingName = pd.Name;
                            return;
                        }
                    }
                }
            }

            value.Name = "Relation " + Count.ToString();
            value.relationNameModified = false;
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

            RelationDescriptor value = (RelationDescriptor)_inner[index];
            OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Remove, index, value, null));
            this.RebuildSortedList();
            _inner.RemoveAt(index);
            for (int n = index; n < Count; n++)
            {
                this[n].index = n;
            }
            ////            if (this._parenTableDescriptor != null)
            ////                value.Dispose();

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

            _sorted = null;
            this._parenTableDescriptor = null;

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
                ////this.EnsureInitialized(false);
                OnChanging(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
                _inner.Clear();
                if (_sorted != null)
                {
                    _sorted.Clear();
                }

                OnChanged(new ListPropertyChangedEventArgs(ListPropertyChangedType.Refresh, -1, null, null));
            }

            modified = true;
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
        /// Returns False since this collection has no fixed size.
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

        int ICollection.Count
        {
            get
            {
                this.EnsureInitialized(true);
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
                this[index] = (RelationDescriptor)value;
            }
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (RelationDescriptor)value);
        }

        void IList.Remove(object value)
        {
            Remove((RelationDescriptor)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((RelationDescriptor)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((RelationDescriptor)value);
        }

        int IList.Add(object value)
        {
            return Add((RelationDescriptor)value);
        }

        #endregion

        #region ICollection Private Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((RelationDescriptor[])array, index);
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
    /// Enumerator class for <see cref="RelationDescriptor"/> elements of a <see cref="RelationDescriptorCollection"/>.
    /// </summary>
    public class RelationDescriptorCollectionEnumerator : IEnumerator
    {
        int _cursor = -1, _next = -1;
        RelationDescriptorCollection _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        public RelationDescriptorCollectionEnumerator(RelationDescriptorCollection collection)
        {
            _coll = collection;
            _next = ((ICollection)_coll).Count > 0 ? 0 : -1;
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = -1;
            _next = ((ICollection)_coll).Count > 0 ? 0 : -1;
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
        public RelationDescriptor Current
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
            if (_next >= ((ICollection)_coll).Count)
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
    /// The type converter for <see cref="RelationDescriptor"/> objects. <see cref="RelationDescriptorTypeConverter"/>
    /// is a <see cref="DescriptorBaseConverter"/>. It overrides the default behavior of the
    /// ConvertTo method and adds support for design-time code serialization.
    /// </summary>
    public class RelationDescriptorTypeConverter : DescriptorBaseConverter
    {
        /// <override/>
        /// <summary>
        /// Returns a collection of properties for the specified type.
        /// </summary>
        /// <param name="context">Format context.</param>
        /// <param name="value">Object type.</param>
        /// <param name="attributes">An array of objects of type <see cref="System.Attribute"/> that will be used as a filter.</param>
        /// <returns>Property descriptor collection.</returns>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(value.GetType(), attributes);

            string[] atts = new string[]
            {
                "Name",
                "HeaderText",
                "ChildTableName",
                "RelationKind",
                "AllowCacheChildList",
                "RelationKeys",
                "ChildTableDescriptor"
            };

            return pds.Sort(atts);
        }
    }
    #endregion

    /// <summary>
    /// An IComparer implementation for comparing the names of
    /// two <see cref="RelationDescriptor"/> objects.
    /// </summary>
    public class RelationDescriptorNameComparer : IComparer
    {
        #region IComparer Members

        /// <summary>
        /// Casts the objects to <see cref="RelationDescriptor"/> and compares
        /// the <see cref="RelationDescriptor.Name"/> of both objects.
        /// </summary>
        /// <param name="x">The first object.</param>
        /// <param name="y">The second object.</param>
        /// <returns>True if both have the same name; False otherwise.</returns>
        public int Compare(object x, object y)
        {
            RelationDescriptor c = (RelationDescriptor)x;
            RelationDescriptor d = (RelationDescriptor)y;
            return c.Name.CompareTo(d.Name);
        }
        #endregion
    }

    /// <summary>
    /// A RelationDescriptor defines constraints for a relation
    /// between two tables and schema information of child tables.
    /// RelationDescriptors are managed by the <see cref="RelationDescriptorCollection"/> which
    /// is returned by the <see cref="TableDescriptor.Relations"/> property
    /// of a <see cref="TableDescriptor"/>.
    /// </summary>
    [TypeConverter(typeof(RelationDescriptorTypeConverter))]
    public class RelationDescriptor : DescriptorBase, ICloneable, IStandardValuesProvider
    {
        #region Fields
        TableDescriptor parentTableDescriptor;
        RelationDescriptorCollection collection;
        internal int index;

        string relationName = string.Empty;
        internal bool relationNameModified = false;

        string headerText = string.Empty;
        bool headerTextModified = false;

        RelationKind relationKind;
        bool relationKindModified = false;
        bool allowCacheChildList = true;

        string mappingName = string.Empty;
        int parentTableDescriptorItemPropertiesVersion = -1;
        string childTableName = string.Empty;  // standard values from Engine.SourceListSet
        bool childTableNameModified = false;
        TableDescriptor childTableDescriptor; // created internally, initialized with properties from Engine.SourceListSet[childListName]

        RelationKeyDescriptorCollection relationKeys;

        internal int parentTableDescriptorFieldsVersion = -1;

        ////bool isDefault = true;
        #endregion

        static int tableCounter = 0;
        int tableId = 0;

        #region Ctor
        /// <summary>
        /// Initializes a new empty relation descriptor.
        /// </summary>
        /// <summary>
        /// Initializes a new empty relation descriptor.
        /// </summary>
        public RelationDescriptor()
        {
            relationKeys = new RelationKeyDescriptorCollection(this);

            tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(tableId);
            }
        }

        /// <summary>
        /// Initializes a new relation descriptor with the given name.
        /// </summary>
        /// <param name="relationName">Name of the relation.</param>
        public RelationDescriptor(string relationName)
            : this()
        {
            this.relationName = relationName;
            this.headerText = relationName;
            this.childTableName = relationName;
        }
        #endregion

        static int disposedCounter = 0;

        int disposedId = -1;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int DisposedId
        {
            get
            {
                return disposedId;
            }

            set
            {
                disposedId = value;
            }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.relationName, this.tableId);
            }

            if (disposing)
            {
                disposedCounter++;
                DisposedId = disposedCounter;
                ////                if (DisposedId == 36)
                //                    System.Diagnostics.Debugger.Break();
                if (childTableDescriptor != null)
                {
                    childTableDescriptor.PropertyChanged -= new DescriptorPropertyChangedEventHandler(childTableDescriptor_PropertyChanged);
                    childTableDescriptor.PropertyChanging -= new DescriptorPropertyChangedEventHandler(childTableDescriptor_PropertyChanging);
                    childTableDescriptor.Dispose();
                    childTableDescriptor = null;
                }

                relationName = "Disposed";
                parentTableDescriptor = null;
                collection = null;
                relationKeys.Dispose();
            }

            base.Dispose(disposing);
        }

        internal bool mappingNameModified = false;

        /// <summary>
        /// The name of a PropertyDescriptor in the parent table that contains details
        /// about this relation. With a DataTable for example, there will be a DataRelation
        /// with such a MappingName. The MappingName can also be the name of child-collection
        /// when the main table is a strong-typed collection.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("Specifies which column of a datatable to be used as DataRelation.")]
        public string MappingName
        {
            get
            {
                return this.mappingName;
            }

            set
            {
                if (this.mappingName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MappingName"));
                    bool allowName = !this.relationNameModified || ((this.childTableName == string.Empty && this.mappingName == string.Empty) || InPropertyGrid());
                    string strchildTableName = childTableName;
                    string strName = relationName;
                    string strMappingName = mappingName;
                    this.mappingName = value;
                    mappingNameModified = true;
                    this.relationKeys.inner.Clear();
                    SyncNameFromMappingName(true);
                    ////this.parentTableDescriptorFieldsVersion = -1;
                    try
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
                    }
                    catch
                    {
                        relationName = strName;
                        mappingName = strMappingName;
                        childTableName = strchildTableName;
                        throw;
                    }
                }
            }
        }

        internal bool ShouldSerializeMappingName()
        {
            return this.mappingName != string.Empty; //// && mappingNameModified;
        }

        /// <summary>
        /// Resets the mapping name.
        /// </summary>
        public void ResetMappingName()
        {
            if (this.ShouldSerializeMappingName())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("MappingName"));
                this.mappingName = string.Empty;
                mappingNameModified = false;
                if (ShouldSerializeChildTableName())
                {
                    SyncNameFromChildTableName(false);
                }

                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("MappingName"));
            }
        }

        void SyncNameFromDataTable(DataTable dataTable, bool allowName)
        {
            if (dataTable == null)
            {
                return;
            }

            if (!this.ShouldSerializeName() && allowName)
            {
                this.relationName = dataTable.TableName;
            }

            if (!this.ShouldSerializeChildTableName())
            {
                this.childTableName = dataTable.TableName;
            }

            if (!this.ShouldSerializeHeaderText())
            {
                this.headerText = dataTable.TableName;
            }

            SourceListSetEntry se = ParentTableDescriptor.Engine.SourceListSet[dataTable.TableName];
            if (se != null)
            {
                if (this.childTableDescriptor != null)
                {
                    this.childTableDescriptor.SetItemProperties(se.List);
                    itemProperties = this.ChildTableDescriptor.ItemProperties;
                }
                else
                {
                    itemProperties = ListUtil.GetItemProperties(se.List);
                }
            }
            else
            {
                if (this.childTableDescriptor != null)
                {
                    this.childTableDescriptor.SetItemProperties(dataTable);
                    itemProperties = this.ChildTableDescriptor.ItemProperties;
                }
                else
                {
                    itemProperties = ListUtil.GetItemProperties(dataTable);
                }
            }
        }

        void SyncNameFromMappingName(bool allowName)
        {
            if (parentTableDescriptor != null)
            {
                if (parentTableDescriptor.ShouldSerializeItemProperties())
                {
                    if (parentTableDescriptor.ItemProperties != null)
                    {
                        // Master Details
                        PropertyDescriptor pd = parentTableDescriptor.ItemProperties[mappingName];
                        if (pd != null)
                        {
                            DataRelation dr = ListUtil.GetRelatedDataRelation(pd);
                            if (dr != null)
                            {
                                if (!this.ShouldSerializeRelationKind())
                                {
                                    relationKind = RelationKind.RelatedMasterDetails;
                                }

                                DataTable dataTable = dr.ChildTable;
                                SyncNameFromDataTable(dataTable, allowName);
                                SyncRelationKeysFromDataRelation(dr);
                                return;
                            }

                            // Nested collection (UniformChildList)
                            PropertyDescriptorCollection pdc = null;
                            string name = string.Empty;
                            object itemList = parentTableDescriptor.GetList();
                            object arrayList = null;

                            //// Find first item in parent list that returns a list with 
                            //// at least one entry.

                            if (itemList is IPassThroughGroupingResult)
                            {
                                pdc = ListUtil.GetItemProperties(pd.PropertyType);
                            }
                            else
                            {
                                pdc = ListUtil.GetRelatedItemProperties(itemList, pd, out name, out arrayList);
                            }

                            if (pdc != null && pdc.Count > 0)
                            {
                                if (!this.ShouldSerializeRelationKind())
                                {
                                    relationKind = RelationKind.UniformChildList;
                                }

                                if (!this.ShouldSerializeName())
                                {
                                    this.relationName = mappingName;
                                }

                                if (!this.ShouldSerializeChildTableName())
                                {
                                    this.childTableName = name;
                                }

                                if (!this.ShouldSerializeHeaderText())
                                {
                                    this.headerText = mappingName;
                                }

                                itemProperties = pdc;
                                if (this.childTableDescriptor != null)
                                {
                                    this.childTableDescriptor.SetItemProperties(pdc);
                                    this.childTableDescriptor.list = arrayList;
                                }

                                if (relationKind == RelationKind.UniformChildList && !this.parentTableDescriptor.Engine.UseOldUniformChildListRelation)
                                {
                                    // New in version 4.2: Add a relation key that maps parent record to child record similar to 
                                    // a master-details relation.
                                    relationKeys.inner.Clear();
                                    relationKeys.InnerAdd("##This", "##Parent");
                                }
                            }
                        }
                    }
                }

                if (parentTableDescriptor.Engine != null
                    && parentTableDescriptor.Engine.SourceListSet.DataSet != null)
                {
                    // Master Details
                    DataRelation dr = parentTableDescriptor.Engine.SourceListSet.DataSet.Relations[mappingName];
                    if (dr != null)
                    {
                        if (!this.ShouldSerializeRelationKind())
                        {
                            relationKind = RelationKind.RelatedMasterDetails;
                        }

                        SyncNameFromDataTable(dr.ChildTable, allowName);
                        SyncRelationKeysFromDataRelation(dr);
                        return;
                    }

                    // Foreign Key
                    DataTable dataTable = ListUtil.GetDataTable(parentTableDescriptor.GetList());

                    if (dataTable != null && dataTable.Constraints.Contains(mappingName))
                    {
                        ForeignKeyConstraint foreignKeyConstraint = dataTable.Constraints[mappingName] as ForeignKeyConstraint;

                        if (foreignKeyConstraint != null)
                        {
                            if (!this.ShouldSerializeRelationKind())
                            {
                                relationKind = RelationKind.ForeignKeyReference;
                            }

                            // Add keys without triggering Changed event
                            RelationKeyDescriptorCollection rkdc = RelationKeys;
                            rkdc.inner.Clear();
                            for (int n = 0; n < foreignKeyConstraint.Columns.Length; n++)
                            {
                                rkdc.InnerAdd(foreignKeyConstraint.Columns[n].ColumnName, foreignKeyConstraint.RelatedColumns[n].ColumnName);
                            }

                            SyncNameFromDataTable(foreignKeyConstraint.RelatedTable, allowName);
                        }
                    }
                }
            }
        }

        void SyncRelationKeysFromDataRelation(DataRelation dr)
        {
            if (dr != null)
            {
                string[] childColumnNames = new string[dr.ChildColumns.Length];
                string[] parentColumnNames = new string[dr.ParentColumns.Length];
                for (int n = 0; n < childColumnNames.Length; n++)
                {
                    childColumnNames[n] = dr.ChildColumns[n].ColumnName;
                    parentColumnNames[n] = dr.ParentColumns[n].ColumnName;
                }

                this.RelationKeys.inner.Clear();
                for (int n = 0; n < childColumnNames.Length; n++)
                {
                    relationKeys.InnerAdd(parentColumnNames[n], childColumnNames[n]);
                }
            }
        }

        void SyncNameFromChildTableName(bool allowName)
        {
            if (parentTableDescriptor != null && parentTableDescriptor.Engine != null)
            {
                SourceListSetEntry entry = parentTableDescriptor.Engine.SourceListSet[childTableName];
                if (entry != null && entry.List != null)
                {
                    object list = entry.List;
                    if (!this.ShouldSerializeName() && allowName)
                    {
                        this.relationName = ListUtil.GetListName(list);
                    }

                    if (!this.ShouldSerializeHeaderText())
                    {
                        this.headerText = relationName;
                    }

                    if (this.childTableDescriptor != null)
                    {
                        this.childTableDescriptor.SetItemProperties(list);
                    }
                }
            }
        }

        /// <summary>
        /// The type of relation this descriptor defines.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The type of relation this descriptor defines.")]
        public RelationKind RelationKind
        {
            get
            {
                return this.relationKind;
            }

            set
            {
                if (this.relationKind != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RelationKind"));
                    this.relationKind = value;
                    this.relationKindModified = true;
                    if (this.childTableDescriptor != null)
                    {
                        this.childTableDescriptor.itemPropertiesVersion = -1;
                    }

                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RelationKind"));
                }
            }
        }

        /// <summary>
        /// Determines if <see cref="RelationKind"/> property was modified.
        /// </summary>
        /// <returns>True if the property was modified.</returns>
        public bool ShouldSerializeRelationKind()
        {
            return relationKindModified;
            ////            this.relationKind != RelationKind.RelatedMasterDetails &&
            //                (mappingName == "" || relationKind == RelationKind.UniformChildList);
        }

        /// <summary>
        /// Resets the relation kind to RelationDescriptor.
        /// </summary>
        public void ResetRelationKind()
        {
            if (this.relationKindModified)
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("RelationKind"));
                relationKindModified = false;
                if (this.mappingName != string.Empty)
                {
                    this.SyncNameFromMappingName(false);
                }
                else if (this.childTableName != string.Empty)
                {
                    this.SyncNameFromChildTableName(false);
                }

                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("RelationKind"));
            }
        }

        /// <summary>
        /// If RelationKind.UniformChildList was specified, this value indicates whether
        /// the ChildList / Related View that is associated with a view can be cached or
        /// if it should be requeried each time by calling PropertyDescriptor.GetValue(MappingName).
        /// Note that a DataView will always create a new DataRelatedView in such case and
        /// therefore it is recommended to allow cache the child list.
        /// </summary>
        [Description("Specifies the ChildList / Related View that is associated with a view can be cached or if it should be required each time by calling PropertyDescriptor.GetValue(MappingName).")]
        public bool AllowCacheChildList
        {
            get
            {
                return this.allowCacheChildList;
            }

            set
            {
                if (this.allowCacheChildList != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("AllowCacheChildList"));
                    this.allowCacheChildList = value;
                    OnPropertyChanged(new DescriptorPropertyChangedEventArgs("AllowCacheChildList"));
                }
            }
        }

        /// <summary>
        /// Determines if <see cref="AllowCacheChildList"/> property was modified.
        /// </summary>
        /// <returns>True if the property was modified.</returns>
        public bool ShouldSerializeAllowCacheChildList()
        {
            return this.allowCacheChildList == false && this.RelationKind == RelationKind.UniformChildList;
        }

        /// <summary>
        /// Resets the AllowCacheChildList property.
        /// </summary>
        public void ResetAllowCacheChildList()
        {
            this.allowCacheChildList = true;
        }
        
        #region RelationKeys

        /// <summary>
        /// A collection of <see cref="RelationKeyDescriptor"/> that defines the mapping between parent and child columns in a master details relation.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [EditorAttribute(typeof(GroupingCollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("A collection of RelationKeyDescriptor that defines the mapping between parent and child columns in a master details relation.")]
        public RelationKeyDescriptorCollection RelationKeys
        {
            get
            {
                return relationKeys;
            }
        }

        /// <summary>
        /// Determines if RelationKeys have been added.
        /// </summary>
        /// <returns>True if the RelationKeys have been added.</returns>
        public bool ShouldSerializeRelationKeys()
        {
            return !this.ShouldSerializeMappingName() && RelationKeys.Count > 0;
        }

        /// <summary>
        /// Clears <see cref="RelationKeys"/>.
        /// </summary>     
        public void ResetRelationKeys()
        {
            if (ShouldSerializeRelationKeys())
            {
                RelationKeys.Clear();
            }
            else
            {
                RelationKeys.inner.Clear();
            }
        }

        #endregion

        #region Parent

        internal void SetCollection(RelationDescriptorCollection collection)
        {
            this.collection = collection;
            if (this.ShouldSerializeHeaderText() ||
                this.ShouldSerializeName() ||
                (this.childTableDescriptor != null && childTableDescriptor.GetModified()))
            {
                this.collection.Modify();
            }

            this.SetParentTableDescriptor(collection._parenTableDescriptor);
        }

        /// <summary>
        /// The collection this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RelationDescriptorCollection Collection
        {
            get
            {
                return collection;
            }
        }

        bool inSetParentTableDescriptor = false;
        internal void SetParentTableDescriptor(TableDescriptor parentTableDescriptor)
        {
            if (this.parentTableDescriptor != parentTableDescriptor
                || (parentTableDescriptor != null && parentTableDescriptorItemPropertiesVersion != parentTableDescriptor.ItemPropertiesVersion))
            {
                inSetParentTableDescriptor = true;
                this.parentTableDescriptor = parentTableDescriptor;
                if (this.collection == null)
                {
                    this.collection = this.parentTableDescriptor.Relations;
                }

                if (this.childTableDescriptor != null)
                {
                    this.childTableDescriptor.SetParentTableDescriptor(parentTableDescriptor);
                }

                if (this.mappingName != string.Empty)
                {
                    this.SyncNameFromMappingName(false);
                }

                foreach (RelationKeyDescriptor relationKey in RelationKeys.inner)
                {
                    relationKey.SetParentRelationDescriptor(this);
                }

                parentTableDescriptorFieldsVersion = -1;
                inSetParentTableDescriptor = false;
                parentTableDescriptorItemPropertiesVersion = parentTableDescriptor.ItemPropertiesVersion;
            }
        }

        /// <summary>
        /// The parent TableDescriptor this descriptor belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public TableDescriptor ParentTableDescriptor
        {
            get
            {
                return parentTableDescriptor;
            }
        }

        /// <summary>
        /// Ensures relation keys are properly initialized after changes to the parent table descriptor.
        /// </summary>
        public void EnsureInitialized()
        {
            if (this.parentTableDescriptor != null)
            {
                if (this.parentTableDescriptorFieldsVersion != parentTableDescriptor.Fields.Version)
                {
                    this.parentTableDescriptorFieldsVersion = parentTableDescriptor.Fields.Version;

                    if (this.mappingName != string.Empty)
                    {
                        this.SyncNameFromMappingName(false);
                    }

                    int count1 = ParentTableDescriptor.Fields.Count;
                    int count2 = ChildTableDescriptor.Fields.Count;

                    bool same = ParentTableDescriptor.Name == ChildTableDescriptor.Name;

                    foreach (RelationKeyDescriptor relationKey in RelationKeys.inner)
                    {
                        relationKey.SetParentRelationDescriptor(this);
                        if (relationKey.ChildKeyFieldName != null)
                        {
                            relationKey.SetChildKeyField(ChildTableDescriptor.Fields[relationKey.ChildKeyFieldName]);
                        }
                        else
                        {
                            relationKey.SetChildKeyField(null);
                        }

                        if (relationKey.ParentKeyField != null)
                        {
                            relationKey.SetParentKeyField(ParentTableDescriptor.Fields[relationKey.ParentKeyFieldName]);
                        }
                        else
                        {
                            relationKey.SetParentKeyField(null);
                        }

                        same &= relationKey.ChildKeyFieldName == relationKey.ParentKeyFieldName;
                    }

                    if (same)
                    {
                        throw new InvalidOperationException("Self-relation with recursion in fields ..." + this.Name);
                    }

                    ChildTableDescriptor.SetParentTableDescriptor(this.parentTableDescriptor);
                }
            }
        }

        #endregion

        #region Events

        internal void RaisePropertyChanging(string name, EventArgs inner)
        {
            if (ShouldRaiseChangeEvent())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs(name, inner));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param relationName="e" name="e">A <see cref="PropertyChangedEventArgs" /> that contains the event data.</param>
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
        /// Occurs when a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs before a property is changed.
        /// </summary>
        public event DescriptorPropertyChangedEventHandler PropertyChanging;

        internal void RaisePropertyChanged(string name, EventArgs inner)
        {
            if (ShouldRaiseChangeEvent())
            {
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs(name, inner));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param relationName="e" name="e">A <see cref="DescriptorPropertyChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnPropertyChanged(DescriptorPropertyChangedEventArgs e)
        {
            if (this.Disposing)
            {
                return;
            }

            parentTableDescriptorFieldsVersion = -1;

            if (childTableDescriptor != null)
            {
                childTableDescriptor.EnableOneTimePopulate();
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

        #region Copy Members

        /// <summary>Resets the relation.</summary>
        /// <override/>
        public override void Reset()
        {
            ////this.relationNameModified = false;
            this.relationKeys.inner.Clear();
            ////this.headerTextModified = false;
            this.parentTableDescriptorFieldsVersion = -1;
        }

        /// <summary>
        /// Initializes this object and copies properties from another object. <see cref="PropertyChanging"/>
        /// and <see cref="PropertyChanged"/> events are raised for every property that is modified. If both
        /// objects are equal, no events are raised.
        /// </summary>
        /// <param name="other">The source object.</param>
        public virtual void InitializeFrom(RelationDescriptor other)
        {
            if (this.Equals(other))
            {
                return;
            }

            if (other.ShouldSerializeMappingName())
            {
                this.MappingName = other.MappingName;
            }
            else
            {
                this.ResetMappingName();
            }

            if (other.ShouldSerializeName())
            {
                this.Name = other.Name;
            }
            else
            {
                this.ResetName();
            }

            this.relationName = other.relationName;
            if (other.ShouldSerializeHeaderText())
            {
                this.HeaderText = other.HeaderText;
            }
            else
            { 
                this.ResetHeaderText(); 
            }

            if (other.ShouldSerializeChildTableName())
            {
                this.ChildTableName = other.ChildTableName;
            }
            else
            {
                this.ResetChildTableName();
            }

            if (other.ShouldSerializeRelationKind())
            {
                this.RelationKind = other.RelationKind;
            }
            else
            {
                this.ResetRelationKind();
            }

            if (other.ShouldSerializeAllowCacheChildList())
            {
                this.AllowCacheChildList = other.AllowCacheChildList;
            }
            else
            {
                this.ResetAllowCacheChildList();
            }

            if (other.ShouldSerializeRelationKeys())
            {
                this.RelationKeys.InitializeFrom(other.RelationKeys);
            }
            else
            {
                this.ResetRelationKeys();
            }

            if (other.ShouldSerializeChildTableDescriptor())
            {
                this.ChildTableDescriptor.InitializeFrom(other.ChildTableDescriptor);
            }
            else
            {
                this.ResetChildTableDescriptor();
            }

            this.parentTableDescriptorFieldsVersion = -1;
        }

        int suspendChange = 0;

        void SuspendChangeEvents()
        {
            suspendChange++;
        }

        void ResumeChangeEvents()
        {
            suspendChange--;
        }

        bool ShouldRaiseChangeEvent()
        {
            return suspendChange <= 0;
        }
        
        /// <summary>
        /// Copies all properties from another element without raising Changing or Changed events.
        /// </summary>
        /// <param name="other">The source object.</param>
        protected virtual void CopyMembersFrom(RelationDescriptor other)
        {
            this.relationName = other.relationName;
            this.relationKind = other.relationKind;
            this.relationKindModified = other.relationKindModified;
            this.allowCacheChildList = other.allowCacheChildList;
            this.relationNameModified = other.relationNameModified;
            this.relationKeys.inner.Clear();
            foreach (RelationKeyDescriptor relationKey in other.relationKeys.inner)
            {
                this.relationKeys.InnerAdd(relationKey.ParentKeyFieldName, relationKey.ChildKeyFieldName);
            }

            this.headerText = other.headerText;
            this.headerTextModified = other.headerTextModified;
            this.childTableName = other.childTableName;
            this.childTableNameModified = other.childTableNameModified;
            this.mappingName = other.mappingName;
            if (other.ShouldSerializeChildTableDescriptor())
            {
                this.ChildTableDescriptor.InitializeFrom(other.ChildTableDescriptor);
            }
            else
            {
                this.ResetChildTableDescriptor();
            }

            this.parentTableDescriptorFieldsVersion = -1;
            // collection, parentTableDescriptor will be set from parent object/collection
        }

        #endregion

        #region ICloneable

        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Creates a copy of this descriptor and copies also the child table descriptor.
        /// </summary>
        /// <returns>A copy of this descriptor.</returns>
        public virtual RelationDescriptor Clone()
        {
            RelationDescriptor rd = new RelationDescriptor();
            rd.CopyMembersFrom(this);
            return rd;
        }

        #endregion

        #region Object overrides

        /// <summary>Determines if the specified object is equivalent to the current object.</summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if the objects are equivalent; False otherwise.</returns>
        /// <override/>
        public override bool Equals(object obj)
        {
            RelationDescriptor other = obj as RelationDescriptor;
            if (obj == null)
            {
                return this == null;
            }
            else if (other == null)
            {
                return false;
            }
            else
            {
                bool equals = true;
                equals &= this.relationName == other.relationName;
                equals &= this.relationKind == other.relationKind;
                equals &= this.relationKindModified == other.relationKindModified;
                equals &= this.allowCacheChildList == other.allowCacheChildList;
                equals &= this.relationNameModified == other.relationNameModified;
                equals &= this.relationKeys.Equals(other.RelationKeys);
                equals &= this.headerText == other.headerText;
                equals &= this.headerTextModified == other.headerTextModified;
                equals &= this.childTableName == other.childTableName;
                equals &= this.mappingName == other.mappingName;
                equals &= this.ChildTableDescriptor.Equals(other.ChildTableDescriptor);
                return equals;
            }
        }

        /// <summary>Serves as a hash function.</summary>
        /// <returns>Hash code.</returns>
        /// <override/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region Initalize Helpers

        internal PropertyDescriptorCollection itemProperties = null;

        void InitializeName(string name)
        {
            if (!childTableNameModified)
            {
                childTableName = name;
            }

            SyncHeaderAndName();
        }

        void SyncHeaderAndName()
        {
            if (!this.relationNameModified)
            {
                if (childTableName == string.Empty)
                {
                    relationName = mappingName;
                }
                else
                {
                    relationName = childTableName;
                }
            }

            if (!this.headerTextModified)
            {
                headerText = relationName;
            }
        }

        #endregion

        #region Name

        /// <summary>Returns descriptor name.</summary>
        /// <returns>Descriptor name.</returns>
        /// <override/>
        public override string GetName()
        {
            return Name;
        }

        /// <summary>
        /// The name of this relation.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of this relation.")]
        public virtual string Name
        {
            get
            {
                if (relationNameModified)
                {
                    return relationName;
                }

                if (relationName != string.Empty)
                {
                    return relationName;
                }

                if (childTableName != string.Empty)
                {
                    return childTableName;
                }

                return mappingName;
            }

            set
            {
                if (Name != value)
                {
                    string str = relationName;
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                    relationName = value;
                    try
                    {
                        relationNameModified = true;
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
                        if (!ShouldSerializeHeaderText())
                        {
                            headerText = relationName;
                        }
                    }
                    catch
                    {
                        relationName = str;
                        throw;
                    }
                }

                relationName = value;
                relationNameModified = true;
            }
        }

        /// <summary>
        /// Determines if relation name was modified from default.
        /// </summary>
        /// <returns>True if relation name was modified.</returns>
        public bool ShouldSerializeName()
        {
            return relationNameModified;
        }

        /// <summary>
        /// Resets the relation name back to default.
        /// </summary>
        public void ResetName()
        {
            if (relationNameModified)
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("Name"));
                relationNameModified = false;
                SyncHeaderAndName();
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("Name"));
            }
        }

        #endregion

        #region Child Table

        /// <summary>
        /// The TableDescriptor that describes the child table.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [RefreshProperties(RefreshProperties.All)]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        public TableDescriptor ChildTableDescriptor
        {
            get
            {
                if (childTableDescriptor == null)
                {
                    childTableDescriptor = CreateChildTableDescriptor();
                    childTableDescriptor.PropertyChanged += new DescriptorPropertyChangedEventHandler(childTableDescriptor_PropertyChanged);
                    childTableDescriptor.PropertyChanging += new DescriptorPropertyChangedEventHandler(childTableDescriptor_PropertyChanging);
                }

                return childTableDescriptor;
            }

            set
            {
                ChildTableDescriptor.InitializeFrom(value);
            }
        }

        /// <summary>
        /// Determines if the child table descriptor or any of its inner properties was modified.
        /// </summary>
        /// <returns>True if the child table descriptor was modified.</returns>
        public bool ShouldSerializeChildTableDescriptor()
        {
            return IsChildTableDescriptorCreated() && childTableDescriptor.GetModified();
        }

        /// <summary>
        /// Resets the child table descriptor back to its default state.
        /// </summary>
         public void ResetChildTableDescriptor()
        {
            if (IsChildTableDescriptorCreated() && ChildTableDescriptor.GetModified())
            {
                ChildTableDescriptor.ResetTableDescriptor();
            }
        }

        /// <summary>
        /// Determines if an instance for the ChildTableDescriptor has been created.
        /// </summary>
        /// <returns>true if TableDescriptor was created,</returns>
        public bool IsChildTableDescriptorCreated()
        {
            return childTableDescriptor != null;
        }

        /// <summary>
        /// Called to create the <see cref="TableDescriptor"/> for the child table.
        /// </summary>
        /// <returns>A new <see cref="TableDescriptor"/>.</returns>
        public virtual TableDescriptor CreateChildTableDescriptor()
        {
            if (parentTableDescriptor != null)
            {
                Engine engine = this.parentTableDescriptor.Engine;
                if (inSetParentTableDescriptor)
                {
                    throw new InvalidOperationException();
                }

                if (engine != null)
                {
                    childTableDescriptor = engine.CreateTableDescriptor(this);
                    return childTableDescriptor;
                }
            }

            return new TableDescriptor(this);
        }

        #endregion

        #region IStandardValuesProvider Members

        /// <summary>
        /// Returns an array of standard values used by <see cref="StandardValuesCollectionConverter"/>.
        /// </summary>
        /// <param name="propertyDescriptor">The context.PropertyDescriptor of a TypeConverter.GetStandardValues method.</param>
        /// <returns>An array of standard values used by <see cref="StandardValuesCollectionConverter"/>.</returns>
        public ICollection GetStandardValues(PropertyDescriptor propertyDescriptor)
        {
            switch (propertyDescriptor.Name)
            {
                case "ChildTableName":
                    {
                        if (parentTableDescriptor != null)
                        {
                            Engine engine = this.parentTableDescriptor.Engine;
                            ArrayList al = new ArrayList();
                            al.AddRange(engine.SourceListSet.GetStandardValues(propertyDescriptor));
                            ////                        foreach (PropertyDescriptor pd in parentTableDescriptor.ItemProperties)
                            //                        {
                            //                            if (ListUtil.PropertyDescriptorIsARelation(pd))
                            //                            {
                            //                                if (ListUtil.GetRelatedDataRelation(pd) == null)
                            //                                {
                            //                                    if (!al.Contains(pd.Name))
                            //                                        al.Add(pd.Name);
                            //                                }
                            //                            }
                            //                            else
                            //                            {
                            //                                if (!pd.PropertyType.IsPrimitive && !pd.PropertyType.IsInterface)
                            //                                {
                            //                                    if (!al.Contains(pd.Name))
                            //                                        al.Add(pd.Name);
                            //                                }
                            //                            }
                            //                        }
                            return al.ToArray();
                        }

                        break;
                    }

                case "MappingName":
                    {
                        if (parentTableDescriptor != null)
                        {
                            Engine engine = this.parentTableDescriptor.Engine;
                            ArrayList al = new ArrayList();
                            foreach (PropertyDescriptor pd in parentTableDescriptor.ItemProperties)
                            {
                                if (ListUtil.PropertyDescriptorIsARelation(pd))
                                {
                                    al.Add(pd.Name);
                                }
                            }

                            return al.ToArray();
                        }

                        break;
                    }
            }

            return null;
        }
        #endregion

        #region Child Table Name

        /// <summary>
        /// The name of the child table.
        /// </summary>
        [TypeConverter(typeof(StandardValuesCollectionConverter))]
        [RefreshProperties(RefreshProperties.All)]
        [Description("The name of the child table.")]
        public string ChildTableName
        {
            get
            {
                if (this.childTableName == string.Empty && this.mappingName != null)
                {
                    this.SyncNameFromMappingName(false);
                }

                return this.childTableName;
            }

            set
            {
                if (this.childTableName != value)
                {
                    OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ChildTableName"));
                    bool allowName = !this.relationNameModified || ((this.childTableName == string.Empty && this.mappingName == string.Empty) || InPropertyGrid());
                    string strchildTableName = childTableName;
                    string strName = relationName;
                    string strMappingName = mappingName;
                    this.childTableName = value;
                    this.childTableNameModified = true;
                    this.SyncNameFromChildTableName(allowName);
                    try
                    {
                        OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ChildTableName"));
                    }
                    catch
                    {
                        relationName = strName;
                        mappingName = strMappingName;
                        childTableName = strchildTableName;
                        throw;
                    }
                }
            }
        }
        
        bool InPropertyGrid()
        {
            return (this.collection != null && collection.insideCollectionEditor) || (this.parentTableDescriptor != null && parentTableDescriptor.IsDesignTime());
        }

        /// <summary>
        /// Determines if a ChildTableName was specified.
        /// </summary>
        /// <returns>True if the ChilTableName was specified.</returns>
        public bool ShouldSerializeChildTableName()
        {
            return this.childTableNameModified;
        }

        /// <summary>
        /// Resets the name of the child table.
        /// </summary>
        public void ResetChildTableName()
        {
            if (this.ShouldSerializeChildTableName())
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("ChildTableName"));
                this.childTableName = string.Empty;
                this.childTableNameModified = false;
                if (this.ShouldSerializeMappingName())
                {
                    this.SyncNameFromMappingName(false);
                }

                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("ChildTableName"));
            }
        }

        #endregion

        #region Header Text
        /// <summary>
        /// The header text to be displayed in the column header.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [LocalizableAttribute(true)]
        internal string HeaderText
        {
            get
            {
                if (this.childTableName == string.Empty && this.mappingName != null)
                {
                    this.SyncNameFromMappingName(false);
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

        private bool ShouldSerializeHeaderText()
        {
            return headerTextModified && headerText.Length != 0;
        }

        /// <summary>
        /// Resets the header text.
        /// </summary>
        internal void ResetHeaderText()
        {
            if (headerTextModified)
            {
                OnPropertyChanging(new DescriptorPropertyChangedEventArgs("HeaderText"));
                headerTextModified = false;
                SyncHeaderAndName();
                OnPropertyChanged(new DescriptorPropertyChangedEventArgs("HeaderText"));
            }
        }

        #endregion

        private void childTableDescriptor_PropertyChanged(object sender, DescriptorPropertyChangedEventArgs e)
        {
            ////if (e.PropertyName == "PrimaryKeyColumns" || e.PropertyName =="RelationChildColumns")
            //    TraceUtil.TraceCurrentMethodInfo(e);
            this.RaisePropertyChanged("ChildTableDescriptor", e);
        }

        private void childTableDescriptor_PropertyChanging(object sender, DescriptorPropertyChangedEventArgs e)
        {
            ////if (e.PropertyName == "PrimaryKeyColumns" || e.PropertyName =="RelationChildColumns")
            //    TraceUtil.TraceCurrentMethodInfo(e);
            ////if (e.PropertyName != "PrimaryKeyColumns" && e.PropertyName !="RelationChildColumns")
            this.RaisePropertyChanging("ChildTableDescriptor", e);
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            return GetType().Name + " { " + Name + "(" + this.tableId + isdisposed + ") }";
        }

        protected override PropertyDescriptorCollection GetCustomPDC(PropertyDescriptorCollection baseprops)
        {
            PropertyDescriptorCollection pds = base.GetCustomPDC(baseprops);
            ArrayList newpds = new ArrayList();
            foreach (PropertyDescriptor pd in pds)
            {
                if (pd.Name == "RelationKeys")
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

    #region QueryAddRelation
    // eva QueryAddRelation SyncfusionCancel TableDescriptor tableDescriptor RelationDescriptor column

    /// <summary>
    /// Represents a method that handles an event with <see cref="QueryAddRelationEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void QueryAddRelationEventHandler(object sender, QueryAddRelationEventArgs e);

    /// <summary>
    /// The Engine.QueryAddRelation event affects the autopopulation of the RelationDescriptorCollection. <para/>
    /// It is called for each relation that is found in a DataView. You can set e.Cancel = True to avoid specific relations
    /// being added.
    /// </summary>
    public sealed class QueryAddRelationEventArgs : SyncfusionCancelEventArgs
    {
        TableDescriptor tableDescriptor;
        RelationDescriptor column;

        /// <summary>
        /// Initializes the event args
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor.</param>
        /// <param name="column">The relation descriptor.</param>
        public QueryAddRelationEventArgs(TableDescriptor tableDescriptor, RelationDescriptor column)
        {
            this.tableDescriptor = tableDescriptor;
            this.column = column;
        }

        /// <summary>
        /// The TableDescriptor
        /// </summary>
        [TraceProperty(true)]
        public TableDescriptor TableDescriptor
        {
            get
            {
                return tableDescriptor;
            }
        }

        /// <summary>
        /// The Relation
        /// </summary>
        [TraceProperty(true)]
        public RelationDescriptor Relation
        {
            get
            {
                return column;
            }
        }
    }
    #endregion
}
