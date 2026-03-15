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
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class InsertCommentCommand : CommandBase
    {
        public InsertCommentCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && 
                !this.AssociatedSpreadsheet.GridProperties.IsDisableMode && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet.GridProperties.CurrentCell != null && this.AssociatedSpreadsheet.ExcelProperties.WorkBook != null && this.AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets.Count > 0)
            {
                var excelStyle = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName][AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex, AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex];
                InsertCommentWindow window = new InsertCommentWindow
                {
                    AssociatedSpreadsheet = AssociatedSpreadsheet,
                    Comment = excelStyle.Comment != null ? excelStyle.Comment.Text : string.Empty,
                    Title = excelStyle.Comment != null ? SpreadsheetResourceWrapper.EditComment : SpreadsheetResourceWrapper.NewComment
                };
                window.ShowDialog();
            }
            else
            {
#if !SILVERLIGHT
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_SelectCellToInsertComment, SpreadsheetResourceWrapper.ValidationMessage_Error);
#else
                MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_SelectCellToInsertComment, SpreadsheetResourceWrapper.ValidationMessage_Error, MessageBoxButton.OK);
#endif

            }
            base.ExecuteCommand(parameter);
        }
    }
}