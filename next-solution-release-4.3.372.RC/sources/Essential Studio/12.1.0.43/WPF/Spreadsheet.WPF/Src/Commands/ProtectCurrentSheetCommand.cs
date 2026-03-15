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
    public class ProtectCurrentSheetCommand:CommandBase
    {
        public ProtectCurrentSheetCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            IWorksheet worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (!worksheet.IsPasswordProtected)
            {
                ProtectCommandWindow window=new ProtectCommandWindow
                                                {
                                                    AssociatedSpreadsheet=AssociatedSpreadsheet,
                                                    Description = "Protect worksheet and contents of locked cells",
                                                    DescriptionText = "Password to unprotect sheet:",
                                                    Title = "Protect Sheet"
                                                };
                window.ShowDialog();
            }
            else
            {
                MessageBox.Show("Current sheet already protected by password");
            }
            base.ExecuteCommand(parameter);
        }
    }
}
