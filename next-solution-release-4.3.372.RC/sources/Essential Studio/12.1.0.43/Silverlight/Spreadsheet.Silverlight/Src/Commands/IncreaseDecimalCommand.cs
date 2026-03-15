#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Text.RegularExpressions;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.XlsIO;

namespace Syncfusion.Windows.Controls.Spreadsheet.Commands
{
    public class IncreaseDecimalCommand:CommandBase
    {
        public IncreaseDecimalCommand(SpreadsheetControl excelEditorControl) : base(excelEditorControl)
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
            if (parameter != null && parameter is bool)
            {
                var worksheet =
                    AssociatedSpreadsheet.ExcelProperties.WorkBook.Worksheets[
                        AssociatedSpreadsheet.GridProperties.CurrentSheetName];
                GridStyleInfo[] cellsInfo=null;

                if (AssociatedSpreadsheet.GridProperties.HasSelectedRange)
                {
                    foreach (GridRangeInfo item in AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.SelectedRanges)
                    {
                        string format = GetNumberFormat(worksheet.Range[item.Top == 0 ? item.Top + 1 : item.Top, item.Left == 0 ? item.Left + 1 : item.Left], (bool)parameter);
                        string range = item.ConvertGridRangeToExcelRange(AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel);
                        worksheet.Range[range].NumberFormat = format;
                        cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(item);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(item, cellsInfo, Styles.StyleModifyType.Copy);
                        AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(item);
                    }
                }
                else if (AssociatedSpreadsheet.GridProperties.CurrentCellStyle != null)
                {
                    var row = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.RowIndex;
                    var col = AssociatedSpreadsheet.GridProperties.CurrentCellStyle.ColumnIndex;
                    string range = GridRangeInfo.GetAlphaLabel(col) + row;
                    string format = GetNumberFormat(worksheet.Range[range], (bool) parameter);
                    worksheet.Range[range].NumberFormat = format;
                    GridRangeInfo rangeInfo = AssociatedSpreadsheet.GridProperties.CurrentCell.RangeInfo;
                    cellsInfo = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.GetCellsInfo(rangeInfo);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.ChangeCells(rangeInfo, cellsInfo, Styles.StyleModifyType.Copy);
                    AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateCell(GridRangeInfo.Cell(row, col));
                }
                AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel.InvalidateVisual(true);
                base.ExecuteCommand(parameter);
            }
        }

        internal string GetNumberFormat(IRange range,bool increase)
        {
            Match match=Match.Empty;
            if(range!=null)
            {
                var numberFormat = range.NumberFormat;

                if ((range.HasNumber || range.HasFormula) && !Regex.IsMatch(range.NumberFormat, @"([mdMy][,][ mdMy])|([mdMy][-/][mdMy])|([mdMy][-/](.*)[-/][mdMy])|([Mdy][\s][Mdy])", RegexOptions.IgnorePatternWhitespace))
                {
                    if (numberFormat == "0")
                    {
                        if (!increase)
                            return numberFormat;
                        else
                            return "0.0";
                    }

                    if (range.HasNumber)
                    {
                        if (numberFormat.Contains("."))
                            match = Regex.Match(numberFormat, @"\d+[.]\d+");
                        else if (range.Number.ToString().Contains("."))
                            match = Regex.Match(range.Number.ToString(), @"\d+[.]\d+");
                    }
                    else if (range.HasFormula)
                    {
                        string cellValue = AssociatedSpreadsheet.GridProperties.CurrentExcelGridModel[range.Row, range.Column].FormattedText;
                        if (Regex.IsMatch(cellValue, @"^\d+$") || numberFormat.Contains("."))
                        {
                            if (numberFormat.Contains("."))
                                match = Regex.Match(numberFormat, @"\d+[.]\d+");
                            else
                                match = Regex.Match(cellValue, @"\d+[.]\d+");
                        }
                        else if(!increase)
                            return numberFormat;
                    }

                    if (match.Success)
                    {
                        string replace;
                        if (increase)
                            replace = match.Value + "0";
                        else
                        {
                            var length = match.Value.Length;
                            replace = match.Value[length - 2] != '.' ? match.Value.Substring(0, length - 1) : match.Value.Substring(0, length - 2);
                        }

                        replace = Regex.Replace(replace, @"[1-9\-]", "0");
                        int indexOfPoint = replace.IndexOf('.');
                        replace = indexOfPoint > 1 ? replace.Substring(indexOfPoint - 1) : replace;

                        numberFormat = Regex.Replace(numberFormat,match.Value, replace);
                           
                        return numberFormat;
                    }
                    else if (increase)
                        return Regex.Replace(numberFormat, "0", "0.0");
                }                    
            }
                
            return range.NumberFormat;
        }

    }
}
