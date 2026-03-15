//-------------------------------------------------------------------------------------------------
// <copyright file="SummarySection.cs" company="syncfusion">
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
    /// Identifies element as summary section.
    /// </summary>
    public interface ISummarySection : IDisposable
    {
    }

    /// <summary>
    /// The summary section with one visible element for showing a summary below a group in <see cref="Table.DisplayElements"/>. The summary
    /// information can be determined by calling <see cref="Group.GetSummaries"/> on the <see cref="Group"/> that this summary section
    /// belongs to.
    /// </summary>
    public class SummarySection : Section, ISummarySection, IDisplayElement
    {
        /// <summary>
        /// Initializes a new section in the specified group.
        /// </summary>
        /// <param name="parent">The group this section is created in</param>
        public SummarySection(Group parent)
            : base(parent)
        {
        }

        /// <summary>Gets the kind of display element.</summary>
        /// <override/>
        public override DisplayElementKind Kind
        {
            get { return DisplayElementKind.Summary; }
        }

        /// <summary>Gets the number of visible elements.</summary>
        /// <returns>Visible element count.</returns>
        /// <override/>
        public override int GetVisibleCount()
        {
            return 1;
        }

        /// <summary>Gets the element height.</summary>
        /// <returns>Element height.</returns>
        /// <override/>
        public override double GetYAmountCount()
        {
            return ParentTable.DefaultSummaryRowHeight;
        }

        /// <summary>Resets the counter for all elements.</summary>
        /// <param name="notifyCounterSource">When true notifies the counter source.</param>
        /// <override/>
        public override void InvalidateCounterTopDown(bool notifyCounterSource)
        {
        }

        /// <summary>Resets the summary for all elements.</summary>
        /// <override/>
        public override void InvalidateSummariesTopDown()
        {
        }

        /// <summary>Resets the summary.</summary>
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

        /// <override/>
        /// <summary>
        /// Gets summary information for the current element and all its childs.
        /// </summary>
        /// <param name="parentTable">A reference to the parent table.</param>
        /// <param name="summaryChanged">Returns true if changes were detected.</param>
        /// <returns>Summary information.</returns>
        public override ITreeTableSummary[] GetSummaries(Table parentTable, out bool summaryChanged)
        {
            summaryChanged = false;
            SummaryDescriptorCollection sdc = parentTable.TableDescriptor.Summaries;
            return sdc.CreateSummaries(this);
        }

        /// <summary>Returns a string holding the current object.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            Syncfusion.Collections.BinaryTree.ITreeTableSummary[] summaries;
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            if (ParentGroup != null)
            {
                summaries = ParentGroup.GetSummaries(this.ParentTable);
                int i = 0;
                if (summaries != null)
                {
                    foreach (ITreeTableSummary sc in summaries)
                    {
                        sb.Append(Environment.NewLine);
                        sb.Append("            " + ParentTableDescriptor.Summaries[i++].Name + ": " + sc.ToString());
                    }
                }
            }

            return sb.ToString();
        }
    }
}
