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
    public class ProtectCurrentSheetCommand:CommandBase
    {
        public ProtectCurrentSheetCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null &&
                !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            IWorksheet worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            if (worksheet != null)
            {
                if (!worksheet.IsPasswordProtected)
                {
                    ProtectCommandWindow window = new ProtectCommandWindow
                                                    {
                                                        AssociatedSpreadsheet = AssociatedSpreadsheet,
                                                        Description = SpreadsheetResourceWrapper.ProtectSheetDescription,
                                                        DescriptionText = SpreadsheetResourceWrapper.ProtectSheetDescriptionText,
                                                        Title = SpreadsheetResourceWrapper.ProtectSheetTitle,
                                                    };
                    window.ShowDialog();
                }
                else
                {
                    ProtectCommandWindow window = new ProtectCommandWindow
                    {
                        AssociatedSpreadsheet = AssociatedSpreadsheet,
                        Description = SpreadsheetResourceWrapper.UnProtectSheetDescription,
                        DescriptionText = SpreadsheetResourceWrapper.UnProtectSheetDescriptionText,
                        Title = SpreadsheetResourceWrapper.UnProtectSheetTitle
                    };
                    window.ShowDialog();
                }
            }
            base.ExecuteCommand(parameter);
        }
    }

    public class ProtectSheetCommand : CommandBase
    {
        public ProtectSheetCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null)
            {
                IWorksheet worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[parameter.ToString()];
                if (worksheet != null)
                {
                    if (!worksheet.IsPasswordProtected)
                    {
                        ProtectCommandWindow window = new ProtectCommandWindow
                        {
                            AssociatedSpreadsheet = AssociatedSpreadsheet,
                            Description = SpreadsheetResourceWrapper.ProtectSheetDescription,
                            DescriptionText = SpreadsheetResourceWrapper.ProtectSheetDescriptionText,
                            Title = SpreadsheetResourceWrapper.ProtectSheetTitle,
                            SheetName = parameter.ToString()
                        };
                        window.ShowDialog();
                    }
                    else
                    {
                        ProtectCommandWindow window = new ProtectCommandWindow
                        {
                            AssociatedSpreadsheet = AssociatedSpreadsheet,
                            Description = SpreadsheetResourceWrapper.UnProtectSheetDescription,
                            DescriptionText = SpreadsheetResourceWrapper.UnProtectSheetDescriptionText,
                            Title = SpreadsheetResourceWrapper.UnProtectSheetTitle,
                            SheetName = parameter.ToString()
                        };
                        window.ShowDialog();
                    }
                }
            }
            base.ExecuteCommand(parameter);
        }
    }
}
