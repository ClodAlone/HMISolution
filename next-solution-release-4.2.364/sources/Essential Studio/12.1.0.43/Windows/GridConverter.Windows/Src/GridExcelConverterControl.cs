#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;
//using Syncfusion.Drawing;

#endregion

namespace Syncfusion.GridExcelConverter
{
  /// <summary>
  /// GridExcelConverterControl class provides support for Exporting datas from a gridControl into an Excel spreadsheet
  /// for verification and/or computation. This Control automatically Copies the Grid's Styles, Formats 
  /// to Excel. 
  /// The GridExcelConverter Control  is derived from 
  ///<see cref="GridExcelConverterBase"/>.
  /// </summary>
  [ToolboxItem(false)]
  public class GridExcelConverterControl : GridExcelConverterBase
  {
    #region Class members
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    private static readonly Hashtable s_hashBorderIndexToExcelLineStyle = new Hashtable();
    /// <summary>
    /// Indicates whether converter should autofit columns.
    /// </summary>
    private bool m_bAutoFitColumns;
    /// <summary>
    /// Indicates whether converter should autofit rows.
    /// </summary>
    private bool m_bAutoFitRows;

    private int iStartColIndex = 1;
    private int iEndColIndex = 1;
    private int iStartRowIndex = 1;
    private int iEndRowIndex = 1;
    private int iRowRange = 1;

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static GridExcelConverterControl()
    {
      FormulaUtil.RegisterAdditionalAlias( "POW", ExcelFunction.POWER );
      FormulaUtil.RegisterAdditionalAlias( "AVG", ExcelFunction.AVERAGE );
    }
    //    /// <summary>
    //    /// Static constructor.
    //    /// </summary>
    //    static GridExcelConverterControl()
    //    {
    //      FillBordersHash();
    //    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public GridExcelConverterControl()
    {
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose( bool disposing )
    {
      base.Dispose( disposing );
    }

    #endregion

    #region Class Export Grid to Excel methods

    #region Selected Range Export methods

    string[] messageStrings = null;

      /// <summary>
      /// Gets a string array that holds strings used with MessageBoxes that display error conditions.
      /// </summary>
      /// <example>Here is the code that initializes this string array so you can see what strings occupy
      /// which positions in the array.
      /// <code lang="C#">
      /// public string[] MessageStrings
      /// {
      ///     get 
      ///     {
      ///         if (messageStrings == null)
      ///         {
      ///             messageStrings = new string[4]
      ///             {
      ///                 "No range is currently selected. Please select a range to export and try again. Exporting terminated", //0
      ///                 "Exporting Error", //1
      ///                 "Empty Range can't be exported. Please input a valid range to export. Exporting terminated", //2
      ///                 "Exporting Error" //3
      ///             };
      ///         }
      ///         return messageStrings; 
      ///     }
      /// }
      /// </code>
      /// </example>
    public string[] MessageStrings
    {
        get 
        {
            if (messageStrings == null)
            {
                messageStrings = new string[4]
                {
                    "No range is currently selected. Please select a range to export and try again. Exporting terminated", //0
                    "Exporting Error", //1
                    "Empty Range can't be exported. Please input a valid range to export. Exporting terminated", //2
                    "Exporting Error" //3
                };

            }
            return messageStrings; 
        }
    }



    /// <summary>
    /// A method that exports selected range from grid model passed in.
    /// </summary>
    /// <param name="model">The grid model.</param>
    /// <param name="FileName">Name of the file to which the grid is to be exported.</param>
    /// <param name="options">The converter options.</param>
    public void SelectedExport(GridModel model, string FileName, ConverterOptions options)
    {
        if (model.SelectedRanges.Count == 0)
        {
            MessageBox.Show(MessageStrings[0], MessageStrings[1], MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        ExcelEngine engine = CreateEngine();
        IWorkbook book = engine.Excel.Workbooks.Create(1);
        IWorksheet sheet = book.Worksheets[0];
        GridRangeInfoList list = model.SelectedRanges;
        foreach (GridRangeInfo range in list)
        {
            switch (range.RangeType)
            {
                case GridRangeInfoType.Cells:
                    InternalExportRange(range, model, sheet, options);
                    break;
                case GridRangeInfoType.Cols:
                    InternalExportRange(GridRangeInfo.Cells(1, range.Left, model.RowCount, range.Right), model, sheet, options);
                    break;
                case GridRangeInfoType.Rows:
                    InternalExportRange(GridRangeInfo.Cells(range.Top, 1, range.Bottom, model.ColCount), model, sheet, options);
                    break;
                case GridRangeInfoType.Table:
                    book.Close();
                    engine.Dispose();
                    this.GridToExcel(model, FileName, options);
                    return;
            }
            if (options == ConverterOptions.ColumnHeaders)
                ExportColumnHeaders(model, sheet);
            else if (options == ConverterOptions.RowHeaders)
                ExportRowHeaders(model, sheet);
        }


        book.SaveAs(FileName);
        book.Close();
        engine.Dispose();
    }

    /// <summary>
    /// A method that exports specified range to excel from GridModel object passed in.
    /// </summary>
    /// <param name="range">Range of type <see cref="GridRangeInfo"/> to be exported.</param>
    /// <param name="model">The grid model.</param>
    /// <param name="sheet">Excel sheet to which data is to be exported.</param>
    /// <param name="options">The converter options.</param>
    public void ExportRange(GridRangeInfo range, GridModel model, IWorksheet sheet, ConverterOptions options)
    {
        if (range == GridRangeInfo.Empty)
        {
            MessageBox.Show(MessageStrings[2], MessageStrings[3], MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        switch (range.RangeType)
        {
            case GridRangeInfoType.Cells:
                InternalExportRange(range, model, sheet, options);
                break;
            case GridRangeInfoType.Cols:
                InternalExportRange(GridRangeInfo.Cells(1, range.Left, model.RowCount, range.Right), model, sheet, options);
                break;
            case GridRangeInfoType.Rows:
                InternalExportRange(GridRangeInfo.Cells(range.Top, 1, range.Bottom, model.ColCount), model, sheet, options);
                break;
            case GridRangeInfoType.Table:
                this.GridToExcel(model, sheet, options);
                break;
        }
        if (options == ConverterOptions.ColumnHeaders)
            ExportColumnHeaders(model, sheet);
        else if (options == ConverterOptions.RowHeaders)
            ExportRowHeaders(model, sheet);
    }

    // SF4485 Add support to pass filename as an argument
    /// <summary>
    /// A method that exports specified range to excel from GridModel object passed in.
    /// </summary>
    /// <param name="range">Range of type <see cref="GridRangeInfo"/> to be exported.</param>
    /// <param name="model">The grid model.</param>
    /// <param name="FileName">Name of the file to which the grid is to be exported.</param>
    /// <param name="options">The converter options.</param>
    public void ExportRange(GridRangeInfo range, GridModel model, string FileName, ConverterOptions options)
    {
        if (FileName == null)
            throw new ArgumentNullException("FileName");

        if (FileName.Length == 0)
            throw new ArgumentException("FileName - string can not be empty");

        using (ExcelEngine engine = CreateEngine())
        {
            IWorkbook book = engine.Excel.Workbooks.Create(1);
            IWorksheet sheet = book.Worksheets[0];

            ExportRange(range, model, sheet, options);

            book.Close(true, FileName);
        }
    }

    private void InternalExportRange(GridRangeInfo range, GridModel model, IWorksheet sheet, ConverterOptions options)
    {
        if (iStartColIndex < range.Left)
            iStartColIndex = range.Left;
        if (iEndColIndex < range.Right)
            iEndColIndex = range.Right;
        if (iStartRowIndex < range.Top)
            iStartRowIndex = range.Top;
        if (iEndRowIndex < range.Bottom)
            iEndRowIndex = range.Bottom;

        if (options == ConverterOptions.RowHeaders)
        {
            for (int i = range.Top, iRowRange = sheet.UsedRange.LastRow + 1; i <= range.Bottom; i++, iRowRange++)
            {
                for (int j = range.Left, iColRange = 2; j <= range.Right; j++, iColRange++)
                {
                    this.CopyStyle(model[i, j], sheet[iRowRange, iColRange]);
                    CopyFormat(sheet[iRowRange, iColRange], model[i, j]);
                }
            }
        }
        else
        {
            for (int i = range.Top, iRowRange = sheet.UsedRange.LastRow + 1; i <= range.Bottom; i++, iRowRange++)
            {
                for (int j = range.Left, iColRange = 1; j <= range.Right; iColRange++)
                {

                    int iGridRow = i;
                    int iGridCol = j;

                    GridStyleInfo gridCell = GetGridCellStyle(model, iGridRow, iGridCol);//grid[ iGridRow, iGridCol ];

                    GridRangeInfo gridRange = model.CoveredRanges.FindRange(iGridRow, iGridCol);

                    IRange xlRange = xlRange = sheet.Range[i, j, i + gridRange.Height - 1, j + gridRange.Width - 1];

                    if (gridRange.Height > 1 || gridRange.Width > 1)
                    {
                        if (gridRange.Top < iGridRow)
                        {
                            j += gridRange.Width;
                            continue;
                        }
                        xlRange.Merge();
                    }
                    j += gridRange.Width;
                    GridCellToExcel(model, iGridRow, iGridCol, xlRange);
                }
            }
        }
    }

    private void CopyFormat(IRange range, GridStyleInfo gridCell)
    {
      switch( gridCell.CellType )
      {
          //case "MonthCalendar":
          case "Image":
              if (ExportImage && ExportStyle)
                  ExportImageToExcelCell(range, gridCell);
              else
                  range.Text = gridCell.FormattedText;
              break;
          case "PushButton":
          case "CheckBox":
          case "RadioButton":
          range.Text = gridCell.Description;
          break;
          case "ComboBox":
          case "RichText":
          range.Text = gridCell.FormattedText;
          break;

          case "ProgressBar":
          GridProgressBarInfo progressBar = gridCell.ProgressBar;
          int iValue = progressBar.ProgressValue;

          if( progressBar.TextStyle == ProgressBarTextStyles.Percentage )
          {

            int iMin = progressBar.Minimum;
            int iMax = progressBar.Maximum;
            int iRange = iMax - iMin;
            int iPercent = Math.Min( 100, iValue * 100 / iRange );
            range.Number = iPercent / 100.0;
            range.NumberFormat = "0%";
          }
          else
          {
            range.Number = iValue;
          }
          break;

          case "Currency":

          System.Globalization.NumberFormatInfo nfi = (gridCell != null) ? gridCell.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
          Double val;
          if (gridCell.CellValue.ToString() != string.Empty)
          {
              val = Convert.ToDouble(gridCell.CellValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
              range.Number = val;
          }
          else
              val = 0;

          string valString = val.ToString("C", nfi);
          String FormatString = string.Empty;
          if (gridCell.CurrencyEdit.CurrencyDecimalDigits.Equals(0))
              FormatString = "#,##0";
          else
              FormatString = "#,##0.";
          for (int i = 0; i < nfi.CurrencyDecimalDigits; i++)
              FormatString += "0";
          if (Char.IsNumber(valString, 0))
              FormatString += " [$" + nfi.CurrencySymbol + "-411]";
          else
              FormatString = "[$" + nfi.CurrencySymbol + "-411]" + FormatString;
          range.CellStyle.NumberFormat = FormatString;
          break;

        default:
          object objValue = gridCell.CellValue;
          
          if( objValue is DateTime )
          {
              if (string.IsNullOrEmpty(gridCell.Format))
                  range.DateTime = (DateTime)objValue;
              else
              {
                  range.Text = gridCell.FormattedText;
                  range.NumberFormat = gridCell.Format;
              }
          }
          else if( objValue is string )
          {
            string strText = gridCell.Text;
            double value = 0;

                  if (gridCell.CellType == GridCellTypeName.FormulaCell && strText.Length != 0
              && gridCell.Text[ 0 ] == '=' )
            {
                this.sheetNames = new ArrayList();
                if (range.Worksheet.Workbook != null)
                    sheetNames.Add(range.Worksheet.Name);
				//Fix for defect:#13685.
                //range.Formula = MakeUpper(strText);                 
				range.Formula = strText;
            }
            else if (double.TryParse(strText, out value) && string.IsNullOrEmpty(gridCell.Format) && strText.ToUpper().IndexOf('E') != -1)
            {
                range.NumberFormat = "text";
                range.Text = strText;
                range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText; //ignore error
            }
            else
            {
                if (gridCell.CellValueType == null || gridCell.CellValueType == typeof(string))
                    range.Text = strText;
                else
                    range.Value2 = objValue;
            }
          }
          else if (gridCell.CellValueType == typeof(string))
          {
              range.Text = gridCell.Text;
          }
          else if (objValue is ulong || objValue is ushort)
          {
              range.Text = objValue.ToString();
              range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText;
          }
          else
          {
            range.Value2 = objValue;
          }
          break;
      }      
    }

    private void ExportColumnHeaders(GridModel model, IWorksheet sheet)
    {
        sheet.InsertRow(1);
        for (int j = iStartColIndex, iColRange = 1; j <= iEndColIndex; j++, iColRange++)
        {
            this.CopyStyle(model[0, j], sheet[1, iColRange]);
            sheet[1, iColRange].Text = model[0, j].FormattedText;
        }
    }

    private void ExportRowHeaders(GridModel model, IWorksheet sheet)
    {
        sheet.InsertColumn(1);
        for (int j = iStartRowIndex; j <= iEndRowIndex; j++, iRowRange++)
        {
            this.CopyStyle(model[j, 0], sheet[iRowRange, 1]);
            sheet[iRowRange, 1].Text = model[j, 0].FormattedText;
        }
    }

    #endregion

    /// <summary>
    /// Converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="fileName">File to export into.</param>
    public void GridToExcel( GridControl grid, string fileName )
    {
      GridToExcel( grid, fileName, ConverterOptions.Default );
    }
    /// <summary>
    /// Converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="fileName">File to export into.</param>
    /// <param name="options">Converter options.</param>
    public void GridToExcel( GridControl grid, string fileName, ConverterOptions options )
    {
      GridToExcel( grid.Model, fileName, options );
    }
    /// <summary>
    /// A method that converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    public void GridToExcel( GridControl grid, IWorksheet sheet )
    {
      GridToExcel( grid, sheet, ConverterOptions.Default );
    }
    /// <summary>
    /// A method that converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="options">Converter options.</param>
    public void GridToExcel( GridControl grid, IWorksheet sheet, ConverterOptions options )
    {
      GridToExcel( grid.Model, sheet, options );
    }
    /// <summary>
    /// A method that converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="fileName">File to export into.</param>
    public void GridToExcel( GridModel grid, string fileName )
    {
      GridToExcel( grid, fileName, ConverterOptions.Default );
    }
    /// <summary>
    /// A method that converts grid into excel file.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="fileName">File to export into.</param>
    /// <param name="options">Converter options.</param>
    public void GridToExcel( GridModel grid, string fileName, ConverterOptions options )
    {
        using (ExcelEngine engine = CreateEngine())
        {
            IWorkbook book = engine.Excel.Workbooks.Create(1);
            IWorksheet sheet = book.Worksheets[0];

            GridToExcel(grid, sheet, options);

            book.Close(true, fileName);
        }
    }
    /// <summary>
    /// A method that converts grid into excel worksheet.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    public void GridToExcel( GridModel grid, IWorksheet sheet )
    {
      GridToExcel( grid, sheet, ConverterOptions.Default );
    }
    /// <summary>
    /// A method that converts grid into excel worksheet.
    /// </summary>
    /// <param name="grid">Grid to export.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="options">Convert options.</param>
    public void GridToExcel( GridModel grid, IWorksheet sheet, ConverterOptions options )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );
      Grid = grid;
      int iColumnDelta = ( ( options & ConverterOptions.RowHeaders ) != 0 ) ? 1 : 0;
      int iRowDelta = ( ( options & ConverterOptions.ColumnHeaders ) != 0 ) ? 1 : 0;

      //CopyMergesFromGrid( grid, sheet, options );

      if( !m_bAutoFitRows )   CopyRowHeightFromGrid( grid, sheet, 1 - iRowDelta );
      if( !m_bAutoFitColumns )CopyColumnWidthFromGrid( grid, sheet, 1 - iColumnDelta );
      else
      {
          int MaxColumn = Math.Min(grid.ColCount, DEF_MAX_COLUMN_ONE_INDEX + iColumnDelta - 1);
          for (int i = 1 - iColumnDelta; i <=MaxColumn; i++)
          {
              if (grid.HideCols[i] )
              {
                  sheet.Range.Worksheet.ShowColumn(i, false);
              }
          }
      }
      int iFrozenRows = grid.Rows.FrozenCount;
      int iFrozenCols = grid.Cols.FrozenCount;

      if( iFrozenRows > 0 || iFrozenCols > 0 )
      {
        sheet.Range[ iFrozenRows + 1, iFrozenCols + 1 ].FreezePanes();
      }

      int iMaxRow = Math.Min(DEF_MAX_ROW_ONE_INDEX, grid.RowCount + iRowDelta );

      int iMaxColumn = Math.Min( DEF_MAX_COLUMN_ONE_INDEX, grid.ColCount + iColumnDelta );

      if (sheet.Application.DefaultVersion != ExcelVersion.Excel97to2003)
      {
          iMaxRow = Math.Min(DEF_EXCEL2007_MAX_ROW_COUNT, grid.RowCount + iRowDelta);
          iMaxColumn = Math.Min(DEF_EXCEL2007_MAX_COLUMN_COUNT, grid.ColCount + iColumnDelta);
      }

#if ( !ASPNET && MEASURE_PERFORMANCE )
      DateTime start = DateTime.Now;
#endif

      this.sheetNames = new ArrayList();
      if (sheet.Workbook != null)
      {
          foreach (IWorksheet sheet1 in sheet.Workbook.Worksheets)
              sheetNames.Add(sheet1.Name);
          sheetNames.Sort(new GridFormulaEngine.LenComparer());
      }

      // TODO: Optimization is needed
      if ((options & ConverterOptions.Visible)== ConverterOptions.Visible)
      {
          DateTime start = DateTime.Now;
          for (int iRow = 1, row = 1; iRow <= iMaxRow; iRow++, row++)
          {
              if (DateTime.Now.Subtract(start).TotalSeconds > 3)
              {
                  Application.DoEvents();
                  start = DateTime.Now;
              } 
              for (int iColumn = 1,column = 1; iColumn <= iMaxColumn;)
              {
                  int iGridRow = iRow - iRowDelta;
                  int iGridCol = iColumn - iColumnDelta;

                  GridStyleInfo gridCell = GetGridCellStyle(grid, iGridRow, iGridCol);//grid[ iGridRow, iGridCol ];
                  GridRangeInfo gridRange = grid.CoveredRanges.FindRange(iGridRow, iGridCol);

                  IRange xlRange = xlRange = sheet.Range[iRow, iColumn, iRow + gridRange.Height - 1, iColumn + gridRange.Width - 1];
                  IRange range2 = sheet.Range[row, column, row + gridRange.Height - 1, column + gridRange.Width - 1];

                  if (xlRange.RowHeight != 0)
                  {
                      if (gridRange.Height > 1 || gridRange.Width > 1)
                      {
                          if (gridRange.Top < iGridRow)
                          {
                              iColumn += gridRange.Width;
                              continue;
                          }
                          xlRange.Merge();
                      }
                      if (xlRange.ColumnWidth != 0)
                      {
                          // To skip the hidden column 
                          iColumn++;
                          column++;

                          // exports hidden column data to Excel
                          GridCellToExcel(grid, iGridRow, iGridCol, range2);
                      }
                      else if(xlRange.ColumnWidth == 0)
                      {
                          // To skip the hidden column
                          iColumn++;
                      }
                      if (iRow == iMaxRow)
                      {
                          // To show the column hidden in the Grid
                          xlRange.Worksheet.ShowColumn(xlRange.Column, true);
                      }
                  }

                  else
                  {
                      iRow++;
                      xlRange.Worksheet.ShowRow(xlRange.Row, true);
                  }
              }
          }
      }

      else
      {
          DateTime start = DateTime.Now;
          for (int iRow = 1; iRow <= iMaxRow; iRow++)
          {
              if (DateTime.Now.Subtract(start).TotalSeconds > 3)
              {
                  Application.DoEvents();
                  start = DateTime.Now;
              } 
              for (int iColumn = 1; iColumn <= iMaxColumn; )
              {
                  int iGridRow = iRow - iRowDelta;
                  int iGridCol = iColumn - iColumnDelta;

                  GridStyleInfo gridCell = GetGridCellStyle(grid, iGridRow, iGridCol);//grid[ iGridRow, iGridCol ];

                  GridRangeInfo gridRange = grid.CoveredRanges.FindRange(iGridRow, iGridCol);

                  IRange xlRange = xlRange = sheet.Range[iRow, iColumn, iRow + gridRange.Height - 1, iColumn + gridRange.Width - 1];

                  if (gridRange.Height > 1 || gridRange.Width > 1)
                  {
                      if (gridRange.Top < iGridRow)
                      {
                          iColumn += gridRange.Width;
                          continue;
                      }
                      xlRange.Merge();
                  }
                  iColumn += gridRange.Width;
                  GridCellToExcel(grid, iGridRow, iGridCol, xlRange);

              }
          }
      }

      this.sheetNames = null;

      if( m_bAutoFitColumns )sheet.UsedRange.AutofitColumns();
      if( m_bAutoFitRows )sheet.UsedRange.AutofitRows();

#if ( !ASPNET && MEASURE_PERFORMANCE )
      TimeSpan totalTime = DateTime.Now - start;
      Debug.WriteLine( string.Format( "Total time of iteration through cells is {0}", totalTime ) );
#endif
    }

      //used to make names invariant for capitalization
    private ArrayList sheetNames = null;

    /// <summary>
    /// Copies merged ranges.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="options">Converter options.</param>
    private void CopyMergesFromGrid( GridModel grid, IWorksheet sheet, ConverterOptions options )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      GridModelCoveredRanges coveredRanges = grid.CoveredRanges;
      GridRangeInfoList ranges = coveredRanges.Ranges;

      for( int i = 0, len = ranges.Count; i < len; i++ )
      {
        GridRangeInfo rangeInfo = ranges[ i ];
        Debug.WriteLine( i, "Range index" );

        string strMessage = string.Format( "({0}, {1})-({2}, {3})", rangeInfo.Top, rangeInfo.Left, rangeInfo.Bottom, rangeInfo.Right );
        Debug.WriteLine( strMessage, "Range rect" );

        if( rangeInfo.Top == 0 || rangeInfo.Left == 0 ) continue;

        sheet.Range[ rangeInfo.Top, rangeInfo.Left, rangeInfo.Bottom, rangeInfo.Right ].Merge();
      }
    }
    /// <summary>
    /// Copies one grid cell into excel cell.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="iRow">Grid cell row index to copy.</param>
    /// <param name="iColumn">Grid cell column index to copy.</param>
    /// <param name="range">Destination excel range.</param>
    public virtual void GridCellToExcel(GridModel grid, int iRow, int iColumn, IRange range)
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( range == null )
        throw new ArgumentNullException( "range" );

    GridStyleInfo gridCell = GetGridCellStyle(grid, iRow, iColumn);//grid[ iRow, iColumn ]

      //if( gridCell.IsEmpty && !(gridCell.BaseStyle == GridCellTypeName.Header
      //    || gridCell.CellType == GridCellTypeName.Header)) return;
        
    //Force Alignment
    gridCell.VerticalAlignment = gridCell.VerticalAlignment;
    gridCell.HorizontalAlignment = gridCell.HorizontalAlignment;

      GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(range, gridCell, GridConverterAction.Export, iRow, iColumn);
      RaiseQueryImportExportCellInfo(e);
      if (e.Handled)
          return;

      switch( gridCell.CellType )
      {
          //case "MonthCalendar":
          case "Image":
              if (ExportImage && ExportStyle)
                  ExportImageToExcelCell(range, gridCell);
              else
                  range.Text = gridCell.FormattedText;
              break;
          case "PushButton":
          case "CheckBox":
          case "RadioButton":
          range.Text = gridCell.Description;
          break;
          case "ComboBox":
          case "RichText":
          range.Text = gridCell.FormattedText;
          break;

          case "ProgressBar":
          GridProgressBarInfo progressBar = gridCell.ProgressBar;
          int iValue = progressBar.ProgressValue;

          if( progressBar.TextStyle == ProgressBarTextStyles.Percentage )
          {

            int iMin = progressBar.Minimum;
            int iMax = progressBar.Maximum;
            int iRange = iMax - iMin;
            int iPercent = Math.Min( 100, iValue * 100 / iRange );
            range.Number = iPercent / 100.0;
            range.NumberFormat = "0%";
          }
          else
          {
            range.Number = iValue;
          }
          break;

          case "Currency":

          System.Globalization.NumberFormatInfo nfi = (gridCell != null) ? gridCell.CurrencyEdit.NumberFormatInfoObject : System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
          Double val;
          if (gridCell.CellValue.ToString() != string.Empty)
          {
              val = Convert.ToDouble(gridCell.CellValue, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
              range.Number = val;
          }
          else
              val = 0;

          string valString = val.ToString("C", nfi);
          String FormatString = string.Empty;
          if (gridCell.CurrencyEdit.CurrencyDecimalDigits.Equals(0))
              FormatString = "#,##0";
          else
              FormatString = "#,##0.";
          for (int i = 0; i < nfi.CurrencyDecimalDigits; i++)
              FormatString += "0";
          if (Char.IsNumber(valString, 0))
              FormatString += " [$" + nfi.CurrencySymbol + "-411]";
          else
              FormatString = "[$" + nfi.CurrencySymbol + "-411]" + FormatString;
          range.CellStyle.NumberFormat = FormatString;
          break;

        default:
          object objValue = gridCell.CellValue;
          
          if( objValue is DateTime )
          {
              if (string.IsNullOrEmpty(gridCell.Format))
                  range.DateTime = (DateTime)objValue;
              else
              {
                  range.Text = gridCell.FormattedText;
                  range.NumberFormat = gridCell.Format;
              }
          }
          else if( objValue is string )
          {
            string strText = gridCell.Text;
            double value = 0;

                  if (gridCell.CellType == GridCellTypeName.FormulaCell && strText.Length != 0
              && gridCell.Text[ 0 ] == '=' )
            {
                this.sheetNames = new ArrayList();
                if (range.Worksheet.Workbook != null)
                    sheetNames.Add(range.Worksheet.Name);
				//Fix for defect:#13685.
                //range.Formula = MakeUpper(strText);                 
				range.Formula = strText;
            }
            else if (double.TryParse(strText, out value) && string.IsNullOrEmpty(gridCell.Format) && strText.ToUpper().IndexOf('E') != -1)
            {
                range.NumberFormat = "text";
                range.Text = strText;
                range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText; //ignore error
            }
            else
            {
                if ((gridCell.CellValueType == null || gridCell.CellValueType == typeof(string)) && objValue != null && (objValue.ToString() != "TRUE" || objValue.ToString() != "FALSE"))
                    range.Text = strText;
                else
                    range.Value2 = objValue;
            }
          }
          else if (gridCell.CellValueType == typeof(string))
          {
              range.Text = gridCell.Text;
          }
          else if (objValue is ulong || objValue is ushort)
          {
              range.Text = objValue.ToString();
              range.IgnoreErrorOptions = ExcelIgnoreError.NumberAsText;
          }
          else
          {
            range.Value2 = objValue;
          }
          break;
      }

        if(ExportStyle)
            CopyStyles(grid, iRow, iColumn, gridCell, range);
            //CopyStyles( grid, iRow, iColumn, range );
    }

      //GridStyleInfo cacheStyleInfo;
      private GridStyleInfo GetGridCellStyle(int row, int col)
      {
          return GetGridCellStyle(Grid, row, col);
      }
      private GridStyleInfo GetGridCellStyle(GridModel grid, int row, int col)
      {
          return grid.ActiveGridView.GetViewStyleInfo(row, col);
      }

      /// <summary>
      /// Add the custom function in excel work book while exporting
      /// </summary>
      /// <param name="xlafile">file path of the user define function.</param>
      /// <param name="functionName">Function name of user define function</param>
      /// <param name="book">excel work book which is used for exporting</param>
      public void AddCustomFunction(string xlafile, string functionName, IWorkbook book)
      {
          IAddInFunctions unknownFunctions = book.AddInFunctions;
          unknownFunctions.Add(xlafile, functionName);
      }

      /// <summary>
      /// Add the custom function in excel file while exporting
      /// </summary>
      /// <param name="xlafile">file path of the user define function.</param>
      /// <param name="functionName">Function name of user define function</param>
      /// <param name="excelfile">file name of excel book</param>
      public void AddCustomFunction(string xlafile, string functionName, string excelfile)
      {
          using (ExcelEngine engine = CreateEngine())
          {
              IWorkbook book = engine.Excel.Workbooks.Create(1);
              AddCustomFunction(xlafile, functionName, book);
              book.SaveAs(excelfile);
              book.Close();
              Process.Start(xlafile);
          }
      }

    //sheetNames are cached in ConvertExcelRangeToGrid
      private string MakeUpper(string strText)
      {
          string s = strText;
          Hashtable save = SaveStrings(ref s);
          s = s.ToUpper();
          if(save != null)
            SetStrings(ref s, save);
        foreach (string name in this.sheetNames)
        {
            string upper = name.ToUpper() + "!";
            string regular = name + "!";
            s = s.Replace(upper, regular);
        }     
          return s;
      }

      #region String Support
 
      private Hashtable SaveStrings(ref string text)
      {
          Hashtable strings = null;
            
           if (text.IndexOf("\"") > -1)
           {      
               string TIC = "'";
               string TICs2 = TIC + TIC;
               text = text.Replace("\"", TIC);
               int id = 0;
               int i = -1;
               if ((i = text.IndexOf(TIC)) > -1)
               {
                   while (i > -1 && i < text.Length)
                   {
                       if (strings == null)
                           strings = new Hashtable();

                       int j = (i + 1) < text.Length ? text.IndexOf(TIC, i + 1) : -1;
                       if (j > -1)
                       {
                           string key = TIC + id.ToString() + TIC;
                           if (j < text.Length - 2 && text[j + 1] == TIC[0])
                           {
                               j = text.IndexOf(TIC, j + 2);
                               if (j == -1)
                                   throw new ArgumentException("mismatched_tics");
                           }

                           string s = text.Substring(i, j - i + 1);
                           strings.Add(key, s);
                           s = s.Replace(TICs2, TIC);
                           id++;
                           text = text.Substring(0, i) + key + text.Substring(j + 1);
                           i = i + key.Length;
                           if (i < text.Length)
                               i = text.IndexOf(TIC, i);
                       }
                       else
                       {
                           throw new ArgumentException("mismatched_tics");
                       }
                   }
               }
           }
          return strings;
      }

      private void SetStrings(ref string retValue, Hashtable strings)
      {
          foreach (string s in strings.Keys)
          {
              retValue = retValue.Replace(s, (string)strings[s]);
          }
          retValue = retValue.Replace("'", "\"");
         
      }
      #endregion
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether converter should autofit columns.
    /// </summary>
    public bool AutoFitColumns
    {
      get
      {
        return m_bAutoFitColumns;
      }
      set
      {
        m_bAutoFitColumns = value;
      }
    }
    /// <summary>
    /// Indicates whether converter should autofit rows.
    /// </summary>
    public bool AutoFitRows
    {
      get
      {
          return m_bAutoFitRows;
      }
      set
      {
        m_bAutoFitRows = value;
      }
    }

    #endregion
  }
}
