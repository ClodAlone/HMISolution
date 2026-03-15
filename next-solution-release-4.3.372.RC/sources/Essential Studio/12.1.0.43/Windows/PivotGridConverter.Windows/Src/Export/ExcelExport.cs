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
using Color = System.Drawing.Color;
using System.Collections;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using Syncfusion.Windows.Forms.Collections;
using System.Windows;
using Syncfusion.Collections;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.PivotAnalysis;

namespace Syncfusion.PivotGridConverter
{
    /// <summary>
    /// GridExcelExport exports the Pivot data to Excel sheet with the applied style
    /// </summary>
    public class ExcelExport
    {
        #region [ Private Members ]

        private ExcelEngine m_ExcelEngine;
        private DataTable dt;
        private PivotSubtotalTypes subTotalType;
        private IWorksheet pivotSheet;
        private int rowCount = 1;
        private int colCount = 1;
        private List<IPivotFieldItem> itemsSet;

        #endregion

        private PivotGridControl[] GridCollection
        {
        get;
            set;
        }

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
        /// Declare Workbook to be added in Excel  file
        /// </summary>
        public IWorkbook workBook { get; set; }

        /// <summary>
        /// Declare sheets to be added in workBook . 
        /// </summary>
        public IWorksheet sheet { get; set; }

