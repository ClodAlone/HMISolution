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
using Syncfusion.Data;


namespace Syncfusion.UI.Xaml.Grid
{
    public static class GridIndexResolver
    {
        public static int ResolveToTableSummaryIndex(this SfDataGrid dataGrid, int rowIndex)
        {
            if (dataGrid.VisualContainer != null)
            {
                var endindex = dataGrid.VisualContainer.RowCount - 1;
                var startindex = dataGrid.VisualContainer.RowCount - dataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom);
                if (rowIndex >= startindex && rowIndex <= endindex)
                    return rowIndex - startindex;

                if (rowIndex >= 0)
                    return rowIndex;
            }
            return -1;
        }

        public static int ResolveToRecordIndex(this SfDataGrid dataGrid, int rowIndex)
        {
            if (dataGrid.VisualContainer != null && dataGrid.View!=null)
            {
                rowIndex = rowIndex - dataGrid.ResolveStartIndexBasedOnPosition();
                if (dataGrid.GridModel.HasGroup)
                {
                    if (rowIndex > dataGrid.View.TopLevelGroup.DisplayElements.Count)
                        rowIndex = rowIndex - dataGrid.VisualContainer.FooterRows;
                }
                else
                {
#if !WP
                    if(dataGrid.DetailsViewManager.HasDetailsView)
                        rowIndex = rowIndex/(dataGrid.DetailsViewDefinition.Count + 1);
#endif
                    if (rowIndex > dataGrid.View.Records.Count)
                        rowIndex = rowIndex - dataGrid.VisualContainer.FooterRows;
                }
                if (rowIndex >= 0)
                    return rowIndex;
            }
            return -1;
        }

        

        /// <summary>
        /// Will return RowIndex when passing the resolved RecordIndex
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="recordIndex"></param>
        /// <returns></returns>
        public static int ResolveToRowIndex(this SfDataGrid dataGrid, int recordIndex)
        {
            if (recordIndex > -1)
            {
                if (dataGrid.GridModel.HasGroup)
                {
                    if (recordIndex > -1)
                    {
                        var groupPos = recordIndex + dataGrid.ResolveStartIndexBasedOnPosition();
                        return groupPos;
                    }
                    return -1;
                }
                else
                {
#if !WP
                    var index = recordIndex*(dataGrid.DetailsViewDefinition.Count + 1);
#else
                    var index = recordIndex;
#endif
                    return index + dataGrid.ResolveStartIndexBasedOnPosition();
                }
            }

            return -1;
        }

        public static int ResolveToRowIndex(this SfDataGrid dataGrid, object recordItem)
        {
            var recordIndex = dataGrid.View.Records.IndexOfRecord(recordItem);
            if (recordIndex < 0)
                return -1;
            if (!dataGrid.GridModel.HasGroup)
            {
#if !WP
                if (dataGrid.DetailsViewManager.HasDetailsView)
                    return (recordIndex * (dataGrid.DetailsViewDefinition.Count+1)) + dataGrid.ResolveStartIndexBasedOnPosition();
                else
#endif
                    return recordIndex + dataGrid.ResolveStartIndexBasedOnPosition();
            }
            else
            {
                var record = dataGrid.View.Records.GetRecord(recordItem);
                if (record.Parent != null)
                {
                    var grpRecordIndex = dataGrid.View.TopLevelGroup.DisplayElements.IndexOf(record);
                    if (grpRecordIndex > -1)
                    {
                        var groupPos = grpRecordIndex + dataGrid.ResolveStartIndexBasedOnPosition();
                        return groupPos;
                    }
                }
            }
            return -1;
        }

        public static int ResolveStartIndexOfGroup(this SfDataGrid dataGrid, Group group)
        {
            if (dataGrid.GridModel.HasGroup)
            {
                var startIndex = dataGrid.ResolveStartIndexBasedOnPosition();
                var grpIdx = dataGrid.View.TopLevelGroup.DisplayElements.IndexOf(group);
                return grpIdx + startIndex;
            }
            return -1;
        }

        public static int ResolveStartIndexBasedOnPosition(this SfDataGrid dataGrid)
        {
            return dataGrid.HeaderLineCount;
        }
        //Columns
        //Returns GridColumn Index from VisibleColumn Index
        internal static int ResolveToGridVisibleColumnIndex(this SfDataGrid dataGrid, int visibleColumnIndex)
        {
#if !WP
            var indentColumnCount = (dataGrid.View != null ? dataGrid.View.GroupDescriptions.Count : 0) +
                (dataGrid.DetailsViewManager.HasDetailsView ? 1 : 0);
            int resolvedIndex = visibleColumnIndex - (indentColumnCount + (dataGrid.ShowRowHeader ? 1 : 0));
#else
            int resolvedIndex = visibleColumnIndex - (dataGrid.View != null ? dataGrid.View.GroupDescriptions.Count : 0);
#endif
            return resolvedIndex;
        }

        //Returns VisibleColumn index from GridColumn Index
        public static int ResolveToScrollColumnIndex(this SfDataGrid dataGrid, int gridColumnIndex)
        {
#if !WP
            var indentColumnCount = (dataGrid.DetailsViewManager.HasDetailsView ? 1 : 0) +
                (dataGrid.View != null ? dataGrid.View.GroupDescriptions.Count : 0);
            return ((dataGrid.ShowRowHeader ? 1 : 0) + indentColumnCount) + gridColumnIndex;
#else
            return (dataGrid.ShowRowHeader ? 1 : 0) + (dataGrid.View != null ? dataGrid.View.GroupDescriptions.Count : 0) + gridColumnIndex;
#endif
        }

#if !WP

