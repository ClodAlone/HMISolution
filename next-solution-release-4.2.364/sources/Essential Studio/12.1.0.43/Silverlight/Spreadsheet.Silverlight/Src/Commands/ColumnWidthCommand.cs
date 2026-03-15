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
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class ColumnWidthCommand:CommandBase
    {
        public ColumnWidthCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing
                && !AssociatedSpreadsheet.GridProperties.IsDisableMode)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            double width = 0;
            IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            {
                int col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                if (col > 0)
                    width = worksheet.GetColumnWidthInPixels(col);
            }
            CellFormatWindow window = new CellFormatWindow
                                          {
                                              AssociatedSpreadsheet=AssociatedSpreadsheet,
                                              CellFormat = CellFormat.ColumnWidth,
                                              Value = width
                                          };
            window.ShowDialog();

            base.ExecuteCommand(parameter);
        }
    }
}
