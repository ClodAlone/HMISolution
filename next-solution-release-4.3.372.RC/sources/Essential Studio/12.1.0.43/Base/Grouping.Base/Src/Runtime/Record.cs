//-------------------------------------------------------------------------------------------------
// <copyright file="Record.cs" company="syncfusion">
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
using Syncfusion.Collections.BinaryTree;
using Syncfusion.ComponentModel;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Represents a record with data. Each record in the datasource has an associated <see cref="Record"/> object
    /// in the engine. Records are created when the datasource is assigned to a table and before it is sorted or filtered.
    /// When a new record is inserted in the datasource, a <see cref="Record"/> is created. When the grouping or sorting
    /// of a <see cref="Table"/> changes, all <see cref="Record"/> elements stay in sync with their underlying record-counterparts
    /// in the datasource.
    /// <para/>
    /// By default, a record will not appear in the <see cref="Table.DisplayElements"/>. Instead, a record serves as a container
    /// of multiple row elements and nested tables.
    /// </summary>
    /// <remarks>
    /// There are multiple ways to get access to a specific record. <para/>
    /// <list type="bullet">
    /// <item><term>
    /// The <see cref="Table.UnsortedRecords"/> collection of the <see cref="Table"/> class provides access to the records
    /// in the same order as they appear in the datasource. The <see cref="UnsortedRecordsCollection.IndexOf"/> method
    /// of an <see cref="UnsortedRecordsCollection"/> determines the index of any record in the underlying datasource.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.Records"/> collection of the <see cref="Table"/> class provides access to the records
    /// in the order as they were sorted in the engine. The <see cref="RecordsInTableCollectionBase.IndexOf"/> method
    /// of a <see cref="RecordsInTableCollection"/> determines the index of any record in the Table.Records collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Table.FilteredRecords"/> collection of the <see cref="Table"/> class provides access to records that
    /// meet filter criteria in the order they were sorted in the engine. The <see cref="RecordsInTableCollectionBase.IndexOf"/> method
    /// of a <see cref="RecordsInTableCollection"/> determines the index of any record in the Table.FilteredRecords collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Group.Records"/> collection of the <see cref="Group"/> class provides access to the records
    /// in the order as they appear in the group. The <see cref="RecordsInDetailsCollection.IndexOf"/> method
    /// of a <see cref="RecordsInDetailsCollection"/> determines the index of any record in the Group.Records collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Group.FilteredRecords"/> collection of the <see cref="Group"/> class provides access to the records
    /// in the order as they appear in the group. The <see cref="FilteredRecordsInDetailsCollection.IndexOf"/> method
    /// of a <see cref="FilteredRecordsInDetailsCollection"/> determines the index of any record in the Group.FilteredRecords collection.
    /// </term></item>
    /// </list>
    /// <para/>
    /// Given a <see cref="RecordRow"/> or <see cref="NestedTable"/>, you can query its <see cref="Element.ParentRecord"/>
    /// property to determine which record these elements belong to.
    /// <para/>
    /// Since record elements always stay in sync with their underlying record-counterparts
    /// in the datasource, you can keep a bookmark (reference) to a record. For example, you can save a reference to a record,
    /// change the sort order of the table,and then later check Records.IndexOf to determine the new position where
    /// the record can be located after the sort.
    /// <para/>
    /// By default, a record will not appear in the <see cref="Table.DisplayElements"/>. Instead, a record serves as a container
    /// of multiple row elements and nested tables. One exception is if you specified <see cref="Engine.RecordAsDisplayElements"/>.
    /// You can set <see cref="Engine.RecordAsDisplayElements"/> to True if you do not want the engine to treat Record and ColumnHeaderSection
    /// elements as ContainerElements and instead have these elements be returned as a display element in the Table.DisplayElements collection.
    /// However, with a GridGroupingControl you must not change this property since a GridGroupingControl
    /// relies on the behavior that a record is not a display element but a container for rows
    /// and nested tables.
    /// <para/>
    /// The <see cref="GetData"/> method will give return a reference to the original record with data in the datasource.
    /// <para/>
    /// A record can be navigated to a current record if you call its <see cref="SetCurrent"/> method.
    /// <para/>
    /// Individual field contents can be retrieved with the <see cref="GetValue"/> method.
    /// </remarks>
    public class Record : Element, IContainerElement, IComparable
    {
        internal object[] sortKeys;
        internal object[] primaryKeys;
        const int ROWS = 0;
        const int NESTED = 1;
        const int PREVIEW = 2;
        int id;

        /// <summary>Indicates whether this object can be uniquely identified by its Id.</summary>
        /// <returns>True if Id is supported.</returns>
        /// <override/>
        public override bool SupportsId()
        {
            return true;
        }

        /// <summary>Gets or sets the key to identify this object.</summary>
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

        bool recordRowCountChanged
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

        internal bool added
        {
            get
            {
                return Reserved3;
            }

            set
            {
                Reserved3 = value;
            }
        }

        bool isExpanded
        {
            get
            {
                return Reserved4;
            }

            set
            {
                Reserved4 = value;
            }
        }

        bool invalidateCounterBottomUpCalled
        {
            get
            {
                return Reserved5;
            }

            set
            {
                Reserved5 = value;
            }
        }

        bool savedMeetFilterCriteria
        {
            get
            {
                return Reserved6;
            }

            set
            {
                Reserved6 = value;
            }
        }

        UnsortedRecordsTreeEntry sourceEntry;
        PrimaryKeySortedRecordsTreeEntry primaryKeySortedEntry;
        SortedRecordsTreeTableEntry sortedEntry;
        int _sourceIndex = -1;
       
        internal int sourceIndex
        {
            get
            {
                return _sourceIndex;
            }

            set
            {
                _sourceIndex = value;
                data = null;
                ////CheckSourceIndex();
            }
        }

        /*void CheckSourceIndex()
        {
            if ( this.UnsortedEntry == null)
                return;

            int i = this.UnsortedEntry.GetPosition();
            if (this.sourceIndex > 0 && i > 0
                && this.sourceIndex != i)
                Console.WriteLine("lkdfkldsf");
        }  */

        int _sourceListVersion = -1;

        internal int sourceListVersion
        {
            get
            {
                return _sourceListVersion;
            }

            set
            {
                _sourceListVersion = value;
                data = null;
                ////CheckSourceIndex();
            }
        }

        internal object data = null;

        RecordPartsTreeTable recordPartEntries;

        int visibleCount
        {
            get
            {
                return RecordPartEntries.visibleCount;
            }

            set
            {
                RecordPartEntries.visibleCount = value;
            }
        }

        double yAmountCount
        {
            get
            {
                return RecordPartEntries.yAmountCount;
            }

            set
            {
                RecordPartEntries.yAmountCount = value;
            }
        }

        RecordPartInRecordCollection _recordParts
        {
            get
            {
                return RecordPartEntries._recordParts;
            }

            set
            {
                RecordPartEntries._recordParts = value;
            }
        }

        /// <summary>
        /// Gets the value from the underlying datasource.
        /// </summary>
        /// <param name="columnName">The field to be retrieved.</param>
        /// <returns>returns value from the underlying datasource</returns>
        [Browsable(false)]
        public object this[string columnName]
        {
            get
            {
                return this.GetValue(columnName);
            }
        }
        /// <summary>
        /// Initializes a new record in the specified parent table.
        /// </summary>
        /// <param name="parentTable">The table this record belongs to.</param>
        public Record(Table parentTable)
            : base(parentTable)
        {
            this.ParentElement = parentTable;
            this.filterState = -1;
            this.recordRowCountChanged = true;
            this.savedMeetFilterCriteria = true;
        }
        
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (recordPartEntries != null)
                {
                    recordPartEntries.Dispose();
                }

                recordPartEntries = null;
                this.data = null;
            }

            base.Dispose(disposing);
        }

        /// <override/>
        /// <summary>Gets the display element kind.</summary>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Record; }
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return this.ShouldCreateChildElementTreeTable();
        }

        /// <summary>
        /// Returns the relative row position where the first nested table is displayed.
        /// </summary>
        /// <returns>The relative row position where the first nested table is displayed (zero-based).</returns>
        public int GetDisplayElementOffsetOfFirstNestedTable()
        {
            return GetRecordRowsVisibleCount();
        }

        /// <summary>
        /// Returns what the number of visible rows including rows in nested tables would be if the record is expanded.
        /// </summary>
        /// <returns>The number of visible rows including rows in nested tables if the record is expanded.</returns>
        public int GetExpandedVisibleCount()
        {
            return GetRecordRowsVisibleCount() + GetNestedTablesVisibleCount();
        }

        /// <summary>
        /// Returns what the number of visible rows would be if the record is collapsed.
        /// </summary>
        /// <returns>The number of visible rows if the record is collapsed.</returns>
        public int GetCollapsedVisibleCount()
        {
            return GetRecordRowsVisibleCount();
        }

        /// <summary>
        /// Returns what the number of visible rows in nested tables would be if the record is expanded.
        /// </summary>
        /// <returns>The number of visible rows in nested tables if the record is expanded.</returns>
        public int GetNestedTablesVisibleCount()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return 0;
            }

            int _expandCount = 0;
            foreach (NestedTable nt in NestedTables)
            {
                _expandCount += nt.GetVisibleCount();
            }

            return _expandCount;
        }

        /// <summary>
        /// Returns the number of visible record rows excluding nested tables.
        /// </summary>
        /// <returns>The number of visible record rows excluding nested tables.</returns>
        public int GetRecordRowsVisibleCount()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return 1;
            }

            int _expandFrom = 0;
            foreach (RecordRow rr in RecordRows)
            {
                _expandFrom += rr.GetVisibleCount();
            }

            return _expandFrom;
        }

        /// <summary>
        /// Returns what the number of visible preview rows would be if the record is collapsed.
        /// </summary>
        /// <returns>The number of visible preview rows if the record is collapsed.</returns>
        public int GetRecordPreviewRowsVisibleCount()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return 0;
            }

            int _expandFrom = 0;
            foreach (RecordPreviewRow rr in RecordPreviewRows)
            {
                _expandFrom += rr.GetVisibleCount();
            }

            return _expandFrom;
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            if (!ShouldCreateChildElementTreeTable())
            {
                return null;
            }

            this.AdjustRecordRowCount();
            return recordPartEntries;
        }

        /// <summary>
        /// Shoulds the create child element tree table.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <exclude/>
        protected virtual bool ShouldCreateChildElementTreeTable()
        {
            return this.ParentTable != null && !this.ParentTable.RecordsAsDisplayElements;
        }

        internal RecordPartsTreeTable RecordPartEntries
        {
            get
            {
                if (recordPartEntries == null)
                {
                    recordPartEntries = new RecordPartsTreeTable(this);
                }

                return recordPartEntries;
            }
        }

        /// <summary>
        /// Returns the collection of record parts which includes <see cref="RecordRowsPart"/>, <see cref="RecordNestedTablesPart"/>, and
        /// <see cref="RecordPreviewRowsPart"/>.
        /// </summary>
        public RecordPartInRecordCollection RecordParts
        {
            get
            {
                if (_recordParts == null)
                {
                    _recordParts = new RecordPartInRecordCollection(this);
                    _recordParts.Add(Engine.CreateRecordRowsPart(this));
                    _recordParts.Add(Engine.CreateRecordNestedTablesPart(this));
                    _recordParts.Add(Engine.CreateRecordPreviewRowsPart(this));
                }

                return _recordParts;
            }
        }

        /// <summary>
        /// Determines if this record is the current record in the parent table.
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return ParentTable.CurrentRecord == this;
            }
        }

        /// <summary>
        /// Determines if this record is the current record in the parent table and if BeginEdit was called.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return IsCurrent && ParentTable.CurrentRecordManager.IsEditing;
            }
        }

        /// <summary>
        /// Deletes the record from the underlying data source.
        /// </summary>
        public void Delete()
        {
            ParentTable.DeleteRecord(this);
        }

        /// <summary>
        /// Makes this record the current record in the parent table.
        /// </summary>
        /// <returns>True if record could be made current record; False if navigating to record failed.</returns>
        public bool SetCurrent()
        {
            return (Record)ParentTable.CurrentRecordManager.NavigateTo(this) == this;
        }

        /// <summary>
        /// Makes this record the current record in the parent table.
        /// </summary>
        /// <returns>True if record could be made current record; False if navigating to record failed.</returns>
        public bool SetCurrent(string fieldName)
        {
            Table table = ParentTable;
            if (fieldName != null && fieldName != string.Empty)
            {
                table.CurrentRecordManager.CurrentField = table.ParentTableDescriptor.Fields[fieldName];
            }

            return (Record)ParentTable.CurrentRecordManager.NavigateTo(this) == this;
        }

        /// <summary>
        /// Makes this record the current record in the parent table and calls <see cref="CurrentRecordManager.BeginEdit"/>.
        /// </summary>
        /// <returns>True if record could be made current record and BeginEdit was successful; False otherwise.</returns>
        public bool BeginEdit()
        {
            if (SetCurrent())
            {
                ParentTable.CurrentRecordManager.BeginEdit();
                return ParentTable.CurrentRecordManager.IsEditing;
            }

            return false;
        }

        /// <summary>
        /// Saves pending changes to the datasource after <see cref="CurrentRecordManager.BeginEdit"/> was called.
        /// </summary>
        /// <returns>True if changes could be saved; False otherwise.</returns>
        public bool EndEdit()
        {
            if (ParentTable.CurrentRecord != this)
            {
                return false;
            }

            ParentTable.CurrentRecordManager.EndEdit();
            return !ParentTable.CurrentRecordManager.IsEditing;
        }

        /// <summary>
        /// Saves pending changes to the datasource after <see cref="CurrentRecordManager.BeginEdit"/> was called.
        /// </summary>
        /// <param name="record">Returns the record that was changed. Normally this is a reference to the current record. But if the current
        /// record is an AddNewRecord, a new record will be added to the table and instead of returning the AddNewRecord, a reference
        /// to the newly created Record element is returned.</param>
        /// <returns>True if changes could be saved; False otherwise.</returns>
        public bool EndEdit(out Record record)
        {
            record = null;
            if (ParentTable.CurrentRecord != this)
            {
                return false;
            }

            record = ParentTable.CurrentRecordManager.EndEdit();
            return !ParentTable.CurrentRecordManager.IsEditing;
        }

        /// <summary>
        /// Discards pending changes after <see cref="CurrentRecordManager.BeginEdit"/> was called.
        /// </summary>
        /// <returns>True if changes could be discarded; False otherwise.</returns>
        public bool CancelEdit()
        {
            if (ParentTable.CurrentRecord != this)
            {
                return false;
            }

            ParentTable.CurrentRecordManager.CancelEdit();
            return !ParentTable.CurrentRecordManager.IsEditing;
        }

        /// <summary>
        /// Saves the value into the underlying datasource. If <see cref="BeginEdit"/> was called, the value
        /// is saved as a pending change in the <see cref="CurrentRecordPropertyCollection"/>. If the field is
        /// an unbound field, a <see cref="TableDescriptor.SaveValue"/> event is raised.
        /// </summary>
        /// <param name="fieldDescriptor">The field to be saved.</param>
        /// <param name="value">The new value.</param>
        public virtual void SetValue(FieldDescriptor fieldDescriptor, object value)
        {
            // TODO: RelationKind.ForeignKeyKeyWords // ForeignListItems
            if (fieldDescriptor.GetRelatedSummaryDescriptor() != null)
            {
                return;
            }

            if (fieldDescriptor.ReadOnly)
            {
                throw new NotSupportedException(fieldDescriptor.Name + " is Read-only.");
            }

            if (IsEditing && !fieldDescriptor.ForceImmediateSaveValue)
            {
                ParentTable.CurrentRecordManager.Properties[fieldDescriptor.Name].ModifiedValue = value;
            }
            else
            {
                fieldDescriptor.SetValue(this, value);
            }
        }

        /// <summary>
        /// Gets the value from the underlying datasource.
        /// </summary>
        /// <param name="cd">The field to be retrieved.</param>
        /// <returns>returns value from the underlying datasource.</returns>
        public virtual object GetValue(SortColumnDescriptor cd)
        {
            if (cd != null)
            {
                FieldDescriptor fd = cd.FieldDescriptor;
                if (fd != null)
                {
                    return GetValue(fd);
                }

                return GetValue(cd.Name);
            }

            return null;
        }

        /// <summary>
        /// Gets the value from the underlying datasource.
        /// </summary>
        /// <param name="fieldDescriptor">The field to be retrieved.</param>
        /// <returns>returns value from the underlying datasource.</returns>
        public virtual object GetValue(FieldDescriptor fieldDescriptor)
        {
            if (fieldDescriptor != null)
            {
                if (!ParentTable.IsSorting && IsEditing && ParentTable.CurrentRecordManager.IsModified)
                {
                    return ParentTable.CurrentRecordManager.Properties[fieldDescriptor.Name].CurrentValue;
                }
                else
                {
                    return fieldDescriptor.GetValue(this);
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the value into the underlying datasource. If <see cref="BeginEdit"/> was called, the value
        /// is saved as a pending change in the <see cref="CurrentRecordPropertyCollection"/>. If the field is
        /// an unbound field, a <see cref="TableDescriptor.SaveValue"/> event is raised.
        /// </summary>
        /// <param name="name">The field to be saved.</param>
        /// <param name="value">The new value.</param>
        public void SetValue(string name, object value)
        {
            int indexOf = ParentTable.TableDescriptor.Fields.IndexOf(name);
            if (indexOf != -1)
            {
                FieldDescriptor cd = ParentTable.TableDescriptor.Fields[indexOf];
                if (cd != null)
                {
                    SetValue(cd, value);
                }
            }
        }

        /// <summary>
        /// Gets the value from the underlying datasource.
        /// </summary>
        /// <param name="name">The field to be retrieved.</param>
        /// <returns>returns value from the underlying datasource</returns>
        public object GetValue(string name)
        {
            int indexOf = ParentTable.TableDescriptor.Fields.IndexOf(name);
            if (indexOf != -1)
            {
                FieldDescriptor cd = ParentTable.TableDescriptor.Fields[indexOf];
                if (cd != null)
                {
                    return GetValue(cd);
                }
            }
            else
            {
                //// New in version 4.2: Special fields ##this and ##parent help
                //// associate records similar to a RelatedMasterDetails realtion for
                //// UniformChildList realtions.
                ////
                //// Both fields will be added to RelationKeys of RelationDescriptor.

                name = name.ToLower();
                if (name == "##this")
                {
                    return this;
                }
                else if (name == "##parent")
                {
                    return this.Parent;
                }
            }

            return null;
        }

        Record parent;

        /// <summary>
        /// A reference to the parent record in a UnformChildList relation. Is not defined
        /// for other relation kinds.
        /// </summary>
        public Record Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>Resets the summaries for all the elements.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            if (this.sortedEntry != null)
            {
                this.sortedEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>
        /// The <see cref="RecordsDetails"/> this record belongs to.
        /// </summary>
        public RecordsDetails ParentDetails
        {
            get
            {
                return (RecordsDetails)base.ParentElement;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>
        /// Gets the array of sort keys that define the sort-order of this record.
        /// </summary>
        public object[] SortKeys
        {
            get
            {
                return sortKeys;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns source index</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int GetSourceIndex()
        {
            if (this.UnsortedEntry == null)
            {
                return sourceIndex;
            }

            if (ParentTable !=null&&(sourceIndex == -1 || sourceListVersion != ParentTable.SourceListVersion))
            {
                // Make sure we are not call from InitUnsortedRecords and sourceIndex was set manually before ...
                if (UnsortedEntry.Record != null)
                {
                    sourceIndex = UnsortedEntry.GetPosition();
                }

                sourceListVersion = ParentTable.SourceListVersion;
            }

            return sourceIndex;
        }

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void SetSourceIndex(int index, int sourceListVersion, object data)
        {
            this.sourceIndex = index;
            this.sourceListVersion = sourceListVersion;
            this.data = data;
        }

        /// <summary>
        /// Returns the source index in the current child list if the record belongs
        /// to a UniformChildList relation. The index is zero-based for each ChildTable.
        /// </summary>
        /// <returns>The source index in the current child list.</returns>
        public int GetUniformChildListSourceIndex()
        {
            if (this.UnsortedEntry == null)
            {
                return sourceIndex;
            }

            if (ParentTable.IsNewUniformChildListRelation())
            {
                ChildTable ct = ParentChildTable;
                UnsortedRecordsTreeEntry ue = ct.UnsortedEntry;
                return UnsortedEntry.GetPosition() - ue.GetPosition();
            }

            return GetSourceIndex();
        }

        /// <summary>
        /// Determines if record is marked as selected.
        /// </summary>
        /// <returns>True if selected.</returns>
        public virtual bool IsSelected()
        {
            return ParentTable != null && ParentTable.SelectedRecords.Contains(this);
        }

        /// <summary>
        /// Marks record as selected and adds it to the <see cref="SelectedRecordsCollection"/>.
        /// </summary>
        /// <param name="value">True if the record should be marked as selected.</param>
        public virtual void SetSelected(bool value)
        {
            if (IsSelected() != value)
            {
                if (value)
                {
                    ParentTable.SelectedRecords.Add(this);
                }
                else
                {
                    ParentTable.SelectedRecords.Remove(this);
                }
            }
        }

        /// <summary>
        /// Marks record and recursively all child records as selected and adds it to the <see cref="SelectedRecordsCollection"/>.
        /// </summary>
        /// <param name="value">True if the record should be marked as selected.</param>
        public virtual void SetSelectedRecursive(bool value)
        {
            SetSelected(value);

            foreach (NestedTable nt in this.NestedTables)
            {
                // Fix: SD4186 - Records Add/Delete problem with UseLazyUniformChildListRelation setting
                if (nt.ChildTable != null)
                {
                    foreach (Record r in nt.ChildTable.FlattenedFilteredRecords)
                    {
                        r.SetSelectedRecursive(value);
                    }
                }
            }
        }

        /// <summary>Gets the record data.</summary>
        /// <returns>Record data.</returns>
        /// <override/>
        public override object GetData()
        {
            Table tb = ParentTable;

            // Only return cached data when the table's underlying list was not changed.
            // Also new: Always return cached data when IsNewUniformChildListRelation
            if (data != null && (sourceListVersion == tb.SourceListVersion || tb.IsNewUniformChildListRelation()))
            {
                return data;
            }

            if (this.UnsortedEntry != null)
            {
                if (sourceIndex == -1 || sourceListVersion != ParentTable.SourceListVersion)
                {
                    // Make sure we are not call from InitUnsortedRecords and sourceIndex was set manually before ...
                    if (UnsortedEntry.Record != null)
                    {
                        sourceIndex = UnsortedEntry.GetPosition();
                    }

                    sourceListVersion = ParentTable.SourceListVersion;
                }

                // Check if caching is allowed at all.
                if (!ParentTable.cacheRecordData && !tb.IsNewUniformChildListRelation())
                {
                    if (ParentTable.deletedrecord != null && ParentTable.IsInSourceListItemDeleted)
                    {
                        return ParentTable.deletedrecord;
                    }
                    return ParentTable.GetSourceListItem(sourceIndex);
                }
            }

            if (sourceIndex == -1)
            {
                return null;
            }

            ////Trace.WriteLine(String.Format("GetData {0} : {1}", ParentTable, sourceIndex));
            if (tb.IsNewUniformChildListRelation())
            {
                ChildTable ct = ParentChildTable;
                if (ct != null)
                {
                    int n = this.GetUniformChildListSourceIndex();
                    data = ParentChildTable.SourceList[n];
                    return data;
                }
            }

            data = ParentTable.GetSourceListItem(sourceIndex);
            return data;
        }

        /// <summary>
        /// Refreshed the SortKeys collection.
        /// </summary>
        /// <param name="isSorted">True if the list is sorted.</param>
        /// <param name="arrayOfColumnDescriptors">Array of column descriptors.</param>
        /// <param name="arrayOfPropertyDescriptor">array of property descriptors.</param>
        public void UpdateSortInfo(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, PropertyDescriptor[] arrayOfPropertyDescriptor)
        {
            if (isSorted)
            {
                object value = GetData();

                ////CheckSourceIndex();

                if (value != null && isSorted)
                {
                    this.sortKeys = new object[arrayOfColumnDescriptors.Length];
                    System.Data.DataRowView drv = value as System.Data.DataRowView;
                    if (drv != null && (drv.Row.RowState == System.Data.DataRowState.Deleted || drv.Row.RowState == System.Data.DataRowState.Detached))
                    {
                        for (int i = 0; i < arrayOfColumnDescriptors.Length; i++)
                        {
                            this.sortKeys[i] = null;
                        }

                        return;
                    }

                    for (int i = 0; i < arrayOfColumnDescriptors.Length; i++)
                    {
                        SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[i];
                        if (arrayOfPropertyDescriptor == null || arrayOfPropertyDescriptor[i] == null)
                        {
                            this.sortKeys[i] = this.GetValue(columnDescriptor);
                        }
                        else
                        {
                            this.sortKeys[i] = arrayOfPropertyDescriptor[i].GetValue(value);
#if TESTING
                            if (SortColumnComparer._Compare(this.sortKeys[i], GetValue(columnDescriptor)) != 0)
                                Debugger.Break();
#endif
                        }
                    }

                    return;
                }
            }

            this.sortKeys = null;
        }

        /// <summary>
        /// Gets the array of primary keys that define this record.
        /// </summary>
        public object[] PrimaryKeys
        {
            get
            {
                if (primaryKeys == null)
                {
                    SortColumnDescriptor[] arrayOfPKColumnDescriptors;
                    PropertyDescriptor[] arrayOfPKPropertyDescriptor;
                    bool isPKSorted;
                    ParentTableDescriptor.GetPrimaryKeySortInfo(out isPKSorted, out arrayOfPKColumnDescriptors, out arrayOfPKPropertyDescriptor);
                    UpdatePrimaryKeys(isPKSorted, arrayOfPKColumnDescriptors, arrayOfPKPropertyDescriptor);
                }

                return this.primaryKeys;
            }
        }

        /// <summary>
        /// Refreshed the PrimaryKeys collection.
        /// </summary>
        /// <param name="isSorted">True it the list is sorted.</param>
        /// <param name="arrayOfColumnDescriptors">Array of column descriptors.</param>
        /// <param name="arrayOfPropertyDescriptor">Array of property descriptors.</param>
        public void UpdatePrimaryKeys(bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, PropertyDescriptor[] arrayOfPropertyDescriptor)
        {
            if (isSorted)
            {
                object value = GetData();

                ////CheckSourceIndex();

                if (value != null && isSorted)
                {
                    this.primaryKeys = new object[arrayOfColumnDescriptors.Length];
                    System.Data.DataRowView drv = value as System.Data.DataRowView;
                    if (drv != null && (drv.Row.RowState == System.Data.DataRowState.Deleted || drv.Row.RowState == System.Data.DataRowState.Detached))
                    {
                        for (int i = 0; i < arrayOfColumnDescriptors.Length; i++)
                        {
                            this.primaryKeys[i] = null;
                        }

                        return;
                    }

                    for (int i = 0; i < arrayOfColumnDescriptors.Length; i++)
                    {
                        SortColumnDescriptor columnDescriptor = arrayOfColumnDescriptors[i];
                        if (arrayOfPropertyDescriptor == null || arrayOfPropertyDescriptor[i] == null)
                        {
                            this.primaryKeys[i] = this.GetValue(columnDescriptor);
                        }
                        else
                        {
                            this.primaryKeys[i] = arrayOfPropertyDescriptor[i].GetValue(value);
                        }
                    }

                    return;
                }
            }

            this.primaryKeys = null;
        }

        internal void SetData(object value, bool isSorted, SortColumnDescriptor[] arrayOfColumnDescriptors, PropertyDescriptor[] arrayOfPropertyDescriptor)
        {
            if (this.ParentElement == null)
            {
                data = value;
                sourceListVersion = -1;
            }
            else
            {
                // Could be called for example when Item was changed (ListChangeType.ItemChanged)
                sourceListVersion = ParentTable.SourceListVersion;
                data = value;
            }

            UpdateSortInfo(isSorted, arrayOfColumnDescriptors, arrayOfPropertyDescriptor);
        }

        internal UnsortedRecordsTreeEntry UnsortedEntry
        {
            get
            {
                return sourceEntry;
            }

            set
            {
                sourceEntry = value;
            }
        }

        internal PrimaryKeySortedRecordsTreeEntry PrimaryKeySortedEntry
        {
            get
            {
                return primaryKeySortedEntry;
            }

            set
            {
                primaryKeySortedEntry = value;
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
            return SortedEntry;
        }

        /// <summary>
        /// Gets summary information for this element and child elements. The summaries
        /// are in the same order as the <see cref="TableDescriptor.Summaries"/> of the
        /// parent table descriptor.
        /// </summary>
        /// <param name="parentTable">A reference to the parent table of this element.</param>
        /// <param name="summaryChanged">Returns True if changes were detected.</param>
        /// <returns>An array of <see cref="ITreeTableSummary"/> objects.</returns>
        /// <override/>
        public override ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            summaryChanged = false;
            SummaryDescriptorCollection sdc = parentTable.TableDescriptor.Summaries;
            if (!sdc.AnyDescriptorIgnoreRecordFilter && !MeetsFilterCriteria())
            {
                return parentTable.GetEmptySummaries();
            }
            else
            {
                return sdc.CreateSummaries(this);
            }
        }

        /// <summary>
        /// Returns the next record in the Table.Records collection.
        /// </summary>
        /// <returns>Next record.</returns>
        public Record GetNextRecord()
        {
            return (Record)ElementHelper.GetNextSibling(this);
        }

        /// <summary>
        /// Returns the previous record in the Table.Records collection.
        /// </summary>
        /// <returns>Previous record.
        /// </returns>
        public Record GetPreviousRecord()
        {
            return (Record)ElementHelper.GetPreviousSibling(this);
        }

        /// <summary>
        /// Detaches the record temporarily from the group and so that it can be reinserted
        /// at a new sort position with a call to AttachToGroup or ReinsertRecord.
        /// </summary>
        /// <returns>returns -1 value</returns>
        public int DetachFromGroup()
        {
            if (ParentTable.VirtualMode)
            {
                return -1;
            }

            SortedRecordsTreeTableEntry entry = SortedEntry;
            if (entry != null)
            {
                ////int p = entry.GetPosition();
                entry.Tree.Remove(entry);
            }

            return -1;
        }

        /// <summary>
        /// Inserts the record into the same group at the correct sort position. 
        /// AttachToGroup inserts it into the same group the record used to belong to
        /// before it was detached. Call ReinsertRecord if the record should be 
        /// inserted into a different group.
        /// </summary>
        /// <returns>returns Index.</returns>
        public int AttachToGroup()
        {
            if (ParentTable.VirtualMode)
            {
                return -1;
            }

            SortedRecordsTreeTableEntry entry = SortedEntry;
            return entry.Tree.Add(entry);
        }

        /// <summary>
        /// Determines the group the record belongs to and inserts it at the
        /// correct sort position. The record must have previously been
        /// detached from the engine with DetachRecord
        /// </summary>       
        /// <returns>returns Group</returns>
        public Group ReinsertRecord()
        {
            EnsureSortedEntry(this);
            return ParentTable.InsertSortedRecordsTreeEntry(SortedEntry);
        }

        internal void EnsureSortedEntry(Record record)
        {
            if (record.SortedEntry == null)
            {
                SortedRecordsTreeTableEntry sortedEntry = new SortedRecordsTreeTableEntry();
                sortedEntry.Element = record;
                record.SortedEntry = sortedEntry;
            }
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns boolean value.</returns>
        /// <exclude/>
        public bool GetSavedMeetsFilterCriteria()
        {
            Table table = ParentTable;
            Engine engine = Engine;
            if (engine.GetDesignMode() || (table != null && table.WithoutCounter))
            {
                return true;
            }

            return this.savedMeetFilterCriteria;
        }

        /// <summary>
        /// Determines if the record meets filter criteria.
        /// </summary>
        /// <returns>True if the record meets filter criteria; False otherwise.</returns>
        public virtual bool MeetsFilterCriteria()
        {
            Table table = ParentTable;
            Engine engine = Engine;
            if (engine.GetDesignMode() || (table != null && table.WithoutCounter))
            {
                return true;
            }

            if (filterState == -1)
            {
                QueryRecordMeetsFilterCriteriaEventArgs e = new QueryRecordMeetsFilterCriteriaEventArgs(this, true);
                engine.RaiseQueryRecordMeetsFilterCriteria(e);
                if (e.Handled)
                {
                    return e.Result;
                }

                filterState = ParentTableDescriptor.RecordFilters.CompareRecord(this) ? 1 : 0;
            }

            return filterState == 1;
        }

        /// <summary>
        /// Determines if the record has been added to the table and inserted into a group.
        /// </summary>
        public bool IsInitialized
        {
            get
            {
                return sortedEntry != null && sortedEntry.SortedRecordsTreeTable != null;
            }
        }

        internal SortedRecordsTreeTableEntry SortedEntry
        {
            get
            {
                return sortedEntry;
            }

            set
            {
                sortedEntry = value;
            }
        }

        /// <summary>Resets the counter.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            if (this.ShouldCreateChildElementTreeTable())
            {
                // Note: Accessing visibleCount, yAmountCount  will create RecordPartsTreeTable ...
                visibleCount = -1;
                yAmountCount = -1;
            }

            filterState = -1;
            recordRowCountChanged = true;
            base.InvalidateCounter();
        }

        /// <summary>Called when the counters are reset.</summary>
        /// <override/>
        public override void OnElementTreeInvalidateCounterBottomUp()
        {
            this.invalidateCounterBottomUpCalled = true;
            base.OnElementTreeInvalidateCounterBottomUp();
        }
        
        /// <summary>
        /// Gets the number of visible elements in this group.
        /// </summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                this.savedMeetFilterCriteria = MeetsFilterCriteria();
                if (savedMeetFilterCriteria)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }

            this.AdjustRecordRowCount();
            // Note: Accessing visibleCount, yAmountCount  will create RecordPartsTreeTable ...
            if (visibleCount != -1)
            {
                return visibleCount;
            }

            // NOTE: InvalidateCounter might be called from Table.EnsureInitialized
            // while GetVisibleCount was called. It is important that GetVisibleCount()
            // therefore uses a local variable for calculating visible count.
            int visCount = 0;

            this.savedMeetFilterCriteria = MeetsFilterCriteria();
            if (savedMeetFilterCriteria)
            {
                foreach (RecordPart s in this.RecordParts)
                {
                    if (this.IsChildVisible(s))
                    {
                        visCount += s.GetVisibleCount();
                    }
                }
            }

            visibleCount = visCount;
            return visibleCount;
        }

        ////        internal bool HasCount(int counterKind)
        ////        {
        ////            if (CounterKind.IsVisibleCounter(counterKind))
        ////                return MeetsFilterCriteria();
        ////
        ////            return true;
        ////        }

        /// <summary>Gets the counter.</summary>
        /// <returns>returns Counter.</returns>
        /// <override/>
        public override ITreeTableCounter GetCounter()
        {
            ////            if (!(ParentSection is AddNewRecordSection))
            ////                Console.WriteLine(this.GetType());
            Table parentTable = ParentTable;
            TableDescriptor td = parentTable.TableDescriptor;
            int previewRowsPerRecord = td.PreviewRowsPerRecord;
            int rowsPerRecord = td.RowsPerRecord;
            int elementCount = previewRowsPerRecord + rowsPerRecord;
            int filterRecordCount = 1;
            int recordCount = 1;
            int _visibleCount = 0;
            double _yAmountCount = 0;
            this.savedMeetFilterCriteria = MeetsFilterCriteria();
            if (savedMeetFilterCriteria)
            {
                if (!this.ShouldCreateChildElementTreeTable())
                {
                    _visibleCount = 1;
                    _yAmountCount = parentTable.DefaultRecordRowHeight;
                }
                else if (!invalidateCounterBottomUpCalled && td.Relations.NestedCount == 0)
                {
                    // could also check for _recordParts == null but !invalidateCounterBottomUpCalled performs slightly better / less memory is consumed
                    _visibleCount = rowsPerRecord;
                    if (!IsExpanded && this.ShouldShowRecordPreviewRows())
                    {
                        _visibleCount += previewRowsPerRecord;
                    }

                    _yAmountCount = parentTable.DefaultRecordRowHeight * rowsPerRecord;
                    if (!IsExpanded && this.ShouldShowRecordPreviewRows())
                    {
                        _yAmountCount += parentTable.DefaultRecordPreviewRowHeight;
                    }
                }
                else
                {
                    _visibleCount = GetVisibleCount();
                    _yAmountCount = GetYAmountCount();
                    elementCount = GetElementCount();
                }
            }
            else
            {
                _visibleCount = 0;
                elementCount = 0;
                filterRecordCount = 0;
                _yAmountCount = 0;
            }

            if (this is AddNewRecord)
            {
                filterRecordCount = 0;
                recordCount = 0;
            }

            if (this.ParentElement == null || this.ParentElement.IsChildVisible(this))
            {
                return CounterFactory.CreateCounter(_visibleCount, _yAmountCount, filterRecordCount, elementCount, recordCount, GetCustomCount(), GetVisibleCustomCount());
            }
            else
            {
                return CounterFactory.CreateCounter(0, 0, filterRecordCount, elementCount, recordCount, GetCustomCount(), 0);
            }
        }

        /// <summary>
        /// Gets the height (e.g. screen pixels) for the element.
        /// </summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            Table parentTable = ParentTable;
            if (parentTable == null)
            {
                return 0;
            }

            TableDescriptor td = parentTable.TableDescriptor;
            if (!this.ShouldCreateChildElementTreeTable())
            {
                if (this.MeetsFilterCriteria())
                {
                    return parentTable.DefaultRecordRowHeight;
                }
                else
                {
                    return 0;
                }
            }

            // NOTE: InvalidateCounter might be called from Table.EnsureInitialized
            // while GetVisibleCount was called. It is important that GetVisibleCount()
            // therefore uses a local variable for calculating visible count.
            double yCount = 0;

            if (this.MeetsFilterCriteria())
            {
                foreach (RecordPart s in this.RecordParts)
                {
                    if (this.IsChildVisible(s))
                    {
                        yCount += s.GetYAmountCount();
                    }
                }
            }

            // Note: Accessing visibleCount, yAmountCount  will create RecordPartsTreeTable ...
            yAmountCount = yCount;
            return yAmountCount;
        }

        /// <summary>Gets the custom count for the element.</summary>
        /// <returns>Custom count.</returns>
        /// <override/>
        public override double GetCustomCount()
        {
            CustomCountEventArgs e = new CustomCountEventArgs(this, 0);
            this.EngineTable.RaiseQueryCustomCount(e);
            return e.CustomCount;
        }

        /// <summary>Gets the custom count for the visible elements.</summary>
        /// <returns>Visible custom count.</returns>
        /// <override/>
        public override double GetVisibleCustomCount()
        {
            if (this.MeetsFilterCriteria())
            {
                CustomCountEventArgs e = new CustomCountEventArgs(this, 0);
                this.EngineTable.RaiseQueryVisibleCustomCount(e);
                return e.CustomCount;
            }

            return 0;
        }

        /// <summary>Determines if the child elements are visible.</summary>
        /// <param name="el">The Element.</param>
        /// <returns>True if the childs are visible.</returns>
        /// <override/>
        public override bool IsChildVisible(Element el)
        {
            int partNum = RecordParts.IndexOf((RecordPart)el);
            if (partNum == 0)
            {
                return ShouldShowRecordRows();
            }
            else if (partNum == 1)
            {
                return IsExpanded;
            }
            else if (partNum == 2)
            {
                return !IsExpanded && this.ShouldShowRecordPreviewRows();
            }

            return false;
        }

        /// <summary>
        /// Called to determine if preview rows should be visible in <see cref="Table.DisplayElements"/>.
        /// </summary>
        /// <returns>True if preview rows should be visible in <see cref="Table.DisplayElements"/>; False otherwise.</returns>
        public virtual bool ShouldShowRecordPreviewRows()
        {
            return false;
        }

        /// <summary>
        /// Called to determines if record rows should be visible in <see cref="Table.DisplayElements"/>.
        /// </summary>
        /// <returns>True if record rows should be visible in <see cref="Table.DisplayElements"/>; False otherwise.</returns>
        public virtual bool ShouldShowRecordRows()
        {
            return true;
        }

        /// <summary>
        /// Gets if record can be collapsed.
        /// </summary>
        public virtual bool IsCollapsible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets / sets the records expansion state.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return isExpanded || !IsCollapsible;

                // if no plusminus cell is shown (and record has nested tables),
                // record must always be expanded in that case
            }

            set
            {
                SetExpanded(value, true, true);
            }
        }

        static bool warnOnChildTableNullReference = false;

        /// <summary>
        /// Sets the record's expansion state and optionally invalidates counters and optionally raises <see cref="Table.DisplayElementChanging"/>
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

            if (ParentTable == null)
            {
                isExpanded = value;
            }
            else if (IsExpanded != value)
            {
                foreach (NestedTable t in NestedTables)
                {
                    if (t.ChildTable == null)
                    {
                        if (warnOnChildTableNullReference)
                        {
                            throw new Exception("A RelationDescriptor.MappingName could not be resolved.");
                        }
                    }
                }

                bool success;
                if (value)
                {
                    success = ParentTable.RaiseRecordExpanding(this, raiseDisplayElementChangeEvents);
                }
                else
                {
                    success = ParentTable.RaiseRecordCollapsing(this, raiseDisplayElementChangeEvents);
                }

                if (!success)
                {
                    return;
                }

                isExpanded = value;

                if (refreshCounters)
                {
                    this.InvalidateCounterTopDown(false);
                    this.InvalidateCounterBottomUp(); // this also calls NestedTableEntry, not just GroupEntry for nested tables.
                }

                ParentTable.ClearCollectionCaches();

                if (value)
                {
                    ParentTable.RaiseRecordExpanded(this, raiseDisplayElementChangeEvents);
                }
                else
                {
                    ParentTable.RaiseRecordCollapsed(this, raiseDisplayElementChangeEvents);
                }
            }
        }

        /// <summary>
        /// Detemines if record has nested tables.
        /// </summary>
        public bool HasNestedTables
        {
            get
            {
                return this.ParentTableDescriptor.Relations.NestedCount > 0;
            }
        }

        internal void SetExpandedInternal(bool value)
        {
            isExpanded = value;
        }

        /// <summary>Gets the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return MeetsFilterCriteria() ? 1 : 0;
        }

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 1;
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <param name="notifyCounterSource">Indicates whether to notify the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            this.InvalidateCounter();
            if (recordPartEntries != null)
            {
                recordPartEntries.InvalidateCounterTopDown(notifyCounterSource);
            }
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <override/>
        public override void InvalidateCounterBottomUp()
        {
            invalidateCounterBottomUpCalled = true;
            ////if(ParentTable.hasGroupSortOrderEntry)
            ////{
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
            ////}
            base.InvalidateCounterBottomUp();
        }

        /// <summary>
        /// This virtual method is called from <see cref="OnEnsureInitialized"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="EnsureInitialized"/> call.</param>
        /// <returns>
        /// True if changes were detected and the object was updated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            if (this.HasNestedTables)
            {
                AdjustRecordRowCount();
            }

            return base.OnEnsureInitialized(sender);
        }

        /// <summary>Reset the summaries for all the elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            if (recordPartEntries != null)
            {
                recordPartEntries.InvalidateSummariesTopDown();
            }
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>
        /// Returns a concatenated string with fields and their values in the record.
        /// </summary>
        /// <returns>String with debug information.</returns>
        public string FieldsToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("({0}): ", GetType().Name);
            if (ParentTable == null)
            {
                sb.Append(" Disposed.");
            }
            else
            {
                bool first = true;
                foreach (FieldDescriptor fd in ParentTable.TableDescriptor.Fields)
                {
                    if (true)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(", ");
                        }

                        sb.AppendFormat("{0} = {1}", fd.Name, GetValue(fd));
                    }
                }

                ////sb.AppendFormat(", {0} = {1}", "##this", GetValue("##this"));
                sb.AppendFormat(", {0} = {1}", "##parent", GetValue("##parent"));
            }

            return sb.ToString();
        }

        /// <summary>Returns a string holding the record data.</summary>
        /// <returns>String representation of the record object.</returns>
        /// <override/>
        public override string ToString()
        {
            return FieldsToString();
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.BeginEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnBeginEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnBeginEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EndEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnEndEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnEndEditComplete(bool success)
        {
            if (success)
            {
                this.InvalidateCounterTopDown(true);
            }
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.CancelEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnCancelEditCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnCancelEditComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.LeaveRecord"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnLeaveRecordCalled()
        {
            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.LeaveRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnLeaveRecordComplete(bool success)
        {
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EnterRecord"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnEnterRecordCalled()
        {
            ////            object data = GetData();
            ////            if (data is System.Data.DataRowView)
            ////                data = ((System.Data.DataRowView) data).Row;
            ////
            ////            if (data is System.Data.DataRow)
            ////                return ((System.Data.DataRow) data).RowState != System.Data.DataRowState.Detached;

            return true;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EnterRecord"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnEnterRecordComplete(bool success)
        {
        }

        /// <overload>
        /// Returns a record from a related table with a RelationKind.ForeignKeyReference relation
        /// based on the foreign keys specified with RelationDescriptor.RelationKeys.
        /// </overload>
        /// <summary>
        /// Returns a record from a related table with a RelationKind.ForeignKeyReference relation
        /// based on the foreign keys specified with RelationDescriptor.RelationKeys and the values
        /// stored in this record's underlying data row.
        /// </summary>
        /// <param name="rd">The relation descriptor for relation keys.</param>
        /// <returns>The record from the related table.</returns>
        public Record GetRelatedRecord(RelationDescriptor rd)
        {
            int relationIndex = this.ParentTableDescriptor.Relations.IndexOf(rd.Name);
            if (relationIndex != -1)
            {
                Table relatedTable = this.ParentTable.RelatedTables[rd.Name];

                // Get foreign key values that identify record in related table
                object[] foreignKeyValues = new object[rd.RelationKeys.Count];
                for (int n = 0; n < rd.RelationKeys.Count; n++)
                {
                    RelationKeyDescriptor key = this.ParentTableDescriptor.Relations[relationIndex].RelationKeys[n];
                    foreignKeyValues[n] = GetValue(key.ParentKeyField);  // edited values have higher precedence
                    if (foreignKeyValues[n] == null)
                    {
                        return null;
                    }
                }

                // Search in related table.
                relatedTable.EnsureInitialized(this, false);
                int recordIndex = relatedTable.PrimaryKeySortedRecords.FindRecord(foreignKeyValues);
                if (recordIndex != -1)
                {
                    return relatedTable.PrimaryKeySortedRecords[recordIndex];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a record from a related table with a RelationKind.ForeignKeyReference relation
        /// based on the foreign keys specified with RelationDescriptor.RelationKeys. This overload lets
        /// you dynamically specify the foreign key value as a parameter.
        /// </summary>
        /// <param name="rd">The relation descriptor for relation keys.</param>
        /// <param name="foreignKeyField">The field descriptor for which a value should be specified manually (and
        /// not be retrieved from underlying records data row).</param>
        /// <param name="foreignKeyValue">The foreign key value.</param>
        /// <returns>The record from the related table.</returns>
        public Record GetRelatedRecord(RelationDescriptor rd, FieldDescriptor foreignKeyField, object foreignKeyValue)
        {
            int relationIndex = this.ParentTableDescriptor.Relations.IndexOf(rd.Name);
            if (relationIndex != -1)
            {
                Table relatedTable = this.ParentTable.RelatedTables[rd.Name];

                // Get foreign key values that identify record in related table.
                object[] foreignKeyValues = new object[rd.RelationKeys.Count];
                for (int n = 0; n < rd.RelationKeys.Count; n++)
                {
                    RelationKeyDescriptor key = this.ParentTableDescriptor.Relations[relationIndex].RelationKeys[n];
                    if (foreignKeyField == key.ParentKeyField)
                    {
                        foreignKeyValues[n] = foreignKeyValue;
                    }
                    else
                    {
                        foreignKeyValues[n] = GetValue(key.ParentKeyField);  // edited values have higher precedence
                    }

                    // 4.1.0.21: Do not check for null value - this could be a valid key, e.g. key = null, display = <select>
                    // if (foreignKeyValues[n] == null)
                    //     return null;
                }

                // Search in related table.
                relatedTable.EnsureInitialized(this, false);
                int recordIndex = relatedTable.PrimaryKeySortedRecords.FindRecord(foreignKeyValues);
                if (recordIndex != -1)
                {
                    return relatedTable.PrimaryKeySortedRecords[recordIndex];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a ChildTable from a related table with a RelationKind.ForeignKeyKeyWords relation
        /// based on the foreign keys specified with RelationDescriptor.RelationKeys.
        /// </summary>
        /// <param name="rd">The relation descriptor for relation keys.</param>
        /// <returns>The ChildTable from the related table.</returns>
        public ChildTable GetRelatedChildTable(RelationDescriptor rd)
        {
            ChildTable childTable = null;
            int relationIndex = this.ParentTableDescriptor.Relations.IndexOf(rd.Name);
            if (relationIndex != -1)
            {
                Table relatedTable = this.ParentTable.RelatedTables[rd.Name];

                // Relation with foreign / primary key pairs.
                TableDescriptor tableDescriptor = relatedTable.ParentTableDescriptor;

                // Gather foreign keys from current record.
                RelationChildColumnDescriptorCollection relatedChildColumns = relatedTable.ParentTableDescriptor.RelationChildColumns;
                int relatedChildColumnsCount = relatedChildColumns.Count;
                object[] categoryKeys = new object[relatedChildColumnsCount];
                for (int n = 0; n < relatedChildColumnsCount; n++)
                {
                    FieldDescriptor cd = relatedChildColumns[n].ParentFieldDescriptor;
                    if (cd != null)
                    {
                        categoryKeys[n] = this.GetValue(cd);
                    }
                    else
                    {
                        categoryKeys[n] = this.GetValue(relatedChildColumns[n].ParentColumnName);
                    }
                }

                childTable = relatedTable.AddChildTableIfNotExists(categoryKeys);
            }

            return childTable;
        }                                // ForeignListItems

        /// <summary>
        /// For internal use.
        /// </summary>       
        /// <returns>returns ParentRecord</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Record GetParentRecord(Element el)
        {
            if (el is Record)
            {
                return (Record)el;
            }
            else if (el != null)
            {
                return el.ParentRecord;
            }

            return null;
        }

        /// <summary>
        /// Adjusts the record row count.
        /// </summary>
        /// <returns>returns Record RowCount</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal bool AdjustRecordRowCount()
        {
            if (added)
            {
                Trace.WriteLineIf(Switches.GroupingEngine.TraceVerbose, "Record Added - AdjustRecordRowCount");
            }

            added = false;
            if (recordRowCountChanged)
            {
                if (!this.ShouldCreateChildElementTreeTable())
                {
                    recordRowCountChanged = false;
                    return false;
                }

                this.InvalidateCounter();
                recordRowCountChanged = false;

                bool isAddNew = this is AddNewRecord;
                System.Data.DataRowView drv = GetData() as System.Data.DataRowView;
                if (drv != null && (drv.Row.RowState == System.Data.DataRowState.Deleted || (!isAddNew && drv.Row.RowState == System.Data.DataRowState.Detached)))
                {
                    return false;
                }

                // Add summary rows on demand - this is triggered when InvalidateCounterTopDown is called on Table
                // see GridTable.RecordRows_Changed: CountersDirty = True;
                Table table = this.ParentTable;
                if (table != null)
                {
                    int rowCount = table.TableDescriptor.RowsPerRecord;
                    if (RecordRows.Count != rowCount)
                    {
                        RecordRows.Clear();

                        while (RecordRows.Count < rowCount)
                        {
                            RecordRow row = Engine.CreateRecordRow(this.RecordParts[ROWS] as RecordRowsPart);
                            RecordRows.Add(row);
                        }
                    }

                    int previewRowCount = table.TableDescriptor.PreviewRowsPerRecord;
                    if (RecordPreviewRows.Count != previewRowCount)
                    {
                        RecordPreviewRows.Clear();

                        while (RecordPreviewRows.Count < previewRowCount)
                        {
                            RecordPreviewRow row = Engine.CreateRecordPreviewRow(this.RecordParts[PREVIEW] as RecordPreviewRowsPart);
                            RecordPreviewRows.Add(row);
                        }
                    }

                    int relCount = 0;
                    if (!(this is AddNewRecord))
                    {
                        relCount = table.RelatedTables.Count;  // Note: also includes foreign key relations!
                    }

                    int targetRelationIndex = 0;
                    for (int i = 0; i < relCount; i++)
                    {
                        Table relatedTable = table.RelatedTables[i];
                        RelationDescriptor rd = this.ParentTableDescriptor.Relations[i];

                        //// Skip ForeignKeyReference since that only represents addition Field Values
                        if (rd.RelationKind == RelationKind.ForeignKeyReference
                            || rd.RelationKind == RelationKind.ListItemReference
                            || rd.RelationKind == RelationKind.ForeignKeyKeyWords)
                        {
                            continue;            // ForeignListItems
                        }

                        //// TopLevelgroup should be ChildTable

                        ChildTable childTable = null;
                        ChildTable topLevel = relatedTable.TopLevelGroup;

                        NestedTable nestedTable;
                        if (targetRelationIndex < NestedTables.Count)
                        {
                            nestedTable = NestedTables[targetRelationIndex];
                        }
                        else
                        {
                            nestedTable = Engine.CreateNestedTable((RecordNestedTablesPart)this.RecordParts[NESTED]);
                            NestedTables.Add(nestedTable);
                        }

                        targetRelationIndex++;

                        RelationChildColumnDescriptorCollection relatedChildColumns = relatedTable.ParentTableDescriptor.RelationChildColumns;

                        int relatedChildColumnsCount = relatedChildColumns.Count;
                        if (relatedChildColumnsCount == 0)
                        {
                            if (Engine.UseOldUniformChildListRelation)
                            {
                                #region UseOldUniformChildListRelation
                                // This is the case for:
                                // 1) nested strong typed collections
                                // 2) DataTables with in a DataSet
                                // 3) DataView that are related in a dataset and user specified UniformChildList
                                //    so that RelatedView object are used instead of engines own key pairing (RelationChildColumns).
                                //
                                // In this case there is no foreign key relation ship with the related table. The whole
                                // table is nested within the parent and consists only of one ChildTable (the TopLevelGroup).
                                if (rd.MappingName != string.Empty)
                                {
                                    ////rd.RelationKind == RelationKind.PolymorphChildList
                                    if (rd.RelationKind == RelationKind.UniformChildList ||
                                        rd.RelationKind == RelationKind.RelatedMasterDetails)
                                    {
                                        PropertyDescriptor pd = this.ParentTableDescriptor.ItemProperties[rd.MappingName];
                                        if (pd != null)
                                        {
                                            childTable = nestedTable.ChildTable;
                                            object sourceList = null;

                                            // Reuse existing related view, avoid pd.GetValue creating new RelatedView objects repeatedly.
                                            if (rd.RelationKind == RelationKind.UniformChildList
                                                && childTable != null
                                                && rd.AllowCacheChildList)
                                            {
                                                sourceList = nestedTable.ChildTable.ParentTable.SourceList;
                                            }

                                            if (sourceList == null)
                                            {
                                                // Special case for DataTables in a DataSet (which has only one record with no columns but DataTablePropertyDescriptor(s)
                                                if (pd.GetType().FullName == "System.Data.DataTablePropertyDescriptor")
                                                {
                                                    sourceList = Engine.SourceListSet[rd.MappingName].List;
                                                }
                                                else
                                                {
                                                    // A RelatedView in a DataSet (creates a new RelatedView)
                                                    sourceList = pd.GetValue(this.GetData());
                                                }

                                                if (sourceList == null || sourceList is DBNull)
                                                {
                                                    sourceList = Activator.CreateInstance(pd.PropertyType);
                                                }

                                                Table childParentTable = null;
                                                if (sourceList != null && !(sourceList is DBNull))
                                                {
                                                    if (nestedTable.ChildTable == null)
                                                    {
                                                        childParentTable = this.ParentTableDescriptor.CreateRelatedTable(relatedTable.TableDescriptor, this.ParentTable);
                                                    }
                                                    else
                                                    {
                                                        childParentTable = nestedTable.ChildTable.ParentTable;
                                                    }

                                                    childParentTable.SourceList = (IList)sourceList;
                                                    childTable = childParentTable.TopLevelGroup;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // TODO: Obsolete?
                                        // Add support for ListItemReference (similar to ForeignKeyReference).
                                        PropertyDescriptor pd = this.ParentTableDescriptor.ItemProperties[rd.MappingName];
                                        if (pd != null)
                                        {
                                            object sourceList = pd.GetValue(this.GetData());
                                            Table childParentTable;
                                            if (nestedTable.ChildTable == null)
                                            {
                                                childParentTable = this.ParentTableDescriptor.CreateRelatedTable(relatedTable.TableDescriptor, this.ParentTable);
                                            }
                                            else
                                            {
                                                childParentTable = nestedTable.ChildTable.ParentTable;
                                            }

                                            childParentTable.SourceList = new object[] { sourceList };
                                            childTable = childParentTable.TopLevelGroup;
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                // New in version 4.2: Add a relation key that maps parent record to child record similar to 
                                // a master-details relation.

                                // Relation with foreign / primary key pairs.
                                TableDescriptor tableDescriptor = relatedTable.ParentTableDescriptor;

                                // Gather foreign keys from current record.
                                object[] categoryKeys = new object[] { this };

                                // Look up ChildTable in related tables TopLevel group.                              
                             
                                GroupsDetails detailSectionWithGroups = null;
                                if (topLevel.Details is RecordsDetails)
                                {                                    
                                    detailSectionWithGroups = ((RecordsDetails)topLevel.Details).ParentGroup.Details as GroupsDetails;
                                }
                                else
                                {
                                    detailSectionWithGroups = (GroupsDetails)topLevel.Details;
                                }

                                GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
                                GroupCategoryTreeTableEntry entry = (GroupCategoryTreeTableEntry)detailSectionWithGroups.GroupCategoryTreeTable.TreeTable.AddIfNotExists(categoryKeys, childNodeEntry);

                                if (entry != childNodeEntry)
                                {
                                    // Entry was found.
                                    childTable = (ChildTable)entry.Element;
                                }
                                else
                                {
                                    //// Add an empty group with a AddNewRecord.
                                    childTable = relatedTable.AddEmptyChildTableWithGroups(childNodeEntry, categoryKeys);
                                    //// Fixes issue with incident 15234, grid crashed when record was added
                                    //// with following settings:
                                    //// this.grd.TopLevelGroupOptions.ShowAddNewRecordAfterDetails = true;
                                    //// this.grd.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                                    childTable.GroupCategoryEntry.InvalidateCounterBottomUp(true);
                                    ////relatedTable.TopLevelGroup.SectionEntries.InvalidateCounterTopDown(false);
                                }

                                //// Now that we have a valid group that is inside the related table, we can add it as nested table.
                            }

                            nestedTable.ChildTable = childTable;
                        }
                        else
                        {
                            if (relatedTable.AllowInitChildTableInAdjustRecordRowCount)
                            {
                                //// Relation with foreign / primary key pairs.
                                TableDescriptor tableDescriptor = relatedTable.ParentTableDescriptor;

                                //// Gather foreign keys from current record.
                                object[] categoryKeys = new object[relatedChildColumnsCount];
                                for (int n = 0; n < relatedChildColumnsCount; n++)
                                {
                                    FieldDescriptor cd = relatedChildColumns[n].ParentFieldDescriptor;
                                    if (cd != null)
                                    {
                                        categoryKeys[n] = this.GetValue(cd);
                                    }
                                    else
                                    {
                                        categoryKeys[n] = this.GetValue(relatedChildColumns[n].ParentColumnName);
                                    }
                                }

                                // Look up ChildTable in related tables TopLevel group.
                                GroupsDetails detailSectionWithGroups = (GroupsDetails)topLevel.Details;
                                GroupCategoryTreeTableEntry childNodeEntry = new GroupCategoryTreeTableEntry();
                                GroupCategoryTreeTableEntry entry = (GroupCategoryTreeTableEntry)detailSectionWithGroups.GroupCategoryTreeTable.TreeTable.AddIfNotExists(categoryKeys, childNodeEntry);

                                if (entry != childNodeEntry)
                                {
                                    //// Entry was found.
                                    childTable = (ChildTable)entry.Element;
                                }
                                else
                                {
                                    //// Add an empty group with a AddNewRecord.
                                    childTable = relatedTable.AddEmptyChildTableWithGroups(childNodeEntry, categoryKeys);
                                    if (relatedTable.IsNewUniformChildListRelation())
                                    {
                                        //// New in version 4.2: Associate source list of nested collection
                                        //// with newly creaed child table

                                        string mappingName = rd.MappingName;
                                        string relationName = rd.Name;
                                        PropertyDescriptor pd = this.ParentTableDescriptor.ItemProperties[mappingName];
                                        object data = GetData();
                                        object obj = pd.GetValue(GetData());
                                        while (obj is IListSource && ((IListSource)obj).ContainsListCollection)
                                        {
                                            obj = ((IListSource)obj).GetList();
                                        }

                                        IEnumerable list = obj as IEnumerable;
                                        childTable.SourceList = relatedTable.GetIListWrapper(list);
                                        if (childTable.SourceList == null)
                                        {
                                            data = GetData();
                                        }
                                    }
                                    //// Fixes issue with incident 15234, grid crashed when record was added
                                    //// with following settings:
                                    //// this.grd.TopLevelGroupOptions.ShowAddNewRecordAfterDetails = true;
                                    //// this.grd.TopLevelGroupOptions.ShowAddNewRecordBeforeDetails = false;
                                    childTable.GroupCategoryEntry.InvalidateCounterBottomUp(true);
                                    ////relatedTable.TopLevelGroup.SectionEntries.InvalidateCounterTopDown(false);
                                }

                                //// Now that we have a valid group that is inside the related table, we can add it as nested table.

                                nestedTable.ChildTable = childTable;
                            }
                        }

                        visibleCount = -1;
                        yAmountCount = -1;
                        filterState = -1;
                        ////TraceUtil.TraceCurrentMethodInfo(this, childTable);
                    }

                    for (int relationIndex = NestedTables.Count - 1; relationIndex >= targetRelationIndex; relationIndex--)
                    {
                        NestedTables.RemoveAt(relationIndex);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            TableDescriptor td = ParentTable.TableDescriptor;
            if (td.Relations.Count == 0 || !this.IsExpanded)
            {
                return td.RowsPerRecord + td.PreviewRowsPerRecord;
            }

            AdjustRecordRowCount();
            if (TreeEntries == null)
            {
                return 1;
            }

            return TreeEntries.ElementCount;
        }

        /// <summary>
        /// Gets the collection of <see cref="RecordRow"/> elements that belong to this record.
        /// </summary>
        public RecordRowCollection RecordRows
        {
            get
            {
                RecordRowsPart rr = this.RecordParts[ROWS] as RecordRowsPart;
                return rr.RecordRows;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="RecordPreviewRow"/> elements that belong to this record.
        /// </summary>
        public RecordPreviewRowCollection RecordPreviewRows
        {
            get
            {
                RecordPreviewRowsPart rr = this.RecordParts[PREVIEW] as RecordPreviewRowsPart;
                return rr.RecordPreviewRows;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="NestedTable"/> elements that belong to this record.
        /// </summary>
        public NestedTablesCollection NestedTables
        {
            get
            {
                RecordNestedTablesPart rr = this.RecordParts[NESTED] as RecordNestedTablesPart;
                return rr.NestedTables;
            }
        }

        /// <summary>Gets the display element for the record.</summary>
        /// <returns>Display element.</returns>
        /// <exclude/>
        public Element GetRecordDisplayElement()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return this;
            }

            return this.RecordParts[ROWS];
        }

        internal new RecordPartsTreeTable TreeEntries
        {
            get
            {
                if (!this.ShouldCreateChildElementTreeTable())
                {
                    return null;
                }

                this.AdjustRecordRowCount();
                return this.recordPartEntries;
            }
        }

        #region IComparable Members
        
        /// <summary>
        /// Compares to records unsorted source position.
        /// </summary>
        /// <param name="obj">Record object to compare.</param>
        /// <returns>0 if both are equal, ;gt 0 if current record has higher index, ;lt 0 if given record has higher index.</returns>
        public int CompareTo(object obj)
        {
            Record r = obj as Record;
            if (r == null)
            {
                return 1;
            }
            else
            {
                return GetSourceIndex() - r.GetSourceIndex();
            }
        }

        #endregion

        /// <summary>
        /// Ensures that record values are cached. The method is implemented only
        /// in the RecordWithValueCache class.
        /// </summary>
        public virtual void EnsureValues()
        {
        }

        /// <summary>
        /// Resets cached values. The method is implemented only
        /// in the RecordWithValueCache class. You should follow this
        /// call by a call to EnsureValues.
        /// </summary>
        public virtual void ResetValues()
        {
        }

        /// <summary>
        /// Returns the old value for a record. The record class will
        /// look up changed values in the Table.ChangeFieldsArray
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>Old value of the record.</returns>
        public virtual object GetOldValue(int fieldIndex)
        {
            ////using (MeasureTime.Measure("GroupingRecord.GetOldValue"))
            {
                Table tb = ParentTable;
                if (tb.listChangedRecordIndex == this.GetSourceIndex()
                    && tb.changedFields.Contains(fieldIndex))
                {
                    ChangedFieldInfo ci = (ChangedFieldInfo)tb.changedFields[fieldIndex];
                    FieldDescriptor fd = ParentTableDescriptor.Fields[ci.FieldIndex];
                    Type type = fd.GetPropertyType();

                    object value = ci.OldValue;
                    if (type != null && value != null && !(value is DBNull))
                    {
                        value = NullableHelper.ChangeType(ci.OldValue, type);
                    }

                    return value;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns an ArrayList with ChangedFieldInfo objects and updates
        /// the values in the record with changes found in underylying datasource.
        /// Only fields with a PropertyDescriptor are updated, others (unbound, expression fields) are ignored. The method is implemented only
        /// in the RecordWithValueCache class.
        /// </summary>
        /// <returns>The collection with detected changes.</returns>
        public virtual ChangedFieldInfoCollection CompareAndUpdateValues()
        {
            return new ChangedFieldInfoCollection();
        }

        /// <summary>
        /// Enumerates through values in the collection of ChangedFieldInfo objects
        /// and updates the old and new values in this record. The method is implemented only
        /// in the RecordWithValueCache class.
        /// </summary>
        /// <param name="changedFields">The changed fields.</param>
        public virtual void UpdateValues(IEnumerable changedFields)
        {
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns YAmountCount</returns>
        /// <exclude/>
        public double GetInternalYAmountCount()
        {
            Table parentTable = ParentTable;
            if (parentTable == null)
            {
                return 0;
            }

            TableDescriptor td = parentTable.TableDescriptor;
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return parentTable.DefaultRecordRowHeight;
            }

            double yCount = 0;
            foreach (RecordPart s in this.RecordParts)
            {
                if (this.IsChildVisible(s))
                {
                    yCount += s.GetYAmountCount();
                }
            }

            return yCount;
        }

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <returns>returns Internal Visible Count</returns>
        /// <exclude/>
        public int GetInternalVisibleCount()
        {
            if (!this.ShouldCreateChildElementTreeTable())
            {
                return 1;
            }

            int visCount = 0;

            foreach (RecordPart s in this.RecordParts)
            {
                if (this.IsChildVisible(s))
                {
                    visCount += s.GetVisibleCount();
                }
            }

            return visCount;
        }
    }

    /// <summary>
    /// Record that shadows values in table and can gives hints which values were changed in ListChanged event. The
    /// new GetOldValue method also gives access to the previous value before a change and can be used for
    /// calculating the difference between two values.
    /// </summary>
    public class RecordWithValueCache : Record
    {
        object[] fieldValues;
        object[] oldValues;

        /// <summary>
        /// Constructor for RecordWithValueCache.
        /// </summary>
        /// <param name="parentTable">Parent table.</param>
        public RecordWithValueCache(Table parentTable)
            : base(parentTable)
        {
        }

        /// <summary>
        /// Resets cached values.
        /// </summary>
        /// <override/>
        public override void ResetValues()
        {
            fieldValues = null;
        }

        /// <summary>
        /// Ensures that record values are cached. The method is implemented only
        /// in the RecordWithValueCache class.
        /// </summary>
        /// <override/>
        public override void EnsureValues()
        {
            ////using (MeasureTime.Measure("GroupingRecord.EnsureValues"))
            {
                if (fieldValues == null)
                {
                    FieldDescriptorCollection fc = ParentTableDescriptor.Fields;
                    int fcCount = fc.Count;

                    this.fieldValues = null;

                    object[] values = new object[fcCount];
                    object[] oldValues = new object[fcCount];

                    for (int n = 0; n < fcCount; n++)
                    {
                        FieldDescriptor fd = fc[n];
                        object value = null;
                        if (fd.IsPropertyField())
                        {
                            value = base.GetValue(fd);
                        }

                        values[n] = value;
                        oldValues[n] = value;
                    }

                    this.fieldValues = values;
                    this.oldValues = oldValues;
                }
            }
        }

        /// <summary>
        /// Gets the value from the underlying datasource.
        /// </summary>
        /// <param name="fieldDescriptor">The field to be retrieved.</param>
        /// <returns>returns Value.</returns>
        /// <override/>
        public override object GetValue(FieldDescriptor fieldDescriptor)
        {
            ////using (MeasureTime.Measure("GroupingRecord.GetValue"))
            {
                if (fieldValues == null || IsCurrent || !fieldDescriptor.IsPropertyField())
                {
                    return base.GetValue(fieldDescriptor);
                }

                FieldDescriptorCollection fc = ParentTableDescriptor.Fields;
                return fieldValues[fc.IndexOf(fieldDescriptor)];
            }
        }

        /// <summary>
        /// Returns the old value for a record.
        /// </summary>
        /// <param name="fieldIndex">Field index.</param>
        /// <returns>Old value of the record.</returns>
        /// <override/>
        public override object GetOldValue(int fieldIndex)
        {
            ////using (MeasureTime.Measure("GroupingRecord.GetOldValue"))
            {
                if (oldValues != null)
                {
                    return oldValues[fieldIndex];
                }

                return base.GetOldValue(fieldIndex);
            }
        }

        /// <summary>
        /// Returns an ArrayList with ChangedFieldInfo objects and updates
        /// the values in the record with changes found in underylying datasource.
        /// Only fields with a PropertyDescriptor are updated, others (unbound, expression fields) are ignored. The method is implemented only
        /// in the RecordWithValueCache class.
        /// </summary>
        /// <returns>The collection with detected changes.</returns>
        /// <override/>
        public override ChangedFieldInfoCollection CompareAndUpdateValues()
        {
            ////using (MeasureTime.Measure("GroupingRecord.CompareAndUpdateValues"))
            {
                if (fieldValues != null)
                {
                    ChangedFieldInfoCollection changedFields = new ChangedFieldInfoCollection();
                    FieldDescriptorCollection fields = ParentTableDescriptor.Fields;
                    for (int n = 0; n < fieldValues.Length; n++)
                    {
                        FieldDescriptor fd = fields[n];
                        object value = fd.IsPropertyField() ? base.GetValue(fd) : null;
                        bool isEqual = Object.ReferenceEquals(fieldValues[n], value);
                        if (!isEqual && fieldValues[n] != null && value != null)
                        {
                            isEqual |= fieldValues[n].Equals(value);
                        }

                        if (!isEqual)
                        {
                            changedFields.Add(new ChangedFieldInfo(this.ParentTableDescriptor, fd.Name, value, fieldValues[n]));
                        }

                        oldValues[n] = fieldValues[n];
                        fieldValues[n] = value;
                    }

                    return changedFields;
                }

                return null;
            }
        }

        /// <summary>
        /// Enumerates through values in the collection of ChangedFieldInfo objects
        /// and updates the old and new values in this record.
        /// </summary>
        /// <param name="changedFields">The changed fields.</param>
        /// <override/>
        public override void UpdateValues(IEnumerable changedFields)
        {
            if (fieldValues == null)
            {
                return;
            }

            foreach (ChangedFieldInfo ci in changedFields)
            {
                if (ci.HasValue)
                {
                    FieldDescriptor fd = ParentTableDescriptor.Fields[ci.FieldIndex];
                    Type type = fd.GetPropertyType();

                    oldValues[ci.FieldIndex] = ci.OldValue;
                    fieldValues[ci.FieldIndex] = ci.NewValue;

                    if (type != null)
                    {
                        if (ci.OldValue != null && !(ci.OldValue is DBNull))
                        {
                            oldValues[ci.FieldIndex] = NullableHelper.ChangeType(ci.OldValue, type);
                        }

                        if (ci.NewValue != null && !(ci.NewValue is DBNull))
                        {
                            fieldValues[ci.FieldIndex] = NullableHelper.ChangeType(ci.NewValue, type);
                        }
                    }
                }
            }
        }
    }
}

