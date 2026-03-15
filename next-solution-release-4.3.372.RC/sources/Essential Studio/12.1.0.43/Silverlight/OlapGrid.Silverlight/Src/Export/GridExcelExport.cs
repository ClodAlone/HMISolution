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
using System.Text;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using System.IO;
using Syncfusion.XlsIO;
using Syncfusion.OlapSilverlight.Engine;
using System.ComponentModel;
using Syncfusion.Silverlight.Grid.Olap.Common;

namespace Syncfusion.Silverlight.Grid.Olap
{
    /// <summary>
    /// GridExcelExport exports the Pivot data to Excel with the applied style
    /// </summary>
    public class GridExcelExport
    {
        #region Private Members

        /// <summary>
        /// Engine to be used for export
        /// </summary>
        private PivotEngine __pivotData;
        /// <summary>
        /// bold value
        /// </summary>
        private bool bold;
        /// <summary>
        /// italic
        /// </summary>
        private bool italic;
        /// <summary>
        /// Declare IAplication to instantiate various kinds applications  
        /// </summary>
        private IApplication application;
        /// <summary>
        /// Create instance for Excel Engine
        /// </summary>
        private ExcelEngine excelEngine = new ExcelEngine();
        /// <summary>
        /// Declare Woorkbook to be added in Excel  file
        /// </summary>
        private IWorkbook workBook;
        /// <summary>
        /// Declare sheets to be added in workBook . 
        /// </summary>
        private IWorksheet sheet;
        /// <summary>
        /// GridExport style information
        /// </summary>
        private ExportingGridStyleInfo GridExportStyle=new ExportingGridStyleInfo();
        /// <summary>
        /// Total value
        /// </summary>
        private const string m_Total = "Total";
        /// <summary>
        /// Grand Total value
        /// </summary>
        private const string m_GrandTotal = "Grand Total";
        /// <summary>
        /// /// To Get and Set pivot engine values
        /// </summary>
        [DefaultValue((PivotEngine)null)]
        private PivotEngine PivotData
        {
            get
            {
                if (__pivotData != null)
                {
                    return __pivotData;
                }
                else
                {
                    throw new NullReferenceException();
                }
            }

            set
            {
                __pivotData = value;
            }
        }

