#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Input;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class ConditionalFormatCommand:CommandBase
    {
        public ConditionalFormatCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
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
            if (parameter != null && parameter is ExcelComparisonOperator)
            {
                if((ExcelComparisonOperator) parameter == ExcelComparisonOperator.None) return;
                ConditionalFormatWindow window = new ConditionalFormatWindow
                                                     {
                                                         AssociatedSpreadsheet = AssociatedSpreadsheet,
                                                         Operator = (ExcelComparisonOperator) parameter
                                                     };
                window.ShowDialog();
                base.ExecuteCommand(parameter);
            }
        }
        
    }
}
