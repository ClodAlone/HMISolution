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
    public class HideRowCommand:CommandBase
    {
        public HideRowCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if(parameter != null && parameter is bool)
            {
                var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                var currentcell = AssociatedSpreadsheet.GridProperties.CurrentCell;
                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    var isEditing = currentcell != null ? currentcell.IsEditing : false;
                    if (isEditing)
                        AssociatedSpreadsheet.GridProperties.CurrentCell.Deactivate();
                    int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;

                    for (int i = 0; i < upper; i++)
                    {
                        var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                        item = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.ExpandSelectedCellsRange(item);
                        if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                        {
                            if (!(bool)parameter && AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                            {
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("Hide");
                                for (int row = item.Top; row <= item.Bottom; row++)
                                {
                                    if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.HiddenRowRanges.Contains(GridRangeInfo.Row(row)))
                                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetSetRowHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, row, row, true));
                                }
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                            }
                            else if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                            {
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetSetRowHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, item.Top, item.Bottom, false));
                            }
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(item.Top, item.Bottom, (bool)parameter);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(item.Top, item.Bottom));
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;

                    if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {                        
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetSetRowHideCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, row, row, false));                                                    
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = true;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights.SetHidden(row, row, (bool) parameter);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.SuspendUndo = false;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
            }
            base.ExecuteCommand(parameter);
        }
    }
}
