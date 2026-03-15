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
using Syncfusion.Windows.Controls.PivotGrid;
using System.Collections;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using Syncfusion.Windows.Collections;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Windows;

namespace Syncfusion.Windows.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridExcelExport exports the Pivot data to Excel sheet with the applied style
    /// </summary>
    public class GridExcelExport
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
        public PivotGridControl GridControl { get; internal set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        public GridExcelExport(PivotGridControl gridControl)
            : this(gridControl, ExcelVersion.Excel97to2003)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="gridControl">The grid control.</param>
        /// <param name="version"> Excel Version.<remarks> Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        public GridExcelExport(PivotGridControl gridControl, ExcelVersion version)
            :this(gridControl , version, ExportModes.Cell)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControl">PivotGridControl</param>
        /// <param name="mode">Export Mode</param>
        public GridExcelExport(PivotGridControl gridControl, ExportModes mode)
            : this(new PivotGridControl[] { gridControl }, ExcelVersion.Excel97to2003, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControl">PivotGridControl</param>
        /// <param name="version">Excel Version.<remarks> Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>
        public GridExcelExport(PivotGridControl gridControl, ExcelVersion version, ExportModes mode)
            : this(new PivotGridControl[] { gridControl }, version, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControls">Array of PivotGridControl.</param>
        /// <param name="version">Excel Version.<remarks>Accessible through adding Syncfusion.XlsIO.Base as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>        
        public GridExcelExport(PivotGridControl[] gridControls, ExcelVersion version, ExportModes mode)
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
        /// Event to handle the templated cell export.
        /// </summary>
        public event QueryExportCellInfoEventHandler QueryExportCellInfo;
        /// <summary>
        /// Event to handle updation of data source when Custom summary is used
        /// </summary>
        public event UpdateDataSourceEventHandler UpdateDataSource;

        void OnQueryExportCellInfo(ExportingToExcelEventArgs e)
        {
            if (QueryExportCellInfo != null)
            {
                QueryExportCellInfo(this, e);
            }
        }
        void OnUpdateDataSource(DataSourceUpdateEventArgs e)
        {
            if (UpdateDataSource != null)
            {
                UpdateDataSource(this,e);
            }
        }
        
        /// <summary>
        /// Raises the CurrentCellExported event which is raised 
        /// when the templated cell is exported to excel sheet
        /// </summary>
        public void RaiseCurrentCellExported(ExportingToExcelEventArgs e)
        {
            OnQueryExportCellInfo(e);
        }

        /// <summary>
        /// Raises the UpdateDataSource event which is raised when Custom summary column is exported to excel sheet
        /// </summary>
        /// <param name="e"> an event parameter </param>
        public void RaiseUpdateDataSourceForCustomSummaries(DataSourceUpdateEventArgs e)
        {
            OnUpdateDataSource(e);
        }
        /// <summary>
        /// Show/Hide the PivotRows and PivotColumn header in Exported excel document when the export mode is "Cell"
        /// </summary>

        public bool ShowRowColumnHeaders { get; set; }

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
            IStyle rowHeaderStyle = workBook.Styles.Add("RowHeaderStyle");
            IStyle colHeaderStyle = workBook.Styles.Add("ColumnHeaderStyle");
            IStyle summaryHeaderStyle = workBook.Styles.Add("SummaryHeaderStyle");
            IStyle summaryCellStyle = workBook.Styles.Add("SummaryCellStyle");
            IStyle valueCellStyle = workBook.Styles.Add("ValueCellStyle");
            if (this.ExportMode == ExportModes.Cell)
            {
                // Row Header Style
                rowHeaderStyle.BeginUpdate();
                rowHeaderStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                rowHeaderStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                rowHeaderStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                rowHeaderStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                rowHeaderStyle.EndUpdate();
                // Column Header Style
                colHeaderStyle.BeginUpdate();
                colHeaderStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                colHeaderStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                colHeaderStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                colHeaderStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                colHeaderStyle.EndUpdate();
                // Summary Header Style
                summaryHeaderStyle.BeginUpdate();
                summaryHeaderStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                summaryHeaderStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                summaryHeaderStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                summaryHeaderStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                summaryHeaderStyle.EndUpdate();
                // Summary Cell Style
                summaryCellStyle.BeginUpdate();
                summaryCellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                summaryCellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                summaryCellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                summaryCellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                summaryCellStyle.EndUpdate();
                // Value Style
                valueCellStyle.BeginUpdate();
                valueCellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
                valueCellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                valueCellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
                valueCellStyle.Borders[ExcelBordersIndex.EdgeRight].LineStyle = ExcelLineStyle.Thin;
                valueCellStyle.EndUpdate();
            }
            for (int index = 0, count = 0; index < this.GridCollection.Length; index++, count++)
            {
                this.GridControl = this.GridCollection[index];
                this.sheet = workBook.Worksheets[count];
                sheet.IsGridLinesVisible = true;
                if (this.ExportMode == ExportModes.PivotTable)
                {
                    this.pivotSheet = workBook.Worksheets[++count];
                    this.pivotSheet.IsGridLinesVisible = true;
                }
                rowCount = this.GridControl.PivotEngine.RowCount;
                colCount = this.GridControl.PivotEngine.ColumnCount;
                if (!this.GridControl.PivotEngine.ShowGrandTotals)
                {
                    rowCount -= 1;
                    colCount -= this.GridControl.PivotCalculations.Count > 1 ? this.GridControl.PivotCalculations.Count - 1 : 1;
                }
                if (this.GridControl.RowPivotsOnly)
                {
                    colCount -= 1; //no grandtotal on right...
                }
                if (this.ExportMode == ExportModes.Cell)
                {
                    int dummy = 0;
                    int hiddenKeyLoc = this.GridControl.PivotEngine.GetHiddenRowKeyValueColumnIndex();
                    if (this.GridControl.RowHeaderCellStyle != null)
                    {
                        rowHeaderStyle.Color = HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Background));
                        rowHeaderStyle.Font.RGBColor = HexToColor(GetColorFromGradient(this.GridControl.RowHeaderCellStyle.Foreground));
                        rowHeaderStyle.Font.Size = this.GridControl.RowHeaderCellStyle.FontSize;
                        rowHeaderStyle.Font.FontName = this.GridControl.RowHeaderCellStyle.FontFamily.ToString();
                    }
                    else
                    {
                        rowHeaderStyle.Color = Color.White;
                        rowHeaderStyle.Font.RGBColor = Color.Black;
                        rowHeaderStyle.Font.Size = 10f;
                        rowHeaderStyle.Font.FontName = "TimesRoman";
                    }
                    if (this.GridControl.ColumnHeaderCellStyle != null)
                    {
                        colHeaderStyle.Color = HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Background));
                        colHeaderStyle.Font.RGBColor = HexToColor(GetColorFromGradient(this.GridControl.ColumnHeaderCellStyle.Foreground));
                        colHeaderStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                        colHeaderStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                    }
                    else
                    {
                        colHeaderStyle.Color = Color.White;
                        colHeaderStyle.Font.RGBColor = Color.Black;
                        colHeaderStyle.Font.Size = 10f;
                        colHeaderStyle.Font.FontName = "TimesRoman";
                    }
                    if (this.GridControl.SummaryHeaderStyle != null)
                    {
                        summaryHeaderStyle.Color = HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Background));
                        summaryHeaderStyle.Font.RGBColor = HexToColor(GetColorFromGradient(this.GridControl.SummaryHeaderStyle.Foreground));
                        summaryHeaderStyle.Font.Size = this.GridControl.SummaryHeaderStyle.FontSize;
                        summaryHeaderStyle.Font.FontName = this.GridControl.SummaryHeaderStyle.FontFamily.ToString();
                    }
                    else
                    {
                        summaryHeaderStyle.Color = Color.White;
                        summaryHeaderStyle.Font.RGBColor = Color.Black;
                        summaryHeaderStyle.Font.Size = 10f;
                        summaryHeaderStyle.Font.FontName = "TimesRoman";
                    }
                    if (this.GridControl.SummaryCellStyle != null)
                    {
                        summaryCellStyle.Color = HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Background));
                        summaryCellStyle.Font.RGBColor = HexToColor(GetColorFromGradient(this.GridControl.SummaryCellStyle.Foreground));
                        summaryCellStyle.Font.Size = this.GridControl.SummaryCellStyle.FontSize;
                        summaryCellStyle.Font.FontName = this.GridControl.SummaryCellStyle.FontFamily.ToString();
                    }
                    else
                    {
                        summaryCellStyle.Color = Color.White;
                        summaryCellStyle.Font.RGBColor = Color.Black;
                        summaryCellStyle.Font.Size = 10f;
                        summaryCellStyle.Font.FontName = "TimesRoman";
                    }
                    if (this.GridControl.ValueCellStyle != null)
                    {
                        valueCellStyle.Color = HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Background));
                        valueCellStyle.Font.RGBColor = HexToColor(GetColorFromGradient(this.GridControl.ValueCellStyle.Foreground));
                        valueCellStyle.Font.Size = this.GridControl.ValueCellStyle.FontSize;
                        valueCellStyle.Font.FontName = this.GridControl.ValueCellStyle.FontFamily.ToString();
                    }
                    int[] hiddenCount = new int[this.GridControl.PivotRows.Count];
                    PivotCellInfo[] hiddenCellInfo = new PivotCellInfo[this.GridControl.PivotRows.Count];

                    int rowIndex = 1;
                    HashSet<int> numericRowPivots = new HashSet<int>();
                    if (this.GridControl.RowPivotsOnly && this.GridControl.PivotEngine.ItemProperties != null)
                    {
                        for (int i = 0; i < this.GridControl.PivotRows.Count; ++i)
                        {
                            PropertyDescriptor pd = this.GridControl.PivotEngine.ItemProperties[this.GridControl.PivotRows[i].FieldMappingName];
                            Type t = pd.PropertyType;
                            if (t == typeof(double) || t == typeof(int) || t == typeof(long) || t == typeof(float))
                            {
                                numericRowPivots.Add(i);
                            }
                        }
                    }
                    for (int i = 1; i < rowCount; i++)
                    {
                        for (int k = 0; k < this.GridControl.PivotRows.Count; ++k)
                        {
                            hiddenCount[k] = 0;
                        }

                        if (!(this.GridControl.RowPivotsOnly && this.GridControl.PivotEngine.HiddenRowIndexes != null && this.GridControl.PivotEngine.HiddenRowIndexes.Contains(this.GridControl.PivotEngine[i - 1, hiddenKeyLoc])))
                        {
                            for (int j = 1, colIndex = 1; j <= colCount; j++, colIndex++)
                            {
                                if (this.GridControl.RowPivotsOnly && this.GridControl.InternalGrid.Model.ColumnWidths.GetHidden(j - 1, out dummy))
                                {
                                    colIndex--;
                                    continue;
                                }
                                PivotCellInfo cellInfo = this.GridControl.PivotEngine[i - 1, j - 1];
                                if (this.GridControl.RowPivotsOnly && j <= this.GridControl.PivotRows.Count)
                                {
                                    if (hiddenCellInfo[j - 1] != null)
                                    {
                                        cellInfo = hiddenCellInfo[j - 1];
                                        hiddenCellInfo[j - 1] = null;
                                    }
                                    hiddenCount[j - 1] = GetHiddenCount(cellInfo, hiddenKeyLoc);
                                }
                                if (cellInfo != null)
                                {
                                    if (cellInfo.CellRange != null && hiddenCount.Length > j - 1 && ((this.GridControl.RowPivotsOnly && rowIndex > 1) || !this.GridControl.RowPivotsOnly) && (this.GridControl.PivotRows.Count == 0 || (ShowRowColumnHeaders ? (cellInfo.CellType != PivotCellType.TopLeftCell) : true)))
                                    {
                                        int height = Math.Max(cellInfo.CellRange.Bottom - hiddenCount[j - 1] - cellInfo.CellRange.Top, 0);
                                        this.sheet.Range[rowIndex, cellInfo.CellRange.Left + 1, rowIndex + height, cellInfo.CellRange.Right + 1].Merge();
                                    }

                                    if (cellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                                        !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                        !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                        this.sheet.Range[rowIndex, colIndex].CellStyleName = "ColumnHeaderStyle";
                                    }

                                    else if (cellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.TotalCell.ToString()) &&
                                             !cellInfo.CellType.ToString().Contains(PivotCellType.GrandTotalCell.ToString()))
                                    {
                                        this.sheet.Range[rowIndex, colIndex].CellStyleName = "RowHeaderStyle";

                                    }

                                    else if (cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell | PivotCellType.CalculationHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.ColumnHeaderCell) ||
                                             cellInfo.CellType == (PivotCellType.HeaderCell | PivotCellType.GrandTotalCell | PivotCellType.RowHeaderCell))
                                    {
                                        this.sheet.Range[rowIndex, colIndex].CellStyleName = "SummaryHeaderStyle";

                                    }

                                    else if (cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) ||
                                             cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell) ||
                                        cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell))
                                    {
                                        this.sheet.Range[rowIndex, colIndex].CellStyleName = "SummaryCellStyle";

                                    }

                                    else if (cellInfo.CellType == PivotCellType.ValueCell)
                                    {
                                        this.sheet.Range[rowIndex, colIndex].CellStyleName = "ValueCellStyle";
                                    }

                                  
                                    this.sheet.Range[rowIndex, colIndex].IgnoreErrorOptions = ExcelIgnoreError.All;
                                    if ((cellInfo.ParentCell == null || this.GridControl.PivotEngine.UseIndexedEngine) && cellInfo.FormattedText != null)
                                    {
                                        //check for special Summary
                                        if (j - 1 >= GridControl.PivotRows.Count && GridControl.PivotCalculations.Count > 0)
                                        {
                                            int k = (j - GridControl.PivotRows.Count - 1) % GridControl.PivotCalculations.Count;
                                            if (GridControl.PivotCalculations[k].Summary is DisplayIfDiscreteValuesEqual)
                                            {
                                                if (cellInfo.Value != null)
                                                {
                                                    if (cellInfo.DoubleValue > 0.0 || cellInfo.Value.ToString() == "0")
                                                        sheet.Range[rowIndex, colIndex].Number = cellInfo.DoubleValue;
                                                    else
                                                        sheet.Range[rowIndex, colIndex].Text = cellInfo.FormattedText;
                                                }
                                                continue;
                                            }
                                        }


                                        //For the value cells
                                        if ((cellInfo.CellType == PivotCellType.ValueCell || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell) || cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)) && cellInfo.Format != null && cellInfo.Format != String.Empty)
                                        {
                                            System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentUICulture.NumberFormat;
                                            double val;
                                            if (cellInfo.DoubleValue != 0)
                                            {
                                                val = Convert.ToDouble(cellInfo.DoubleValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                                this.sheet.Range[rowIndex, colIndex].Number = val;
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
                                                {
                                                    if (nfi.CurrencyPositivePattern == 0)
                                                    {
                                                        formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                                                    }
                                                    else if (nfi.CurrencyPositivePattern == 1)
                                                    {
                                                        formatString = formatString + "[$" + nfi.CurrencySymbol + "-411]";
                                                    }
                                                    else if (nfi.CurrencyPositivePattern == 2)
                                                    {
                                                        formatString = "[$" + nfi.CurrencySymbol + "-411]" + " " + formatString;
                                                    }
                                                    else if (nfi.CurrencyPositivePattern == 3)
                                                    {
                                                        formatString = formatString + " " + "[$" + nfi.CurrencySymbol + "-411]";
                                                    }
                                                }

                                                this.sheet.Range[rowIndex, colIndex].CellStyle.NumberFormat = formatString;
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
                                                this.sheet.Range[rowIndex, colIndex].CellStyle.NumberFormat = formatString;
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
                                                this.sheet.Range[rowIndex, colIndex].CellStyle.NumberFormat = formatString;
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
                                                this.sheet.Range[rowIndex, colIndex].CellStyle.NumberFormat = formatString;

                                            }
                                            else
                                            {
                                                double doubleValue = 0.0d;
                                                sheet.Range[rowIndex, colIndex].CellStyle.NumberFormatIndex = 4;
                                                sheet.Range[rowIndex, colIndex].NumberFormat = cellInfo.Format;
                                                if (cellInfo.Value != null)
                                                {
                                                    if (double.TryParse(cellInfo.Value.ToString(), out doubleValue))
                                                    {
                                                        sheet.Range[rowIndex, colIndex].Number = doubleValue;
                                                    }
                                                    else
                                                    {
                                                        sheet.Range[rowIndex, colIndex].Text = cellInfo.FormattedText;
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            double d;
                                            if (GridControl.RowPivotsOnly && rowIndex > 1 && numericRowPivots.Contains(colIndex - 1) && cellInfo.Value != null && double.TryParse(cellInfo.Value.ToString(), out d))
                                            {
                                                this.sheet.Range[rowIndex, colIndex].Number = cellInfo.DoubleValue;
                                            }
                                            else
                                            {
                                                this.sheet.Range[rowIndex, colIndex].Text = cellInfo.FormattedText;
                                            }
                                        }
                                    }
                                    if (!this.GridControl.ShowSubTotals)
                                    {
                                        if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                        {
                                            int subTotalColumnIndices = j;
                                            for (int k = 0; k < this.GridControl.PivotCalculations.Count; k++)
                                            {
                                                this.sheet.ShowColumn(subTotalColumnIndices, false);
                                                subTotalColumnIndices++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                        {
                                            if (!this.GridControl.PivotColumns[rowIndex - 1].ShowSubTotal)
                                            {
                                                int subTotalColumnIndices = j;
                                                for (int k = 0; k < this.GridControl.PivotCalculations.Count; k++)
                                                {
                                                    this.sheet.ShowColumn(subTotalColumnIndices, false);
                                                    subTotalColumnIndices++;
                                                }
                                            }
                                        }
                                    }
                                    if (!this.GridControl.ShowSubTotals)
                                    {
                                        if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                        {
                                            this.sheet.ShowRow(i, false);
                                        }
                                    }
                                    else
                                    {
                                        if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                        {
                                            if (!this.GridControl.PivotRows[colIndex - 1].ShowSubTotal)
                                                this.sheet.ShowRow(i, false);
                                        }
                                    }
                                    if (!this.GridControl.ShowGrandTotals)
                                    {
                                        if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell))
                                        {
                                            int subTotalColumnIndices = j;
                                            for (int k = 0; k < this.GridControl.PivotCalculations.Count; k++)
                                            {
                                                this.sheet.ShowColumn(subTotalColumnIndices, false);
                                                subTotalColumnIndices++;
                                            }
                                        }
                                    }
                                    if (!this.GridControl.ShowGrandTotals)
                                    {
                                        if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell))
                                        {
                                            this.sheet.ShowRow(i, false);
                                        }
                                    }
                                }

                                if (cellInfo != null)
                                {
                                    //give user a change to modify the default look
                                    ExportingToExcelEventArgs args = new ExportingToExcelEventArgs(this.sheet.Range[rowIndex, colIndex], cellInfo, rowIndex, colIndex);
                                    this.RaiseCurrentCellExported(args);
                                }

                            }
                            rowIndex++;
                        }

                        else if (this.GridControl.RowPivotsOnly)
                        {
                            for (int k = 0; k < this.GridControl.PivotRows.Count; ++k)
                            {
                                // hidden filtered first row in a block - need to save its settings to use on the first visible row in the block
                                PivotCellInfo cellInfo = this.GridControl.PivotEngine[i - 1, k];
                                if (cellInfo != null)
                                {
                                    if (cellInfo.CellRange != null)
                                    {
                                        hiddenCount[k] = GetHiddenCount(cellInfo, hiddenKeyLoc);
                                        int height = cellInfo.CellRange.Bottom - hiddenCount[k] - cellInfo.CellRange.Top;
                                        if (height < 0) //no visible rows in block
                                        {
                                            hiddenCellInfo[k] = null;
                                        }
                                        else
                                        {
                                            this.sheet.Range[rowIndex, cellInfo.CellRange.Left + 1, rowIndex + height, cellInfo.CellRange.Right + 1].Merge();
                                            hiddenCellInfo[k] = cellInfo;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //handle the row above the GrandTotal row that is not properly set in the above code.
                    if (GridControl.RowPivotsOnly && GridControl.PivotRows.Count > 2 && rowIndex - 2 > 0 && rowIndex - 2 < rowCount)
                    {
                        this.sheet.Range[rowIndex - 2, 1, rowIndex - 2, GridControl.PivotRows.Count].Merge();
                    }
                    for (int k = 1; k <= colCount; k++)
                    {
                        this.sheet.AutofitColumn(k);
                    }
                    if (ShowRowColumnHeaders == true)
                    {
                        for (int i = 1; i <= GridControl.PivotRows.Count; i++)
                        {
                            int offset = GridControl.PivotCalculations.Count > 1 ? 1 : 0;
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].CellStyle.Color = Color.White;
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].CellStyle.Font.RGBColor = Color.Black;
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].CellStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].CellStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].ColumnWidth = this.GridControl.ColumnHeaderCellStyle.FontSize + 10f;
                            this.sheet.Range[GridControl.PivotColumns.Count + offset, i].Text = GridControl.PivotRows[i - 1].FieldHeader;
                        }
                        sheet.InsertRow(1);
                        for (int i = 1; i <= GridControl.PivotColumns.Count; i++)
                        {
                            this.sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].CellStyle.Color = Color.White;
                            this.sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].CellStyle.Font.RGBColor = Color.Black;
                            this.sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].CellStyle.Font.Size = this.GridControl.ColumnHeaderCellStyle.FontSize;
                            this.sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].CellStyle.Font.FontName = this.GridControl.ColumnHeaderCellStyle.FontFamily.ToString();
                            this.sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].ColumnWidth = this.GridControl.ColumnHeaderCellStyle.FontSize + 10f;
                            sheet.Range[1, i + (GridControl.PivotRows.Count > 0 ? GridControl.PivotRows.Count : GridControl.PivotRows.Count + 1)].Text = GridControl.PivotColumns[i - 1].FieldHeader;
                        }
                    }
                }
                else
                {
                    if (this.GridControl.ItemSource is DataView)
                    {
                        dt = (this.GridControl.ItemSource as DataView).Table;
                    }
                    else if (this.GridControl.ItemSource is DataTable)
                    {
                        dt = this.GridControl.ItemSource as DataTable;
                    }
                    else if (this.GridControl.ItemSource is IEnumerable)
                    {
                        IList list = this.GridControl.ItemSource as IList;
                        PropertyDescriptorCollection propCol = this.GridControl.PivotEngine.ItemProperties;
                        dt = new DataTable(propCol[0].ComponentType.Name);
                        for (int i = 0; i < propCol.Count; i++)
                        {
                            Type type = propCol[i].PropertyType;
                            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                                type = Nullable.GetUnderlyingType(type);
                            DataColumn col = new DataColumn(propCol[i].Name, type);
                            dt.Columns.Add(col);
                        }
                        if (this.GridControl.AllowedFields.Count > 0)
                        {
                            foreach (Syncfusion.PivotAnalysis.Base.FieldInfo field in this.GridControl.AllowedFields)
                            {
                                if (field.FieldType == FieldTypes.Expression)
                                {
                                    DataColumn column = new DataColumn(field.Name, typeof(double));
                                    column.Expression = field.Expression;
                                    dt.Columns.Add(column);
                                }
                            }
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
                    DataSourceUpdateEventArgs args = new DataSourceUpdateEventArgs() { DataSource = dt };
                    if (this.GridControl.PivotCalculations.Any(x => x.SummaryType == SummaryType.Custom))
                    {
                        foreach (PivotComputationInfo info in this.GridControl.PivotCalculations)
                            if (info.SummaryType == SummaryType.Custom)
                            {
                                args.ComputationProperty = new Dictionary<string, DataSourceUpdateEventArgs.ComputationProperties>();
                                args.ComputationProperty.Add(info.FieldName, null);
                            }
                        this.RaiseUpdateDataSourceForCustomSummaries(args);
                    }
                    //Import the data from grid to excel sheet.
                    sheet.ImportDataTable(dt, true, 1, 1, -1, -1);

                    IPivotCache cache = this.workBook.PivotCaches.Add(sheet["A1:" + GridRangeInfo.GetAlphaLabel(dt.Columns.Count) + (dt.Rows.Count + 1)]);
                    IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A1"], cache);
                    if (!this.GridControl.ShowGrandTotals)
                    {
                        pivotTable.ShowColumnGrand = false;
                        pivotTable.ShowRowGrand = false;
                    }
                    foreach (PivotItem item in this.GridControl.PivotColumns)
                    {
                        IPivotField field = pivotTable.Fields[item.FieldMappingName];
                        if (field != null)
                        {
                            field.Axis = PivotAxisTypes.Column;
                            field.Name = item.FieldHeader!=null ? item.FieldHeader : item.FieldMappingName;
                            if (item.Format != null)
                            {
                                this.ApplyNumberFormat(item, field);
                            }
                            if (!this.GridControl.ShowSubTotals)
                            {
                                field.Subtotals = PivotSubtotalTypes.None;
                            }
                            else if (this.GridControl.ShowSubTotals && !item.ShowSubTotal)
                            {
                                field.Subtotals = PivotSubtotalTypes.None;
                            }
                            
                            field.CanDragToData = field.CanDragToRow = field.CanDragToColumn = item.AllowRunTimeGroupByField;
                            ApplyFilteredView(field);
                        }
                    }

                    foreach (PivotItem item in this.GridControl.PivotRows)
                    {
                        IPivotField field = pivotTable.Fields[item.FieldMappingName];
                        if (field != null)
                        {
                            field.Name = item.FieldHeader != null ? item.FieldHeader : item.FieldMappingName;
                            field.Axis = PivotAxisTypes.Row;
                            if (item.Format != null)
                            {
                                this.ApplyNumberFormat(item, field);
                            }
                            if (!this.GridControl.ShowSubTotals)
                            {
                                field.Subtotals = PivotSubtotalTypes.None;
                            }
                            else if (this.GridControl.ShowSubTotals && !item.ShowSubTotal)
                            {
                                field.Subtotals = PivotSubtotalTypes.None;
                            }
                            field.CanDragToColumn = field.CanDragToData = field.CanDragToRow = item.AllowRunTimeGroupByField;
                            ApplyFilteredView(field);
                        }
                    }

                    foreach (PivotComputationInfo item in this.GridControl.PivotCalculations)
                    {
                        IPivotField field = null;
                        if (item.SummaryType == SummaryType.Custom)
                        {
                            field = args.ComputationProperty[item.FieldName] != null ? pivotTable.Fields[args.ComputationProperty[item.FieldName].FieldName] : null;
                            if (field != null)
                            {
                                if (item.Format != null)
                                {
                                    this.ApplyNumberFormat(item, field);
                                }
                                field.CanDragToColumn = field.CanDragToRow = field.CanDragToData = item.AllowRunTimeGroupByField;
                                pivotTable.DataFields.Add(field, args.ComputationProperty[item.FieldName].FieldName, args.ComputationProperty[item.FieldName].SubTotalType);
                                field.Name = item.FieldHeader != null ? item.FieldHeader : item.FieldName;
                                ApplyFilteredView(field);
                            }
                        }
                        if (field == null)
                        {
                            field = pivotTable.Fields[item.FieldName];
                            if (field != null)
                            {
                                if (item.Format != null)
                                {
                                    this.ApplyNumberFormat(item, field);
                                }
                                field.CanDragToColumn = field.CanDragToRow = field.CanDragToData = item.AllowRunTimeGroupByField;
                                pivotTable.DataFields.Add(field, item.FieldName, GetSubTotalType(item.SummaryType));
                                field.Name = item.FieldHeader != null ? item.FieldHeader : item.FieldName;
                                ApplyFilteredView(field);
                            }
                        }
                    }

                    if (this.GridControl.GroupingBar != null && this.GridControl.GroupingBar.FilterHeaderArea.Items.CurrentItem != null)
                    {
                        foreach (FilterItemsCollection item in this.GridControl.GroupingBar.FilterHeaderArea.Items)
                        {
                            PivotItem filterItem = this.GridControl.GroupingBar.GetPivotItem(item);
                            pivotTable.Fields[filterItem.FieldMappingName].Axis = PivotAxisTypes.Page;
                            if (filterItem.Format != null)
                            {
                                this.ApplyNumberFormat(filterItem, pivotTable.Fields[filterItem.FieldMappingName]);
                            }
                            ApplyFilteredView(pivotTable.Fields[filterItem.FieldMappingName]);
                        }
                    }

                    pivotTable.ShowColumnGrand = pivotTable.ShowRowGrand = this.GridControl.ShowGrandTotals;
                    pivotTable.Options.DisplayErrorString = true;
                    if (this.GridControl.PivotEngine.ShowNullAsBlank)
                        pivotTable.Options.NullString = pivotTable.Options.ErrorString = string.Empty;
                    else
                        pivotTable.Options.NullString = pivotTable.Options.ErrorString = "0";
                    pivotTable.ShowDataFieldInRow = !this.GridControl.ShowCalculationsAsColumns;
                    pivotTable.Options.ShowTooltips = this.GridControl.ToolTipEnabled;
                    pivotTable.Options.IsSaveData = true;
                    pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                    if (this.GridControl.VisualStyle == PivotGridVisualStyle.Office2007Blue)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium2;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Office2007Silver)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight1;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Office2007Black)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark1;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Metro)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight13;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Blend)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark15;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Office2003)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight16;
                    else if (this.GridControl.VisualStyle == PivotGridVisualStyle.Default)
                        pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium9;

                   
                    //Activate the pivot worksheet.
                    pivotSheet.Activate();
                    pivotSheet.GridLineColor = ExcelKnownColors.Blue_grey;
                }
                this.sheet.GridLineColor = ExcelKnownColors.Blue_grey;
            }
            ////Apply styles to rows and column for Excel sheet .
            //this.FormatExcel();
            this.workBook.SaveAs(fileName, ExcelSaveType.SaveAsXLS);
            workBook.Close();
            excelEngine.Dispose();
        }

        private int GetHiddenCount(PivotCellInfo cellInfo, int hiddenKeyLoc)
        {
            int count = 0;
            if (cellInfo.CellRange != null && cellInfo.CellRange.Bottom >= cellInfo.CellRange.Top)
            {
                for (int i = cellInfo.CellRange.Top; i <= cellInfo.CellRange.Bottom; ++i)
                {
                    if (this.GridControl.PivotEngine.HiddenRowIndexes.Contains(this.GridControl.PivotEngine[i, hiddenKeyLoc]))
                        count++;
                }
            }

            return count;
        }
        /// <summary>
        /// Apply filtered view to respective pivot field.
        /// </summary>
        /// <param name="field">PivotField to which filtered view to apply.</param>
        private void ApplyFilteredView(IPivotField field)
        {
            itemsSet = new List<IPivotFieldItem>();
            for (int i = 0; i < this.GridControl.Filters.Count; i++)
            {
                if (this.GridControl.Filters[i].Name.Equals(field.Name))
                {
                    FilterItemsCollection items = this.GridControl.Filters[i].Tag as FilterItemsCollection;
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
                            else if (field.Items[items[j].Key] != null)
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
        private void ApplyNumberFormat(object item, IPivotField field)
        {
            int decimalPlaces = 0, result;
            string formatString;
            if (item != null)
            {
                System.Globalization.NumberFormatInfo nfi = System.Globalization.CultureInfo.CurrentUICulture.NumberFormat;
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
                        if (nfi.CurrencyPositivePattern == 0)
                        {
                            formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                        }
                        else if (nfi.CurrencyPositivePattern == 1)
                        {
                            formatString = formatString + "[$" + nfi.CurrencySymbol + "-411]";
                        }
                        else if (nfi.CurrencyPositivePattern == 2)
                        {
                            formatString = "[$" + nfi.CurrencySymbol + "-411]" + " " + formatString;
                        }
                        else if (nfi.CurrencyPositivePattern == 3)
                        {
                            formatString = formatString + " " + "[$" + nfi.CurrencySymbol + "-411]";
                        }
                        
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
                        if (nfi.CurrencyPositivePattern == 0)
                        {
                            formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                        }
                        else if (nfi.CurrencyPositivePattern == 1)
                        {
                            formatString = formatString + "[$" + nfi.CurrencySymbol + "-411]";
                        }
                        else if (nfi.CurrencyPositivePattern == 2)
                        {
                            formatString = "[$" + nfi.CurrencySymbol + "-411]" + " " + formatString;
                        }
                        else if (nfi.CurrencyPositivePattern == 3)
                        {
                            formatString = formatString + " " + "[$" + nfi.CurrencySymbol + "-411]";
                        }
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
                                 cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell | PivotCellType.CalculationHeaderCell) ||
                                 cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                 cellInfo.CellType==(PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell| PivotCellType.CalculationHeaderCell))
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
        /// <returns>Color</returns>
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

        /// <summary>
        /// Converts Hexadecimal value to Color format.
        /// </summary>
        /// <param name="hexVal">Hexadecimal value as string</param>
        /// <returns>Color of hexadecimal value.</returns>
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
        /// Enables cell wise exporting.
        /// </summary>
        Cell,
        /// <summary>
        /// Enables pivot table exporting.
        /// </summary>
        PivotTable
    }
    /// <summary>
    /// A delegate handler for the event QueryExportCellInfo
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">an event argument </param>
    public delegate void QueryExportCellInfoEventHandler(object sender, ExportingToExcelEventArgs e);
    /// <summary>
    /// A delegate handler for the event UpdateDataSource
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">an event argument </param>
    public delegate void UpdateDataSourceEventHandler(object sender, DataSourceUpdateEventArgs e);
    /// <summary>
    /// Class holds all the properties which helps to export templated cell 
    /// </summary>
    public class ExportingToExcelEventArgs : EventArgs
    {
        /// <summary>
        /// A constructor method that executes on instantiation of ExportingToExcelEventArgs class.
        /// </summary>
        /// <param name="excelCell">Range</param>
        /// <param name="cellInfo">Pivot Cell Information</param>
        /// <param name="rowIndex">row index</param>
        /// <param name="colIndex">column index</param>
        public ExportingToExcelEventArgs(IRange excelCell, PivotCellInfo cellInfo, int rowIndex, int colIndex)
        {
            ExcelCellInfo = excelCell;
            PivotCellInfo = cellInfo;
            RowIndex = rowIndex;
            ColumnIndex = colIndex;
        }
        /// <summary>
        /// Gets or sets whether the event should be handled or not
        /// </summary>
        public bool Handled { get; set; }
        /// <summary>
        /// Gets the cell range of the excel sheet
        /// </summary>
        public IRange ExcelCellInfo { get; internal set; }
        /// <summary>
        /// Gets the row index of the cell
        /// </summary>
        public int RowIndex { get; internal set; }
        /// <summary>
        /// Gets the column index of the cell
        /// </summary>
        public int ColumnIndex { get; internal set; }
        /// <summary>
        /// Gets the Pivot cell information such as FormattedText,Key,Value, CellRange, etc
        /// </summary>
        public PivotCellInfo PivotCellInfo { get; internal set; }
       
    }
    /// <summary>
    /// Class which holds the property for updating data source
    /// </summary>
    public class DataSourceUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataSourceUpdateEventArgs()
        {

        }
        /// <summary>
        /// Gets or Sets the DataTable 
        /// </summary>
        public DataTable DataSource { get; set; }
        /// <summary>
        /// Gets or Sets properties need for CustomSummary computation
        /// </summary>
        public Dictionary<string, ComputationProperties> ComputationProperty { get; set; }
        /// <summary>
        /// Class which holds the property for computing custom summary
        /// </summary>
        public class ComputationProperties
        {
            /// <summary>
            /// Construct for the class ComputationProperties to initialize the parameters
            /// </summary>
            /// <param name="newFieldName">string</param>
            /// <param name="subTotalType">Pivot Subtotal types</param>
            public ComputationProperties(string newFieldName, PivotSubtotalTypes subTotalType)
            {
                FieldName = newFieldName;
                SubTotalType = subTotalType;
            }
            /// <summary>
            /// Gets or Sets the Field name
            /// </summary>
            public string FieldName { get; set; }

            /// <summary>
            /// Gets or Sets the SubTotalType
            /// </summary>
            public PivotSubtotalTypes SubTotalType { get; set; }
        }
    }
}
