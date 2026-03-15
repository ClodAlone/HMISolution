//-------------------------------------------------------------------------------------------------
// <copyright file="AddNewRecord.cs" company="syncfusion">
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
using System.Collections.Generic;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// The section with the AddNewRecord:
    /// </summary>
    /// <remarks>
    /// This is the section within a group which provides the AddNewRecord that is shown above the
    /// table records and / or below the records for each group and implements logic to add new records.
    /// See the AddNewRecord class for more information.<para/>
    /// <para/>
    /// The AddNewRecordSection is a RecordsDetails section and thus also a container element. Normally,
    ///  a RecordsDetails section has a collection of one or multiple records but with a
    ///  AddNewRecordSection you only have exactly one child record, the AddNewRecord.<para/>
    /// <para/>
    /// If AddNewRecord(s) should be displayed at end of each group, then AddNewRecordSection(s) are
    /// created when the grouping for a table is initialized. The grouping engine loops through all
    /// sorted records and categorizes them. For each new group the virtual TableDescriptor.CreateGroup
    /// factory method is called. The TableDescriptor.CreateGroup method instantiates an AddNewRecordSection
    /// by calling the virtual TableDescriptor.CreateAddNewRecordSection factory method. <para/>
    /// <para/>
    /// One AddNewRecord is displayed at the top of the table below the column headers. It belongs
    /// to the AddNewRecordSection of the TopLevelGroup of a table. The TopLevelGroup is created
    /// with the virtual TableDescriptor.CreateGroup factory method. The TableDescriptor.CreateGroup
    /// method instantiates an AddNewRecordSection by calling the virtual
    /// TableDescriptor.CreateAddNewRecordSection factory method.<para/>
    /// <para/>
    /// Because the AddNewRecordSection is a container element and not a display element, it will not
    /// be an item returned by the Table.DisplayElements and Table.GroupedElements collection. Instead,
    /// AddNewRecordSection can be accessed only through the Group.Sections collection of its parent
    /// Group.<para/>
    /// <para/>
    /// See also:<para/>
    /// TableDescriptor.CreateGroup, TableDescriptor.CreateAddNewRecordSection, Table.TopLevelGroup,
    /// AddNewRecord, IContainerElement, Group.Sections.
    /// </remarks>
    public class AddNewRecordSection : RecordsDetails
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public AddNewRecordSection(Group parent)
            : base(parent)
        {
            // Add one AddNewRecord into element tree.
            recordsTree.BeginInit();
            InsertAddNewRecord();
            recordsTree.EndInit();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is the AddNewRecord that is shown above the DetailsSection
        /// or below.
        /// </summary>
        public bool IsBeforeDetails
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

        /// <summary>
        /// Returns reference to AddNewRecord.
        /// </summary>
        public AddNewRecord AddNewRecord
        {
            get
            {
                return this.Records.Count > 0 ? this.Records[0] as AddNewRecord : null;
            }
        }

        void InsertAddNewRecord()
        {
            AddNewRecord r = Engine.CreateAddNewRecord(this);
            SortedRecordsTreeTableEntry sortedEntry = new SortedRecordsTreeTableEntry();
            sortedEntry.Tree = RecordTreeEntries.TreeTable;
            sortedEntry.Element = r;
            r.SortedEntry = sortedEntry;
            r.ParentElement = this;
            recordsTree.Add(sortedEntry);
            ////if (ParentTable != null)
            ////ParentTable.ClearCollectionCaches();
        }

        /// <summary>Returns string representation of AddNewRecord.</summary>
        /// <returns>A string holding the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            if (AddNewRecord != null)
            {
                return GetType().Name + " " + AddNewRecord.FieldsToString();
            }
            else
            {
                return GetType().Name;
            }
        }
    }

    /// <summary>
    /// A record element for adding new records to the underlying datasource.
    /// This element represents the empty record that is shown above the table records and / or below
    /// the records for each group and implements logic to add new records.
    /// </summary>
    /// <remarks>
    /// This element represents the empty record that is shown above the table records and / or below
    /// the records for each group and implements logic to add new records. <para/>
    /// <para/>
    /// The class is derived from Record. It adds special behavior such that the AddNewRecord is
    /// not bound to a record in the underlying table. Instead all fields are initially empty (
    /// or default values based on parent group by criteria). When changes are made, the changes
    /// will be committed to the underlying table by adding a new record at the end of the
    /// underlying data source. The grouping engine will automatically insert the record into
    /// its sorted records collection and insert it into the group it belongs to. If no group
    /// is found where the record fits in, a new group will be created on the fly.<para/>
    /// <para/>
    /// AddNewRecord(s) can be optionally displayed at the start of table below the column headers
    /// and / or at the end of each group. <para/>
    /// <para/>
    /// AddNewRecord belongs to an AddNewRecordSection and is its only element.<para/>
    /// <para/>
    /// If AddNewRecord(s) should be displayed at end of each group (see also Engine.ShowAddNewRecordInGroups),
    ///  then AddNewRecordSection(s) are created when the grouping for a table is initialized.
    ///  The grouping engine loops through all sorted records and categorizes them. For each new
    ///  group, the virtual TableDescriptor.CreateGroup factory method is called. The TableDescriptor.CreateGroup
    ///  method instantiates an AddNewRecordSection by calling the virtual TableDescriptor.CreateAddNewRecordSection
    ///  factory method. <para/>
    /// <para/>
    /// One AddNewRecord is displayed at the top of the table below the column headers. It belongs to
    /// the AddNewRecordSection of the TopLevelGroup of a table. The TopLevelGroup is created with the
    /// virtual TableDescriptor.CreateGroup factory method. The TableDescriptor.CreateGroup method
    /// instantiates an AddNewRecordSection by calling the virtual TableDescriptor.CreateAddNewRecordSection
    /// factory method. <para/>
    /// <para/>
    /// AddNewRecord is a container element. It contains one or multiple RecordParts. <para/>
    /// <para/>
    /// Because the AddNewRecord is a container element and not a display element, it will not be an
    /// item returned by the Table.DisplayElements and Table.GroupedElements collection. Instead,
    /// AddNewRecord is only returned by the RecordsDetails.Records collection of its parent
    /// AddNewRecordSection (or the Group.Records collection of its ParentGroup which is a shortcut
    /// instead of accessing it through AddNewRecordSection). <para/>
    /// <para/>
    /// See also the Engine.RecordAsDisplayElements how to make AddNewRecord be treated as a display
    /// element if you have no need for nested elements within a record., e.g. if you only want to
    /// display a single table and you do not need multiple row elements per record and / or nested tables.
    ///  (However, do not change this setting if you intend to use the engine as grid data model.)<para/>
    /// <para/>
    /// The first record part is a RecordRowsPart. A RecordRowsPart contains one or multiple RecordRows
    /// displayed in a grid for the record. <para/>
    /// <para/>
    /// The second record part represents nested tables. If relations have been defined for the table
    /// (see TableDescriptor.Relations) and the the current table has a foreign key relationship setup
    /// to other tables, a table is added to the RecordPart[1].NestedTables collection for each relation.
    /// You can access nested tables in a record with the Record.NestedTables collection.<para/>
    /// <para/>
    /// AddNewRecord can be set as the current record in a table if you want to edit its data through
    /// the engine.<para/>
    /// <para/>
    /// The first time a value is changed by the grid, Record.BeginEdit is called before the value is assigned.
    /// When the user moves the current cell in the grid to a new record, the changes are committed with a
    /// Record.EndEdit call. If the user presses Escape, the changes are rolled back with a Record.CancelEdit
    /// call. If the user presses Enter, the changes are committed and the record's new display position is
    /// determined and the current cell is moved to the new display position of the record. If validation
    /// fails during EndEdit, the grid will try to identify the column which violated the record constraints
    /// and display a red error indicator for that column with a ToolTip text showing the exception text
    /// when the user hovers the mouse over the red arrow indicator. <para/>
    /// <para/>
    /// See the CurrentRecordManager class for a description of events raised when modifying columns in the
    /// AddNewRecord row or when BeginEdit, CancelEdit or EndEdit was called.<para/>
    /// <para/>
    /// See also: CurrentRecordManager, CurrentRecordManager.IsAddNewRecord, CurrentRecordManager.NavigateTo,
    /// Group.AddNewRecord, Table.AddNewRecord, TableDescriptor.CreateGroup, Table.TopLevelGroup,
    /// AddNewRecordSection, IContainerElement, RecordsDetails.Records, Group.Records
    /// </remarks>
    /// <example>The following example shows how you can add record and keep track of its sorted
    /// display position within the table:
    /// <code lang="C#">
    ///             // Add a record to Table table2.
    ///             Record r = table2.AddNewRecord;
    ///             r.SetCurrent();   // Makes it the current record, saving any pending changes previous current record.
    ///             if (r.IsCurrent)
    ///             {
    ///                 r.BeginEdit();
    ///                 if (r.IsEditing)
    ///                 {
    ///                     r.SetValue("Country", "USA");
    ///                     r.SetValue("Region", "NC");
    ///                     r.SetValue("CustomerID", "SYNC
    ///                     r.SetValue("CompanyName", "Syncfusion");
    ///                     r.EndEdit();
    ///                 }
    ///             }
    /// <para/>
    ///             int displayPos = table.DisplayElements.IndexOf(r);
    ///             // Now, you can navigate the current cell of a grid to row displayPos and highlight the newly added row
    ///             // within its correct group category and sort order.
    /// </code>
    /// </example>
    public class AddNewRecord : Record
    {
        /// <summary>
        /// Initializes the new element in the specified AddNewRecordSection.
        /// </summary>
        /// <param name="parent">The parent that this element belongs to.</param>
        public AddNewRecord(AddNewRecordSection parent)
            : base(parent.ParentTable)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            this.data = null;
            base.Dispose(disposing);
        }

        /// <summary>Gets the display element kind.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get
            {
                return DisplayElementKind.AddNewRecord;
            }
        }

        /// <summary>Returns the record data.</summary>
        /// <returns>Record data.</returns>
        /// <override/>
        public override object GetData()
        {
            return data;
        }

        bool useDefaultValueInGetValue = false;

        /// <summary>
        /// Gets / sets if the AddNewRecord should retrieve the default value when its GetValue method is called.
        /// If UseDefaultValueInGetValue is false and also Engine.ShowDefaultValuesInAddNewRecord is false then
        /// the default values will only be assigned when AddNewRecord.BeginEdit is called. Prior calls to GetValue
        /// will return no value.
        /// </summary>
        public bool UseDefaultValueInGetValue
        {
            get { return useDefaultValueInGetValue; }
            set { useDefaultValueInGetValue = value; }
        }

        /// <summary>Returns the value for the field specified.</summary>
        /// <param name="fieldDescriptor">The descriptor of the field whose value is to be fetched.</param>
        /// <returns>Requested field value.</returns>
        /// <override/>
        public override object GetValue(FieldDescriptor fieldDescriptor)
        {
            if (fieldDescriptor != null)
            {
                if (IsEditing)
                {
                    return ParentTable.CurrentRecordManager.Properties[fieldDescriptor.Name].CurrentValue;
                }

                if (useDefaultValueInGetValue || Engine.ShowDefaultValuesInAddNewRecord)
                {
                    return GetDefaultValue(fieldDescriptor);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the default value for the specied field taking FieldDescriptor.DefaultValue and
        /// category keys of the child group into account.
        /// </summary>
        /// <param name="fieldDescriptor">The fieldDescriptor</param>
        /// <returns>returns the default value</returns>
        object GetDefaultValue(FieldDescriptor fieldDescriptor)
        {
            // check if this AddNewRecord belongs to a child group and if I can extract values
            // from the groups categories.
            AddNewRecordSection nsc = ParentSection as AddNewRecordSection;
            if (nsc != null)
            {
                Group g = nsc.ParentGroup;
                if (g != null)
                {
                    return g.GetDefaultValue(fieldDescriptor);
                }
            }

            return null;
        }

        /// <summary>Sets a value for the given field.</summary>
        /// <param name="fieldDescriptor">The field descriptor.</param>
        /// <param name="value">The Value.</param>
        /// <override/>
        public override void SetValue(FieldDescriptor fieldDescriptor, object value)
        {
            if (fieldDescriptor.ReadOnly)
            {
                throw new NotSupportedException(fieldDescriptor.Name + " is readonly.");
            }

            if (IsEditing)
            {
                ParentTable.CurrentRecordManager.Properties[fieldDescriptor.Name].ModifiedValue = value;
            }
            else
            {
                throw new InvalidOperationException("AddNew not called");
            }
        }

        /// <summary>
        /// Determines if the source list allows adding new record.
        /// </summary>
        /// <returns>True if new record is allowed; False otherwise.</returns>
        /// <override/>
        public override bool OnEnterRecordCalled()
        {
            return ParentTable.SourceListAllowNew;
        }

        /// <summary>
        /// Addes support for adding new records.
        /// </summary>
        /// <returns>True if editing new record can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnBeginEditCalled()
        {
            if (ParentTable.SourceListAllowNew)
            {
                // 1. Added support for adding records with PassThroughGrouping
                IPassThroughGroupingResult ptg = ParentTable.PassThroughGroupingResult;
                if (ptg != null)
                {
                    data = Activator.CreateInstance(ptg.GetListItemType());
                }
                else
                {
                    IList sourceList = ParentTable.SourceList;
                    if (sourceList == null)
                    {
                        sourceList = ParentChildTable.SourceList;
                        if (sourceList == null)
                        {
                            // 2. Also Added support for adding records to a nested collection
                            // in a UniformChildListRelation when the collection property in 
                            // the parent item is null. In such case I instantiate the collection
                            // and add an item to it. The collection is then assigned to
                            // the parent property setter in OnEndEdit.
                            Record parentRecord;
                            PropertyDescriptor pd = GetNestedCollectionProperty(ParentChildTable, out parentRecord);
                            if (pd != null && !pd.IsReadOnly)
                            {
                                sourceList = Activator.CreateInstance(pd.PropertyType) as IList;
                                if (sourceList != null)
                                {
                                    ParentChildTable.SourceList = sourceList;
                                }
                            }
                        }
                    }

                    IBindingList bindingList = sourceList as IBindingList;
                    if (bindingList != null && bindingList.AllowNew)
                    {
                        Type itemType = ListUtil.GetListItemType(sourceList);
#if SyncfusionFramework4_0
                        if (itemType == null && sourceList is BindingList<dynamic>)
                        {
                            itemType = typeof(System.Dynamic.ExpandoObject);
                            data = Activator.CreateInstance(itemType);
                            bindingList.Add(data);
                        }
                        else                        
#endif
                            data = bindingList.AddNew();
                    }
                    else
                    {
                        if (sourceList != null)
                        {
                            Type itemType = ListUtil.GetListItemType(sourceList);
#if SyncfusionFramework4_0
                            if (itemType == null && (sourceList is IList<dynamic> || sourceList is IList))
                            {
                                itemType = typeof(System.Dynamic.ExpandoObject);
                            }
#endif
                            if (itemType != null)
                            {
                                data = Activator.CreateInstance(itemType);
                            }
                        }
                    }
                }

                if (data == null)
                {
                    return false;
                }
            }

            return true;
        }

        private PropertyDescriptor GetNestedCollectionProperty(ChildTable parentChildTable, out Record parentRecord)
        {
            parentRecord = null;
            NestedTable nt = parentChildTable.ParentNestedTable;
            if (nt != null)
            {
                parentRecord = nt.ParentRecord;
                int index = parentRecord.NestedTables.IndexOf(nt);
                TableDescriptor ptd = parentRecord.ParentTableDescriptor;
                RelationDescriptor rd = ptd.Relations[index];
                if (rd != null && rd.RelationKind == RelationKind.UniformChildList)
                {
                    PropertyDescriptor pd = ptd.ItemProperties[rd.MappingName];
                    return pd;
                }
            }

            return null;
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.BeginEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnBeginEditComplete(bool success)
        {
            if (!success)
            {
                return;
            }

            // Initialize default values.
            foreach (FieldDescriptor fd in this.ParentTableDescriptor.Fields)
            {
                if (fd != null && !fd.ReadOnly)
                {
                    CurrentRecordProperty prop = ParentTable.CurrentRecordManager.Properties[fd.Name];
                    if (prop != null && !prop.IsModified)
                    {
                        try
                        {
                            object value = GetDefaultValue(fd);
                            if (value != null)
                            {
                                prop.ModifiedValue = value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when <see cref="CurrentRecordManager.CancelEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnCancelEditComplete(bool success)
        {
            if (success)
            {
                data = null;
            }

            base.OnCancelEditComplete(success);
        }

        /// <override/>
        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> is called.
        /// </summary>
        /// <returns>True if <see cref="CurrentRecordManager.EndEdit"/> can proceed; False if it should abort.</returns>
        /// <override/>
        public override bool OnEndEditCalled()
        {
            return base.OnEndEditCalled();
        }

        /// <override/>
        /// <summary>
        /// Called when <see cref="CurrentRecordManager.EndEdit"/> successfully finishes.
        /// </summary>
        /// <param name="success">True, if it is successfully finished; False, otherwise.</param>
        /// <override/>
        public override void OnEndEditComplete(bool success)
        {
            if (success)
            {
                // Here I check if a collection was created in OnBeginEditCalled.
                // In such case the parent item's property for the collection
                // is null and I'll assign the collection to the property setter
                // in this case.
                Record parentRecord;
                PropertyDescriptor pd = GetNestedCollectionProperty(ParentChildTable, out parentRecord);
                if (pd != null && !pd.IsReadOnly)
                {
                    object parentItem = parentRecord.GetData();
                    object sourceList = pd.GetValue(parentItem) as IList;
                    if (!Object.ReferenceEquals(sourceList, ParentChildTable.SourceList))
                    {
                        pd.SetValue(parentItem, ParentChildTable.SourceList);
                    }
                }

                data = null;
            }

            base.OnEndEditComplete(success);
        }

        /// <summary>Returns the number of filtered records.</summary>
        /// <returns>Number of filtered records.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Returns the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }

        /// <summary>Returns the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
        }
        
        /// <summary>
        /// Determines whether the record meets filter criteria. Returns True.
        /// </summary>
        /// <returns>Returns True.</returns>
        /// <override />
        public override bool MeetsFilterCriteria()
        {
            return true;
        }

        /// <summary>
        /// Returns False.
        /// </summary>
        /// <returns>Returns boolean value false.</returns>
        public override bool IsSelected()
        {
            return false;
        }

        /// <summary>
        /// Not supported for AddNewRecord.
        /// </summary>
        /// <param name="value">True if the record should be marked as selected.</param>
        public override void SetSelected(bool value)
        {
        }
    }
}
