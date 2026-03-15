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
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class DeleteCurrentSheetCommand:CommandBase
    {
        public DeleteCurrentSheetCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null &&!this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            string sheetName = AssociatedSpreadsheet.GridProperties.CurrentSheetName;
            IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[sheetName];
            if (sheet.UsedCells.Length <= 0)
                AssociatedSpreadsheet.RemoveSheet(sheetName);
            else
            {
                var result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_DeleteNonEmptySheet,SpreadsheetResourceWrapper.Warning, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                    AssociatedSpreadsheet.RemoveSheet(sheetName);
            }
            base.ExecuteCommand(parameter);
        }
    }


    public class DeleteSheetCommand : CommandBase
    {
        public DeleteSheetCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null)
            {
                string sheetName = parameter.ToString();
                IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[sheetName];
                if (sheet.UsedCells.Length <= 0)
                    AssociatedSpreadsheet.RemoveSheet(sheetName);
                else
                {
                    var result = MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_DeleteNonEmptySheet, SpreadsheetResourceWrapper.Warning, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                        AssociatedSpreadsheet.RemoveSheet(sheetName);
                }
            }
            base.ExecuteCommand(parameter);
        }
    }
}
