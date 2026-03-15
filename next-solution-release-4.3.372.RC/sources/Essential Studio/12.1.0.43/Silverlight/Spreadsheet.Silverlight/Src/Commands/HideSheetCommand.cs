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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class HideCurrentSheetCommand:CommandBase
    {
        public HideCurrentSheetCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
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
            if (parameter != null && parameter is bool)
            {
                if ((bool)parameter)
                {
                    AssociatedSpreadsheet.HideSheet(AssociatedSpreadsheet.GridProperties.CurrentSheetName);
                }
                else
                {
                    UnhideWindow window=new UnhideWindow
                                            {
                                                AssociatedSpreadsheet=AssociatedSpreadsheet
                                            };
                    window.ShowDialog();
                }
                base.ExecuteCommand(parameter);
            }
        }
    }

    public class HideSheetCommand : CommandBase
    {
        public HideSheetCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
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
            if (parameter != null && parameter is string)
            {
                AssociatedSpreadsheet.HideSheet(parameter.ToString());
                base.ExecuteCommand(parameter);
            }
        }
    }

    public class UnHideSheetCommand : CommandBase
    {
        public UnHideSheetCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (!this.AssociatedSpreadsheet.ExcelProperties.IsWorkBookProtected)
            {
                var workbook = AssociatedSpreadsheet.ExcelProperties.WorkBook;
                foreach (IWorksheet sheet in workbook.Worksheets)
                {
                    if (sheet.Visibility == WorksheetVisibility.Hidden)
                        return true;
                }
                return false;
            }
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            UnhideWindow window = new UnhideWindow
            {
                AssociatedSpreadsheet = AssociatedSpreadsheet
            };
            window.ShowDialog();
            this.ExecuteChanged();
        }
    }
}
