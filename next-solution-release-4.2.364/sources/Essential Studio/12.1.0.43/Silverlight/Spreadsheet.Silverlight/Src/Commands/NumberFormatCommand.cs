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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class NumberFormatCommand:CommandBase
    {
        public NumberFormatCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && parameter is string)
            {
                var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                GridStyleInfo[] cellsInfo = null;
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                    {
                        string range = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        worksheet.Range[range].NumberFormat = (string) parameter;
                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                    var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                    string range=GridRangeInfo.GetAlphaLabel(col) + row;
                    worksheet.Range[range].NumberFormat = (string) parameter;
                    GridRangeInfo rangeInfo = AssociatedSpreadsheet.GridProperties.CurrentCell.RangeInfo;
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo, Styles.StyleModifyType.Copy);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            }
            base.ExecuteCommand(parameter);
        }
        
    }
}
