#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.PivotAnalysis.Base;
using Color = System.Drawing.Color;
using Syncfusion.Windows.Forms.PivotAnalysis;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;

namespace Syncfusion.PivotGridConverter
{
    /// <summary>
    /// Class set for exporting Pivot Grid to PDF document
    /// </summary>
    public class PivotPdfExport
    {
        /// <summary>
        /// Constructor for PivotPdfExport class with single argument
        /// </summary>
        /// <param name="gridControl"></param>
        public PivotPdfExport(PivotGridControl gridControl)
        {
            this.PivotGridControl = gridControl;
        }
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public PivotGridControl PivotGridControl { get; internal set; }

        PdfBrush headerColumnBackColor;
        PdfBrush headerColumnForeColor;
        PdfBrush headerRowBackColor;
        PdfBrush headerRowForeColor;
        PdfBrush summaryHeaderBackColor;
        PdfBrush summaryHeaderForeColor;
        PdfBrush summaryCellBackColor;
        PdfBrush summaryCellForeColor;
        PdfBrush valueCellBackColor;
        PdfBrush valueCellForeColor;
        /// <summary>
        /// Exports the specified file to PDF
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        public void Export(string fileName)
        {
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.PageSettings.Margins.All = 20;
            PdfPage page = pdfDocument.Pages.Add();
            PdfGrid pdfGrid = new PdfGrid();
            pdfGrid.Columns.Add(this.PivotGridControl.PivotEngine.ColumnCount);

            if (PivotGridControl.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
              { 
                    headerColumnBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#2abff1")));
                    headerColumnForeColor = new PdfSolidBrush(new PdfColor(Color.White));
                    headerRowBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#2abff1")));
                    headerRowForeColor = new PdfSolidBrush(new PdfColor(Color.White));
                    summaryHeaderBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#C0C0C0")));
                    summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.White));
                    valueCellBackColor =  new PdfSolidBrush(new PdfColor(Color.White));
                    valueCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    summaryCellBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#C0C0C0")));
                    summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.White));
               }

            if (PivotGridControl.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue||PivotGridControl.GridVisualStyles==Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue)
            {
                
                    headerColumnBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#dbeaff")));
                    headerColumnForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    headerRowBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#dbeaff")));
                    headerRowForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    summaryHeaderBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
                    valueCellBackColor = new PdfSolidBrush(new PdfColor(Color.White));
                    valueCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    summaryCellBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
             }

            if (PivotGridControl.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black||PivotGridControl.GridVisualStyles==Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black)
            {   
                    headerColumnBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#656565")));
                    headerColumnForeColor = new PdfSolidBrush(new PdfColor(Color.White));
                    headerRowBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#656565")));
                    headerRowForeColor = new PdfSolidBrush(new PdfColor(Color.White));
                    summaryHeaderBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
                    valueCellBackColor = new PdfSolidBrush(new PdfColor(Color.White));
                    valueCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    summaryCellBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
             }

            if (PivotGridControl.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver||PivotGridControl.GridVisualStyles==GridVisualStyles.Office2010Silver)
            {
                    headerColumnBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#dcddde")));
                    headerColumnForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
                    headerRowBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#dcddde")));
                    headerRowForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
                    summaryHeaderBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
                    valueCellBackColor = new PdfSolidBrush(new PdfColor(Color.White));
                    valueCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                    summaryCellBackColor = new PdfSolidBrush(new PdfColor(HexToColor("#fbe292")));
                    summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.Gray));
             }
                PdfStringFormat format = new PdfStringFormat();
                format.LineAlignment = PdfVerticalAlignment.Middle;
                int rowCount = this.PivotGridControl.PivotEngine.RowCount;
                int colCount = this.PivotGridControl.PivotEngine.ColumnCount;

                if (!this.PivotGridControl.PivotEngine.ShowGrandTotals)
                {
                    rowCount -= 1;
                    colCount -= this.PivotGridControl.PivotCalculations.Count > 1 ? this.PivotGridControl.PivotCalculations.Count - 1 : 1;
                }
                for (int row = 0; row < rowCount; row++)
                {
                    PdfGridRow pdfRow = pdfGrid.Rows.Add();
                    for (int cell = 0; cell < colCount; cell++)
                    {
                        PivotCellInfo pivotCellInfo = this.PivotGridControl.PivotEngine[row, cell];
                        PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                        if (pivotCellInfo != null)
                        {                            
                            if (pivotCellInfo.CellRange != null)
                            {
                                pdfRow.Cells[cell].ColumnSpan = pivotCellInfo.CellRange.Right - pivotCellInfo.CellRange.Left + 1;
                                pdfRow.Cells[cell].RowSpan = pivotCellInfo.CellRange.Bottom - pivotCellInfo.CellRange.Top + 1;
                            }

                            switch (pivotCellInfo.CellType)
                            {
                                case (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell):
                                case (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell):
                                case (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell):
                                    cellStyle = ColHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, format);
                                    break;
                                case (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell):
                                case (PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell):
                                case (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell):
                                    cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, format);
                                    break;
                                case (PivotCellType.TotalCell | PivotCellType.RowHeaderCell):
                                case (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell):
                                case (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell):
                                case (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell):
                                case (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell):
                                case (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell):
                                case (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell):
                                case (PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell):
                                     cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor,
                                                               format);
                                    break;
                                case (PivotCellType.ValueCell | PivotCellType.TotalCell) :
                                case (PivotCellType.ValueCell | PivotCellType.GrandTotalCell):
                                case (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell):
                                     cellStyle = SummaryCellStyle(cellStyle, summaryCellBackColor, summaryCellForeColor, format);
                                     break;
                                case (PivotCellType.ValueCell):
                                    cellStyle = ValueCellStyle(cellStyle, valueCellBackColor, valueCellForeColor, format);
                                    break;

                            }
                            cellStyle.Borders.All = new PdfPen(new PdfColor(Color.Black), .2f);
                            pdfRow.Cells[cell].Value = pivotCellInfo.FormattedText ?? string.Empty;
                            pdfRow.Cells[cell].Style = cellStyle;
                        }
                    }
                }
                pdfGrid.Style.AllowHorizontalOverflow = true;
                pdfGrid.Draw(page, PointF.Empty);
                pdfDocument.Save(fileName);
        }
        /// <summary>
        /// Formats the summary cells
        /// </summary>
        /// <param name="cellStyle">Style for the cell</param>
        /// <param name="summaryCellBackColor">Background color for the cell</param>
        /// <param name="summaryCellForeColor">Background color for the fonts</param>
        /// <param name="format">Format for the contents</param>
        /// <returns></returns>
        private PdfGridCellStyle SummaryCellStyle(PdfGridCellStyle cellStyle, PdfBrush summaryCellBackColor, PdfBrush summaryCellForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryCellBackColor;
            cellStyle.TextBrush = summaryCellForeColor;
            cellStyle.Font = GetFontInfo("Segoe UI", "Normal", 8);
            cellStyle.StringFormat = format;
            return cellStyle;
        }
        /// <summary>
        /// Formats the value cell
        /// </summary>
        /// <param name="cellStyle">Style for the cell</param>
        /// <param name="valueCellBackColor">Background color for the cell</param>
        /// <param name="valueCellForeColor">Background color for the fonts</param>
        /// <param name="format">Format for the contents</param>
        /// <returns></returns>
        private PdfGridCellStyle ValueCellStyle(PdfGridCellStyle cellStyle, PdfBrush valueCellBackColor, PdfBrush valueCellForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = valueCellBackColor;
            cellStyle.TextBrush = valueCellForeColor;
            cellStyle.Font = GetFontInfo("Segoe UI", "Normal",8);
            cellStyle.StringFormat = format;
            return cellStyle;
        }
        /// <summary>
        /// Applies styles for the column headers
        /// </summary>
        /// <param name="cellStyle">Style for the cell</param>
        /// <param name="headerColBackColor">Background color for the column</param>
        /// <param name="headerColForeColor">Background color for the fonts</param>
        /// <param name="format">Format for the contents</param>
        /// <returns></returns>
        private PdfGridCellStyle ColHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerColBackColor, PdfBrush headerColForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerColBackColor;
            cellStyle.TextBrush = headerColForeColor;
            cellStyle.Font = GetFontInfo("Segoe UI", "Normal", 8);
            cellStyle.StringFormat = format;
            return cellStyle;
        }
        /// <summary>
        /// Applies styles for the row headers
        /// </summary>
        /// <param name="cellStyle">Style for the cell</param>
        /// <param name="headerRowBackColor">Background color for the row</param>
        /// <param name="headerRowForeColor">Background color for the fonts</param>
        /// <param name="format">Format for the contents</param>
        /// <returns></returns>
        private PdfGridCellStyle RowHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerRowBackColor, PdfBrush headerRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerRowBackColor;
            cellStyle.TextBrush = headerRowForeColor;
            cellStyle.Font = GetFontInfo("Segoe UI", "Normal", 8);
            cellStyle.StringFormat = format;
            return cellStyle;
        }
        /// <summary>
        /// Applies styles for the Summary header
        /// </summary>
        /// <param name="cellStyle">Style for the Summary header</param>
        /// <param name="summaryRowBackColor">Background color for the header</param>
        /// <param name="summaryRowForeColor">Background color for the font</param>
        /// <param name="format">Format for the contents</param>
        /// <returns></returns>
        private PdfGridCellStyle SummaryHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush summaryRowBackColor, PdfBrush summaryRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryRowBackColor;
            cellStyle.TextBrush = summaryRowForeColor;
            cellStyle.Font = GetFontInfo("Segoe UI", "Normal", 8);
            cellStyle.StringFormat = format;
            return cellStyle;
        }
        /// <summary>
        /// Gets the font styles
        /// </summary>
        /// <param name="fontName">Name of the font</param>
        /// <param name="fontStyle">Style of the font</param>
        /// <param name="fontSize">Size of the font</param>
        /// <returns></returns>
        private PdfFont GetFontInfo(string fontName, string fontStyle, double fontSize)
        {
            switch (fontName)
            {
                case "Courier":
                    if (fontStyle == PdfFontStyle.Bold.ToString())
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, float.Parse(fontSize.ToString()), PdfFontStyle.Bold);
                        return font;
                    }
                    else
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, float.Parse(fontSize.ToString()), PdfFontStyle.Regular);
                        return font;
                    }
                case "Helvetica":
                    if (fontStyle == PdfFontStyle.Bold.ToString())
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, float.Parse(fontSize.ToString()), PdfFontStyle.Bold);
                        return font;
                    }
                    else
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, float.Parse(fontSize.ToString()), PdfFontStyle.Regular);
                        return font;
                    }
                case "TimesRoman":
                    if (fontStyle == PdfFontStyle.Bold.ToString())
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, float.Parse(fontSize.ToString()), PdfFontStyle.Bold);
                        return font;
                    }
                    else
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.TimesRoman, float.Parse(fontSize.ToString()), PdfFontStyle.Regular);
                        return font;
                    }
                case "ZapfDingbats":
                    if (fontStyle == PdfFontStyle.Bold.ToString())
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.ZapfDingbats, float.Parse(fontSize.ToString()), PdfFontStyle.Bold);
                        return font;
                    }
                    else
                    {
                        PdfFont font = new PdfStandardFont(PdfFontFamily.ZapfDingbats, float.Parse(fontSize.ToString()), PdfFontStyle.Regular);
                        return font;
                    }
                default:
                    PdfFont pdfFont = new PdfStandardFont(PdfFontFamily.TimesRoman, float.Parse(fontSize.ToString()), PdfFontStyle.Regular);
                    return pdfFont;
            }
            
        }
        /// <summary>
        /// Converts the haxadecimal value to color
        /// </summary>
        /// <param name="hexVal">haxadecimal value</param>
        /// <returns>color for the concerned hexadecimal value</returns>
        private Color HexToColor(string hexVal)
        {
            string hAlpha;
            string hRed;
            string hGreen;
            string hBlue;
            hexVal = hexVal.Replace("#", string.Empty);
            if (hexVal.Length == 6 || hexVal.Length == 8)
            {
                if (hexVal.Length == 6)
                {
                    hRed = hexVal.Substring(0, 2);
                    hGreen = hexVal.Substring(2, 2);
                    hBlue = hexVal.Substring(4, 2);
                    return Color.FromArgb(Convert.ToInt32(hRed, 16),Convert.ToInt32(hGreen, 16), Convert.ToInt32(hBlue, 16));
                }
                hAlpha = hexVal.Substring(0, 2);
                hRed = hexVal.Substring(2, 2);
                hGreen = hexVal.Substring(4, 2);
                hBlue = hexVal.Substring(6, 2);
                return Color.FromArgb(Convert.ToInt32(hAlpha, 16), Convert.ToInt32(hRed, 16),Convert.ToInt32(hGreen, 16), Convert.ToInt32(hBlue, 16));
            }
            throw new ArgumentException(@"HexVal must be 6 or 8 characters in length", "hexVal");
        }

    }
}
