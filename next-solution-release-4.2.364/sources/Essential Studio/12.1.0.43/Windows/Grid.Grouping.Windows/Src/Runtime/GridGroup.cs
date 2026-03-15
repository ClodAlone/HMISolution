//-------------------------------------------------------------------------------------------------
// <copyright file="GridGroup.cs" company="syncfusion">
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

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

using Table = Syncfusion.Grouping.Table;

#if ASPNET
using Syncfusion.Windows.Forms.Grid;
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// A Group defines a group of records that belong to a category. A group has
    /// multiple sections such as CaptionSection, SummarySection, GroupsDetailsSection,
    /// and RecordsDetailsSection.
    /// </summary>
    /// <remarks>
    /// Groups are created when the records of the table are categorized or when a new record
    /// is inserted. Normally,
    /// the categories are based on the <see cref="TableDescriptor.GroupedColumns"/> but
    /// programmers can also provide their own categorization routines by implementing
    /// a <see cref="SortColumnDescriptor.Comparer"/>
    /// for a <see cref="SortColumnDescriptor"/>. Before groups are categorized, the records are
    /// sorted in the order as specified by <see cref="TableDescriptor.GroupedColumns"/>. After the
    /// records were sorted, the <see cref="Table"/> object loops through all records to determine
    /// the categories records belong to.
    /// <para/>
    /// Another collection in the TableDescriptor that defines categorization is the
    /// <see cref="TableDescriptor.RelationChildColumns"/> collection. RelationChildColumns will be
    /// added when there is a parent-child relation between two tables. The child table of such a relation must be
    /// sorted by the columns that are used to identify a record. These columns match the foreign key
    /// columns of the parent table. For every new category key with regards to RelationChildColumns,
    /// a <see cref="ChildTable"/> is created. The ChildTable class is derived from <see cref="Group"/>.
    /// <para/>
    /// A group can either be a final node with records or it can be a node with nested groups. If a
    /// group has records, its <see cref="Group.Groups"/> collection will be empty and the <see cref="Group.Records"/>
    /// collection will contain all records. If a group has nested groups, its <see cref="Group.Groups"/>
    /// collection will have the nested groups and the <see cref="Group.Records"/> collection will be empty.
    /// <para/>
    /// A group can be expanded and collapsed with its <see cref="Group.IsExpanded"/> property. Expansion
    /// of groups will show or hide nested elements of the group within the <see cref="Table.DisplayElements"/>
    /// collection.
    /// <para/>
    /// A table has at least one group. The <see cref="Table.TopLevelGroup"/> is a <see cref="ChildTable"/>
    /// which is derived from <see cref="Group"/>.
    /// <para/>
    /// The <see cref="Element.GroupLevel"/> property will return the group level how deep the group is nested.
    /// </remarks>
    public class GridGroup : Group, IGridTableCellAppearanceSource, IGridGroupOptionsSource
    {
        /// <summary>
        /// Initializes a new group and assigns the parent.
        /// </summary>
        /// <param name="parent">The parent element this object belongs to.</param>
        public GridGroup(Section parent)
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

            if (groupOptions != null)
            {
                this.groupOptions.Dispose();
            }

            groupOptions = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Create the GroupTypedListRecordsCollection
        /// </summary>
        /// <returns>returns the GroupTypedListRecordsCollection</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override GroupTypedListRecordsCollection CreateGroupTypedListRecordsCollection()
        {
            return new GridGroupTypedListRecordsCollection(this);
        }

        /// <summary>
        /// Determines whether this group should be visible. By default a group is hidden if it does
        /// not have any records that meet filter criteria.
        /// </summary>
        /// <returns>
        /// true if group should be visibe; false otherwise.
        /// </returns>
        /// <override/>
        protected override bool IsGroupVisible()
        {
            return ParentTable.IsPassThroughGrouping || GetFilteredRecordCount() > 0 || this.ReadGroupOptions.ShowEmptyGroups;
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
        #region GroupOptions
        GridGroupOptionsStyleInfo groupOptions;

        /// <summary>
        /// Determines if GroupOptions were specified.
        /// </summary>
        public bool HasGroupOptions
        {
            get
            {
                return groupOptions != null;
            }
        }
        
        /// <summary>
        /// Lets you control the look of inner groups like whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo GroupOptions
        {
            get
            {
                if (groupOptions == null)
                {
                    GridGroupOptionsType t = GridGroupOptionsType.Groups;
                    if (this.IsTopLevelGroup)
                    {
                        t = GridGroupOptionsType.TopLevelGroup;
                    }
                    ////    else if (this is ChildTable)
                    ////        t = GridGroupOptionsType.ChildTable;

                    groupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity((IGridGroupOptionsSource)this, t));
                }

                return groupOptions;
            }

            set
            {
                GroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// If this element is modified, the GroupOptions returns this object's GroupOptions; otherwise it
        /// returns a <see cref="GridGroupOptionsStyleInfo"/> of the first parent element with GroupOptions
        /// in the hierarchy.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo ReadGroupOptions
        {
            get
            {
                if (groupOptions != null)
                {
                    return groupOptions;
                }

                return ((IGridGroupOptionsSource)this).GetParentGroupOptionsSource().GroupOptions;
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e)
        {
            this.InvalidateCounterTopDown(true);
            ////            If this needs to be called, check for ParentDisplayElement != null
            ////            if (this is ChildTable)
            ////                this.ParentDisplayElement.InvalidateCounterBottomUp();
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e)
        {
        }

        static bool useCategoryColumnGroupOptions = true;

        /// <summary>
        /// Enable or prevent each group from inheriting group options from the categories GridColumnDescriptor.GroupOptions.
        /// Set this property to false if you are concerned about real-time updates performance.
        /// </summary>
        public static bool UseCategoryColumnGroupOptions
        {
            get
            {
                return useCategoryColumnGroupOptions;
            }

            set
            {
                useCategoryColumnGroupOptions = value;
            }
        }

        IGridGroupOptionsSource IGridGroupOptionsSource.GetParentGroupOptionsSource()
        {
            if (UseCategoryColumnGroupOptions)
            {
                if (this.CategoryColumns.Count > 0)
                {
                    GridColumnDescriptor column = ((GridTableDescriptor)this.ParentTableDescriptor).Columns[this.CategoryColumns[0].Name];
                    if (column != null)
                    {
                        return column;
                    }
                }
            }

            return (IGridGroupOptionsSource)this.ParentTableDescriptor;
        }
        #endregion
        #region GridGroupExtend

        /// <override/>
        protected override void OnInitializeVisibleCounters()
        {
            GridGroupExtend.OnInitializeVisibleCounters(this, ReadGroupOptions);
        }

        ////int cini = 0;

        /// <summary>
        /// This virtual method is called from <see cref="M:Syncfusion.Grouping.Element.OnEnsureInitialized(System.Object)"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="M:Syncfusion.Grouping.Element.EnsureInitialized(System.Object)"/> call.</param>
        /// <returns>
        /// True if changes were detected and the object was updated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            if (this.CachedVisibleCount == -1)
            {
                ////                cini++;
                ////                if (cini > 1)
                ////                    Debugger.Break();
                GridGroupExtend.EnsureInitialized(this, this.ReadGroupOptions);
            }

            return base.OnEnsureInitialized(sender);
        }

        /// <override/>
        protected override void OnInitializeSections(bool hasRecords, SortColumnDescriptorCollection fields)
        {
            GridGroupExtend.OnInitializeSections(this, hasRecords, fields);
        }

        /// <override/>
        /// <summary>Determines whether the child of the given element is visible.</summary>
        /// <param name="el">The element.</param>
        /// <returns>True if it is visible.</returns>
        public override bool IsChildVisible(Element el)
        {
            return GridGroupExtend.IsChildVisible(this, el, ReadGroupOptions);
        }

        /// <override/>
        /// <summary>Determines whether the caption row should be shown</summary>
        /// <returns>True if it is made visible.</returns>
        public override bool ShouldShowCaption()
        {
            return GroupOptions.ShowCaption;
        }

        /// <override/>
        /// <summary>Determines whether this group can be collapsed.</summary>
        public override bool IsCollapsible
        {
            get
            {
                GridGroupOptionsStyleInfo go = ReadGroupOptions;
                return !Engine.GetDesignMode() && go.ShowCaption && go.ShowCaptionPlusMinus;
            }
        }
        #endregion

        /// <overload>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </overload>
        /// <summary>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </summary>       
        /// <param name="summaryRowName">The name of the GridSummaryRowDescriptor in the GridTableDescriptor.SummaryRows collection.</param>
        /// <param name="summaryColumnName">The name of the GridSummaryColumnDescriptor in the GridSummaryRowDescriptor.Summaries collection.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <remarks>
        /// See <see cref="GridSummaryColumnDescriptor"/> for an example.
        /// </remarks>
        /// <seealso cref="GridEngine"/>
        public string GetSummaryText(string summaryRowName, string summaryColumnName)
        {
            GridTable table = (GridTable)ParentTable;
            GridTableDescriptor td = table.TableDescriptor;
            GridSummaryRowDescriptor srd = td.SummaryRows[summaryRowName];
            GridSummaryColumnDescriptor scd = srd.SummaryColumns[summaryColumnName];

            return GetSummaryText(scd);
        }

        /// <summary>
        /// Returns the formatted summary text for the given group and summary column.
        /// </summary>
        /// <param name="scd">The GridSummaryColumnDescriptor.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <genoverload/>
        public string GetSummaryText(GridSummaryColumnDescriptor scd)
        {
            if (scd != null)
            {
                return scd.GetDisplayText(this);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the caption text for a group.
        /// </summary>
        /// <returns>Caption text for the group.</returns>
        public string GetGroupCaptionText()
        {
            return GridEngine.GetGroupCaptionText(this);
        }

        /// <summary>
        /// Returns the caption text for a group using a specified format.
        /// </summary>
        /// <param name="format">See GroupOptions.CaptionText, e.g. "{CategoryCaption}: {Category} - {RecordCount} Items";</param>
        /// <returns>Caption text for the group.</returns>
        public string GetGroupCaptionDisplayText(string format)
        {
            return GridEngine.GetGroupCaptionDisplayText(this, format);
        }

        internal int hasSummary = 0x00;
    }

    /// <summary>
    /// A ChildTable is either a TopLevelGroup or a group that can can be referenced
    /// as a nested table from a record in a parent table. ChildTable groups are created
    /// for every new key found based on <see cref="RelationDescriptor.RelationKeys"/>.<para/>
    /// You can get access to a ChildTable through the <see cref="NestedTable.ChildTable"/>
    /// property of a <see cref="NestedTable"/> in a record.
    /// </summary>
    /// <remarks>
    /// A ChildTable defines a group of records that belong to a category specified with
    /// <see cref="RelationDescriptor.RelationKeys"/>.
    /// ChildTables are created just like regular groups when the records of the table are
    /// categorized or when a new record is inserted. The collection in the TableDescriptor that defines categorization is the
    /// <see cref="TableDescriptor.RelationChildColumns"/> collection. RelationChildColumns will be
    /// added when there is a parent-child relation between two tables. The child table of such a relation must be
    /// sorted by the columns that are used to identify a record. These columns match the foreign key
    /// columns of the parent table. For every new category key with regards to RelationChildColumns,
    /// a <see cref="ChildTable"/> is created. The ChildTable class is derived from <see cref="Group"/>.
    /// <para/>
    /// A child table can either be a final node with records or it can be a node with nested groups. If a
    /// group has records, its <see cref="Group.Groups"/> collection will be empty and the <see cref="Group.Records"/>
    /// collection will contain all records. If a group has nested groups, its <see cref="Group.Groups"/>
    /// collection will have the nested groups and the <see cref="Group.Records"/> collection will be empty.
    /// <para/>
    /// A child table can be expanded and collapsed with its <see cref="Group.IsExpanded"/> property. Expansion
    /// of child tables will show or hide nested elements of the group within the <see cref="Table.DisplayElements"/>
    /// and <see cref="Table.NestedDisplayElements"/> collection.
    /// <para/>
    /// A Table has at least one child table. The <see cref="Table.TopLevelGroup"/> is a <see cref="ChildTable"/>
    /// which is derived from <see cref="Group"/>.
    /// </remarks>
    public class GridChildTable : ChildTable, IGridTableCellAppearanceSource, IGridGroupOptionsSource
    {
        GridCurrentCell currentCell;
        GridRangeInfoList selectedRanges = null;

        /// <summary>
        /// The current cell for this child table.
        /// </summary>
        public GridCurrentCell CurrentCell
        {
            get
            {
                return currentCell;
            }

            set
            {
                ////int id1 = ((GridNestedTableControl) value.Grid).id;
                ////if (this.ToString().IndexOf("46") != -1 && id1 == 0)
                ////    Debugger.Break();
                ////Console.WriteLine(this.ToString() + id1.ToString());
                currentCell = value;
            }
        }

        /// <internalonly/>
        /// <summary>Internal only.</summary>
        public virtual GridRangeInfoList SelectedRanges
        {
            get
            {
                if (selectedRanges == null)
                {
                    selectedRanges = new GridRangeInfoList();
                }

                return selectedRanges;
            }
        }
        
        /// <summary>
        /// Initializes a new child table and assigns the parent.
        /// </summary>
        /// <param name="parent">The parent element this object belongs to.</param>
        public GridChildTable(Element parent)
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

            if (groupOptions != null)
            {
                this.groupOptions.Dispose();
            }

            groupOptions = null;
            if (currentCell != null)
            {
                currentCell.Dispose();
            }

            currentCell = null;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Create GroupTypedListRecordsCollection
        /// </summary>
        /// <returns>returns GroupTypedListRecordsCollection</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude]
        protected override GroupTypedListRecordsCollection CreateGroupTypedListRecordsCollection()
        {
            return new GridGroupTypedListRecordsCollection(this);
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
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XmlIgnore]
        public GridTableCellAppearance ReadOnlyAppearance
        {
            get
            {
                if (appearance == null && !this.IsTopLevelGroup)
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
                    if (this.IsTopLevelGroup)
                    {
                        appearance.GroupCaptionCell.Enabled = false;
                    }
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
            return IsTopLevelGroup || (appearance != null && appearance.IsModified);
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
        #region GroupOptions
        GridGroupOptionsStyleInfo groupOptions;

        /// <summary>
        /// Determines if GroupOptions were specified.
        /// </summary>
        public bool HasGroupOptions
        {
            get
            {
                return groupOptions != null;
            }
        }

        /// <summary>
        /// Lets you control the look of inner groups such as whether the Caption Row is visible or what CaptionText is.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo GroupOptions
        {
            get
            {
                if (groupOptions == null)
                {
                    GridGroupOptionsType t = GridGroupOptionsType.Groups;
                    if (this.IsTopLevelGroup)
                    {
                        t = GridGroupOptionsType.TopLevelGroup;
                    }
                    else
                    {
                        ////if (this is ChildTable)
                        t = GridGroupOptionsType.ChildTable;
                    }

                    groupOptions = new GridGroupOptionsStyleInfo(new GridGroupOptionsStyleInfoIdentity((IGridGroupOptionsSource)this, t));
                }

                return groupOptions;
            }

            set
            {
                GroupOptions.CopyFrom(value);
            }
        }

        /// <summary>
        /// If this element is modified, the GroupOptions returns this object's GroupOptions; otherwise it
        /// returns a <see cref="GridGroupOptionsStyleInfo"/> of the first parent element with GroupOptions
        /// in the hierarchy.
        /// </summary>
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GridGroupOptionsStyleInfo ReadGroupOptions
        {
            get
            {
                return GroupOptions;
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanged(GridGroupOptionsChangedEventArgs e)
        {
            this.InvalidateCounterTopDown(true);
            ////if (this is ChildTable)
            //// Sometimes this is null in ASPNET
            if (this.ParentDisplayElement != null)
            {
                this.ParentDisplayElement.InvalidateCounterBottomUp();
            }
        }

        void IGridGroupOptionsSource.RaiseGroupOptionsChanging(GridGroupOptionsChangedEventArgs e)
        {
        }

        /// <summary>
        /// Returns a <see cref="IGridGroupOptionsSource"/> of the first parent element with group options in the hierarchy.
        /// </summary>
        /// <returns>returns IGridGroupOptionsSource for hosting GridGroupOptionStyleInfo</returns>
        IGridGroupOptionsSource IGridGroupOptionsSource.GetParentGroupOptionsSource()
        {
            return (IGridGroupOptionsSource)this.ParentTableDescriptor;
        }
        #endregion
        #region GridGroupExtend

        /// <override/>
        protected override void OnInitializeVisibleCounters()
        {
            GridGroupExtend.OnInitializeVisibleCounters(this, ReadGroupOptions);
        }

        /// <summary>
        /// This virtual method is called from <see cref="M:Syncfusion.Grouping.Element.OnEnsureInitialized(System.Object)"/> and
        /// lets derived elements implement element-specific logic to ensure object
        /// is up to data.
        /// </summary>
        /// <param name="sender">The object that triggered the <see cref="M:Syncfusion.Grouping.Element.EnsureInitialized(System.Object)"/> call.</param>
        /// <returns>
        /// True if changes were detected and the object was updated; False otherwise.
        /// </returns>
        /// <override/>
        protected override bool OnEnsureInitialized(object sender)
        {
            if (this.CachedVisibleCount == -1)
            {
                GridGroupExtend.EnsureInitialized(this, this.ReadGroupOptions);
            }

            ////GridGroupExtend.EnsureInitialized(this, ReadGroupOptions);
            ////EnsureInitialized(this);
            return base.OnEnsureInitialized(sender);
        }

        /// <override/>
        protected override void OnInitializeSections(bool hasRecords, SortColumnDescriptorCollection fields)
        {
            GridGroupExtend.OnInitializeSections(this, hasRecords, fields);
        }

        /// <override/>
        /// <summary>
        /// Determines whether the child of current element is visible.
        /// </summary>
        /// <param name="el">The specified element.</param>
        /// <returns>True if it is visible; False otherwise.</returns>
        public override bool IsChildVisible(Element el)
        {
            return GridGroupExtend.IsChildVisible(this, el, ReadGroupOptions);
        }

        /// <override/>
        /// <summary>Indicates whether caption row is visible.</summary>
        /// <returns>True if it is visible; False otherwise.</returns>
        public override bool ShouldShowCaption()
        {
            return ReadGroupOptions.ShowCaption;
        }

        /// <override/>
        /// <summary>
        /// Indicates whether this element can be collapsed.
        /// </summary>
        public override bool IsCollapsible
        {
            get
            {
                GridGroupOptionsStyleInfo go = ReadGroupOptions;
                return !Engine.GetDesignMode() && go.ShowCaption && go.ShowCaptionPlusMinus && !(ParentGroup == null && ParentTableDescriptor.IsForeignKeyRelationChildTableDescriptor());
            }
        }

        #endregion

        /// <overload>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </overload>
        /// <summary>
        /// Returns the formatted summary text for the given group, summary row and column.
        /// </summary>
        /// <param name="summaryRowName">The name of the GridSummaryRowDescriptor in the GridTableDescriptor.SummaryRows collection.</param>
        /// <param name="summaryColumnName">The name of the GridSummaryColumnDescriptor in the GridSummaryRowDescriptor.Summaries collection.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <remarks>
        /// See <see cref="GridSummaryColumnDescriptor"/> for an example.
        /// </remarks>
        /// <seealso cref="GridEngine"/>
        public string GetSummaryText(string summaryRowName, string summaryColumnName)
        {
            GridTable table = (GridTable)ParentTable;
            GridTableDescriptor td = table.TableDescriptor;
            GridSummaryRowDescriptor srd = td.SummaryRows[summaryRowName];
            GridSummaryColumnDescriptor scd = srd.SummaryColumns[summaryColumnName];

            return GetSummaryText(scd);
        }

        /// <summary>
        /// Returns the formatted summary text for the given group and summary column.
        /// </summary>
        /// <param name="scd">The GridSummaryColumnDescriptor.</param>
        /// <returns>The summary formatted text as specified with GridSummaryColumnDescriptor.Format</returns>
        /// <genoverload/>
        public string GetSummaryText(GridSummaryColumnDescriptor scd)
        {
            if (scd != null)
            {
                return scd.GetDisplayText(this);
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns the caption text for a group.
        /// </summary>
        /// <returns>Caption text for a group.</returns>
        public string GetGroupCaptionText()
        {
            return GridEngine.GetGroupCaptionText(this);
        }

        /// <summary>
        /// Returns the caption text for a group using a specified format.
        /// </summary>
        /// <param name="format">See GroupOptions.CaptionText, e.g. "{CategoryCaption}: {Category} - {RecordCount} Items";</param>
        /// <returns>Group caption display text.</returns>
        public string GetGroupCaptionDisplayText(string format)
        {
            return GridEngine.GetGroupCaptionDisplayText(this, format);
        }

        internal int hasSummary = 0x00;
    }

    /// <internalonly/>
    /// <summary>Used internally.</summary>
    [Syncfusion.Documentation.DocumentationExclude]
    public class GridGroupExtend
    {
        /// <summary>
        /// Default Constructor. Used internally.
        /// </summary>
        public GridGroupExtend()
            : base()
        {
        }

        #region reserved fields
        static bool getHasGroupHeaderSection(Group owner)
        {
            return owner.Reserved3;
        }

        static void setHasGroupHeaderSection(Group owner, bool value)
        {
            owner.Reserved3 = value;
        }

        static bool getHasColumnHeaderSection(Group owner)
        {
            return owner.Reserved4;
        }

        static void setHasColumnHeaderSection(Group owner, bool value)
        {
            owner.Reserved4 = value;
        }

        static bool getHasAddNewRecordSectionBeforeDetails(Group owner)
        {
            return owner.Reserved5;
        }

        static void setHasAddNewRecordSectionBeforeDetails(Group owner, bool value)
        {
            owner.Reserved5 = value;
        }

        static bool getHasAddNewRecordAfterDetails(Group owner)
        {
            return owner.Reserved6;
        }

        static void setHasAddNewRecordAfterDetails(Group owner, bool value)
        {
            owner.Reserved6 = value;
        }

        static bool getHasSummary(Group owner)
        {
            return owner.Reserved7;
        }

        static void setHasSummary(Group owner, bool value)
        {
            owner.Reserved7 = value;
        }

        static bool getHasGroupFooterSection(Group owner)
        {
            return owner.Reserved8;
        }

        static void setHasGroupFooterSection(Group owner, bool value)
        {
            owner.Reserved8 = value;
        }

        static bool getHasGroupPreviewSection(Group owner)
        {
            return owner.Reserved9;
        }

        static void setHasGroupPreviewSection(Group owner, bool value)
        {
            owner.Reserved9 = value;
        }

        static bool getHasFilterBarSection(Group owner)
        {
            return owner.Reserved10;
        }

        static void setHasFilterBarSection(Group owner, bool value)
        {
            owner.Reserved10 = value;
        }

        static bool getHasStackedHeaderSection(Group owner)
        {
            return owner.Reserved11;
        }

        static void setHasStackedHeaderSection(Group owner, bool value)
        {
            owner.Reserved11 = value;
        }

        #endregion

        #region summary site support

        ////BeforeFilter = 0x01,
        ////BeforeDetails = 0x02,
        ////AfterDetails = 0x04,

        static bool getHasSummaryRowsBeforeFilter(Group owner)
        {
            if (owner is GridGroup)
            {
                return ((owner as GridGroup).hasSummary & 0x01) == 0x01;
            }

            if (owner is GridChildTable)
            {
                return ((owner as GridChildTable).hasSummary & 0x01) == 0x01;
            }

            return false;
        }

        static void setHasSummaryRowsBeforeFilter(Group owner, bool value)
        {
            if (owner is GridGroup)
            {
                if (value)
                {
                    (owner as GridGroup).hasSummary |= 0x01;
                }
                else
                {
                    (owner as GridGroup).hasSummary ^= 0x01;
                }
                ////setHasSummary(owner, value);
            }

            if (owner is GridChildTable)
            {
                if (value)
                {
                    (owner as GridChildTable).hasSummary |= 0x01;
                }
                else
                {
                    (owner as GridChildTable).hasSummary ^= 0x01;
                }
                ////setHasSummary(owner, value);
            }
        }

        static bool getHasSummaryRowsBeforeDetails(Group owner)
        {
            if (owner is GridGroup)
            {
                return ((owner as GridGroup).hasSummary & 0x02) == 0x02;
            }

            if (owner is GridChildTable)
            {
                return ((owner as GridChildTable).hasSummary & 0x02) == 0x02;
            }

            return false;
        }

        static void setHasSummaryRowsBeforeDetails(Group owner, bool value)
        {
            if (owner is GridGroup)
            {
                if (value)
                {
                    (owner as GridGroup).hasSummary |= 0x02;
                }
                else
                {
                    (owner as GridGroup).hasSummary ^= 0x02;
                }
                ////setHasSummary(owner, value);
            }

            if (owner is GridChildTable)
            {
                if (value)
                {
                    (owner as GridChildTable).hasSummary |= 0x02;
                }
                else
                {
                    (owner as GridChildTable).hasSummary ^= 0x02;
                }
                ////setHasSummary(owner, value);
            }
        }

        static bool getHasSummaryRowsAfterDetails(Group owner)
        {
            if (owner is GridGroup)
            {
                return ((owner as GridGroup).hasSummary & 0x04) == 0x04;
            }

            if (owner is GridChildTable)
            {
                return ((owner as GridChildTable).hasSummary & 0x04) == 0x04;
            }

            return false;
        }

        static void setHasSummaryRowsAfterDetails(Group owner, bool value)
        {
            if (owner is GridGroup)
            {
                if (value)
                {
                    (owner as GridGroup).hasSummary |= 0x04;
                }
                else
                {
                    (owner as GridGroup).hasSummary ^= 0x04;
                }
                ////setHasSummary(owner, value);
            }

            if (owner is GridChildTable)
            {
                if (value)
                {
                    (owner as GridChildTable).hasSummary |= 0x04;
                }
                else
                {
                    (owner as GridChildTable).hasSummary ^= 0x04;
                }
                ////setHasSummary(owner, value);
            }
        }

        static bool HasSummaryRows(GridGroupOptionsStyleInfo go, GridSummaryRowPlacement ss, bool isExpanded)
        {
            if (go.ShowSummaries && ((go.SummaryRowPlacement == ss && isExpanded) ||
                (go.ShowGroupSummaryWhenCollapsed && ss == GridSummaryRowPlacement.AfterDetails && !isExpanded)))
            {
                return true;
            }

            return false;
        }
        #endregion

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public static void OnInitializeSections(Group owner, bool hasRecords, SortColumnDescriptorCollection fields)
        {
            // The base grid works under the assumption that there is at least one column header with
            // scrollable rows below. Event setting the frozen count to 0 will not make
            // the first row scrollable. So, what we do here is make the first row be of height 0.
            // GridHiddenSection returns GetVisibleCount() == 1 and GetYAmountCount() == 0.
            //
            // It simulates setting grid.Model.RowHeights[0] = 0;
            //
            // This enables us to adjust the frozen count to be 0 or more and this will allow
            // the records section to start right at the top (no caption, header etc. visible above records)
            //
            // The hidden section is only needed for the very first group in a grid, not for child groups
            // and not for nested tables.
            if (owner.IsTopLevelGroup)
            {
                owner.Sections.Add(new GridHiddenSection(owner));
            }

            GridGroupOptionsStyleInfo go = ((IGridGroupOptionsSource)owner).GetParentGroupOptionsSource().GroupOptions;
            if (owner is ChildTable && !owner.IsMainGroup)
            {
                go = ((GridTableDescriptor)owner.ParentTableDescriptor).TopLevelGroupOptions;
            }

            owner.Sections.Add(owner.ParentTableDescriptor.CreateCaptionSection(owner));

            if (go.ShowGroupHeader)
            {
                owner.Sections.Add(owner.Engine.CreateGroupHeaderSection(owner));
                setHasGroupHeaderSection(owner, true);
            }

            if (go.ShowStackedHeaders)
            {
                owner.Sections.Add(((GridEngine)owner.Engine).CreateStackedHeaderSection(owner));
                setHasStackedHeaderSection(owner, true);
            }

            if (owner.IsTopLevelGroup || owner is ChildTable || go.ShowColumnHeaders)
            {
                owner.Sections.Add(owner.ParentTableDescriptor.CreateColumnHeaderSection(owner));
                setHasColumnHeaderSection(owner, true);
            }

            if (go.SummaryRowPlacement == GridSummaryRowPlacement.BeforeFilter)
            {
                if (((GridTableDescriptor)owner.ParentTableDescriptor).SummaryRows.VisibleRowCount > 0)
                {
                    setHasSummaryRowsBeforeFilter(owner, true);
                    owner.Sections.Add(owner.ParentTableDescriptor.CreateSummarySection(owner));
                }
            }

            if (go.ShowFilterBar)
            {
                owner.Sections.Add(owner.ParentTableDescriptor.CreateFilterBarSection(owner));
                setHasFilterBarSection(owner, true);
            }

            if (go.SummaryRowPlacement == GridSummaryRowPlacement.BeforeDetails)
            {
                if (((GridTableDescriptor)owner.ParentTableDescriptor).SummaryRows.VisibleRowCount > 0)
                {
                    setHasSummaryRowsBeforeFilter(owner, true);
                    owner.Sections.Add(owner.ParentTableDescriptor.CreateSummarySection(owner));
                }
            }

            if (owner.IsMainGroup || go.ShowAddNewRecordBeforeDetails)
            {
                AddNewRecordSection addNewRecordSectionBeforeDetails = owner.ParentTableDescriptor.CreateAddNewRecordSection(owner);
                addNewRecordSectionBeforeDetails.IsBeforeDetails = true;
                owner.Sections.Add(addNewRecordSectionBeforeDetails);
                setHasAddNewRecordSectionBeforeDetails(owner, true);
            }

            if (hasRecords)
            {
                owner.Sections.Add(owner.ParentTableDescriptor.CreateRecordsDetails(owner, fields));
            }
            else
            {
                owner.Sections.Add(owner.ParentTableDescriptor.CreateGroupsDetails(owner, fields));
            }

            owner.IsExpanded = go.IsExpandedInitialValue;

            if (go.ShowAddNewRecordAfterDetails)
            {
                owner.Sections.Add(owner.ParentTableDescriptor.CreateAddNewRecordSection(owner));
                setHasAddNewRecordAfterDetails(owner, true);
            }

            if (go.ShowGroupPreview && go.ShowCaptionPlusMinus && go.ShowCaption)
            {
                owner.Sections.Add(owner.Engine.CreateGroupPreviewSection(owner));
                setHasGroupPreviewSection(owner, true);
            }

            ////AfterDetail
            if (/*go.ShowSummaries || */go.ShowGroupSummaryWhenCollapsed)
            {
                if (((GridTableDescriptor)owner.ParentTableDescriptor).SummaryRows.VisibleRowCount > 0)
                {
                    owner.Sections.Add(owner.ParentTableDescriptor.CreateSummarySection(owner));
                    ////setHasSummary(owner, true);
                    setHasSummaryRowsAfterDetails(owner, true);
                }
            }
            ////Init(owner, go, true);

            if (go.ShowGroupFooter)
            {
                owner.Sections.Add(owner.Engine.CreateGroupFooterSection(owner));
                setHasGroupFooterSection(owner, true);
                setHasGroupFooterSection(owner, true);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public static void EnsureInitialized(Group owner, GridGroupOptionsStyleInfo go)
        {
            Init(owner, go, false);
        }

        static void Init(Group owner, GridGroupOptionsStyleInfo go, bool fromInitializeSections)
        {
            //// Note: When you make changes here and add support for additional groups, make sure
            //// you also adjust the OnInitializeSections method above. It is not a good idea
            //// when sections are added/removed later. Adding, removing of sections of existing
            //// groups is more intended towards being able to react to changes in GroupOptions
            //// after the table was initialized.

            int index = 0;
            int count = owner.Sections.Count;
            int oldCount = count;
            bool allowRemove = true;
            bool isExpanded = owner.IsExpanded;
            bool changed = false;

            //// GridHiddenSection
            if (owner.IsTopLevelGroup)
            {
                index++;
            }

            //// CreateCaptionSection
            index++;

            //// CreateGroupHeaderSection
            if (!getHasGroupHeaderSection(owner))
            {
                if (isExpanded && go.ShowGroupHeader)
                {
                    setHasGroupHeaderSection(owner, true);
                    changed = true;
                    owner.Sections.Insert(index, owner.Engine.CreateGroupHeaderSection(owner));
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowGroupHeader)
                {
                    index++;
                }
                else
                {
                    setHasGroupHeaderSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasStackedHeaderSection
            if (!getHasStackedHeaderSection(owner))
            {
                if (isExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).StackedHeaderRows.Count > 0 && go.ShowStackedHeaders)
                {
                    setHasStackedHeaderSection(owner, true);
                    changed = true;
                    owner.Sections.Insert(index, ((GridEngine)owner.Engine).CreateStackedHeaderSection(owner));
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowStackedHeaders)
                {
                    index++;
                }
                else
                {
                    setHasStackedHeaderSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasColumnHeaderSection
            if (!getHasColumnHeaderSection(owner))
            {
                if (isExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).Columns.Count > 0 && go.ShowColumnHeaders)
                {
                    setHasColumnHeaderSection(owner, true);
                    changed = true;
                    owner.Sections.Insert(index, owner.Engine.CreateColumnHeaderSection(owner));
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || owner.IsTopLevelGroup || owner is ChildTable || go.ShowColumnHeaders)
                {
                    index++;
                }
                else
                {
                    setHasColumnHeaderSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasSummaryRowsBeforeFilter
            if (!getHasSummaryRowsBeforeFilter(owner))
            {
                if (HasSummaryRows(go, GridSummaryRowPlacement.BeforeFilter, isExpanded))
                {
                    setHasSummaryRowsBeforeFilter(owner, true);
                    changed = true;
                    if (index < count)
                    {
                        owner.Sections.Insert(index, owner.Engine.CreateSummarySection(owner));
                    }
                    else
                    {
                        owner.Sections.Add(owner.Engine.CreateSummarySection(owner));
                    }

                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || HasSummaryRows(go, GridSummaryRowPlacement.BeforeFilter, isExpanded))
                {
                    index++;
                }
                else
                {
                    setHasSummaryRowsBeforeFilter(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            if (!getHasFilterBarSection(owner))
            {
                if (isExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).Columns.Count > 0 && go.ShowFilterBar)
                {
                    setHasFilterBarSection(owner, true);
                    changed = true;
                    owner.Sections.Insert(index, owner.Engine.CreateFilterBarSection(owner));
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowFilterBar)
                {
                    index++;
                }
                else
                {
                    setHasFilterBarSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasSummaryRowsBeforeDetails
            if (!getHasSummaryRowsBeforeDetails(owner))
            {
                if (HasSummaryRows(go, GridSummaryRowPlacement.BeforeDetails, isExpanded))
                {
                    setHasSummaryRowsBeforeDetails(owner, true);
                    changed = true;
                    if (index < count)
                    {
                        owner.Sections.Insert(index, owner.Engine.CreateSummarySection(owner));
                    }
                    else
                    {
                        owner.Sections.Add(owner.Engine.CreateSummarySection(owner));
                    }

                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || HasSummaryRows(go, GridSummaryRowPlacement.BeforeDetails, isExpanded))
                {
                    index++;
                }
                else
                {
                    setHasSummaryRowsBeforeDetails(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasAddNewRecordSectionBeforeDetails
            if (!getHasAddNewRecordSectionBeforeDetails(owner))
            {
                if (isExpanded && owner.ParentTableDescriptor.AllowNew && owner.ParentTable.SourceListAllowNew && go.ShowAddNewRecordBeforeDetails)
                {
                    setHasAddNewRecordSectionBeforeDetails(owner, true);
                    changed = true;
                    AddNewRecordSection addNewRecordSectionBeforeDetails = owner.ParentTableDescriptor.CreateAddNewRecordSection(owner);
                    addNewRecordSectionBeforeDetails.IsBeforeDetails = true;
                    if (index < count)
                    {
                        owner.Sections.Insert(index, addNewRecordSectionBeforeDetails);
                    }
                    else
                    {
                        owner.Sections.Add(addNewRecordSectionBeforeDetails);
                    }

                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || owner.IsMainGroup || go.ShowAddNewRecordBeforeDetails)
                {
                    index++;
                }
                else
                {
                    setHasAddNewRecordSectionBeforeDetails(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// Details
            index++;

            //// getHasAddNewRecordAfterDetails
            if (!getHasAddNewRecordAfterDetails(owner))
            {
                if (isExpanded && owner.ParentTableDescriptor.AllowNew && owner.ParentTable.SourceListAllowNew && go.ShowAddNewRecordAfterDetails)
                {
                    setHasAddNewRecordAfterDetails(owner, true);
                    AddNewRecordSection addNewRecordSectionAfterDetails = owner.ParentTableDescriptor.CreateAddNewRecordSection(owner);
                    addNewRecordSectionAfterDetails.IsBeforeDetails = false;
                    changed = true;
                    if (index < count)
                    {
                        owner.Sections.Insert(index, addNewRecordSectionAfterDetails);
                    }
                    else
                    {
                        owner.Sections.Add(addNewRecordSectionAfterDetails);
                    }

                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowAddNewRecordAfterDetails)
                {
                    index++;
                }
                else
                {
                    setHasAddNewRecordAfterDetails(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// getHasGroupPreviewSection
            if (!getHasGroupPreviewSection(owner))
            {
                if (!isExpanded && go.ShowGroupPreview)
                {
                    setHasGroupPreviewSection(owner, true);
                    if (index < count)
                    {
                        owner.Sections.Insert(index, owner.Engine.CreateGroupPreviewSection(owner));
                    }
                    else
                    {
                        owner.Sections.Add(owner.Engine.CreateGroupPreviewSection(owner));
                    }

                    changed = true;
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowGroupPreview)
                {
                    index++;
                }
                else
                {
                    setHasGroupPreviewSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// setHasSummaryRowsAfterDetails
            if (!getHasSummaryRowsAfterDetails(owner))
            {
                if (HasSummaryRows(go, GridSummaryRowPlacement.AfterDetails, isExpanded))
                {
                    setHasSummaryRowsAfterDetails(owner, true);
                    changed = true;
                    if (index < count)
                    {
                        owner.Sections.Insert(index, owner.Engine.CreateSummarySection(owner));
                    }
                    else
                    {
                        owner.Sections.Add(owner.Engine.CreateSummarySection(owner));
                    }

                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || HasSummaryRows(go, GridSummaryRowPlacement.AfterDetails, isExpanded))
                {
                    index++;
                }
                else
                {
                    setHasSummaryRowsAfterDetails(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            //// getHasSummary
            ////if (!getHasSummary(owner))
            ////{
            ////    if (((GridTableDescriptor) owner.ParentTableDescriptor).SummaryRows.VisibleRowCount > 0)
            ////    {
            ////        if (owner.Engine.CounterLogic == EngineCounters.All || 
            ////            (isExpanded && go.ShowSummaries) ||
            ////            (!isExpanded && go.ShowGroupSummaryWhenCollapsed))
            ////        {
            ////            setHasSummary(owner, true);
            ////            changed = true;
            ////            if (index < count)
            ////                owner.Sections.Insert(index, owner.Engine.CreateSummarySection(owner));
            ////            else
            ////                owner.Sections.Add(owner.Engine.CreateSummarySection(owner));
            ////            index++;
            ////            count++;
            ////        }
            ////    }
            ////}
            ////else
            ////{
            ////    if (!allowRemove || go.ShowSummaries || go.ShowGroupSummaryWhenCollapsed)
            ////        index++;
            ////    else
            ////    {
            ////        setHasSummary(owner, false);
            ////        changed = true;
            ////        owner.Sections.RemoveAt(index);
            ////        count--;
            ////    }

            ////}

            //// getHasGroupFooterSection
            if (!getHasGroupFooterSection(owner))
            {
                if (isExpanded && go.ShowGroupFooter)
                {
                    setHasGroupFooterSection(owner, true);
                    if (index < count)
                    {
                        owner.Sections.Insert(index, owner.Engine.CreateGroupFooterSection(owner));
                    }
                    else
                    {
                        owner.Sections.Add(owner.Engine.CreateGroupFooterSection(owner));
                    }

                    changed = true;
                    index++;
                    count++;
                }
            }
            else
            {
                if (!allowRemove || go.ShowGroupFooter)
                {
                    index++;
                }
                else
                {
                    setHasGroupFooterSection(owner, false);
                    changed = true;
                    owner.Sections.RemoveAt(index);
                    count--;
                }
            }

            if (!fromInitializeSections && changed)
            {
                ////Trace.WriteLine(String.Format("{4} - {0}: {1} Sections.Count {2} count, oldCount {3}", owner.Category, owner.Sections.Count, count, oldCount, owner.ParentTableDescriptor.Name));
                owner.InvalidateCounter();
                ////owner.InvalidateCounterBottomUp();
                //// owner.InvalidateCounterTopDown(true);
                owner.Engine.BumpVersion();
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        public static void OnInitializeVisibleCounters(Group owner, GridGroupOptionsStyleInfo go)
        {
            int visibleCount = 0;
            double yAmountCount = 0;
            double visibleCustomCount = 0;

            EnsureInitialized(owner, go);

            int count = owner.Sections.InnerCount;
            for (int n = 0; n < count; n++)
            {
                Section s = owner.Sections.GetInnerItem(n);
                if (IsChildVisible(owner, s, go))
                {
                    visibleCount += s.GetVisibleCount();
                    yAmountCount += s.GetYAmountCount();
                    visibleCustomCount += s.GetVisibleCustomCount();
                }
            }

            owner.CachedVisibleCount = visibleCount;
            owner.CachedYamountCount = yAmountCount;
            owner.CachedVisibleCustomCount = visibleCustomCount;
        }

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <param name="owner">The Group that defines a set of records that belong to a category.</param>
        /// <param name="el">The Element.</param>
        /// <param name="go">The GridGroupOptionsStyleInfo.</param>
        /// <returns>
        /// <c>true</c> if [is child visible] [the specified owner]; otherwise, <c>false</c>.
        /// </returns>
        /// <internalonly/>
        public static bool IsChildVisible(Group owner, Element el, GridGroupOptionsStyleInfo go)
        {
            if (el is CaptionSection)
            {
                return go.ShowCaption;
            }
            else if (el is GroupHeaderSection)
            {
                return owner.IsExpanded && go.ShowGroupHeader;
            }
            else if (el is ColumnHeaderSection)
            {
                return owner.IsExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).Columns.Count > 0 && go.ShowColumnHeaders;
            }
            else if (el is GridStackedHeaderSection)
            {
                return owner.IsExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).Columns.Count > 0 && go.ShowStackedHeaders;
            }
            else if (el is FilterBarSection)
            {
                return owner.IsExpanded && ((GridTableDescriptor)owner.ParentTableDescriptor).Columns.Count > 0 && go.ShowFilterBar;
            }
            else if (el is AddNewRecordSection)
            {
                AddNewRecordSection section = (AddNewRecordSection)el;
                if (section.IsBeforeDetails)
                {
                    return owner.IsExpanded && owner.ParentTableDescriptor.AllowNew && owner.ParentTable.SourceListAllowNew && go.ShowAddNewRecordBeforeDetails;
                }
                else
                {
                    return owner.IsExpanded && owner.ParentTableDescriptor.AllowNew && owner.ParentTable.SourceListAllowNew && go.ShowAddNewRecordAfterDetails;
                }
            }
            else if (el is DetailsSection)
            {
                return owner.IsExpanded;
            }
            else if (el is GridSummarySection)
            {
                return (owner.IsExpanded && go.ShowSummaries) ||
                    (!owner.IsExpanded && go.ShowGroupSummaryWhenCollapsed);
            }
            else if (el is GroupFooterSection)
            {
                return owner.IsExpanded && go.ShowGroupFooter;
            }
            else if (el is GroupPreviewSection)
            {
                return !owner.IsExpanded && go.ShowGroupPreview;
            }

            return true;
        }
    }
}
