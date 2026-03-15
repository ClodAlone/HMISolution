#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using System.Windows;
namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class CopyCommand : CommandBase
    {
        public CopyCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {
            
        }

        public override bool CanExcuteCommand(object parameter)
        {
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            CopyPaste copyPasteOption;
            if (parameter != null)
                copyPasteOption = (CopyPaste)parameter;
            else
                copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;

            CommandExtensions.CommandExtensions.Copy(AssociatedSpreadsheet, false, copyPasteOption);
            base.ExecuteCommand(parameter);
        }
    }

    public class CutCommand : CommandBase
    {
        public CutCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {

        }

        public override bool CanExcuteCommand(object parameter)
        {
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            CopyPaste copyPasteOption;
            if (parameter != null)
                copyPasteOption = (CopyPaste)parameter;
            else
                copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;
            CommandExtensions.CommandExtensions.Copy(AssociatedSpreadsheet, true, copyPasteOption);
            base.ExecuteCommand(parameter);
        }
    }

    public class PasteCommand : CommandBase
    {
        public PasteCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        {

        }

        public override bool CanExcuteCommand(object parameter)
        {
            return Clipboard.ContainsText();
        }

        protected override void ExecuteCommand(object parameter)
        {
            CopyPaste copyPasteOption;
            if (parameter != null)
                copyPasteOption = (CopyPaste)parameter;
            else
                copyPasteOption = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption;

            if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel != null)
            {
                this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.Options.CopyPasteOption = copyPasteOption;
                (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CutPaste as GridModelCutPaste).Paste();
                if (this.AssociatedSpreadsheet.GridProperties.CurrentCell != null && this.AssociatedSpreadsheet.GridProperties.CurrentCell.IsEditing)
                    this.AssociatedSpreadsheet.GridProperties.CurrentCell.EndEdit();
            }
            base.ExecuteCommand(parameter);
        }
    }
}
