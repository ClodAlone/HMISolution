#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Documents;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Grid.Converter;
using Syncfusion.XlsIO;
using System.Text.RegularExpressions;
using System;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    public static class ImportExportHelper
    {
        /// <summary>
        /// Gets the excel like formula.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="workbook">The workbook.</param>
        /// <param name="formula">The formula.</param>
        /// <returns></returns>
        public static string GetExcelLikeFormula(GridModel model, IWorkbook workbook, string formula)
        {
            for (int i = 0; i < workbook.TabSheets.Count; i++)
            {
                string s = "'" + workbook.TabSheets[i].Name + "'";
                formula = formula.Replace(workbook.TabSheets[i].Name, s);
            }
            return formula;
        }

        /// <summary>
        /// Alignment was changed based on the cell value as like in excel
        /// </summary>
        /// <param name="range">Excel Range</param>
        /// <param name="Style">Grid cell style</param>
        public static void SetAlignment(IRange range, GridStyleInfo Style)
        {
            string cellvalue = string.Empty;
            int ivalue; double dvalue; bool bvalue; DateTime dtDate;
            if (Style.CellType == "FormulaCell" && !string.IsNullOrEmpty(Style.Text) && Style.Text[0] == '=')
            {
                if (Style.FormulaTag != null && Style.FormulaTag.Text != string.Empty)
                    cellvalue = Style.FormulaTag.Text;
                else
                    cellvalue = Style.FormattedText;
            }
            else if (Style.CellValue != null)
            {
                if (Style.CellValue.ToString().EndsWith("%"))
                    cellvalue = Style.CellValue.ToString().TrimEnd('%');
                else
                    cellvalue = Style.CellValue.ToString();
            }
            if (int.TryParse(cellvalue, out ivalue))
            {
                if (!Style.HasHorizontalAlignment && (Style.CellValue2 == null || Style.CellValue2.ToString() != "HAlignLeft") && !Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Right;
                    range.HorizontalAlignment = ExcelHAlign.HAlignRight;
                }
                else if (Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            else if (double.TryParse(cellvalue, out dvalue))
            {
                if (!Style.HasHorizontalAlignment && (Style.CellValue2 == null || Style.CellValue2.ToString() != "HAlignLeft") && !Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Right;
                    range.HorizontalAlignment = ExcelHAlign.HAlignRight;
                }
                else if (Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            else if (DateTime.TryParse(cellvalue, out dtDate))
            {
                if (!Style.HasHorizontalAlignment && (Style.CellValue2 == null || Style.CellValue2.ToString() != "HAlignLeft") && !Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Right;
                    range.HorizontalAlignment = ExcelHAlign.HAlignRight;
                }
                else if (Style.HasTextMargins)
                {
                    Style.HorizontalAlignment = HorizontalAlignment.Left;
                }
            }
            else if (bool.TryParse(cellvalue, out bvalue))
            {
                Style.HorizontalAlignment = HorizontalAlignment.Center;
                range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                if (cellvalue != "TRUE" || cellvalue != "FALSE")
                    Style.CellValue = bvalue ? "TRUE" : "FALSE";
            }
            //else if (Style.HasHorizontalAlignment && Style.HorizontalAlignment == HorizontalAlignment.Right)
            //{
            //    Style.HorizontalAlignment = HorizontalAlignment.Left;
            //}
        }

        /// <summary>
        /// Exports the cell to excel.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="workbook">The workbook.</param>
        /// <param name="range">The range.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridQueryCellInfoEventArgs"/> instance containing the event data.</param>
        public static void ExportCellToExcel(GridModel model, IWorkbook workbook, IRange range, GridCommitCellInfoEventArgs e)
        {
            int ivalue; double dvalue; bool bvalue; DateTime datevalue;
            string cellvalue = string.Empty;
            try
            {
                if (e.Style.CellValue != null)
                    cellvalue = e.Style.CellValue.ToString();
                if (e.Style.CellType == "FormulaCell" && e.Style.HasFormulaTag && !string.IsNullOrEmpty(e.Style.Text) && e.Style.Text[0] == '=')
                    cellvalue = e.Style.FormattedText;
                else if (e.Style.CellType == "RichText")
                {
#if !SILVERLIGHT 
                    RichTextBoxHelper.SetRichTextValue(range, e.Style.CellValue as FlowDocument, e.Style.FormattedText);
#else
                    RichTextBoxHelper.SetRichTextValue(range, e.Style.CellValue as Paragraph, e.Style.FormattedText);
#endif

                    return;
                }
                if (!range.HasFormula || e.Style.Text.Length == 0 || (e.Style.Text.Length > 0 && e.Style.Text[0] != '='))
                {
                    //To new Save Formula
                    if (e.Style.CellValue != null && e.Style.CellValue.ToString() != string.Empty && e.Style.CellValue.ToString()[0] == '=')
                    {
                        try
                        {
                            range.Formula = e.Style.CellValue.ToString().ToUpper();
                            //When applying the formulas not in excel, 
                            //below code will catch the exception and Text property will be updated
                            string text = range.Value;
                        }
                        catch (Exception)
                        {
                            range.Text = e.Style.CellValue.ToString().ToUpper();
                        }
                        if (!e.Style.HasHorizontalAlignment)
                        {
                            range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                        }
                        if (int.TryParse(e.Style.FormattedText, out ivalue))
                        {
                            e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                        }
                        else if (double.TryParse(e.Style.FormattedText, out dvalue))
                        {
                            e.Style.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                        }
                    }
                    //In Excel having more date time format like long date, short date and custom date, so I have used the following Regex to identify the Date time format.
                    else if (e.Style.HasFormat && Regex.IsMatch(e.Style.Format, @"([mdMy][,][ mdMy])|([mdMy][-/][mdMy])|([mdMy][-/](.*)[-/][mdMy])|([Mdy][\s][Mdy])", RegexOptions.IgnorePatternWhitespace))
                    {
                        string numberformat = range.NumberFormat;
                        if (double.TryParse(cellvalue, out dvalue))
                        {
                            DateTime dt = DateTime.FromOADate(dvalue);
                            range.DateTime = dt;
                            if (numberformat != string.Empty)
                            {
                                range.NumberFormat = numberformat;
                            }
                            return;
                        }
                        else if (DateTime.TryParse(cellvalue, out datevalue))
                        {
                            range.DateTime = datevalue;
                            if (numberformat != string.Empty)
                            {
                                range.NumberFormat = numberformat;
                            }
                            return;
                        }
                        else
                        {
                            range.Value = cellvalue;
                        }
                    }
                    else if (int.TryParse(cellvalue, out ivalue))
                    {
                        range.Number = ivalue;
                    }
                    else if (double.TryParse(cellvalue, out dvalue))
                    {
                        range.Number = dvalue;
                    }
                    else if (bool.TryParse(cellvalue, out bvalue))
                    {
                        range.Boolean = bvalue;
                    }
                    else
                    {
                        //When we type the number it aligns to Right and again if we change number to text it aligns to left. 
                        if (range.HasNumber)
                            range.HorizontalAlignment = ExcelHAlign.HAlignGeneral;
                        //To Save the Multiline text in the Workbook
                        if (cellvalue != null && cellvalue.Contains("\r\n"))
                            range.Text = cellvalue;
                        else
                            range.Value = cellvalue;
                    }
                }
                else if (e.Style.CellValue != null && e.Style.CellValue.ToString() != string.Empty && e.Style.CellValue.ToString()[0] == '=')
                {
                    string formula = GetExcelLikeFormula(model,workbook, e.Style.CellValue.ToString());
                    if (string.Compare(range.Formula, formula, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        try
                        {
                            range.Formula = formula;
                            //When applying the formulas not in excel, 
                            //below code will catch the exception and Text property will be updated
                            string text = range.Value;
                        }
                        catch (Exception)
                        {
                            range.Text = formula;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
