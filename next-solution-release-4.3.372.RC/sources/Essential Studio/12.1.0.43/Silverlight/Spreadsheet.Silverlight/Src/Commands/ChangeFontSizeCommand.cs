#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Spreadsheet.CommandExtensions;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class FontSizeCommand:CommandBase
    {
        public FontSizeCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            //return AssociatedSpreadsheet.HasCurrentCell;
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null)
            {
                double size;
                double.TryParse(parameter.ToString(), out size);
                AssociatedSpreadsheet.ChangeFontSize(size);
            }
            base.ExecuteCommand(parameter);
        }
    }
}
