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
using System.Linq;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.XlsIO;
using System.IO;
using Syncfusion.Silverlight.Controls.PivotGrid;
using System.Collections;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Grid;
using System.Reflection;
using Syncfusion.XlsIO.Implementation.PivotTables;

namespace Syncfusion.Silverlight.Controls.PivotGrid.Converter
{
    /// <summary>
    /// GridExcelExport exports the Pivot data to Excel Sheet with the applied style
    /// </summary>
    public class GridExcelExport
    {
        #region Members

        private ExcelEngine _excelEngine;
        private object[] array;
        private PivotSubtotalTypes subTotalType;
        private IWorksheet pivotSheet;        
        private int rowCount = 1;
        private int colCount = 1;
        private List<IPivotFieldItem> itemsSet;
        private PivotFieldDataFormat fieldDataFormat;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="pivotGridControl">The pivot grid control.</param>
        public GridExcelExport(PivotGridControl pivotGridControl)
            :this(new PivotGridControl[]{ pivotGridControl },ExcelVersion.Excel97to2003,ExportModes.Cell)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="control">The pivot grid control.</param>
        /// <param name="mode">Export Mode</param>
        public GridExcelExport(PivotGridControl control, ExportModes mode)
            : this(new PivotGridControl[] { control }, ExcelVersion.Excel97to2003, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="control">The pivot grid control</param>
        /// <param name="version">Excel Version.<remarks>Accessible through adding Syncfusion.XlsIO.Silverlight as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>
        /// <remarks></remarks>
        public GridExcelExport(PivotGridControl control, ExcelVersion version, ExportModes mode)
            : this(new PivotGridControl[]{control}, version, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Silverlight.Controls.PivotGrid.Converter.GridExcelExport">GridExcelExport</see> class. 
        /// </summary>
        /// <param name="gridControls">Array of PivotGridControl.</param>
        /// <param name="version">Excel Version.<remarks>Accessible through adding Syncfusion.XlsIO.Silverlight as project reference.</remarks></param>
        /// <param name="mode">Export Mode</param>
        public GridExcelExport(PivotGridControl[] gridControls, ExcelVersion version, ExportModes mode)
        {
            if (gridControls == null || gridControls.Length <= 0)
                throw new ArgumentNullException("gridControls", "PivotGridControl array might be null or its length might be zero");
            this.Application = this.ExcelEngine.Excel;
            // TODO: Version hardcoded for PivotTable Mode due to parsing issue with other versions in XlsIO.
            this.Application.DefaultVersion = mode == ExportModes.Cell ? version : ExcelVersion.Excel2007;
            this.ExportMode = mode;
            this.subTotalType = PivotSubtotalTypes.Count;
            this.WorkBook = this.ExportMode == ExportModes.Cell ? this.Application.Workbooks.Create(gridControls.Length) : this.Application.Workbooks.Create(2 * gridControls.Length);
            this.GridCollection = gridControls;
        }
        #endregion

        #region Properties

        private PivotGridControl[] GridCollection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>The application.</value>
        public IApplication Application { get; set; }

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
        /// Event to handle updation of data source when Custom summary is used
        /// </summary>
        public event UpdateDataSourceEventHandler UpdateDataSource;

        void OnUpdateDataSource(DataSourceUpdateEventArgs e)
        {
            if (UpdateDataSource != null)
            {
                UpdateDataSource(this, e);
            }
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
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            if (this.ExportMode == ExportModes.PivotTable && ((2 * this.GridCollection.Length) != this.WorkBook.Worksheets.Count))
            {
                this.Application.DefaultVersion = ExcelVersion.Excel2007;
                this.WorkBook = this.Application.Workbooks.Create(2 * GridCollection.Length);
            }

            for (int index = 0, count = 0; index < this.GridCollection.Length; index++, count++)
            {
                this.PivotGridControl = this.GridCollection[index];
                this.Sheet = this.WorkBook.Worksheets[count];
                this.Sheet.IsGridLinesVisible = true;
                if (this.ExportMode == ExportModes.PivotTable)
                {
                    this.pivotSheet = this.WorkBook.Worksheets[++count];
                    this.pivotSheet.IsGridLinesVisible = true;
                }
                rowCount = this.PivotGridControl.PivotEngine.RowCount;
                colCount = this.PivotGridControl.PivotEngine.ColumnCount;
                if (this.ExportMode == ExportModes.Cell)
                {
                    for (int i = 1; i <= rowCount; i++)
                    {
                        for (int j = 1; j <= colCount; j++)
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
                                         cellInfo.CellType == (PivotCellType.TotalCell |PivotCellType.CalculationHeaderCell | PivotCellType.RowHeaderCell) ||
                                         cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell) ||
                                         cellInfo.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell) ||
                                         cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell) ||
                                         cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell) ||
                                         cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
                                         cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.GrandTotalCell | PivotCellType.CalculationHeaderCell) ||
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
                                         cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.GrandTotalCell)||
                                         cellInfo.CellType == (PivotCellType.ValueCell | PivotCellType.TotalCell | PivotCellType.GrandTotalCell))
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
                                if ((cellInfo.ParentCell == null || this.PivotGridControl.PivotEngine.UseIndexedEngine) && cellInfo.FormattedText != null)
                                {
                                    this.Sheet.Range[i, j].Text = cellInfo.FormattedText;
                                }
                                if (!this.PivotGridControl.ShowSubTotals)
                                {
                                    if (cellInfo.CellType == (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                    {
                                        int subTotalColumnIndices = j;
                                        for (int k = 0; k < this.PivotGridControl.PivotCalculations.Count; k++)
                                        {
                                            this.Sheet.ShowColumn(subTotalColumnIndices, false);
                                            subTotalColumnIndices++;
                                        }
                                    }
                                }
                                if (!this.PivotGridControl.ShowSubTotals)
                                {
                                    if (cellInfo.CellType == (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                    {
                                        this.Sheet.ShowRow(i, false);
                                    }
                                }
                                if (!this.PivotGridControl.ShowGrandTotals)
                                {
                                    if (cellInfo.FormattedText!=null && cellInfo.FormattedText.Contains(this.PivotGridControl.PivotEngine.GrandString))
                                    {
                                        int grandTotalColumnIndices = j;
                                        for (int k = 0; k < this.PivotGridControl.PivotCalculations.Count; k++)
                                        {
                                            this.Sheet.ShowColumn(grandTotalColumnIndices, false);
                                            grandTotalColumnIndices++;
                                        }
                                    }
                                }
                                
                            }
                        }
                        if (!this.PivotGridControl.ShowGrandTotals)
                        {
                            if (this.PivotGridControl.PivotEngine[i-1,0].FormattedText!=null && this.PivotGridControl.PivotEngine[i-1,0].FormattedText.Contains(this.PivotGridControl.PivotEngine.GrandString))
                            {
                                this.Sheet.ShowRow(i, false);
                            }
                        }
                    }
                }
                else
                {
                    if (this.PivotGridControl.ItemSource is IEnumerable)
                    {
                        IEnumerable query = this.PivotGridControl.ItemSource as IEnumerable;
                        array = query.OfType<object>().ToArray();

                        int fieldCount = 0;
                        PropertyInfo[] fields = null;
                        IEnumerator enumerator = query.GetEnumerator();
                        if (enumerator.MoveNext())
                        {
                            object item = enumerator.Current;
                            fields = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                            fieldCount = fields.Count();
                        }

                        object[,] array2D = new object[array.Length + 1, fieldCount];

                        for (int i = 0; i <= array.Length; i++)
                        {
                            for (int j = 0; j < fieldCount; j++)
                            {
                                if (i == 0)
                                    array2D[i, j] = fields[j].Name;
                                else
                                    array2D[i, j] = array[i - 1].GetType().GetProperty(fields[j].Name).GetValue(array[i - 1], null);
                            }
                        }

                        DataSourceUpdateEventArgs args = new DataSourceUpdateEventArgs() { DataSource = array2D };
                        if (this.PivotGridControl.PivotCalculations.Any(x => x.SummaryType == SummaryType.Custom))
                        {
                            foreach (PivotComputationInfo info in this.PivotGridControl.PivotCalculations)
                                if (info.SummaryType == SummaryType.Custom)
                                {
                                    args.ComputationProperty = new Dictionary<string, DataSourceUpdateEventArgs.ComputationProperties>();
                                    args.ComputationProperty.Add(info.FieldName, null);
                                }
                            this.RaiseUpdateDataSourceForCustomSummaries(args);
                        }
                        //Import the data from grid to excel sheet.
                        Sheet.ImportArray(array2D, 1, 1);

                        IPivotCache cache = this.WorkBook.PivotCaches.Add(Sheet["A1:" + GridRangeInfo.GetAlphaLabel(array2D.GetLength(1)) + (array2D.GetLength(0))]);
                        IPivotTable pivotTable = pivotSheet.PivotTables.Add("PivotTable1", pivotSheet["A1"], cache);
                        if (!this.PivotGridControl.ShowGrandTotals)
                        {
                            pivotTable.ShowColumnGrand = false;
                            pivotTable.ShowRowGrand = false;
                        }
                        foreach (PivotItem item in this.PivotGridControl.PivotColumns)
                        {
                            IPivotField field = pivotTable.Fields[item.FieldMappingName];
                            if (field != null)
                            {
                                field.Axis = PivotAxisTypes.Column;
                                if (item.Format != null)
                                {
                                    this.ApplyNumberFormat(item, field);
                                }
                                if (!this.PivotGridControl.ShowSubTotals)
                                {
                                    field.Subtotals = PivotSubtotalTypes.None;
                                }
                                field.CanDragToData = field.CanDragToRow = field.CanDragToColumn = item.AllowRunTimeGroupByField;
                                ApplyFilteredView(field);
                            }
                        }

                        foreach (PivotItem item in this.PivotGridControl.PivotRows)
                        {
                            IPivotField field = pivotTable.Fields[item.FieldMappingName];
                            if (field != null)
                            {
                                field.Axis = PivotAxisTypes.Row;
                                if (item.Format != null)
                                {
                                    this.ApplyNumberFormat(item, field);
                                }
                                if (!this.PivotGridControl.ShowSubTotals)
                                {
                                    field.Subtotals = PivotSubtotalTypes.None;
                                }
                                field.CanDragToColumn = field.CanDragToData = field.CanDragToRow = item.AllowRunTimeGroupByField;
                                ApplyFilteredView(field);
                            }
                        }

                        foreach (PivotComputationInfo item in this.PivotGridControl.PivotCalculations)
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
                                    for (int i = 0; i < pivotTable.DataFields.Count; i++)
                                    {
                                        PivotDataField dataField = pivotTable.DataFields[i] as PivotDataField;
                                        if (dataField.Field.Name.Equals(field.Name))
                                        {
                                            dataField.ShowDataAs = GetDataFormat(item.CalculationType);
                                            break;
                                        }
                                    }
                                }
                            }
                            if(field == null)
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
                                    for (int i = 0; i < pivotTable.DataFields.Count; i++)
                                    {
                                        PivotDataField dataField = pivotTable.DataFields[i] as PivotDataField;
                                        if (dataField.Field.Name.Equals(field.Name))
                                        {
                                            dataField.ShowDataAs = GetDataFormat(item.CalculationType);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (this.PivotGridControl.AllowedFields.Count > 0)
                                    {
                                        foreach (Syncfusion.PivotAnalysis.Base.Silverlight.FieldInfo fieldInfo in this.PivotGridControl.AllowedFields)
                                        {
                                            if (fieldInfo.FieldType == FieldTypes.Expression && fieldInfo.Name == item.FieldName)
                                            {
                                                string expression = fieldInfo.Expression.Replace("[", "").Replace("]", "");

                                                pivotTable.CalculatedFields.Add(fieldInfo.Name, expression);
                                                if (item.Format != null)
                                                {
                                                    this.ApplyNumberFormat(item, pivotTable.CalculatedFields[fieldInfo.Name]);
                                                }
                                                for (int i = 0; i < pivotTable.DataFields.Count; i++)
                                                {
                                                    PivotDataField dataField = pivotTable.DataFields[i] as PivotDataField;
                                                    if (dataField.Field.Name.Equals(field.Name))
                                                    {
                                                        dataField.ShowDataAs = GetDataFormat(item.CalculationType);
                                                        break;
                                                    }
                                                }
                                                pivotTable.CalculatedFields[fieldInfo.Name].Axis = PivotAxisTypes.Data;
                                                pivotTable.CalculatedFields[fieldInfo.Name].CanDragToColumn = pivotTable.CalculatedFields[fieldInfo.Name].CanDragToRow = pivotTable.CalculatedFields[fieldInfo.Name].CanDragToPage = false;
                                                pivotTable.CalculatedFields[fieldInfo.Name].CanDragToData = item.AllowRunTimeGroupByField;
                                                for (int i = 0; i < pivotTable.DataFields.Count; i++)
                                                {
                                                    PivotDataField dataField = pivotTable.DataFields[i] as PivotDataField;
                                                    if (dataField.Field.Name.Equals(fieldInfo.Name))
                                                    {
                                                        dataField.ShowDataAs = PivotFieldDataFormat.Normal;
                                                        dataField.Subtotal = GetSubTotalType(item.SummaryType);
                                                        break;
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (this.PivotGridControl.GroupingBar != null && this.PivotGridControl.GroupingBar.Filters.Count > 0)
                        {
                            foreach (FilterItemsCollection item in this.PivotGridControl.GroupingBar.FilterHeaderArea.Items)
                            {
                                PivotItem filterItem = this.PivotGridControl.GroupingBar.GetPivotItem(item);
                                pivotTable.Fields[filterItem.FieldMappingName].Axis = PivotAxisTypes.Page;
                                if (filterItem.Format != null)
                                {
                                    this.ApplyNumberFormat(filterItem, pivotTable.Fields[filterItem.FieldMappingName]);
                                }
                                ApplyFilteredView(pivotTable.Fields[filterItem.FieldMappingName]);
                            }
                        }



                        pivotTable.ShowColumnGrand = pivotTable.ShowRowGrand = this.PivotGridControl.ShowGrandTotals;
                        pivotTable.Options.DisplayErrorString = true;
                        if(this.PivotGridControl.PivotEngine.ShowNullAsBlank)
                            pivotTable.Options.NullString = pivotTable.Options.ErrorString = string.Empty;
                        else
                            pivotTable.Options.NullString = pivotTable.Options.ErrorString = "0";
                        pivotTable.ShowDataFieldInRow = !this.PivotGridControl.ShowCalculationsAsColumns;
                        pivotTable.Options.ShowTooltips = this.PivotGridControl.ToolTipEnabled;
                        pivotTable.Options.IsSaveData = true;
                        pivotTable.Options.RowLayout = PivotTableRowLayout.Tabular;

                        if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Blue)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium2;
                        else if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Silver)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight1;
                        else if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Office2007Black)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark1;
                        else if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Metro)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight13;
                        else if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Blend)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleDark15;
                        else if (this.PivotGridControl.VisualStyle ==  Syncfusion.Windows.Controls.Theming.VisualStyle.Default)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleMedium9;
                        else if (this.PivotGridControl.VisualStyle == Syncfusion.Windows.Controls.Theming.VisualStyle.Office2003)
                            pivotTable.BuiltInStyle = PivotBuiltInStyles.PivotStyleLight16;
                        //Activate the pivot worksheet.
                        pivotSheet.Activate();
                        pivotSheet.GridLineColor = ExcelKnownColors.Blue_grey;
                    }
                    else
                    {
                        MessageBox.Show("Export is not supported for this datasource type in PivotTable mode!");
                        return;
                    }
                }
                this.Sheet.GridLineColor = ExcelKnownColors.Black;
            }
            ////Apply styles to rows and column for Excel Sheet .
            //this.FormatExcel();
            this.WorkBook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
            this.WorkBook.Close();
            this.ExcelEngine.Dispose();
        }
        private PivotFieldDataFormat GetDataFormat(CalculationType calculationType)
        {
            if (calculationType == CalculationType.NoCalculation)
                fieldDataFormat = PivotFieldDataFormat.Normal;
            else if (calculationType == CalculationType.PercentageOfColumnTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfColumn;
            else if (calculationType == CalculationType.PercentageOfRowTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfRow;
            else if (calculationType == CalculationType.PercentageOfParentTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfParent;
            else if (calculationType == CalculationType.PercentageOfParentRowTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfParentRow;
            else if (calculationType == CalculationType.PercentageOfParentColumnTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfParentColumn;
            else if (calculationType == CalculationType.Index)
                fieldDataFormat = PivotFieldDataFormat.Index;
            else if (calculationType == CalculationType.PercentageOfGrandTotal)
                fieldDataFormat = PivotFieldDataFormat.PercentageOfTotal;
            return fieldDataFormat;
        }

        /// <summary>
        /// Apply filtered view to respective pivot field.
        /// </summary>
        /// <param name="field">PivotField to which filtered view to apply.</param>
        private void ApplyFilteredView(IPivotField field)
        {
            itemsSet = new List<IPivotFieldItem>();
            for (int i = 0; i < this.PivotGridControl.Filters.Count; i++)
            {
                if (this.PivotGridControl.Filters[i].Name.Equals(field.Name))
                {
                    FilterItemsCollection items = this.PivotGridControl.Filters[i].Tag as FilterItemsCollection;
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
        private void ApplyNumberFormat(object item, IPivotField field)
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
        /// Gets the color from gradient.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>Color</returns>
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
        /// Converts Hexadecimal value to Color format.
        /// </summary>
        /// <param name="hexVal">Hexadecimal value as string</param>
        /// <returns>Color of hexadecimal value.</returns>
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

    /// <summary>
    /// Export Modes
    /// </summary>
    /// <remarks>To export the PivotGrid either cell wise or as PivotTable.</remarks>
    public enum ExportModes
    {
        Cell,
        PivotTable
    }

    /// <summary>
    /// A delegate handler for the event UpdateDataSource
    /// </summary>
    /// <param name="sender">object</param>
    /// <param name="e">an event argument </param>
    public delegate void UpdateDataSourceEventHandler(object sender, DataSourceUpdateEventArgs e);

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
        /// Gets or Sets the DataSource (2D array) 
        /// </summary>
        public object[,] DataSource { get; set; }
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
