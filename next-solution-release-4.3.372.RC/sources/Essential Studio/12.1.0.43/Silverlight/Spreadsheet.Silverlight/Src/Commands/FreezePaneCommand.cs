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

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class FreezePaneCommand:CommandBase
    {
        public FreezePaneCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && parameter is Freeze)
            {
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                var worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[
                        AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                var currentcell = AssociatedSpreadsheet.GridProperties.CurrentCell;
                var isEditing = currentcell != null ? currentcell.IsEditing : false;
                if(currentcell != null && currentcell.HasCurrentCell)
                {
                    var row = currentcell.RowIndex;
                    var col = currentcell.ColumnIndex;
                    var frozenRow = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows;
                    var frozenColumn = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns;
                    switch ((Freeze)parameter)
                    {
                        case Freeze.FreezePanes:
                            if (frozenRow <= 1 && frozenColumn <= 1)
                            {
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows = row;
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns = col;
                                worksheet.HorizontalSplit = row;
                                worksheet.VerticalSplit = col;
                            }
                            else
                            {
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows = 1;
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns = 1;
                                worksheet.HorizontalSplit = 0;
                                worksheet.VerticalSplit = 0;
                            }
                            break;
                        case Freeze.FreezeTopRow:
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows = 2;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns = 1;
                            worksheet.HorizontalSplit = 1;
                            worksheet.VerticalSplit = 0;
                            break;
                        case Freeze.FreezeFirstColumn:
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows = 1;
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns = 2;
                            worksheet.HorizontalSplit = 0;
                            worksheet.VerticalSplit = 1;
                            break;
                    }
                }
                if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenColumns > 1 || this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.FrozenRows > 1)
                    this.AssociatedSpreadsheet.GridProperties.IsFrozen = true;
                else
                    this.AssociatedSpreadsheet.GridProperties.IsFrozen = false;
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                base.ExecuteCommand(parameter);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
            }
        }
    }
}
