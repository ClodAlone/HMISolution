//-------------------------------------------------------------------------------------------------
// <copyright file="GridSummaries.cs" company="syncfusion">
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
using System.Text;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;

using ISummary = Syncfusion.Collections.BinaryTree.ITreeTableSummary;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Summarizes string fields and lets you determine the maximum length of a string in the column.
    /// </summary>
    public sealed class GridMaxLengthSummary : SummaryBase
    {
        int maxLength;

        /// <summary>
        /// The initial summary object for empty records or tables.
        /// </summary>
        public static readonly GridMaxLengthSummary Empty = new GridMaxLengthSummary(0);

        /// <summary>
        /// Creates a summary object for the specified SummaryDescriptor and Record.
        /// </summary>
        /// <param name="sd">The summary descriptor.</param>
        /// <param name="element">The element with data.</param>
        /// <returns>A new summary object.</returns>
        public static ISummary CreateSummaryFromElementMethod(SummaryDescriptor sd, Element element)
        {
            if (element is ColumnHeaderSection)
            {
                GridTableDescriptor td = (GridTableDescriptor) element.ParentTableDescriptor;
                if (td != null && sd.FieldDescriptor != null)
                {
                    GridColumnDescriptor cd = td.Columns[sd.Name];
                    if (cd != null)
                    {
                        return new GridMaxLengthSummary(cd.HeaderText.Length + 3); //// + 3: leave space for sort indicators ...
                    }
                }

                return Empty;
            }
            else if (element is GridSummaryRow)
            {
                if (sd.Name.EndsWith("AutoSizeMaxLength"))
                {
                    GridSummaryRow summaryRow = (GridSummaryRow) element;
                    GridSummaryRowDescriptor summaryRowDescriptor = summaryRow.SummaryRowDescriptor;
                    if (summaryRowDescriptor.Visible)
                    {
                        foreach (GridSummaryColumnDescriptor sc in summaryRowDescriptor.SummaryColumns)
                        {
                            if (sc.DisplayColumn != string.Empty)
                            {
                                if (sc.DisplayColumn + "AutoSizeMaxLength" == sd.Name)
                                {
                                    return new GridMaxLengthSummary(sc.MaxLength);
                                }
                            }
                        }
                    }
                }

                return Empty;
            }

            Record record = element as Record;
            object obj = sd.GetValue(record);
            bool isNull = obj == null || obj is DBNull;
            if (isNull)
            {
                return Empty;
            }
            else
            {
                string text = Convert.ToString(obj).TrimEnd();
                return new GridMaxLengthSummary(text.Length);
            }
        }

        /// <summary>
        /// Initializes a new summary object with the specified values.
        /// </summary>
        /// <param name="maxLength">Maximum length.</param>
        public GridMaxLengthSummary(int maxLength)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// The maximum length value.
        /// </summary>
        public int MaxLength
        {
            get
            {
                return maxLength;
            }
        }

        /// <override/>
        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object.</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public override SummaryBase Combine(SummaryBase other)
        {
            if (other is GridMaxLengthSummary)
                return Combine((GridMaxLengthSummary)other);
            return other;
        }

        /// <summary>
        /// Combines the values of this summary with another summary and returns
        /// a new summary object.
        /// </summary>
        /// <param name="other">Another summary object (of the same type).</param>
        /// <returns>A new summary object with combined values of both summaries.</returns>
        public GridMaxLengthSummary Combine(GridMaxLengthSummary other)
        {
            if (other != null && other.maxLength >= this.maxLength)
            {
                return other;
            }
            else
            {
                return this;
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return "MaxLength = " + this.maxLength.ToString();
        }
    }
}
