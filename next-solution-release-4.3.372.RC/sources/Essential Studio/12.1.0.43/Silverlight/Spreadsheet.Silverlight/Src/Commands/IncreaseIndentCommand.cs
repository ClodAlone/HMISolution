#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using System.Windows;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class IncreaseIndentCommand:CommandBase
    {
        public IncreaseIndentCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            try
            {
                if (parameter != null && parameter is bool)
                {
                    var worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                    GridStyleInfo[] cellsInfo = null;
                    if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                    {
                        foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                        {
                            string cell = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                            IRange range = worksheet.Range[cell];
                            if ((bool)parameter)
                                range.CellStyle.IndentLevel += 1;
                            else
                            {
                                if (range.CellStyle.IndentLevel > 0)
                                    range.CellStyle.IndentLevel -= 1;
                            }
                            cellsInfo = this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                            this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                        }
                    }
                    else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                    {
                        var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                        var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;

                        string cell = GridRangeInfo.GetAlphaLabel(col) + row;


                        if ((bool)parameter)
                        {
                            worksheet.Range[cell].CellStyle.IndentLevel += 1;
                        }
                        else
                        {
                            if (worksheet.Range[cell].IndentLevel != 0)
                                worksheet.Range[cell].CellStyle.IndentLevel -= 1;
                        }
                        GridRangeInfo range = AssociatedSpreadsheet.GridProperties.CurrentCell.RangeInfo;
                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(range);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(range, cellsInfo, Styles.StyleModifyType.Copy);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);

                    base.ExecuteCommand(parameter);
                }
            }
            catch (System.Exception)
            {
                //In excel 2007 format can only increase the intent level up to 15
            }
        }
    }
}
