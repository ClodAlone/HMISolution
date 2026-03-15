//-------------------------------------------------------------------------------------------------
// <copyright file="GroupHeaderSection.cs" company="syncfusion">
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
    /// A place holder section with 1 visible element for showing a header above a group in <see cref="Table.DisplayElements"/>.
    /// </summary>
    public class GroupHeaderSection : Section, IDisplayElement
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public GroupHeaderSection(Group parent)
            : base(parent)
        {
        }

        /// <summary>Gets the kind of the element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.GroupHeader; }
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Returns the height for the element.</summary>
        /// <returns>Element Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultGroupHeaderSectionHeight;
        }

        /// <summary>Resets the counter fields for all elements.</summary>
        /// <param name="notifyCounterSource">If true notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        /// <summary>Resets the summaries for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
        }

        /// <summary>Resets the summaries.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Gets the filtered record count.</summary>
        /// <returns>Number of filtered records.</returns>
        /// <override/>
        public override int GetFilteredRecordCount()
        {
            return 0;
        }

        /// <summary>Gets the number of elements.</summary>
        /// <returns>Element count.</returns>
        /// <override/>
        public override int GetElementCount()
        {
            return 1;
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
