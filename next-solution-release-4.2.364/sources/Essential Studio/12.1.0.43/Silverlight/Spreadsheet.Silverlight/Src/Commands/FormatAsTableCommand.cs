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
using Syncfusion.Windows.Controls.Cells;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Windows.Controls.Spreadsheet.Resources;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class FormatAsTableCommand:CommandBase
    {
        private bool isRangeIntersect = false;
        private bool isRangeContainsCurrentCell = false;
        private List<int> intersectedTableIndex = new List<int>();

        public FormatAsTableCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel != null && AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count >= 2)
                return false;

            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing)
            {
                FindExistingTable();
                if (isRangeIntersect && !isRangeContainsCurrentCell)
                    return false;
                return true;   
            }
            else
                return false;
        }

        protected override void ExecuteCommand(object parameter)
        {
            if (parameter != null && parameter is TableBuiltInStyles && AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count == 1)
            {
                var range = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[0];
                if (range.RangeType != GridRangeInfoType.Cells) return;
                IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet;
                if (isRangeContainsCurrentCell)
                {
                    IListObject table;
                    foreach (int index in intersectedTableIndex)
                    {
                        table = sheet.ListObjects[index];
                        table.BuiltInTableStyle = (TableBuiltInStyles)parameter;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(sheet.ListObjects[index].Location.ConvertExcelRangeToGridRange());
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                    return;
                }
                string startingCell = GridRangeInfo.GetAlphaLabel(range.Left) + range.Top;
                string endingCell = GridRangeInfo.GetAlphaLabel(range.Right) + range.Bottom;
                string cells = startingCell + ":" + endingCell;
                FormatAsTableWindow window = new FormatAsTableWindow
                {
                    Description = SpreadsheetResourceWrapper.FormatAsTableDescription,
                    Title = SpreadsheetResourceWrapper.FormatAsTableWindowTitle,
                    Text = cells,
                    AssociatedSpreadsheet = AssociatedSpreadsheet,
                    TableStyle = (TableBuiltInStyles)parameter
                };
                window.ShowDialog();
                base.ExecuteCommand(parameter);
            }
        }

        private void FindExistingTable()
        {
            isRangeContainsCurrentCell = false;
            isRangeIntersect = false;
            intersectedTableIndex.Clear();
            if (AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets.Count <= 0 ||
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel == null)
                return;

            IWorksheet sheet = AssociatedSpreadsheet.ExcelProperties.WorkBook.ActiveSheet;
            var range = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.ActiveRange;
            for(int i =0; i< sheet.ListObjects.Count;i++)
            {
                IListObject table = sheet.ListObjects[i];
                GridRangeInfo tableRange = table.Location.ConvertExcelRangeToGridRange();
                if (range.IntersectsWith(tableRange))
                {
                    isRangeIntersect = true;
                    intersectedTableIndex.Add(i);
                    var currentRowCol = AssociatedSpreadsheet.GridProperties.ActiveSpreadsheetGrid.CurrentCell.CellRowColumnIndex;
                    if (tableRange.Contains(GridRangeInfo.Cell(currentRowCol.RowIndex, currentRowCol.ColumnIndex)))
                        isRangeContainsCurrentCell = true;
                }
            }
        }
    }
}
