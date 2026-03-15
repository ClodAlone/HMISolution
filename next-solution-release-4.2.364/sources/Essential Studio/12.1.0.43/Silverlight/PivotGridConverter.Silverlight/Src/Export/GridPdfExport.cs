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
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.IO;
using Syncfusion.Silverlight.Controls.PivotGrid;


namespace Syncfusion.Silverlight.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridPdfExport exports the Pivot data to Word with the applied style
    /// </summary>
    public class GridPdfExport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridPdfExport"/> class.
        /// </summary>
        /// <param name="pivotGridControl">The pivot grid control.</param>
        public GridPdfExport(PivotGridControl pivotGridControl)
        {
            this.PivotGridControl = pivotGridControl;
        }
        
        /// <summary>
        /// Gets or sets the pivot grid control.
        /// </summary>
        /// <value>The pivot grid control.</value>
        public PivotGridControl PivotGridControl { get; internal set; }

        /// <summary>
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.PageSettings.Margins.All = 20;
            PdfPage page = pdfDocument.Pages.Add();

            PdfGrid pdfGrid = new PdfGrid();

            pdfGrid.Columns.Add(this.PivotGridControl.PivotEngine.ColumnCount);

            PdfBrush headerColumnBackColor;
            PdfBrush headerColumnForeColor;
            PdfBrush headerRowBackColor;
            PdfBrush headerRowForeColor;
            if (this.PivotGridControl.ColumnHeaderCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Background);
                headerColumnBackColor = colorObjBack != null
                                            ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjBack.ToString())))
                                            : new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E9F1")));

                //headerColumnForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Foreground);
                headerColumnForeColor = colorObjFore != null
                                            ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjFore.ToString())))
                                            : new PdfSolidBrush(new PdfColor(Colors.Black));
            }
            else
            {
                headerColumnBackColor =
                    new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E9F1")));
                headerColumnForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
            }

            if (this.PivotGridControl.RowHeaderCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Background);
                headerRowBackColor = colorObjBack != null
                                         ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjBack.ToString())))
                                         : new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E9F1")));

                //headerRowForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Foreground);
                headerRowForeColor = colorObjFore != null
                                         ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjFore.ToString())))
                                         : new PdfSolidBrush(new PdfColor(Colors.Black));
            }
            else
            {
                headerRowBackColor =
                    new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E9F1")));
                headerRowForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
            }


            ////Sets Pdf Table summary style color
            PdfBrush summaryHeaderBackColor;
            PdfBrush summaryHeaderForeColor;
            if (this.PivotGridControl.SummaryHeaderStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Background);
                summaryHeaderBackColor = colorObjBack != null
                                             ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjBack.ToString())))
                                             : new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E9F1")));

                //summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Foreground);
                summaryHeaderForeColor = colorObjFore != null
                                             ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjFore.ToString())))
                                             : new PdfSolidBrush(new PdfColor(Colors.Black));
            }
            else
            {
                summaryHeaderBackColor =
                    new PdfSolidBrush(new PdfColor(GetColorFromHex("#FFE3E5E6")));
                summaryHeaderForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
            }

            //// Vaule Cell
            PdfBrush valueCellBackColor;
            PdfBrush valueCellForeColor;
            if (this.PivotGridControl.ValueCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Background);
                valueCellBackColor = colorObjBack != null
                                         ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjBack.ToString())))
                                         : new PdfSolidBrush(new PdfColor(Colors.White));

                //valueCellForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Foreground);
                valueCellForeColor = colorObjFore != null
                                         ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjFore.ToString())))
                                         : new PdfSolidBrush(new PdfColor(Colors.Black));
            }
            else
            {
                valueCellBackColor =
                    new PdfSolidBrush(new PdfColor(Colors.White));
                valueCellForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
            }

            //// Vaule Cell
            PdfBrush summaryCellBackColor;
            PdfBrush summaryCellForeColor;
            if (this.PivotGridControl.SummaryCellStyle != null)
            {
                Object colorObjBack = GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Background);
                summaryCellBackColor = colorObjBack != null
                                           ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjBack.ToString())))
                                           : new PdfSolidBrush(new PdfColor(Colors.White));

                //summaryCellForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
                Object colorObjFore = GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Foreground);
                summaryCellForeColor = colorObjFore != null
                                           ? new PdfSolidBrush(new PdfColor(GetColorFromHex(colorObjFore.ToString())))
                                           : new PdfSolidBrush(new PdfColor(Colors.Black));
            }
            else
            {
                summaryCellBackColor =
                    new PdfSolidBrush(new PdfColor(Colors.White));
                summaryCellForeColor = new PdfSolidBrush(new PdfColor(Colors.Black));
            }

            ////Sets format for Pdf Text and allignment.
            PdfStringFormat headerStringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Left };
            PdfStringFormat valueStringFormat = new PdfStringFormat { LineAlignment = PdfVerticalAlignment.Middle, Alignment = PdfTextAlignment.Right };

            for (int row = 0; row < this.PivotGridControl.PivotEngine.RowCount; row++)
            {
                PdfGridRow pdfRow = pdfGrid.Rows.Add();
                for (int cell = 0; cell < this.PivotGridControl.PivotEngine.ColumnCount; cell++)
                {
                    PivotCellInfo pivotCellInfo = this.PivotGridControl.PivotEngine[row, cell];
                    PdfGridCellStyle cellStyle = new PdfGridCellStyle();
                    if (pivotCellInfo != null)
                    {
                        if (pivotCellInfo.CellRange != null)
                        {
                            pdfRow.Cells[cell].ColumnSpan = pivotCellInfo.CellRange.Right -
                                                            pivotCellInfo.CellRange.Left + 1;

                            pdfRow.Cells[cell].RowSpan = pivotCellInfo.CellRange.Bottom -
                                                         pivotCellInfo.CellRange.Top + 1;
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
                                 pivotCellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
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
                        cellStyle.Borders.All = new PdfPen(new PdfColor(Colors.Black), .2f);
                        pdfRow.Cells[cell].Value = pivotCellInfo.FormattedText ?? string.Empty;
                        pdfRow.Cells[cell].Style = cellStyle;
                    }
                }
            }
            pdfGrid.Style.AllowHorizontalOverflow = true;
            pdfGrid.Draw(page, 0, 0);
            pdfDocument.Save(stream);
        }


        /// <summary>
        /// Summaries the cell style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="summaryCellBackColor">Color of the summary cell back.</param>
        /// <param name="summaryCellForeColor">Color of the summary cell fore.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Values the cell style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="valueCellBackColor">Color of the value cell back.</param>
        /// <param name="valueCellForeColor">Color of the value cell fore.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Cols the header style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="headerColBackColor">Color of the header col back.</param>
        /// <param name="headerColForeColor">Color of the header col fore.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Rows the header style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="headerRowBackColor">Color of the header row back.</param>
        /// <param name="headerRowForeColor">Color of the header row fore.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Summaries the header style.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="summaryRowBackColor">Color of the summary row back.</param>
        /// <param name="summaryRowForeColor">Color of the summary row fore.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the font info.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="fontStyle">The font style.</param>
        /// <param name="fontSize">Size of the font.</param>
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
        /// Gets the color from gradient.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        private string GetColorFromGradient(Brush brush)
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

        /// <summary>
        /// Gets the color from hex.
        /// </summary>
        /// <param name="myColor">My color.</param>
        /// <returns></returns>
        private Color GetColorFromHex(string myColor)
        {
            return Color.FromArgb(
                Convert.ToByte(myColor.Substring(1, 2), 16),
                Convert.ToByte(myColor.Substring(3, 2), 16),
                Convert.ToByte(myColor.Substring(5, 2), 16),
                Convert.ToByte(myColor.Substring(7, 2), 16));
        }
    }
}
