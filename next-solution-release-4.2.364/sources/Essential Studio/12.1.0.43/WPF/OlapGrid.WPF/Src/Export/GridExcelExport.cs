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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.IO;
using System.ComponentModel;
using Syncfusion.XlsIO;
using Syncfusion.Olap.Engine;
using Syncfusion.Olap.Manager;
using Syncfusion.Windows.Grid.Olap;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Grid.Olap
{
    /// <summary>
    /// GridExcelExport exports the Pivot data to Excel with the applied style
    /// </summary>
    public class GridExcelExport
    {
        #region Private Members 

        private Color __anyStyleColor;

        private Color __columnHeaderStyleColor;

        private PivotEngine __pivotData;

        private Color __rowHeaderStyleColor;

        private Color __summaryStyleColor;

        private Color __summaryColor;

        private Color __textStyleColor;

        private bool bold;

        private bool italic;

        #endregion
        /// <summary>
        /// Declare IAplication to instantiate variouss kinds applications  
        /// </summary>
        IApplication application;
        /// <summary>
        /// Create instance for Excel Engine
        /// </summary>
        ExcelEngine excelEngine = new ExcelEngine();
        /// <summary>
        /// Declare Woorkbook to be added in Excel  file
        /// </summary>
        IWorkbook workBook;
        /// <summary>
        /// Declare sheets to be added in workBook . 
        /// </summary>
        IWorksheet sheet;
                     
        /// <summary>
        /// Create instance for ExportingGridStyleInfo
        /// </summary>
        ExportingGridStyleInfo GridExportStyle=new ExportingGridStyleInfo();

        /// <summary>
        /// Gets or sets the color of any style.
        /// </summary>
        /// <value>The color of any style.</value>
        private Color AnyStyleColor
        {
            get
            {
                return __anyStyleColor;
            }

            set
            {
                __anyStyleColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the column header style.
        /// </summary>
        /// <value>The color of the column header style.</value>
        private Color ColumnHeaderStyleColor
        {
            get
            {
                return __columnHeaderStyleColor;
            }

            set
            {
                __columnHeaderStyleColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the row header style.
        /// </summary>
        /// <value>The color of the row header style.</value>
        private Color RowHeaderStyleColor
        {
            get
            {
                return __rowHeaderStyleColor;
            }

            set
            {
                __rowHeaderStyleColor = value;
            }
            
        }

        /// <summary>
        /// Gets or sets the color of the summary.
        /// </summary>
        /// <value>The color of the summary.</value>
        private Color summaryColor
        {
            get
            {
                return __summaryColor;
            }

            set
            {
                __summaryColor = value;
            }
        }


        /// <summary>
        /// Gets or sets the color of the summary style.
        /// </summary>
        /// <value>The color of the summary style.</value>
        private Color SummaryStyleColor
        {
            get
            {
                return __summaryStyleColor;
            }

            set
            {
                __summaryStyleColor = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the color of the text style.
        /// </summary>
        /// <value>The color of the text style.</value>
        private Color TextStyleColor
        {
            get
            {
                return __textStyleColor;
            }

            set
            {
                __textStyleColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the pivot data.
        /// </summary>
        /// <value>The pivot data.</value>
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
          /// <summary>
        /// Total value
        /// </summary>
        private const string m_Total = "Total";
        /// <summary>
        /// Grand Total value
        /// </summary>
        private const string m_GrandTotal = "Grand Total";
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GridExcelExport"/> class.
        /// </summary>
        /// <param name="DrillResult">The drill result.</param>
        /// <param name="styleinfo">The styleinfo.</param>
        /// <param name="layout">The layout.</param>
        /// <param name="ItemSource">if set to <c>true</c> [item source].</param>
        public GridExcelExport(PivotEngine DrillResult, ExportingGridStyleInfo styleinfo, GridLayout layout,bool ItemSource)
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

        /// <summary>
        /// Exports the specified file name.
        /// </summary>
        /// <param name="FileName">Name of the file.</param>
        public void Export(string FileName)
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
                        if (PivotData.TableColumns[j - 1].Cells[i - 1].CellExTypes.Contains("SummaryRow"))
                        {
                            //for (int start = 1; start <= PivotData.RowsCount; start++)
                            {
                                this.sheet.Range[i, j].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnBackgroundColor);
                                this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor);
                            }
                        }
                    }
                }
            }
            ///Apply styles to rows and column for Excel sheet .
            this.FormatExcel();
            this.sheet.GridLineColor = ExcelKnownColors.Black;
            this.workBook.SaveAs(FileName, ExcelSaveType.SaveAsXLS);
            workBook.Close();
            excelEngine.Dispose();
        }

        /// <summary>
        ///  To save the Workbook with Exported Data
        /// </summary>
        /// <param name="Filename">The filename.</param>
        public void SaveAs(string Filename)
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
            if (GridExportStyle.CellFontStyle != "")
            {
                if (GridExportStyle.CellFontStyle.ToString() == "Bold")
                    bold = true;
                else if (GridExportStyle.CellFontStyle.ToString() == "Italic")
                    italic = true;
            }
            for (int i = 1; i <= PivotData.RowsCount; i++)
            {
                for (int j = 1; j <= PivotData.TableColumns.Count; j++)
                {
                    PivotCellDescriptor cellDescriptor = PivotData.TableColumns[j - 1].Cells[i - 1];
                    if (cellDescriptor.CellType == PivotCellDescriptorType.Value)
                    {
                        this.sheet.Range[i, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.CellFontColor); //convert.ValueTextColor;
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
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.HeaderBackgroundColor);//convert.HeaderBackGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.HeaderForeGroundColor); //convert.HeaderForeGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (GridExportStyle.CellFontStyle.ToString() == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (GridExportStyle.CellFontStyle.ToString() == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }

                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 11f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    ////Apply color to summary row and colummns
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal || this.ItemSource)
                                    {
                                        for (int start = 1; start <= PivotData.RowsCount; start++)
                                        {
                                            this.sheet.Range[start, j].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnBackgroundColor); //convert.SummaryColumnBackGroundColor;
                                            this.sheet.Range[start, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor);//convert.SummaryColumnForeGroundColor ; 
                                            this.sheet.Range[start, j].CellStyle.Font.FontName = GridExportStyle.SummaryFontName;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor); //convert.SummaryColumnForeGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 11f;
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
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.HeaderRowBackgroundColor); //convert.HeaderRowBackGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.HeaderRowForegroundColor);  //convert.HeaderRowForeGroundColor;                                this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = styles.HeaderFontName != "" ? styles.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName;
                                    if (GridExportStyle.CellFontStyle.ToString() == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (GridExportStyle.CellFontStyle.ToString() == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 11f;
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
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.SummaryRowBackgroundColor); //convert.SummaryRowBackGroundColor;
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryRowForegroundColor);  //convert.SummaryRowForeGroundColor; 
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.FontName = GridExportStyle.SummaryFontName;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryRowForegroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 11f;
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
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.HeaderBackgroundColor);//convert.HeaderBackGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.HeaderForeGroundColor); //convert.HeaderForeGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName != "" ? GridExportStyle.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    if (GridExportStyle.CellFontStyle.ToString() == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (GridExportStyle.CellFontStyle.ToString() == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }

                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 11f;
                                    j += cellDescriptor.Range.Width;
                                }
                                else if (cellDescriptor.CellType == PivotCellDescriptorType.SummaryColumn)
                                {
                                    ///Apply color to summary row and colummns
                                    ///
                                    this.sheet.Range[st.ToString()].Merge();
                                    if (cellDescriptor.CellValue == m_Total || cellDescriptor.CellValue == m_GrandTotal || this.ItemSource)
                                    {
                                        for (int start = 1; start <= PivotData.RowsCount; start++)
                                        {
                                            this.sheet.Range[start, j].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnBackgroundColor); //convert.SummaryColumnBackGroundColor;
                                            this.sheet.Range[start, j].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor);//convert.SummaryColumnForeGroundColor ; 
                                            this.sheet.Range[start, j].CellStyle.Font.FontName = GridExportStyle.SummaryFontName;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryColumnForegroundColor); //convert.SummaryColumnForeGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 11f;
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
                                    this.sheet.Range[range1.ToString()].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.HeaderRowBackgroundColor); //convert.HeaderRowBackGroundColor;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.HeaderRowForegroundColor);  //convert.HeaderRowForeGroundColor;                                this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = styles.HeaderFontName != "" ? styles.HeaderFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.HeaderFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.HeaderFontName;
                                    if (GridExportStyle.CellFontStyle.ToString() == "Bold")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = true;
                                    else if (GridExportStyle.CellFontStyle.ToString() == "Italic")
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = true;
                                    else
                                    {
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Bold = false;
                                        this.sheet.Range[range1.ToString()].CellStyle.Font.Italic = false;
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.HeaderFontSize + 11f;
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
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Color = ColorTranslator.FromHtml(GridExportStyle.SummaryRowBackgroundColor); //convert.SummaryRowBackGroundColor;
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryRowForegroundColor);  //convert.SummaryRowForeGroundColor; 
                                            this.sheet.Rows[i - 1].Cells[start - 1].CellStyle.Font.FontName = GridExportStyle.SummaryFontName;
                                        }
                                    }
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.RGBColor = ColorTranslator.FromHtml(GridExportStyle.SummaryRowForegroundColor);
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.FontName = GridExportStyle.SummaryFontName != "" ? GridExportStyle.SummaryFontName : "Times New Roman";
                                    this.sheet.Range[range1.ToString()].CellStyle.Font.Size = GridExportStyle.SummaryFontSize;
                                    this.sheet.Range[range1.ToString()].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                                    this.sheet.Range[range1.ToString()].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
                                    this.sheet.Range[range1.ToString()].ColumnWidth = GridExportStyle.SummaryFontSize + 11f;
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
        /// Sets Excel Range to draw OlapGrid data
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
       
    }
}
