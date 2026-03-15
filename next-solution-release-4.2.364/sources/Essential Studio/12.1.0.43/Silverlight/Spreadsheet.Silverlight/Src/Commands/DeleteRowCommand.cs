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
using Syncfusion.XlsIO;
using System.Collections;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
   public class GridFormulaInfo
    {
        public GridFormulaInfo()
        {
        }

        private int rowIndex;
        public int RowIndex
        {
            get 
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        private int colIndex;
        public int ColIndex
        {
            get 
            {
                return colIndex;
            }
            set
            {
                colIndex = value;
            }
        }

        private GridStyleInfo styleInfo; 
        public GridStyleInfo StyleInfo
        {
            get
            {
                return styleInfo;
            }
            set
            {
                styleInfo = value;
            }
        }

    }

    public class DeleteRowCommand:CommandBase 
    {
        public DeleteRowCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
        {
        }

        public override bool CanExcuteCommand(object parameter)
        {
            if (this.AssociatedSpreadsheet != null && this.AssociatedSpreadsheet.GridProperties != null && !this.AssociatedSpreadsheet.GridProperties.IsEditing
                && !this.AssociatedSpreadsheet.GridProperties.IsDisableMode)
            {
                var model = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel;
                if (model != null && (model.SelectedRanges.ActiveRange.IsCols || model.SelectedRanges.ActiveRange.IsTable))
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
            
            if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
            {
                int upper = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges.Count;
                for (int i = 0; i < upper; i++)
                {
                    var item = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges[i];
                    var count = item.Height;

                    if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                    {
                        double[] rowHeights = new double[count];
                        int arrayIndex = 0;
                        for (int row = item.Top; row <= item.Bottom; row++)
                        {
                            rowHeights[arrayIndex] = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights[row];
                            arrayIndex++;
                        }

                        GridStyleInfo[] cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cells(item.Top, 1, item.Bottom, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnCount - 1));
                        GridFormulaEngine formulaEngine = new GridFormulaEngine(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        GridFormulaInfo[] formulaInfos = new GridFormulaInfo[1000];
                        int formulaIndex = 0;
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = true;   
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
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.IsInInsert = false;  
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetModelInsertRowsCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, item.Top,count, cellsInfo,formulaInfos,rowHeights));
                    }
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RemoveRows(item.Top, count);                    
                    worksheet.DeleteRow(item.Top, count);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Rows(item.Top, item.Bottom));
                }
            }
            else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
            {
                var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                GridStyleInfo[] cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(GridRangeInfo.Cells(row, 1, row, AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ColumnCount - 1));

                if (AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.ShouldGenerateUndoInfo)
                {
                    double[] rowHeights = new double[] { AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RowHeights[row] };
                    GridFormulaEngine formulaEngine = new GridFormulaEngine(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                    GridFormulaInfo[] formulaInfo = new GridFormulaInfo[1000];                
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
                                    formulaInfo[formulaIndex] = formula;
                                    formulaIndex++;                          
                                }                                            
                        }                
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.CommandStack.Push(new SpreadsheetModelInsertRowsCommand(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel, row, 1, cellsInfo,formulaInfo,rowHeights));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.RemoveRows(row, 1);
                worksheet.DeleteRow(row);
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(row));
            }
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Row(0));
            AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Col(0));
            base.ExecuteCommand(parameter);
        }
    }
}
