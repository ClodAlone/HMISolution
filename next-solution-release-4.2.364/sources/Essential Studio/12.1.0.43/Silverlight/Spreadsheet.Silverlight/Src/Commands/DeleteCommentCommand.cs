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
    public class DeleteCommentCommand:CommandBase
    {
        public DeleteCommentCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
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
            GridStyleInfo[] cellsInfo;
            IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
            {
                foreach (GridRangeInfo range in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                {
                    string cells = range.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                    worksheet[cells].Comment.Remove();
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(range);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(range, cellsInfo, Styles.StyleModifyType.Copy);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(range);
                }
            }
            else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            {
                var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                string cells = GridRangeInfo.GetAlphaLabel(col) + row;
                worksheet[cells].Comment.Remove();
                cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cell(row, col));
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(GridRangeInfo.Cell(row, col), cellsInfo, Styles.StyleModifyType.Copy);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row,col));
            }
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            this.AssociatedSpreadsheet.GridProperties.RefreshCurrentStyle();
            base.ExecuteCommand(parameter);
        }
    }
}
