//-------------------------------------------------------------------------------------------------
// <copyright file="Group.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A group defines a set of records that belong to a category. A group has
    /// multiple sections such as CaptionSection, SummarySection, GroupsDetailsSection,
    /// and RecordsDetailsSection.
    /// </summary>
    /// <remarks>
    /// Groups are created when the records of a table are categorized or when a new record
    /// is inserted. Normally,
    /// the categories are based on the <see cref="TableDescriptor.GroupedColumns"/> but
    /// programmers can also provide their own categorization routines by implementing
    /// a <see cref="SortColumnDescriptor.Comparer"/>
    /// for a <see cref="SortColumnDescriptor"/>. Before groups are categorized, the records are
    /// sorted in the order as specified by <see cref="TableDescriptor.GroupedColumns"/>. After the
    /// records are sorted, the <see cref="Table"/> object loops through all records to determine
    /// the categories records belong to.
    /// <para/>
    /// Another collection in the TableDescriptor that defines categorization is the
    /// <see cref="TableDescriptor.RelationChildColumns"/> collection. RelationChildColumns will be
    /// added when there is a parent-child relation between two tables. The child table of such a relation must be
    /// sorted by the columns that are used to identify a record. These columns match the foreign key
    /// columns of the parent table. For every new category key with regards to RelationChildColumns
    /// a <see cref="ChildTable"/> is created. The ChildTable class is derived from the <see cref="Group"/>.
    /// <para/>
    /// A group can either be a final node with records or it can be a node with nested groups. If a
    /// group has records, its <see cref="Groups"/> collection will be empty and the <see cref="Records"/>
    /// collection will contain all records. If a group has nested groups, its <see cref="Groups"/>
    /// collection will have the nested groups and the <see cref="Records"/> collection will be empty.
    /// <para/>
    /// A group can be expanded and collapsed with its <see cref="IsExpanded"/> property. Expansion
    /// of groups will show or hide nested elements of the group within the <see cref="Table.DisplayElements"/>
    /// collection.
    /// <para/>
    /// A table has at least one group. The <see cref="Table.TopLevelGroup"/> is a <see cref="ChildTable"/>
    /// which is derived from <see cref="Group"/>.
    /// <para/>
    /// The <see cref="Element.GroupLevel"/> property will return the group level how deep the group is nested.
    /// </remarks>
    public class Group : Element, IContainerElement
    {
        object[] categoryKeys;
        GroupCategoryTreeTableEntry groupCategoryEntry;
        GroupSortOrderTreeTableEntry groupSortOrderEntry;
        internal SectionsTreeTable sectionEntries;
        SectionInGroupCollection _sections = null;
        int visibleCount = -1;
        double yAmountCount = -1;
        int visibleItemCount = -1;
        double visibleCustomCount = -1;
        internal string oldCaptionText = string.Empty;
        internal object[] categoryForeignKeyParentIds;
        private object passThroughItem;
        private IList sourceList;
        private UnsortedRecordsTreeEntry unsortedEntry;

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public string OldCaptionText
        {
            get
            {
                return oldCaptionText;
            }

            set
            {
                oldCaptionText = value;
            }
        }

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int CachedVisibleCount
        {
            get
            {
                return this.visibleCount;
            }

            set
            {
                this.visibleCount = value;
            }
        }

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public double CachedYamountCount
        {
            get
            {
                return yAmountCount;
            }

            set
            {
                this.yAmountCount = value;
            }
        }

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int CachedVisibleItemCount
        {
            get
            {
                return this.visibleItemCount;
            }

            set
            {
                this.visibleItemCount = value;
            }
        }

        /// <summary>Only for internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public double CachedVisibleCustomCount
        {
            get
            {
                return visibleCustomCount;
            }

            set
            {
                this.visibleCustomCount = value;
            }
        }

        bool isExpanded
        {
            get
            {
                return Reserved1;
            }

            set
            {
                Reserved1 = value;
            }
        }

        bool summaryDirty
        {
            get
            {
                return Reserved2;
            }

            set
            {
                Reserved2 = value;
            }
        }

        // Cached child elements
        int captionIndex
        {
            get
            {
                // Max possible value: 15 !!!
                return Reserved16a - 1;
            }

            set
            {
                Reserved16a = value + 1;
            }
        }

        int detailsIndex
        {
            get
            {
                return Reserved16b - 1;
            }

            set
            {
                Reserved16b = value + 1;
            }
        }

        int summaryIndex
        {
            get
            {
                return Reserved16c - 1;
            }

            set
            {
                Reserved16c = value + 1;
            }
        }
        
        /// <summary>
        /// Initializes a new group and assigns the parent.
        /// </summary>
        /// <param name="parent">The parent element this object belongs to.</param>
        public Group(Element parent)
            : base(parent)
        {
            sectionEntries = new SectionsTreeTable(this);
            summaryDirty = true;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IBindingList bindingList = sourceList as IBindingList;
                if (bindingList != null)
                {
                    bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
                }

                if (sectionEntries != null)
                {
                    sectionEntries.Dispose();
                }

                detailsIndex = -1;
                captionIndex = -1;
                visibleCustomCount = -1;
            }

            base.Dispose(disposing);
        }

        #region SupportsId
        /// <summary>Specifies whether this object can be uniquely idenfied with the id.</summary>
        /// <returns>returns True.</returns>
        /// <override/>
        public override bool SupportsId()
        {
            return true;
        }

        int id;

        /// <summary>Gets group id.</summary>
        /// <override/>
        public override int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
        #endregion

        /// <summary>
        /// Determines the <see cref="AddNewRecord"/> for the group within the <see cref="AddNewRecordSection"/>.
        /// </summary>
        /// <returns>A reference to the <see cref="AddNewRecord"/> or NULL if group has not such a record.</returns>
        public Record FindAddNewRecord()
        {
            AddNewRecordSection addNewSection = null;
            foreach (Section section in Sections)
            {
                if (section is AddNewRecordSection)
                {
                    addNewSection = (AddNewRecordSection)section;
                    if (IsChildVisible(section))
                    {
                        return (AddNewRecord)addNewSection.Records[0];
                    }
                }
            }

            if (addNewSection != null)
            {
                return (AddNewRecord)addNewSection.Records[0];
            }

            return null;
        }

        void ResetCounterFields()
        {
            detailsIndex = -1;
            captionIndex = -1;
            summaryIndex = -1;
            visibleCustomCount = -1;
            ////_addNewRecord = null;

            ///// Cached counters and summaries
            visibleItemCount = -1;
            //// NOTE: ResetCounterFields might be called from Table.EnsureInitialized
            //// while GetVisibleCount was called. It is important that GetVisibleCount()
            //// therefore uses a local variable for calculating visible count.
#if DIAGNOSING
if (this.inGetVisibleCount)
            Debugger.Break();
#endif
            visibleCount = -1;
            yAmountCount = -1;
        }

        /// <summary>
        /// Determines if this is the root group of the table. A TopLevelGroup is the root group.
        /// </summary>
        public bool IsMainGroup
        {
            get
            {
                return this.ParentElement is Table;
            }
        }
        
        /// <summary>
        /// Initializes the sections for a new group.
        /// </summary>
        /// <param name="hasRecords">Specifies if group will be filled with records or nested groups.</param>
        /// <param name="sortColumns">The sortColumns that define the category of the group.</param>
        /// <param name="isExpanded">The initial expansion state for the group.</param>
        public void InitializeDetails(bool hasRecords, SortColumnDescriptorCollection sortColumns, bool isExpanded)
        {
            this.isExpanded = isExpanded;

            BeginInit();
            OnInitializeSections(hasRecords, sortColumns);
            EndInit();
        }

        /// <summary>
        /// Called from <see cref="InitializeDetails"/> to initialize the sections for a new group. Override
        /// this method if you want to customize the sections that are available for a group.
        /// </summary>
        /// <param name="hasRecords">Specifies if group will be filled with records or nested groups.</param>
        /// <param name="sortColumns">The sortColumns that define the category of the group.</param>
        /// <example>
        /// <code lang="C#">
        ///         protected override void OnInitializeSections(bool hasRecords, SortColumnDescriptorCollection sortColumns)
        ///         {
        ///             this.Sections.Add(this.ParentTableDescriptor.CreateCaptionSection(this));
        ///             if (this.IsTopLevelGroup)
        ///                 this.Sections.Add(this.ParentTableDescriptor.CreateAddNewRecordSection(this));
        ///             if (hasRecords)
        ///                 this.Sections.Add(this.ParentTableDescriptor.CreateRecordsDetails(this, sortColumns));
        ///             else
        ///                 this.Sections.Add(this.ParentTableDescriptor.CreateGroupsDetails(this, sortColumns));
        ///             this.Sections.Add(this.ParentTableDescriptor.CreateSummarySection(this));
        ///         }
        /// </code>
        /// </example>
        protected virtual void OnInitializeSections(bool hasRecords, SortColumnDescriptorCollection sortColumns)
        {
            this.Sections.Add(this.ParentTableDescriptor.CreateCaptionSection(this));
            if (this.IsMainGroup)
            {
                this.Sections.Add(this.ParentTableDescriptor.CreateAddNewRecordSection(this));
            }

            if (hasRecords)
            {
                this.Sections.Add(this.ParentTableDescriptor.CreateRecordsDetails(this, sortColumns));
            }
            else
            {
                this.Sections.Add(this.ParentTableDescriptor.CreateGroupsDetails(this, sortColumns));
            }

            this.Sections.Add(this.ParentTableDescriptor.CreateSummarySection(this));
        }

        /// <summary>
        /// Returns the columns that define the category of the group.
        /// </summary>
        public SortColumnDescriptorCollection CategoryColumns
        {
            get
            {
                Group g = ParentGroup;
                if (g != null)
                {
                    return ((GroupsDetails)g.Details).columnDescriptors;
                }

                return SortColumnDescriptorCollection.Empty;
            }
        }
        
        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }
        
        /// <summary>
        /// The name of the column by which the group is categorized (see <see cref="TableDescriptor.GroupedColumns"/>).
        /// </summary>
        public virtual string Name
        {
            get
            {
                if (GroupLevel != -1 && ParentElement is GroupsDetails && ParentTable.ParentTableDescriptor.GroupedColumns.Count > 0)
                {
                    return ParentTable.ParentTableDescriptor.GroupedColumns[GroupLevel].Name;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Begins initialization of sections.
        /// </summary>
        public void BeginInit()
        {
            SectionEntries.BeginInit();
        }

        /// <summary>
        /// Ends initialization of sections.
        /// </summary>
        public void EndInit()
        {
            SectionEntries.EndInit();
        }

        /// <summary>
        /// Searches for the first record in the group. If the group has nested groups, they
        /// will be recursively searched.
        /// </summary>
        /// <returns>The first record in the group; NULL if group is empty.</returns>
        public Record GetFirstRecord()
        {
            DetailsSection details = Details;
            while (!details.HasRecords)
            {
                if (((GroupsDetails)details).Groups.Count == 0)
                {
                    return null;
                }

                Group g = ((GroupsDetails)details).Groups[0];

                if (ParentTable.IsPassThroughGrouping)
                {
                    ParentTable.PopulatePassThroughGroupIfEmpty(g);
                }

                details = g.Details;
                if (details == null)
                {
                    return null;
                }
            }

            RecordsDetails rd = (RecordsDetails)details;
            if (rd.Records.Count > 0)
            {
                return rd.Records[0];
            }

            return null;
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            ////this.OnEnsureInitialized(this);
            return sectionEntries;
        }

        /// <summary>Returns the child count for a given group.</summary>
        /// <returns>Child count.</returns>
        /// <override/>
        public override int GetChildCount()
        {
            if (ParentTable.PassThroughGroupingResult != null)
            {
                return ParentTable.PassThroughGroupingResult.GetItemCount(this, passThroughItem);
            }

            return this.Details.GetChildCount();
        }

        /// <summary>
        /// Expands this group and all nested groups.
        /// </summary>
        public void ExpandAllGroups()
        {
            ExpandAllGroups(true);
        }

        /// <summary>
        /// Expands this group and all nested groups and only optionally raises the <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void ExpandAllGroups(bool raiseDisplayElementChangeEvents)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            ExpandAllGroups(false, false);
            InvalidateCounterTopDown(true);
            InvalidateCounterBottomUp();
            ParentTable.ClearCollectionCaches();
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        /// <summary>
        /// Expands this group and all nested groups and only optionally raises the <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events and only optionally invalidates counters.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">True if events should be raised.</param>
        /// <param name="refreshCounters">True if counters should be invalidated.</param>
        public void ExpandAllGroups(bool raiseDisplayElementChangeEvents, bool refreshCounters)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            foreach (Group g in Groups)
            {
                g.ExpandAllGroups(false, refreshCounters);
                g.SetExpanded(true, refreshCounters, false);
            }

            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        /// <summary>
        /// Expands all records in this group and all nested groups and only optionally raises the <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void ExpandAllRecords(bool raiseDisplayElementChangeEvents)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            ExpandAllRecords(false, false);
            InvalidateCounterTopDown(true);
            InvalidateCounterBottomUp();

            ParentTable.ClearCollectionCaches();
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        /// <summary>
        /// Expands all records in this group and all nested groups and only optionally raises the <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events and only optionally invalidates counters.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">Specifies if DispalyElementChanged events should be raised.</param>
        /// <param name="refreshCounters">Specifies if the counters should be invalidated.</param>
        public void ExpandAllRecords(bool raiseDisplayElementChangeEvents, bool refreshCounters)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            foreach (Group g in Groups)
            {
                g.SetExpanded(true, refreshCounters, false);
                g.ExpandAllRecords(false, refreshCounters);
            }

            foreach (Record r in Records)
            {
                r.SetExpanded(true, refreshCounters, false);
                foreach (NestedTable nestedTable in r.NestedTables)
                {
                    ChildTable childTable = nestedTable.ChildTable;
                    if (childTable != null)
                    {
                        childTable.SetExpanded(true, refreshCounters, false);
                        childTable.ExpandAllRecords(false, refreshCounters);
                        childTable.InvalidateCounterBottomUp();
                    }
                }
            }

            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        /// <summary>
        /// Collapses this group and all nested groups.
        /// </summary>
        public void CollapseAllGroups()
        {
            CollapseAllGroups(true);
        }

        /// <summary>
        /// Collapses this group and all nested groups and only optionally raises the <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void CollapseAllGroups(bool raiseDisplayElementChangeEvents)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            foreach (Group g in Groups)
            {
                g.CollapseAllGroups(false);
                g.IsExpanded = false;
            }

            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        /// <summary>
        /// Collapses all records in this group and all nested groups.
        /// </summary>
        public void CollapseAllRecords()
        {
            CollapseAllRecords(true);
        }

        /// <summary>
        /// Collapses all records in this group and all nested groups and only optionally raises <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events.
        /// </summary>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void CollapseAllRecords(bool raiseDisplayElementChangeEvents)
        {
            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanging(ParentTable, -1, -1, true, true, false);
            }

            foreach (Group g in Groups)
            {
                g.CollapseAllRecords(false);
                g.SetExpanded(false, true, false);
            }

            foreach (Record r in Records)
            {
                r.SetExpanded(false, true, false);
                foreach (NestedTable nestedTable in r.NestedTables)
                {
                    ChildTable childTable = nestedTable.ChildTable;
                    if (childTable != null)
                    {
                        childTable.CollapseAllRecords(false);
                        childTable.SetExpanded(false, false, false);
                    }
                }
            }

            if (raiseDisplayElementChangeEvents)
            {
                EngineTable.RaiseDisplayElementChanged(ParentTable, -1, -1, true, true, false);
            }
        }

        void EnsureIndexes()
        {
            if (captionIndex == -1 || detailsIndex == -1)
            {
                int count = Sections.InnerCount;
                for (int n = 0; n < count; n++)
                {
                    Section section = Sections.GetInnerItem(n);
                    if (section is CaptionSection)
                    {
                        captionIndex = n;
                    }
                    else if (section is DetailsSection && !(section is AddNewRecordSection))
                    {
                        detailsIndex = n;
                    }
                    else if (section is ISummarySection)
                    {
                        summaryIndex = n;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="CaptionSection"/> of this group.
        /// </summary>
        public CaptionSection Caption
        {
            get
            {
                EnsureIndexes();
                if (captionIndex != -1)
                {
                    return (CaptionSection)Sections.GetInnerItem(captionIndex);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="ISummarySection"/> of this group.
        /// </summary>
        public ISummarySection Summary
        {
            get
            {
                EnsureIndexes();
                if (summaryIndex != -1)
                {
                    return (ISummarySection)Sections.GetInnerItem(summaryIndex);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="DetailsSection"/> of this group.
        /// </summary>
        public DetailsSection Details
        {
            get
            {
                EnsureIndexes();
                if (detailsIndex != -1)
                {
                    return (DetailsSection)Sections.GetInnerItem(detailsIndex);
                }

                EnsureInitialized(this, false);
                EnsureIndexes();
                if (detailsIndex != -1)
                {
                    return (DetailsSection)Sections.GetInnerItem(detailsIndex);
                }

                return null;
            }
        }

        ////        public AddNewRecord AddNewRecord
        ////        {
        ////            get
        ////            {
        ////                if (_addNewRecord == null)
        ////                {
        ////                    foreach (Section section in Sections)
        ////                    {
        ////                        if (section is AddNewRecordSection && this.IsChildVisible(section))
        ////                        {
        ////                            _addNewRecord = (AddNewRecord) ((AddNewRecordSection) section).Records[0];
        ////                            break;
        ////                        }
        ////                    }
        ////                }
        ////                return _addNewRecord;
        ////            }
        ////            set
        ////            {
        ////                _addNewRecord = value;
        ////            }
        ////        }

        internal SectionsTreeTable SectionEntries
        {
            get
            {
                return sectionEntries;
            }
        }

        /// <summary>
        /// ShortCut for ((GroupsDetails) Details).Groups. A group can either be a final node with records or it can be a node with nested groups. If a
        /// group has records, its <see cref="Groups"/> collection will be empty and the <see cref="Records"/>
        /// collection will contain all records. If a group has nested groups, its <see cref="Groups"/>
        /// collection will have the nested groups and the <see cref="Records"/> collection will be empty.
        /// </summary>
        public GroupsInDetailsCollection Groups
        {
            get
            {
                if (Details.HasRecords)
                {
                    return GroupsInDetailsCollection.Empty;
                }
                else
                {
                    return ((GroupsDetails)Details).Groups;
                }
            }
        }

        /// <summary>
        /// Returns a flattened collection of records that belong to this group and nested groups.
        /// </summary>
        public FlattenedRecordsInGroupCollection FlattenedRecords
        {
            get
            {
                FlattenedRecordsInGroupCollection coll = ParentTable.flattenedRecordsGroupCollection.Target as FlattenedRecordsInGroupCollection;
                Group g = ParentTable.flattenedRecordsGroup.Target as Group;
                if (coll != null && g == this)
                {
                    return coll;
                }

                coll = new FlattenedRecordsInGroupCollection(this);
                ParentTable.flattenedRecordsGroupCollection = new WeakReference(coll);
                ParentTable.flattenedRecordsGroup = new WeakReference(this);
                return coll;
            }
        }

        /// <summary>
        /// Returns a flattened collection of filtered records that belong to this group and nested groups.
        /// </summary>
        public FlattenedFilteredRecordsInGroupCollection FlattenedFilteredRecords
        {
            get
            {
                FlattenedFilteredRecordsInGroupCollection coll = ParentTable.flattenedFilteredRecordsGroupCollection.Target as FlattenedFilteredRecordsInGroupCollection;
                Group g = ParentTable.flattenedFilteredRecordsGroup.Target as Group;
                if (coll != null && g == this)
                {
                    return coll;
                }

                coll = new FlattenedFilteredRecordsInGroupCollection(this);
                ParentTable.flattenedFilteredRecordsGroupCollection = new WeakReference(coll);
                ParentTable.flattenedFilteredRecordsGroup = new WeakReference(this);
                return coll;
            }
        }

        /// <summary>
        /// Returns a flattened collection of filtered records that belong to this group and nested groups.
        /// </summary>
        public GroupTypedListRecordsCollection GroupTypedListRecords
        {
            get
            {
                GroupTypedListRecordsCollection coll = ParentTable.groupTypedListRecordsCollection.Target as GroupTypedListRecordsCollection;
                Group g = ParentTable.groupTypedListRecordsGroup.Target as Group;
                if (coll != null && g == this)
                {
                    return coll;
                }

                coll = CreateGroupTypedListRecordsCollection();
                ParentTable.groupTypedListRecordsCollection = new WeakReference(coll);
                ParentTable.groupTypedListRecordsGroup = new WeakReference(this);
                return coll;
            }
        }

        /// <summary>
        /// Creates the group typed list records collection.
        /// </summary>
        /// <returns>returns GroupTypedListRecordsCollection</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual GroupTypedListRecordsCollection CreateGroupTypedListRecordsCollection()
        {
            return new GroupTypedListRecordsCollection(this);
        }

        /// <summary>
        /// ShortCut for ((RecordsDetails) Details).Records. This returns all records in the group including records that
        /// do not meet filter criteria.
        /// </summary>
        public RecordsInDetailsCollection Records
        {
            get
            {
                if (Details.HasRecords)
                {
                    return ((RecordsDetails)Details).Records;
                }
                else
                {
                    return RecordsInDetailsCollection.Empty;
                }
            }
        }

        /// <summary>
        /// ShortCut for ((RecordsDetails) Details).FilteredRecords. This returns only records in the group that
        /// meet filter criteria.
        /// </summary>
        public FilteredRecordsInDetailsCollection FilteredRecords
        {
            get
            {
                if (Details.HasRecords)
                {
                    return ((RecordsDetails)Details).FilteredRecords;
                }
                else               
                {   
                    return FilteredRecordsInDetailsCollection.Empty; 
                }
            }
        }

        /// <summary>
        /// Returns all sections in the group.
        /// </summary>
        public SectionInGroupCollection Sections
        {
            get
            {
                if (_sections == null)
                {
                    _sections = new SectionInGroupCollection(this);
                }

                return _sections;
            }
        }

        /// <summary>Gets summary information for this element and child elements.</summary>
        /// <param name="parentTable">A reference to the parent table.</param>
        /// <param name="summaryChanged">True if changes were detected.</param>
        /// <returns>Array of summaries.</returns>
        /// <override/>
        public override ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            parentTable.TableDescriptor.EnsureSummaryDescriptors();
            summaryDirty = false;
            if (parentTable.TableDescriptor.Summaries.Count == 0)
            {
                summaryChanged = false;
                return parentTable.GetEmptySummaries();
            }

            return base.GetSummaries(parentTable, out summaryChanged);
        }

        /// <overload>
        /// Returns summary information for this group.
        /// </overload>
        /// <summary>
        /// Returns a summary at the specified index.
        /// </summary>
        /// <param name="indexOfSummaryDescriptor">The index of the summary in the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <returns>An object that implements the <see cref="ITreeTableSummary"/>. You should cast the object to the
        /// correct runtime type depending on the <see cref="SummaryRowDescriptor.SummaryType"/>, e.g. <see cref="DoubleAggregateSummary"/> in case of a
        /// <see cref="SummaryType"/>.<see cref="SummaryType.DoubleAggregate"/>
        /// </returns>
        /// <example>
        /// <code lang="C#">
        ///       string GetAverageSummary(SummaryDescriptor summaryDescriptor, Group group)
        ///         {
        ///             Table table = group.ParentTable;
        ///             TableDescriptor td = table.TableDescriptor;
        ///             string summaryText = "";
        /// <para/> 
        ///             bool use31Code = true;
        ///             if (use31Code)
        ///             {
        ///                 // Option 1: Strong typed access to DoubleAggregateSummary.
        ///                 DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummary(summaryDescriptor);
        ///                 summaryText = string.Format("{0:c}", summary1.Average);
        ///  <para/> 
        ///                 // or Option 2: Use reflection to get "Average" property of summary
        ///                 summaryText = string.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"));
        ///  <para/> 
        ///                 // or Option 3: Use reflection to get "Average" property of summary and format it
        ///                 summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}");
        ///             }
        ///  <para/> 
        ///             else
        ///             {
        ///                 // This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///                 if (summaryDescriptor != null)
        ///                 {
        ///                     int indexOfSd1 = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor);
        ///  <para/> 
        ///                     // strong typed - you have to cast to DoubleAggregateSummary.
        ///  <para/> 
        ///                     DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummaries(table)[indexOfSd1];
        ///                     summaryText = string.Format("{0:c}", summary1.Average);
        ///                 }
        ///             }
        ///             return summaryText;
        ///         }
        /// </code>
        /// <code lang="VB">
        ///  <para/> 
        ///     Function GetAverageSummary(ByVal summaryDescriptor As SummaryDescriptor, ByVal group As Group) As String
        ///         Dim table As Table = group.ParentTable
        ///         Dim td As TableDescriptor = table.TableDescriptor
        ///         Dim summaryText As String = ""
        ///  <para/> 
        ///         Dim use31Code As Boolean = True
        ///         If use31Code Then
        ///             ' Option 1: Strong typed access to DoubleAggregateSummary.
        ///             Dim summary1 As DoubleAggregateSummary = CType(group.GetSummary(summaryDescriptor), DoubleAggregateSummary)
        ///             summaryText = String.Format("{0:c}", summary1.Average)
        ///  <para/> 
        ///             ' or Option 2: Use reflection to get "Average" property of summary
        ///             summaryText = String.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"))
        /// <para/>  
        ///             ' or Option 3: Use reflection to get "Average" property of summary and format it
        ///             summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}")
        ///  <para/> 
        ///         Else
        ///             ' This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///             If Not (summaryDescriptor Is Nothing) Then
        ///                 Dim indexOfSd1 As Integer = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor)
        /// <para/>  
        ///                 ' strong typed - you have to cast to DoubleAggregateSummary.
        ///                 Dim summary1 As DoubleAggregateSummary = CType(group.GetSummaries(table)(indexOfSd1), DoubleAggregateSummary)
        ///                 summaryText = String.Format("{0:c}", summary1.Average)
        ///             End If
        ///         End If
        ///         Return summaryText
        ///     End Function 'GetSummary
        ///  <para/> 
        /// </code>
        ///  <para/> 
        /// </example>
        /// <seealso cref="Syncfusion.Grouping.TableDescriptor.Summaries"/>
        /// <seealso cref="GetSummaryProperty"/>
        public ITreeTableSummary GetSummary(int indexOfSummaryDescriptor)
        {
            if (indexOfSummaryDescriptor == -1)
            {
                return null;
            }

            Table parentTable = this.ParentTable;
            summaryDirty = false;
            bool summaryChanged;
            ITreeTableSummary[] summaries = TreeEntries.GetSummaries(parentTable.tableEmptySummaries, out summaryChanged);
            if (summaries == null)
            {
                summaries = parentTable.GetEmptySummaries();
            }

            return summaries[indexOfSummaryDescriptor];
        }

        /// <summary>
        /// Returns the value of the specified summary.
        /// </summary>
        /// <param name="sd">A summary of the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <returns>An object that implements the <see cref="ITreeTableSummary"/>. You should cast the object to the
        /// correct runtime type depending on the <see cref="SummaryRowDescriptor.SummaryType"/>, e.g. <see cref="DoubleAggregateSummary"/> in case of a
        /// <see cref="SummaryType"/>.<see cref="SummaryType.DoubleAggregate"/>
        /// </returns>
        /// <genoverload/>
        public ITreeTableSummary GetSummary(SummaryDescriptor sd)
        {
            return GetSummary(sd.Name);
        }

        /// <summary>
        /// Returns the value of the summary with the specified name.
        /// </summary>
        /// <param name="summaryDescriptorName">A summary in the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <returns>An object that implements the <see cref="ITreeTableSummary"/>. You should cast the object to the
        /// correct runtime type depending on the <see cref="SummaryRowDescriptor.SummaryType"/>, e.g. <see cref="DoubleAggregateSummary"/> in case of a
        /// <see cref="SummaryType"/>.<see cref="SummaryType.DoubleAggregate"/>
        /// </returns>
        /// <genoverload/>
        public ITreeTableSummary GetSummary(string summaryDescriptorName)
        {
            Table parentTable = this.ParentTable;
            TableDescriptor td = parentTable.TableDescriptor;
            td.EnsureSummaryDescriptors();
            summaryDirty = false;
            bool summaryChanged;
            ITreeTableSummary[] summaries = TreeEntries.GetSummaries(parentTable.tableEmptySummaries, out summaryChanged);
            if (summaries == null)
            {
                summaries = parentTable.GetEmptySummaries();
            }

            int indexOfSummary = td.Summaries.IndexOf(summaryDescriptorName);
            if (indexOfSummary == -1)
            {
                return null;
            }

            return summaries[indexOfSummary];
        }

        /// <overload>
        /// Returns the value of the specified summary.
        /// </overload>
        /// <summary>
        /// Returns the value of the specified summary.
        /// </summary>
        /// <param name="sd">A summary of the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <param name="propertyName">The property in the summary. The value will be determined with reflection and converted to a string.</param>
        /// <returns>The specified property as string.</returns>
        /// <example>
        /// <code lang="C#">
        ///       string GetAverageSummary(SummaryDescriptor summaryDescriptor, Group group)
        ///         {
        ///             Table table = group.ParentTable;
        ///             TableDescriptor td = table.TableDescriptor;
        ///             string summaryText = "";
        /// <para/>  
        ///             bool use31Code = true;
        ///             if (use31Code)
        ///             {
        ///                 // Option 1: Strong typed access to DoubleAggregateSummary.
        ///                 DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummary(summaryDescriptor);
        ///                 summaryText = string.Format("{0:c}", summary1.Average);
        /// <para/>  
        ///                 // or Option 2: Use reflection to get "Average" property of summary
        ///                 summaryText = string.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"));
        /// <para/>  
        ///                 // or Option 3: Use reflection to get "Average" property of summary and format it
        ///                 summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}");
        ///             }
        /// <para/>  
        ///             else
        ///             {
        ///                 // This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///                 if (summaryDescriptor != null)
        ///                 {
        ///                     int indexOfSd1 = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor);
        ///  <para/> 
        ///                     // strong typed - you have to cast to DoubleAggregateSummary.
        ///  <para/> 
        ///                     DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummaries(table)[indexOfSd1];
        ///                     summaryText = string.Format("{0:c}", summary1.Average);
        ///                 }
        ///             }
        ///             return summaryText;
        ///         }
        /// </code>
        /// <code lang="VB">
        ///  <para/> 
        ///     Function GetAverageSummary(ByVal summaryDescriptor As SummaryDescriptor, ByVal group As Group) As String
        ///         Dim table As Table = group.ParentTable
        ///         Dim td As TableDescriptor = table.TableDescriptor
        ///         Dim summaryText As String = ""
        ///  <para/> 
        ///         Dim use31Code As Boolean = True
        ///         If use31Code Then
        ///             ' Option 1: Strong typed access to DoubleAggregateSummary.
        ///             Dim summary1 As DoubleAggregateSummary = CType(group.GetSummary(summaryDescriptor), DoubleAggregateSummary)
        ///             summaryText = String.Format("{0:c}", summary1.Average)
        ///  <para/> 
        ///             ' or Option 2: Use reflection to get "Average" property of summary
        ///             summaryText = String.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"))
        ///  <para/> 
        ///             ' or Option 3: Use reflection to get "Average" property of summary and format it
        ///             summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}")
        ///  <para/> 
        ///         Else
        ///             ' This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///             If Not (summaryDescriptor Is Nothing) Then
        ///                 Dim indexOfSd1 As Integer = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor)
        /// <para/>  
        ///                 ' strong typed - you have to cast to DoubleAggregateSummary.
        ///                 Dim summary1 As DoubleAggregateSummary = CType(group.GetSummaries(table)(indexOfSd1), DoubleAggregateSummary)
        ///                 summaryText = String.Format("{0:c}", summary1.Average)
        ///             End If
        ///         End If
        ///         Return summaryText
        ///     End Function 'GetSummary
        ///  <para/> 
        /// </code>
        ///  <para/> 
        /// </example>
        /// <seealso cref="Syncfusion.Grouping.TableDescriptor.Summaries"/>
        /// <seealso cref="GetSummary"/>
        public object GetSummaryProperty(SummaryDescriptor sd, string propertyName)
        {
            return GetSummaryProperty(sd.Name, propertyName);
        }

        /// <overload>
        /// Returns the value of the summary with the specified name.
        /// </overload>
        /// <summary>
        /// Returns the value of the summary with the specified name.
        /// </summary>
        /// <param name="summaryDescriptorName">A summary of the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <param name="propertyName">The property in the summary. The value will be determined with reflection and converted to a string.</param>
        /// <returns>The specified property.</returns>
        /// <example>
        /// <code lang="C#">
        ///       string GetAverageSummary(SummaryDescriptor summaryDescriptor, Group group)
        ///         {
        ///             Table table = group.ParentTable;
        ///             TableDescriptor td = table.TableDescriptor;
        ///             string summaryText = "";
        /// <para/>  
        ///             bool use31Code = true;
        ///             if (use31Code)
        ///             {
        ///                 // Option 1: Strong typed access to DoubleAggregateSummary.
        ///                 DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummary(summaryDescriptor);
        ///                 summaryText = string.Format("{0:c}", summary1.Average);
        /// <para/>  
        ///                 // or Option 2: Use reflection to get "Average" property of summary
        ///                 summaryText = string.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"));
        /// <para/>  
        ///                 // or Option 3: Use reflection to get "Average" property of summary and format it
        ///                 summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}");
        ///             }
        ///  <para/> 
        ///             else
        ///             {
        ///                 // This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///                 if (summaryDescriptor != null)
        ///                 {
        ///                     int indexOfSd1 = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor);
        ///  <para/> 
        ///                     // strong typed - you have to cast to DoubleAggregateSummary.
        /// <para/>  
        ///                     DoubleAggregateSummary summary1 = (DoubleAggregateSummary) group.GetSummaries(table)[indexOfSd1];
        ///                     summaryText = string.Format("{0:c}", summary1.Average);
        ///                 }
        ///             }
        ///             return summaryText;
        ///         }
        /// </code>
        /// <code lang="VB">
        ///  <para/> 
        ///     Function GetAverageSummary(ByVal summaryDescriptor As SummaryDescriptor, ByVal group As Group) As String
        ///         Dim table As Table = group.ParentTable
        ///         Dim td As TableDescriptor = table.TableDescriptor
        ///         Dim summaryText As String = ""
        ///  <para/> 
        ///         Dim use31Code As Boolean = True
        ///         If use31Code Then
        ///             ' Option 1: Strong typed access to DoubleAggregateSummary.
        ///             Dim summary1 As DoubleAggregateSummary = CType(group.GetSummary(summaryDescriptor), DoubleAggregateSummary)
        ///             summaryText = String.Format("{0:c}", summary1.Average)
        ///  <para/> 
        ///             ' or Option 2: Use reflection to get "Average" property of summary
        ///             summaryText = String.Format("{0:c}", group.GetSummaryProperty(summaryDescriptor, "Average"))
        ///  <para/> 
        ///             ' or Option 3: Use reflection to get "Average" property of summary and format it
        ///             summaryText = group.GetFormattedSummaryProperty(summaryDescriptor, "Average", "{0:c}")
        ///  <para/> 
        ///         Else
        ///             ' This is the code you had to use in version 3.0 and earlier (still working but bit more complicate)
        ///             If Not (summaryDescriptor Is Nothing) Then
        ///                 Dim indexOfSd1 As Integer = table.TableDescriptor.Summaries.IndexOf(summaryDescriptor)
        ///  <para/> 
        ///                 ' strong typed - you have to cast to DoubleAggregateSummary.
        ///                 Dim summary1 As DoubleAggregateSummary = CType(group.GetSummaries(table)(indexOfSd1), DoubleAggregateSummary)
        ///                 summaryText = String.Format("{0:c}", summary1.Average)
        ///             End If
        ///         End If
        ///         Return summaryText
        ///     End Function 'GetSummary
        ///  <para/> 
        /// </code>
        ///  <para/> 
        /// </example>
        /// <seealso cref="Syncfusion.Grouping.TableDescriptor.Summaries"/>
        /// <seealso cref="GetSummary"/>
        public object GetSummaryProperty(string summaryDescriptorName, string propertyName)
        {
            ITreeTableSummary summary = this.GetSummary(summaryDescriptorName);

            if (summary == null)
            {
                return null;
            }

            PropertyDescriptor pd = TypeDescriptor.GetProperties(summary.GetType())[propertyName];
            return pd.GetValue(summary);
        }

        /// <summary>
        /// Determines the value of the summary with the specified name and formats its output.
        /// </summary>
        /// <param name="sd">A summary of the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <param name="propertyName">The property in the summary. The value will be determined with reflection and converted to a string.</param>
        /// <param name="format">The format to be used in <see cref="System.String"/>.<see cref="System.String.Format"/> for formmatting the result, e.g. "{0:c}". See also http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconformattingtypes.asp</param>
        /// <returns>The specified property as formatted string.</returns>
        /// <genoverload/>
        public string GetFormattedSummaryProperty(SummaryDescriptor sd, string propertyName, string format)
        {
            return GetFormattedSummaryProperty(sd.Name, propertyName, format);
        }

        /// <summary>
        /// Determines the value of the summary with the specified name and formats its output.
        /// </summary>
        /// <param name="summaryDescriptorName">A summary of the <see cref="Syncfusion.Grouping.TableDescriptor"/>.<see cref="Syncfusion.Grouping.TableDescriptor.Summaries"/> collection.</param>
        /// <param name="propertyName">The property in the summary. The value will be determined with reflection and converted to a string.</param>
        /// <param name="format">The format to be used in <see cref="System.String"/>.<see cref="System.String.Format"/> for formmatting the result, e.g. "{0:c}". See also http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconformattingtypes.asp</param>
        /// <returns>The specified property as formatted string.</returns>
        /// <genoverload/>
        public string GetFormattedSummaryProperty(string summaryDescriptorName, string propertyName, string format)
        {
            ITreeTableSummary summary = this.GetSummary(summaryDescriptorName);

            if (summary == null)
            {
                return null;
            }

            PropertyDescriptor pd = TypeDescriptor.GetProperties(summary.GetType())[propertyName];
            object value = pd.GetValue(summary);

            return string.Format(format, value);
        }

        /// <summary>Walks up parent branches and reset summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            if (this.GroupCategoryEntry != null)
            {
                this.GroupCategoryEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>Resets the counter fields.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            ResetCounterFields();
            base.InvalidateCounter();
        }

        /// <summary>Resets the counter fields for all the elements.</summary>
        /// <param name="notifyCounterSource">When true notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            ResetCounterFields();
            SectionEntries.InvalidateCounterTopDown(notifyCounterSource);
        }

        /// <summary>Resets the summary fields for all the elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            SectionEntries.InvalidateSummariesTopDown();
        }

        /// <summary>Resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
            if (!this.summaryDirty)
            {
                ParentTable.RaiseGroupSummaryInvalidated(this);
                this.summaryDirty = true;
            }
        }
        
#if DIAGNOSING
        bool inGetVisibleCount = false;
#endif
        void InitVisibleCounters()
        {
            // NOTE: ResetCounterFields might be called from Table.EnsureInitialized
            // while GetVisibleCount was called. It is important that GetVisibleCount()
            // therefore uses a local variable for calculating visible count.
            // this.ParentElement is Table - is Main Group; this.ParentTableDescriptor.ParentRelation != null - in a related table; ((ChildTable)this).ParentNestedTable == null) - and this is not referenced as child table from parent table
            if (ParentElement == null ||
               (this is ChildTable
                && this.ParentElement is Table   
                && this.ParentTableDescriptor.ParentRelation != null 
                && ((ChildTable)this).ParentNestedTable == null)) 
            {
                this.CachedVisibleCount = TreeEntries.VisibleCount; // Make all visible.  Otherwise ElementHelper.FindElement chokes since it things the table has no records.
                this.CachedYamountCount = TreeEntries.YAmountCount; // Make all visible.  Otherwise ElementHelper.FindElement chokes since it things the table has no records.
                this.CachedVisibleCustomCount = TreeEntries.VisibleCustomCount; // Make all visible.  Otherwise ElementHelper.FindElement chokes since it things the table has no records.
            }
            else
            {
                if (this is ChildTable || IsGroupVisible())
                {
                    OnInitializeVisibleCounters();
                }
                else
                {
                    this.CachedVisibleCount = 0;
                    this.CachedYamountCount = 0;
                    this.CachedVisibleCustomCount = 0;
                }
            }
        }

        /// <summary>
        /// Determines whether this group should be visible. By default a group is hidden if it does
        /// not have any records that meet filter criteria.
        /// </summary>
        /// <returns>true if group should be visibe; false otherwise.</returns>
        protected virtual bool IsGroupVisible()
        {
            if (ParentTable.IsPassThroughGrouping)
            {
                return true;
            }

            return GetFilteredRecordCount() > 0;
        }

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void OnInitializeVisibleCounters()
        {
            int visibleCount = 0;
            double yAmountCount = 0;
            double visibleCustomCount = 0;

            foreach (Section s in this.Sections)
            {
                if (this.IsChildVisible(s))
                {
                    visibleCount += s.GetVisibleCount();
                    yAmountCount += s.GetYAmountCount();
                    visibleCustomCount += s.GetVisibleCustomCount();
                }
            }

            this.CachedVisibleCount = visibleCount;
            this.CachedYamountCount = yAmountCount;
            this.CachedVisibleCustomCount = visibleCustomCount;
        }

        ////        public int GetTreeEntriesVisibleCount()
        ////        {
        ////            return TreeEntries.VisibleCount;
        ////        }

        /// <summary>Determines whether the child elements are visible.</summary>
        /// <param name="el">The Element.</param>
        /// <returns>True if the childs are visible.</returns>
        /// <override/>
        public override bool IsChildVisible(Element el)
        {
            if (el is CaptionSection)
            {
                return ShouldShowCaption();
            }

            return IsExpanded;
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (visibleCount == -1)
            {
                InitVisibleCounters();
            }

            return visibleCount;
        }

        /// <summary>Returns the height for the element.</summary>
        /// <returns>returns Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            if (yAmountCount == -1)
            {
                InitVisibleCounters();
            }

            if (this.TreeEntries.CounterFactory is FilteredRecordCounterFactory)
            {
                yAmountCount = (double)visibleCount * (double)ParentTable.DefaultRecordRowHeight;
            }

            return yAmountCount;
        }

        /// <summary>Gets the custom count for visible elements.</summary>
        /// <returns>Visible custom count.</returns>
        /// <override/>
        public override double GetVisibleCustomCount()
        {
            if (visibleCustomCount == -1)
            {
                InitVisibleCounters();
            }

            return visibleCustomCount;
        }

        /// <summary>Gets the custom count for the element.</summary>
        /// <returns>Custom count.</returns>
        /// <override/>
        public override double GetCustomCount()
        {
            return SectionEntries.CustomCount;
        }

        /// <summary>Gets the element count.</summary>
        /// <returns>Number of elements.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return SectionEntries.ElementCount;
        }

        /// <summary>Gets the filtered record count.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return SectionEntries.FilteredRecordCount;
        }

        /// <summary>Gets the record count.</summary>
        /// <returns>Number of records.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return SectionEntries.RecordCount;
        }

        /// <summary>
        /// Returns the number of direct child elements (either records or nested groups) in the group. 
        /// </summary>
        public int FilteredChildNodeCount
        {
            get
            {
                if (this.visibleItemCount == -1)
                {
                    if (this.Details.HasRecords)
                    {
                        this.visibleItemCount = this.FilteredRecords.Count;
                    }
                    else
                    {
                        visibleItemCount = 0;
                        foreach (Group g in this.Groups)
                        {
                            if (g.FilteredRecords.Count > 0)
                            {
                                this.visibleItemCount++;
                            }
                        }
                    }
                }

                return visibleItemCount;
            }
        }

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry()
        {
            if (this.groupSortOrderEntry != null)
            {
                return this.groupSortOrderEntry;
            }

            return this.groupCategoryEntry;
        }

        /// <summary>
        /// Gets the summary element entry.
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetSummaryElementEntry()
        {
            return this.groupCategoryEntry;
        }

        internal GroupCategoryTreeTableEntry GroupCategoryEntry
        {
            get
            {
                return groupCategoryEntry;
            }

            set
            {
                groupCategoryEntry = value;
            }
        }

        internal GroupSortOrderTreeTableEntry GroupSortOrderEntry
        {
            get
            {
                return groupSortOrderEntry;
            }

            set
            {
                groupSortOrderEntry = value;
                if (value != null)
                {
                    ParentTable.hasGroupSortOrderEntry = true;
                }
            }
        }

        /// <summary>
        /// Determines if this is the <see cref="Table.TopLevelGroup"/>.
        /// </summary>
        public virtual bool IsTopLevelGroup
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets if the groups can be collapsed or if they should always be shown expanded.
        /// </summary>
        public virtual bool IsCollapsible
        {
            get
            {
                return !Engine.GetDesignMode() && ShouldShowCaption() && !(this.ParentGroup == null && this.ParentTableDescriptor.IsForeignKeyRelationChildTableDescriptor());
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void InvalidateCounterBottomUp()
        {
            TreeTableEntry e = this.GroupSortOrderEntry;
            if (e == null)
            {
                e = this.GroupCategoryEntry;
            }

            if (e != null)
            {
                e.InvalidateCounterBottomUp(true);
            }

            //// GroupSortOrderEntry.InvalidateCounterBottomUp will also 
            //// call this.GroupCategoryEntry.InvalidateCounterBottomUp

            ////Check if parent group has groupsortorder entry and invalidate
            ////Group g = this.ParentGroup;
            ////while (g != null)
            ////{
            ////    if (g.GroupSortOrderEntry != null)
            ////    {
            ////        g.InvalidateCounterBottomUp();
            ////        break;
            ////    }
            ////    g = g.ParentGroup;
            ////}
            ////
            ////e = this.GroupCategoryEntry;
            ////if (e != null)
            ////    e.InvalidateCounterBottomUp(true);  // CategoryEntry will also invalidate parent elements.
        }

        /// <summary>
        /// Sets the group's expansion state and optionally invalidates counters and optionally raises <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events.
        /// </summary>
        /// <param name="value">The new expansion state.</param>
        /// <param name="refreshCounters">True if counters should be invalidated; False otherwise.</param>
        /// <param name="raiseDisplayElementChangeEvents">True if <see cref="Table.DisplayElementChanging"/>
        /// and <see cref="Table.DisplayElementChanged"/> events should be raised; False otherwise.</param>
        public void SetExpanded(bool value, bool refreshCounters, bool raiseDisplayElementChangeEvents)
        {
            if (!IsCollapsible && value == false)
            {
                return;  // if no caption is shown and therefore no plusminus cell is shown either,
            }

            //// group must always be expanded in that case

            if (Engine.HelpTracing)
            {
                if (value == true)
                {
                    TraceUtil.TraceCurrentMethodInfo(value);
                }
            }

            if (ParentTable == null)
            {
                isExpanded = value;
            }
            else if (isExpanded != value)
            {
                bool success;
                if (value)
                {
                    success = ParentTable.RaiseGroupExpanding(this, raiseDisplayElementChangeEvents);
                }
                else
                {
                    success = ParentTable.RaiseGroupCollapsing(this, raiseDisplayElementChangeEvents);
                }

                if (!success)
                {
                    return;
                }

                this.InvalidateCounter();

                if (refreshCounters)
                {
                    this.InvalidateCounterTopDown(false);
                    this.InvalidateCounterBottomUp(); // this also calls NestedTableEntry, not just GroupEntry for nested tables.
                }

                ParentTable.ClearCollectionCaches();

                isExpanded = value;

                if (value)
                {
                    ParentTable.RaiseGroupExpanded(this, raiseDisplayElementChangeEvents);
                }
                else
                {
                    ParentTable.RaiseGroupCollapsed(this, raiseDisplayElementChangeEvents);
                }
                ////                if (refreshCounters)
                ////                    Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, ParentTable.GetVisibleCount()); // be carefull with GetVisibleCount calls - they gonna recalc counters!
            }
        }

        /// <summary>
        /// Gets or sets the groups expansion state.
        /// </summary>
        public virtual bool IsExpanded
        {
            get
            {
                if (IsCollapsible)
                {
                    return isExpanded;
                }

                return true; // if no caption is shown and therefore no plusminus cell is shown either,
                // group must always be expanded in that case
            }

            set
            {
                SetExpanded(value, true, true);
            }
        }

        /// <summary>
        /// Gets the main category key of this group.
        /// </summary>
        public object Category
        {
            get
            {
                return CategoryKeys.Length > 0 ? categoryKeys[0] : null;
            }
        }

        /// <summary>
        /// Gets the collection of category keys for this group. Child tables can have more than one category key
        /// if multiple relation keys were specified in the <see cref="RelationDescriptor"/>.
        /// </summary>
        public object[] CategoryKeys
        {
            get
            {
                if (categoryKeys == null)
                {
                    categoryKeys = new object[0];
                }

                return categoryKeys;
            }

            set
            {
                categoryKeys = value;
            }
        }

        /// <summary>
        /// Gets the collection of foreign key parent ids associated with category keys for this group when 
        /// the table is grouped by the display member of a foreign key relation. Child tables can have more than one category key
        /// if multiple relation keys were specified in the <see cref="RelationDescriptor"/>.
        /// </summary>
        public object[] CategoryForeignKeyParentIds
        {
            get
            {
                return categoryForeignKeyParentIds;
            }

            set
            {
                categoryForeignKeyParentIds = value;
            }
        }

        /// <summary>
        /// Returns the default value for the specied field taking FieldDescriptor.DefaultValue and
        /// category keys of the child group into account.
        /// </summary>
        /// <param name="fieldDescriptor">The field descriptor.</param>
        /// <returns>Default value.</returns>
        public object GetDefaultValue(FieldDescriptor fieldDescriptor)
        {
            Group g = this;
            while (g != null && !g.IsTopLevelGroup)
            {
                SortColumnDescriptorCollection columns = g.CategoryColumns;
                for (int n = 0; n < g.CategoryKeys.Length; n++)
                {
                    FieldDescriptor fd = this.ParentTableDescriptor.Fields[columns[n].Name];
                    if (fd == null)
                    {
                        continue;
                    }

                    if (fd == fieldDescriptor)
                    {
                        return g.CategoryKeys[n];
                    }

                    // Support for foreign key ids when grouped by display member of foreign key reference.
                    // See Comparer.cs, GetCategoryForeignKeyParentIds method which calculates the 
                    // groups CategoryForeignKeyParentIds 
                    if (fd.IsRelatedField() && g.CategoryForeignKeyParentIds != null)
                    {
                        object[] pids = (object[])g.CategoryForeignKeyParentIds[n];
                        RelationDescriptor rd = fd.relation;
                        if (rd.RelationKeys.Count > 0)
                        {
                            for (int r = 0; r < rd.RelationKeys.Count; r++)
                            {
                                FieldDescriptor fd2 = rd.RelationKeys[r].ParentKeyField;
                                if (fd2 == fieldDescriptor)
                                {
                                    return pids[r];
                                }
                            }
                        }
                        else if (rd.MappingName != string.Empty)
                        {
                            if (rd.MappingName == fieldDescriptor.Name)
                            {
                                return pids[0];
                            }
                        }
                    }
                    else if (fd.ReferencedFields != null && fd.ReferencedFields != string.Empty)
                    {
                        object[] pids = (object[])g.CategoryForeignKeyParentIds[n];
                        string rf = fd.ReferencedFields;
                        string[] parts = rf.Split(';');
                        for (int r = 0; r < parts.Length; r++)
                        {
                            if (parts[r] == fieldDescriptor.Name)
                            {
                                if (pids[r] != null)
                                {
                                    if (!ListUtil.IsComplexType(pids[r].GetType()))
                                    {
                                        return pids[r];
                                    }
                                    else if (pids[r] is ICloneable)
                                    {
                                        return ((ICloneable)pids[r]).Clone();
                                    }
                                }
                            }
                        }
                    }
                }

                if (g is ChildTable)
                {
                    break;
                }

                g = g.ParentGroup;
            }

            if (fieldDescriptor.DefaultValue != null && fieldDescriptor.DefaultValue != string.Empty)
            {
                return fieldDescriptor.DefaultValue;
            }

            ////if (checkAddNewRecord)
            ////{
            //// possibly could add support for getting foreign key reference fields here. which could
            //// then be used by caption bar, e.g. group by Address_City but then also display Address_country in caption.
            ////}
            return null;
        }

        /// <summary>
        /// Returns an array of category keys for this group and all parent groups which is 
        /// used by FilterBarCells and FilterBarSummary to compare whether conditions should
        /// be applied to this group.
        /// </summary>
        public object[] UniqueGroupId
        {
            get
            {
                if (this.IsTopLevelGroup)
                {
                    return null;
                }

                object[] categoryKeys1 = CategoryKeys;
                Group g = ParentGroup;
                if (g == null)
                {
                    return categoryKeys1;
                }

                object[] categoryKeys2 = ParentGroup.UniqueGroupId;
                if (categoryKeys2 == null || categoryKeys2.Length == 0)
                {
                    return categoryKeys1;
                }

                object[] uniqueIds = new object[categoryKeys1.Length + categoryKeys2.Length];

                int i = 0;
                for (int n = 0; n < categoryKeys1.Length; n++)
                {
                    uniqueIds[i++] = categoryKeys1[n];
                }

                for (int n = 0; n < categoryKeys2.Length; n++)
                {
                    uniqueIds[i++] = categoryKeys2[n];
                }

                return uniqueIds;
            }
        }

        /// <summary>
        /// Returns a string combined of all group categories which is used by 
        /// FilterBarCells to name the RecordFilterDescriptor specific for a group.
        /// </summary>
        /// <returns>A string combining all group categories.</returns>
        public string UniqueGroupIdsToString()
        {
            StringBuilder sb = new StringBuilder();
            if (UniqueGroupId != null)
            {
                int n = 0;
                foreach (object id in this.UniqueGroupId)
                {
                    if (n > 0)
                    {
                        sb.Append(":");
                    }

                    // if id is a record use the record identifirer instead.
                    string s;
                    if (id is Record)
                    {
                        s = ((Record)id).Id.ToString();
                    }
                    else
                    {
                        s = id == null ? string.Empty : id.ToString();
                    }

                    sb.AppendFormat("{0}", s);
                    n++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string in format (TableDescriptor.Name) {GetChildCount()}-Items: CategoryKeys.
        /// </summary>
        /// <returns>String with state information about the group.</returns>
        public string CategoriesToString()
        {
            StringBuilder sb = new StringBuilder();
            if (ParentTable != null)
            {
                sb.AppendFormat("({0}) {1}-Items ", ParentTable.TableDescriptor.Name, this.GetChildCount());
            }

            if (CategoryKeys != null)
            {
                int n = 0;
                foreach (SortColumnDescriptor sd in this.CategoryColumns)
                {
                    if (n > 0)
                    {
                        sb.Append(", ");
                    }

                    string s = n >= this.CategoryKeys.Length || this.CategoryKeys[n] == null ? "(null)" : CategoryKeys[n].ToString();
                    sb.AppendFormat("{0} = {1}", sd.Name, s);
                    n++;
                }
            }

            return sb.ToString();
        }

        /// <summary>Returns string representation of the object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            return GetType().Name + " " + CategoriesToString();
        }

        /// <summary>
        /// Gets if caption should be visible in <see cref="Table.DisplayElements"/> or not.
        /// </summary>
        /// <returns>returns True.</returns>
        public virtual bool ShouldShowCaption()
        {
            return true;
        }

        #region PassThroughGroups support
        /// <summary>
        /// Gets or sets a PassThrough group item.
        /// </summary>
        public object PassThroughItem
        {
            get { return passThroughItem; }
            set { passThroughItem = value; }
        }

        /// <summary>
        /// Returns the list that is associated with this ChildTable. The value will only be initialized
        /// when you have a UniformChildList relation and Engine.UseOldUniformChildListRelation is false.
        /// The list is a reference to the strong-typed nested collection in this child table. The ChildTable 
        /// listens to ListChanged events on that list if IBindingList interface is implemented. For all other
        /// relation kinds this value will always be null.
        /// </summary>
        public IList SourceList
        {
            get 
            { 
                return sourceList; 
            }

            set
            {
                if (!Object.ReferenceEquals(sourceList, value))
                {
                    IBindingList bindingList = sourceList as IBindingList;
                    if (bindingList != null)
                    {
                        bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
                    }

                    sourceList = value;

                    bindingList = sourceList as IBindingList;
                    if (bindingList != null)
                    {
                        bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
                    }
                }
            }
        }

        /// <summary>
        /// A reference to the first unsorted record entry for that list. This is used to determine
        /// to map indexes in the child list (e.g. the e.NewIndex from a ListChanged event) to the
        /// absolute unsorted record index in the whole table across all nested child tables.
        /// </summary>
        internal UnsortedRecordsTreeEntry UnsortedEntry
        {
            get { return unsortedEntry; }
            set { unsortedEntry = value; }
        }

        /// <summary>
        /// Returns the position of the first unsorted record in this list
        /// in the Table.UnsortedRecords collection. The ChildTable must belong
        /// to a UniformChildList relation. For other types of relations the
        /// funtion returns -1.
        /// </summary>
        /// <returns>Position of first unsorted record.</returns>
        public int GetChildListPosition()
        {
            if (unsortedEntry == null)
            {
                return -1;
            }

            return unsortedEntry.GetPosition();
        }

        /// <summary>
        /// Specifies the position of the first unsorted record in this list
        /// in the Table.UnsortedRecords collection. The ChildTable must belong
        /// to a UniformChildList relation. 
        /// </summary>
        /// <param name="index">The Index.</param>
        public void SetChildListPosition(int index)
        {
            if (index < 0)
            {
                UnsortedEntry = null;
            }
            else
            {
                UnsortedEntry = this.ParentTable.UnsortedRecords._inner[index];
            }
        }

        void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (ParentTable != null)
            ParentTable.SimulateListChangedWithSourceList(this.sourceList, this, e);
        }
        #endregion
    }
}