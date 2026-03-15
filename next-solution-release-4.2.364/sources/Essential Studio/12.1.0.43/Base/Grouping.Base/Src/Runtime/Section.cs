//-------------------------------------------------------------------------------------------------
// <copyright file="Section.cs" company="syncfusion">
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
    /// An abstract base class for sections in a group. A group has
    /// multiple sections such as CaptionSection, SummarySection, GroupsDetailsSection,
    /// and RecordsDetailsSection.
    /// </summary>
    public abstract class Section : Element
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        protected Section(Group parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            sectionEntry = null;
            base.Dispose(disposing);
        }

        SectionsTreeTableEntry sectionEntry;

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry() 
        { 
            return SectionEntry; 
        }

        internal SectionsTreeTableEntry SectionEntry
        {
            get
            {
                return sectionEntry;
            }

            set
            {
                sectionEntry = value;
            }
        }

        /// <summary>Walks up to the parent branches and resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            if (this.SectionEntry != null)
            {
                this.SectionEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>
        /// Gets / sets the group this elements belongs to.
        /// </summary>
        public new Group ParentElement
        {
            get
            {
                return base.ParentElement as Group;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>Creates a counter for the current element.</summary>
        /// <returns>A counter for the current element.</returns>
        /// <override/>
        public override ITreeTableCounter GetCounter()
        {
            if (this.GetVisibleInParent())
            {
                return CounterFactory.CreateCounter(GetVisibleCount(), GetYAmountCount(), GetFilteredRecordCount(), GetElementCount(), GetRecordCount(), GetCustomCount(), GetVisibleCustomCount());
            }
            else
            {
                return CounterFactory.CreateCounter(0, 0, GetFilteredRecordCount(), GetElementCount(), GetRecordCount(), GetCustomCount(), 0);
            }
        }
    }
}
