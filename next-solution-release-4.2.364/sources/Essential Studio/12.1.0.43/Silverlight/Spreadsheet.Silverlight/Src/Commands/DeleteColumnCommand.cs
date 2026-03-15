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
using System.Collections;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class DeleteColumnCommand:CommandBase
    {
        public DeleteColumnCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing
                 && !AssociatedSpreadsheet.GridProperties.IsDisableMode)
            {
                var model = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel;
                if (model != null && (model.SelectedRanges.ActiveRange.IsRows || model.SelectedRanges.ActiveRange.IsTable))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

      
        protected override void ExecuteCommand(object parameter)
        {
            var worksheet =
                AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[AssociatedSpreadsheet.GridProperties.CurrentSheetName];
            var currentcell = AssociatedSpreadsheet.GridProperties.CurrentCell;
            
            GridStyleInfo[] cellsInfo;
            if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
            {
                int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                for (int i = 0; i < upper; i++)
                {
                    var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                    var count = item.Width;                    
                    if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        double[] columnWidths = new double[count];
                        int arrayIndex = 0;
                        for (int column = item.Left; column <= item.Right; column++)
                        {
                            columnWidths[arrayIndex] = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths[column];
                            arrayIndex++;
                        }
                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cells(1, item.Left, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowCount - 1, item.Right));
                        GridFormulaEngine formulaEngine = new GridFormulaEngine(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        GridFormulaInfo[] formulaInfos = new GridFormulaInfo[1000];
                        int formulaIndex = 0;
                        foreach (string s in formulaEngine.DependentCells.Keys)
                           {
                                Hashtable ht = (Hashtable)formulaEngine.DependentCells[s];

                                foreach (object o in ht.Keys)
                                    {
                                        string str = o as string;
                                        int rowIndex = formulaEngine.RowIndex(str);
                                        int colIndex = formulaEngine.ColIndex(str);

                                        GridStyleInfo style = new GridStyleInfo();
                                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellInfo(rowIndex, colIndex, style);
                                        GridFormulaInfo formula = new GridFormulaInfo();
                                        formula.RowIndex = rowIndex;
                                        formula.ColIndex = colIndex;
                                        formula.StyleInfo = style;
                                        formulaInfos[formulaIndex] = formula;
                                        formulaIndex++;
                                    }
                            }
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetModelInsertColumnsCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, item.Left, count, cellsInfo, formulaInfos, columnWidths));
                        }

                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = true;
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RemoveColumns(item.Left, count);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = false;
                    worksheet.DeleteColumn(item.Left, count);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(item.Left, item.Right));
                }
            }
            else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            {
                var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                double[] columnWidths =new double[]{ AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnWidths[col]};

                if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                {
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cells(1, col, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowCount - 1, col));
                    GridFormulaEngine formulaEngine = new GridFormulaEngine(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                            GridFormulaInfo[] formulaInfos = new GridFormulaInfo[1000];
                            int formulaIndex = 0;
                            foreach (string s in formulaEngine.DependentCells.Keys)
                                {
                                    Hashtable ht = (Hashtable)formulaEngine.DependentCells[s];

                                    foreach (object o in ht.Keys)
                                        {
                                            string str = o as string;
                                            int rowIndex = formulaEngine.RowIndex(str);
                                            int colIndex = formulaEngine.ColIndex(str);

                                            GridStyleInfo style = new GridStyleInfo();
                                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellInfo(rowIndex, colIndex, style);
                                            GridFormulaInfo formula = new GridFormulaInfo();
                                            formula.RowIndex = rowIndex;
                                            formula.ColIndex = colIndex;
                                            formula.StyleInfo = style;
                                            formulaInfos[formulaIndex] = formula;
                                            formulaIndex++;
                                        }
                                }
                            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetModelInsertColumnsCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, col, 1, cellsInfo, formulaInfos, columnWidths));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = true;
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RemoveColumns(col, 1);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = false;
                worksheet.DeleteColumn(col);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(col));
            }
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(0));
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(0));
            base.ExecuteCommand(parameter);
        }
    }
}
