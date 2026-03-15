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
using System.Windows.Media;
using Syncfusion.PivotAnalysis.Base;
using Color = System.Drawing.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using Syncfusion.Windows.Controls.PivotGrid;

namespace Syncfusion.Windows.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridPdfExport exports the Pivot data to PDF document with the applied style
    /// </summary>
    public class GridPdfExport
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridPdfExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public GridPdfExport(PivotGridControl gridControl)
        {
            this.PivotGridControl = gridControl;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the pivot grid control.
        /// </summary>
        /// <value>The pivot grid control.</value>
        public PivotGridControl PivotGridControl { get; internal set; }


        #endregion

        #region Helper Methods
        /// <summary>
        /// Exports the specified file name based on document settings.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="pdfDocument">Document settings as a <see cref="PdfDocument"/> type.</param>
        public void Export(string fileName, PdfDocument pdfDocument)
        {
            this.Export(fileName, pdfDocument, true);
        }

        /// <summary>
        /// Exports the specified file name based on document settings.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="pdfDocument">Document settings as a <see cref="PdfDocument"/> type.</param>
        /// <param name="allowHorizontalOverflow">Define whether can allow horizontal overflow.</param>
        public void Export(string fileName, PdfDocument pdfDocument, bool allowHorizontalOverflow)
        {
            if (pdfDocument == null) 
            { 
                pdfDocument = new PdfDocument();
                pdfDocument.PageSettings.Margins.All = 20;
            }
             PdfPage page = pdfDocument.Pages.Add();

            PdfGrid pdfGrid = new PdfGrid();

            List<int> sumcol= new List<int>();

            List<int> sumrow = new List<int>();

            int rowOffset = this.PivotGridControl.PivotColumns.Count + (this.PivotGridControl.PivotCalculations.Count > 0 ? 1 : 0);

            int colOffset = this.PivotGridControl.PivotRows.Count;

            for (int p = 0; p < this.PivotGridControl.PivotColumns.Count - 1; p++)
            {
                for (int j = 0; j < this.PivotGridControl.PivotEngine.ColumnCount; j++)
                {
                    PivotCellInfo pivotCellInfo = this.PivotGridControl.PivotEngine[rowOffset, j];
                    if (pivotCellInfo != null)
                    {
                        if (pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell))
                        {
                            if (!this.PivotGridControl.ShowSubTotals || !this.PivotGridControl.PivotEngine.PivotColumns[p].ShowSubTotal)
                            {
                                sumcol.Add(j);
                            }
                        }
                    }
                }
            }
            for (int p = 0; p < this.PivotGridControl.PivotRows.Count - 1; p++)
            {
                for (int i = 0; i < this.PivotGridControl.PivotEngine.RowCount; i++)
                {
                    PivotCellInfo pivotCellInfo = this.PivotGridControl.PivotEngine[i, colOffset];
                    if (pivotCellInfo != null)
                    {
                        if (pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell))
                        {
                            if (!this.PivotGridControl.ShowSubTotals || !this.PivotGridControl.PivotEngine.PivotRows[p].ShowSubTotal)
                            {
                                sumrow.Add(i);
                            }
                        }
                    }
                }
            }
            pdfGrid.Columns.Add(this.PivotGridControl.PivotEngine.ColumnCount - sumcol.Count);

            PdfBrush headerColumnBackColor;
            PdfBrush headerColumnForeColor;
            PdfBrush headerRowBackColor;
            PdfBrush headerRowForeColor;
            if (this.PivotGridControl.ColumnHeaderCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Background);
                headerColumnBackColor = colorObjBack != null ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjBack.ToString()))) : new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E9F1")));

                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Foreground);
                headerColumnForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjFore.ToString()))) : new PdfSolidBrush(new PdfColor(Color.Black));
            }
            else
            {
                headerColumnBackColor =
                    new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E9F1")));
                headerColumnForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
            }

            if (this.PivotGridControl.RowHeaderCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Background);
                headerRowBackColor = colorObjBack != null ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjBack.ToString()))) : new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E9F1")));

                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Foreground);
                headerRowForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjFore.ToString()))) : new PdfSolidBrush(new PdfColor(Color.Black));
            }
            else
            {
                headerRowBackColor =
                    new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E9F1")));
                headerRowForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
            }


            ////Sets Pdf Table summary style color
            PdfBrush summaryHeaderBackColor;
            PdfBrush summaryHeaderForeColor;
            if (this.PivotGridControl.SummaryHeaderStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Background);
                summaryHeaderBackColor = colorObjBack != null ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjBack.ToString()))) : new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E9F1")));

                //summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                Object colorObjFore = this.PivotGridControl.SummaryHeaderStyle.Foreground;// GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Foreground);
                summaryHeaderForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjFore.ToString()))) : new PdfSolidBrush(new PdfColor(Color.Black));
            }
            else
            {
                summaryHeaderBackColor =
                   new PdfSolidBrush(new PdfColor(HexToColor("#FFE3E5E6")));
                summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
            }

            //// Vaule Cell
            PdfBrush valueCellBackColor;
            PdfBrush valueCellForeColor;
            if (this.PivotGridControl.ValueCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Background);
                valueCellBackColor = colorObjBack != null ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjBack.ToString()))) : new PdfSolidBrush(new PdfColor(Color.White));

                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Foreground);
                valueCellForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjFore.ToString()))) : new PdfSolidBrush(new PdfColor(Color.Black));
            }
            else
            {
                valueCellBackColor =
                    new PdfSolidBrush(new PdfColor(Color.White));
                valueCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
            }

            //// Vaule Cell
            PdfBrush summaryCellBackColor;
            PdfBrush summaryCellForeColor;
            if (this.PivotGridControl.SummaryCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Background);
                summaryCellBackColor = colorObjBack != null ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjBack.ToString()))) : new PdfSolidBrush(new PdfColor(Color.White));

                //summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Foreground);
                summaryCellForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(ColorTranslator.FromHtml(colorObjFore.ToString()))) : new PdfSolidBrush(new PdfColor(Color.Black));
            }
            else
            {
                summaryCellBackColor =
                    new PdfSolidBrush(new PdfColor(Color.White));
                summaryCellForeColor = new PdfSolidBrush(new PdfColor(Color.Black));
            }

            ////Sets format for Pdf Text and allignment.
            PdfStringFormat headerStringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Left };
            PdfStringFormat valueStringFormat = new PdfStringFormat { Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Middle };

            int rowCount = this.PivotGridControl.PivotEngine.RowCount;
            int colCount = this.PivotGridControl.PivotEngine.ColumnCount;
            if (!this.PivotGridControl.PivotEngine.ShowGrandTotals)
            { 
                rowCount -= 1;
                colCount -= this.PivotGridControl.PivotCalculations.Count > 1 ? this.PivotGridControl.PivotCalculations.Count - 1 : 1;
            }
            for (int row = 0; row < rowCount; row++)
            {
                if (!sumrow.Contains(row))
                {
                    PdfGridRow pdfRow = pdfGrid.Rows.Add();
                    for (int cell = 0, col = 0; cell < colCount - sumcol.Count; col++)
                    {
                        if (sumcol.Contains(col))
                            continue;
                        PivotCellInfo pivotCellInfo = this.PivotGridControl.PivotEngine[row, col];
                        PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                        if (pivotCellInfo != null)
                        {
                            if (pivotCellInfo.CellRange != null)
                            {
                                if (!sumcol.Contains(col) && !sumrow.Contains(row))
                                {
                                    pdfRow.Cells[cell].ColumnSpan = pivotCellInfo.CellRange.Right -
                                                                    pivotCellInfo.CellRange.Left + 1;
                                    pdfRow.Cells[cell].RowSpan = pivotCellInfo.CellRange.Bottom -
                                                                 pivotCellInfo.CellRange.Top + 1;
                                }

                            }

                            if (pivotCellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                                !pivotCellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                !pivotCellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                            {
                                cellStyle = ColHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, headerStringFormat);
                            }
                            else if (pivotCellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                     !pivotCellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                     !pivotCellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                            {
                                cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, headerStringFormat);
                            }
                            else if (pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell | PivotCellType.CalculationHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell))
                            {
                                cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor,
                                                               headerStringFormat);
                            }
                            else if (pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell) ||
                                     pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell))
                            {
                                cellStyle = SummaryCellStyle(cellStyle, summaryCellBackColor, summaryCellForeColor, valueStringFormat);
                            }
                            else if (pivotCellInfo.CellType == PivotCellType.ValueCell)
                            {
                                cellStyle = ValueCellStyle(cellStyle, valueCellBackColor, valueCellForeColor, valueStringFormat);
                            }
                            /*switch (pivotCellInfo.CellType)
                            {
                                case PivotCellType.TotalCell | PivotCellType.RowHeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.HeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell | PivotCellType.HeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.HeaderCell | PivotCellType.RowHeaderCell:
                                    cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, format);
                                    break;
                                case PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell:
                                    cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, format);
                                    break;
                                case PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell:
                                    cellStyle = ColHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, format);
                                    break;
                                case PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell:
                                    cellStyle = SummaryHeaderStyle(cellStyle, summaryHeaderBackColor, summaryHeaderForeColor, format);
                                    break;
                                case PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell:
                                    cellStyle = ColHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, format);
                                    break;
                                case PivotCellType.ValueCell:
                                    cellStyle = ValueCellStyle(cellStyle, valueCellBackColor, valueCellForeColor, format);
                                    break;
                                case PivotCellType.ValueCell | PivotCellType.TotalCell:
                                    cellStyle = SummaryCellStyle(cellStyle, summaryCellBackColor, summaryCellForeColor, format);
                                    break;
                                case PivotCellType.ValueCell | PivotCellType.GrandTotalCell:
                                    cellStyle = SummaryCellStyle(cellStyle, summaryCellBackColor, summaryCellForeColor, format);
                                    break;
                            }*/
                            cellStyle.Borders.All = new PdfPen(new PdfColor(Color.Black), .2f);
                            if (this.PivotGridControl.ShowSubTotals)
                            {
                                pdfRow.Cells[cell].Value = pivotCellInfo.FormattedText ?? string.Empty;
                                pdfRow.Cells[cell].Style = cellStyle;
                            }
                            else
                            {
                                if (cell < pdfGrid.Columns.Count && !sumrow.Contains(row) && !sumcol.Contains(col))
                                {
                                    pdfRow.Cells[cell].Value = pivotCellInfo.FormattedText ?? string.Empty;
                                    pdfRow.Cells[cell].Style = cellStyle;
                                }
                            }
                        }
                        cell++;
                    }
                }
            }
            pdfGrid.Style.AllowHorizontalOverflow = allowHorizontalOverflow; 
            pdfGrid.Draw(page, PointF.Empty);
            pdfDocument.Save(fileName);
        }

        /// <summary>
        /// Exports the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Export(string fileName)
        {
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.PageSettings.Margins.All = 20;
            Export(fileName, pdfDocument);
        }
           
        private PdfGridCellStyle SummaryCellStyle(PdfGridCellStyle cellStyle, PdfBrush summaryCellBackColor, PdfBrush summaryCellForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryCellBackColor;
            cellStyle.TextBrush = summaryCellForeColor;
            if (this.PivotGridControl.SummaryCellStyle != null)
            {
                cellStyle.Font = GetFontInfo(this.PivotGridControl.SummaryCellStyle.FontFamily.ToString(),
                                             this.PivotGridControl.SummaryCellStyle.FontWeight.ToString(),
                                             this.PivotGridControl.SummaryCellStyle.FontSize);
            }
            else
            {
                cellStyle.Font = GetFontInfo("TimesRoman", "Normal", 10);
            }
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        private PdfGridCellStyle ValueCellStyle(PdfGridCellStyle cellStyle, PdfBrush valueCellBackColor, PdfBrush valueCellForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = valueCellBackColor;
            cellStyle.TextBrush = valueCellForeColor;
            if (this.PivotGridControl.ValueCellStyle != null)
            {
                cellStyle.Font = GetFontInfo(this.PivotGridControl.ValueCellStyle.FontFamily.ToString(),
                                             this.PivotGridControl.ValueCellStyle.FontWeight.ToString(),
                                             this.PivotGridControl.ValueCellStyle.FontSize);
            }
            else
            {
                cellStyle.Font = GetFontInfo("TimesRoman", "Normal", 10);
            }
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        private PdfGridCellStyle ColHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerColBackColor, PdfBrush headerColForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerColBackColor;
            cellStyle.TextBrush = headerColForeColor;
            if (this.PivotGridControl.ColumnHeaderCellStyle != null)
            {
                cellStyle.Font = GetFontInfo(this.PivotGridControl.ColumnHeaderCellStyle.FontFamily.ToString(),
                                             this.PivotGridControl.ColumnHeaderCellStyle.FontWeight.ToString(),
                                             this.PivotGridControl.ColumnHeaderCellStyle.FontSize);
            }
            else
            {
                cellStyle.Font = GetFontInfo("TimesRoman", "Normal", 10);
            }
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        private PdfGridCellStyle RowHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerRowBackColor, PdfBrush headerRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerRowBackColor;
            cellStyle.TextBrush = headerRowForeColor;
            if (this.PivotGridControl.RowHeaderCellStyle != null)
            {
                cellStyle.Font = GetFontInfo(this.PivotGridControl.RowHeaderCellStyle.FontFamily.ToString(),
                                             this.PivotGridControl.RowHeaderCellStyle.FontWeight.ToString(),
                                             this.PivotGridControl.RowHeaderCellStyle.FontSize);
            }
            else
            {
                cellStyle.Font = GetFontInfo("TimesRoman", "Normal", 10);
            }
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        private PdfGridCellStyle SummaryHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush summaryRowBackColor, PdfBrush summaryRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryRowBackColor;
            cellStyle.TextBrush = summaryRowForeColor;
            if (this.PivotGridControl.SummaryHeaderStyle != null)
            {
                cellStyle.Font = GetFontInfo(this.PivotGridControl.SummaryHeaderStyle.FontFamily.ToString(),
                                             this.PivotGridControl.SummaryHeaderStyle.FontWeight.ToString(),
                                             this.PivotGridControl.SummaryHeaderStyle.FontSize);
            }
            else
            {
                cellStyle.Font = GetFontInfo("TimesRoman", "Normal", 10);
            }
            cellStyle.StringFormat = format;
            return cellStyle;
        }

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
            //return null;
        }

        private string GetColorFromGradient(System.Windows.Media.Brush brush)
        {
            if (brush is SolidColorBrush)
            {
                SolidColorBrush solidColorBrush = brush as SolidColorBrush;
                return solidColorBrush.Color.ToString();
            }
            if (brush is LinearGradientBrush)
            {
                LinearGradientBrush gradientBrush = brush as LinearGradientBrush;
                if (gradientBrush.GradientStops.Count > 0)
                {
                    return gradientBrush.GradientStops[0].Color.ToString();
                }
                return brush.ToString();
            }

            return Colors.White.ToString();
        }

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
                    return Color.FromArgb(Convert.ToInt32(hRed, 16),
                                          Convert.ToInt32(hGreen, 16), Convert.ToInt32(hBlue, 16));
                }

                hAlpha = hexVal.Substring(0, 2);
                hRed = hexVal.Substring(2, 2);
                hGreen = hexVal.Substring(4, 2);
                hBlue = hexVal.Substring(6, 2);
                return Color.FromArgb(Convert.ToInt32(hAlpha, 16), Convert.ToInt32(hRed, 16),
                                      Convert.ToInt32(hGreen, 16), Convert.ToInt32(hBlue, 16));
            }

            throw new ArgumentException(@"HexVal must be 6 or 8 characters in length", "hexVal");
        }

        #endregion
    }
}
