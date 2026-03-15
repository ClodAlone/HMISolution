#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class DataValidationCommand:CommandBase
    {
        public DataValidationCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing
                && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {

            DataValidationWindow window = new DataValidationWindow
                                              {
                                                  AssociatedSpreadsheet = AssociatedSpreadsheet,
                                                  Title = SpreadsheetResourceWrapper.DataValidation
                                              };
            window.ShowDialog();

            base.ExecuteCommand(parameter);
        }
    }
}