        public static int ResolveToGroupRecordIndexForDetailsView(this SfDataGrid dataGrid, int rowIndex)
        {
            rowIndex = rowIndex - dataGrid.ResolveStartIndexBasedOnPosition();
            if (dataGrid.GridModel.HasGroup)
            {
                if (dataGrid.DetailsViewDefinition != null && dataGrid.DetailsViewDefinition.Count > 0)
                {
                    var indexToReturn = -1;
                    if (rowIndex > -1 && rowIndex < dataGrid.View.TopLevelGroup.DisplayElements.Count)
                    {
                        var displayEl = dataGrid.View.TopLevelGroup.DisplayElements[rowIndex];
                        var isRecord = (displayEl is RecordEntry) && !(displayEl is NestedRecordEntry);
                        if (isRecord)
                        {
                            var record = (displayEl as RecordEntry);
                            indexToReturn = dataGrid.View.TopLevelGroup.DisplayElements.IndexOf(record);
                        }
                        else if (displayEl is NestedRecordEntry)
                        {
                            var parent = (displayEl as NestedRecordEntry).Parent as RecordEntry;
                            indexToReturn = dataGrid.View.TopLevelGroup.DisplayElements.IndexOf(parent);
                        }
                        rowIndex = indexToReturn;
                    }
                }
                return rowIndex;
            }
            return -1;
        }

        public static int GetOrderForDetailsViewBasedOnIndex(this SfDataGrid dataGrid, int actualRowIdx)
        {
            // simply find the index which is not in DetailsView Index
            var counter0 = 0;
            for (int i = actualRowIdx; i > 0; i--)
            {
                if (!dataGrid.IsInDetailsViewIndex(i))
                {
                    break;
                }
                counter0++;
            }
            return counter0;
        }

        public static bool IsInDetailsViewIndex(this SfDataGrid dataGrid, int rowIdx)
        {
            var startIdx = dataGrid.ResolveStartIndexBasedOnPosition();
            var counter0 = Math.Max((rowIdx - startIdx), 0);

            if (dataGrid.GridModel.HasGroup)
            {
                var displayEl = dataGrid.View.TopLevelGroup.DisplayElements[counter0];
                return displayEl is NestedRecordEntry;
            }
            if (dataGrid.DetailsViewManager.HasDetailsView)
                return (counter0%(dataGrid.DetailsViewDefinition.Count + 1)) != 0;
            return false;
        }

        public static int GetGridDetailsViewRowIndex(this SfDataGrid dataGrid, DetailsViewDataGrid detailsViewDataGrid)
        {
            if (detailsViewDataGrid != null)
            {
                var dataRow = dataGrid.RowGenerator.Items.FirstOrDefault(
                    row => (row is DetailsViewDataRow) && (row as DetailsViewDataRow).DetailsViewDataGrid.Equals(detailsViewDataGrid));
                if (dataRow != null) return dataRow.RowIndex;
            }
            return -1;
        }

        public static object GetGridDetailsViewRecord(this SfDataGrid dataGrid, DetailsViewDataGrid detailsViewDataGrid)
        {
            if (detailsViewDataGrid != null)
            {
                var index = dataGrid.GetGridDetailsViewRowIndex(detailsViewDataGrid);
                if (index != -1)
                {
                    RecordEntry record = null;
                    if (dataGrid.GridModel.HasGroup)
                    {
                        var recordIndex = dataGrid.ResolveToGroupRecordIndexForDetailsView(index);
                        record = dataGrid.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                    }
                    else
                    {
                        var recordIndex = dataGrid.ResolveToRecordIndex(index);
                        record = dataGrid.View.Records[recordIndex];
                    }
                    return record;
                }
            }
            return null;
        }

        public static int ResolveToStartColumnIndex(this SfDataGrid dataGrid)
        {
            int startIndex = 0;
            if (dataGrid.ShowRowHeader)
                startIndex += 1;
            if (dataGrid.View != null && dataGrid.View.GroupDescriptions.Count > 0)
                startIndex += dataGrid.View.GroupDescriptions.Count;
            if (dataGrid.DetailsViewManager.HasDetailsView)
                startIndex += 1;
            return startIndex;
        }
#endif

        public static int GetTableSummaryCount(this SfDataGrid grid, TableSummaryRowPosition position)
        {
            return position == TableSummaryRowPosition.Top ? (grid.TableSummaryRows.Where(row => (row is GridTableSummaryRow && (row as GridTableSummaryRow).Position == TableSummaryRowPosition.Top)).Count()) : (grid.TableSummaryRows.Count - grid.GetTableSummaryCount(TableSummaryRowPosition.Top));
        }

        public static int GetHeaderIndex(this SfDataGrid grid)
        {
            return (grid.AddNewRowPosition == AddNewRowPosition.Top ? grid.HeaderLineCount - 2 : grid.HeaderLineCount - 1) - (grid.GetTableSummaryCount(TableSummaryRowPosition.Top));
        }

        public static bool IsAddNewIndex(this SfDataGrid dataGrid,int rowIndex)
        {
            if (dataGrid.AddNewRowPosition == AddNewRowPosition.Top)
                return rowIndex == dataGrid.HeaderLineCount - 1;
            else if (dataGrid.AddNewRowPosition == AddNewRowPosition.Bottom)
                return rowIndex == dataGrid.VisualContainer.RowCount - (dataGrid.GetTableSummaryCount(TableSummaryRowPosition.Bottom) + 1);
            return false;
        }

        public static bool IsHeaderTableSummaryRow(this SfDataGrid dataGrid, int rowIndex)
        {
            return (rowIndex <= dataGrid.HeaderLineCount - 1 - (dataGrid.AddNewRowPosition == AddNewRowPosition.Top ? 1 : 0) && rowIndex > dataGrid.StackedHeaderRows.Count);
        }

    }
}


