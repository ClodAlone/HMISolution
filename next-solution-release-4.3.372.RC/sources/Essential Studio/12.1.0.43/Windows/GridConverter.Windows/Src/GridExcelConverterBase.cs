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
using Syncfusion.Drawing;
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Syncfusion.GridExcelConverter
{
  /// <summary>
  /// GridExcelConverterBase is a base class for specific Grid to Excel Converter controls.
  /// Provides Support for Importing the Grid with data's from the spreadsheet.
  /// </summary>
  [ ToolboxItem( false ) ]
  public class GridExcelConverterBase : System.ComponentModel.Component
  {
    #region Class constants
    private const char BMARKER = 'b';
    /// <summary>
    /// General format.
    /// </summary>
    protected const string DEF_GENERAL_FORMAT = "GENERAL";
    /// <summary>
    /// Default number format in Grid cell
    /// </summary>
    protected const string DEF_NUMBER_FORMAT = "0";
      /// <summary>
      /// Gets the number format in Excel
      /// </summary>
    protected int[] DEF_EXCEL_NUMBER_FORMATS = new int[] { 2, 173};
    ///// <summary>
    ///// Indicates that cell contains image.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_IMAGE = "Image";
    ///// <summary>
    ///// Indicates that cell contains currency value.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_CURRENCY = "Currency";
    ///// <summary>
    ///// Indicates that cell contains text value.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_TEXT = "TextBox";
    ///// <summary>
    ///// Indicates that cell contains combobox.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_COMBOBOX = "ComboBox";
    ///// <summary>
    ///// Indicates that cell contains progress bar.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_PROGRESSBAR = "ProgressBar";
    ///// <summary>
    ///// Indicates that cell contains formula.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_FORMULA = "FormulaCell";
    ///// <summary>
    ///// Indicates that cell contains RichText.
    ///// </summary>
    //protected const string DEF_CELL_TYPE_RTF = "RichText";
    /// <summary>
    /// General format in Excel.
    /// </summary>
    protected const int DEF_EXCEL_GENERAL_FORMAT = 0;
    /// <summary>
    /// Currency format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_CURRENCY_FORMAT = 7;
    /// <summary>
    /// Decimal format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_DECIMAL_FORMAT = 4;
    /// <summary>
    /// Scientific format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_SCIENTIFIC_FORMAT = 11;
    /// <summary>
    /// Fixed-point format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_FIXEDPOINT_FORMAT = 4;
    /// <summary>
    /// Number format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_NUMBER_FORMAT = 4;
    /// <summary>
    /// Percent format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_PERCENT_FORMAT = 9;
    /// <summary>
    /// Date format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_DATE_FORMAT = 14;
    /// <summary>
    /// Hex format index in the MS Excel.
    /// </summary>
    protected const int DEF_EXCEL_HEX_FORMAT = 0;
    /// <summary>
    /// Maximum zero-based index of the column in MS Exce.
    /// </summary>
    protected const int DEF_MAX_COLUMN_ZERO_INDEX = 255;
    /// <summary>
    /// Maximum one-based index of the column in MS Excel.
    /// </summary>
    public const int DEF_MAX_COLUMN_ONE_INDEX = DEF_MAX_COLUMN_ZERO_INDEX + 1;
    /// <summary>
    /// Maximum zero-based index of the row in MS Excel.
    /// </summary>
    public const int DEF_MAX_ROW_ZERO_INDEX = ushort.MaxValue;
    /// <summary>
    /// Maximum one-based index of the row in MS Excel.
    /// </summary>
    public const int DEF_MAX_ROW_ONE_INDEX = DEF_MAX_ROW_ZERO_INDEX + 1;
    /// <summary>
    /// Maximum one-based index of the column in MS Excel 2007.
    /// </summary>
    public const int DEF_EXCEL2007_MAX_COLUMN_COUNT = 16384;
    /// <summary>
    /// Maximum one-based index of the row in MS Excel 2007.
    /// </summary>
    public const int DEF_EXCEL2007_MAX_ROW_COUNT = 1048576;

    /// <summary>
    /// Currency format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_CURRENCY = 'C';
    /// <summary>
    /// Date format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_DATE = 'D';
    /// <summary>
    /// Sceintific format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_SCIENTIFIC = 'E';
    /// <summary>
    /// Fixed point format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_FIXEDPOINT = 'F';
    /// <summary>
    /// General format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_GENERAL = 'G';
    /// <summary>
    /// Number format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_NUMBER = 'N';
    /// <summary>
    /// Percent format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_PERCENT = 'P';
    /// <summary>
    /// Hexadecimal format in the .Net.
    /// </summary>
    protected const char DEF_CHAR_HEX = 'X';
    /// <summary>
    /// Default horizontal alignment.
    /// </summary>
    protected const ExcelHAlign DEF_DEFAULT_HALIGNMENT = ExcelHAlign.HAlignLeft;
    /// <summary>
    /// Default vertical alignment.
    /// </summary>
    protected const ExcelVAlign DEF_DEFAULT_VALIGNMENT = ExcelVAlign.VAlignTop;
    /// <summary>
    /// Right angle value.
    /// </summary>
    protected const int DEF_RIGHT_ANGLE = 90;
    /// <summary>
    /// Maximum possible angle value.
    /// </summary>
    protected const int DEF_MAXIMUM_ANGLE = 360;
    /// <summary>
    /// Angle that corresponds to 90 rotation value in the XlsIO.
    /// </summary>
    protected const int DEF_MINIMUM_SUPPORTED_ANGLE = DEF_MAXIMUM_ANGLE - DEF_RIGHT_ANGLE;
    /// <summary>
    /// Color mask to remove Alpha component.
    /// </summary>
    private const int DEF_COLOR_MASK = 0xFFFFFF;
    /// <summary>
    /// Default color (white).
    /// </summary>
    private const int DEFAULT_COLOR = 0xFFFFFF;
    /// <summary>
    /// Excel default color.
    /// </summary>
    private Color EXCEL_DEFAULT_COLOR = Color.FromArgb(255, 51, 51, 51);

    /// <summary>
    /// Transparent Color
    /// </summary>
    private Color TRANSPARENT_COLOR = Color.FromArgb(255, 255, 255, 255);

    private const char ZMARKER = 'z';
    /// <summary>
    /// Pattern style with corresponding ExcelPattern value.
    /// </summary>
    private static readonly DictionaryEntry[] DEF_PATTERN_STYLE = new DictionaryEntry[]
    {
      new DictionaryEntry( PatternStyle.BackwardDiagonal,     ExcelPattern.LightUpwardDiagonal ),
      new DictionaryEntry( PatternStyle.Cross,                ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.DarkDownwardDiagonal, ExcelPattern.DarkDownwardDiagonal ),
      new DictionaryEntry( PatternStyle.DarkHorizontal,       ExcelPattern.DarkHorizontal ),
      new DictionaryEntry( PatternStyle.DarkUpwardDiagonal,   ExcelPattern.DarkUpwardDiagonal ),
      new DictionaryEntry( PatternStyle.DashedVertical,       ExcelPattern.Vertical ),
      new DictionaryEntry( PatternStyle.DashedHorizontal,     ExcelPattern.Horizontal ),
      new DictionaryEntry( PatternStyle.DiagonalBrick,        ExcelPattern.LightUpwardDiagonal ),
      new DictionaryEntry( PatternStyle.DiagonalCross,        ExcelPattern.Percent60 ),
      new DictionaryEntry( PatternStyle.Divot,                ExcelPattern.Percent05 ),
      new DictionaryEntry( PatternStyle.DottedDiamond,        ExcelPattern.Percent10 ),
      new DictionaryEntry( PatternStyle.DottedGrid,           ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.ForwardDiagonal,      ExcelPattern.ForwardDiagonal ),
      new DictionaryEntry( PatternStyle.Horizontal,           ExcelPattern.Horizontal ),
      new DictionaryEntry( PatternStyle.HorizontalBrick,      ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.LargeCheckerBoard,    ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.LargeConfetti,        ExcelPattern.Percent05 ),
      new DictionaryEntry( PatternStyle.LightDownwardDiagonal,ExcelPattern.LightDownwardDiagonal ),
      new DictionaryEntry( PatternStyle.LightHorizontal,      ExcelPattern.Horizontal ),
      new DictionaryEntry( PatternStyle.LightUpwardDiagonal,  ExcelPattern.LightUpwardDiagonal ),
      new DictionaryEntry( PatternStyle.LightVertical,        ExcelPattern.Vertical ),
      new DictionaryEntry( PatternStyle.NarrowHorizontal,     ExcelPattern.Horizontal ),
      new DictionaryEntry( PatternStyle.NarrowVertical,       ExcelPattern.Vertical ),
      new DictionaryEntry( PatternStyle.None,                 ExcelPattern.None ),
      new DictionaryEntry( PatternStyle.OutlinedDiamond,      ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Percent05,            ExcelPattern.Percent05 ),
      new DictionaryEntry( PatternStyle.Percent10,            ExcelPattern.Percent10 ),
      new DictionaryEntry( PatternStyle.Percent20,            ExcelPattern.Percent25 ),
      new DictionaryEntry( PatternStyle.Percent25,            ExcelPattern.Percent25 ),
      new DictionaryEntry( PatternStyle.Percent30,            ExcelPattern.Percent25 ),
      new DictionaryEntry( PatternStyle.Percent40,            ExcelPattern.Percent50 ),
      new DictionaryEntry( PatternStyle.Percent50,            ExcelPattern.Percent50 ),
      new DictionaryEntry( PatternStyle.Percent60,            ExcelPattern.Percent60 ),
      new DictionaryEntry( PatternStyle.Percent70,            ExcelPattern.Percent70 ),
      new DictionaryEntry( PatternStyle.Percent75,            ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Percent80,            ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Percent90,            ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Plaid,                ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.Shingle,              ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.SmallCheckerBoard,    ExcelPattern.ForwardDiagonal ),
      new DictionaryEntry( PatternStyle.SmallConfetti,        ExcelPattern.Percent10 ),
      new DictionaryEntry( PatternStyle.SmallGrid,            ExcelPattern.Angle ),
      new DictionaryEntry( PatternStyle.SolidDiamond,         ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Sphere,               ExcelPattern.ForwardDiagonal ),
      new DictionaryEntry( PatternStyle.Trellis,              ExcelPattern.Percent75 ),
      new DictionaryEntry( PatternStyle.Vertical,             ExcelPattern.Vertical ),
      new DictionaryEntry( PatternStyle.Wave,                 ExcelPattern.ForwardDiagonal ),
      new DictionaryEntry( PatternStyle.Weave,                ExcelPattern.ForwardDiagonal ),
      new DictionaryEntry( PatternStyle.WideDownwardDiagonal, ExcelPattern.LightDownwardDiagonal ),
      new DictionaryEntry( PatternStyle.WideUpwardDiagonal,   ExcelPattern.LightUpwardDiagonal ),
      new DictionaryEntry( PatternStyle.ZigZag,               ExcelPattern.Horizontal ),
    };
    /// <summary>
    /// PatternStyle - to - ExcelPattern hashtable.
    /// </summary>
    protected static readonly Hashtable PatternStyleToExcelPattern = new Hashtable( DEF_PATTERN_STYLE.Length );
    /// <summary>
    /// Array with default currency number formats.
    /// </summary>
    private static readonly int[] DEF_CURRENCY_NUMBER_FORMATS = new int[]
    {
      //Look at Syncfusion.XlsIO.Implementation.Collections.FormatsCollection 
      //InsertDefaultFormats() method for format indices
      5, 6, 7, 8, 0x2a, 0x2c,
    };
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static GridExcelConverterBase()
    {
      for( int i = 0, len = DEF_PATTERN_STYLE.Length; i < len; i++ )
      {
        DictionaryEntry entry = DEF_PATTERN_STYLE[ i ];
        PatternStyleToExcelPattern.Add( entry.Key, entry.Value );
      }
    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public GridExcelConverterBase()
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates excel engine.
    /// </summary>
    /// <returns>Newly created ExcelEngine.</returns>
    /// <remarks>
    /// The caller of this method is responsible for calling
    /// Dispose on the created ExcelEngine after all workbooks
    /// have been closed and the caller is finished with the engine.
    /// </remarks>
    protected ExcelEngine CreateEngine()
    {
      ExcelEngine engine = new ExcelEngine();
      engine.ThrowNotSavedOnDestroy = false;
      return engine;
    }
    /// <summary>
    /// Copies row height settings from grid model into excel worksheet.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    protected void CopyRowHeightFromGrid( GridModel grid, IWorksheet sheet )
    {
      CopyRowHeightFromGrid( grid, sheet, 1 );
    }
    /// <summary>
    /// Copies row height settings from grid model into excel worksheet.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="iStartGridRow">Start row in the grid.</param>
    protected void CopyRowHeightFromGrid( GridModel grid, IWorksheet sheet, int iStartGridRow )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int iMaxRow = Math.Min( grid.RowCount, DEF_MAX_ROW_ONE_INDEX + iStartGridRow - 1 );

      GridModelRowColSizeIndexer arrHeights = grid.RowHeights;

      for( int i = iStartGridRow; i <= iMaxRow; i++ )
      {
        int iRowHeight = arrHeights[ i ];
        sheet.SetRowHeightInPixels( i - iStartGridRow + 1, iRowHeight );
      }
    }
    /// <summary>
    /// Copies column width settings from grid model into excel worksheet.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    protected void CopyColumnWidthFromGrid( GridModel grid, IWorksheet sheet )
    {
      CopyColumnWidthFromGrid( grid, sheet, 1 );
    }
    /// <summary>
    /// Copies column width settings from grid model into excel worksheet.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="iStartGridColumn">Start column in the grid.</param>
    protected void CopyColumnWidthFromGrid( GridModel grid, IWorksheet sheet, int iStartGridColumn )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );
      GridModelRowColSizeIndexer arrWidth = grid.ColWidths;
      int iMaxColumn = Math.Min( grid.ColCount, DEF_MAX_COLUMN_ONE_INDEX + iStartGridColumn - 1 );

      for( int i = iStartGridColumn; i <= iMaxColumn; i++ )
      {
        int iWidthInPixels = arrWidth[ i ];
        double dWidth = sheet.PixelsToColumnWidth( iWidthInPixels );
        sheet.SetColumnWidth( i - iStartGridColumn + 1, dWidth );
      }
    }
      /// <summary>
      /// Copies the styles
      /// </summary>
    /// <param name="grid">GridModel</param>
      /// <param name="iRow">int</param>
      /// <param name="iColumn">int</param>
    /// <param name="range">IRange</param>
      protected void CopyStyles(GridModel grid, int iRow, int iColumn, IRange range)
      {
          CopyStyles(grid, iRow, iColumn, grid[iRow,iColumn], range);
      }

    /// <summary>
    /// Copies style of grid cell into excel cell.
    /// </summary>
    /// <param name="grid">Source grid.</param>
    /// <param name="iRow">Source cell row index.</param>
    /// <param name="iColumn">Source cell column index.</param>
      /// <param name="gridCell">Style for the grid cell</param>
    /// <param name="range">Destination range.</param>
    protected void CopyStyles( GridModel grid, int iRow, int iColumn, GridStyleInfo gridCell, IRange range )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( range == null )
        throw new ArgumentNullException( "range" );

      GridStyleInfo[] arrStyles = new GridStyleInfo[ 8 ]
      {
        gridCell,//grid[ iRow, iColumn ],
        null,
        grid.RowStyles[ iRow ],
        null,
        grid.ColStyles[ iColumn ],
        null,
        grid.TableStyle,
        null,
      };

      int iStylesCount = arrStyles.Length;
      for( int i = 0; i < iStylesCount; i+= 2 )
      {
        GridStyleInfo style = arrStyles[ i ];

        if( style.HasBaseStyle || !GridUtil.IsEmpty(style.BaseStyle))
        {
          arrStyles[ i + 1 ] = grid.BaseStylesMap[ style.BaseStyle ].StyleInfo;
        }
      }

      //Fix(FR #1207) - Export Column Header Color 
      GridStyleInfo finalGridStyle = new GridStyleInfo( );
        CombineStyles(finalGridStyle, arrStyles[ 0 ] );
        if ((arrStyles[0].CellType == GridCellTypeName.Header || arrStyles[0].BaseStyle == GridCellTypeName.Header))
        {
            if(hasHeaderBKColor)
            finalGridStyle.BackColor = HeaderBackColor;
            if (ExportBorders)
                finalGridStyle.Borders.All = new GridBorder(GridBorderStyle.Solid);
        }

        for (int i = 1; i < iStylesCount; i++)
        {
            GridStyleInfo style = arrStyles[i];

            if (style == null) continue;

            if (!CombineStyles(finalGridStyle, style)) break;
            if (range.IsMerged)
                range.Merge(true);
        }

        if(!finalGridStyle.IsEmpty)
      CopyStyle( finalGridStyle, range );
    }
    /// <summary>
    /// Copies style from grid style into range style.
    /// </summary>
    /// <param name="gridStyle">Grid style to copy.</param>
    /// <param name="destRange">Destination range.</param>
    protected void CopyStyle( GridStyleInfo gridStyle, IRange destRange )
    {
      if( gridStyle == null )
        throw new ArgumentNullException( "gridStyle" );

      if( destRange == null )
        throw new ArgumentNullException( "destRange" );

      CopyBrush( gridStyle, destRange );
      CopyFont( gridStyle, destRange );
      CopyBorders( gridStyle, destRange );
      CopyAlignment( gridStyle, destRange );
      SetNumberFormatIndex( destRange, gridStyle );
    }

    /// <summary>
    /// Sets number format index for a range from specified grid style.
    /// </summary>
    /// <param name="range">Range to add format to.</param>
    /// <param name="gridStyle">Style to get number format from.</param>
    private void SetNumberFormatIndex( IRange range, GridStyleInfo gridStyle )
    {
        //Currency formats needs customizations and are handled in the Converter
        if (gridStyle.HasFormat && gridStyle.CellType != "Currency")
        {
            //destRange.CellStyle.NumberFormatIndex = ConvertFormatFromGridToExcel( gridStyle.Format );
            int iFormatIndex = ConvertFormatFromGridToExcel(range.Worksheet.Workbook, gridStyle.Format);
            range.CellStyle.NumberFormatIndex = iFormatIndex;
        }
        else if ((gridStyle.CellValueType == typeof(Double) || gridStyle.CellValueType == typeof(Int32) || gridStyle.CellValueType == typeof(Decimal)) && gridStyle.Format == string.Empty)
        {
            range.CellStyle.NumberFormatIndex = DEF_EXCEL_NUMBER_FORMAT;
            string decimalSeperator = Application.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!string.IsNullOrEmpty(gridStyle.Text) && gridStyle.Text.IndexOf(decimalSeperator) == -1)
                range.NumberFormat = DEF_NUMBER_FORMAT;
        }
        
      //else
      //{
      //  switch( gridStyle.CellType )
      //  {
      //    case DEF_CELL_TYPE_CURRENCY:
      //      range.CellStyle.NumberFormatIndex = DEF_EXCEL_CURRENCY_FORMAT;
      //      break;
      //  }
      //}
    }
    
    /// <summary>
    /// Copies brush settings from grid cell into excel cell.
    /// </summary>
    /// <param name="gridCell">Source grid cell.</param>
    /// <param name="range">Destination excel cell.</param>
    private void CopyBrush( GridStyleInfo gridCell, IRange range )
    {
      // TODO: finish implementation.
      //if( !gridCell.HasInterior ) return;

      if( gridCell.Interior.Style == BrushStyle.Pattern )
      {
        IStyle rangeStyle = range.CellStyle;
        rangeStyle.FillPattern = GetClosestExcelPattern( gridCell.Interior.PatternStyle );
        rangeStyle.PatternColor = gridCell.Interior.ForeColor;
        rangeStyle.Color = gridCell.Interior.BackColor;
        if (range.Row == range.LastRow && range.Column == range.LastColumn)
        {
            range.Worksheet[range.Row, range.Column].CellStyle.Color = gridCell.Interior.BackColor;
        }
        else
        {
            for (int row = range.Row; row <= range.LastRow; row++)
                for (int col = range.Column; col <= range.LastColumn; col++)
                    range.Worksheet[row, col].CellStyle.Color = gridCell.Interior.BackColor;
        }
      }
      else if( gridCell.Interior.Style == BrushStyle.Solid )
      {
        IStyle rangeStyle = range.CellStyle;
        //rangeStyle.PatternColor = gridCell.Interior.ForeColor;
        Color color = gridCell.Interior.BackColor;
        int iColor = color.ToArgb() & DEF_COLOR_MASK;
        if (iColor != DEFAULT_COLOR)
        {
            //rangeStyle.Color = color;
            if (range.Row == range.LastRow && range.Column == range.LastColumn)
            {
                range.Worksheet[range.Row, range.Column].CellStyle.Color = color;
            }
            else
            {
                for (int row = range.Row; row <= range.LastRow; row++)
                    for (int col = range.Column; col <= range.LastColumn; col++)
                        range.Worksheet[row, col].CellStyle.Color = color;
            }
        }

        //Debug.WriteLine( rangeStyle.Color, "FillBackgroud" );
      }

      else if (gridCell.Interior.Style == BrushStyle.Gradient)
      {
          IStyle rangeStyle = range.CellStyle;
          if ((rangeStyle.Interior as CommonObject) == null ? (rangeStyle.Interior as InteriorWrapper).Wrapped.Application.DefaultVersion == ExcelVersion.Excel2007 : (rangeStyle.Interior as CommonObject).Application.DefaultVersion == ExcelVersion.Excel2007)
          {
              ExcelGradientStyle gradStyle;
              if (gridCell.Interior.GradientStyle == GradientStyle.None)
              {
                  rangeStyle.FillPattern = GetClosestExcelPattern(gridCell.Interior.PatternStyle);
                  rangeStyle.PatternColor = gridCell.Interior.ForeColor;
                  rangeStyle.Color = gridCell.Interior.BackColor;
                  if (range.Row == range.LastRow && range.Column == range.LastColumn)
                  {
                      range.Worksheet[range.Row, range.Column].CellStyle.Color = gridCell.Interior.BackColor;
                  }
                  else
                  {
                      for (int row = range.Row; row <= range.LastRow; row++)
                          for (int col = range.Column; col <= range.LastColumn; col++)
                              range.Worksheet[row, col].CellStyle.Color = gridCell.Interior.BackColor;
                  }
              }
              else
              {
                  rangeStyle.Interior.FillPattern = ExcelPattern.Gradient;
                  if (gridCell.Interior.GradientStyle == GradientStyle.Horizontal)
                  {
                      gradStyle = ExcelGradientStyle.Vertical;
                      rangeStyle.Interior.Gradient.ForeColor = gridCell.Interior.ForeColor;
                      rangeStyle.Interior.Gradient.BackColor = gridCell.Interior.BackColor;
                  }
                  else if (gridCell.Interior.GradientStyle == GradientStyle.Vertical)
                  {
                      gradStyle = ExcelGradientStyle.Horizontal;
                      rangeStyle.Interior.Gradient.ForeColor = gridCell.Interior.ForeColor;
                      rangeStyle.Interior.Gradient.BackColor = gridCell.Interior.BackColor;
                  }
                  else if (gridCell.Interior.GradientStyle == GradientStyle.BackwardDiagonal)
                  {
                      gradStyle = ExcelGradientStyle.Diagonl_Down;
                      rangeStyle.Interior.Gradient.ForeColor = gridCell.Interior.ForeColor;
                      rangeStyle.Interior.Gradient.BackColor = gridCell.Interior.BackColor;
                  }
                  else if (gridCell.Interior.GradientStyle == GradientStyle.ForwardDiagonal)
                  {
                      gradStyle = ExcelGradientStyle.Diagonl_Up;
                      rangeStyle.Interior.Gradient.ForeColor = gridCell.Interior.ForeColor;
                      rangeStyle.Interior.Gradient.BackColor = gridCell.Interior.BackColor;
                  }
                  else
                  {
                      gradStyle = ExcelGradientStyle.From_Center;
                      rangeStyle.Interior.Gradient.ForeColor = gridCell.Interior.BackColor;
                      rangeStyle.Interior.Gradient.BackColor = gridCell.Interior.ForeColor;
                  }
                  rangeStyle.Interior.Gradient.TwoColorGradient(gradStyle, ExcelGradientVariants.ShadingVariants_2);
              }
          }
          else
          {
              rangeStyle.FillPattern = GetClosestExcelPattern(gridCell.Interior.PatternStyle);
              rangeStyle.PatternColor = gridCell.Interior.ForeColor;
              bool set = false;
              foreach(Color clr in gridCell.Interior.GradientColors)
              {
                  if(clr != Color.White)
                  {
                      rangeStyle.Color = gridCell.Interior.BackColor;
                      if (range.Row == range.LastRow && range.Column == range.LastColumn)
                      {
                          range.Worksheet[range.Row, range.Column].CellStyle.Color = clr;
                      }
                      else
                      {
                          for (int row = range.Row; row <= range.LastRow; row++)
                              for (int col = range.Column; col <= range.LastColumn; col++)
                                  range.Worksheet[row, col].CellStyle.Color = clr;
                      }
                      //rangeStyle.Interior.Color = clr;
                      set = true;
                  }
              }
              if (!set)
              {
                  rangeStyle.Color = gridCell.Interior.BackColor;
                  if (range.Row == range.LastRow && range.Column == range.LastColumn)
                  {
                      range.Worksheet[range.Row, range.Column].CellStyle.Color = gridCell.Interior.BackColor;
                  }
                  else
                  {
                      for (int row = range.Row; row <= range.LastRow; row++)
                          for (int col = range.Column; col <= range.LastColumn; col++)
                              range.Worksheet[row, col].CellStyle.Color = gridCell.Interior.BackColor;
                  }
              }
          }
      }

      //      if( !gridCell.Interior.BackColor.IsEmpty )
      //      {
      //        xStyle.FillBackground = myWorkBook.SetColorOrGetNearest(style.BackColor);
      //      }
    }
    /// <summary>
    /// Copies font settings from grid cell into excel cell.
    /// </summary>
    /// <param name="gridCell">Source grid cell.</param>
    /// <param name="range">Destination excel cell.</param>
    private void CopyFont( GridStyleInfo gridCell, IRange range )
    {
        //if( gridCell.HasFont || gridCell.HasTextColor )
        {
            IStyle rangeStyle = range.CellStyle;
            IFont rangeFont = rangeStyle.Font;
            GridFontInfo font = gridCell.Font;

            //if( gridCell.HasTextColor )
            {
                rangeFont.RGBColor = gridCell.TextColor;
            }

            rangeFont.Size = Convert.ToInt32(font.Size);

            //if( font.HasFacename )
            {
                rangeFont.FontName = font.Facename;
            }

            //if( font.HasStrikeout )
            {
                rangeFont.Strikethrough = font.Strikeout;
            }

            //if( font.HasUnderline )
            {
                rangeFont.Underline = font.Underline ? ExcelUnderline.Single : ExcelUnderline.None;
            }

            //if( font.HasItalic )
            {
                rangeFont.Italic = font.Italic;
            }

            //if( font.HasBold )
            {
                rangeFont.Bold = font.Bold;
            }

            //if( font.HasOrientation )
            {
                int iAngle = font.Orientation;

                if (iAngle == 0 && gridCell.CellType == GridCellTypeName.ProgressBar)
                {
                    GridProgressBarInfo progressBar = gridCell.ProgressBar;

                    if (progressBar.ProgressOrientation == Orientation.Vertical) iAngle = 90;
                }

                if (iAngle > 90 && iAngle <= DEF_MINIMUM_SUPPORTED_ANGLE)
                    iAngle = iAngle - 90;
                else if (iAngle > DEF_MINIMUM_SUPPORTED_ANGLE && iAngle <= DEF_MAXIMUM_ANGLE)
                {
                    iAngle = DEF_MAXIMUM_ANGLE - iAngle;
                }
                rangeStyle.Rotation = iAngle;
            }
        }
    }
    /// <summary>
    /// Copies borders settings from grid cell into excel cell.
    /// </summary>
    /// <param name="gridCell">Source grid cell.</param>
    /// <param name="range">Destination excel cell.</param>
    private void CopyBorders( GridStyleInfo gridCell, IRange range )
    {
      if( gridCell == null )
        throw new ArgumentNullException( "gridCell" );

      if( range == null )
        throw new ArgumentNullException( "range" );

      if (!ExportBorders)
        return;

      if( gridCell.HasBorders || IsGroupingGrid)
      {
        GridBordersInfo borders = gridCell.Borders;
        IBorders rangeBorders = range.Borders;

        if( borders.HasBottom || IsGroupingGrid)
        {
          CopyBorder( borders.Bottom, rangeBorders[ ExcelBordersIndex.EdgeBottom ] );
        }

        if( borders.HasLeft || IsGroupingGrid)
        {
          CopyBorder( borders.Left, rangeBorders[ ExcelBordersIndex.EdgeLeft ] );
        }

        if( borders.HasRight || IsGroupingGrid)
        {
          CopyBorder( borders.Right, rangeBorders[ ExcelBordersIndex.EdgeRight ] );
        }

        if( borders.HasTop || IsGroupingGrid)
        {
          CopyBorder( borders.Top, rangeBorders[ ExcelBordersIndex.EdgeTop ] );
        }
      }
    }
    /// <summary>
    /// Copies alignment settings from grid cell into excel cell..
    /// </summary>
    /// <param name="gridCell">Source grid cell.</param>
    /// <param name="range">Destination excel cell.</param>
    private void CopyAlignment( GridStyleInfo gridCell, IRange range )
    {
      IStyle rangeStyle = range.CellStyle;

        rangeStyle.HorizontalAlignment = GetClosestExcelHorizontalAlignment(
          gridCell.HorizontalAlignment );

      rangeStyle.VerticalAlignment = ( gridCell.HasVerticalAlignment )
        ?GetClosestExcelVerticalAlignment( gridCell.VerticalAlignment )
        : DEF_DEFAULT_VALIGNMENT;

      if( gridCell.CellType == GridCellTypeName.ProgressBar )
      {
        rangeStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
        rangeStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;
      }
      if (gridCell.HasTextMargins)
      {
          if (gridCell.TextMargins.HasLeft && gridCell.HorizontalAlignment == GridHorizontalAlignment.Left)
          {
              range.CellStyle.IndentLevel = (int)range.Worksheet.PixelsToColumnWidth(gridCell.TextMargins.Left);
              range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
          }
          else if (gridCell.TextMargins.HasRight && gridCell.HorizontalAlignment == GridHorizontalAlignment.Right)
          {
              range.CellStyle.IndentLevel = (int)range.Worksheet.PixelsToColumnWidth(gridCell.TextMargins.Right);
              range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
          }
      }
      
      if( gridCell.HasWrapText )
      { 
        rangeStyle.WrapText = gridCell.WrapText;
      }
      else
      {
        rangeStyle.WrapText = true;
      }
    }
    /// <summary>
    /// Copies border settings from grid border into excel border.
    /// </summary>
    /// <param name="cellBorder">Grid border to copy.</param>
    /// <param name="rangeBorder">Destination excel border.</param>
    private void CopyBorder( GridBorder cellBorder, IBorder rangeBorder )
    {
      if( cellBorder == null )
        throw new ArgumentNullException( "cellBorder" );

      if( rangeBorder == null )
        throw new ArgumentNullException( "rangeBorder" );

      rangeBorder.LineStyle = GetExcelLineStyle(cellBorder);
      rangeBorder.ColorRGB = cellBorder.Color;
      //int iBorderTypeIndex = GetIndex( cellBorder );
      //ExcelLineStyle lineStyle = s_hashBorderIndexToExcelLineStyle[ iBorderTypeIndex ];
    }

    /// <summary>
    /// Combines two styles.
    /// </summary>
    /// <param name="destinationStyle">Style which is primary and in which result will be accumulated.</param>
    /// <param name="baseStyle">Style to combine.</param>
    /// <returns>Value indicating that not all properties were set.</returns>
    protected bool CombineStyles( GridStyleInfo destinationStyle, GridStyleInfo baseStyle )
    {
      if( destinationStyle == null )
        throw new ArgumentNullException( "destinationStyle" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseStyle" );

      bool bResult = false;

      bResult = CombineBorders( destinationStyle, baseStyle );
      bResult |= CombineFont( destinationStyle, baseStyle );
      bResult |= CombineBrush( destinationStyle, baseStyle );
      bResult |= CombineAlignment( destinationStyle, baseStyle );
      bResult |= CombineFormat( destinationStyle, baseStyle );
      bResult |= CombineCellType(destinationStyle, baseStyle);

      return bResult;
    }



    /// <summary>
    /// Combine borders information if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if not all borders in destBorders were set.</returns>
    private bool CombineBorders( GridStyleInfo destStyle, GridStyleInfo baseStyle )
    {
      if( destStyle == null )
        throw new ArgumentNullException( "destBorders" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseBorders" );

      if( !baseStyle.HasBorders ) return true;

      GridBordersInfo destBorders = destStyle.Borders;
      GridBordersInfo baseBorders = baseStyle.Borders;

      bool bResult = !destBorders.HasBottom && !baseBorders.HasBottom;

      if( !destBorders.HasBottom && baseBorders.HasBottom )
      {
        GridBorder border = baseBorders.Bottom;
        destBorders.Bottom = new GridBorder( border.Style, border.Color, border.Weight );
      }

      bResult |= !destBorders.HasLeft && !baseBorders.HasLeft;
      if( !destBorders.HasLeft && baseBorders.HasLeft )
      {
        GridBorder border = baseBorders.Left;
        destBorders.Left = new GridBorder( border.Style, border.Color, border.Weight );
      }

      bResult |= !destBorders.HasRight && !baseBorders.HasRight;
      if( !destBorders.HasRight && baseBorders.HasRight )
      {
        GridBorder border = baseBorders.Right;
        destBorders.Right = new GridBorder( border.Style, border.Color, border.Weight );
      }

      bResult |= !destBorders.HasTop && !baseBorders.HasTop;
      if( !destBorders.HasTop && baseBorders.HasTop )
      {
        GridBorder border = baseBorders.Top;
        destBorders.Top = new GridBorder( border.Style, border.Color, border.Weight );
      }

      return bResult;
    }
    /// <summary>
    /// Combine fonts if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if not all font properties in destFont were set.</returns>
    private bool CombineFont( GridStyleInfo destStyle, GridStyleInfo baseStyle )
    {
      if( destStyle == null )
        throw new ArgumentNullException( "destStyle" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseStyle" );

      bool bResult = false;

      if( !destStyle.HasTextColor )
      {
        if( baseStyle.HasTextColor )
        {
          destStyle.TextColor = baseStyle.TextColor;
        }
        else
        {
          bResult = true;
        }
      }

      if (!(baseStyle.CellType == "ColumnHeaderCell" || baseStyle.CellType == "RowHeaderCell") && !baseStyle.HasFont) return true;

      GridFontInfo destFont = destStyle.Font;
      GridFontInfo baseFont = baseStyle.Font;

      if( !destFont.Bold )
      {
        if( baseFont.Bold )
        {
          destFont.Bold = baseFont.Bold;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.Italic )
      {
        if( baseFont.Italic )
        {
          destFont.Italic = baseFont.Italic;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.Underline )
      {
        if( baseFont.Underline )
        {
          destFont.Underline = baseFont.Underline;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.HasFacename )
      {
        if( baseFont.HasFacename )
        {
          destFont.Facename = baseFont.Facename;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.HasOrientation )
      {
        if( baseFont.HasOrientation )
        {
          destFont.Orientation = baseFont.Orientation;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.HasSize )
      {
        if( baseFont.HasSize )
        {
          destFont.Size = baseFont.Size;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.Strikeout )
      {
        if( baseFont.Strikeout )
        {
          destFont.Strikeout = baseFont.Strikeout;
        }
        else
        {
          bResult = true;
        }
      }

      if( !destFont.HasUnit )
      {
        if( baseFont.HasUnit )
        {
          destFont.Unit = baseFont.Unit;
        }
        else
        {
          bResult = true;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Combines brushes if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if not the Interior property in destStyle is set.</returns>
    private bool CombineBrush( GridStyleInfo destStyle, GridStyleInfo baseStyle )
    {
      if( destStyle == null )
        throw new ArgumentNullException( "destStyle" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseStyle" );

      //if( !baseStyle.HasInterior ) return true;

      if( destStyle.HasInterior ) return false;

      if (baseStyle.Interior == destStyle.Interior) return true;

      destStyle.Interior = new BrushInfo( baseStyle.Interior );

      return false;
    }
    /// <summary>
    /// Combines alignment options if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if not all font properties in destFont were set.</returns>
    private bool CombineAlignment( GridStyleInfo destStyle, GridStyleInfo baseStyle )
    {
      if( destStyle == null )
        throw new ArgumentNullException( "destStyle" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseStyle" );

      bool bResult = false;

      bResult |= !destStyle.HasHorizontalAlignment && !baseStyle.HasHorizontalAlignment;
      if( !destStyle.HasHorizontalAlignment && (baseStyle.HasHorizontalAlignment || baseStyle.CellType == "ColumnHeaderCell"))
      {
        destStyle.HorizontalAlignment = baseStyle.HorizontalAlignment;
      }

      bResult |= !destStyle.HasVerticalAlignment && !baseStyle.HasVerticalAlignment;
      if( !destStyle.HasVerticalAlignment && baseStyle.HasVerticalAlignment )
      {
        destStyle.VerticalAlignment = baseStyle.VerticalAlignment;
      }

      bResult |= !destStyle.HasWrapText && !baseStyle.HasWrapText;
      if( !destStyle.HasWrapText && baseStyle.HasWrapText )
      { 
        destStyle.WrapText = baseStyle.WrapText;
      }
      if (!destStyle.HasTextMargins && baseStyle.HasTextMargins)
      {
          destStyle.TextMargins = baseStyle.TextMargins;
      }

      return bResult;
    }
    /// <summary>
    /// Combines format options if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if not all format properties in destFont were set.</returns>
    private bool CombineFormat( GridStyleInfo destStyle, GridStyleInfo baseStyle )
    {
      if( destStyle == null )
        throw new ArgumentNullException( "destStyle" );

      if( baseStyle == null )
        throw new ArgumentNullException( "baseStyle" );

      bool bResult = false;

      bResult |= !destStyle.HasFormat && !baseStyle.HasFormat;
      if (!destStyle.HasFormat && baseStyle.HasFormat)
      {
          destStyle.Format = baseStyle.Format;
      }

      return bResult;
    }

    /// <summary>
    /// Combines CellType if necessary.
    /// </summary>
    /// <param name="destStyle">Destination style.</param>
    /// <param name="baseStyle">Source style.</param>
    /// <returns>True if success</returns>
      private bool CombineCellType(GridStyleInfo destStyle, GridStyleInfo baseStyle)
      {
          if (destStyle == null)
              throw new ArgumentNullException("destStyle");

          if (baseStyle == null)
              throw new ArgumentNullException("baseStyle");

          bool bResult = false;

          bResult |= !destStyle.HasCellType && !baseStyle.HasCellType;
          if (!destStyle.HasCellType)
          {
              destStyle.CellType = baseStyle.CellType;
          }

          return bResult;
      }
    /// <summary>
    /// Converts PatterStyle into ExcelPattern.
    /// </summary>
    /// <param name="gridPattern">PatternStyle to convert.</param>
    /// <returns>Corresponding ExcelPattern.</returns>
    private ExcelPattern GetClosestExcelPattern( PatternStyle gridPattern )
    {
      return ( ExcelPattern )PatternStyleToExcelPattern[ gridPattern ];
    }

    /// <summary>
    /// Converts GridHorizontalAlignment into ExcelHAlign.
    /// </summary>
    /// <param name="gridHAlign">GridHorizontalAlignment to convert.</param>
    /// <returns>Corresponding ExcelHAlign.</returns>
    internal ExcelHAlign GetClosestExcelHorizontalAlignment( GridHorizontalAlignment gridHAlign )
    {
      switch( gridHAlign )
      {
        case GridHorizontalAlignment.Center: return ExcelHAlign.HAlignCenter;
        case GridHorizontalAlignment.Left:   return ExcelHAlign.HAlignLeft;
        case GridHorizontalAlignment.Right:  return ExcelHAlign.HAlignRight;

        default: throw new ArgumentOutOfRangeException( "gridHAling" );
      }
    }
    /// <summary>
    /// Converts GridVerticalAlignment into ExcelVAlign.
    /// </summary>
    /// <param name="gridVAlign">GridVerticalAlignment to convert.</param>
    /// <returns>Corresponding ExcelVAlign.</returns>
    internal ExcelVAlign GetClosestExcelVerticalAlignment( GridVerticalAlignment gridVAlign )
    {
      switch( gridVAlign )
      {
        case GridVerticalAlignment.Bottom: return ExcelVAlign.VAlignBottom;
        case GridVerticalAlignment.Middle: return ExcelVAlign.VAlignCenter;
        case GridVerticalAlignment.Top:    return ExcelVAlign.VAlignTop;

        default: throw new ArgumentOutOfRangeException( "gridVAlign" );
      }
    }
    /// <summary>
    /// Converts GridBorderStyle into ExcelLineStyle.
    /// </summary>
    /// <param name="gridLineStyle">GridBorderStyle to convert.</param>
    /// <returns>Corresponding ExcelLineStyle.</returns>
    private ExcelLineStyle GetExcelLineStyle( GridBorderStyle gridLineStyle )
    {
        
      switch ( gridLineStyle )
      {
        case GridBorderStyle.DashDot:    return ExcelLineStyle.Dash_dot;
        case GridBorderStyle.DashDotDot: return ExcelLineStyle.Dash_dot_dot;
        case GridBorderStyle.Dashed:     return ExcelLineStyle.Dashed;
        case GridBorderStyle.Dotted:     return ExcelLineStyle.Dotted;
        case GridBorderStyle.None:       return ExcelLineStyle.None;
        case GridBorderStyle.NotSet:     return ExcelLineStyle.None;
        case GridBorderStyle.Solid:      return ExcelLineStyle.Thin;
        case GridBorderStyle.Standard:  return ExcelLineStyle.None;

        default: throw new ArgumentOutOfRangeException( "Border style is out of range" );
      }
    }
      /// <summary>
      /// Converts GridBorder into ExcelLineStyle. 
      /// </summary>
      /// <remarks>
      /// This method also considers GridBorderWeight.
      /// </remarks>
      /// <param name="gridBorder">GridBorder to convert.</param>
      /// <returns>Corresponding ExcelLineStyle.</returns>
      private ExcelLineStyle GetExcelLineStyle(GridBorder gridBorder)
      {
          GridBorderStyle gridLineStyle = gridBorder.Style;
          ExcelLineStyle excelLineStyle = GetExcelLineStyle(gridLineStyle);
          switch (excelLineStyle)
          {
              case ExcelLineStyle.Thin:
                  if (gridBorder.Weight == GridBorderWeight.Medium)
                      return ExcelLineStyle.Medium;
                  else if (gridBorder.Width >= 2)
                      return ExcelLineStyle.Thick;
                  return excelLineStyle;
              case ExcelLineStyle.Dash_dot:
                  if (gridBorder.Width >= 2)
                      return ExcelLineStyle.Medium_dash_dot;
                  return excelLineStyle;
              case ExcelLineStyle.Dash_dot_dot:
                  if (gridBorder.Width >= 2)
                      return ExcelLineStyle.Medium_dash_dot_dot;
                  return excelLineStyle;
              case ExcelLineStyle.Dashed:
                  if (gridBorder.Width >= 2)
                      return ExcelLineStyle.Medium_dashed;
                  return excelLineStyle;
              default:
                  return excelLineStyle;
          }
      }

    /// <summary>
    /// Converts grid number format into excel number format.
    /// </summary>
      /// <param name="book">Grid format to convert.</param>
      ///    /// <param name="strGridFormat">Grid format to convert.</param>
    /// <returns>Index to the excel format.</returns>
    private int ConvertFormatFromGridToExcel( IWorkbook book, string strGridFormat )
    {
      if( strGridFormat == null || strGridFormat.Length == 0 )
        return DEF_EXCEL_GENERAL_FORMAT;

      // TODO: implement better format converter.

      switch( char.ToUpper( strGridFormat[ 0 ] ) )
      {
        case DEF_CHAR_GENERAL:
          return DEF_EXCEL_GENERAL_FORMAT;

        case DEF_CHAR_CURRENCY:
          return DEF_EXCEL_CURRENCY_FORMAT;

        //case DEF_CHAR_DECIMAL:
        //  return DEF_EXCEL_DECIMAL_FORMAT;
          case DEF_CHAR_DATE:
              return DEF_EXCEL_DATE_FORMAT;

        case DEF_CHAR_SCIENTIFIC:
          return DEF_EXCEL_SCIENTIFIC_FORMAT;

        case DEF_CHAR_FIXEDPOINT:
          return DEF_EXCEL_FIXEDPOINT_FORMAT;

        case DEF_CHAR_NUMBER:
          return DEF_EXCEL_NUMBER_FORMAT;

        case DEF_CHAR_PERCENT:
          return DEF_EXCEL_PERCENT_FORMAT;

        case DEF_CHAR_HEX:
          return DEF_EXCEL_HEX_FORMAT;

        default:
        {
          WorkbookImpl workbook = ( WorkbookImpl )book;
          int iIndex = workbook.InnerFormats.CreateFormat( strGridFormat );
          return iIndex;
        }
      }
    }
    #endregion

    #region Class Import into Grid from Excel
    /// <summary>
    /// A method that converts excel file into GridControl.
    /// </summary>
    /// <param name="strFileName">File to convert.</param>
    /// <param name="grid">Grid which will get converted data.</param>
    public void ExcelToGrid(string strFileName, GridModel grid)
    {
        if (strFileName == null)
            throw new ArgumentNullException("strFileName");

        if (strFileName.Length == 0)
            throw new ArgumentException("strFileName - string can not be empty");

        if (grid == null)
            throw new ArgumentNullException("grid");
        //this.grid = grid;
        using (ExcelEngine engine = CreateEngine())
        {
            IWorkbook book = engine.Excel.Workbooks.Open(strFileName);

            ExcelToGrid(book.Worksheets[0], grid);

            book.Close(false);
        }
    }

      // Current Range while importing.
      IRange currentRange = null;

    /// <summary>
    /// A method that exports data from excel worksheet into grid.
    /// </summary>
    /// <param name="sheet">Worksheet to export from.</param>
    /// <param name="grid">Grid to export into.</param>
    public void ExcelToGrid( IWorksheet sheet, GridModel grid )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( grid == null )
        throw new ArgumentNullException( "grid" );

      IRange range = sheet.UsedRange;
      int iFirstRow = range.Row;
      int iLastRow  = range.End.Row;
      int iFirstCol = range.Column;
      int iLastCol  = range.End.Column;

      grid.BeginUpdate();
      grid.RowCount = iLastRow;
      grid.ColCount = iLastCol;

      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = iFirstCol; iCol <= iLastCol; iCol++ )
        {
          if( sheet.Contains( iRow, iCol ) )
          {
            IRange rangeToConvert = sheet.Range[ iRow, iCol ];
            ConvertExcelRangeToGrid( rangeToConvert, grid );
          }
        }
      }

      CopyRowHeightToGrid( sheet, grid );
      CopyColumnWidthToGrid( sheet, grid );
      CopyMergesToGrid( sheet, grid );
      grid.EndUpdate();
    }
    /// <summary>
    /// A method that converts excel workbook into array of grid models.
    /// </summary>
    /// <param name="strFileName">File to convert.</param>
    /// <param name="arrIndexes">Worksheet's indexes to convert.</param>
    /// <param name="arrModels">Array of models to convert into.</param>
      public void ExcelToGrid(string strFileName, int[] arrIndexes, GridModel[] arrModels)
      {
          if (strFileName == null)
              throw new ArgumentNullException("strFileName");

          if (strFileName.Length == 0)
              throw new ArgumentException("strFileName - string can not be empty");

          if (arrIndexes == null)
              throw new ArgumentNullException("arrIndexes");

          if (arrModels == null)
              throw new ArgumentNullException("arrModels");

          if (arrIndexes.Length != arrModels.Length)
              throw new ArgumentException("Indexes and models do not correspond each other");

          using (ExcelEngine engine = CreateEngine())
          {
              IWorkbook book = engine.Excel.Workbooks.Open(strFileName);
              int iSheetsCount = book.Worksheets.Count;

              for (int i = 0, len = arrIndexes.Length; i < len; i++)
              {
                  int index = arrIndexes[i];
                  if (index >= iSheetsCount) continue;

                  IWorksheet sheet = book.Worksheets[index];
                  GridModel model = arrModels[i];
                  ExcelToGrid(sheet, model);
              }

              book.Close();
          }
      }
    /// <summary>
    /// A method that converts excel workbook into array of grid models.
    /// </summary>
    /// <param name="strFileName">File to convert.</param>
    /// <param name="arrRanges">Array of worksheet ranges to convert.</param>
    /// <param name="arrModels">Array of GridModels to convert into.</param>
    public void ExcelToGrid( string strFileName, IndexRange[] arrRanges, GridModel[] arrModels )
    {
      if( strFileName == null )
        throw new ArgumentNullException( "strFileName" );

      if( strFileName.Length == 0 )
        throw new ArgumentException( "strFileName - string can not be empty" );

      if( arrRanges == null )
        throw new ArgumentNullException( "arrRanges" );

      if( arrModels == null )
        throw new ArgumentNullException( "arrModels" );

    using (ExcelEngine engine = CreateEngine())
    {
        IWorkbook book = engine.Excel.Workbooks.Open(strFileName);
        int iSheetsCount = book.Worksheets.Count;
        int iModelIndex = 0;
        int iModelCount = arrModels.Length;

        for (int i = 0, len = arrRanges.Length; i < len; i++)
        {
            IndexRange range = arrRanges[i];
            for (int j = range.FirstIndex, last = range.LastIndex; j <= last; j++)
            {
                if (j >= iSheetsCount) break;

                IWorksheet sheet = book.Worksheets[j];

                if (iModelIndex >= iModelCount)
                {
                    throw new ArgumentOutOfRangeException("Model index was out of range");
                }

                ExcelToGrid(sheet, arrModels[iModelIndex]);
                iModelIndex++;
            }
        }

        book.Close();
    }
    }
    /// <summary>
    /// A method that converts excel file into array of grid models.
    /// </summary>
    /// <param name="strFileName">File name to convert.</param>
    /// <returns>Corresponding grid models.</returns>
      public GridModel[] ExcelToGrid(string strFileName)
      {
          if (strFileName == null)
              throw new ArgumentNullException("strFileName");

          if (strFileName.Length == 0)
              throw new ArgumentException("strFileName - string can not be empty");

          GridModel[] models = null;

          using (ExcelEngine engine = CreateEngine())
          {
              IWorkbook book = engine.Excel.Workbooks.Open(strFileName);
              models = ExcelToGrid(book);
          }

          return models;
      }
    /// <summary>
    /// A method that converts excel workbook into array of grid models.
    /// </summary>
    /// <param name="book">Source workbook to convert.</param>
    /// <returns>Corresponding grid models.</returns>
    public GridModel[] ExcelToGrid( IWorkbook book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      int iSheetsCount = book.Worksheets.Count;

      GridModel[] arrResult = new GridModel[ iSheetsCount ];

      for( int i = 0; i < iSheetsCount; i++ )
      {
        GridModel model = new GridModel();
        ExcelToGrid( book.Worksheets[ i ], model );
        arrResult[ i ] = model;
      }

      book.Close( false );

      return arrResult;
    }
    /// <summary>
    /// Puts data from IRange into GridControl.
    /// </summary>
    /// <param name="rangeToConvert">Range to convert.</param>
    /// <param name="grid">Grid to convert into.</param>
    public virtual void ConvertExcelRangeToGrid( IRange rangeToConvert, GridModel grid )
    {
      if( rangeToConvert == null )
        throw new ArgumentNullException( "rangeToConvert" );

      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( !rangeToConvert.IsInitialized ) return;
      this.grid = grid;
      GridStyleInfo cell = grid[ rangeToConvert.Row, rangeToConvert.Column ];

            GridImportExportCellInfoEventArgs e = new GridImportExportCellInfoEventArgs(rangeToConvert, cell, GridConverterAction.Import, rangeToConvert.Row, rangeToConvert.Column);
            RaiseQueryImportExportCellInfo(e);
            if (e.Handled)
                return;

      if( rangeToConvert.HasDateTime )
      {
        cell.CellValue = rangeToConvert.DateTime;
      }
      else if( rangeToConvert.HasNumber )
      {
          if (rangeToConvert.NumberFormat.Equals("General"))
          {
              cell.CellValue = rangeToConvert.Number;
          }
          else
          {
              cell.Format = rangeToConvert.NumberFormat;
              if (rangeToConvert.NumberFormat.IndexOf("%") != -1)
              {
                  cell.CellValue = rangeToConvert.DisplayText;
              }
              else
              {
                  cell.CellValue = rangeToConvert.Number;
              }
          }
        if(rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
            rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;

      }
      else if( rangeToConvert.IsBoolean )
      {
        cell.CellValue = rangeToConvert.Boolean;
        cell.Text = rangeToConvert.Value;
      }
      else
      {
          if (rangeToConvert.HasFormula || rangeToConvert.HasFormulaArray)
          {
              cell.CellType = "FormulaCell";
              string cellText = rangeToConvert.Value;
              if (cellText.Contains(".xla") && cellText.Contains("!"))
              {
                  int i = cellText.IndexOf('!');
                  cellText = cellText.Substring(i).Replace('!', '=');
              }
              cell.Text = cellText;
              cell.Text = cell.Text.Replace("'", "");
          }
          else
              cell.Text = rangeToConvert.Value;

          if (rangeToConvert.CellStyle.HorizontalAlignment == ExcelHAlign.HAlignGeneral)
          {
              if (!(rangeToConvert.HasFormula || rangeToConvert.HasFormulaArray))
                  rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignLeft;
              else if(rangeToConvert.FormulaNumberValue.ToString() != double.NaN.ToString())
                  rangeToConvert.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
          }
      }

      currentRange = rangeToConvert;
      SetCellStyle( rangeToConvert.CellStyle, cell );
    }
    /// <summary>
    /// Sets grid cell style based on excel style.
    /// </summary>
    /// <param name="excelStyle">Source style.</param>
    /// <param name="cell">Destination cell.</param>
    private void SetCellStyle( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      if( !excelStyle.IsInitialized ) return;

      SetBorders(excelStyle, cell);
      SetAlignment(excelStyle, cell);
      SetFont(excelStyle, cell);
      SetBrush(excelStyle, cell);
      if (excelStyle.Rotation != 0)
          SetOrientation(excelStyle, cell);
      SetNumberFormat(excelStyle, cell);
    }
    /// <summary>
    /// Copies number format from excel style into grid cell.
    /// </summary>
    /// <param name="excelStyle">Source style.</param>
    /// <param name="cell">Destination cell.</param>
    private void SetNumberFormat( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      int iNumberFormat = excelStyle.NumberFormatIndex;

      if((Array.IndexOf( DEF_CURRENCY_NUMBER_FORMATS, iNumberFormat ) != -1  ||
          excelStyle.NumberFormat.IndexOf("$") >= 0 )
        && cell.CellType != GridCellTypeName.FormulaCell )
      {
        cell.CellType = GridCellTypeName.Currency;
      }
      else if( iNumberFormat == DEF_EXCEL_GENERAL_FORMAT
        || excelStyle.NumberFormat.ToUpper() == DEF_GENERAL_FORMAT ) // Comparing with General for OpenOffice compatibility.
      {
        cell.Format = DEF_CHAR_GENERAL.ToString();
      }
      else if (Array.IndexOf(DEF_EXCEL_NUMBER_FORMATS, iNumberFormat) != -1
          || iNumberFormat == DEF_EXCEL_DATE_FORMAT || iNumberFormat == 164 || iNumberFormat == 165 || iNumberFormat == 166 || iNumberFormat == 167 || iNumberFormat == 169 || excelStyle.NumberFormat.Contains("%")
          || excelStyle.NumberFormatSettings.IsScientific || excelStyle.NumberFormatSettings.IsFraction || excelStyle.NumberFormatSettings.IsThousandSeparator)
      {
          CultureInfo info = CultureInfo.CurrentCulture;
          cell.Format = excelStyle.NumberFormat;
          cell.Format = cell.Format.Replace(info.DateTimeFormat.TimeSeparator + "mm", BMARKER.ToString()).Replace("mm" + info.DateTimeFormat.TimeSeparator, ZMARKER.ToString()).Replace('m', 'M').Replace(ZMARKER.ToString(), "mm" + info.DateTimeFormat.TimeSeparator).Replace(BMARKER.ToString(), info.DateTimeFormat.TimeSeparator + "mm");
          cell.Format = cell.Format.Replace("[$-409]", "").Replace("_(*", "").Replace("_(@_)", "").Replace(";@", "").Replace("@", "").Replace("_(", "").Replace("*", "").Replace("_)", "").Replace("??", "").Replace("?","").Replace("AM/PM", "tt");
      }      
      else
      {
          cell.Format = ""; //Returns the underlying cell value for custom formats.
      }
    }
    /// <summary>
    /// Copies orientation settings from excel style into grid cell.
    /// </summary>
    /// <param name="excelStyle">Source cell style.</param>
    /// <param name="cell">Destination grid cell.</param>
    private void SetOrientation( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      int iAngle = excelStyle.Rotation;

      if( iAngle != 0 )
      {
        if( iAngle > DEF_RIGHT_ANGLE && iAngle <= DEF_RIGHT_ANGLE + DEF_RIGHT_ANGLE )
        {
          iAngle -= DEF_RIGHT_ANGLE;
        }
        else if( iAngle > 0 && iAngle <= DEF_RIGHT_ANGLE )
        {
          iAngle = DEF_MAXIMUM_ANGLE - iAngle;
        }
        else
        {
          iAngle = 0;
        }

        cell.Font.Orientation = iAngle;
      }
    }
      /// <summary>
      /// After v6.1 ExcelToGrid Convertion will check for Merged cells when importing borders. 
      /// When there is no merged cells this can be set to true to avoid cehcking for MergeCells on 
      /// importing borders from exccel cells.
      /// <para/>Default value is false.
      /// </summary>
      public static bool SkipMergeCellCheckOnSetBorders = false;

      bool GetMergeCellRange(IRange range, out int bottom, out int right)
      {
          bottom = right = -1;
              
          IWorksheet sheet = range.Worksheet;

          if (!SkipMergeCellCheckOnSetBorders && sheet.MergedCells!=null)
          {
          foreach (IRange cell in sheet.MergedCells)
          {
              if (cell.Row == range.Row && cell.Column == range.Column)
              {
                  bottom = cell.End.Row;
                  right = cell.End.Column;
                  return true;

              }
          }
          }
          return false;
      }

    /// <summary>
    /// Sets grid cell borders based on excel style.
    /// </summary>
    /// <param name="excelStyle">Source style.</param>
    /// <param name="cell">Destination cell.</param>
      private void SetBorders(IStyle excelStyle, GridStyleInfo cell)
      {
          if (excelStyle == null)
              throw new ArgumentNullException("excelStyle");

          if (cell == null)
              throw new ArgumentNullException("cell");

          GridBordersInfo borders = cell.Borders;
          IBorders rangeBorders = excelStyle.Borders;
          GridBorder border1 = new GridBorder();
          IBorder border = rangeBorders[ExcelBordersIndex.EdgeTop];
          GridBordersInfo tempGridBorders = grid[currentRange.Row, currentRange.Column].Borders;
          int rowHeader = grid.Rows.HeaderCount + 1;
          int colHeader = grid.Cols.HeaderCount + 1;
          if (border.LineStyle != ExcelLineStyle.None)
          {
              tempGridBorders = (currentRange.Row > 1) ? grid[currentRange.Row - 1, currentRange.Column].Borders : tempGridBorders;
              border1 = ConvertExcelBorder(border);
              if (currentRange.Row != rowHeader && border1 != tempGridBorders.Bottom)
                  borders.Top = border1;
          }

          border = rangeBorders[ExcelBordersIndex.EdgeLeft];

          if (border.LineStyle != ExcelLineStyle.None)
          {
              tempGridBorders = (currentRange.Column > 1) ? grid[currentRange.Row, currentRange.Column - 1].Borders : tempGridBorders;
              border1 = ConvertExcelBorder(border);
              if (currentRange.Column != colHeader && border1 != tempGridBorders.Right)
                  borders.Left = border1;
          }
          border = rangeBorders[ExcelBordersIndex.EdgeBottom];
          if (border.LineStyle != ExcelLineStyle.None)
          {
              tempGridBorders = grid[currentRange.Row + 1, currentRange.Column].Borders;
              border1 = ConvertExcelBorder(border);
              if (border1 != tempGridBorders.Top)
                  borders.Bottom = border1;
          }

          int bottom, right;
          //ToDo: Is there any optimized way to handle borders with Merged Cells?
          bool isMergedRange = GetMergeCellRange(currentRange, out  bottom, out right);
          if (isMergedRange)
          {
              IWorksheet sheet = currentRange.Worksheet;
              IRange coveredRange = sheet[bottom, right];
              border = coveredRange.CellStyle.Borders[ExcelBordersIndex.EdgeBottom];
              if (border.LineStyle != ExcelLineStyle.None)
              {
                  tempGridBorders = grid[currentRange.Row + 1, currentRange.Column].Borders;
                  border1 = ConvertExcelBorder(border);
                  if (border1 != tempGridBorders.Top)
                      borders.Bottom = border1;
              }

              border = coveredRange.CellStyle.Borders[ExcelBordersIndex.EdgeRight];
              if (border.LineStyle != ExcelLineStyle.None)
              {
                  tempGridBorders = grid[currentRange.Row, currentRange.Column + 1].Borders;
                  border1 = ConvertExcelBorder(border);
                  if (border1 != tempGridBorders.Left)
                      borders.Right = border1;
              }

          }
          else
          {
              border = rangeBorders[ExcelBordersIndex.EdgeBottom];
              if (border.LineStyle != ExcelLineStyle.None)
              {
                  tempGridBorders = grid[currentRange.Row + 1, currentRange.Column].Borders;
                  border1 = ConvertExcelBorder(border);
                  if (border1 != tempGridBorders.Top)
                      borders.Bottom = border1;
              }

              border = rangeBorders[ExcelBordersIndex.EdgeRight];
              if (border.LineStyle != ExcelLineStyle.None)
              {
                  tempGridBorders = grid[currentRange.Row, currentRange.Column + 1].Borders;
                  border1 = ConvertExcelBorder(border);
                  if (border1 != tempGridBorders.Left)
                      borders.Right = border1;
              }
          }
      }
    /// <summary>
    /// Converts excel border into grid border.
    /// </summary>
    /// <param name="border">Border to convert.</param>
    private GridBorder ConvertExcelBorder( IBorder border )
    {
      if( border == null )
        throw new ArgumentNullException( "border" );

      GridBorderStyle borderStyle = GetGridBorderStyle( border.LineStyle );
      GridBorderWeight borderWeight = GetGridBorderWeight( border.LineStyle );

      return new GridBorder( borderStyle, border.ColorRGB, borderWeight );
    }
    /// <summary>
    /// Converts ExcelLineStyle into GridBorderStyle.
    /// </summary>
    /// <param name="excelLineStyle">Line style to convert.</param>
    /// <returns>Converted value.</returns>
    private GridBorderStyle GetGridBorderStyle( ExcelLineStyle excelLineStyle )
    {
      switch( excelLineStyle )
      {
        case ExcelLineStyle.None:
          return GridBorderStyle.None;

        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Medium:
        case ExcelLineStyle.Thick:
        case ExcelLineStyle.Hair:
        case ExcelLineStyle.Double:
          return GridBorderStyle.Solid;

        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Medium_dashed:
          return GridBorderStyle.Dashed;

        case ExcelLineStyle.Dotted:
          return GridBorderStyle.Dotted;


        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Slanted_dash_dot:
          return GridBorderStyle.DashDot;

        case ExcelLineStyle.Dash_dot_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          return GridBorderStyle.DashDotDot;

        default:
          throw new ArgumentOutOfRangeException( "excelLineStyle" );
      }
    }
    /// <summary>
    /// Converts ExcelLineStyle into GridBorderWeight.
    /// </summary>
    /// <param name="excelLineStyle">Line style to convert.</param>
    /// <returns>Covnerted value.</returns>
    private GridBorderWeight GetGridBorderWeight( ExcelLineStyle excelLineStyle )
    {
      switch( excelLineStyle )
      {
        case ExcelLineStyle.None:
        case ExcelLineStyle.Hair:
          return GridBorderWeight.ExtraThin;


        case ExcelLineStyle.Thin:
        case ExcelLineStyle.Dashed:
        case ExcelLineStyle.Dotted:
        case ExcelLineStyle.Dash_dot:
        case ExcelLineStyle.Slanted_dash_dot:
        case ExcelLineStyle.Dash_dot_dot:
          return GridBorderWeight.Thin;

        
        case ExcelLineStyle.Medium_dashed:
        case ExcelLineStyle.Medium_dash_dot:
        case ExcelLineStyle.Medium_dash_dot_dot:
          return GridBorderWeight.Medium;

        case ExcelLineStyle.Thick:
        case ExcelLineStyle.Double:
        case ExcelLineStyle.Medium:
          return GridBorderWeight.Thick;

        default:
          throw new ArgumentOutOfRangeException( "excelLineStyle" );
      }
    }
    /// <summary>
    /// Sets alignment of the GridStyleInfo.
    /// </summary>
    /// <param name="excelStyle">Excel style to get alignment block from.</param>
    /// <param name="cell">GridStyleInfo to set alignment block in.</param>
    private void SetAlignment( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      cell.HorizontalAlignment = GetGridHAlign( excelStyle.HorizontalAlignment );
      cell.VerticalAlignment = GetGridVAlign( excelStyle.VerticalAlignment );
      cell.WrapText = excelStyle.WrapText;
    }
    /// <summary>
    /// Converts ExcelHAling into GridHorizontalAlignment.
    /// </summary>
    /// <param name="align">Alignment to convert.</param>
    /// <returns>Covnerted value.</returns>
    private GridHorizontalAlignment GetGridHAlign( ExcelHAlign align )
    {
      switch( align )
      {
        case ExcelHAlign.HAlignLeft: return GridHorizontalAlignment.Left;
        case ExcelHAlign.HAlignRight: return GridHorizontalAlignment.Right;

        default: return GridHorizontalAlignment.Center;
      }
    }
    /// <summary>
    /// Converts ExcelVAling into GridVerticalAlignment.
    /// </summary>
    /// <param name="align">Alignment to convert.</param>
    /// <returns>Converted value.</returns>
    private GridVerticalAlignment GetGridVAlign( ExcelVAlign align )
    {
      switch( align )
      {
        case ExcelVAlign.VAlignBottom: return GridVerticalAlignment.Bottom;
        case ExcelVAlign.VAlignCenter: return GridVerticalAlignment.Middle;
        case ExcelVAlign.VAlignTop:    return GridVerticalAlignment.Top;

        default: return GridVerticalAlignment.Middle;
      }
    }
    /// <summary>
    /// Sets font for the cell based on ExcelRW style.
    /// </summary>
    /// <param name="excelStyle">ExcelRW style to get font setting from.</param>
    /// <param name="cell">Cell style to set font.</param>
    private void SetFont( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      IFont excelFont = excelStyle.Font;

      GridFontInfo gridFont = cell.Font;
      gridFont.Bold = excelFont.Bold;
      gridFont.Italic = excelFont.Italic;
      gridFont.Underline = (excelFont.Underline != ExcelUnderline.None);
      gridFont.Facename = excelFont.FontName;
      gridFont.Orientation = excelStyle.Rotation;
      gridFont.Size = (float)excelFont.Size;
      gridFont.Strikeout = excelFont.Strikethrough;
      gridFont.Unit = GraphicsUnit.Point;

      if (excelFont.RGBColor != EXCEL_DEFAULT_COLOR && excelFont.RGBColor!= TRANSPARENT_COLOR)
      {
          Color name = Color.FromName(GetClosestNamedColor(excelFont.RGBColor));
          Color clr = Color.FromArgb(name.A, name.R, name.G, name.B);
          if (!excelFont.RGBColor.IsKnownColor && !excelFont.RGBColor.Equals(clr))
              cell.TextColor = name;
          else
              cell.TextColor = (name == Color.Transparent) ? Color.FromArgb(excelFont.RGBColor.R, excelFont.RGBColor.G, excelFont.RGBColor.B) : excelFont.RGBColor;
      }
      else
          cell.TextColor = excelFont.RGBColor;
    }

      /// <summary>
      /// Used internally.
      /// </summary>
      /// <param name="c">color to be parsed for nearest color.</param>
      /// <returns>name of the nearest color.</returns>
      private string GetClosestNamedColor(Color c)
      {
          string closestColorName = null;
          double closestDistance = double.MaxValue;
          System.Reflection.BindingFlags bindFlags = System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static;
          foreach (System.Reflection.PropertyInfo prop in typeof(Color).GetProperties(bindFlags))
          {
              Color knownColor = (Color)(prop.GetValue(null, null));
              double dist = 0;

              dist = Math.Sqrt((System.Math.Pow((System.Convert.ToInt32(c.R) - knownColor.R), 2)) + (System.Math.Pow((System.Convert.ToInt32(c.G) - knownColor.G), 2)) + (System.Math.Pow((System.Convert.ToInt32(c.B) - knownColor.B), 2)));
              if (dist < closestDistance)
              {
                  closestDistance = dist;
                  closestColorName = prop.Name;
              }
          }
          return closestColorName;
      }

      ExcelPattern pattern = ExcelPattern.None;
    /// <summary>
    /// Sets cell's brush.
    /// </summary>
    /// <param name="excelStyle">Excel style containing information about brush.</param>
    /// <param name="cell">Style info to set brush in.</param>
    private void SetBrush( IStyle excelStyle, GridStyleInfo cell )
    {
      if( excelStyle == null )
        throw new ArgumentNullException( "excelStyle" );

      if( cell == null )
        throw new ArgumentNullException( "cell" );

      if( excelStyle.FillPattern == ExcelPattern.None ) return;

        PatternStyle patternStyle = PatternStyle.None;
        if (pattern != excelStyle.FillPattern)
            patternStyle = GetClosestGridPattern(excelStyle.FillPattern);

        pattern = excelStyle.FillPattern;
        BrushInfo brush;

      if( excelStyle.FillPattern == ExcelPattern.Solid )
      {
          if (!excelStyle.Color.IsNamedColor)
              brush = new BrushInfo(Color.FromName(GetClosestNamedColor(excelStyle.Color)));
          else
              brush = new BrushInfo(excelStyle.Color);
      }
      else
      {
          if (!excelStyle.Color.IsNamedColor)
              brush = new BrushInfo(patternStyle, Color.FromName(GetClosestNamedColor(excelStyle.Color)), excelStyle.PatternColor);
          else
              brush = new BrushInfo(patternStyle, excelStyle.Color, excelStyle.PatternColor);
      }

      cell.Interior = brush;
    }
    /// <summary>
    /// Converts ExcelPattern into PatternStyle.
    /// </summary>
    /// <param name="pattern">ExcelPattern to convert.</param>
    /// <returns>Converted value.</returns>
    private PatternStyle GetClosestGridPattern( ExcelPattern pattern )
    {
      switch( pattern )
      {
        case ExcelPattern.Angle:                return PatternStyle.Cross;
        case ExcelPattern.DarkDownwardDiagonal: return PatternStyle.DarkDownwardDiagonal;
        case ExcelPattern.DarkHorizontal:       return PatternStyle.DarkHorizontal;
        case ExcelPattern.DarkUpwardDiagonal:   return PatternStyle.DarkUpwardDiagonal;
        case ExcelPattern.DarkVertical:         return PatternStyle.DarkVertical;

        case ExcelPattern.ForwardDiagonal:      return PatternStyle.ForwardDiagonal;
        case ExcelPattern.Horizontal:           return PatternStyle.Horizontal;
        case ExcelPattern.LightDownwardDiagonal:return PatternStyle.LightDownwardDiagonal;
        case ExcelPattern.LightUpwardDiagonal:  return PatternStyle.LightUpwardDiagonal;
        case ExcelPattern.Vertical:             return PatternStyle.Vertical;
        case ExcelPattern.None:                 return PatternStyle.None;
        case ExcelPattern.Percent05:            return PatternStyle.Percent05;
        case ExcelPattern.Percent10:            return PatternStyle.Percent10;
        case ExcelPattern.Percent25:            return PatternStyle.Percent25;

        case ExcelPattern.Percent50:            return PatternStyle.Percent50;

        case ExcelPattern.Percent60:            return PatternStyle.Percent60;
        case ExcelPattern.Percent70:            return PatternStyle.Percent70;
        case ExcelPattern.Percent75:            return PatternStyle.Percent75;
        case ExcelPattern.Solid:                return PatternStyle.ZigZag;

        default: throw new ArgumentOutOfRangeException( "pattern" );
      }
    }
    /// <summary>
    /// Copies row height information from excel worksheet into grid.
    /// </summary>
    /// <param name="sheet">Sheet to copy from.</param>
    /// <param name="grid">Grid to copy into.</param>
    private void CopyRowHeightToGrid( IWorksheet sheet, GridModel grid )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      IRange usedRange = sheet.UsedRange;

      for( int i = usedRange.Row, last = usedRange.End.Row; i <= last; i++ )
      {
        grid.RowHeights[ i ] = sheet.GetRowHeightInPixels( i );
      }
    }
    /// <summary>
    /// Copies column width information from excel worksheet into grid.
    /// </summary>
    /// <param name="sheet">Sheet to copy from.</param>
    /// <param name="grid">Grid to copy into.</param>
    private void CopyColumnWidthToGrid( IWorksheet sheet, GridModel grid )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      IRange usedRange = sheet.UsedRange;
      int errorCorrectionInLength = 2;
      for (int i = usedRange.Column;  i <= usedRange.End.Column; i++)
      {
          grid.Model.ColWidths[i] = sheet.GetColumnWidthInPixels(i) + errorCorrectionInLength;
      }
    }
    /// <summary>
    /// Copies merged ranges into grid.
    /// </summary>
    /// <param name="sheet">Destination worksheet.</param>
    /// <param name="grid">Source grid.</param>
    private void CopyMergesToGrid( IWorksheet sheet, GridModel grid )
    {
      if( grid == null )
        throw new ArgumentNullException( "grid" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      GridModelCoveredRanges coveredRanges = grid.CoveredRanges;
      IRange[] arrMerges = sheet.MergedCells;
      if (arrMerges != null)
      {
          for( int i = 0, len = arrMerges.Length; i < len; i++ )
          {
            IRange curRange = arrMerges[ i ];
            GridRangeInfo mergedRange = GridRangeInfo.Cells( curRange.Row, curRange.Column,
              curRange.End.Row, curRange.End.Column );

            coveredRanges.Add( mergedRange );
          }
      }
    }


        /// <summary>
        /// Initiates call to <see cref="OnQueryImportExportCellInfo"/>.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected void RaiseQueryImportExportCellInfo(GridImportExportCellInfoEventArgs e)
        {
            try
            {
                OnQueryImportExportCellInfo(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Raises the <see cref="GridExcelConverterBase.QueryImportExportCellInfo"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridImportExportCellInfoEventArgs"/> that contains the event data.</param>
        private void OnQueryImportExportCellInfo(GridImportExportCellInfoEventArgs e)
        {
            if (QueryImportExportCellInfo != null)
                QueryImportExportCellInfo(this, e);
        }

        /// <summary>
        /// Exports the image from the grid cell to excel cell range.
        /// </summary>
        /// <param name="range">The excel cell.</param>
        /// <param name="image">The image to export.</param>
        protected void ExportImageToExcelCell(IRange range, Image image)
        {
            Image bmp = null;
            if (image != null)
            {
                bmp = image;
            }
            if (bmp != null)
            {
                int iRangeRow = range.Row;
                int iRangeColumn = range.Column;

                IPictureShape picture = range.Worksheet.Pictures.AddPicture(
                  iRangeRow, iRangeColumn, bmp);
            }
        }

        /// <summary>
        /// Exports the image from the grid cell to excel cell range.
        /// </summary>
        /// <param name="range">The excel cell.</param>
        /// <param name="gridCell">The grid cell.</param>
        protected void ExportImageToExcelCell(IRange range, GridStyleInfo gridCell)
        {
            if (gridCell.ImageList != null && gridCell.ImageList.Images.Count > 0 && gridCell.ImageIndex < gridCell.ImageList.Images.Count)
            {
                Image bmp = null;
                if (gridCell.ImageIndex < 0)
                {
                    if (gridCell.CellValue is Image)
                    {
                        bmp = gridCell.CellValue as Image;
                    }
                }
                else
                {
                    bmp = gridCell.ImageList.Images[gridCell.ImageIndex] as Image;
                }
                if (bmp != null)
                {
                    int iRangeRow = range.Row;
                    int iRangeColumn = range.Column;

                    IPictureShape picture = range.Worksheet.Pictures.AddPicture(
                      iRangeRow, iRangeColumn, bmp);
                }
            }
            else if (gridCell.CellValue is byte[])
            {
                Image bmp = ByteArrayToImage(gridCell.CellValue as byte[]);
                AddPictureToExcelCell(range, bmp);
                range.Text = string.Empty;
            }
        }

        /// <summary>
        /// Converts byte array to image
        /// </summary>
        /// <param name="byteArray">Byte array</param>
        /// <returns>Converted image from byte array</returns>
        private Image ByteArrayToImage(byte[] byteArray)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            return Image.FromStream(ms);
        }

        /// <summary>
        /// Adds a picture to the given range's Worksheet
        /// </summary>
        /// <param name="range">IRange</param>
        /// <param name="bmp">Image</param>
        private void AddPictureToExcelCell(IRange range, Image bmp)
        {
            if (bmp != null)
            {
                double rowHeight = range.Worksheet.GetRowHeightInPixels(range.Row);
                double colWidth = range.Worksheet.GetColumnWidthInPixels(range.Column);

                Bitmap bmp1 = new Bitmap((int)colWidth, (int)rowHeight);
                Graphics graphic = Graphics.FromImage((Image)bmp1);
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphic.DrawImage(bmp, 0, 0, (float)colWidth, (float)rowHeight);
                graphic.Dispose();
                range.Worksheet.Pictures.AddPicture(range.Row, range.Column, (Image)bmp1);
            }
        }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether default alignment setting should be used when alignment
    /// wasn't set. Read-only.
    /// </summary>
    public virtual bool UseDefaultAlignment
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// A property that gets or sets the exported header cell's back color in worksheet.
    /// </summary>
    public Color HeaderBackColor
    {
        get { return headerBKColor; }
        set
        {
            hasHeaderBKColor = true;
            headerBKColor = value;
        }
    }

    /// <summary>
    /// A Virtual property that gets or sets a value indicating whether the cell borders should be exported.
    /// </summary>
    public virtual bool ExportBorders
    {
        get { return exportBorders; }
        set { exportBorders = value; }
    }
    /// <summary>
    /// A Virtual property that gets or sets a value indicating whether the styles should get exported.
    /// The default value is true.
    /// </summary>
    /// <remarks>
    /// With version 5.1 the GridExcelConverter has been enhanced. 
    /// Since it export all style properties, the time taken to export a huge
    /// grid will be more relative to below versions GridConverter.
    /// If you dont want to export styles this can be set to false.
    /// </remarks>
    public virtual bool ExportStyle
    {
        get { return exportStyle; }
        set
        {
            exportStyle = value;
            if (value == false)
            {
                ExportBorders = false;
                
            }
        }
    }
      /// <summary>
    /// Gets whether the grid is a GroupingGrid
      /// </summary>
      protected virtual bool IsGroupingGrid
      {
          get { return false; }
      }
      /// <summary>
      /// Property getting and setting the grid value
      /// </summary>
      protected GridModel Grid
      {
          get { return grid; }
          set { grid = value; }
      }

      /// <summary>
      /// Allows to export the images into excel. Effective only when ExportStyle is true.
      /// </summary>
      [DefaultValue(true), Description("Allows to export the images into excel. Effective only when ExportStyle is true. Default value is true.")]
      public bool ExportImage
      {
          get
          {
              return _isExportImage;
          }
          set
          {
              this._isExportImage = value;
          }
      }
    #endregion

    //Field 
      Color headerBKColor;
      /// <summary>
      /// variable setting whether header has backcolor
      /// </summary>
    protected bool hasHeaderBKColor = false;
      bool exportBorders = true;
      GridModel grid = null;
      bool exportStyle = true;
      private bool _isExportImage = true;

        /// <summary>
        /// Occurs for each cell before the <see cref="GridExcelConverterControl"/>
        /// imports/ exports and let users customize the cell info.
        /// </summary>
        public event GridImportExportCellInfoEventHandler QueryImportExportCellInfo;
    }

    /// <summary>
    /// Specifies the current action of the <see cref="GridExcelConverterControl"/>.
    /// </summary>
    public enum GridConverterAction
    {
        /// <summary>
        /// Specifies that the event is triggered on importing Excel into GridControl.
        /// </summary>
        Import,
        /// <summary>
        /// Specifies that the event is triggered on exporting GridControl to Excel.
        /// </summary>
        Export
    }

    /// <summary>
    /// Defines the method that handles a <see cref="GridExcelConverterBase.QueryImportExportCellInfo"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="GridImportExportCellInfoEventArgs"/> that contains the event data.</param>
    public delegate void GridImportExportCellInfoEventHandler(object sender, GridImportExportCellInfoEventArgs e);

    /// <summary>
    /// A sealed class that defines data about the GridImportExportCellInfo event.
    /// </summary>
    public sealed class GridImportExportCellInfoEventArgs : Syncfusion.Windows.Forms.Grid.GridCellHandledEventArgs
    {

        /// <summary>
        /// Initializes a new object with excel cell and grid cell style info to import/ export.
        /// </summary>
        /// <param name="excelCell">Excel cell style.</param>
        /// <param name="gridCell">Grid cell stlye.</param>
        /// <param name="action">The row index.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridImportExportCellInfoEventArgs(IRange excelCell, GridStyleInfo gridCell, GridConverterAction action, int rowIndex, int colIndex)
            : base(rowIndex, colIndex)
        {
            this.gridCell = gridCell;
            this.excelCell = excelCell;
            this.action = action;

        }

        // Properties

        /// <summary>
        /// A readonly property that gets the style information for the grid cell.
        /// </summary>
        public GridStyleInfo GridCell
        {
            get
            {
                return gridCell;
            }

        }

        /// <summary>
        /// A readonly property that gets the style information of excel cell to import/ export.
        /// </summary>
        public IRange ExcelCell
        {
            get { return excelCell; }
        }

        /// <summary>
        /// A property that gets an action specifying the current action of <see cref="GridExcelConverterControl"/>.
        /// </summary>
        public GridConverterAction Action
        {
            get { return action;  }
            set { action = value;  }
        }

        // Fields
        private GridStyleInfo gridCell;
        private IRange excelCell;
        private GridConverterAction action;
  }

  /// <summary>
  /// Enum defining options for exporting data from grid/grouping grid to Excel.
  /// </summary>
  [ Flags ]
  public enum ConverterOptions
  {
    /// <summary>
    /// Default options - row / column headers won't be exported
    /// and all columns of grid will be exported.
    /// </summary>
    Default       = 0,
    /// <summary>
    /// Lets you Convert only the visible Columns of Grid to a SpreadSheet format.
    /// </summary>
    Visible       = 1,
    /// <summary>
    /// Lets you convert grid column headers into spreadsheet.
    /// </summary>
    ColumnHeaders = 2,
    /// <summary>
    /// Lets you convert grid row headers into spreadsheet.
    /// </summary>
    RowHeaders    = 4,
  }
}
