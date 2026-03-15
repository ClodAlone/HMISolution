//-------------------------------------------------------------------------------------------------
// <copyright file="ChildTable.cs" company="syncfusion">
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
    /// A ChildTable is either a TopLevelGroup or a group that can can be referenced
    /// as nested table from a record in a parent table. ChildTable groups are created
    /// for every new key found based on <see cref="RelationDescriptor.RelationKeys"/>.<para/>
    /// You can get access to a ChildTable through the <see cref="NestedTable.ChildTable"/>
    /// property of a <see cref="NestedTable"/> in a record.
    /// </summary>
    /// <remarks>
    /// A ChildTable defines a group of records that belong to a category specified with
    /// <see cref="RelationDescriptor.RelationKeys"/>.
    /// ChildTables are created similar tp regular groups when the records of the table are
    /// categorized or when a new record is inserted. The collection in the TableDescriptor that defines categorization is the
    /// <see cref="TableDescriptor.RelationChildColumns"/> collection. RelationChildColumns will be
    /// added when there is a parent-child relation between two tables. The child table of such relation must be
    /// sorted by the columns that are used to identify a record. These columns match the foreign key
    /// columns of the parent table. For every new category key with regards to RelationChildColumns
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
    /// A table has at least one child table. The <see cref="Table.TopLevelGroup"/> is a <see cref="ChildTable"/>
    /// which is derived from <see cref="Group"/>.
    /// </remarks>
    public class ChildTable : Group
    {
        NestedTablesTreeTableEntry nestedTableEntry;
#if WEAKREF
        WeakReference __displayElements;
        WeakReference __groupedElements;
        WeakReference __nestedGroupedElements;
        WeakReference __nestedDisplayElements;


        internal DisplayElementsInTableCollection _displayElements
        {
            get
            {
                if (__displayElements != null)
                    return (DisplayElementsInTableCollection) __displayElements.Target;
                return null;
            }
            set
            {
                __displayElements = new WeakReference(value);
            }
        }

        internal ElementsInTableCollection _groupedElements
        {
            get
            {
                if (__groupedElements != null)
                    return (ElementsInTableCollection) __groupedElements.Target;
                return null;
            }
            set
            {
                __groupedElements = new WeakReference(value);
            }
        }

        internal ElementsInTableCollection _nestedGroupedElements
        {
            get
            {
                if (__nestedGroupedElements != null)
                    return (ElementsInTableCollection) __nestedGroupedElements.Target;
                return null;
            }
            set
            {
                __nestedGroupedElements = new WeakReference(value);
            }
        }

        internal DisplayElementsInTableCollection _nestedDisplayElements
        {
            get
            {
                if (__nestedDisplayElements != null)
                    return (DisplayElementsInTableCollection) __nestedDisplayElements.Target;
                return null;
            }
            set
            {
                __nestedDisplayElements = new WeakReference(value);
            }
        }
#else
        DisplayElementsInTableCollection _displayElements;
        ElementsInTableCollection _groupedElements;
        ElementsInTableCollection _nestedGroupedElements;
        DisplayElementsInTableCollection _nestedDisplayElements;
#endif

        /// <summary>
        /// Initializes a new child table and assigns the parent.
        /// </summary>
        /// <param name="parent">The parent element this object belongs to.</param>
        public ChildTable(Element parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _displayElements = null;
                _groupedElements = null;
                _nestedGroupedElements = null;
                _nestedDisplayElements = null;
                if (this.nestedTableEntry != null && this.nestedTableEntry.Element != null)
                {
                    this.nestedTableEntry.Element.ChildTable = null;
                    this.nestedTableEntry.Element.childTableDisposed = true;
                }

                this.nestedTableEntry = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>Determines if this is the Table.TopLevelGroup.</summary>
        /// <override/>
        public override bool IsTopLevelGroup
        {
            get
            {
                Table parentTable = this.ParentTable;
                return parentTable != null && parentTable.RelationParentTable == null;
            }
        }
        
        /// <summary>
        /// Provides a flat collection of visible elements in the child table. All records, groups,
        /// and sections are only returned by this collection if they are expanded
        /// and meet filter criteria. The collection does not step into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DisplayElementsInTableCollection DisplayElements
        {
            get
            {
                if (_displayElements == null)
                {
                    _displayElements = new DisplayElementsInTableCollection(this);
                }

                return _displayElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of visible elements in the child table. All records, groups,
        /// and sections are only returned by this collection if they are expanded
        /// and meet filter criteria. The collection steps into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public DisplayElementsInTableCollection NestedDisplayElements
        {
            get
            {
                if (_nestedDisplayElements == null)
                {
                    _nestedDisplayElements = new DisplayElementsInTableCollection(this);
                    _nestedDisplayElements._stepInNestedTables = true;
                }

                return _nestedDisplayElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of all elements in the child table. All records, groups,
        /// and sections are returned by this collection no matter if they were expanded
        /// or meet filter criteria. The collection steps into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ElementsInTableCollection NestedElements
        {
            get
            {
                if (_nestedGroupedElements == null)
                {
                    _nestedGroupedElements = new ElementsInTableCollection(this);
                    _nestedGroupedElements._stepInNestedTables = true;
                }

                return _nestedGroupedElements;
            }
        }

        /// <summary>
        /// Provides a flat collection of all elements in the child table. All records, groups,
        /// and sections are returned by this collection no matter if they were expanded
        /// or meet filter criteria. The collection does not step into nested tables.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public ElementsInTableCollection Elements
        {
            get
            {
                if (_groupedElements == null)
                {
                    _groupedElements = new ElementsInTableCollection(this);
                }

                return _groupedElements;
            }
        }

        /// <summary>
        /// Determines if this child table contains <see cref="Table.CurrentElement"/>.
        /// </summary>
        /// <returns>True if child table contains <see cref="Table.CurrentElement"/>; False otherwise.</returns>
        public bool ContainsCurrentRecordOrChildTable()
        {
            Table childTableTable = ParentTable;
            return DisplayElements.Contains((Element)childTableTable.CurrentElement);
        }

        /// <summary>
        /// Deactivates the current record; calls <see cref="CurrentRecordManager.LeaveRecord"/> on the
        /// <see cref="Element.ParentTable"/>.
        /// </summary>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        public void LeaveRecord(bool cancelEditIfNotValid)
        {
            Table childTableTable = ParentTable;
            ChildTable savedFilteredChildTable = childTableTable.FilteredChildTable;
            try
            {
                childTableTable.FilteredChildTable = this;
                ParentTable.CurrentRecordManager.LeaveRecord(cancelEditIfNotValid);
            }
            finally
            {
                childTableTable.FilteredChildTable = savedFilteredChildTable != null && savedFilteredChildTable.ParentElement != null ? savedFilteredChildTable : null;
            }
        }

        /// <summary>Gets the name of table descriptor.</summary>
        /// <override/>
        public override string Name
        {
            get
            {
                return ParentTable.TableDescriptor.Name;
            }
        }

        internal NestedTablesTreeTableEntry NestedTableEntry
        {
            get
            {
                return nestedTableEntry;
            }

            set
            {
                nestedTableEntry = value;
            }
        }

        /// <summary>A reference to the parent nested table this element belongs to.</summary>
        /// <override/>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public NestedTable ParentNestedTable
        {
            get
            {
                Table relationParentTable = this.ParentTable.RelationParentTable;
                if (relationParentTable != null)
                {
                    if (nestedTableEntry == null)
                    {
                        relationParentTable.EnsureInitialized(this, false);
                        relationParentTable.GetVisibleCount(); // will trigger updating records thus establishing a link to
                        // this nested table when the records AdjustRecordRowCount is hit. NestedTableEntry should be
                        //// set for this object!
                    }
                }

                return nestedTableEntry != null ? nestedTableEntry.Element : null;
            }
        }

        ////        bool IsRelationChildList()
        ////        {
        ////            return nestedTableEntry != null;
        ////        }
        ////

        /// <summary>
        /// Returns the parent the element belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override Element ParentDisplayElement
        {
            get
            {
                return ParentNestedTable;
            }
        }

        /// <summary>Walks up parent branches and reset summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            base.InvalidateSummariesBottomUp();
            if (this.nestedTableEntry != null)
            {
                this.nestedTableEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>Resets the counter.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            base.InvalidateCounter();
            if (this.nestedTableEntry != null && !this.nestedTableEntry.IsCounterDirty())
            {
                this.nestedTableEntry.InvalidateCounterBottomUp(true);
            }
        }

        /// <summary>Walks down to child branches and resets counter.</summary>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            base.InvalidateCounterTopDown(notifyCounterSource);
        }
    }
}