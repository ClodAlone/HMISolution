#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class UngroupCommand:CommandBase
    {
        public UngroupCommand(SpreadsheetControl excelEditorControl)
            : base(excelEditorControl)
        { 
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets.Count > 0 && this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count == 1)
            {
                GridRangeInfo gridRange = this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[0];
                string excelRange;

                if (gridRange.IsRows)
                {
                    excelRange = gridRange.ConvertGridRangeToExcelRange(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByRows);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand ungroupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByRows, true);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(ungroupCommand);
                    }
                    this.AssociatedSpreadsheet.OutlineRowCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).RowsOutlineLevel;                   
                }
                else if (gridRange.IsCols)
                {
                    excelRange = gridRange.ConvertGridRangeToExcelRange(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                    this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange].Ungroup(ExcelGroupBy.ByColumns);
                    if (this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        SpreadsheetGroupUngroupCommand ungroupCommand = new SpreadsheetGroupUngroupCommand(this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel,
                                this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet[excelRange], ExcelGroupBy.ByColumns, true);
                        this.AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(ungroupCommand);
                    }
                    this.AssociatedSpreadsheet.OutlineColumnCount = (this.AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet as WorksheetImpl).ColumnsOutlineLevel;                    
                }
                else
                {
                    GroupbyWindow window = new GroupbyWindow
                    {
                        AssociatedSpreadsheet = this.AssociatedSpreadsheet,
                        NeedToGroup=false,
                        Title = SpreadsheetResourceWrapper.Ungroup
                    };

                    window.ShowDialog();
                }
            }
        }
    }
}
