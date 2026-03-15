//-------------------------------------------------------------------------------------------------
// <copyright file="EmptySection.cs" company="syncfusion">
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
    /// An empty section with 1 visible element. Can be used as a place holder to appear in <see cref="Table.DisplayElements"/>. GetYAmountCount will
    /// return <see cref="Table.DefaultEmptySectionHeight"/> of the ParentTable.
    /// </summary>
    public class EmptySection : Section, IDisplayElement
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in.</param>
        public EmptySection(Group parent)
            : base(parent)
        {
        }

        /// <summary>Gets the kind of element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Empty; }
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Height for the empty section.</summary>
        /// <returns>Returns Height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultEmptySectionHeight;
        }

        /// <summary>Not implemented.</summary>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        /// <summary>Not implemented.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
        }

        /// <summary>Not implemented.</summary>
        /// <override/>
        public override void InvalidateSummary()
        {
        }

        /// <summary>Gets the number of filtered records.</summary>
        /// <returns>Filtered record count.</returns>
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
