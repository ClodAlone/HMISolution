//-------------------------------------------------------------------------------------------------
// <copyright file="GridRecord.cs" company="syncfusion">
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Table = Syncfusion.Grouping.Table;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// A record element for adding new records to the underlying datasource.
    /// This element represents the empty record that is shown above the table records and / or below
    /// the records for each group and implements logic to add new records.
    /// </summary>
    /// <remarks>
    /// This element represents the empty record that is shown above the table records and / or below
    /// the records for each group and implements logic to add new records. <para/>
    /// <para/>
    /// The class is derived from a record. It adds special behavior such that the AddNewRecord is
    /// not bound to a record in the underlying table. Instead all fields are initially empty (
    /// or default values based on parent group by criteria). When changes are made, they
    /// will be committed to the underlying table by adding a new record at the end of the
    /// underlying data source. The grouping engine will automatically insert the record into
    /// its sorted records collection and into the group it belongs to. If no group
    /// is found where the record fits in, a new group will be created.<para/>
    /// <para/>
    /// AddNewRecord(s) can be optionally displayed at the start of table below the column headers
    /// and / or at the end of each group. <para/>
    /// <para/>
    /// AddNewRecord belongs to a AddNewRecordSection and is its only element.<para/>
    /// <para/>
    /// If AddNewRecord(s) should be displayed at end of each group (see also Engine.ShowAddNewRecordInGroups)
    ///  then AddNewRecordSection(s) are created when the grouping for a table is initialized.
    ///  The grouping engine loops through all sorted records and categorizes them. For each new
    ///  group the virtual TableDescriptor.CreateGroup factory method is called. The TableDescriptor.CreateGroup
    ///  method instantiates a AddNewRecordSection by calling the virtual TableDescriptor.CreateAddNewRecordSection
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
    /// Because AddNewRecord is a container element and not a display element and it will not be an
    /// item returned by the Table.DisplayElements and Table.GroupedElements collection. Instead,
    /// AddNewRecord is only returned by the RecordsDetails.Records collection of its parent
    /// AddNewRecordSection (or the Group.Records collection of its ParentGroup which is a shortcut
    /// instead of accessing it through AddNewRecordSection). <para/>
    /// <para/>
    /// See also the Engine.RecordAsDisplayElements on how to make AddNewRecord be treated as a display
    /// element if you have no need for nested elements within a record., e.g. if you only want to
    /// display a single table and you do not need multiple row elements per record and / or nested tables.
    ///  (However, do not change this setting if you intend to use the engine as grid data model.)<para/>
    /// <para/>
    /// The first record part is a RecordRowsPart. A RecordRowsPart contains one or multiple RecordRows
    /// displayed in a grid for the record. <para/>
    /// <para/>
    /// The second record part represents nested tables. If relations have been defined for the table
    /// (see TableDescriptor.Relations) and the current table has a foreign key relationship setup
    /// to other tables, a table is added to the RecordPart[1].NestedTables collection for each relation.
    /// You can access nested tables in a record with the Record.NestedTables collection.<para/>
    /// <para/>
    /// AddNewRecord can be set as the current record in a table if you want to edit its data through
    /// the engine.<para/>
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
    /// AddNewRecord row or when BeginEdit, CancelEdit, or EndEdit was called.<para/>
    /// <para/>
    /// See also: CurrentRecordManager, CurrentRecordManager.IsAddNewRecord, CurrentRecordManager.NavigateTo,
    /// Group.AddNewRecord, Table.AddNewRecord, TableDescriptor.CreateGroup, Table.TopLevelGroup,
    /// AddNewRecordSection, IContainerElement, RecordsDetails.Records, Group.Records
    /// </remarks>
    /// <example>The following example shows how you can add records and keep track of sorted
    /// display positions within a table:
    /// <code lang="C#">
    ///             // Add a record to Table table2
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
    ///             // Now, you could navigate the current cell of a grid to row displayPos and highlight the newly added row
    ///             // within its correct group category and sort order
    /// </code>
    /// </example>
    public class GridAddNewRecord : AddNewRecord, IGridTableCellAppearanceSource, IGridTableCellStyleChanged
    {
        /// <summary>
        /// Initializes the new element in the specified AddNewRecordSection.
        /// </summary>
        /// <param name="parent">The parent that this element belongs to.</param>
        public GridAddNewRecord(AddNewRecordSection parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the reference to the <see cref="Table"/> of the <see cref="Engine"/> this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
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
        /// and contents should be serialized at design-time.
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

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion
        #region IGridTableCellStyleChanged Members
        bool inRaiseTableCellStyleChanged
        {
            get
            {
                return Reserved7;
            }

            set
            {
                Reserved7 = value;
            }
        }

        void IGridTableCellStyleChanged.RaiseTableCellStyleChanged(GridTableCellStyleInfoEventArgs e)
        {
            Record record = this;
            if (record == null || inRaiseTableCellStyleChanged)
            {
                e.Handled = true;
                return;
            }

            inRaiseTableCellStyleChanged = true;
            try
            {
                if (e.TableCellIdentity.Column != null)
                {
                    if ((e.Sip == null && e.Style.HasCellValue) || e.Sip.PropertyName == "CellValue")
                    {
                        GridColumnDescriptor column = e.TableCellIdentity.Column;
                        FieldDescriptor fieldDescriptor = column.FieldDescriptor;
                        object newValue = e.Style.CellValue;

                        RelationDescriptor rd = fieldDescriptor.GetRelation();
                        if (fieldDescriptor.IsRelatedField())
                        {
                            if (fieldDescriptor.IsComplexPropertyField())
                            {
                                FieldDescriptor parentField = fieldDescriptor.GetParentFieldDescriptor();
                                fieldDescriptor = parentField;
                            }
                            else if (rd.RelationKeys.Count > 0)
                            {
                                FieldDescriptor foreignKeyField = rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField;
                                fieldDescriptor = foreignKeyField;
                            }
                        }

                        if (IsEditing)
                        {
                            RecordValueChangingEventArgs changingArgs = new RecordValueChangingEventArgs(record, column.Name, fieldDescriptor, newValue);
                            ((GridTable)record.ParentTable).RaiseRecordValueChanging(changingArgs);
                            if (changingArgs.Cancel)
                            {
                                if (fieldDescriptor != null)
                                {
                                    e.Style.CellValue = record.GetValue(fieldDescriptor);
                                }
                            }
                            else
                            {
                                if (fieldDescriptor != null)
                                {
                                    record.SetValue(fieldDescriptor, changingArgs.NewValue);
                                }

                                RecordValueChangedEventArgs changedArgs = new RecordValueChangedEventArgs(record, column.Name, fieldDescriptor);
                                ((GridTable)record.ParentTable).RaiseRecordValueChanged(changedArgs);
                            }
                        }
                    }
                }

                e.Handled = true;
            }
            finally
            {
                inRaiseTableCellStyleChanged = false;
            }
        }
        #endregion

        /// <override/>
        /// <summary>
        /// Indicates whether to show the record preview rows.
        /// </summary>
        /// <returns>returns the boolean value False.</returns>
        public override bool ShouldShowRecordPreviewRows()
        {
            return false; ////((IGridTableOptionsSource) ParentTable).TableOptions.ShowRecordPreviewRow;
        }

        /// <override/>
        /// <summary>Indicates whether the current element is collapsible.</summary>
        public override bool IsCollapsible
        {
            get
            {
                return ((IGridTableOptionsSource)ParentTable).TableOptions.ShowRecordPlusMinus;
            }
        }
    }

    /// <summary>
    /// Represents a record with data. Each record in the datasource has an associated <see cref="Record"/> object
    /// in the engine. Records are created when the datasource is assigned to a table and before they are sorted or filtered.
    /// Also when a new record is inserted in the datasource, a <see cref="Record"/> is created. When the grouping or sorting
    /// of a <see cref="Table"/> changes, all <see cref="Record"/> elements stay in sync with their underlying record-counterparts
    /// in the datasource.
    /// <para/>
    /// By default a record will not appear in the <see cref="Table.DisplayElements"/>. Instead a record serves as a container
    /// of multiple row elements and nested tables.
    /// </summary>
    /// <remarks>
    /// There are multiple ways to get access to a specific record: <para/>
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
    /// meet filter criteria in the order as they were sorted in the engine. The <see cref="RecordsInTableCollectionBase.IndexOf"/> method
    /// of a <see cref="RecordsInTableCollection"/> determines the index of any record in the Table.FilteredRecords collection.
    /// </term></item>
    /// <item><term>
    /// The <see cref="Group.Records"/> collection of the <see cref="Group"/> class provides access to the records
    /// in the order as they appear in the group. The <see cref="RecordsInDetailsCollection"/> method
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
    /// in the datasource, you can keep a bookmark (reference) to a record. For example you can save a reference to a record,
    /// change the sort order of the table, and then later check Records.IndexOf to determine the new position where
    /// the record can be located after the sort.
    /// <para/>
    /// By default, a record will not appear in the <see cref="Table.DisplayElements"/>. Instead a record serves as a container
    /// for multiple row elements and nested tables. One exception is if you specified <see cref="Syncfusion.Grouping.Engine.RecordAsDisplayElements"/>.
    /// You can set <see cref="Syncfusion.Grouping.Engine.RecordAsDisplayElements"/> to True if you do not want the engine to treat record and ColumnHeaderSection
    /// elements as ContainerElements and instead have these elements be returned as a display element in the Table.DisplayElements collection.
    /// However, with a GridGroupingControl you must not change this property since a GridGroupingControl
    /// relies on the behavior that a record is not a display element but a container for rows
    /// and nested tables.
    /// <para/>
    /// The <see cref="Record.GetData"/> method will return a reference to the original record with data in the datasource.
    /// <para/>
    /// A record can be navigated to a current record if you call its <see cref="Record"/> method.
    /// <para/>
    /// Individual field contents can be retrieved with its <see cref="Record"/> method.
    /// </remarks>
    public class GridRecord : Record, IGridTableCellAppearanceSource, IGridTableCellStyleChanged
#if ASPNET
        , ICustomTypeDescriptor
#endif
    {
        /// <summary>
        /// Initializes a new record in the specified parent table.
        /// </summary>
        /// <param name="parent">The table this record belongs to.</param>
        public GridRecord(Table parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (appearance != null)
            {
                this.appearance.Dispose();
            }

            appearance = null;
            base.Dispose(disposing);
        }

        #region Strong Typed Parent Elements
        /// <override/>
        /// <summary>Returns the table descriptor this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTableDescriptor ParentTableDescriptor
        {
            get
            {
                return (GridTableDescriptor)base.ParentTableDescriptor;
            }
        }

        /// <override/>
        /// <summary>
        /// Gets the engine this element belongs to.
        /// </summary>
        public new GridEngine Engine
        {
            get
            {
                return (GridEngine)base.Engine;
            }
        }

        /// <override/>
        /// <summary>Gets the <see cref="Table"/> of the engine this element belongs to.</summary>
        public new GridTable EngineTable
        {
            get
            {
                return (GridTable)base.EngineTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent record this element belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridRecord ParentRecord
        {
            get
            {
                return (GridRecord)base.ParentRecord;
            }
        }

        /// <override/>
        /// <summary>A reference to the child table this element belongs.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridChildTable ParentChildTable
        {
            get
            {
                return (GridChildTable)base.ParentChildTable;
            }
        }

        /// <override/>
        /// <summary>A reference to the parent table this section belongs to.</summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public new GridTable ParentTable
        {
            get
            {
                return (GridTable)base.ParentTable;
            }

            set
            {
                base.ParentTable = value;
            }
        }

        #endregion
        #region Appearance
        GridTableCellAppearance appearance;

        /// <summary>
        /// Returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance in the hierarchy.
        /// </summary>
        /// <returns>A <see cref="GridTableCellAppearance"/>.</returns>
        public GridTableCellAppearance GetBaseAppearance()
        {
            Element parent = ParentElement;
            while (parent != null)
            {
                if (parent is IGridTableCellAppearanceSource && ((IGridTableCellAppearanceSource)parent).ShouldSerializeAppearance())
                {
                    return ((IGridTableCellAppearanceSource)parent).GetAppearance();
                }

                parent = parent.ParentElement;
            }

            return null;
        }

        /// <summary>
        /// If this element is modified, the Appearance returns this object's Appearance; otherwise it
        /// returns a <see cref="GridTableCellAppearance"/> of the first parent element with appearance
        /// in the hierarchy.
        /// </summary>
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null)
                {
                    return GetBaseAppearance();
                }

                return Appearance;
            }
        }

        /// <summary>
        /// The default <see cref="GridTableCellAppearance"/> with <see cref="GridTableCellStyleInfo"/>
        /// information for cells of this element.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GridTableCellAppearance Appearance
        {
            get
            {
                if (appearance == null)
                {
                    appearance = new GridTableCellAppearance(this);
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
        /// and contents should be serialized at design-time.
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

        GridTableCellAppearance IGridTableCellAppearanceSource.GetAppearance()
        {
            return Appearance;
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanged(GridTableCellStyleInfoChangedEventArgs e)
        {
        }

        void IGridTableCellAppearanceSource.RaiseAppearanceChanging(GridTableCellStyleInfoChangedEventArgs e)
        {
        }
        #endregion

        #region IGridTableCellStyleChanged Members
        bool inRaiseTableCellStyleChanged
        {
            get
            {
                return this.Reserved7;
            }

            set
            {
                Reserved7 = value;
            }
        }

        void IGridTableCellStyleChanged.RaiseTableCellStyleChanged(GridTableCellStyleInfoEventArgs e)
        {
            Record record = this;
            if (record == null || inRaiseTableCellStyleChanged)
            {
                e.Handled = true;
                return;
            }

            inRaiseTableCellStyleChanged = true;
            try
            {
                if (e.TableCellIdentity.Column != null)
                {
                    if ((e.Sip == null && e.Style.HasCellValue) || e.Sip.PropertyName == "CellValue")
                    {
                        GridColumnDescriptor column = e.TableCellIdentity.Column;
                        FieldDescriptor fieldDescriptor = column.FieldDescriptor;
                        object newValue = e.Style.CellValue;

                        RelationDescriptor rd = fieldDescriptor.GetRelation();
                        if (fieldDescriptor.IsRelatedField())
                        {
                            if (fieldDescriptor.IsComplexPropertyField())
                            {
                                FieldDescriptor parentField = fieldDescriptor.GetParentFieldDescriptor();
                                fieldDescriptor = parentField;
                            }
                            else if (rd.RelationKeys.Count > 0)
                            {
                                FieldDescriptor foreignKeyField = rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField;
                                fieldDescriptor = foreignKeyField;
                            }
                        }

                        RecordValueChangingEventArgs changingArgs = new RecordValueChangingEventArgs(record, column.Name, fieldDescriptor, newValue);
                        ((GridTable)record.ParentTable).RaiseRecordValueChanging(changingArgs);
                        if (changingArgs.Cancel)
                        {
                            if (fieldDescriptor != null)
                            {
                                e.Style.CellValue = record.GetValue(fieldDescriptor);
                            }
                        }
                        else
                        {
                            if (fieldDescriptor != null)
                            {
                                record.SetValue(fieldDescriptor, changingArgs.NewValue);
                            }
                            if (record != null && !record.IsDisposed)
                            {
                                RecordValueChangedEventArgs changedArgs = new RecordValueChangedEventArgs(record, column.Name, fieldDescriptor);
                                ((GridTable)record.ParentTable).RaiseRecordValueChanged(changedArgs);
                            }
                        }
                    }
                }

                e.Handled = true;
            }
            finally
            {
                inRaiseTableCellStyleChanged = false;
            }
        }
        #endregion

        /// <summary>
        /// Gets the value of the record.
        /// </summary>
        /// <param name="cd">The field to be retrieved.</param>
        /// <returns>Record value.</returns>
        public override object GetValue(SortColumnDescriptor cd)
        {
            GridTableDescriptor td = ParentTableDescriptor;
            if (td.ShouldSortByDisplayMember(cd))
            {
                object[] values = (object[])td.sortByDisplayMemberCols[cd.Name];
                GridComboBoxListBoxHelper listBox = (GridComboBoxListBoxHelper)values[0];
                GridStyleInfo style = (GridStyleInfo)values[1];
                return SortByDisplayMemberHelper.GetDisplayValue(ParentTable.TableModel, listBox, style, base.GetValue(cd));
            }

            return base.GetValue(cd);
        }

        /// <override/>
        /// <summary>Determines whether the record preview rows should be made visible.</summary>
        /// <returns>True if they are visible.</returns>
        public override bool ShouldShowRecordPreviewRows()
        {
            return ((IGridTableOptionsSource)ParentTable).TableOptions.ShowRecordPreviewRow;
        }

        /// <override/>
        /// <summary>Determines whether this element is collapsible.</summary>
        public override bool IsCollapsible
        {
            get
            {
                return !Engine.GetDesignMode()
                    && ((IGridTableOptionsSource)ParentTable).TableOptions.ShowRecordPlusMinus
                    && ((GridTableDescriptor)ParentTableDescriptor).RowsPerRecord > 0;
            }
        }

        /// <override/>
        /// <summary>Determines whether the record rows should be visible.</summary>
        /// <returns>True if they are visible.</returns>
        public override bool ShouldShowRecordRows()
        {
            return this.ParentTable.RelationParentTable != null
                || ((GridTableDescriptor)ParentTableDescriptor).RowsPerRecord > 0;
        }

#if ASPNET
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
            return ((ICustomTypeDescriptor) this).GetProperties(null);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return ((ITypedList)this.ParentTable.TableDescriptor).GetItemProperties(null);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion
#endif
    }

    /// <summary>
    /// Record that shadows values in table and can gives hints which values were changed in ListChanged event. The
    /// new GetOldValue method also gives access to the previous value before a change and can be used for
    /// calculating the difference between two values.
    /// </summary>
    public class GridRecordWithValueCache : GridRecord
    {
        object[] fieldValues;
        object[] oldValues;

        /// <summary>
        /// Constructor for GridRecordWithValueCache.
        /// </summary>
        /// <param name="parentTable">The parent table.</param>
        public GridRecordWithValueCache(Table parentTable)
            : base(parentTable)
        {
        }

        /// <override/>
        /// <summary>Resets the record values.</summary>
        public override void ResetValues()
        {
            fieldValues = null;
        }

        /// <summary>
        /// Ensures that record values are cached.
        /// </summary>
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
        /// Returns the value for a record.
        /// </summary>
        /// <param name="fieldDescriptor">The field descriptor.</param>
        /// <returns>Record value.</returns>
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
        /// <param name="fieldIndex">The index of the field whose value is required.</param>
        /// <returns>Old value of the record.</returns>
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
        /// the values in the record with changes found in underlying datasource.
        /// Only fields with a PropertyDescriptor are updated, others (unbound, expression fields) are ignored.
        /// </summary>
        /// <returns>An ArrayList with ChangedFieldInfo objects.</returns>
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
