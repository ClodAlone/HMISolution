#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;

#if !SILVERLIGHT && !WP7
#endif
#if WinRT
using Syncfusion.UI.Xaml.ScrollAxis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#else
using System.Windows;
using Syncfusion.UI.Xaml.ScrollAxis;
using System.Windows.Controls;

#endif


namespace Syncfusion.UI.Xaml.Grid.Cells
{
    public class GridTableSummaryCellRenderer :GridVirtualizingCellRenderer<TextBlock, GridTableSummaryCell>
    {
        public GridTableSummaryCellRenderer()
        {
            this.UseOnlyRendererElement = true;
            this.SupportsRenderOptimization = false;
            this.IsFocusible = false;
            this.IsEditable = false;
        }

        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, GridTableSummaryCell uiElement, GridColumn column, object dataContext)
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

        public override void OnUpdateEditBinding(RowColumnIndex cellRowcolumnIndex, GridTableSummaryCell element, GridColumn column, object dataContext)
        {
            if (dataContext is SummaryRecordEntry)
            {
                var record = dataContext as SummaryRecordEntry;
                if (record.SummaryRow.ShowSummaryInRow)
                    element.Content = SummaryCreator.GetSummaryDisplayTextForRow(record, this.DataGrid.View);
                else
                    element.Content = SummaryCreator.GetSummaryDisplayText(record, column.MappingName, this.DataGrid.View);
            }
        }
#if !SILVERLIGHT && !WP
        protected override void InitializeCellStyle(RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var summaryCell = cell as GridTableSummaryCell;
            Style newStyle = null;
            if (summaryCell != null && DataGrid != null)
            {
                bool hasTableSummaryCellStyleSelector = DataGrid.hasTableSummaryCellStyleSelector;
                bool hasTableSummaryCellStyle = DataGrid.hasTableSummaryCellStyle;

                if (!hasTableSummaryCellStyleSelector && !hasTableSummaryCellStyle)
                    return;

                if (hasTableSummaryCellStyleSelector && hasTableSummaryCellStyle)
                {
                    newStyle = DataGrid.TableSummaryCellStyleSelector.SelectStyle(record, cell);
                    newStyle = newStyle ?? DataGrid.TableSummaryCellStyle;
                }
                else if (hasTableSummaryCellStyleSelector)
                {
                    newStyle = DataGrid.TableSummaryCellStyleSelector.SelectStyle(record, cell);
                }
                else if (hasTableSummaryCellStyle)
                {
                    newStyle = DataGrid.TableSummaryCellStyle;
                }
                summaryCell.Style = newStyle;
            }
        }
#else
        protected override void InitializeCellStyle(Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex cellRowColumnIndex, object record, UIElement cell, GridColumn column)
        {
            var summaryCell = cell as GridTableSummaryCell;
            Style newStyle = null;
            if (summaryCell != null && DataGrid != null)
            {
                bool hasTableSummaryCellStyle = DataGrid.hasTableSummaryCellStyle;

                if (!hasTableSummaryCellStyle)
                    return;

                if (hasTableSummaryCellStyle)
                {
                    newStyle = DataGrid.TableSummaryCellStyle;
                }
                summaryCell.Style = newStyle;
            }
        }
#endif
    }
}
