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
    public class InsertSheetCommand:CommandBase
    {
        public InsertSheetCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (!this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected)
                return base.CanExcuteCommand(parameter);
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            IWorkbook book = AssociatedSpreadsheet.ExcelProperties.WorkBook;
            int count = book.Worksheets.Count;
            count++;
            string sheetName = "Sheet" + count;
            foreach (IWorksheet sheet in book.Worksheets)
            {
                if (sheet.Name == sheetName)
                    count++;
            }
            sheetName = "Sheet" + count;
            AssociatedSpreadsheet.AddSheet(sheetName);
            base.ExecuteCommand(parameter);
        }
    }
}
