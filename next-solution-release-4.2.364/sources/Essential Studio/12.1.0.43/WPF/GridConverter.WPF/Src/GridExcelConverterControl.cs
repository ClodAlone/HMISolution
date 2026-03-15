#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid.Converter
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using Syncfusion.Windows.Controls.Cells;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.XlsIO;
    using Syncfusion.XlsIO.Implementation;
    using System.Threading;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Globalization;
#if !SILVERLIGHT
    using System.Windows.Forms;
    using System.Data;
    using Syncfusion.Windows.Shared;
    using System.Windows.Documents;
    using Syncfusion.Windows.Controls.Grid;
    using System.Globalization;
   // using System.Windows.Media;
#endif

    /// <summary>
    /// Control class for ExcelExport
    /// </summary>
    public class GridExcelConverterControl : GridExcelConverterBase
    {

        private delegate void GridExportToExcelDelegate(GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange range, GridCellExportToExcelHandler exportingHandler);

#if !SILVERLIGHT
        [NonSerialized]
        private int asyncActiveCount = 0;

        [NonSerialized]
        private AutoResetEvent asyncActiveEvent = null;
        private GridExportToExcelDelegate exportDelegate = null;

        // Begin IAsyncResult method
        public virtual IAsyncResult BeginExportToExcel(GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref this.asyncActiveCount);
            GridExportToExcelDelegate gridExportToExcelDelegate = new GridExportToExcelDelegate(this.ExportToExcel);
            if (this.asyncActiveEvent == null)
            {
                lock (this)
                {
                    if (this.asyncActiveEvent == null)
                    {
                        this.asyncActiveEvent = new AutoResetEvent(true);
                    }
                }
            }

            this.asyncActiveEvent.WaitOne();
            this.exportDelegate = gridExportToExcelDelegate;
            return gridExportToExcelDelegate.BeginInvoke(gridModel, gridRange, workSheet, excelRange, null, callback, state);
        }

        public virtual IAsyncResult BeginExportToExcel(GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler, AsyncCallback callback, object state)
        {
            Interlocked.Increment(ref this.asyncActiveCount);
            GridExportToExcelDelegate gridExportToExcelDelegate = new GridExportToExcelDelegate(this.ExportToExcel);
            if (this.asyncActiveEvent == null)
            {
                lock (this)
                {
                    if (this.asyncActiveEvent == null)
                    {
                        this.asyncActiveEvent = new AutoResetEvent(true);
                    }
                }
            }

            this.asyncActiveEvent.WaitOne();
            this.exportDelegate = gridExportToExcelDelegate;
            return gridExportToExcelDelegate.BeginInvoke(gridModel, gridRange, workSheet, excelRange, exportingHandler, callback, state);
        }
        
        // End IAsyncResult method
        public virtual void EndExportToExcel(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }

            if (this.exportDelegate == null)
            {
                throw new ArgumentException("Delegate is null or is being called multiple times");
            }

            try
            {
                this.exportDelegate.EndInvoke(asyncResult);
            }
            finally
            {
                this.exportDelegate = null;
                this.asyncActiveEvent.Set();
                Interlocked.Decrement(ref this.asyncActiveCount);
                if ((this.asyncActiveEvent != null) && (asyncActiveCount == 0))
                {
                    this.asyncActiveEvent.Close();
                    this.asyncActiveEvent = null;
                }
            }
        }

        internal void ExportToExcel(GridModel gridModel, GridRangeInfo gridRange, IWorksheet workSheet, IRange excelRange, GridCellExportToExcelHandler exportingHandler)
        {
            gridRange = GetExpandedRange(gridRange, gridModel);
            int excelRow = excelRange.Row;
            int excelCol = excelRange.Column;

            for (int row = gridRange.Top; row <= gridRange.Bottom; row++)
            {
                for (int col = gridRange.Left; col <= gridRange.Right; col++)
                {
                    this.GridCellToExcel(gridModel, row, col, workSheet.Range[excelRow, excelCol], null, true);
                    CoveredCellInfo coveredRange = gridModel.CoveredCells.GetCoveredCell(row, col);

                    if (coveredRange != null)
                    {
                        int rowDiff = coveredRange.Top - gridRange.Top;
                        int columnDiff = coveredRange.Left - gridRange.Left;
                        GridRangeInfo gridCRange = GridRangeInfo.Cells(coveredRange.Top, coveredRange.Left, coveredRange.Bottom, coveredRange.Right);
                        IRange exlRange = workSheet.Range[excelRange.Row + rowDiff, excelRange.Column + columnDiff, excelRange.Row + rowDiff + gridCRange.Height - 1, excelRange.Column + columnDiff + gridCRange.Width - 1];
                        if (gridCRange.Height > 1 || gridCRange.Width > 1)
                        {
                            exlRange.Merge();
                            var borders = gridModel[row, col].Borders;

                            if (borders.HasLeft)
                                this.CopyBorder(borders.Left, exlRange.Borders[ExcelBordersIndex.EdgeLeft],true);

                            if (borders.HasTop)
                                this.CopyBorder(borders.Top, exlRange.Borders[ExcelBordersIndex.EdgeTop],true);

                            if (borders.HasBottom)
                                this.CopyBorder(borders.Bottom, exlRange.Borders[ExcelBordersIndex.EdgeBottom],true);

                            if (borders.HasRight)
                                this.CopyBorder(borders.Right, exlRange.Borders[ExcelBordersIndex.EdgeRight],true);
                        }
                    }
                    else
                    {
                        this.CopyBorders(gridModel[row, col], workSheet.Range[excelRow, excelCol],true);
                    }

                    excelCol++;
                }

                excelRow++;
                excelCol = excelRange.Column;
            }
        }

        private static GridRangeInfo GetExpandedRange(GridRangeInfo range, GridModel gridModel)
        {
            if (range.IsCols)
            {
                range = range.ExpandRange(range.Top + 1, range.Left, gridModel.RowCount, range.Left);
            }

            if (range.IsRows)
            {
                range = range.ExpandRange(range.Top, range.Left + 1, range.Top, gridModel.ColumnCount);
            }

            if (range.IsTable)
            {
                range = range.ExpandRange(range.Top + 1, range.Left + 1, gridModel.RowCount, gridModel.ColumnCount);
            }

            return range;
        }
