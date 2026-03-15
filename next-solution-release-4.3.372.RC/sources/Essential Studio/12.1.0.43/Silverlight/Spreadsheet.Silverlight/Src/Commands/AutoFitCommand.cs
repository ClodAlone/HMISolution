#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class AutoFitCommand:CommandBase
    {
        public AutoFitCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && parameter is CellFormat)
            {
                var worksheet =
                        AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[
                            AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                var currentcell = AssociatedSpreadsheet.GridProperties.CurrentCell;
                if ((CellFormat)parameter == CellFormat.RowHeight)
                {
                    if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                    {
                        var isEditing = currentcell != null ? currentcell.IsEditing : false;
                        if (isEditing)
                            AssociatedSpreadsheet.GridProperties.CurrentCell.Deactivate();
                        int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                        for (int i = 0; i < upper; i++)
                        {
                            var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                            var count = item.Height;
                            if (count < 1) return;
                            string range = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                            worksheet[range].AutofitRows();
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ResizeRowsToFit(item, GridResizeToFitOptions.None);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(item.Top, item.Bottom));
                        }
                    }
                    else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                    {
                        var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                        var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                        string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                        worksheet[cell].AutofitRows();
                        bool shouldresize = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.checkWrapRow(row, col);
                        if (shouldresize)
                        {
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ResizeRowsToFit(GridRangeInfo.Row(row), GridResizeToFitOptions.None);
                        }
                       

                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                    }
                }
                else
                {
                    if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                    {
                        var isEditing = currentcell != null ? currentcell.IsEditing : false;
                        if (isEditing)
                            AssociatedSpreadsheet.GridProperties.CurrentCell.Deactivate();
                        int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                        for (int i = 0; i < upper; i++)
                        {
                            var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                            var count = item.Height;
                            if (count < 1) return;
                            string range = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                            worksheet[range].AutofitColumns();
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ResizeColumnsToFit(item, GridResizeToFitOptions.None);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(item.Top, item.Bottom));
                        }
                    }
                    else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                    {
                        var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                        var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                        string cell = GridRangeInfo.GetAlphaLabel(col) + row;
                        worksheet[cell].AutofitColumns();
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ResizeColumnsToFit(GridRangeInfo.Row(row), GridResizeToFitOptions.None);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                    }
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual();
                base.ExecuteCommand(parameter);
            }
        }
    }
}
