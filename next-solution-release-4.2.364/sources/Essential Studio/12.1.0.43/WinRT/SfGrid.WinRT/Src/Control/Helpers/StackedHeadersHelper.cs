#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Data.Extensions;

namespace Syncfusion.UI.Xaml.Grid.Helpers
{
    public static class StackedHeadersHelper
    {
        /// <summary>
        /// Gets the common Column in the same StackedHeaderRow
        /// </summary>
        /// <param name="childColumns"></param>
        /// <param name="header"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<int> IntersectedChildColumn(this SfDataGrid sfGrid, List<int> childColumns, StackedHeaderRow header, StackedColumn column)
        {
            var intersectedIndex = new List<int>();
            foreach (var item in header.StackedColumns)
            {
                if (item == column)
                    return intersectedIndex;
                if (item.ChildColumnsIndex != null)
                    intersectedIndex = intersectedIndex.Union(childColumns.Intersect(item.ChildColumnsIndex)).ToList();
            }
            return intersectedIndex;
        }

        /// <summary>
        /// Get RowSpan for the given covered cell and rowIndex
        /// </summary>
        /// <param name="currHeaderCell"></param>
        /// <param name="rowindex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static int GetHeightIncremeantationLimit(this SfDataGrid sfGrid, CoveredCellInfo currHeaderCell, int rowindex)
        {
            if (rowindex >= 0)
            {
                int incrementHeightLevel = 0;
                var prevHeaderCells = (sfGrid.RowGenerator.Items.FirstOrDefault(row => row.RowIndex == rowindex) as SpannedDataRow).CoveredCells;

                foreach (var prevCell in prevHeaderCells)
                {
                    if (prevCell.Left <= currHeaderCell.Left && prevCell.Right >= currHeaderCell.Right)
                        break;

                    if (prevHeaderCells.IndexOf(prevCell) == prevHeaderCells.Count - 1)
                    {
                        incrementHeightLevel++;
                        incrementHeightLevel += sfGrid.GetHeightIncremeantationLimit(currHeaderCell, rowindex - 1);
                        break;
                    }
                }

                if (prevHeaderCells.Count == 0)
                {
                    incrementHeightLevel++;
                    incrementHeightLevel += sfGrid.GetHeightIncremeantationLimit(currHeaderCell, rowindex - 1);
                }
                return incrementHeightLevel;
            }
            return 0;
        }

        /// <summary>
        /// Get ChildSequence no respect to the GridColumns for ChildColumns in StackedHeaderRow
        /// </summary>
        /// <param name="column"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<int> GetChildSequence(this SfDataGrid sfgrid, StackedColumn column, int rowIndex)
        {
            List<int> childSequencNo = new List<int>();
            int sequenceNo = -1;
            if (column != null)
            {
                var childColumns = column.ChildColumns.Split(',');
                foreach (var child in childColumns)
                {
                    if (sfgrid.Columns.Any(col => col.MappingName == child))
                    {
                        sequenceNo = sfgrid.Columns.IndexOf(sfgrid.Columns.FirstOrDefault(col => col.MappingName == child));
                        childSequencNo.Add(sequenceNo);
                    }
                    else
                    {
                        var childSubSequence = sfgrid.GetChildSequence(sfgrid.GetStackedColumn(child, rowIndex + 1), rowIndex);
                        childSequencNo = childSequencNo.Union(childSubSequence).ToList();
                    }
                }
                return childSequencNo;
            }
            return childSequencNo;
        }

        /// <summary>
        /// Gets StackedColumn corresponding to the given name and index
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private static StackedColumn GetStackedColumn(this SfDataGrid sfgrid, string columnName, int rowIndex)
        {
            StackedHeaderRow header = null;
            StackedColumn column = null;
            if (rowIndex >= sfgrid.StackedHeaderRows.Count)
                return null;
            header = sfgrid.StackedHeaderRows[rowIndex];
            while (true)
            {
                column = header.StackedColumns.FirstOrDefault(child => child.HeaderText == columnName);
                if (column == null)
                    column = sfgrid.GetStackedColumn(columnName, rowIndex + 1);
                return column;
            }
        }

        /// <summary>
        /// Gets the childColumns for the given StackedColumn
        /// </summary>
        /// <param name="childColumns"></param>
        /// <param name="header"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<int> CheckChildSequence(this SfDataGrid sfgrid, List<int> childColumns, StackedHeaderRow header, StackedColumn column)
        {
            foreach (var col in header.StackedColumns)
            {
                var childs = col.ChildColumns.Split(',');
                childs.ForEach(child =>
                {
                    if (child.Equals(column.HeaderText))
                        childColumns = col.ChildColumnsIndex;
                });
            }
            return childColumns;
        }
    }
}
