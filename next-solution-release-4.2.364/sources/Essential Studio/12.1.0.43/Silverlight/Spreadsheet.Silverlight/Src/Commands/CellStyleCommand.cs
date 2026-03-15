#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;
using System;
using System.Windows;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using Syncfusion.Windows.Styles;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class CellStyleCommand:CommandBase
    {
        public CellStyleCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
                return true;
            else
                return false;
            //return base.CanExcuteCommand(parameter);
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && parameter is BuiltInStyles)
            {
                try
                {
                    IWorksheet worksheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                    GridStyleInfo[] cellsInfo = null;
                    if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                    {
                        foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                        {
                            string range = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                            worksheet.Range[range].BuiltInStyle = (BuiltInStyles)parameter;
                            if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                            {
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("CellStyle changed");
                                cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo,Styles.StyleModifyType.Copy);
                                SpreadsheetFontSizeChangedCommand fontCommand =new SpreadsheetFontSizeChangedCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, item, cellsInfo,StyleModifyType.Copy);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(fontCommand);
                                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                            }
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                        }
                    }
                    else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                    {
                        var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                        var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                        string range = GridRangeInfo.GetAlphaLabel(col) + row;
                        worksheet.Range[range].BuiltInStyle = (BuiltInStyles)parameter;
                        GridRangeInfo rangeInfo = AssociatedSpreadsheet.GridProperties.CurrentCell.RangeInfo;
                        if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                        {
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.BeginTrans("CellStyle changed");
                            cellsInfo =AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo,Styles.StyleModifyType.Copy);
                            SpreadsheetFontSizeChangedCommand fontCommand =new SpreadsheetFontSizeChangedCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, rangeInfo, cellsInfo,StyleModifyType.Copy);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(fontCommand);
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.CommitTrans();
                        }
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                    }
                    base.ExecuteCommand(parameter);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                }
                catch (Exception)
                {
                    MessageBox.Show(SpreadsheetResourceWrapper.ValidationMessage_BuildInStyleNotSupportedInExcelVersion, SpreadsheetResourceWrapper.MessageBoxCaption, MessageBoxButton.OK);
                    //throw;
                }
                
            }
        }
    }
}
