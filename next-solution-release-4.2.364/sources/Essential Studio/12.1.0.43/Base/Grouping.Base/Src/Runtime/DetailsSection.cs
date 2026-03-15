//-------------------------------------------------------------------------------------------------
// <copyright file="DetailsSection.cs" company="syncfusion">
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
    /// The section base class for the <see cref="RecordsInDetailsCollection"/> or <see cref="GroupsInDetailsCollection"/>. Instances of this
    /// class are accessed through the <see cref="Group.Details"/> property of a parent <see cref="Group"/>.
    /// </summary>
    public abstract class DetailsSection : Section, IContainerElement
    {
        Table table = null;

        /// <summary>
        /// Gets the parent table this section belongs to.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public override Table ParentTable
        {
            get
            {
                if (table == null)
                {
                    table = GetRootTable();
                }

                return table;
            }

            set
            {
                table = value;
            }
        }

        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        protected DetailsSection(Group parent)
            : base(parent)
        {
        }

        bool IContainerElement.ShouldStepIntoElements()
        {
            return true;
        }

        ElementTreeTable IElementTreeTableSource.GetChildElementTreeTable(bool displayOrder)
        {
            throw new MissingMethodException("must overide this method");
        }

        /// <summary>Resets the counter.</summary>
        /// <override/>
        public override void InvalidateCounter()
        {
            base.InvalidateCounter();
        }

        /// <summary>Resets the counter for all the elements.</summary>
        /// <param name="notifyCounterSource">Indicates whether to notify the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
            TreeEntries.InvalidateCounterTopDown(notifyCounterSource);
        }

        /// <summary>Resets the summaries for all the elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
            TreeEntries.InvalidateSummariesTopDown();
        }

        /// <summary>Resets the summary.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Returns number of visible elements.</summary>
        /// <returns>Number of visible elements.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return TreeEntries.VisibleCount;
        }

        /// <summary>Returns custom count for visible elements.</summary>
        /// <returns>Visible custom count.</returns>
        /// <override/>
        public override double GetVisibleCustomCount()
        {
            return TreeEntries.VisibleCustomCount;
        }

        /// <summary>Gets custom count for the element.</summary>
        /// <returns>Custom count.</returns>
        /// <override/>
        public override double GetCustomCount()
        {
            return TreeEntries.CustomCount;
        }

        /// <summary>Returns element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return TreeEntries.YAmountCount;
        }

        /// <summary>Returns element count.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return TreeEntries.ElementCount;
        }

        /// <summary>Returns filtered record count.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return TreeEntries.FilteredRecordCount;
        }

        /// <summary>Returns number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return TreeEntries.RecordCount;
        }

        internal virtual GroupSortOrderTreeTable GroupSortOrderTreeTable
        {
            get
            {
                return null;
            }
        }

        internal virtual GroupCategoryTreeTable GroupCategoryTreeTable
        {
            get
            {
                return null;
            }
        }

        internal virtual ElementTreeTable GroupDisplayEntries
        {
            get
            {
                return null;
            }
        }

        internal virtual SortedRecordsTreeTable RecordTreeEntries
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Determines if this section contains records or groups.
        /// </summary>
        public bool HasRecords
        {
            get
            {
                return RecordTreeEntries != null;
            }
        }

        internal void BeginInit()
        {
            if (TreeEntries != null)
            {
                TreeEntries.BeginInit();
            }
        }

        internal void EndInit()
        {
            if (TreeEntries != null)
            {
                TreeEntries.EndInit();
            }
        }

        internal bool IsInitializing
        {
            get
            {
                if (TreeEntries != null)
                {
                    return TreeEntries.IsInitializing;
                }

                return false;
            }
        }

        /// <summary>Returns string representation of DetailsSection.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            Group group = ParentGroup;
            if (group != null)
            {
                return GetType().Name + " " + group.CategoriesToString();
            }
            else
            {
                return GetType().Name;
            }
        }
    }
}
