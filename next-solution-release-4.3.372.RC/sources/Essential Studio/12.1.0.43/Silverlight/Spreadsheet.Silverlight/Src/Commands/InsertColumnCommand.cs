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
    public class InsertColumnCommand:CommandBase
    {
        public InsertColumnCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            var currentcell = AssociatedSpreadsheet.GridProperties.CurrentCell;
            if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
            {
                int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                for (int i = 0; i < upper; i++)
                {
                    var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                    var count = item.Width;
                    if (count < 1) return;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = true;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InsertColumns(item.Left, count);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = false;
                    worksheet.InsertColumn(item.Left, count, ExcelInsertOptions.FormatDefault);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cols(item.Left, item.Right));
                }
            }
            else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            {
                var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = true;
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InsertColumns(col, 1);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = false;
                worksheet.InsertColumn(col);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(col));
            }
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(0));
            base.ExecuteCommand(parameter);
        }
    }
}
