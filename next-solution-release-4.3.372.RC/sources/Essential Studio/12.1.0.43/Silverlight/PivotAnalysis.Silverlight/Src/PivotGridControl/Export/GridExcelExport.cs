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
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.XlsIO;
using System.IO;

namespace Syncfusion.Silverlight.Controls.PivotGrid
{
    public class GridExcelExport
    {
        #region Members

        /// <summary>
        /// 
        /// </summary>
        private ExcelEngine _excelEngine;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="pivotGridControl">The pivot grid control.</param>
        public GridExcelExport(PivotGridControl pivotGridControl)
        {
            this.PivotGridControl = pivotGridControl;
            this.Application = this.ExcelEngine.Excel;
            this.WorkBook = this.Application.Workbooks.Create(5);
            this.Sheet = this.WorkBook.Worksheets[0];
            this.Sheet.IsGridLinesVisible = true;
        } 
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        public IApplication Application { get; set; }


        /// <summary>
        /// Gets or sets the excel engine.
        /// </summary>
        /// <value>The excel engine.</value>
        public ExcelEngine ExcelEngine
        {
            get
            {
                if (_excelEngine == null)
                {
                    return _excelEngine = new ExcelEngine();
                }
                    return _excelEngine;
            }
            set
            {
                _excelEngine = value;
            }
        }

        /// <summary>
        /// Gets or sets the work book.
        /// </summary>
        /// <value>The work book.</value>
        public IWorkbook WorkBook { get; set; }

        /// <summary>
        /// Gets or sets the Sheet.
        /// </summary>
        /// <value>The Sheet.</value>
        public IWorksheet Sheet { get; set; }

        /// <summary>
        /// Gets or sets the pivot grid control.
        /// </summary>
        /// <value>The pivot grid control.</value>
        public PivotGridControl PivotGridControl { get; internal set; } 
        #endregion

        #region Helper Methods

        /// <summary>
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            for (int i = 1; i <= this.PivotGridControl.PivotEngine.RowCount; i++)
            {
                for (int j = 1; j <= this.PivotGridControl.PivotEngine.ColumnCount; j++)
                {
                    this.Sheet.Range[i, j].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                    this.Sheet.Range[i, j].BorderAround(ExcelLineStyle.Thin);
                    this.Sheet.Range[i, j].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                    PivotCellInfo cellInfo = this.PivotGridControl.PivotEngine[i - 1, j - 1];
                    if (cellInfo != null)
                    {
                        if (cellInfo.CellRange != null)
                        {
                            this.Sheet.Range[cellInfo.CellRange.Top + 1, cellInfo.CellRange.Left + 1, cellInfo.CellRange.Bottom + 1, cellInfo.CellRange.Right + 1].Merge();
                        }

                        if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                            !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (this.PivotGridControl.ColumnHeaderCellStyle != null)
                            {
                                this.Sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Background));
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.ColumnHeaderCellStyle.Foreground));
                                this.Sheet.Range[i, j].CellStyle.Font.Size = this.PivotGridControl.ColumnHeaderCellStyle.FontSize;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = this.PivotGridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                                this.Sheet.Range[i, j].ColumnWidth = this.PivotGridControl.ColumnHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.Sheet.Range[i, j].CellStyle.Color = Colors.White;
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor = Colors.Black;
                                this.Sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.Sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                 !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                        {
                            if (this.PivotGridControl.RowHeaderCellStyle != null)
                            {
                                this.Sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Background));
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.RowHeaderCellStyle.Foreground));
                                this.Sheet.Range[i, j].CellStyle.Font.Size = this.PivotGridControl.RowHeaderCellStyle.FontSize;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = this.PivotGridControl.RowHeaderCellStyle.FontFamily.ToString();
                                this.Sheet.Range[i, j].ColumnWidth = this.PivotGridControl.RowHeaderCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.Sheet.Range[i, j].CellStyle.Color = Colors.White;
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor = Colors.Black;
                                this.Sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.Sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                        {
                            if (this.PivotGridControl.SummaryHeaderStyle != null)
                            {
                                this.Sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Background));
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.SummaryHeaderStyle.Foreground));
                                this.Sheet.Range[i, j].CellStyle.Font.Size = this.PivotGridControl.SummaryHeaderStyle.FontSize;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.PivotGridControl.SummaryHeaderStyle.FontFamily.ToString();
                                this.Sheet.Range[i, j].ColumnWidth = this.PivotGridControl.SummaryHeaderStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.Sheet.Range[i, j].CellStyle.Color = Colors.White;
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor = Colors.Black;
                                this.Sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.Sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                        {
                            if (this.PivotGridControl.SummaryCellStyle != null)
                            {
                                this.Sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Background));
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.SummaryCellStyle.Foreground));
                                this.Sheet.Range[i, j].CellStyle.Font.Size = this.PivotGridControl.SummaryCellStyle.FontSize;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.PivotGridControl.SummaryCellStyle.FontFamily.ToString();
                                this.Sheet.Range[i, j].ColumnWidth = this.PivotGridControl.SummaryCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.Sheet.Range[i, j].CellStyle.Color = Colors.White;
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor = Colors.Black;
                                this.Sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.Sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }

                        else if (cellInfo.CellType == PivotCellType.ValueCell)
                        {
                            if (this.PivotGridControl.ValueCellStyle != null)
                            {
                                this.Sheet.Range[i, j].CellStyle.Color =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Background));
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor =
                                    HexToColor(GetColorFromGradient(this.PivotGridControl.ValueCellStyle.Foreground));
                                this.Sheet.Range[i, j].CellStyle.Font.Size = this.PivotGridControl.ValueCellStyle.FontSize;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName =
                                    this.PivotGridControl.ValueCellStyle.FontFamily.ToString();
                                this.Sheet.Range[i, j].ColumnWidth = this.PivotGridControl.ValueCellStyle.FontSize + 10f;
                            }
                            else
                            {
                                this.Sheet.Range[i, j].CellStyle.Color = Colors.White;
                                this.Sheet.Range[i, j].CellStyle.Font.RGBColor = Colors.Black;
                                this.Sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.Sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.Sheet.Range[i, j].ColumnWidth = 20f;
                            }
                        }
                        this.Sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                        if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                        {
                            this.Sheet.Range[i, j].Text = cellInfo.FormattedText;
                        }
                    }
                }
            }

            ////Apply styles to rows and column for Excel Sheet .
            //this.FormatExcel();
            this.Sheet.GridLineColor = ExcelKnownColors.Black;
            this.WorkBook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
            this.WorkBook.Close();
            this.ExcelEngine.Dispose();
        }

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

        private Color HexToColor(string hexVal)
        {
            return Color.FromArgb(
                Convert.ToByte(hexVal.Substring(1, 2), 16),
                Convert.ToByte(hexVal.Substring(3, 2), 16),
                Convert.ToByte(hexVal.Substring(5, 2), 16),
                Convert.ToByte(hexVal.Substring(7, 2), 16));
        }

        #endregion
    }
}
