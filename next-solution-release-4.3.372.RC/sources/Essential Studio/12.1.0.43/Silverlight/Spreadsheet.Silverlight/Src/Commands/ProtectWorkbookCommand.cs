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
    public class ProtectWorkbookCommand:CommandBase
    {
        public ProtectWorkbookCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
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
            IWorkbook workbook = AssociatedSpreadsheet.ExcelProperties.WorkBook;
            if (!AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected)
            {
                ProtectWorkbookWindow window = new ProtectWorkbookWindow
                {
                    AssociatedSpreadsheet = AssociatedSpreadsheet,
                    Description = SpreadsheetResourceWrapper.ProtectWorkbookDescription,
                    DescriptionText = SpreadsheetResourceWrapper.ProtectWorkbookDescriptionText,
                    Title = SpreadsheetResourceWrapper.ProtectWorkbookTitle
                };
                window.ShowDialog();
            }
            else
            {
                UnprotectWorkbookWindow UnprotectWindow = new UnprotectWorkbookWindow
                {
                    AssociatedSpreadsheet = AssociatedSpreadsheet,
                    Description = SpreadsheetResourceWrapper.ProtectWorkbookDescriptionText,
                    Title = SpreadsheetResourceWrapper.UnprotectWorkbookTitle
                };
                UnprotectWindow.Show();
            }
            base.ExecuteCommand(parameter);
        }
    }
}
