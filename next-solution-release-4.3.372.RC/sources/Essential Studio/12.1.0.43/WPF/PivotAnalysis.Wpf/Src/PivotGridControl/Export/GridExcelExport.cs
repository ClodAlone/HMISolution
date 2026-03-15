#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.XlsIO;
using Syncfusion.PivotAnalysis.Base;
using System.Drawing;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// This class is used for Exporting PivotGrid data to Excel using XlsIO library
    /// </summary>
    public class GridExcelExport
    {
        #region [ Private Members ]

        private ExcelEngine m_ExcelEngine;

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// Declare IAplication to instantiate variouss kinds applications  
        /// </summary>
        public IApplication application { get; set; }

        /// <summary>
        /// Create instance for Excel Engine
        /// </summary>
        public ExcelEngine excelEngine
        {
            get
            {
                if (m_ExcelEngine == null)
                {
                    return m_ExcelEngine = new ExcelEngine();
                }
                else
                    return m_ExcelEngine;
            }
            set
            {
                m_ExcelEngine = value;
            }
        }

        /// <summary>
        /// Declare Woorkbook to be added in Excel  file
        /// </summary>
        public IWorkbook workBook { get; set; }

        /// <summary>
        /// Declare sheets to be added in workBook . 
        /// </summary>
        public IWorksheet sheet { get; set; }

        /// <summary>
        /// Gets or sets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl GridControl { get; internal set; }

        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public GridExcelExport(PivotGridControl gridControl)
        {
            this.GridControl = gridControl;
            this.application = excelEngine.Excel;
            this.workBook = application.Workbooks.Create(5);
            this.sheet = workBook.Worksheets[0];
            sheet.IsGridLinesVisible = true;
        }

        /// <summary>
        /// Exports the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Export(string fileName)
        {
            for (int i = 1; i <= this.GridControl.PivotEngine.RowCount; i++)
            {
                for (int j = 1; j <= this.GridControl.PivotEngine.ColumnCount; j++)
                {
                    this.sheet.Range[i, j].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    this.sheet.Range[i, j].BorderAround(ExcelLineStyle.Thin);
                    this.sheet.Range[i, j].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                    PivotCellInfo cellInfo = this.GridControl.PivotEngine[i - 1, j - 1];
                    if (cellInfo != null)
                    {
                        if (cellInfo.CellRange != null)
                        {
                            this.sheet.Range[cellInfo.CellRange.Top + 1, cellInfo.CellRange.Left + 1, cellInfo.CellRange.Bottom + 1, cellInfo.CellRange.Right + 1].Merge();
                        }

                        if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (this.GridControl.ColumnHeaderCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                                //ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.ColumnHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (this.GridControl.RowHeaderCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.RowHeaderCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.RowHeaderCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.RowHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                        {
                            if (this.GridControl.SummaryHeaderStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryHeaderStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.SummaryHeaderStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryHeaderStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                        {
                            if (this.GridControl.SummaryCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.SummaryCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == PivotCellType.ValueCell)
                        {
                            if (this.GridControl.ValueCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ValueCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.ValueCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.ValueCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }
                        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                        if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                        {
                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                        }
                    }
                }
            }
            
            ////Apply styles to rows and column for Excel sheet .
            //this.FormatExcel();
            this.sheet.GridLineColor = ExcelKnownColors.Black;
            this.workBook.SaveAs(fileName, ExcelSaveType.SaveAsXLS);
            workBook.Close();
            excelEngine.Dispose();
        }

        /// <summary>
        /// Apply styles to rows and column for Excel sheet .
        /// </summary>
        private void FormatExcel()
        {
            for (int i = 1; i <= this.GridControl.PivotEngine.RowCount; i++)
            {
                for (int j = 1; j <= this.GridControl.PivotEngine.ColumnCount; j++)
                {
                    #region OldBackup
                    
                    //PivotCellInfo cellInfo = this.GridControl.PivotEngine[i - 1, j - 1];
                    //if (cellInfo != null)
                    //{
                    //    if(cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                    //       !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                    //       !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                    //    {
                    //        this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                    //        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(this.GridControl.ColumnHeaderCellStyle.Foreground.ToString());
                    //        this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                    //        this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                    //        this.sheet.Range[i, j].ColumnWidth = this.GridControl.ColumnHeaderCellStyle.FontSize + 10f;
                    //        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    //    }

                    //    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                    //             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                    //             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                    //    {
                    //        this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Background));
                    //        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(this.GridControl.RowHeaderCellStyle.Foreground.ToString());
                    //        this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.RowHeaderCellStyle.FontSize;
                    //        this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.RowHeaderCellStyle.FontFamily.ToString();
                    //        this.sheet.Range[i, j].ColumnWidth = this.GridControl.RowHeaderCellStyle.FontSize + 10f;
                    //        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    //    }

                    //    else if(cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                    //            cellInfo.CellType == (PivotCellType.TotalCell|  PivotCellType.ColumnHeaderCell) || 
                    //            cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) || 
                    //            cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell))
                    //    {
                    //        this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Background));
                    //        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(this.GridControl.SummaryHeaderStyle.Foreground.ToString());
                    //        this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryHeaderStyle.FontSize;
                    //        this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.SummaryHeaderStyle.FontFamily.ToString();
                    //        this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryHeaderStyle.FontSize + 10f;
                    //        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    //    }

                    //    else if(cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || 
                    //        cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                    //    {
                    //        this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.SummaryCellStyle.Background));
                    //        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(this.GridControl.SummaryCellStyle.Foreground.ToString());
                    //        this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryCellStyle.FontSize;
                    //        this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.SummaryCellStyle.FontFamily.ToString();
                    //        this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryCellStyle.FontSize + 10f;
                    //        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    //    }

                    //    else if (cellInfo.CellType == PivotCellType.ValueCell)
                    //    {
                    //        this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.ValueCellStyle.Background));
                    //        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(this.GridControl.ValueCellStyle.Foreground.ToString());
                    //        this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ValueCellStyle.FontSize;
                    //        this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.ValueCellStyle.FontFamily.ToString();
                    //        this.sheet.Range[i, j].ColumnWidth = this.GridControl.ValueCellStyle.FontSize + 10f;
                    //        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    //    }
                    //}
                    #endregion
                    PivotCellInfo cellInfo = this.GridControl.PivotEngine[i - 1, j - 1];
                    if (cellInfo != null)
                    {
                        if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (cellInfo.CellRange != null)
                            {
                                this.sheet.Range[cellInfo.CellRange.Top, cellInfo.CellRange.Left, cellInfo.CellRange.Bottom, cellInfo.CellRange.Right].Merge();
                            }
                            if (this.GridControl.ColumnHeaderCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                                //ColorTranslator.FromHtml(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.ColumnHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (cellInfo.CellRange != null)
                            {
                                this.sheet.Range[cellInfo.CellRange.Top, cellInfo.CellRange.Left, cellInfo.CellRange.Bottom, cellInfo.CellRange.Right].Merge();
                            }
                            if (this.GridControl.RowHeaderCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.RowHeaderCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = this.GridControl.RowHeaderCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.RowHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell))
                        {
                            if (this.GridControl.SummaryHeaderStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryHeaderStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.SummaryHeaderStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryHeaderStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                        {
                            if (this.GridControl.SummaryCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.SummaryCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.SummaryCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.SummaryCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == PivotCellType.ValueCell)
                        {
                            if (this.GridControl.ValueCellStyle != null)
                            {
                                this.sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Background));
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Foreground));
                                this.sheet.Range[i, j].CellStyle.Font.Size = this.GridControl.ValueCellStyle.FontSize;
                                this.sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.GridControl.ValueCellStyle.FontFamily.ToString();
                                this.sheet.Range[i, j].ColumnWidth = this.GridControl.ValueCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }
                        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                        if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                        {
                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the color from gradient.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
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
    }
}