        /// <summary>
        /// Gets the grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl gridcontrol1 { get; internal set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public ExcelExport(PivotGridControl gridControl)
            : this(gridControl, ExcelVersion.Excel97to2003)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        /// <param name="version"> Excel Version.<remarks> Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        public ExcelExport(PivotGridControl gridControl, ExcelVersion version)
            : this(gridControl, version, ExportModes.Cell)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControl">PivotGridControl</param>
        /// <param name="mode">Export Mode</param>
        public ExcelExport(PivotGridControl gridControl, ExportModes mode)
            : this(new PivotGridControl[] { gridControl }, ExcelVersion.Excel97to2003, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControl">PivotGridControl</param>
        /// <param name="version">Excel Version.<remarks> Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>
        public ExcelExport(PivotGridControl gridControl, ExcelVersion version, ExportModes mode)
            : this(new PivotGridControl[] { gridControl }, version, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControls">Array of PivotGridControl.</param>
        /// <param name="version">Excel Version.<remarks>Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>        
        public ExcelExport(PivotGridControl[] gridControls, ExcelVersion version, ExportModes mode)
        {
            if (gridControls == null || gridControls.Length <= 0)
                throw new ArgumentNullException("gridControls", "PivotGridControl array might be null or its length might be zero");
            this.dt = new DataTable();
            this.application = excelEngine.Excel;
            this.application.DefaultVersion = version;
            this.ExportMode = mode;
            this.subTotalType = PivotSubtotalTypes.Count;
            this.workBook = this.ExportMode == ExportModes.Cell ? application.Workbooks.Create(gridControls.Length) : application.Workbooks.Create(2 * gridControls.Length);
            this.GridCollection = gridControls;
        }

        /// <summary>
        /// Gets or sets the mode(Cell/PivotTable) to export.
        /// </summary>
        /// <value>ExportMode</value>        
        public ExportModes ExportMode
        {
            get;
            set;
        }

        /// <summary>
        /// Exports the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Export(string fileName)
        {
            if (this.ExportMode == ExportModes.PivotTable && ((2 * this.GridCollection.Length) != this.workBook.Worksheets.Count))
            {
                this.workBook = this.application.Workbooks.Create(2 * GridCollection.Length);
            }

            for (int index = 0, count = 0; index < this.GridCollection.Length; index++, count++)
            {
                this.gridcontrol1 = this.GridCollection[index];
                this.sheet = workBook.Worksheets[count];
                sheet.IsGridLinesVisible = true;

                if (this.ExportMode == ExportModes.PivotTable)
                {
                    this.pivotSheet = workBook.Worksheets[++count];
                    this.pivotSheet.IsGridLinesVisible = true;
                }
                rowCount = this.gridcontrol1.PivotEngine.RowCount;
                colCount = this.gridcontrol1.PivotEngine.ColumnCount;

                if (!this.gridcontrol1.PivotEngine.ShowGrandTotals)
                {
                    rowCount -= 1;
                    colCount -= this.gridcontrol1.PivotCalculations.Count > 1 ? this.gridcontrol1.PivotCalculations.Count - 1 : 1;
                }
                if (this.ExportMode == ExportModes.Cell)
                {
                    for (int i = 1; i <= rowCount-1; i++)
                    {
                        for (int j = 1; j <= colCount-1; j++)
                        {
                            this.sheet.Range[i, j].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                            this.sheet.Range[i, j].BorderAround(ExcelLineStyle.Thin);
                            this.sheet.Range[i, j].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                            PivotCellInfo cellInfo = this.gridcontrol1.PivotEngine[i - 1, j - 1];

                            if (this.gridcontrol1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Metro)
                            {
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
                                            this.sheet.Range[i, j].CellStyle.IncludeAlignment = true;
                                            this.sheet.Range[i, j].HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                            this.sheet.Range[i, j].Merge();
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#2abff1");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#2abff1");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }                                    

                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }


                                    this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                                    {                                       
                                        if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[i, j].Number = val;
                                            }
                                            else
                                                val = 0;
                                            int decimalPlaces = 0, result;
                                            string formatString;
                                            if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                                            {
                                                string valString = val.ToString("C", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.CurrencyDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                if (Char.IsNumber(valString, 0))
                                                    formatString += " [$" + nfi.CurrencySymbol + "-411]";
                                                else
                                                    formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                                            {
                                                string valString = val.ToString("P", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.PercentDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += nfi.PercentSymbol;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                                            {
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = 6;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += "E+000";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                                            {
                                                string valString = val.ToString("N", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.NumberDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else
                                            {
                                                sheet.Range[i, j].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[i, j].NumberFormat = cellInfo.Format;
                                                sheet.Range[i, j].Number = double.Parse(cellInfo.Value.ToString());
                                            }
                                        }
                                        else
                                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                                    }
                                }
                            }   
                            else if (this.gridcontrol1.GridVisualStyles  == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue||this.gridcontrol1.GridVisualStyles==Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue)
                            {
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
                                            this.sheet.Range[i, j].Merge();
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#deeaf6");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#bc9d9e");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#deeaf6");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#af9e9e");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {   
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#9f9a8f");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)||
                                              cellInfo.CellType==( PivotCellType.CalculationHeaderCell ))
                                    {                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#9f9a8f");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#8aaa8f");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                                    {                                        
                                        if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[i, j].Number = val;
                                            }
                                            else
                                                val = 0;
                                            int decimalPlaces = 0, result;
                                            string formatString;

                                            if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                                            {
                                                string valString = val.ToString("C", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.CurrencyDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                if (Char.IsNumber(valString, 0))
                                                    formatString += " [$" + nfi.CurrencySymbol + "-411]";
                                                else
                                                    formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                                            {
                                                string valString = val.ToString("P", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.PercentDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += nfi.PercentSymbol;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                                            {
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = 6;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += "E+000";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                                            {
                                                string valString = val.ToString("N", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.NumberDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else
                                            {
                                                sheet.Range[i, j].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[i, j].NumberFormat = cellInfo.Format;
                                                sheet.Range[i, j].Number = double.Parse(cellInfo.Value.ToString());
                                            }
                                        }
                                        else
                                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                                    }
                                }
                            }
                            
                            if (this.gridcontrol1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver||this.gridcontrol1.GridVisualStyles==Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver )
                            {
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
                                        this.sheet.Range[i, j].CellStyle.IncludeAlignment = true;
                                            this.sheet.Range[i, j].Merge();
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#dedfe0");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#8a8aac");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#dfe0e1");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#dfe0e1");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#8a8a8a");
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#8a8a8a");
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                                    {                                        
                                        if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[i, j].Number = val;
                                            }
                                            else
                                                val = 0;
                                            int decimalPlaces = 0, result;
                                            string formatString;
                                            if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                                            {
                                                string valString = val.ToString("C", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.CurrencyDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                if (Char.IsNumber(valString, 0))
                                                    formatString += " [$" + nfi.CurrencySymbol + "-411]";
                                                else
                                                    formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                                            {
                                                string valString = val.ToString("P", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.PercentDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += nfi.PercentSymbol;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                                            {
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = 6;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += "E+000";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                                            {
                                                string valString = val.ToString("N", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.NumberDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;

                                            }
                                            else
                                            {
                                                sheet.Range[i, j].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[i, j].NumberFormat = cellInfo.Format;
                                                sheet.Range[i, j].Number = double.Parse(cellInfo.Value.ToString());
                                            }
                                        }
                                        else
                                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                                    }
                                }
                            }
                            if (this.gridcontrol1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black)
                            {
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
                                            this.sheet.Range[i, j].CellStyle.IncludeAlignment = true;
                                            this.sheet.Range[i, j].Merge();
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#e8e8e8");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#e8e8e8");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                     }

                                    this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                                    {                                        
                                        if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[i, j].Number = val;
                                            }
                                            else
                                                val = 0;
                                            int decimalPlaces = 0, result;
                                            string formatString;
                                            if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                                            {
                                                string valString = val.ToString("C", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.CurrencyDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                if (Char.IsNumber(valString, 0))
                                                    formatString += " [$" + nfi.CurrencySymbol + "-411]";
                                                else
                                                    formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                                            {
                                                string valString = val.ToString("P", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.PercentDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += nfi.PercentSymbol;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                                            {
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = 6;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += "E+000";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                                            {
                                                string valString = val.ToString("N", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.NumberDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;

                                            }
                                            else
                                            {
                                                sheet.Range[i, j].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[i, j].NumberFormat = cellInfo.Format;
                                                sheet.Range[i, j].Number = double.Parse(cellInfo.Value.ToString());
                                            }
                                        }
                                        else
                                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                                    }
                                }
                            }

                            if (this.gridcontrol1.GridVisualStyles == Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black)
                            {
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
                                        this.sheet.Range[i, j].CellStyle.IncludeAlignment = true;
                                        this.sheet.Range[i, j].Merge();
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#666666");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#636363");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.White;
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {
                                        this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#b38a8d");
                                        this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                        this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                        this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }
                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                                    {
                                            this.sheet.Range[i, j].CellStyle.Color = HexToColor("#fbe292");
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = HexToColor("#b38a8d");
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {                                            this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Gray;
                                            this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                            this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                            this.sheet.Range[i, j].ColumnWidth = 20f;
                                    }

                                    this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if (cellInfo.ParentCell == null && cellInfo.FormattedText != null)
                                    {
                                       if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[i, j].Number = val;
                                            }
                                            else
                                                val = 0;
                                            int decimalPlaces = 0, result;
                                            string formatString;
                                            if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                                            {
                                                string valString = val.ToString("C", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.CurrencyDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                if (Char.IsNumber(valString, 0))
                                                    formatString += " [$" + nfi.CurrencySymbol + "-411]";
                                                else
                                                    formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                                            {
                                                string valString = val.ToString("P", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.PercentDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += nfi.PercentSymbol;
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                                            {
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = 6;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                formatString += "E+000";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;
                                            }
                                            else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                                            {
                                                string valString = val.ToString("N", nfi);
                                                if (cellInfo.Format.Length == 1)
                                                    decimalPlaces = nfi.NumberDecimalDigits;
                                                else
                                                {
                                                    if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                                        decimalPlaces = result;
                                                }
                                                formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                                                for (int digits = 0; digits < decimalPlaces; digits++)
                                                    formatString += "0";
                                                this.sheet.Range[i, j].CellStyle.NumberFormat = formatString;

                                            }
                                            else
                                            {
                                                sheet.Range[i, j].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[i, j].NumberFormat = cellInfo.Format;
                                                sheet.Range[i, j].Number = double.Parse(cellInfo.Value.ToString());
                                            }
                                        }
                                        else
                                            this.sheet.Range[i, j].Text = cellInfo.FormattedText;
                                    }
                                }
                            }
                        } 
                        for (int k = 1; k <= colCount; k++)
                        {
                            this.sheet.AutofitColumn(k);
                        }
                    }
                }
                else
                {
                    if (this.gridcontrol1.ItemSource is DataView)
                    {
                        dt = (this.gridcontrol1.ItemSource as DataView).Table;
                    }
                    else if (this.gridcontrol1.ItemSource is DataTable)
                    {
                        dt = this.gridcontrol1.ItemSource as DataTable;
                    }
                    else if (this.gridcontrol1.ItemSource is IEnumerable)
                    {
                        IList list = this.gridcontrol1.ItemSource as IList;
                        PropertyDescriptorCollection propCol = ListUtil.GetItemProperties(list);
                        dt = new DataTable(propCol[0].ComponentType.Name);
                        for (int i = 0; i < propCol.Count; i++)
                        {
                            Type type = propCol[i].PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                type = Nullable.GetUnderlyingType(type);
                            DataColumn col = new DataColumn(propCol[i].Name, type);
                            dt.Columns.Add(col);
                        }
                       
                        foreach (object item in list)
                        {
                            DataRow row = dt.NewRow();
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (col.Expression != string.Empty)
                                    continue;
                                object obj = propCol[col.ColumnName].GetValue(item);
                                if (obj == null)
                                    row[col.ColumnName] = DBNull.Value;
                                else
                                    row[col.ColumnName] = obj;
                            }
                            dt.Rows.Add(row);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Export is not supported for this datasource type in PivotTable mode!");
                        return;
                    }
                    sheet.ImportDataTable(dt, true, 1, 1, -1, -1);
                    IPivotCache cache = this.workBook.PivotCaches.Add(sheet["A1:" + GridRangeInfo.GetAlphaLabel(dt.Columns.Count) + (dt.Rows.Count + 1)]);
                    IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A1"], cache);

                    foreach (PivotItem item in this.gridcontrol1.PivotColumns)
                    {
                        IPivotField field = pivotTable.Fields[item.FieldMappingName];
                        field.Axis = PivotAxisTypes.Column;
                        if (item.Format != null)
                        {
                            this.ApplyNumberFormat(item, field);
                        }
                        field.CanDragToData = field.CanDragToRow = field.CanDragToColumn = item.AllowRunTimeGroupByField;
                        ApplyFilteredView(field);
                    }

                    foreach (PivotItem item in this.gridcontrol1.PivotRows)
                    {
                        IPivotField field = pivotTable.Fields[item.FieldMappingName];
                        field.Axis = PivotAxisTypes.Row;
                        if (item.Format != null)
                        {
                            this.ApplyNumberFormat(item, field);
                        }
                        field.CanDragToColumn = field.CanDragToData = field.CanDragToRow = item.AllowRunTimeGroupByField;
                        ApplyFilteredView(field);
                    }

                    foreach (PivotComputationInfo item in this.gridcontrol1.PivotCalculations)
                    {
                        IPivotField field = pivotTable.Fields[item.FieldName];
                        if (item.Format != null)
                        {
                            this.ApplyNumberFormat(item, field);
                        }
                        field.CanDragToColumn = field.CanDragToRow = field.CanDragToData = item.AllowRunTimeGroupByField;
                        if (field != null)
                        {
                            pivotTable.DataFields.Add(field, item.FieldName, GetSubTotalType(item.SummaryType));
                            ApplyFilteredView(field);
                        }
                    }

                    pivotTable.ShowColumnGrand = pivotTable.ShowRowGrand = this.gridcontrol1.ShowGrandTotals;
                    pivotTable.Options.DisplayErrorString = true;
                    if (this.gridcontrol1.PivotEngine.ShowNullAsBlank)
                        pivotTable.Options.NullString = pivotTable.Options.ErrorString = string.Empty;
                    else
                        pivotTable.Options.NullString = pivotTable.Options.ErrorString = "0";

                    pivotTable.ShowDataFieldInRow = !this.gridcontrol1.ShowCalculationsAsColumns;
                    pivotTable.Options.IsSaveData = true;
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                    switch (this.gridcontrol1.GridVisualStyles)
                    {
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue:
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2010Blue:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight20;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2007Black:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark1;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2007Silver:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight15;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2010Black:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark8;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2010Silver:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium1;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Metro:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium13;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Office2003:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight16;
                            break;
                        case Syncfusion.Windows.Forms.GridVisualStyles.Custom:
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium9;
                            break;
                    }
                    pivotSheet.Activate();
                     
                }
            }
            this.workBook.SaveAs(fileName, ExcelSaveType.SaveAsXLS);
            workBook.Close();
            excelEngine.Dispose();
        }

        /// <summary>
        /// Apply filtered view to respective pivot field.
        /// </summary>
        /// <param name="field">PivotField to which filtered view to apply.</param>
        private void ApplyFilteredView(IPivotField field)
        {
            itemsSet = new List<IPivotFieldItem>();
            for (int i = 0; i < this.gridcontrol1.Filters.Count; i++)
            {
                if (this.gridcontrol1.Filters[i].Name.Equals(field.Name))
                {
                    FilterItemsCollection items = this.gridcontrol1.Filters[i].Tag as FilterItemsCollection;
                    if (items.AllFilterItem.IsSelected != null && (bool)items.AllFilterItem.IsSelected)
                    {
                        for (int j = 0; j < field.Items.Count; j++)
                            field.Items[j].Visible = true;
                    }
                    else
                    {
                        for (int j = 0; j < items.Count; j++)
                        {
                            if (items[j].Key == "(All)")
                                continue;
                            if (items[j].IsSelected == null)
                                field.Items[items[j].Key].Visible = false;
                            else
                                field.Items[items[j].Key].Visible = (bool)items[j].IsSelected;
                            itemsSet.Add(field.Items[items[j].Key]);
                        }
                        for (int k = 0; k < field.Items.Count; k++)
                        {
                            if (!itemsSet.Contains(field.Items[k]))
                                field.Items[k].Visible = false;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Applies the number format to pivot fields.
        /// </summary>
        /// <param name="item">An object of type PivotItem or PivotComputationInfo</param>
        /// <param name="field">Pivot Field</param>        
       public  void ApplyNumberFormat(object item, IPivotField field)
        {
            int decimalPlaces = 0, result;
            string formatString;
            if (item != null)
            {
                System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                if (item is PivotItem)
                {
                    PivotItem cellInfo = item as PivotItem;
                    if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.CurrencyDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.PercentDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString += nfi.PercentSymbol;
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = 6;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString += "E+000";
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.NumberDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        field.NumberFormat = formatString;
                    }
                    else
                    {
                        field.NumberFormat = cellInfo.Format;
                    }
                }
                else if (item is PivotComputationInfo)
                {
                    PivotComputationInfo cellInfo = item as PivotComputationInfo;
                    if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'C')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.CurrencyDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'P')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.PercentDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString += nfi.PercentSymbol;
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'E')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = 6;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        formatString += "E+000";
                        field.NumberFormat = formatString;
                    }
                    else if ((char.ToUpper(cellInfo.Format.ElementAt(0))) == 'N')
                    {
                        if (cellInfo.Format.Length == 1)
                            decimalPlaces = nfi.NumberDecimalDigits;
                        else
                        {
                            if (int.TryParse(cellInfo.Format.TrimStart(cellInfo.Format.ElementAt(0)).ToString(), out result))
                                decimalPlaces = result;
                        }
                        formatString = "#,##0" + ((decimalPlaces != 0) ? "." : string.Empty);
                        for (int digits = 0; digits < decimalPlaces; digits++)
                            formatString += "0";
                        field.NumberFormat = formatString;
                    }
                    else
                    {
                        field.NumberFormat = cellInfo.Format;
                    }
                }
            }
        }

        /// <summary>
        /// Converts the summary type of PivotGrid to respective type supported by XlsIO.
        /// </summary>
        /// <param name="summaryType">SummaryType</param>
        /// <returns>PivotSubtotalType</returns>
        private PivotSubtotalTypes GetSubTotalType(SummaryType summaryType)
        {
            if (summaryType == SummaryType.Count)
                subTotalType = PivotSubtotalTypes.Count;
            else if (summaryType == SummaryType.DecimalTotalSum || summaryType == SummaryType.DoubleTotalSum || summaryType == SummaryType.IntTotalSum)
                subTotalType = PivotSubtotalTypes.Sum;
            else if (summaryType == SummaryType.DoubleMaximum)
                subTotalType = PivotSubtotalTypes.Max;
            else if (summaryType == SummaryType.DoubleMinimum)
                subTotalType = PivotSubtotalTypes.Min;
            else if (summaryType == SummaryType.DoubleVariance)
                subTotalType = PivotSubtotalTypes.Var;
            else if (summaryType == SummaryType.DoubleStandardDeviation)
                subTotalType = PivotSubtotalTypes.Stdev;
            else if (summaryType == SummaryType.DoubleAverage)
                subTotalType = PivotSubtotalTypes.Average;
            return subTotalType;
        }

        /// <summary>
        /// Apply styles to rows and column for Excel sheet.
        /// </summary>
        private void FormatExcel()
        {
            for (int i = 1; i <= this.gridcontrol1.PivotEngine.RowCount; i++)
            {
                for (int j = 1; j <= this.gridcontrol1.PivotEngine.ColumnCount; j++)
                {
                    
                    PivotCellInfo cellInfo = this.gridcontrol1.PivotEngine[i - 1, j - 1];

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
                            this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                        }

                        else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell))
                        {                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
                        }

                        else if (cellInfo.CellType == PivotCellType.ValueCell)
                        {   
                                this.sheet.Range[i, j].CellStyle.Color = Color.White;
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = Color.Black;
                                this.sheet.Range[i, j].CellStyle.Font.Size = 10f;
                                this.sheet.Range[i, j].CellStyle.Font.FontName = "TimesRoman";
                                this.sheet.Range[i, j].ColumnWidth = 20f;
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
        /// Gets the color for the given hexadecimal value
        /// </summary>
        /// <param name="hexVal"></param>
        /// <returns>color</returns>
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
                    return Color.FromArgb(Convert.ToInt32(hRed, 16), Convert.ToInt32(hGreen, 16), Convert.ToInt32(hBlue, 16));
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
    /// <summary>
    /// Export Modes
    /// </summary>
    /// <remarks>To export the PivotGrid either cell wise or as PivotTable.</remarks>
    public enum ExportModes
    {
        /// <summary>
        /// Denotes the cell type export of pivot grid
        /// </summary>
        Cell,
        /// <summary>
        /// Denotes the pivot type export of pivot grid
        /// </summary>
        PivotTable
    }
}

