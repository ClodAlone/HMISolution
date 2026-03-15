#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Syncfusion.Silverlight.Grid.Olap.Common;

namespace Syncfusion.Silverlight.Grid.Olap.Converter
{
    /// <summary>
    /// GridPdfExport exports the Pivot data to Pdf with the applied style
    /// </summary>
    public class GridPdfExport
    {
        #region Private Members

        private ExportingGridStyleInfo GridStyleInfo = new ExportingGridStyleInfo();
        private PivotEngine Engine { get; set; }
        private const string m_Total = "Total";
        private const string m_GrandTotal = "Grand Total";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridPdfExport"/> class.
        /// </summary>
        /// <param name="Engine">The engine.</param>
        /// <param name="GridStyleInfo">The grid style info.</param>
        public GridPdfExport(PivotEngine Engine, ExportingGridStyleInfo GridStyleInfo)
        {
            this.Engine = Engine;
            if (GridStyleInfo != null)
                this.GridStyleInfo = GridStyleInfo;               
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridPdfExport"/> class.
        /// </summary>
        /// <param name="engine">The pivot engine</param>
        /// <param name="gridStyleInfo">The grid style info</param>
        public GridPdfExport(PivotEngine engine, object gridStyleInfo)
            : this(engine, (ExportingGridStyleInfo)gridStyleInfo)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            PdfDocument document = new PdfDocument();
            document.PageSettings.Margins.All = 20;
            PdfPage page = document.Pages.Add();

            PdfGrid pdfGrid = new PdfGrid();

            for (int col = 0; col < Engine[0, 0].Range.Width; col++)
            {
                PivotColumnDescriptor colDesc = Engine.TableColumns[col];
                for (int cell = 0; cell < Engine[0, 0].Range.Height; cell++)
                {
                    PivotCellDescriptor cellDescriptor = colDesc.Cells[cell];
                    cellDescriptor.CellValue = string.Empty;
                    cellDescriptor.CellType = PivotCellDescriptorType.Any;
                }
            }

            pdfGrid.Columns.Add(Engine.TableColumns.Count);

            ////Sets Pdf Table header style color
            PdfBrush headerColumnBackColor = new PdfSolidBrush(GridStyleInfo.HeaderBackgroundColor);
            PdfBrush headerColumnForeColor = new PdfSolidBrush(GridStyleInfo.HeaderForeGroundColor);

            PdfBrush headerRowBackColor = new PdfSolidBrush(GridStyleInfo.HeaderRowBackgroundColor);
            PdfBrush headerRowForeColor = new PdfSolidBrush(GridStyleInfo.HeaderRowForegroundColor);

            ////Sets Pdf Table summary style color
            PdfBrush summaryColumnBackColor = new PdfSolidBrush(GridStyleInfo.SummaryColumnBackgroundColor);//ToPdf(styleInfo.SummaryColumnBackgroundColor));
            PdfBrush summaryColumnForeColor = new PdfSolidBrush(GridStyleInfo.SummaryColumnForegroundColor);// ToPdf(styleInfo.SummaryColumnForegroundColor));

            PdfBrush summaryRowBackColor = new PdfSolidBrush(GridStyleInfo.SummaryRowBackgroundColor);// ToPdf(styleInfo.SummaryRowBackgroundColor));
            PdfBrush summaryRowForeColor = new PdfSolidBrush(GridStyleInfo.SummaryRowForegroundColor);//ToPdf(styleInfo.SummaryRowForegroundColor));

            PdfBrush cellValueColor = new PdfSolidBrush(GridStyleInfo.CellFontColor);
            PdfBrush gridLineBrush = new PdfSolidBrush(GridStyleInfo.GridlineColor);

            ////Sets format for Pdf Text and allignment.
            PdfStringFormat format = new PdfStringFormat();
            format.LineAlignment = PdfVerticalAlignment.Middle;

            for (int row = 0; row < Engine.RowsCount; row++)
            {
                PdfGridRow pdfRow = pdfGrid.Rows.Add();
                for (int cell = 0; cell < pdfRow.Cells.Count; cell++)
                {
                    PivotCellDescriptor cellDesc = Engine.TableColumns[cell].Cells[row];
                    PdfGridCellStyle cellStyle = new PdfGridCellStyle();

                    if (cellDesc.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, format);
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        cellStyle = ColumnHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, format);
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryRow)
                    {
                        if (cellDesc.SpanCell == null)
                        {
                            cellStyle = SummaryRowStyle(cellStyle, summaryRowBackColor, summaryRowForeColor, format);
                        }
                        else
                        {
                            PivotCellDescriptor parentCell = GetParentCell(cellDesc);
                            if (parentCell.CellType == PivotCellDescriptorType.RowHeader)
                            {
                                cellStyle = RowHeaderStyle(cellStyle, headerRowBackColor, headerRowForeColor, format);
                            }
                            else if (parentCell.CellType == PivotCellDescriptorType.SummaryRow)
                            {
                                cellStyle = SummaryRowStyle(cellStyle, summaryRowBackColor, summaryRowForeColor, format);
                            }
                        }
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.SummaryColumn)
                    {
                        if (cellDesc.SpanCell == null)
                        {
                            cellStyle = SummaryColumnStyle(cellStyle, summaryColumnBackColor, summaryColumnForeColor, format);
                        }
                        else
                        {
                            PivotCellDescriptor parentCell = GetParentCell(cellDesc);
                            if (parentCell.CellType == PivotCellDescriptorType.ColumnHeader)
                            {
                                cellStyle = ColumnHeaderStyle(cellStyle, headerColumnBackColor, headerColumnForeColor, format);
                            }
                            else if (parentCell.CellType == PivotCellDescriptorType.SummaryColumn)
                            {
                                cellStyle = SummaryColumnStyle(cellStyle, summaryColumnBackColor, summaryColumnForeColor, format);
                            }
                        }
                    }
                    else if (cellDesc.CellType == PivotCellDescriptorType.Value)
                    {
                        if (cellDesc.CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()))
                        {
                            cellStyle = SummaryRowStyle(cellStyle, summaryRowBackColor, summaryRowForeColor, format);
                        }
                        else if (cellDesc.CellExTypes.Contains(PivotCellDescriptorType.SummaryColumn.ToString()))
                        {
                            cellStyle = SummaryColumnStyle(cellStyle, summaryColumnBackColor, summaryColumnForeColor, format);
                        }
                        else if (cellDesc.CellExTypes.Count == 0)
                        {
                            cellStyle = ValueCellStyle(cellStyle, cellValueColor);
                        }
                    }