#endif

        /// <summary>
        /// Copies one grid cell into excel cell.
        /// </summary>
        /// <param name="gridModel">Source grid</param>
        /// <param name="row">Grid cell row index to copy</param>
        /// <param name="column">Grid cell column index to copy</param>
        /// <param name="range">Destination excel range</param>
        public void GridCellToExcel(GridModel gridModel, int row, int column, IRange range, GridCellExportToExcelHandler exportingHandler, bool IsAsyncExport)
        {
            GridStyleInfo gridCell = gridModel[row, column];

            CopyColumnWidthFromGrid(gridCell, range);
            CopyRowHeightFromGrid(gridCell, range);
            object value = gridCell.CellValue;
            ExportingToExcelEventArgs args = new ExportingToExcelEventArgs(range, false);
            args.RowIndex = row;
            args.ColumnIndex = column;
            if (exportingHandler != null)
            {
                exportingHandler(gridModel, args);
            }

            //if (QueryCellExportInfo != null)
            //{
            //    QueryCellExportInfo(this, args);
            //}

            if (value != null && !args.Handled)
            {
                Type cellType = value.GetType();

                if (cellType == typeof(string))
                {
                    if (gridCell.CellType != "FormulaCell")
                    {
                        range.Text = gridCell.CellValue.ToString();
                    }
                    else
                    {
                        range.Text = gridCell.GetFormattedText(value);
                    }
                }
#if !SILVERLIGHT
                else if (cellType == typeof(BitmapImage))
                {
                    BitmapImage bitmap = value as BitmapImage;

                    if (bitmap != null)
                    {
                        int rangeRow = range.Row;
                        int rangeColumn = range.Column;
                        BitmapImage bi = bitmap; // Get bitmapimage from somewhere
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bi));
                        encoder.Save(ms);
                        Bitmap bmp = new Bitmap(ms);
                        Image image = bmp as Image;
                        System.IO.Stream stream = bitmap.StreamSource;
                        IPictureShape picutreShape = range.Worksheet.Pictures.AddPicture(rangeRow, rangeColumn, image);

                        picutreShape.Width = range.Worksheet.ColumnWidthToPixels(range.ColumnWidth);
                        picutreShape.Height = (int)range.RowHeight;
                        double h = 0, w = 0;
                        double alignmentWidth = 0, alignmentHeight = 0;
                        CoveredCellInfo coveredRange = gridModel.CoveredCells.GetCoveredCell(gridCell.RowIndex, gridCell.ColumnIndex);

                        if (coveredRange != null)
                        {
                            for (int i = coveredRange.Top; i <= coveredRange.Bottom; i++)
                            {
                                h = h + gridModel.RowHeights[i];
                                if (i > range.Row)
                                    alignmentHeight += gridModel.RowHeights[i];
                            }

                            for (int j = coveredRange.Left; j <coveredRange.Right; j++)
                            {
                                w = w + gridModel.ColumnWidths[j];
                                if (j > range.Column)
                                    alignmentWidth += gridModel.ColumnWidths[j];
                            }
                            picutreShape.Width = (int)w;
                            picutreShape.Height = (int)h;
                        }
                        //Image gets stretched in the exported excel file when we set the Image Width and Height.
                        if (gridCell.HasImageWidth)
                            if ((int)gridCell.ImageWidth.Value < picutreShape.Width)
                                picutreShape.Width =(int)Math.Round(gridCell.ImageWidth.Value);

                        if (gridCell.HasImageHeight)
                            if ((int)gridCell.ImageHeight.Value < picutreShape.Height)
                                picutreShape.Height = (int)gridCell.ImageHeight.Value;

                        //when the image load in covered cell with Image width and height, we have set the Top and Bottom position in IPictureShape
                        var difWidth = (alignmentWidth - picutreShape.Width);
                        if (difWidth < 0)
                            difWidth = 0;
                        var difHeight = (alignmentHeight - picutreShape.Height);
                        if (difHeight < 0)
                            difHeight = 0;
                        switch (gridCell.HorizontalAlignment)
                        {
                            case System.Windows.HorizontalAlignment.Center:
                                {
                                    if (gridCell.Padding.Left != 0)
                                        picutreShape.Left += (int)(gridCell.Padding.Left/1.2);

                                    else
                                    {
                                    
                                    double d = (w - picutreShape.Width) / 2;
                                    picutreShape.Left += (int)Math.Round(d/1.2);
                                    
                                   }
                                }
                                break;
                            case System.Windows.HorizontalAlignment.Right:
                                {
                                    if (gridCell.Padding.Left != 0)
                                        picutreShape.Left += (int)(gridCell.Padding.Left/1.2);
                                    else
                                    {
                                    double d = w - picutreShape.Width;
                                    picutreShape.Left += (int)Math.Round(d/1.2);
                                    }

                                }

                                break;
                            case System.Windows.HorizontalAlignment.Left:
                                {
                                    if (gridCell.Padding.Left != 0)
                                    {
                                        picutreShape.Left += (int)((gridCell.Padding.Left) / 1.2);
                                        if (picutreShape.Width + picutreShape.Left >= w-15)
                                            picutreShape.Width = (int)(w - (picutreShape.Left + (gridCell.Padding.Right / 1.2)));
                                      
                                    }
                                    else
                                    {
                                        double d = w - picutreShape.Width;
                                        picutreShape.Left += (int)Math.Round(d / 1.2);
                                    }
                                    
                                }
                                break;
                        }

                        switch (gridCell.VerticalAlignment)
                        {
                            case System.Windows.VerticalAlignment.Center:
                                {
                                    int temp = picutreShape.Top;
                                    if (gridCell.Padding.Top != 0)
                                        picutreShape.Top += (int)(gridCell.Padding.Top/1.2);
                                    else
                                    {
                                    
                                    double d = (h - picutreShape.Height) / 2;
                                    picutreShape.Top += (int)Math.Round(d/1.2);
                                    }
                                    if (picutreShape.Top > temp + 15)
                                        picutreShape.Top = temp + 15;
                                }
                                break;
                            case System.Windows.VerticalAlignment.Bottom:
                                {
                                    int temp = picutreShape.Top;
                                    if (gridCell.Padding.Top != 0)
                                        picutreShape.Top += (int)(gridCell.Padding.Top/1.2);
                                    else
                                    {
                                    double d = h - picutreShape.Height;
                                    picutreShape.Top += (int)Math.Round(d/1.2);
                                    }
                                    if (picutreShape.Top > temp + 15)
                                        picutreShape.Top = temp + 15;
                                }
                                break;
                            case System.Windows.VerticalAlignment.Top:
                                {
                                    int temp = picutreShape.Top;
                                    if (gridCell.Padding.Top != 0)
                                        picutreShape.Top += (int)(gridCell.Padding.Top/1.2);
                                    else
                                    {
                                    double d = h - picutreShape.Height;
                                    picutreShape.Top += (int)Math.Round(d/1.2);
                                    }
                                    if (picutreShape.Top > temp + 15)
                                        picutreShape.Top = temp + 15;
                                }
                                break;
                        }
                    }
                }
