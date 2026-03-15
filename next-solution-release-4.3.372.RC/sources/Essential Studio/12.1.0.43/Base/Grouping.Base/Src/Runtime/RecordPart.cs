//-------------------------------------------------------------------------------------------------
// <copyright file="RecordPart.cs" company="syncfusion">
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
    /// A part in a record that serves as a container for the <see cref="RecordRow"/>, <see cref="RecordPreviewRow"/>,
    /// or <see cref="NestedTable"/> element collections in a record.
    /// </summary>
    public abstract class RecordPart : Element
    {
        /// <summary>
        /// Initializes a new object with the given record as parent.
        /// </summary>
        /// <param name="parent">The parent record this object is created in.</param>
        public RecordPart(Record parent)
            : base(parent)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            sectionEntry = null;
            base.Dispose(disposing);
        }

        RecordPartsTreeTableEntry sectionEntry;

        /// <summary>
        /// The ElementTreeTableEntry this element is associated with (either SectionsTreeTableEntry or SortedRecordsTreeTableEntry).
        /// <returns>returns ElementTreeTableEntry</returns>
        /// </summary>
        /// <returns>returns ElementTreeTableEntry</returns>
        /// <override/>
        internal override ElementTreeTableEntry GetElementEntry() 
        { 
            return RecordPartEntry; 
        }

        internal RecordPartsTreeTableEntry RecordPartEntry
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

        /// <summary>Walks up parent branches and resets summaries.</summary>
        /// <override/>
        public override void InvalidateSummariesBottomUp()
        {
            if (this.RecordPartEntry != null)
            {
                this.RecordPartEntry.InvalidateSummariesBottomUp(true);
            }
        }

        /// <summary>
        /// Returns the parent this element belongs to.
        /// </summary>
        public new Record ParentElement
        {
            get
            {
                return base.ParentElement as Record;
            }

            set
            {
                base.ParentElement = value;
            }
        }

        /// <summary>Get the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Gets the number of records.</summary>
        /// <returns>Record count.</returns>
        /// <override/>
        public override int GetRecordCount()
        {
            return 0;
        }
    }
}
