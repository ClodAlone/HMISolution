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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class GroupRowExpandCommand:ICommand
    {
        SpreadsheetGridModel spreadsheetGridModel;
        GridRangeInfo GridRange;
        bool IsExpanded;
        ToggleButton Sender;
       
        public GroupRowExpandCommand(ToggleButton button,SpreadsheetGridModel model)
            :base()
        {
            this.spreadsheetGridModel = model;
            this.GridRange = (button as SpreadsheetGroupButton).Range;
            this.IsExpanded = !(button as SpreadsheetGroupButton).IsCollapsed;
            this.Sender = button;
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
            string ExcelRange = this.GridRange.ConvertGridRangeToExcelRange(this.spreadsheetGridModel);
            if (spreadsheetGridModel.CommandStack.ShouldGenerateUndoInfo)
            {
                this.spreadsheetGridModel.CommandStack.Push(new SpreadsheetExpandCollapseCommand(this.spreadsheetGridModel,
                    this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange], ExcelGroupBy.ByRows, !IsExpanded));

                this.spreadsheetGridModel.CommandStack.SuspendUndo = true;
            }
            this.spreadsheetGridModel.IsInGroup = true;

            if (IsExpanded)
            {
               this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange].CollapseGroup(ExcelGroupBy.ByRows);
               this.spreadsheetGridModel.RowHeights.SetHidden(GridRange.Top, GridRange.Bottom, true);
            }
            else
            {
                this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange].ExpandGroup(ExcelGroupBy.ByRows);

                for (int i = GridRange.Top; i <= GridRange.Bottom; i++)
                {
                    if(this.spreadsheetGridModel.ExcelProperties.WorkBook.ActiveSheet.Range[i,1,i,1].RowHeight>0)
                        this.spreadsheetGridModel.RowHeights.SetHidden(i,i, false);
                }                    
            }

            this.spreadsheetGridModel.IsInGroup = false;
            this.spreadsheetGridModel.CommandStack.SuspendUndo = false;
        }
    }

    public class GroupColumnExpandCommand : ICommand
    {
        SpreadsheetGridModel GridModel;
        GridRangeInfo GridRange;
        bool IsExpanded;
        ToggleButton Sender;

        public GroupColumnExpandCommand(ToggleButton button, SpreadsheetGridModel model)
            : base()
        {
            this.GridModel = model;
            this.GridRange = (button as SpreadsheetGroupButton).Range;
            this.IsExpanded = !(button as SpreadsheetGroupButton).IsCollapsed;
            this.Sender = button;
        }

        public bool CanExecute(object parameter)
        {
            return true;
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
            string ExcelRange = this.GridRange.ConvertGridRangeToExcelRange(this.GridModel);
            if (this.GridModel.CommandStack.ShouldGenerateUndoInfo)
            {
                this.GridModel.CommandStack.Push(new SpreadsheetExpandCollapseCommand(this.GridModel,
                    this.GridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange], ExcelGroupBy.ByColumns, !IsExpanded));

                this.GridModel.CommandStack.SuspendUndo = true;
            }
            this.GridModel.IsInGroup = true;            

            if (IsExpanded)
            {
                this.GridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange].CollapseGroup(ExcelGroupBy.ByColumns);
                this.GridModel.ColumnWidths.SetHidden(GridRange.Left, GridRange.Right, true);
            }
            else
            {
                this.GridModel.ExcelProperties.WorkBook.ActiveSheet[ExcelRange].ExpandGroup(ExcelGroupBy.ByColumns);

                for (int i = GridRange.Left; i <= GridRange.Right; i++)
                {
                    if (this.GridModel.ExcelProperties.WorkBook.ActiveSheet.Range[1, i, 1, i].ColumnWidth > 0)
                        this.GridModel.ColumnWidths.SetHidden(i, i, false);
                }                
            }

            this.GridModel.IsInGroup = false;
        }
    }
}
