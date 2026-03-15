#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.Data;
#if WinRT
using Windows.UI.Xaml.Media;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Data;
#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
    /// <summary>
    /// Rederer for Covered cell which is used in Summary Rows and GroupCaption
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public class GridSummaryCellRenderer : GridVirtualizingCellRenderer<GridGroupSummaryCell,GridGroupSummaryCell>
    {
        public GridSummaryCellRenderer()
        {
            this.SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
            this.IsEditable = false;
            this.IsFocusible = false;
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridGroupSummaryCell uiElement, GridColumn column, object dataContext)
        {
            if (dataContext is SummaryRecordEntry)
            {
                var record = dataContext as SummaryRecordEntry;
                if (record.SummaryRow.ShowSummaryInRow)
                    uiElement.Content = SummaryCreator.GetSummaryDisplayTextForRow(record, this.DataGrid.View);
                else
                    uiElement.Content = SummaryCreator.GetSummaryDisplayText(record, column.MappingName, this.DataGrid.View);
            }
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridGroupSummaryCell element, GridColumn column, object dataContext)
        {
            if (element.DataContext is SummaryRecordEntry)
            {
                var record = element.DataContext as SummaryRecordEntry;
                if (record.SummaryRow.ShowSummaryInRow)
                    element.Content = SummaryCreator.GetSummaryDisplayTextForRow(record, this.DataGrid.View);
                else
                    element.Content = SummaryCreator.GetSummaryDisplayText(record, column.MappingName, this.DataGrid.View);
            }
        }

        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var summaryCell = cell as GridGroupSummaryCell;
            Style newStyle = null;
            if (summaryCell != null && DataGrid != null)
            {
                bool hasGroupSummaryCellStyleSelector = DataGrid.hasGroupSummaryCellStyleSelector;
                bool hasGroupSummaryCellStyle = DataGrid.hasGroupSummaryCellStyle;

                if (!hasGroupSummaryCellStyleSelector && !hasGroupSummaryCellStyle)
                    return;
#if !SILVERLIGHT && !WP
                if (hasGroupSummaryCellStyleSelector&& hasGroupSummaryCellStyle)
                {
                    newStyle = DataGrid.GroupSummaryCellStyleSelector.SelectStyle(record, cell);
                    newStyle = newStyle ?? DataGrid.GroupSummaryCellStyle;
                }
                else if (hasGroupSummaryCellStyleSelector)
                {
                    newStyle = DataGrid.GroupSummaryCellStyleSelector.SelectStyle(record, cell);
                }
#endif
                else if (hasGroupSummaryCellStyle)
                {
                    newStyle = DataGrid.GroupSummaryCellStyle;
                }
            }
            summaryCell.Style = newStyle;
        }
    }

    [ClassReference(IsReviewed = false)]
    public class GridCaptionSummaryCellRenderer : GridVirtualizingCellRenderer<GridCaptionSummaryCell, GridCaptionSummaryCell>
    {
        public GridCaptionSummaryCellRenderer()
        {
            this.SupportsRenderOptimization = false;
            this.UseOnlyRendererElement = true;
            this.IsEditable = false;
            this.IsFocusible = false;
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridCaptionSummaryCell uiElement, GridColumn column, object dataContext)
        {
            if (dataContext is Group)
            {
                var groupRecord = dataContext as Group;
                if (this.DataGrid.CaptionSummaryRow == null)
                {
                    var groupedColumn = this.GetGroupedColumn(groupRecord);
                    string stringFormat = this.DataGrid.GroupCaptionTextFormat ?? this.DataGrid.GroupCaptionConstant;
                    uiElement.Content = this.DataGrid.View.TopLevelGroup.GetGroupCaptionText(groupRecord, stringFormat, groupedColumn.HeaderText);
                }
                else if (this.DataGrid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    uiElement.Content = SummaryCreator.GetSummaryDisplayTextForRow(groupRecord.SummaryDetails, this.DataGrid.View);
                }
                else
                    uiElement.Content = SummaryCreator.GetSummaryDisplayText(groupRecord.SummaryDetails, column.MappingName, this.DataGrid.View);
            }
        }

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridCaptionSummaryCell element, GridColumn column, object dataContext)
        {
            if (element.DataContext is Group && this.DataGrid.View.GroupDescriptions.Count > 0)
            {
                var groupRecord = element.DataContext as Group;
                var groupedColumn = this.GetGroupedColumn(groupRecord);
                if (this.DataGrid.CaptionSummaryRow == null)
                {
                    if (this.DataGrid.View.GroupDescriptions.Count < groupRecord.Level)
                        return;
                    var stringFormat = this.DataGrid.GroupCaptionTextFormat ?? this.DataGrid.GroupCaptionConstant;
                    element.Content = this.DataGrid.View.TopLevelGroup.GetGroupCaptionText(groupRecord, stringFormat, groupedColumn.HeaderText);
                }
                else if (this.DataGrid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    element.Content = SummaryCreator.GetSummaryDisplayTextForRow(groupRecord.SummaryDetails, this.DataGrid.View, groupedColumn.HeaderText);
                }
                else
                    element.Content = SummaryCreator.GetSummaryDisplayText(groupRecord.SummaryDetails, column.MappingName, this.DataGrid.View);
            }
        }

        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var summaryCell = cell as GridCaptionSummaryCell;
            Style newStyle = null;
            if (summaryCell != null && DataGrid != null)
            {
                bool hasCaptionSummaryCellStyleSelector = DataGrid.hasCaptionSummaryCellStyleSelector;
                bool hasCaptionSummaryCellStyle = DataGrid.hasCaptionSummaryCellStyle;

                if (!hasCaptionSummaryCellStyleSelector && !hasCaptionSummaryCellStyle)
                    return;
#if !SILVERLIGHT && !WP
                if (hasCaptionSummaryCellStyleSelector && hasCaptionSummaryCellStyle)
                {
                    newStyle = DataGrid.CaptionSummaryCellStyleSelector.SelectStyle(record, cell);
                    newStyle = newStyle ?? DataGrid.CaptionSummaryCellStyle;
                }
                else if (hasCaptionSummaryCellStyleSelector)
                {
                    newStyle = DataGrid.CaptionSummaryCellStyleSelector.SelectStyle(record, cell);
                }                
#endif
                else if (hasCaptionSummaryCellStyle)
                {
                    newStyle = DataGrid.CaptionSummaryCellStyle;
                }
            }
            summaryCell.Style = newStyle;
        }

        private GridColumn GetGroupedColumn(Group group)
        {
            var groupDesc = this.DataGrid.View.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            //return this.dataGrid.Columns.FirstOrDefault(col => col.MappingName == groupDesc.PropertyName);
            foreach (var column in this.DataGrid.Columns)
            {
                if (column.MappingName == groupDesc.PropertyName)
                {
                    return column;
                }
            }
            return null;
        }
    }
}