        /// <summary>
        /// Gets or sets Grid Layout
        /// </summary>
        private GridLayout Layout { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [item source].
        /// </summary>
        private bool ItemSource { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="DrillResult">The drill result.</param>
        /// <param name="styleinfo">The styleinfo.</param>
        /// <param name="layout">The layout.</param>
        public GridExcelExport(PivotEngine DrillResult,ExportingGridStyleInfo styleinfo,GridLayout layout,bool ItemSource)
        {
            this.application = excelEngine.Excel;
            this.workBook = application.Workbooks.Create(5);
            this.sheet = workBook.Worksheets[0];
            this.PivotData = DrillResult;
            sheet.IsGridLinesVisible = true;
            this.GridExportStyle = styleinfo;
            this.Layout = layout;
            this.ItemSource = ItemSource;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Saves as.
        /// </summary>
        /// <param name="Filename">The filename.</param>
        private void SaveAs(string Filename)
        {
            this.workBook.SaveAs(Filename);
            workBook.Close();
        }

        /// <summary>
        /// Apply styles to rows and column for Excel sheet .
        /// </summary>
        private void FormatExcel()
        {
            ////Remove grid lines in the worksheet.
            this.sheet.IsGridLinesVisible = false;
            String fontstyle=GridExportStyle.HeaderFontStyle;
            if (fontstyle != string.Empty)
            {
                if (fontstyle == "Bold")
                    bold = true;
                else if (fontstyle == "Italic")
                    italic = true;
            }
            for (int i = 1; i <= PivotData.RowsCount; i++)
            {
                for (int j = 1; j <= PivotData.TableColumns.Count; j++)
                {
                    PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                    if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                    {
                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = GridExportStyle.CellFontColor; //styles.CellFontColor;
                        this.sheet.Range[i, j].CellStyle.Font.Size = GridExportStyle.CellFontSize;
                        this.sheet.Range[i, j].CellStyle.Font.FontName = GridExportStyle.CellFontName;
                        this.sheet.Range[i, j].ColumnWidth = GridExportStyle.CellFontSize + 10f;
                        this.sheet.Range[i, j].IgnoreErrorOptions = ExcelIgnoreError.All;
                    }
                }
            }

            if (this.ItemSource)
            {
                for (int i = 1; i <= PivotData.HeaderSection.Height; i++)
                {
                    for (int j = 1; j <= PivotData.TableColumns.Count; )
                    {
                        PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                        ////Adding styles to the Column
                        if (cellDescriptor.Range != null)
                        {
                            StringBuilder range1 = new StringBuilder();
                            StringBuilder st = new StringBuilder();

                            st.Append(GetExcelRange((j - 1), i, ((j + cellDescriptor.Range.Width - 1) - 1), (i + cellDescriptor.Range.Bottom)));
                            range1.Append(GetColumnRange((j - 1), i));

                            if (!this.sheet.Range[st.ToString()].IsMerged)
                            {
                                if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                                {
                                    this.sheet.Range[st.ToString()].Merge();
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = (GridExportStyle.HeaderBackgroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = (GridExportStyle.HeaderForeGroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (fontstyle == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (fontstyle == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }

                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    ///Apply color to summary row ad colummns
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal || this.ItemSource)
                                    {
                                        for (int start = 1; start <= PivotData.RowsCount; start++)
                                        {
                                            this.sheet.Range[start, j].CellStyle.Color = GridExportStyle.SummaryColumnBackgroundColor;
                                            this.sheet.Range[start, j].CellStyle.Font.RGBColor = GridExportStyle.SummaryColumnForegroundColor;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = GridExportStyle.SummaryColumnForegroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 10f;

                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.Any)
                                {
                                    this.sheet.Range[st.ToString()].Clear();
                                    this.sheet.Range[st.ToString()].Merge();
                                    j += cellDescriptor.Range.Width;
                                }

                                else
                                {
                                    j++;
                                }
                            }
                            else
                            {
                                j += cellDescriptor.Range.Width;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
                for (int i = PivotData.RowHeaderSection.Top; i < PivotData.RowsCount; i++)
                {
                    for (int j = 1; j <= PivotData.RowHeaderSection.Width; )
                    {
                        PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i];
                        ////Adding styles to the Column
                        if (cellDescriptor.Range != null)
                        {
                            StringBuilder range1 = new StringBuilder();
                            StringBuilder st = new StringBuilder();

                            st.Append(GetExcelRange((j - 1), i + 1, ((j + cellDescriptor.Range.Width - 1) - 1), ((i) + cellDescriptor.Range.Height)));
                            range1.Append(GetColumnRange((j - 1), i + 1));
                            if (!this.sheet.Range[st.ToString()].IsMerged)
                            {
                                if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                                {
                                    this.sheet.Range[st.ToString()].Merge();
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = GridExportStyle.HeaderRowBackgroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = GridExportStyle.HeaderRowForegroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (fontstyle == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (fontstyle == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                                {
                                    ////Apply Color to sumary  Column
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal || this.ItemSource)
                                    {
                                        for (int start = j; start <= PivotData.TableColumns.Count; start++)
                                        {
                                            if (this.ItemSource)
                                            {
                                                this.sheet.Rows[i].Cells[start - 1].CellStyle.Color = GridExportStyle.SummaryRowBackgroundColor;
                                                this.sheet.Rows[i].Cells[start - 1].CellStyle.Font.RGBColor = GridExportStyle.SummaryRowForegroundColor;
                                            }
                                            else
                                            {
                                                this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Color = GridExportStyle.SummaryRowBackgroundColor;
                                                this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.RGBColor = GridExportStyle.SummaryRowForegroundColor;
                                            }
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = (GridExportStyle.SummaryRowForegroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }

                                else if (cellDescriptor.CellType == PivotCellDescriptorType.Any)
                                {
                                    this.sheet.Range[st.ToString()].Clear();
                                    this.sheet.Range[st.ToString()].Merge();
                                    j += cellDescriptor.Range.Width;
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            else
                            {
                                j++;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 1; i <= PivotData.HeaderSection.Height; i++)
                {
                    for (int j = 1; j <= PivotData.TableColumns.Count; )
                    {
                        PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                        ////Adding styles to the Column
                        if (cellDescriptor.Range != null)
                        {
                            StringBuilder range1 = new StringBuilder();
                            StringBuilder st = new StringBuilder();
                            st.Append(GetExcelRange((j - 1), i, ((j + cellDescriptor.Range.Right) - 1), (i + cellDescriptor.Range.Bottom)));
                            range1.Append(GetColumnRange((j - 1), i));
                            if (!this.sheet.Range[st.ToString()].IsMerged)
                            {
                                if (cellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                                {
                                    this.sheet.Range[st.ToString()].Merge();
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = (GridExportStyle.HeaderBackgroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = (GridExportStyle.HeaderForeGroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (fontstyle == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (fontstyle == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }

                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    ///Apply color to summary row ad colummns
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal)
                                    {
                                        for (int start = 1; start <= PivotData.RowsCount; start++)
                                        {
                                            this.sheet.Range[start, j].CellStyle.Color = GridExportStyle.SummaryColumnBackgroundColor;
                                            this.sheet.Range[start, j].CellStyle.Font.RGBColor = GridExportStyle.SummaryColumnForegroundColor;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = GridExportStyle.SummaryColumnForegroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 10f;

                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.Any)
                                {
                                    this.sheet.Range[st.ToString()].Clear();
                                    this.sheet.Range[st.ToString()].Merge();
                                    j += cellDescriptor.Range.Width;
                                }

                                else
                                {
                                    j++;
                                }
                            }
                            else
                            {
                                j += cellDescriptor.Range.Width;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
                for (int i = PivotData.RowHeaderSection.Top; i <= PivotData.RowsCount; i++)
                {
                    for (int j = 1; j <= PivotData.RowHeaderSection.Width; )
                    {
                        PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                        ////Adding styles to the Column
                        if (cellDescriptor.Range != null)
                        {
                            StringBuilder range1 = new StringBuilder();
                            StringBuilder st = new StringBuilder();
                            st.Append(GetExcelRange((j - 1), i, ((j + cellDescriptor.Range.Right) - 1), (i + cellDescriptor.Range.Bottom)));
                            range1.Append(GetColumnRange((j - 1), i));
                            if (!this.sheet.Range[st.ToString()].IsMerged)
                            {
                                if (cellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                                {
                                    this.sheet.Range[st.ToString()].Merge();

                                    this.sheet.Range[range1.ToString()].CellStyle.Color = GridExportStyle.HeaderRowBackgroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = GridExportStyle.HeaderRowForegroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (fontstyle == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (fontstyle == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryRow)
                                {
                                    ////Apply Color to sumary  Column
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal)
                                    {
                                        for (int start = j; start <= PivotData.TableColumns.Count; start++)
                                        {
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Color = GridExportStyle.SummaryRowBackgroundColor;
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.RGBColor = GridExportStyle.SummaryRowForegroundColor;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = (GridExportStyle.SummaryRowForegroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 10f;
                                    j += cellDescriptor.Range.Width;
                                }

                                else if (cellDescriptor.CellType == PivotCellDescriptorType.Any)
                                {
                                    this.sheet.Range[st.ToString()].Clear();
                                    this.sheet.Range[st.ToString()].Merge();
                                    j += cellDescriptor.Range.Width;
                                }
                                else
                                {
                                    j++;
                                }
                            }
                            else
                            {
                                j++;
                            }
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Set the cell range to span row or column
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private StringBuilder GetColumnRange(int col, int row)
        {
            string[] alphabets = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",  "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            StringBuilder range = new StringBuilder();
            StringBuilder st = new StringBuilder();
            if (col <= (alphabets.Length - 1))
            {
                range.Append(alphabets[col].ToString());
            }
            else
            {
                int m = col / alphabets.Length;
                int l = col % alphabets.Length;
                range.Append(alphabets[m - 1].ToString());
                range.Append(alphabets[l].ToString());
            }
            range.Append(row.ToString());
            return range;
        }
        /// <summary>
        /// Sets Excel Range to draw olapgrid data
        /// </summary>
        /// <param name="col1"></param>
        /// <param name="row1"></param>
        /// <param name="col2"></param>
        /// <param name="row2"></param>
        /// <returns></returns>
        private StringBuilder GetExcelRange(int col1, int row1, int col2, int row2)
        {
            StringBuilder st = new StringBuilder();
            st.Append(GetColumnRange(col1, row1));
            st.Append(":");
            st.Append(GetColumnRange(col2, row2));
            return st;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Exports the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Export(Stream stream)
        {
            for (int i = 1; i <= PivotData.RowsCount; i++)
            {
                for (int j = 1; j <= PivotData.TableColumns.Count; j++)
                {
                    this.sheet.Range[i, j].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    this.sheet.Range[i, j].BorderAround(ExcelLineStyle.Thin);
                    this.sheet.Range[i, j].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
                    this.sheet.Range[i, j].Text = PivotData.TableColumns[j - 1].Cells[i - 1].CellValue;

                    if (this.Layout == GridLayout.ExcelLikeLayout)
                    {
                        if (PivotData.TableColumns[j - 1].Cells[i - 1].CellExTypes.Contains(PivotCellDescriptorType.SummaryRow.ToString()))
                        {
                            this.sheet.Range[i, j].CellStyle.Color = GridExportStyle.SummaryColumnBackgroundColor;// Color(GridExportStyle.SummaryColumnBackgroundColor);
                            this.sheet.Range[i, j].CellStyle.Font.RGBColor = GridExportStyle.SummaryColumnForegroundColor;
                        }
                    }
                }
            }
            ///Apply styles to rows and column for Excel sheet .
            FormatExcel();
            this.sheet.GridLineColor =  ExcelKnownColors.Black;
            this.workBook.SaveAs(stream, ExcelSaveType.SaveAsXLS);
            workBook.Close();
            excelEngine.Dispose();
        }
        #endregion
    }
}
