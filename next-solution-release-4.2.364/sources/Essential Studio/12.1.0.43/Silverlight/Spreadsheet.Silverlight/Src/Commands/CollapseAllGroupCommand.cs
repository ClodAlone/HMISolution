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
using Syncfusion.XlsIO.Parser.Biff_Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    class CollapseAllGroupCommand:ICommand
    {
        SpreadsheetGridModel spreadsheetGridModel;
        IWorksheet workSheet;
        ExcelGroupBy groupBy;
        int labelNo;
        IRange excelRange;
        GridRangeInfo gridRange;

        public CollapseAllGroupCommand(int labelNo, ExcelGroupBy groupBy, SpreadsheetGridModel gridModel, IWorksheet workSheet)
            : base()
        {
            this.spreadsheetGridModel = gridModel;
            this.labelNo = labelNo;
            this.groupBy = groupBy;
            this.workSheet = workSheet;
        }

        public bool CanExecute(object parameter)
        {
            if (this.spreadsheetGridModel != null && !this.spreadsheetGridModel.ActiveGridView.CurrentCell.IsEditing &&
                !this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.IsPasswordProtected)
                return true;
            else
                return false;
        }

        public event EventHandler CanExecuteChanged;

        public void ExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            List<IOutlineWrapper> outlineWrappers = (this.workSheet as WorksheetImpl).OutlineWrappers;

            if (this.spreadsheetGridModel.CommandStack.Enabled)
            {
                AddToCommandStack();
            }

            this.spreadsheetGridModel.IsInGroup = true;
            this.spreadsheetGridModel.CommandStack.SuspendUndo = true;
            foreach (OutlineWrapper outlineWrapper in outlineWrappers)
            {
                if (outlineWrapper.GroupBy == this.groupBy && outlineWrapper.Outline.OutlineLevel <= this.labelNo)
                {
                    excelRange = outlineWrapper.OutlineRange;
                    gridRange = excelRange.ConvertExcelRangeToGridRange();

                    if (this.groupBy == ExcelGroupBy.ByRows)
                    {
                        if (this.labelNo == outlineWrapper.Outline.OutlineLevel && !outlineWrapper.Outline.IsHidden)
                        {
                            this.excelRange.CollapseGroup(ExcelGroupBy.ByRows);
                            this.spreadsheetGridModel.RowHeights.SetHidden(gridRange.Top, gridRange.Bottom, true);
                        }
                        else if (outlineWrapper.Outline.OutlineLevel < this.labelNo && outlineWrapper.Outline.IsHidden)
                        {
                            this.excelRange.ExpandGroup(ExcelGroupBy.ByRows);

                            for (int i = gridRange.Top; i <= gridRange.Bottom; i++)
                            {
                                if (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.Range[i, 1, i, 1].RowHeight > 0)
                                    this.spreadsheetGridModel.RowHeights.SetHidden(i, i, false);
                            }   
                        }
                    }
                    else
                    {
                        if(this.labelNo==outlineWrapper.Outline.OutlineLevel && !outlineWrapper.Outline.IsHidden)
                        {
                            this.excelRange.CollapseGroup(ExcelGroupBy.ByColumns);
                            this.spreadsheetGridModel.ColumnWidths.SetHidden(gridRange.Left, gridRange.Right, true);
                        }
                        else if (outlineWrapper.Outline.OutlineLevel < this.labelNo && outlineWrapper.Outline.IsHidden)
                        {
                            this.excelRange.ExpandGroup(ExcelGroupBy.ByColumns);

                            for (int i = gridRange.Left; i <= gridRange.Right; i++)
                            {
                                if (this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.Range[1, i, 1, i].ColumnWidth > 0)
                                    this.spreadsheetGridModel.ColumnWidths.SetHidden(i, i, false);
                            }  
                        }
                    }
                }
            }

            this.spreadsheetGridModel.IsInGroup = false;
            this.spreadsheetGridModel.CommandStack.SuspendUndo = false;            
        }


        internal void AddToCommandStack()
        {
            this.spreadsheetGridModel.CommandStack.BeginTrans("CollapseGroups");
            List<IOutlineWrapper> outlineWrappers = (this.workSheet as WorksheetImpl).OutlineWrappers;

            foreach (OutlineWrapper outlineWrapper in outlineWrappers)
            {
                if (outlineWrapper.Outline.OutlineLevel == this.labelNo && !outlineWrapper.Outline.IsHidden && this.spreadsheetGridModel.CommandStack.ShouldGenerateUndoInfo)
                {
                    this.spreadsheetGridModel.CommandStack.Push
                        (new SpreadsheetExpandCollapseCommand(this.spreadsheetGridModel, outlineWrapper.OutlineRange, this.groupBy, false));
                }
                else if (outlineWrapper.Outline.OutlineLevel < this.labelNo && outlineWrapper.Outline.IsHidden && this.spreadsheetGridModel.CommandStack.ShouldGenerateUndoInfo)
                {
                    this.spreadsheetGridModel.CommandStack.Push(new SpreadsheetExpandCollapseCommand(this.spreadsheetGridModel,outlineWrapper.OutlineRange,this.groupBy,true));
                }
            }
            this.spreadsheetGridModel.CommandStack.CommitTrans();
        }
    }
}