#endif
                else if (cellType == typeof(DateTime))
                {
                    object objValue = value;
                    if (objValue is DateTime)
                    {
                        DateTime dt = (DateTime)objValue;
#if !SILVERLIGHT
                        string text = gridCell.GetText(value);
                        DateTime dateTime = DateTime.MinValue;
                        var culture = gridCell.GetCulture(true);
                        string result = culture.DateTimeFormat.ShortDatePattern;

                        if (DateTime.TryParse(text, culture, System.Globalization.DateTimeStyles.None, out dateTime) && gridCell.DateTimeEdit.HasDateTimePattern)
                        {
                            switch (gridCell.DateTimeEdit.DateTimePattern)
                            {
                                case DateTimePattern.ShortDate:
                                    result = culture.DateTimeFormat.ShortDatePattern;
                                    break;
                                case DateTimePattern.ShortTime:
                                    result = culture.DateTimeFormat.ShortTimePattern;
                                    break;
                                case DateTimePattern.LongDate:
                                    result = culture.DateTimeFormat.LongDatePattern;
                                    break;
                                case DateTimePattern.LongTime:
                                    result = dateTime.ToString(culture.DateTimeFormat.LongTimePattern);
                                    break;
                                case DateTimePattern.FullDateTime:
                                    result = culture.DateTimeFormat.FullDateTimePattern;
                                    break;
                                case DateTimePattern.MonthDay:
                                    result = culture.DateTimeFormat.MonthDayPattern;
                                    break;
                                case DateTimePattern.RFC1123:
                                    result = culture.DateTimeFormat.RFC1123Pattern;
                                    break;
                                case DateTimePattern.SortableDateTime:
                                    result = culture.DateTimeFormat.SortableDateTimePattern;
                                    break;
                                case DateTimePattern.UniversalSortableDateTime:
                                    result = culture.DateTimeFormat.UniversalSortableDateTimePattern;
                                    break;
                                case DateTimePattern.YearMonth:
                                    result = culture.DateTimeFormat.YearMonthPattern;
                                    break;
                                case DateTimePattern.CustomPattern:
                                    if (gridCell.DateTimeEdit.HasCustomPattern)
                                    {
                                        result = gridCell.DateTimeEdit.CustomPattern;
                                    }
                                    break;
                            }
                        }

#endif
                        range.DateTime = dt;
#if !SILVERLIGHT
                        result = result.Replace("tt", "AM/PM");
                        range.CellStyle.NumberFormat = result;
#endif
                    }
                }
                else if (cellType == typeof(Int32))
                {
                    Int32 val;
                    if (value.ToString() != string.Empty)
                    {
                        val = Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        range.Number = val;
                    }
                    if (gridCell.Format != string.Empty)
                    {
                        range.CellStyle.NumberFormat = gridCell.Format;
                    }
                }
                else if (cellType == typeof(Int16))
                {
                    Int16 val;
                    if (value.ToString() != string.Empty)
                    {
                        val = Convert.ToInt16(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        range.Number = val;
                    }
                    if (gridCell.Format != string.Empty)
                    {
                        range.CellStyle.NumberFormat = gridCell.Format;
                    }
                }

                else if (cellType == typeof(Int64))
                {
                    Int64 val;
                    if (value.ToString() != string.Empty)
                    {
                        val = Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        range.Number = val;
                    }
                    if (gridCell.Format != string.Empty)
                    {
                        range.CellStyle.NumberFormat = gridCell.Format;
                    }
                }

                else if (cellType == typeof(Double))
                {
                    Double val;

                    if (value.ToString() != string.Empty)
                    {
                        val = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        range.Number = val;

                    }

                    if (gridCell.Format != string.Empty)
                    {
                        //No need to get the number format form the FormattedText
                        //Diectly set the number format to the excel range from the GridStyleInfo.Format
                        range.CellStyle.NumberFormat = gridCell.Format;
                    }
                }

                else if (cellType == typeof(NumericUpDown))
                {
                    range.Value = gridCell.GetFormattedText(value);
                }

                else if (cellType == typeof(Decimal))
                {
                    Double val;

                    if (value.ToString() != string.Empty)
                    {
                        val = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                        range.Number = val;   // range does not contains Decimal celltype in xlsio. Hence the value is converted to nearest format Double . This fixs the issue #Sd6061 ( Inc 72966).
                    }
                    if (gridCell.Format != string.Empty)
                    {
                        range.CellStyle.NumberFormat = gridCell.Format;
                    }
                }
                else if (cellType == typeof(Single))
                {
                    range.Value = gridCell.GetFormattedText(value);
                }
                else if (cellType.IsEnum)
                {
                    range.Value = gridCell.GetFormattedText(value);
                }
                else if (cellType == typeof(bool))
                {
                    range.Boolean = bool.Parse(value.ToString());
                    //range.Value = gridCell.GetFormattedText(value);                    
                }
#if !SILVERLIGHT
                else if (cellType == typeof(FlowDocument))
                {
                    FlowDocument Flowdocument = value as FlowDocument;
                    RichTextBoxHelper.SetRichTextValue(range, Flowdocument, gridCell.GetFormattedText(value));
                }
#else
                else if (cellType == typeof(Paragraph))
                {
                    Paragraph paragraph = value as Paragraph;
                    RichTextBoxHelper.SetRichTextValue(range, paragraph, gridCell.GetFormattedText(value));
                }
#endif
            }

            if (!args.Handled)
            {
                switch (gridCell.CellType)
                {
                    case "FormulaCell":
                        {
                            if (gridCell.CellType == "FormulaCell" && gridCell.Text.Length != 0
                       && gridCell.Text[0] == '=')
                            {
                                if (gridCell.CellValue2 != null)
                                {
                                    range.Formula = gridCell.CellValue2.ToString();
                                }
                                else
                                {
                                    range.Formula = gridCell.Text;
                                }
                            }
                            if (gridCell.Format != string.Empty)
                            {
                                range.CellStyle.NumberFormat = gridCell.Format;
                            }
                        }

                        break;

                    case "CurrencyEdit":

                        System.Globalization.NumberFormatInfo nfi = (gridCell != null) ? gridCell.NumberFormat : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
                        if (nfi != null)
                        {
                            Double val;
                            if (value != null && value.ToString() != string.Empty)
                            {
                                val = Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                                range.Number = val;
                            }
                            else
                                val = 0;

                            ////  string valString = val.ToString("C", nfi);
                            string valString = gridCell.GetFormattedText(value);

                            string formatString = "#,##0.";
                            for (int i = 0; i < nfi.CurrencyDecimalDigits; i++)
                            {
                                formatString += "0";
                            }

                            if (!string.IsNullOrEmpty(valString) && Char.IsNumber(valString, 0))
                            {
                                formatString += " [$" + nfi.CurrencySymbol + "-411]";
                            }
                            else
                            {
                                formatString = "[$" + nfi.CurrencySymbol + "-411]" + formatString;
                            }

                            range.CellStyle.NumberFormat = formatString;
                        }
                        break;

                    case "PercentEdit":
#if !SILVERLIGHT
                        string text = (value != null && !(value is DBNull)) ? value.ToString() : string.Empty;
                        var numberFormat = gridCell.HasNumberFormat ? gridCell.NumberFormat : gridCell.GetCulture(false).NumberFormat;
                        if (gridCell.NumberFormat != null && gridCell.NumberFormat != gridCell.GetCulture(false).NumberFormat)
                        {
                            numberFormat = gridCell.NumberFormat;
                        }
                        else
                        {
                            numberFormat = gridCell.GetCulture(false).NumberFormat;
                        }
                        //var formattedText = this.GetFormattedText(text, numberFormat);
                        double dValue = 0.0;
                        string retString = "";
                        text = this.GetNumber(text, numberFormat);
                        if (double.TryParse(text, NumberStyles.Any, numberFormat, out dValue))
                        {
                            if (gridCell.PercentEditMode == PercentEditMode.DoubleMode)
                                dValue /= 100.0;
                            retString = dValue.ToString("P", numberFormat);
                        }

                        range.Value = retString;
                        break;
#else
                        decimal dValue = 0;
                        string retString = string.Empty;
                        if (value != null && value.ToString().Length > 0)
                        {
                            double temp = Convert.ToDouble(value.ToString());  // The value is temp converted as Raw double without any format as decimal.TryParse() can't convert the value like " 5.02323-E05" which contains E in the text. 
                            dValue = Convert.ToDecimal(temp);
                        }
                        string text = dValue.ToString();
                        var numberFormat = gridCell.NumberFormat;

                        if (decimal.TryParse(text, NumberStyles.Currency, numberFormat, out dValue))
                        {
                            retString = dValue.ToString("C", numberFormat);
                        }
                        range.Value = retString;
                        break;
#endif
                        
                    default:
                        break;
                }
            }
          
            CopyStyle(gridModel[row, column], range, IsAsyncExport);

        }

        private string GetNumber(string text, NumberFormatInfo numberFormat)
        {
            string retString = String.Empty;
            int len = text.Length;

            for (int i = 0; i < len; i++)
            {
                char c = text[i];
                if (Char.IsDigit(c) || numberFormat.NegativeSign.Contains(c.ToString()) || c.ToString() == numberFormat.PercentDecimalSeparator)
                {
                    retString += c;
                }
            }

            return retString;
        }

    }


}