                    cellStyle.Borders.All = new PdfPen(gridLineBrush, .5f);
                    pdfRow.Cells[cell].Value = cellDesc.CellValue;
                    pdfRow.Cells[cell].Style = cellStyle;

                }
                pdfRow.Height = GetRowHeight(GridStyleInfo.CellFontSize, GridStyleInfo.HeaderFontSize, GridStyleInfo.SummaryFontSize) + 3;
            }

            pdfGrid.Style.AllowHorizontalOverflow = true;
            pdfGrid.Draw(page, PointF.Empty);
            document.Save(stream);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Cell style for Value Cell.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="cellValueColor">Color of the value cell.</param>
        /// <returns></returns>
        private PdfGridCellStyle ValueCellStyle(PdfGridCellStyle cellStyle, PdfBrush cellValueColor)
        {
            cellStyle.BackgroundBrush = new PdfSolidBrush(Colors.White);
            cellStyle.Font = GetFontInfo(GridStyleInfo.CellFontName, GridStyleInfo.CellFontStyle, GridStyleInfo.CellFontSize);
            cellStyle.StringFormat = new PdfStringFormat();
            cellStyle.StringFormat.Alignment = PdfTextAlignment.Center;
            cellStyle.TextBrush = cellValueColor;
            return cellStyle;
        }

        /// <summary>
        /// Cell style for Summary Column.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="summaryColumnBackColor">Backcolor of the summary column.</param>
        /// <param name="summaryColumnForeColor">ForeColor of the summary column.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfGridCellStyle SummaryColumnStyle(PdfGridCellStyle cellStyle, PdfBrush summaryColumnBackColor, PdfBrush summaryColumnForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryColumnBackColor;
            cellStyle.TextBrush = summaryColumnForeColor;
            cellStyle.Font = GetFontInfo(GridStyleInfo.SummaryFontName, "Normal", GridStyleInfo.HeaderFontSize);
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        /// <summary>
        /// Cell style for Summary Row.
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="summaryRowBackColor">BackColor of the summary row.</param>
        /// <param name="summaryRowForeColor">ForeColor of the summary row.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfGridCellStyle SummaryRowStyle(PdfGridCellStyle cellStyle, PdfBrush summaryRowBackColor, PdfBrush summaryRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = summaryRowBackColor;
            cellStyle.TextBrush = summaryRowForeColor;
            cellStyle.Font = GetFontInfo(GridStyleInfo.SummaryFontName, "Normal", GridStyleInfo.HeaderFontSize);
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        /// <summary>
        /// CellStyle for Column Header
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="headerColumnBackColor">BackColor of the header column .</param>
        /// <param name="headerColumnForeColor">ForeColor of the header column .</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfGridCellStyle ColumnHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerColumnBackColor, PdfBrush headerColumnForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerColumnBackColor;
            cellStyle.TextBrush = headerColumnForeColor;
            cellStyle.Font = GetFontInfo(GridStyleInfo.HeaderFontName, GridStyleInfo.HeaderFontStyle, GridStyleInfo.HeaderFontSize);
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        /// <summary>
        /// CellStyle for RowHeader
        /// </summary>
        /// <param name="cellStyle">The cell style.</param>
        /// <param name="headerRowBackColor">BackColor of the header row.</param>
        /// <param name="headerRowForeColor">ForeColor of the header row.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfGridCellStyle RowHeaderStyle(PdfGridCellStyle cellStyle, PdfBrush headerRowBackColor, PdfBrush headerRowForeColor, PdfStringFormat format)
        {
            cellStyle.BackgroundBrush = headerRowBackColor;
            cellStyle.TextBrush = headerRowForeColor;
            cellStyle.Font = GetFontInfo(GridStyleInfo.HeaderFontName, GridStyleInfo.HeaderFontStyle, GridStyleInfo.HeaderFontSize);
            cellStyle.StringFormat = format;
            return cellStyle;
        }

        /// <summary>
        /// Gets the parent cell.
        /// </summary>
        /// <param name="cellDesc">The child celldescriptor.</param>
        /// <returns></returns>
        private PivotCellDescriptor GetParentCell(PivotCellDescriptor cellDesc)
        {
        label:
            PivotCellDescriptor parentCell = cellDesc.SpanCell;
            if (parentCell.SpanCell == null)
            {
                return parentCell;
            }
            else
            {
                cellDesc = parentCell;
                goto label;
            }
        }

        /// <summary>
        /// Gets the height of the row.
        /// </summary>
        /// <param name="CellFontSize">Size of the cell font.</param>
        /// <param name="HeaderFontSize">Size of the header font.</param>
        /// <param name="SummaryFontSize">Size of the summary font.</param>
        /// <returns></returns>
        private float GetRowHeight(double CellFontSize, double HeaderFontSize, double SummaryFontSize)
        {
            List<double> height = new List<double>();
            height.Add(CellFontSize);
            height.Add(HeaderFontSize);
            height.Add(SummaryFontSize);
            height.Sort();
            return float.Parse(height[2].ToString());
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
            }
            return null;
        }

        #endregion
    }    
}
