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
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Globalization;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;

using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

using Syncfusion.XlsIO.Implementation.Shapes;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using TRangeValueType = Syncfusion.XlsIO.Implementation.WorksheetImpl.TRangeValueType;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif


#if  SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#endif
#if !SILVERLIGHT && !WINRT && !WP && !(WP)
using System.Drawing;
using System.Data;
using Syncfusion.XlsIO.Implementation.Clipboard;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Range represents one cell or a rectangle of cells.
  /// </summary>
  public class RangeImpl
    : /*CommonObject
    ,*/ IRange
    , IEnumerable<IRange>
    , IReparse
    , ICombinedRange
    , ICellPositionFormat
    , INativePTG
    , IDisposable
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    /// Returns or sets the formula label type for the specified 
    /// range. Can be xlNone if the range contains no labels, or one of 
    /// the following ExcelFormulaLabel constants. Read/write ExcelFormulaLabel.
    /// </summary>
    public ExcelFormulaLabel FormulaLabel
    {
      get
      {
        return ExcelFormulaLabel.NoLabels;
      }
      set
      {
        throw new NotImplementedException();
      }
    }
    /// <summary>
    /// True if the range can be edited on a protected worksheet. 
    /// Read-only Boolean.
    /// </summary>
    public bool       AllowEdit
    {
      get
      {
        return m_bAllowEdit;
      }
    }
    /// <summary>
    /// True if text is automatically indented when the text alignment in a 
    /// cell is set to equal distribution either horizontally or vertically. 
    /// Read/write Variant.
    /// </summary>
    public object     AddIndent
    {
      get
      {
        if( IsSingleCell )
        {
          return m_style.AddIndent;
        }

        return null;
      }
      set
      {
        if( IsSingleCell )
        {
          m_style.AddIndent = ( bool )value;
        }
        else
        {
          foreach( IRange cell in Cells ) cell.AddIndent = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public object     Orientation
    {
      get
      {
        // TODO: Finish RangeImpl.Orientation getter implementation.
        if( IsSingleCell )
        {
          IExtendedFormat format = ( ( StyleImpl )( ( ( StyleImplWrapper )m_style ).Wrapped ) ).ExtFormat;
          return format.Orientation;
        }
        return null;
      }
      set
      {
        // TODO: Finish RangeImpl.Orientation setter implementation.
        if( IsSingleCell ) m_style.Orientation = ( int )value;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the columns in the specified range. 
    /// Read-only.
    /// </summary>
    public IRange     Columns
    {
      get
      {
        // TODO: Add RangeImpl.Columns getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns or sets the formula for the object, using R1C1-style 
    /// notation in the language of the user. Read/write Variant for 
    /// Range objects, read/write String for Series objects.
    /// </summary>
    public object     FormulaR1C1Local
    {
      get
      {
        // TODO: Add RangeImpl.FormulaR1C1Local getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add RangeImpl.FormulaR1C1Local setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the specified cell is part of an array formula.
    /// </summary>
    public object     HasArray
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The height of the range. Read-only Variant.
    /// </summary>
    public object     Height
    {
      get
      {
        // TODO: Add RangeImpl.Height getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the rows or columns are hidden. The specified range must 
    /// span an entire column or row. Read/write Variant.
    /// </summary>
    public object     Hidden
    {
      get
      {
        // TODO: Add RangeImpl.Hidden getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add RangeImpl.Hidden setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The distance from the left edge of column A to the left edge of the 
    /// range. If the range is discontinuous, the first area is used. If the 
    /// range is more than one column wide, the leftmost column in the range 
    /// is used. Read-only Variant.
    /// </summary>
    public object     Left
    {
      get
      {
        // TODO: Add RangeImpl.Left getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the range or style contains merged cells. Read / write Variant.
    /// </summary>
    public object     MergeCells
    {
      get
      {
        // TODO: Add RangeImpl.MergeCells setter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add RangeImpl.MergeCells setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the 
    /// next sheet or cell. Read-only.
    /// </summary>
    public IRange     Next
    {
      get
      {
        // TODO: Add RangeImpl.Next getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns a Range object that represents a range that is offset from 
    /// the specified range. Read-only.
    /// </summary>
    public IRange     Offset
    {
      get
      {
        // TODO: Add RangeImpl.Offset getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the 
    /// previous sheet or cell. Read-only.
    /// </summary>
    public IRange     Previous
    {
      get
      {
        // TODO: Add RangeImpl.Previous getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Resizes the specified range. Returns a Range object that represents 
    /// the resized range.
    /// </summary>
    public IRange     Resize
    {
      get
      {
        // TODO: Add RangeImpl.Resize getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the range is an outlining summary row or column. The range should 
    /// be a row or a column. Read-only Variant.
    /// </summary>
    public object     Summary
    {
      get
      {
        // TODO: Add RangeImpl.Summary getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The distance from the top edge of row 1 to the top edge of 
    /// the range. If the range is discontinuous, the first area is 
    /// used. If the range is more than one row high, the top (lowest 
    /// numbered) row in the range is used. Read-only Variant.
    /// </summary>
    public object     Top
    {
      get
      {
        // TODO: Add RangeImpl.Top getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the row height of the Range object equals the standard 
    /// height of the sheet. Returns NULL if the range contains more 
    /// than one row and the rows arent all the same height. 
    /// Read/write Variant.
    /// </summary>
    public object     UseStandardHeight
    {
      get
      {
        // TODO: Add RangeImpl.UseStandardHeight getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add RangeImpl.UseStandardHeight setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if the column width of the Range object equals the standard 
    /// width of the sheet. Returns NULL if the range contains more than 
    /// one column and the columns aren't all the same width. 
    /// Read/write Variant.
    /// </summary>
    public object     UseStandardWidth
    {
      get
      {
        // TODO: Add RangeImpl.UseStandardWidth getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add RangeImpl.UseStandardWidth setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// The width of the range. Read-only Variant.
    /// </summary>
    public object     Width
    {
      get
      {
        // TODO: Add RangeImpl.Width getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Adds a border to a range and sets the Color, LineStyle, and Weight 
    /// properties for the new border. Variant.
    /// </summary>
    /// <param name="LineStyle">Style of the border line.</param>
    /// <param name="Weight">Weight of the border line.</param>
    /// <param name="ColorIndex">Color index of the border line.</param>
    /// <param name="Color">Color of the border.</param>
    /// <returns></returns>
    public object BorderAround( object LineStyle, XlBorderWeight Weight, 
      XlColorIndex ColorIndex, object Color )
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Clears the entire object.
    /// </summary>
    /// <returns></returns>
    public object Clear()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Clears all cell comments from the specified range.
    /// </summary>
    public void   ClearComments()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Clears the formulas from the range. Clears the data from a chart but leaves 
    /// the formatting.
    /// </summary>
    /// <returns></returns>
    public object ClearContents()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Clears the formatting of the object.
    /// </summary>
    /// <returns></returns>
    public object ClearFormats()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Copies the range to the specified range or to the Clipboard.
    /// </summary>
    /// <param name="Destination">Destination object.</param>
    /// <returns></returns>
    public object Copy(object Destination)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Cuts the object to the Clipboard or pastes it into a specified destination.
    /// </summary>
    /// <param name="Destination"></param>
    /// <returns></returns>
    public object Cut(object Destination)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes the object.
    /// </summary>
    /// <param name="Shift"></param>
    /// <returns></returns>
    public object Delete(object Shift)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Get enumerator of cells in range.
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Inserts a cell or a range of cells into the worksheet or macro 
    /// sheet and shifts other cells away to make space.
    /// </summary>
    /// <param name="Shift"></param>
    /// <param name="CopyOrigin"></param>
    /// <returns></returns>
    public object Insert(object Shift, object CopyOrigin)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Adds an indent to the specified range.
    /// </summary>
    /// <param name="InsertAmount"></param>
    public void   InsertIndent(int InsertAmount)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Rearranges the text in a range so that it fills the range evenly.
    /// </summary>
    /// <returns></returns>
    public object Justify()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Pastes a Range from the Clipboard into the specified range.
    /// </summary>
    /// <param name="Paste"></param>
    /// <param name="Operation"></param>
    /// <param name="SkipBlanks"></param>
    /// <param name="Transpose"></param>
    /// <returns></returns>
    public object PasteSpecial( XlPasteType Paste, 
      XlPasteSpecialOperation Operation, object SkipBlanks, object Transpose )
    {
      throw new NotImplementedException();
    }

#endif
    #endregion

    #region Class constants
    /// <summary>
    /// Enumeration that contains all possible cell types for the range.
    /// </summary>
    public enum TCellType
    {
      /// <summary>
      /// Indicates that range contains NumberRecord.
      /// </summary>
      Number    = TBIFFRecord.Number,
      /// <summary>
      /// Indicates that range contains RKRecord.
      /// </summary>
      RK        = TBIFFRecord.RK,
      /// <summary>
      /// Indicates that range contains LabelSSTRecord.
      /// </summary>
      LabelSST  = TBIFFRecord.LabelSST,
      /// <summary>
      /// Indicates that range contains BlankRecord.
      /// </summary>
      Blank     = TBIFFRecord.Blank,
      /// <summary>
      /// Indicates that range contains FormulaRecord.
      /// </summary>
      Formula   = TBIFFRecord.Formula,
      /// <summary>
      /// Indicates that range contains BoolErrRecord.
      /// </summary>
      BoolErr   = TBIFFRecord.BoolErr,
      /// <summary>
      /// Indicates that range contains RStringRecord.
      /// </summary>
      RString   = TBIFFRecord.RString,
      /// <summary>
      /// Indicates that range contains LabelRecord.
      /// </summary>
      Label     = TBIFFRecord.Label,
    }
    /// <summary>
    /// Default format for date values.
    /// </summary>
    public const string DEF_DATE_FORMAT = "mm/dd/yyyy";
    /// <summary>
    /// Default format for time values.
    /// </summary>
    public const string DEF_TIME_FORMAT = "h:mm:ss";
    /// <summary>
    /// Default OleDateValue
    /// </summary>
    private const double DEF_OLE_DOUBLE = 2958465.9999999884;
    /// <summary>
    /// Maximum OleDateValue
    /// </summary>
    private const double DEF_MAX_DOUBLE = 2958466.0;
    /// <summary>
    /// Default format for number values.
    /// </summary>
    public const string DEF_NUMBER_FORMAT = "0.00";
    /// <summary>
    /// Default format for text values.
    /// </summary>
    public const string DEF_TEXT_FORMAT = "@";
    /// <summary>
    /// General format.
    /// </summary>
    public const string DEF_GENERAL_FORMAT = "General";
    /// <summary>
    /// Format for array-entered formula representation.
    /// </summary>
    internal const string DEF_FORMULAARRAY_FORMAT = "{{{0}}}";
    /// <summary>
    /// Error message when method that should be called only for single-cell ranges
    /// was called for range with multiple cells.
    /// </summary>
    private const string DEF_SINGLECELL_ERROR = "This method should be called for single cells only.";
    /// <summary>
    /// Default style.
    /// </summary>
    public const string DEF_DEFAULT_STYLE = "Normal";
    /// <summary>
    /// Index of extended format for normal style.
    /// </summary>
    internal const int DEF_NORMAL_STYLE_INDEX = 15;
    /// <summary>
    /// Default format for WrapText values.
    /// </summary>
    private const bool DEF_WRAPTEXT_VALUE = false;
    /// <summary>
    /// Whitspace for the numberformat.
    /// </summary>
    private const string DEF_EMPTY_DIGIT = " ";
    /// <summary>
    /// Cell types that can contain date time values.
    /// </summary>
    private static readonly TCellType[] DEF_DATETIMECELLTYPES = new TCellType[]
    {
      TCellType.RK,
      TCellType.Number,
      TCellType.Formula,
    };
    /// <summary>
    /// Represents auto format types, that contain right horizontal alignment.
    /// </summary>
    private static readonly ExcelAutoFormat[] DEF_AUTOFORMAT_RIGHT =
    {
      ExcelAutoFormat.Classic_2,
      ExcelAutoFormat.Classic_3,
      ExcelAutoFormat.Accounting_1,
      ExcelAutoFormat.Accounting_2,
      ExcelAutoFormat.Accounting_3,
      ExcelAutoFormat.Colorful_2,
      ExcelAutoFormat.Colorful_3
    };
    /// <summary>
    /// Represents auto format types, that contain number format.
    /// </summary>
    private static readonly ExcelAutoFormat[] DEF_AUTOFORMAT_NUMBER =
    {
      ExcelAutoFormat.Accounting_1,
      ExcelAutoFormat.Accounting_2,
      ExcelAutoFormat.Accounting_3,
      ExcelAutoFormat.Accounting_4,
    };
    /// <summary>
    /// Represents default cell name separator.
    /// </summary>
    private const char DEF_CELL_NAME_SEPARATER = '$';
    /// <summary>
    /// Column section start in the R1C1 reference string.
    /// </summary>
    private const char DEF_R1C1_COLUMN = 'C';
    /// <summary>
    /// Row section start in the R1C1 reference string.
    /// </summary>
    private const char DEF_R1C1_ROW = 'R';
    /// <summary>
    /// Opening bracket for relative row / column index in the R1C1 reference mode.
    /// </summary>
    private const char DEF_R1C1_OPENBRACKET = '[';
    /// <summary>
    /// Closing bracket for relative row / column index in the R1C1 reference mode.
    /// </summary>
    private const char DEF_R1C1_CLOSEBRACKET = ']';
    /// <summary>
    /// Local address format in R1C1 notation.
    /// </summary>
    private const string DEF_R1C1_FORMAT = "R{0}C{1}";
    /// <summary>
    /// Maximum OADate value.
    /// </summary>
    private const long DEF_MIN_OADATE = 31241376000000000;
    /// <summary>
    /// Minimum supported date time value.
    /// </summary>
    internal static readonly DateTime DEF_MIN_DATETIME = new DateTime(1900, 1, 1, 0, 0, 0, 0);
    /// <summary>
    /// Minimum supported date time value.
    /// </summary>
    private static readonly long MinAllowedDateTicks = new DateTime( 1900, 1, 1, 0, 0, 0, 0 ).Ticks;
    /// <summary>
    /// Represents default number format index.
    /// </summary>
    private const int DEF_AUTOFORMAT_NUMBER_INDEX = 0;
    /// <summary>
    /// Represents first number format index.
    /// </summary>
    private const int DEF_AUTOFORMAT_NUMBER_INDEX_1 = 43;
    /// <summary>
    /// Represents second number format index.
    /// </summary>
    private const int DEF_AUTOFORMAT_NUMBER_INDEX_2 = 44;
    /// <summary>
    /// Number of bits in cell index that holds column value.
    /// </summary>
    private const int ColumnBitsInCellIndex = 32;
    /// <summary>
    /// Defines whether to set XF index in SetFormulaArrayRecord method.
    /// </summary>
    private const int ArrayFormulaXFFlag = -1;
   
    private static readonly ExcelLineStyle[] ThinBorders = new ExcelLineStyle[]
    {
      ExcelLineStyle.None,
      ExcelLineStyle.Hair,
      ExcelLineStyle.Thin,
    };
    private const int FormulaLengthXls = 255;
    private const int FormulaLengthXlsX = 8192;
    /// <summary>
    /// Defines the singleQuote.
    /// </summary>
    internal const char SingleQuote = '\'';
    /// <summary>
    /// Defines a new line character 
    /// </summary>
    private const string NEW_LINE = "\n";
    #endregion

    #region Class members
    /// <summary>
    /// Reference on worksheet to which current range belongs to.
    /// </summary>
    private WorksheetImpl m_worksheet;
    /// <summary>
    /// Reference on workbook to which worksheet and current range belong to.
    /// </summary>
    private WorkbookImpl  m_book;
    /// <summary>
    /// Index of the left column.
    /// </summary>
    protected int m_iLeftColumn;
    /// <summary>
    /// Index of the right column.
    /// </summary>
    protected int m_iRightColumn;
    /// <summary>
    /// Index of the top row.
    /// </summary>
    protected int m_iTopRow;
    /// <summary>
    /// Index of the bottom row.
    /// </summary>
    protected int m_iBottomRow;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsNumReference;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsMultiReference;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsStringReference;
//    /// <summary>
//    /// Index of top left cell.
//    /// </summary>
//    protected int m_iTopLeftCell;
//    /// <summary>
//    /// Index of bottom right cell.
//    /// </summary>
//    protected int m_iBottomRightCell;
    ///// <summary>
    ///// The object value of the range.
    ///// </summary>
    //private object        m_objValue;
    /// <summary>
    /// This array stores references of all cells that this
    /// range represents.
    /// </summary>
    private List<IRange>     m_cells;
    /// <summary>
    /// Style wrapper for this range.
    /// </summary>
    protected CellStyle     m_style;
    /// <summary>
    /// True - indicates that cells collection was filled before, otherwise False.
    /// </summary>
    private bool          m_bCells;
    /// <summary>
    /// Represents data validation.
    /// </summary>
    protected DataValidationWrapper m_dataValidation;
    /// <summary>
    /// Represents RTF string.
    /// </summary>
    protected IRTFWrapper m_rtfString;
    private char[] unnecessaryChar = new char[] { '_', '?' ,'*'};
    private string[] osCultureSpecficFormats = new string[]
      {
          @"[$-F800]dddd\,\ mmmm\ dd\,\ yyyy",
          "m/d/yyyy",
      };
    private string[] floatNumberStyleCultures = new string[]
      {
          "de-AT",      
          "de-DE",      
          "de-CH",       
          "de-LI",       
          "de-LU",
    };
    /// <summary>
    /// Represents the UK culture Name.
    /// </summary>
    internal const string UKCultureName = "cy-GB";
    /// <summary>
    /// True if it is Entire row.
    /// </summary>
    private bool m_isEntireRow = false;
    /// <summary>
    /// True if it is Entire column.
    /// </summary>
    private bool m_isEntireColumn = false;

    internal bool updateCellValue = true;
    /// <summary>
    /// Represents the OutlineWrapperUtility object
    /// </summary>
    private OutlineWrapperUtility m_outlineWrapperUtility;
    /// <summary>
    /// Creates a new outline levels dictionary.
    /// </summary>
    private Dictionary<int, List<Point>> m_outlineLevels;
    #endregion
      
    #region IRange Properties
    internal string[] DefaultStyleNames
    {
        get
        {
            return m_book.AppImplementation.DefaultStyleNames;
        }
    }
    /// <summary>
    /// Returns the range reference in the language of the macro. 
    /// Read-only String.
    /// </summary>
    public string       Address
    {
      get
      {
        CheckDisposed();
        return m_worksheet.QuotedName + "!" + AddressLocal;
      }
    }
    /// <summary>
    /// Returns the range reference for the specified range in the language 
    /// of the user. Read-only String.
    /// </summary>
    public string       AddressLocal
    {
      get
      {
        CheckDisposed();
        return GetAddressLocal( FirstRow, FirstColumn, LastRow, LastColumn );
      }
    }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public string       AddressR1C1
    {
      get
      {
        CheckDisposed();
        return m_worksheet.QuotedName + "!" + AddressR1C1Local;
      }
    }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public string       AddressR1C1Local
    {
      get
      {
        CheckDisposed();
        string strResult = string.Format( DEF_R1C1_FORMAT, Row, Column );

        if( !IsSingleCell )
        {
          strResult += ":" + string.Format( DEF_R1C1_FORMAT, LastRow, LastColumn );
        }

        return strResult;
      }
    }
    /// <summary>
    /// Get / set boolean value that is contained by this range.
    /// </summary>
    public bool         Boolean
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( !m_worksheet.GetBoolean( i, j ) )
              return false;
          }
        }

        return true;
      }
      set
      {
          CheckDisposed();
          TryRemoveFormulaArrays();

          if (IsSingleCell)
          {
              if (Boolean != value)
                  OnCellValueChanged(Boolean, value, this);
              SetBoolean(value);
              SetChanged();
          }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.Boolean = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns a  Borders collection that represents the borders of a style 
    /// or a range of cells (including a range defined as part of a 
    /// conditional format).
    /// </summary>
    public IBorders     Borders
    {
      get
      {
        CheckDisposed();
        return CellStyle.Borders;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the cells in the specified range. 
    /// Read-only.
    /// </summary>
    public IRange[]     Cells
    {
      get
      {
        CheckDisposed();

        if( m_cells == null && !m_bCells )
          InfillCells();

        if( m_cells == null )
          throw new ArgumentNullException();

        return m_cells.ToArray();
      }
    }
    /// <summary>
    /// Returns the number of the first column in the first area of the specified 
    /// range. Read-only.
    /// </summary>
    public int          Column
    {
      get
      {
        CheckDisposed();
        return FirstColumn;
      }
    }
    /// <summary>
    /// Column group level. Read-only.
    /// -1 - not all column in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - group level.
    /// </summary>
    public int          ColumnGroupLevel
    {
      get
      {
        CheckDisposed();

        int iFirstColumn = FirstColumn;
        int iLastColumn = LastColumn;
        int iResult;

        if( iFirstColumn == iLastColumn )
        {
          ColumnInfoRecord column = m_worksheet.ColumnInformation[ iFirstColumn ];

          iResult = ( column != null )
            ? ( int )column.OutlineLevel
            : 0;
        }
        else
        {
          int iFirstRow = FirstRow;
          iResult = this[ iFirstRow, iFirstColumn ].ColumnGroupLevel;

          for( int i = iFirstColumn + 1; i <= iLastColumn; i++ )
          {
            if( iResult != this[ iFirstRow, i ].ColumnGroupLevel ) return -1;
          }
        }

        return iResult;
      }
    }
    /// <summary>
    /// Returns or sets the width of all columns in the specified range. 
    /// Read/write Double.
    /// </summary>
    public double       ColumnWidth
    {
      get
      {
        CheckDisposed();
        double dResult = double.MinValue;

        if( m_iLeftColumn == m_iRightColumn )
        {
          dResult = m_worksheet.InnerGetColumnWidth( m_iLeftColumn );
        }
        else
        {
          dResult = m_worksheet.InnerGetColumnWidth( m_iLeftColumn );

          for( int i = m_iLeftColumn + 1; i <= m_iRightColumn; i++ )
          {
            if( dResult != m_worksheet.InnerGetColumnWidth( i ) )
            {
              dResult = double.MinValue;
              break;
            }
          }
        }
        
        return dResult;
      }
      set
      {
        CheckDisposed();

        if( value < 0 || value > 255 )
          throw new ArgumentOutOfRangeException( "ColumnWidth", 
            "Column Width cannot be larger then 255 or zeroless" );

        for( int i = FirstColumn, last = LastColumn; i <= last; i++ )
        {
          m_worksheet.SetColumnWidth( i, value );
        }
      }
    }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    public int          Count
    {
      get
      {
        CheckDisposed();
        return ( LastColumn - FirstColumn + 1 ) * ( LastRow - FirstRow + 1 );
      }
    }
    /// <summary>
    /// Indicates whether specified range object has data validation.
    /// If Range is not single cell, then returns true only if all cells have data validation. Read-only.
    /// </summary>
    public bool         HasDataValidation
    {
      get
      {
        bool bResult;

        if( IsSingleCell )
        {
          bResult = FindDataValidation() != null;
        }
        else
        {
          bResult = true;
          IMigrantRange range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = m_iTopRow; iRow <= m_iBottomRow; iRow++ )
          {
            for( int iColumn = m_iLeftColumn; iColumn <= m_iRightColumn; iColumn++ )
            {
              range.ResetRowColumn( iRow, iColumn );

              if( !range.HasDataValidation )
              {
                bResult = false;
              }
            }
          }
        }

        return bResult;
      }
    }
    /// <summary>
    /// Indicates whether each cell of the range has some conditional formatting. Read-only.
    /// </summary>
    public bool HasConditionFormats
    {
      get
      {
        bool bResult;

        if( IsSingleCell )
        {
          bResult = m_worksheet.ConditionalFormats.Find( GetRectangles() ) != null;
        }
        else
        {
          bResult = true;
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = m_iTopRow; iRow <= m_iBottomRow; iRow++ )
          {
            for( int iColumn = m_iLeftColumn; iColumn <= m_iRightColumn; iColumn++ )
            {
              range.ResetRowColumn( iRow, iColumn );

              if( !range.HasConditionFormats )
              {
                bResult = false;
              }
            }
          }
        }

        return bResult;
      }
    }
    /// <summary>
    /// Get / set DateTime contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    public DateTime     DateTime
    {
      get
      {
        CheckDisposed();

        double value = m_worksheet.GetNumber( Row, Column );

        //If the provided date is before 1/1/1900, then the above value must be < 0.
        //So, the cell value which is in text format can be converted to date format
        //to display the date as it is.
        if ((value < 0) || (InnerNumberFormat.GetFormatType(value) != ExcelFormatType.DateTime))
        {
            string cellValue = m_worksheet.GetText(Row, Column);
            return Convert.ToDateTime(cellValue);
        }

        if( value == Double.NaN || InnerNumberFormat.GetFormatType( value ) != ExcelFormatType.DateTime )
          return DateTime.MinValue;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double dVal = m_worksheet.GetNumber( i, j );

            if( dVal == Double.NaN || value != dVal || InnerNumberFormat.GetFormatType( dVal )
              != ExcelFormatType.DateTime )
            {
              return DateTime.MinValue;
            }
          }
        }

        return UtilityMethods.ConvertNumberToDateTime( value,m_book.Date1904 );
      }
      set
      {
        CheckDisposed();
        if (m_book.Date1904)
        {
            double num = value.ToOADate() - WorkbookImpl.Date1904SystemDifference;
            value = 
#if ( WINRT )
                DateTimeExtension.FromOADate(num);
#else
                DateTime.FromOADate(num);
#endif
        }
        if( IsSingleCell )
        {
          FormatType = ExcelFormatType.DateTime;
            DateTime dateTime = DateTime;
          if (dateTime != value) ;
          OnCellValueChanged(dateTime, value, this);
          SetDateTime( value );
          SetChanged();
        }
        else
        {
          TryRemoveFormulaArrays();
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.DateTime = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns cell value after number format application. Read-only.
    /// </summary>
    public string       DisplayText
    {
      get
      {
        string result = string.Empty;
        updateCellValue = false;
        string calculatedValuetoChange = null;
        bool hascalculatedValue = false;
        if (((WorksheetImpl)this.Worksheet).HasSheetCalculation && (HasFormula || HasFormulaArray) && !HasFormulaDateTime)
        {
            calculatedValuetoChange = this.CalculatedValue;
            hascalculatedValue = true;
        }
        string appliedNumberFormat = InnerNumberFormat.ApplyFormat(GetDisplayString());
        if( ContainsNumber )
        {
            
            bool IsFormulaErroValue = HasFormulaErrorValue;
            string value = FormulaErrorValue;
          if (HasFormula && double.IsNaN(FormulaNumberValue) && !((WorksheetImpl)this.Worksheet).HasSheetCalculation)
          {
            if( HasFormulaBoolValue )
            {
              FormatImpl numberFormat = InnerNumberFormat;
              result = FormulaBoolValue.ToString().ToUpper();
              result = numberFormat.ApplyFormat(result);
              if (result == null)
                  result = "";
            }
            else if( HasFormulaErrorValue )
            {
              result = FormulaErrorValue;
            }
            else if( HasFormulaStringValue )
            {
              result = FormulaStringValue;
            }
          }
          else
          {
            double dNumber = GetNumber();
            FormatImpl numberFormat = InnerNumberFormat;

              if(hascalculatedValue)
                  double.TryParse(calculatedValuetoChange, out dNumber);
            
            if (double.IsNaN(dNumber) && IsFormulaErroValue)
                return value;
           

            result = null;

            if( dNumber == 0 && !m_worksheet.WindowTwo.IsDisplayZeros )
            {
              ExcelFormatType formatType = numberFormat.GetFormatType( 0 );

              if (formatType == ExcelFormatType.Number || formatType == ExcelFormatType.General)
                  result = GetDisplayString();
              else if (appliedNumberFormat.Length == 0)
                  result = string.Empty;
            }

            if (result == null)
            {
                bool bValue = false;
                bool isDateTime = numberFormat.GetFormatType(0) == ExcelFormatType.DateTime;
                
                if (isDateTime && m_book.Date1904)
                    dNumber += WorkbookImpl.Date1904SystemDifference;
                else if (isDateTime && dNumber < 60 && m_worksheet.WindowTwo.IsDisplayZeros)
                    dNumber++;
                                
                if (dNumber == 0 && isDateTime)
                {
                    DateTime dtTime = new System.DateTime();
                    if (DateTime.TryParse(calculatedValuetoChange, out dtTime))
                        dNumber = dtTime.ToOADate();
                    result = numberFormat.ApplyFormat(dNumber);
                }
                else if (bool.TryParse(calculatedValuetoChange, out bValue))
                    result = bValue.ToString().ToUpper();
                else if (isDateTime && dNumber > CultureInfo .CurrentCulture.DateTimeFormat .Calendar .MaxSupportedDateTime .ToOADate ())
                {
                    result = "######";
                }
                else if (HasFormulaErrorValue)
                {
                    result = FormulaErrorValue;
                }
                else
                    result = numberFormat.ApplyFormat(dNumber);
            }
            if (InnerNumberFormat.FormatType == ExcelFormatType.DateTime && CheckOSSpecificDateFormats(InnerNumberFormat) && result != string.Empty)
            {
                
              string cultureName = 
#if !(WINRT )
                  System.Threading.Thread.CurrentThread.CurrentCulture.Name;
#else
                  CultureInfo.CurrentCulture.Name;
#endif
              CultureInfo culture = new CultureInfo(cultureName);
              
#if !(WINRT )
           System.DateTime dt = System.DateTime.FromOADate(dNumber);
#else
              System.DateTime dt = DateTimeExtension.FromOADate(dNumber);
#endif
           if (HasFormulaErrorValue)
               result = FormulaErrorValue;
           else
              result = numberFormat.IsTimeFormat(dNumber)?
                       dt.ToString(culture)
                      :dt.ToString("d",culture);
            }
          }
        }
        else
        {
            result = appliedNumberFormat;
          if (result == null)
              result = "";
        }
        

        return result;
      }
    }

    /// <summary>
    /// Checks the OS specific formats.
    /// </summary>
    /// <param name="InnerNumberFormat">The inner number format.</param>
    /// <returns></returns>
    private bool CheckOSSpecificDateFormats(FormatImpl InnerNumberFormat)
    {
        if (InnerNumberFormat == null)
            throw new ArgumentNullException("InnerNumberFormat");

        if (Array.IndexOf(osCultureSpecficFormats, InnerNumberFormat.FormatString) >= 0 && 
#if ( WINRT )
            CultureInfo.CurrentCulture.Name
#else
            System.Threading.Thread.CurrentThread.CurrentCulture.Name 
#endif
            != "en-US")
            return true;

        return false;
    }
    /// <summary>
    /// Returns a Range object that represents the cell at the end of the 
    /// region that contains the source range.
    /// </summary>
    public IRange       End
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell ) return this;
        return m_worksheet.InnerGetCell( LastColumn, LastRow );
      }
    }
    /// <summary>
    /// True if it is Entire row.
    /// </summary>
    public bool IsEntireRow
    {
        get 
        { 
            return m_isEntireRow; 
        }
        set 
        { 
            m_isEntireRow = value; 
        }
    }
    /// <summary>
    /// True if it is Entire Column.
    /// </summary>
    public bool IsEntireColumn
    {
        get 
        { 
            return m_isEntireColumn; 
        }
        set 
        { 
            m_isEntireColumn = value; 
        }
    }
    /// <summary>
    /// Returns a Range object that represents the entire column (or 
    /// columns) that contains the specified range. Read-only.
    /// </summary>
    public IRange       EntireColumn
    {
      get
      {
        CheckDisposed();

        int iFirstRow = 1;
        int iLastRow = m_book.MaxRowCount;
        //m_worksheet.InnerGetColumnDimensions( FirstColumn, out iFirstRow, out iLastRow );

        RangeImpl impl=this[ iFirstRow, FirstColumn, iLastRow, LastColumn ] as RangeImpl;
        impl.IsEntireColumn=true;
        return impl;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the entire row (or 
    /// rows) that contains the specified range. Read-only.
    /// </summary>
    public IRange       EntireRow
    {
      get
      {
        CheckDisposed();

        int iFirstColumn = 1;
        int iLastColumn = m_book.MaxColumnCount;
        //m_worksheet.InnerGetRowDimensions( FirstRow, out iFirstColumn, out iLastColumn );

        RangeImpl impl = this[FirstRow, iFirstColumn, LastRow, iLastColumn] as RangeImpl;
        impl.IsEntireRow = true;
        return impl;
      }
    }
    /// <summary>
    /// Get / set error value that is contained by this range.
    /// </summary>
    public string       Error
    {
      get
      {
        CheckDisposed();

        string strError = m_worksheet.GetError( Row, Column );

        if( strError == null )
          return null;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( strError != m_worksheet.GetError( i, j ) )
              return null;
          }
        }

        return strError;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          SetError( value );
          SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.Error = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns or sets the object's formula in A1-style notation and in 
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public string       Formula
    {
      get
      {
        CheckDisposed();
        string strResult = null;
        
        if( IsSingleCell )
        {
          strResult = HasFormulaArray
            ? string.Format( DEF_FORMULAARRAY_FORMAT, FormulaArray )
            : m_worksheet.GetFormula( Row, Column, false );
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
          range.ResetRowColumn( Row, Column );
          strResult = range.Formula;

          if( strResult != null )
          {
            for( int i = Row, iLen = LastRow; i <= iLen; i++ )
            {
              for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
              {
                range.ResetRowColumn( i, j );
                string value = range.Formula;

                if( strResult != value )
                {
                  strResult = null;
                  break;
                }
              }
            }
          }
        }

        return strResult;
      }
      set
      {
          if (this.Workbook.Version == ExcelVersion.Excel97to2003 && value.Length > FormulaLengthXls)
              throw new ArgumentException("The formula is too long.Formulas length should not be longer then 255");
          else if (value.Length > FormulaLengthXlsX)
              throw new ArgumentException("The formula is too long.Formulas length should not be longer then 8192");
          CheckDisposed();
          TryRemoveFormulaArrays();

          if (value[0] != '=') value = '=' + value;

          Value = value;
      }
    }
    /// <summary>
    ///Represents array formula which can perform multiple calculations on one or more of the items in an array.
    /// </summary>
    public string       FormulaArray
    {
      get
      {
        CheckDisposed();
        return GetFormulaArray( false );
      }
      set
      {
        CheckDisposed();
        SetFormulaArray( value, false );
      }
    }
    /// <summary>
    /// Returns the Formula in the cell as a string.
    /// </summary>
    public string       FormulaStringValue
    {
      get
      {
        CheckDisposed();
        string strResult = m_worksheet.GetFormulaStringValue( Row, Column );
        UpdateCellValue(Parent, Column, Row, updateCellValue);
        if( !IsSingleCell && strResult != null )
        {
          for( int iRow = Row, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iColumn = Column, iLastColumn = LastColumn; iColumn <= iLastColumn; iColumn++ )
            {
              if( strResult != m_worksheet.GetFormulaStringValue( iRow, iColumn ) )
              {
                return null;
                //strResult = null;
                //break;
              }
            }
          }
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          //m_strFormulaValue = value;
          m_worksheet.CellRecords.SetStringValue( CellIndex, value );
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaStringValue = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a number.
    /// </summary>
    public double       FormulaNumberValue
    {
      get
      {
        CheckDisposed();
        double dResult = m_worksheet.GetFormulaNumberValue( Row, Column );

        if( !IsSingleCell && !Double.IsNaN( dResult ) )
        {
          for( int i = Row, iLen = LastRow; i <= iLen; i++ )
          {
            for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
            {
              double value = m_worksheet.GetFormulaNumberValue( i, j );

              if( dResult != value )
              {
                return double.NaN;
                //dResult = double.NaN;
                //break;
              }
            }
          }
        }

        return dResult;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          FormulaRecord formula = Record as FormulaRecord;

          if ( formula == null )
            throw new NotSupportedException( "This property is only for formula ranges" );

          formula.Value = value;
          Record = formula;
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaNumberValue = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a boolean.
    /// </summary>
    public bool         FormulaBoolValue
    {
      get
      {
        CheckDisposed();
        UpdateCellValue(Parent , Column ,Row ,updateCellValue  );
        //if( IsSingleCell )
        //  return m_worksheet.GetFormulaBoolValue( Row, Column );

        //return false;
        bool bResult = m_worksheet.GetFormulaBoolValue( Row, Column );

        if( !IsSingleCell && bResult )
        {
          for( int iRow = Row, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iColumn = Column, iLastColumn = LastColumn; iColumn <= iLastColumn; iColumn++ )
            {
              if( bResult != m_worksheet.GetFormulaBoolValue( iRow, iColumn ) )
              {
                return false;
                //strResult = false;
                //break;
              }
            }
          }
        }

        return bResult;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          FormulaRecord formula = Record as FormulaRecord;

          if ( formula == null )
            throw new NotSupportedException( "This property is only for formula ranges" );

          formula.BooleanValue = value;
          Record = formula;
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaBoolValue = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a string.
    /// </summary>
    public string       FormulaErrorValue
    {
      get
      {
        CheckDisposed();
        string strResult = m_worksheet.GetFormulaErrorValue( Row, Column );

        if( !IsSingleCell && strResult != null )
        {
          for( int iRow = Row, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iColumn = Column, iLastColumn = LastColumn; iColumn <= iLastColumn; iColumn++ )
            {
              if( strResult != m_worksheet.GetFormulaErrorValue( iRow, iColumn ) )
              {
                return null;
                //strResult = null;
                //break;
              }
            }
          }
        }

        return strResult;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          FormulaRecord formula = Record as FormulaRecord;

          if ( formula == null )
            throw new NotSupportedException( "This property is only for formula ranges" );

          int iCode = GetErrorCodeByString( value );

          if( iCode == -1 ) iCode = 0;

          formula.ErrorValue = ( byte )iCode;
          Record = formula;
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaErrorValue = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Gets formula value.
    /// </summary>
    public object FormulaValue
    {
      get
      {
        object result;

        if( HasFormula )
        {
          string value = FormulaStringValue;

          if( value != null )
          {
            result = value;
          }
          else if( HasFormulaDateTime )
          {
            result = FormulaDateTime;
          }
          else if( HasFormulaBoolValue )
          {
            result = FormulaBoolValue;
          }
          else if( HasFormulaErrorValue )
          {
            result = FormulaErrorValue;
          }
          else
          {
            result = FormulaNumberValue;
          }
        }
        else
        {
          result = null;
        }

        return result;
      }
    }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// False if at least part of formula in the range is not hidden.
    /// </summary>
    public bool         FormulaHidden
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell )
          return CellStyle.FormulaHidden;

        MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

        bool bResult = m_worksheet[ FirstRow, FirstColumn ].FormulaHidden;

        if( bResult )
        {
          for( int iRow = FirstRow; iRow <= LastRow && bResult; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn && bResult; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              bResult = range.FormulaHidden;
            }
          }
        }

        return bResult;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          CellStyle.FormulaHidden = value;
          //SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaHidden = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Get / set formula DateTime value contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    public DateTime     FormulaDateTime
    {
      get
      {
        CheckDisposed();

        double value = m_worksheet.GetFormulaNumberValue( Row, Column );

        if( value == Double.NaN || InnerNumberFormat.GetFormatType( value ) != ExcelFormatType.DateTime )
          return DateTime.MinValue;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double dVal = m_worksheet.GetFormulaNumberValue( i, j );

            if( dVal == Double.NaN || value != dVal || InnerNumberFormat.GetFormatType( dVal )
              != ExcelFormatType.DateTime )
            {
              return DateTime.MinValue;
            }
          }
        }

        return UtilityMethods.ConvertNumberToDateTime( value ,m_book.Date1904);
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          if( CellType != TCellType.Formula )
            throw new NotSupportedException( "This property is only for formula ranges" );

          FormatType = ExcelFormatType.DateTime;
          m_worksheet.SetFormulaNumberValue( Row, Column, value.ToOADate() );
        }
        else
        {
          //TryRemoveFormulaArrays();
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.FormulaDateTime = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns or sets the formula for the range, using R1C1-style notation.
    /// </summary>
    public string       FormulaR1C1
    {
      get
      {
        CheckDisposed();
        string strResult = null;

        if( IsSingleCell )
        {

          return HasFormulaArray
            ? string.Format( DEF_FORMULAARRAY_FORMAT, FormulaArrayR1C1 )
            : m_worksheet.GetFormula( Row, Column, true );
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
          range.ResetRowColumn( Row, Column );
          strResult = range.FormulaR1C1;

          if( strResult != null )
          {
            for( int i = Row, iLen = LastRow; i <= iLen; i++ )
            {
              for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
              {
                range.ResetRowColumn( i, j );
                string value = range.FormulaR1C1;

                if( strResult != value )
                {
                  strResult = null;
                  break;
                }
              }
            }
          }
        }

        return strResult;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value[ 0 ] == '=' ) value = value.Substring( 1 );

        for( int iRow = m_iTopRow; iRow <= m_iBottomRow; iRow++ )
        {
          for( int iColumn = m_iLeftColumn; iColumn <= m_iRightColumn; iColumn++ )
          {
            // TODO: here we can optimize - not to parse formula string several times.
            m_worksheet.SetFormula( iRow, iColumn, value, true );
          }
        }
      }
    }
    /// <summary>
    /// Returns or sets the formula array for the range, using R1C1-style notation.
    /// </summary>
    public string       FormulaArrayR1C1
    {
      get
      {
        CheckDisposed();
        return GetFormulaArray( true );
      }
      set
      {
        CheckDisposed();
        SetFormulaArray( value, true );
      }
    }
    /// <summary>
    /// True if all cells in the range contain formulas; False if 
    /// none of the cells in the range contains a formula; NULL 
    /// otherwise. Read-only Variant.
    /// </summary>
    public bool         HasFormula
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType valType = m_worksheet.GetCellType( i, j, false );

            if( valType != TRangeValueType.Formula )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether range contains array-entered formula. Read-only.
    /// </summary>
    public bool         HasFormulaArray
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( !m_worksheet.HasArrayFormulaRecord( i, j ) )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object. 
    /// Read/write ExcelHAlign.
    /// </summary>
    public ExcelHAlign  HorizontalAlignment
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell ) return CellStyle.HorizontalAlignment;
        
        return ExcelHAlign.HAlignGeneral;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          CellStyle.HorizontalAlignment = value;
          //SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.HorizontalAlignment = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns hyperlinks for this range.
    /// </summary>
    public IHyperLinks  Hyperlinks
    {
      get
      {
        HyperLinksCollection hyperLinksCollection = ( HyperLinksCollection )m_worksheet.HyperLinks;

        return hyperLinksCollection.GetRangeHyperlinks( this );
      }
    }
    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an integer
    /// from 0 to 15 for Excel 97-2003 and 250 for Excel 2007. Read/write Integer.
    /// </summary>
    public int          IndentLevel
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell ) return CellStyle.IndentLevel;

        return int.MinValue;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          CellStyle.IndentLevel = ( ushort )value;
          //SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.IndentLevel = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Indicates whether range contains boolean value.
    /// </summary>
    public bool         IsBoolean
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType valType = m_worksheet.GetCellType( i, j, false );

            if( valType != TRangeValueType.Boolean )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether range contains error value.
    /// </summary>
    public bool         IsError
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType valType = m_worksheet.GetCellType( i, j, false );

            if( valType != TRangeValueType.Error )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether this range is grouped by column. Read-only.
    /// </summary>
    public bool         IsGroupedByColumn
    {
      get
      {
        CheckDisposed();

        int iFirstCol = FirstColumn;
        int iLastCol = LastColumn;

        if( iFirstCol == iLastCol )
        {
          ColumnInfoRecord column = /*( ColumnInfoRecord )*/m_worksheet.ColumnInformation[ iFirstCol ];

          return ( column != null )
            ? column.OutlineLevel != 0
            : false;
        }

        int iFirstRow = FirstRow;

        for( int i = iFirstCol; i <= iLastCol; i++ )
        {
          if( !this[ iFirstRow, i ].IsGroupedByColumn ) return false;
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether this range is grouped by row. Read-only.
    /// </summary>
    public bool         IsGroupedByRow
    {
      get
      {
        CheckDisposed();

        int iFirstRow = FirstRow;
        int iLastRow = LastRow;

        if( iFirstRow == iLastRow )
        {
          IOutline row = WorksheetHelper.GetRowOutline( m_worksheet, iFirstRow );

          return( row != null )
            ? ( row.OutlineLevel != 0 )
            : false;
        }

        int iFirstCol = FirstColumn;

        for( int i = iFirstRow; i <= iLastRow; i++ )
        {
          if( !this[ i, iFirstCol ].IsGroupedByRow ) return false;
        }

        return true;
      }
    }
    /// <summary>
    /// Gets / sets last column of the range.
    /// </summary>
    public int          LastColumn
    {
      [ DebuggerStepThrough ]
      get
      {
        //return GetColumnFromCellIndex( m_iBottomRightCell );
        return m_iRightColumn;
      }
      set
      {
        if( value < 1 || value > m_book.MaxColumnCount )
          throw new ArgumentOutOfRangeException( "FirstRow" );

        if( value != LastColumn )
        {
          //m_iBottomRightCell = GetCellIndex( value, LastRow );
          m_iRightColumn = value;
          OnLastColumnChanged();
        }
      }
    }
    /// <summary>
    /// Gets / sets last row of the range.
    /// </summary>
    public int          LastRow
    {
      [ DebuggerStepThrough ]
      get
      {
        //return GetRowFromCellIndex( m_iBottomRightCell );
        return m_iBottomRow;
      }
      set
      {
        if( value < 1 || value > m_book.MaxRowCount )
          throw new ArgumentOutOfRangeException( "FirstRow" );

        if( value != LastRow )
        {
          //m_iBottomRightCell = GetCellIndex( LastColumn, value );
          m_iBottomRow = value;
          OnLastRowChanged();
        }
      }
    }
    /// <summary>
    /// Get / set number value that is contained by this range
    /// </summary>
    /// <exception cref="System.FormatException">
    /// When range value is not a number.
    /// </exception>
    public double       Number
    {
      get
      {
        CheckDisposed();

        double dValue = m_worksheet.GetNumber( Row, Column );

        if( dValue == Double.NaN )
          return dValue;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if(( dValue != m_worksheet.GetNumber( i, j ) ) && double.IsNaN(dValue) ) 
              return double.NaN;
          }
        }

        return dValue;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
            if (BitConverter.DoubleToInt64Bits(value)== BitConverter.DoubleToInt64Bits(-0.0))
                value = 0;
//          if( NumberFormat != DEF_GENERAL_FORMAT )
//          {
//            FormatType = ExcelFormatType.Number;
//          }
//
//          SetNumber( value );
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                double number = Number;
                if (number != value)
                    OnCellValueChanged(number, value, this);

                if (m_rtfString == null)
                {
                    CreateRichTextString();
                }

                m_rtfString.BeginUpdate();
                m_rtfString.Text = "#N/A";
                string strNumberFormat = NumberFormat;

                if (strNumberFormat != DEF_GENERAL_FORMAT)
                {
                    NumberFormat = DEF_TEXT_FORMAT;
                }
                m_rtfString.ClearFormatting();
                m_rtfString.EndUpdate();

                SetChanged();
            }
            else
            {
                
                double number = Number;
                if (number != value)
                    OnCellValueChanged(number, value, this);
                SetNumberAndFormat(value,false );

                SetChanged();
            }
        }
        else
        {
          TryRemoveFormulaArrays();
          for( int iRow = FirstRow, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iCol = FirstColumn, iLastCol = LastColumn; iCol <= iLastCol; iCol++ )
            {
              this[ iRow, iCol ].Number = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns or sets the format code for the object. Returns NULL 
    /// if all cells in the specified range don't have the same number 
    /// format. Read/write String.
    /// </summary>
    public string       NumberFormat
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell ) return GetNumberFormat();
        
        return GetNumberFormat( CellsList );
      }
      set
      {
        CheckDisposed();
          
        value = FormatParser.FormatTokens.AmPmToken.CheckAndApplyAMPM(value);
        if( IsSingleCell )
        {
          CellStyle.NumberFormat  = value;
          SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.NumberFormat = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns the number of the first row of the first area in 
    /// the range. Read-only Long.
    /// </summary>
    public int          Row
    {
      get
      {
        CheckDisposed();
        return FirstRow;
      }
    }
    /// <summary>
    /// Row group level. Read-only.
    /// -1 - not all row in the range have same group level.
    /// 0 - No grouping.
    /// 1 - 7 - group level.
    /// </summary>
    public int          RowGroupLevel
    {
      get
      {
        CheckDisposed();

        int iFirstRow = FirstRow;
        int iLastRow = LastRow;

        if( iFirstRow == iLastRow )
        {
          IOutline row = WorksheetHelper.GetOrCreateRow( m_worksheet, iFirstRow - 1, false );

          return ( row != null )
            ? ( int )row.OutlineLevel
            : 0;
        }

        int iFirstCol = FirstColumn;
        int iResult = this[ iFirstRow, iFirstCol ].RowGroupLevel;

        for( int i = iFirstRow + 1; i <= iLastRow; i++ )
        {
          if( iResult != this[ i, iFirstCol ].RowGroupLevel ) return -1;
        }

        return iResult;
      }
    }
    /// <summary>
    /// Returns the height of all the rows in the range specified, 
    /// measured in points. Returns Double.MinValue if the rows in the specified range 
    /// arent all the same height. Read / write. Double. Maximum Row height can be 409 
    /// value, minimum is zero.
    /// </summary>
    public double       RowHeight
    {
      get
      {
        CheckDisposed();
        double dResult = double.MinValue;

        if( m_iTopRow == m_iBottomRow )
        {
          dResult = m_worksheet.GetRowHeight( Row );
        }
        else
        {
          dResult = m_worksheet.GetRowHeight( m_iTopRow );

          for( int i = m_iTopRow + 1; i <= m_iBottomRow; i++ )
          {
            if( dResult != m_worksheet.GetRowHeight( i ) )
            {
              dResult = double.MinValue;
              break;
            }
          }
        }
        
        return dResult;
      }
      set
      {
        CheckDisposed();
        SetRowHeight( value, true );
      }
    }
    /// <summary>
    /// For a Range object, it returns an array of Range objects that represent the 
    /// rows in the specified range.
    /// </summary>
    public IRange[]     Rows
    {
      get
      {
        CheckDisposed();

        int iCount = ( FirstColumn != 0 && LastColumn != 0 && LastRow != 0 ) ?
          LastRow - FirstRow + 1 :
          0;

        IRange[] result = new IRange[ iCount ];

        if( iCount > 0 )
        {
          for( int i = FirstRow; i <= LastRow; i++ )
          {
            result[ i - FirstRow ] = m_worksheet.Range[ i, FirstColumn, i, LastColumn ];
          }
        }

        return result;
      }
    }
    /// <summary>
    /// For a Range object, it returns an array of Range objects that represent the 
    /// columns in the specified range.
    /// </summary>
    public IRange[]     Columns
    {
      get
      {
        CheckDisposed();

        if( FirstColumn == 0 || FirstColumn > m_book.MaxColumnCount ) return new IRange[]{};

        IRange[] result = new IRange[ LastColumn - FirstColumn + 1 ];

        for( int i = FirstColumn; i <= LastColumn; i++ )
        {
          result[ i - FirstColumn ] = m_worksheet.Range[ FirstRow, i, LastRow, i ];
        }

        return result;
      }
    }
    /// <summary>
    /// Create style for Entire row or Entire column.
    /// </summary>
    private IStyle CreateStyleForEntireRowEntireColumn()
    {
        List<IRange> lstRange = new List<IRange>();

        if (m_style == null)
        {
            CreateStyle();
            lstRange.Add(this);
        }
        else
            return m_style;

        if (IsEntireRow)
        {
            for (int i = Row; i <= LastRow; i++)
            {
                for (int j = Column; j <= LastColumn; j++)
                {
                    ICellPositionFormat cellrecord = m_worksheet.CellRecords.GetCellRecord(i, j);
                    if (cellrecord != null)
                    {
                        RangeImpl rang = this[i, j] as RangeImpl;
                        lstRange.Add(rang);
                    }
                }
            }
        }

        if (IsEntireColumn)
        {
            for (int i = Column; i <= LastColumn; i++)
            {
                for (int j = Row; j <= LastRow; j++)
                {
                    ICellPositionFormat cellrecord = m_worksheet.CellRecords.GetCellRecord(j, i);
                    if (cellrecord != null)
                    {
                        RangeImpl rang = this[j, i] as RangeImpl;
                        lstRange.Add(rang);
                    }
                }
            }
        }

        if (lstRange.Count == 1)
            return m_style;
        else
            return new StyleArrayWrapper(Application,lstRange,m_worksheet);
    }
    /// <summary>
    /// Returns a Style object that represents the style of the specified 
    /// range. Read/write Variant.
    /// </summary>
    public IStyle       CellStyle
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          if( m_style == null ) CreateStyle();

          return m_style;
        }
        
        if (IsEntireRow || IsEntireColumn)
            return CreateStyleForEntireRowEntireColumn();

        return new StyleArrayWrapper( this );
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          SetChanged();
          TCellType oldCellType = this.CellType;
          ushort iXFIndex;

          if( value is ExtendedFormatWrapper )
          {
            iXFIndex = ( ushort )( value as ExtendedFormatWrapper ).Wrapped.Index;
          }
          else
          {
            string strStyleName = ( value == null )
              ? DefaultStyleNames[ 0 ]
              : value.Name;

            StyleImpl style = ( StyleImpl )m_book.Styles[ strStyleName ];
            iXFIndex = ( ushort )style.Wrapped.Index;
          }

          ExtendedFormatIndex = iXFIndex;

          // TODO: Formatting must be updated after style change.
          BiffRecordRaw record = Record;

          if( record != null && record.TypeCode == TBIFFRecord.Formula || record == null )
          {
            string strValue = Value;
            OnValueChanged( strValue, strValue );
          }

          OnStyleChanged( oldCellType );
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.CellStyle = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Gets / sets name of the style for the current range.
    /// </summary>
    public string       CellStyleName
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          return GetStyleName();
        }

        return GetCellStyleName( CellsList );
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          TCellType oldCellType = this.CellType;

          string strStyleName = ( value == null )
            ? DefaultStyleNames[ 0 ]
            : value;

          ChangeStyleName( value );

          string strValue = Value;
          OnValueChanged( strValue, strValue );
          OnStyleChanged( oldCellType );
          SetChanged();
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.CellStyleName = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Gets/sets built in style.
    /// </summary>
    public BuiltInStyles? BuiltInStyle
    {
      get
      {
        string styleName = CellStyleName;
        return ( BuiltInStyles )Array.IndexOf( DefaultStyleNames, styleName );
      }
      set
      {
        string styleName = DefaultStyleNames[ ( int )value ];
        CellStyleName = styleName;
      }
    }
    /// <summary>
    /// Gets / sets text contained by this cell.
    /// </summary>
    public string       Text
    {
      get
      {
        CheckDisposed();

        string strRes = m_worksheet.GetText( Row, Column );

        if( strRes == null )
          return null;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( strRes != m_worksheet.GetText( i, j ) )
              return null;
          }
        }
        if (ExtendedFormat.IsFirstSymbolApostrophe)
            strRes = "'" + strRes;
        return strRes;
      }
      set
      {
        CheckDisposed();

        if( value == null )
          throw new ArgumentNullException( "Text" );

        TryRemoveFormulaArrays();

        if( IsSingleCell )
        {
          if( value.Length == 0 )
          {
            Value = value;
          }
          else
          {
              if (Text != value)
                  OnCellValueChanged(Text, value, this);
            value = CheckApostrophe( value );

            if( m_rtfString == null )
            {
              CreateRichTextString();
            }

            m_rtfString.BeginUpdate();
            m_rtfString.Text = value;
            string strNumberFormat = NumberFormat;

            if( strNumberFormat != DEF_GENERAL_FORMAT )
            {
              NumberFormat = DEF_TEXT_FORMAT;
            }
            //FormatType = ExcelFormatType.Text;
            m_rtfString.EndUpdate();

            SetChanged();
          }
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.Text = value;
            }
          }
        }
        if (value.Contains(Environment.NewLine) || value.Contains(NEW_LINE))
            this.WrapText = true;
      }
    }
    /// <summary>
    /// Gets / sets TimeSpan contained by this cell.
    /// </summary>
    public TimeSpan     TimeSpan
    {
      get
      {
        CheckDisposed();

        double value = m_worksheet.GetNumber( Row, Column );

        if( value == Double.NaN || InnerNumberFormat.GetFormatType( value ) != ExcelFormatType.DateTime )
          return TimeSpan.MinValue;

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double dVal = m_worksheet.GetNumber( i, j );

            if( dVal == Double.NaN || value != dVal || InnerNumberFormat.GetFormatType( dVal )
              != ExcelFormatType.DateTime )
            {
              return TimeSpan.MinValue;
            }
          }
        }

        if (value < DEF_MAX_DOUBLE)
            return TimeSpan.FromDays(value);
        else
            return TimeSpan.FromDays(DEF_OLE_DOUBLE);
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          FormatType = ExcelFormatType.DateTime;
            TimeSpan span = TimeSpan;
          if (span != value)
              OnCellValueChanged(span, value, this);
          SetTimeSpan( value );
          SetChanged();
        }
        else
        {
          TryRemoveFormulaArrays();
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.TimeSpan = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns or sets the value of the specified range. 
    /// Read/write Variant.
    /// </summary>
    public string Value
    {
        get
        {
            CheckDisposed();
            string strResult = null;

            if (IsSingleCell)
            {
                strResult = m_worksheet.GetValue(Record as ICellPositionFormat, false);//GetStringValue();
            }
            else
            {
                MigrantRangeImpl range = new MigrantRangeImpl(Application, m_worksheet);
                range.ResetRowColumn(Row, Column);
                strResult = range.Value;

                for (int i = Row, iLen = LastRow; i <= iLen; i++)
                {
                    for (int j = Column, iCount = LastColumn; j <= iCount; j++)
                    {
                        range.ResetRowColumn(i, j);
                        string value = range.Value;

                        if (strResult != value)
                        {
                            strResult = null;
                            break;
                        }
                    }
                }
            }

            return strResult;
        }
        set
        {
            CheckDisposed();
            TryRemoveFormulaArrays();

            if (IsSingleCell)
            {
                string strValue = Value;
                if (value != strValue)
                {
                    OnValueChanged(strValue, value);
                    //            m_strValue = value;
                }
            }
            else
            {
                MigrantRangeImpl range = new MigrantRangeImpl(Application, m_worksheet);

                for (int iRow = FirstRow; iRow <= LastRow; iRow++)
                {
                    for (int iCol = FirstColumn; iCol <= LastColumn; iCol++)
                    {
                        range.ResetRowColumn(iRow, iCol);
                        range.Value = value;
                    }
                }
            }
            if (value != null)
                if (value.Contains(Environment.NewLine) || value.Contains(NEW_LINE))
                    this.WrapText = true;
        }
    }
    
    /// <summary>
    /// Returns the calculated value of a formula using the most current inputs.
    /// </summary>
    public string CalculatedValue
    {
        get
        {
            if (Parent is IWorksheet && ((IWorksheet)Parent).CalcEngine != null)
            {
                string cellRef = Syncfusion.Calculate.RangeInfo.GetAlphaLabel(Column) + Row.ToString();
                return ((IWorksheet)Parent).CalcEngine.PullUpdatedValue(cellRef);
            }
            return null;
        }
    }

    /// <summary>
    /// Returns or sets the cell value. Read/write Variant.
    /// The only difference between this property and the Value property is 
    /// that the Value2 property doesn't use the Currency and Date data types.
    /// </summary>
    public object       Value2
    {
      get
      {
        object objResult = null;
        CheckDisposed();

        //if( IsSingleCell ) return m_strValue;
        //if( IsSingleCell )
        {
          objResult = TryCreateValue2();

          if( objResult == null )
          {
            objResult = Value;
          }
        }

        return objResult;
      }
      set
      {
        CheckDisposed();
        TryRemoveFormulaArrays();



        if( IsSingleCell )
        {
          SetSingleCellValue2( value );
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.Value2 = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is num reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is num reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsNumReference
    {
        get
        {
            return m_bIsNumReference;
        }
        set
        {
            m_bIsNumReference = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is a string reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is string reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsStringReference
    {
        get
        {
            return m_bIsStringReference;
        }
        set
        {
            m_bIsStringReference = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is multi reference for chart axis.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is multi reference; otherwise, <c>false</c>.
    /// </value>
    internal bool IsMultiReference
    {
        get
        {
            return m_bIsMultiReference;
        }
        set
        {
            m_bIsMultiReference = value;
        }
    }
      
    /// <summary>
    /// Returns or sets the vertical alignment of the specified object. 
    /// Read/write ExcelVAlign.
    /// </summary>
    public ExcelVAlign  VerticalAlignment
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell ) return CellStyle.VerticalAlignment;

        return ExcelVAlign.VAlignBottom;
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          CellStyle.VerticalAlignment = ( ExcelVAlign )value;
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.VerticalAlignment = value;
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns a worksheet object that represents the worksheet 
    /// containing the specified range. Read-only.
    /// </summary>
    public IWorksheet   Worksheet
    {
      get
      {
        CheckDisposed();
        return m_worksheet;
      }
    }
    /// <summary>
    /// Gets or sets cell by row and column index. Row and column indexes are one-based.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        CheckDisposed();
        CheckRange( row, column );

        return m_worksheet.InnerGetCell( column, row );
      }
      set
      {
        CheckDisposed();
        CheckRange( row, column );

        m_worksheet.InnerSetCell( column, row, ( RangeImpl )value );
        SetChanged();
      }
    }
    /// <summary>
    /// Get cell range. Row and column indexes are one-based. Read-only.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        CheckDisposed();

        row = NormalizeRowIndex( row, column, lastColumn );
        lastRow = NormalizeRowIndex( lastRow, column, lastColumn );
        column = NormalizeColumnIndex( column, row, lastRow );
        lastColumn = NormalizeColumnIndex( lastColumn, row, lastRow );

        CheckRange( row, column );
        CheckRange( lastRow, lastColumn );

        return ( row == lastRow && column == lastColumn )
          ? this[ row, column ]
          : AppImplementation.CreateRange( Parent, column, row, lastColumn, lastRow );
      }
    }
    /// <summary>
    /// Gets cell range. Read-only.
    /// </summary>
    public IRange this[ string name ]
    {
      get
      {
        return this[ name, false ];
      }
    }
    /// <summary>
    /// Gets cell range. Read-only.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        CheckDisposed();
        string strSheetName = GetWorksheetName( ref name );

        if( strSheetName != null && m_worksheet.Name != strSheetName )
          return FindWorksheet( strSheetName ).Range[ name ];

        // Check if it is a named range in the current sheet else check in the workbook.
        IName result = m_worksheet.Names[ name ]; 

        if( result != null )
          return result.RefersToRange;

        result = m_book.Names[ name ];

        if( result != null )
          return result.RefersToRange;

        name = name.ToUpper();

        IRange range;

        if( IsR1C1Notation )
        {
          range = ParseR1C1Reference( name );
        }
        else
        {
          int iFirstRow, iFirstColumn, iLastRow, iLastColumn;

          int iCount = ParseRangeString( name, Workbook, out iFirstRow, out iFirstColumn,
            out iLastRow, out iLastColumn );

          if( iCount == 1 )
          {
            range = this[ iFirstRow, iFirstColumn ];
          }
          else if( iCount == 2 )
          {
            range = this[ iFirstRow, iFirstColumn, iLastRow, iLastColumn ];
          }
          else
          {
            throw new ArgumentException();
          }
        }

        return range;
      }
    }
    /// <summary>
    /// Collection of conditional formats for the range.
    /// </summary>
    public IConditionalFormats ConditionalFormats
    {
      get
      {
        m_worksheet.ParseSheetCF();

        CheckDisposed();

        return AppImplementation.CreateCondFormatCollectionWrapper( this );

//        if( IsSingleCell )
//        {
//          if( InternalConditionalFormats == null )
//          {
//            InternalConditionalFormats = AppImplementation.CreateConditionalFormats( this, this.CellIndex );
//          }
//
//          return InternalConditionalFormats;
//        }
//        else
//        {
//          return AppImplementation.CreateCondFormatCollectionWrapper( this );
//        }
      }
    }
    /// <summary>
    /// Gets data validation.
    /// </summary>
    public IDataValidation     DataValidation
    {
      get
      {
        CheckDisposed();

        if (IsSingleCell)
        {
          if( m_dataValidation == null )
          {
            DataValidationImpl dv = FindDataValidation();
            m_dataValidation = AppImplementation.CreateDataValidationWrapper( 
              this, dv );
          }

          return m_dataValidation;
        }
        else
        {
            return AppImplementation.CreateDataValidationArrayImpl(this);
        }
      }
    }
    /// <summary>
    /// Indicates if current range has formula bool value. Read-only.
    /// </summary>
    public bool HasFormulaBoolValue
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType type = m_worksheet.GetCellType( i, j, true );

            if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula
              || ( type & TRangeValueType.Boolean ) != TRangeValueType.Boolean )
            {
              return false;
            }
          }
        }

        return true;
      }
    }
    
    /// <summary>
    /// Indicates if current range has formula error value. Read-only.
    /// </summary>
    public bool HasFormulaErrorValue
    {
      get
      {
        CheckDisposed();
        UpdateCellValue(Parent, Column, Row,updateCellValue);
        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType type = m_worksheet.GetCellType( i, j, true );

            if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula
              || ( type & TRangeValueType.Error ) != TRangeValueType.Error )
            {
              return false;
            }
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates if current range has formula value formatted as DateTime. Read-only.
    /// </summary>
    public bool HasFormulaDateTime
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double value = m_worksheet.GetFormulaNumberValue( i, j );

            if( !Double.IsNaN( value ) )
            {
              ExcelFormatType formatType = InnerNumberFormat.GetFormatType( value );

              if( formatType == ExcelFormatType.DateTime )
                continue;
            }

            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether current range has formula number value. Read-only.
    /// </summary>
    public bool HasFormulaNumberValue
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double value = m_worksheet.GetFormulaNumberValue( i, j );

            if( !Double.IsNaN( value ) )
            {
              ExcelFormatType formatType = InnerNumberFormat.GetFormatType( value );

              if( formatType != ExcelFormatType.DateTime )
                continue;
            }
            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether current range has formula value evaluated as string. Read-only.
    /// </summary>
    public bool HasFormulaStringValue
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( m_worksheet[ i, j ].FormulaStringValue == null )
            {
              return false;
            }
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether the range is blank. Read-only.
    /// </summary>
    public bool   IsBlank
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( m_worksheet.GetCellType( i, j, false ) != TRangeValueType.Blank )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether the range has value or style. Read-only.
    /// </summary>
    public bool IsBlankorHasStyle
    {
        get
        {
            CheckDisposed();

            for (int i = Row, iLen = LastRow; i <= iLen; i++)
            {
                for (int j = Column, iCount = LastColumn; j <= iCount; j++)
                {
                    if (m_worksheet.GetCellType(i, j, false) != TRangeValueType.Blank || (this.Worksheet[i, j].HasStyle && this.Worksheet[i, j].CellStyle.FillPattern != ExcelPattern.None) || this.Worksheet[i, j].CellStyle.HasBorder)
                        return false;
                }
            }

            return true;
        }
    }
    /// <summary>
    /// Indicates whether range contains bool value. Read-only.
    /// </summary>
    public bool HasBoolean
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType valType = m_worksheet.GetCellType( i, j, false );

            if( valType != TRangeValueType.Boolean )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether cell contains DateTime value. Read-only.
    /// </summary>
    public bool   HasDateTime
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double value = m_worksheet.GetNumber( i, j );

            if( !Double.IsNaN( value ) )
            {
              ExcelFormatType formatType = InnerNumberFormat.GetFormatType( value );

              if( formatType == ExcelFormatType.DateTime )
                continue;
            }

            return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether the range contains number. Read-only.
    /// </summary>
    public bool   HasNumber
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            double value = m_worksheet.GetNumber( i, j );

            if( !Double.IsNaN( value ) )//value != Double.NaN )
            {
              ExcelFormatType formatType = InnerNumberFormat.GetFormatType( value );

              if( formatType == ExcelFormatType.Unknown && value == 0 )
              {
                formatType = InnerNumberFormat.GetFormatType( 1 ); // any positive number.
              }

              if (formatType == ExcelFormatType.DateTime || formatType==ExcelFormatType.Text)
              {
                  continue;
              }
              else
              {
                  return true;
              }
            }
          }
        }

        return false;
      }
    }
    /// <summary>
    /// Indicates whether the range contains string. Read-only.
    /// </summary>
    public bool   HasString
    {
      get
      {
        CheckDisposed();

        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            TRangeValueType valType = m_worksheet.GetCellType( i, j, false );

            if( valType != TRangeValueType.String )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Comment applied to this range.
    /// </summary>
    public ICommentShape Comment
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          return m_worksheet.InnerComments[ FirstRow, FirstColumn ];
        }

        return ( ( ApplicationImpl ) Application ).CreateCommentsRange( this );
      }
    }
    /// <summary>
    /// Gets rich text.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        CheckDisposed();

        if( m_rtfString == null )
        {
          CreateRichTextString();
        }

        return m_rtfString;
      }
    }
    /// <summary>
    /// Indicates whether cell contains formatted rich text string.
    /// </summary>
    public bool HasRichText
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell && HasString )
        {
          return RichText.IsFormatted;
        }

        return false;
      }
    }
    /// <summary>
    /// Checks whether this range is part of merged range.
    /// </summary>
    public bool   IsMerged
    {
      get
      {
        CheckDisposed();

        Rectangle rectTopLeft = new Rectangle( FirstColumn - 1, FirstRow - 1, 0, 0 );

        if( IsSingleCell )
        {
          return m_worksheet.MergeCells[ rectTopLeft ] != null;
        }

        Rectangle rectBottomRight = new Rectangle( LastColumn - 1, LastRow - 1, 0, 0 );
        MergeRegion regionTopLeft = m_worksheet.MergeCells[ rectTopLeft ];
        MergeRegion regionBottomRight = m_worksheet.MergeCells[ rectBottomRight ];

        if (regionTopLeft != null || regionBottomRight != null)
            return true;
        else
            return false;
        //return regionTopLeft != null && regionTopLeft.Equals( regionBottomRight );
      }
    }
    /// <summary>
    /// Returns a Range object that represents the merged range containing
    /// the specified cell. If the specified cell isn�t in a merged range,
    /// this property returns NULL. Read-only.
    /// </summary>
    public IRange MergeArea
    {
      get
      {
        CheckDisposed();

        MergeRegion region = ParentMergeRegion;

        if( region == null ) return null;

        return m_worksheet[ region.RowFrom + 1, region.ColumnFrom + 1, region.RowTo + 1,
          region.ColumnTo + 1 ];
      }
    }
    /// <summary>
    /// Indicates whether cell is initialized. Read-only.
    /// </summary>
    public bool   IsInitialized
    {
      get
      {
        CheckDisposed();
        return !( IsBlank && !HasStyle );
      }
    }
    /// <summary>
    /// Indicates whether range's style differs from default style.
    /// Read-only.
    /// </summary>
    public bool   HasStyle
    {
      get
      {
        CheckDisposed();

        bool bStylePresent = ( m_style != null && m_style.IsInitialized );
        int iXFIndex = ExtendedFormatIndex;
        bool bXFIndexChanged = ( iXFIndex != 0 && iXFIndex != m_book.DefaultXFIndex );

        return bStylePresent || bXFIndexChanged;
      }
    }
    /// <summary>
    /// True if Microsoft Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    public bool   WrapText
    {
      get
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          return GetWrapText();
        }
        else
        {
          return GetWrapText( CellsList );
        }
      }
      set
      {
        CheckDisposed();

        if( IsSingleCell )
        {
          CellStyle.WrapText = value;

          if (Value.Length > 32000)
              throw new Exception("Text length must be less than or equal to 32K.");

          RowStorage row = WorksheetHelper.GetOrCreateRow(this.Worksheet as IInternalWorksheet,Row-1, false);
          if (row != null && !row.IsBadFontHeight && !this.Workbook.Loading )
              this.Worksheet.AutofitRow(Row);
        }
        else
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
          {
            for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
            {
              range.ResetRowColumn( iRow, iCol );
              range.WrapText = value;
            }
          }
        }

        SetChanged();
      }
    }
    /// <summary>
    /// Represents ignore error options. If not single cell returns concatenated flags.
    /// </summary>
    public ExcelIgnoreError IgnoreErrorOptions
    {
      get
      {
        ErrorIndicatorsCollection errorIndicators = m_worksheet.ErrorIndicators;
        Rectangle[] arrRanges = GetRectangles();
        ErrorIndicatorImpl errorIndicator = errorIndicators.Find( arrRanges );
        return ( errorIndicator != null ) ? errorIndicator.IgnoreOptions : ExcelIgnoreError.None;
      }
      set
      {
        ErrorIndicatorsCollection errorIndicators = m_worksheet.ErrorIndicators;
        Rectangle range = Rectangle.FromLTRB( Column - 1, Row - 1, LastColumn - 1, LastRow - 1 );

        if( value == ExcelIgnoreError.None )
        {
          errorIndicators.Remove( new Rectangle[ 1 ] { range } );
        }
        else
        {
          ErrorIndicatorImpl errorIndicator = new ErrorIndicatorImpl( range, value );
          errorIndicators.Add( errorIndicator );
        }

        //ErrorIndicatorImpl errorIndicator = new ErrorIndicatorImpl( this, value );

        //for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        //{
        //  for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
        //  {
        //    Rectangle range = new Rectangle( j - 1, i - 1, 0, 0 );

        //    if( value == ExcelIgnoreError.None )
        //    {
        //      errorIndicators.Remove( new Rectangle[ 1 ] { range } );
        //    }
        //    else
        //    {
        //      ErrorIndicatorImpl errorIndicator = new ErrorIndicatorImpl( range, value );
        //      errorIndicators.Add( errorIndicator );
        //    }
        //  }
        //}
      }
    }
    /// <summary>
    /// Indicates is current range has external formula. Read-only.
    /// </summary>
    public bool HasExternalFormula
    {
      get
      {
        for( int i = Row, iLen = LastRow; i <= iLen; i++ )
        {
          for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
          {
            if( !m_worksheet.IsExternalFormula( i, j ) )
              return false;
          }
        }

        return true;
      }
    }
    /// <summary>
    /// Indicates whether all values in the range are preserved as strings.
    /// </summary>
    public bool? IsStringsPreserved
    {
      get
      {
        return m_worksheet.GetStringPreservedValue( this );
      }
      set
      {
        m_worksheet.SetStringPreservedValue( this, value );
      }
    }
    #endregion 

    #region IParentApplication Properties
    /// <summary>
    /// Application object for this object.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return m_worksheet.Application;
      }
    }
    /// <summary>
    /// Parent object for this object.
    /// </summary>
    public object Parent
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Application object for this object.
    /// </summary>
    private ApplicationImpl AppImplementation
    {
      get
      {
        return m_worksheet.AppImplementation;
      }
    }
    #endregion

    #region Implementation class properties
    /// <summary>
    /// Returns the range reference in the language of the macro. 
    /// Read-only String.
    /// </summary>
    public string       AddressGlobal
    {
      get
      {
        string result = m_worksheet.QuotedName + "!" + AddressGlobalWithoutSheetName;
        return result;
      }
    }

    /// <summary>
    /// Return global address (with $ signs) without worksheet name.
    /// </summary>
    public string       AddressGlobalWithoutSheetName
    {
      get
      {
        string result = string.Empty;
        string cell0 = RangeImpl.GetCellNameWithDollars( FirstColumn, FirstRow );

        if( IsSingleCell )
        {
          return result + cell0;
        }
        else
        {
          string cell1 =  RangeImpl.GetCellNameWithDollars( LastColumn, LastRow );
          return result + cell0 + ":" + cell1;
        }
      }
    }
    /// <summary>
    /// Gets list of all cells.
    /// </summary>
    internal List<IRange>  CellsList
    {
      get
      {
        CheckDisposed();

        if( m_cells == null && !m_bCells )
          InfillCells();

        if( m_cells == null )
          throw new ArgumentNullException();

        return m_cells;
      }
    }
    /// <summary>
    /// Checks if the range represents a single cell or range of cells. Read-only.
    /// </summary>
    internal protected bool IsSingleCell
    {
      get
      {
        return ( m_iLeftColumn == m_iRightColumn )
          && ( m_iTopRow == m_iBottomRow );
      }
    }

    /// <summary>
    /// Gets / sets first row of the range.
    /// </summary>
    internal protected int FirstRow
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_iTopRow;
        //return GetRowFromCellIndex( m_iTopLeftCell );
      }
      set
      {
        if( value < 1 || value > m_book.MaxRowCount )
          throw new ArgumentOutOfRangeException( "FirstRow" );

        if( value != FirstRow )
        {
          //m_iTopLeftCell = GetCellIndex( FirstColumn, value );
          m_iTopRow = value;
          OnFirstRowChanged();
        }
      }
    }

    /// <summary>
    /// Gets / sets first column of the range.
    /// </summary>
    internal protected int FirstColumn
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_iLeftColumn;
        //return GetColumnFromCellIndex( m_iTopLeftCell );
      }
      set
      {
        if( value < 1 || value > m_book.MaxColumnCount )
          throw new ArgumentOutOfRangeException( "FirstRow", "Value was out of range." );

        if( value != FirstColumn )
        {
          //m_iTopLeftCell = GetCellIndex( value, FirstRow );
          m_iLeftColumn = value;
          OnFirstColumnChanged();
        }
      }
    }

    /// <summary>
    /// If it is a single cell, then it returns its name; otherwise returns NULL. Read-only.
    /// </summary>
    internal protected string CellName
    {
      get
      {
        if( IsSingleCell )
          return GetCellName( FirstColumn, FirstRow );

        return null;
      }
    }
    /// <summary>
    /// If single cell, returns its name; otherwise returns -1. Read-only.
    /// </summary>
    internal protected long CellIndex
    {
      get
      {
        if( IsSingleCell )
          return GetCellIndex( FirstColumn, FirstRow );

        return -1;
      }
    }
    /// <summary>
    /// Returns type of the cell. Read-only. 
    /// </summary>
    internal protected TCellType CellType
    {
      get
      {
        BiffRecordRaw record = Record;

        if( record != null )
        {
          return ( TCellType )Record.TypeCode;
        }

        return TCellType.Blank;
      }
    }
    /// <summary>
    /// Gets index of extended format. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    internal protected ushort StyleXFIndex
    {
      get
      {
        if( IsSingleCell )
        {
          if( m_style != null )
          {
            return ( ushort )m_style.Wrapped.Index;
          }
          else if( Record != null )
          {
            ICellPositionFormat format = ( ICellPositionFormat )Record;
            return format.ExtendedFormatIndex;
          }
        }

        return ( ushort )m_book.DefaultXFIndex;
      }
    }
    /// <summary>
    /// Sets / gets index of extended format.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// When method is applied for the range that contains 
    /// more than one cell.
    /// </exception>
    [ CLSCompliant( false ) ]
    public ushort ExtendedFormatIndex
    {
      get
      {
          if (IsEntireRow)
              return (ushort)m_worksheet.GetXFIndex(m_iTopRow);
          else if (IsEntireColumn)
              return (ushort)m_worksheet.GetColumnXFIndex(m_iLeftColumn);
          else
              return (ushort)m_worksheet.GetXFIndex(m_iTopRow, m_iLeftColumn);
      }
      set
      {
          if (!IsSingleCell && !IsEntireRow && !IsEntireColumn)
          throw new ArgumentException( "This property can be used only for single cell not a range" );

        SetXFormatIndex( value );
      }
    }
    /// <summary>
    /// Converts range to rk subrecord. Read-only.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// When cell does not contain rk record.
    /// </exception>
    [ CLSCompliant( false ) ]
    internal protected MulRKRecord.RkRec RKSubRecord
    {
      get
      {
        if( CellType != TCellType.RK )
          throw new ArgumentException( "This property can be accessed only when range represent RKRecord" );

        return new MulRKRecord.RkRec( StyleXFIndex, 
          RKRecord.ConvertToRKNumber( ( ( RKRecord )Record ).RKNumber ) );
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    internal protected WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Returns merge region if range is part of any merge region; otherwise
    /// returns null. Read-only.
    /// </summary>
    private MergeRegion ParentMergeRegion
    {
      get
      {
        Rectangle rectTopLeft = new Rectangle( FirstColumn - 1, FirstRow - 1, 0, 0 );
        
        if( IsSingleCell )
        {
          return m_worksheet.MergeCells[ rectTopLeft ];
        }

        Rectangle rectBottomRight = new Rectangle( LastColumn - 1, LastRow - 1, 0, 0 );
        MergeRegion regionTopLeft = m_worksheet.MergeCells[ rectTopLeft ];
        MergeRegion regionBottomRight = m_worksheet.MergeCells[ rectBottomRight ];

        return ( MergeRegion.Equals( regionTopLeft, regionBottomRight ) ) ?
          regionTopLeft :
          null;
      }
    }
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    internal protected WorksheetImpl InnerWorksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Internal record.
    /// </summary>
    [ CLSCompliant( false ) ]
    internal protected BiffRecordRaw Record
    {
      get
      {
        return ( BiffRecordRaw )m_worksheet.GetRecord( FirstRow, FirstColumn );// CellIndex );
      }
      set
      {
        //FormulaRecord formula = Record as FormulaRecord;

        //if( formula != null )
        //{
        //  AddRemoveEventListenersForNameX( formula.ParsedExpression, -1, -1, false );
        //}

        m_worksheet.CellRecords.AddRecord( value, false );
      }
    }
    /// <summary>
    /// Gets Dictionary where key is ArrayRecord that at least partially intersects
    /// with this range. Read-only.
    /// </summary>
    public Dictionary<ArrayRecord, object> FormulaArrays
    {
      get
      {
        Dictionary<ArrayRecord, object> hashFormulas = null;

        if( IsSingleCell )
        {
          ArrayRecord array = m_worksheet.CellRecords.GetArrayRecord( m_iTopRow, m_iLeftColumn );

          if( array != null )
          {
            hashFormulas = new Dictionary<ArrayRecord, object>();
            hashFormulas[ array ] = null;
          }
        }
        else
        {
          hashFormulas = new Dictionary<ArrayRecord, object>();
          Dictionary<long, object> hashCellIndexes = new Dictionary<long, object>();
          CellRecordCollection cellRecords = m_worksheet.CellRecords;

          for( int iRow = FirstRow, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iCol = FirstColumn, iLastCol = LastColumn; iCol <= iLastCol; iCol++ )
            {
              ArrayRecord array = cellRecords.GetArrayRecord( iRow, iCol );

              if( array != null )
              {
                long lCellIndex = RangeImpl.GetCellIndex( array.FirstColumn, array.FirstRow );

                if( !hashCellIndexes.ContainsKey( lCellIndex ) )
                {
                  hashFormulas[ array ] = null;
                  hashCellIndexes.Add( lCellIndex, null );
                }
              }
            }
          }

          hashCellIndexes.Clear();
        }

        return hashFormulas;
      }
    }
    /// <summary>
    /// Checks if all formula arrays partially contained by this range
    /// are fully contained by this range. Read-only.
    /// </summary>
    public bool AreFormulaArraysNotSeparated
    {
      get
      {
        return GetAreArrayFormulasNotSeparated( null );
      }
    }
    /// <summary>
    /// Checks if all formula arrays partially contained by this range
    /// or fully contained by this range. Read-only.
    /// </summary>
    /// <param name="colFormulas">Collection of array formula records.</param>
    /// <returns>Value indicating whether all formula arrays partially contained by this range.</returns>
    protected internal bool CheckFormulaArraysNotSeparated( ICollection<ArrayRecord> colFormulas )
    {
      if( colFormulas == null )
        throw new ArgumentNullException( "colFormulas" );

      int iFirstRow = FirstRow;
      int iFirstColumn = FirstColumn;
      int iLastRow = LastRow;
      int iLastColumn = LastColumn;

      foreach( ArrayRecord array in colFormulas )
      {
        if( array.FirstRow + 1 < iFirstRow || array.LastRow + 1 > iLastRow
          || array.FirstColumn + 1 < iFirstColumn || array.LastColumn + 1 > iLastColumn )
        {
          return false;
        }
      }

      return true;
    }
    /// <summary>
    /// Number of cells in the range. Read-only.
    /// </summary>
    public int CellsCount
    {
      get
      {
        return ( LastRow - FirstRow + 1 ) * ( LastColumn - FirstColumn + 1 );
      }
    }
    /// <summary>
    /// Returns number format object corresponding to this range. Read-only.
    /// </summary>
    public FormatImpl InnerNumberFormat
    {
      get
      {
        int iFormatIndex = m_book.GetExtFormat( ExtendedFormatIndex ).NumberFormatIndex;

        //MS Excel sets 14th index by default if the index is out of range for any the datatype. 
        //So, using the same here in XlsIO.
        if (m_book.InnerFormats.Count > 14 && !m_book.InnerFormats.Contains(iFormatIndex))
            iFormatIndex = 14;

        return m_book.InnerFormats[ iFormatIndex ];
      }
    }
    /// <summary>
    /// Gets address global in the format required by Excel 2007.
    /// </summary>
    public string AddressGlobal2007
    {
      get
      {
        return AddressGlobal;
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns number from the style name, i.e. Normal_1 result is 1.
    /// </summary>
    /// <param name="pre">Style name.</param>
    /// <returns>Parsed number.</returns>
    protected int  CurrentStyleNumber( string pre )
    {
      return m_book.CurrentStyleNumber( pre );
    }
    /// <summary>
    /// This method is called after changing last column index.
    /// </summary>
    protected void OnLastColumnChanged()
    {

    }

    /// <summary>
    /// This method is called after changing first column index.
    /// </summary>
    protected void OnFirstColumnChanged()
    {

    }

    /// <summary>
    /// This method is called after changing last row index.
    /// </summary>
    protected void OnLastRowChanged()
    {

    }

    /// <summary>
    /// This method is called after changing first row index.
    /// </summary>
    protected void OnFirstRowChanged()
    {

    }
    /// <summary>
    /// This method is called after changing style of the range.
    /// </summary>
    /// <param name="oldType">Cell type.</param>
    protected void OnStyleChanged( TCellType oldType )
    {
      if( oldType == TCellType.LabelSST && CellType != TCellType.LabelSST )
      {
        string strValue = Value;

        if( strValue != null && strValue.Length != 0 )
        {
          m_rtfString.Clear();
        }
      }

      SetChanged();
    }
    /// <summary>
    /// This method is called after changing of value.
    /// </summary>
    /// <param name="old">Old value.</param>
    /// <param name="value">New value.</param>
    protected void OnValueChanged( string old, string value )
    {
      SetChanged();
      if(old != value)
      OnCellValueChanged(old, value, this);
      int formatIndex = m_book.InnerExtFormats[ExtendedFormatIndex].NumberFormatIndex;
      FormatImpl format = m_book.InnerFormats[formatIndex];
      // Check if the string already exists in the collection.
      // If it exists, then just get its index.
      // Otherwise, add it to collection.
      int iLength = ( value != null ) ? value.Length : 0;

      if( iLength == 0 )
      {
        if( !( Record is BlankRecord ) )
        {
          Record = CreateRecordWithoutAdd( TBIFFRecord.Blank );
        }
      }
      else if( value == old )
      {
        return;
      }
      else
      {
        bool? bStringsPreserved = IsStringsPreserved;

        if (bStringsPreserved == null)
        {
            bStringsPreserved = m_worksheet.IsStringsPreserved;  
        }
          

        if( bStringsPreserved == true )
        {
          Text = value;
        }
        else if( value[ 0 ] == '=' && iLength > 1 && value[1] != '&') // This could be a formula
        {
          SetFormula( value );
        }
        else if( DetectAndSetBoolErrValue( value ) )
        {
        }
        else
        {
          double dValue;
          DateTime dateValue = 
#if ( WINRT )
              DateTimeExtension.FromOADate(0);
#else
            DateTime.FromOADate( 0 );
#endif

          CultureInfo cultureInfo = this.AppImplementation.CheckAndApplySeperators();
          bool bNumber = double.TryParse(value, Array.IndexOf(floatNumberStyleCultures,
#if ( WINRT )
              CultureInfo.CurrentCulture.Name
#else
              System.Threading.Thread.CurrentThread.CurrentCulture.Name
#endif
            ) >= 0 ? NumberStyles.Float : NumberStyles.Any, cultureInfo, out dValue);
          bool bDateTime = bNumber ? false : TryParseDateTime( value, out dateValue );

          if( bDateTime )
          {
            long ticks = dateValue.Ticks;

            if( ticks < MinAllowedDateTicks && ticks != 0 )
            {
              bDateTime = false;
            }
            else
            {
              dValue = dateValue.ToOADate();
            }
          }

          if( (bNumber || bDateTime ) && (format.FormatType==ExcelFormatType.General || 
               format.FormatType==ExcelFormatType.Number || format.FormatType==ExcelFormatType.DateTime) &&
              !(bDateTime && format.FormatType == ExcelFormatType.General))
          {
            SetNumber( dValue );

            if( bDateTime )
            {
              FormatType = ExcelFormatType.DateTime;
            }
            else if( format.FormatString != DEF_GENERAL_FORMAT )
            {
              FormatType = ExcelFormatType.Number;
            }
          }
          else
          {
            value = CheckApostrophe( value );
            RichText.Text = value;
          }
        }
      }
      if (this.Parent is WorksheetImpl
#if!(SILVERLIGHT)
 && ((WorksheetImpl)this.Parent).CalcEngine != null && value != null
#endif
)
      {
          ((WorksheetImpl)Parent).OnValueChanged(this.Row, this.Column, value);
      }
    }
    /// <summary>
    /// Checks whether first symbol is apostrophe and sets appropriate cell style.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <returns>Updated string value.</returns>
    private string CheckApostrophe( string value )
    {
      if( value == null || value.Length == 0 || m_book.Loading )
        return value;

      if( value[ 0 ] == '\'' )
      {
        CellStyle.IsFirstSymbolApostrophe = true;
        value = value.Substring( 1 );
      }
      else
      {
        ExtendedFormatsCollection arrFormats = m_book.InnerExtFormats;
        ExtendedFormatImpl format = arrFormats[ ExtendedFormatIndex ];

        if( format.IsFirstSymbolApostrophe )
          CellStyle.IsFirstSymbolApostrophe = false;
      }

      return value;
    }
    /// <summary>
    /// Converts object to double if possible.
    /// </summary>
    /// <param name="value">Object to convert.</param>
    /// <returns>Converted value.</returns>
    protected double ObjectToDouble( object value )
    {
      if( value is double )
      {
        return ( double )value;
      }
      else if( value is int )
      {
        return Convert.ToDouble( ( int )value );
      }
      else 
      {
        return double.Parse( value.ToString() );
      }
    }
    /// <summary>
    /// Group or ungroup current range.
    /// </summary>
    /// <param name="groupBy">Should we perform operation on rows or columns?</param>
    /// <param name="isGroup">If True then group, otherwise ungroup.</param>
    /// <param name="bCollapsed">Indicates whether created group should be collapsed.</param>
    /// <returns>This range after grouping / ungrouping.</returns>
    protected RangeImpl ToggleGroup( ExcelGroupBy groupBy, bool isGroup, bool bCollapsed )
    {
      int iStart, iFinish;
      IOutline outline=null;
      WorksheetImpl sheetImpl = this.Worksheet as WorksheetImpl;
      Dictionary<int, int> levelAndIndexes = new Dictionary<int, int>();
       levelAndIndexes= sheetImpl.IndexAndLevels;
        if (isGroup)
        {
            SetWorksheetSize();
        }

      if (m_outlineLevels == null)
          m_outlineLevels = new Dictionary<int, List<Point>>();

      if (m_outlineWrapperUtility == null)
          m_outlineWrapperUtility = new OutlineWrapperUtility(m_outlineLevels);

      sheetImpl.OutlineWrappers = null;

        OutlineGetter getOutline;

        if (groupBy == ExcelGroupBy.ByRows)
        {
            iStart = FirstRow;
            iFinish = LastRow;
            getOutline = new OutlineGetter(GetRowOutline);
        }
        else
        {
            iStart = FirstColumn;
            iFinish = LastColumn;
            getOutline = new OutlineGetter(GetColumnOutline);
            //information = m_worksheet.ColumnInformation;
        }
        int currentLevel=0;
        for (int i = iStart; i <= iFinish; i++)
        {
            outline = getOutline(i);//GetOrCreateOutline( groupBy, information, i, true );

            if (isGroup && outline.OutlineLevel < 7)
            {
                outline.OutlineLevel++;
                if (groupBy == ExcelGroupBy.ByColumns)
                {
                    if (levelAndIndexes.ContainsKey(i))
                    {
                        levelAndIndexes[i] = levelAndIndexes[i] + 1;
                    }
                    else
                    {
                        levelAndIndexes.Add(i, outline.OutlineLevel);        
                    }
                    
                              
                }
            }
            else if (!isGroup && outline.OutlineLevel > 0)
            {
                currentLevel=outline.OutlineLevel--;
                if (groupBy == ExcelGroupBy.ByColumns)
                {
                    if (currentLevel > 1)
                    {
                        levelAndIndexes[i] = levelAndIndexes[i] - 1;
                    }
                    else
                    {
                        levelAndIndexes.Remove(i);
                    }
                }
            }

            if (outline.OutlineLevel == 0)
            {
                outline.IsHidden = false;
            }
            else if (isGroup && (outline.OutlineLevel >= 1 || bCollapsed))
            {
                outline.IsHidden = bCollapsed;
                outline.IsCollapsed = bCollapsed;
            }
        }
        if (groupBy == ExcelGroupBy.ByRows)
        {
            m_outlineWrapperUtility.UpdateOutlineRowStorage(sheetImpl, null);
        }
        else
        {
            List<int> keyList;
            int[] keys=new int[levelAndIndexes.Count];
            levelAndIndexes.Keys.CopyTo(keys, 0);
            keyList = new List<int>(keys);
            keyList.Sort();
            Dictionary<int, int> temp = levelAndIndexes;
            levelAndIndexes = new Dictionary<int, int>();
            foreach (int key in keyList)
            {
                levelAndIndexes.Add(key, temp[key]);
            }
            m_outlineWrapperUtility.UpdateOutlineColumn(sheetImpl, levelAndIndexes);
        }
        return this;
    }

    /// <summary>
    /// Gets Row from collection.
    /// </summary>
    /// <param name="iRowIndex">One-based row index.</param>
    /// <returns>Row information.</returns>
    public IOutline GetRowOutline( int iRowIndex )
    {
      RowStorage storage = WorksheetHelper.GetOrCreateRow( m_worksheet, iRowIndex - 1, true );
      return storage;//.RowInformation;
    }
    /// <summary>
    /// This method creates subtotal on Corresponding ranges
    /// </summary>
    /// <param name="groupBy">Indicates the Group By Column</param>
    /// <param name="function">ConsolidationFunction to be applied</param>
    /// <param name="totalList">Columns to be added</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList)
    {
        this.SubTotal(groupBy, function, totalList, true, false, true);
    }
    /// <summary>
    /// This method creates subtotal on Corresponding ranges
    /// </summary>
    /// <param name="groupBy">Indicates the Group By Column</param>
    /// <param name="function">ConsolidationFunction to be applied</param>
    /// <param name="totalList">Columns to be added</param>
    /// <param name="replace">Replaces Exisiting SubTotal</param>
    /// <param name="pageBreaks">Insert PageBreaks</param>
    /// <param name="summaryBelowData">SummaryBelowData</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData)
    {
        if (totalList != null)
        {
            SubTotalImpl impl = new SubTotalImpl(m_worksheet);
            impl.CalculateSubTotal(this.FirstRow, this.FirstColumn - 1, this.LastRow, this.LastColumn - 1, groupBy, function, totalList, replace, pageBreaks, summaryBelowData);
        }
    } 
    /// <summary>
    /// Gets Column from collection.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index. </param>
    /// <returns>Column information.</returns>
    public IOutline GetColumnOutline( int iColumnIndex )
    {
      //throw new NotImplementedException();
      //return GetOrCreateOutline( ExcelGroupBy.ByColumns, m_worksheet.ColumnInformation, iColumnIndex, true );
      ColumnInfoRecord columnInfo = m_worksheet.ColumnInformation[ iColumnIndex ];

      if( columnInfo == null )
      {
        columnInfo = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
        columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( iColumnIndex - 1 );
        columnInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
        m_worksheet.ColumnInformation[ iColumnIndex ] = columnInfo;
      }

      return columnInfo;
    }
    /// <summary>
    /// Sets dimensions of parent worksheet to fit this range.
    /// </summary>
    private void SetWorksheetSize()
    {
      if( m_worksheet.FirstRow > FirstRow
        || m_worksheet.FirstRow == -1)
      {
        m_worksheet.FirstRow = FirstRow;
      }

      if( m_worksheet.LastRow < LastRow )
        m_worksheet.LastRow = LastRow;

      if( m_worksheet.FirstColumn > FirstColumn 
        || m_worksheet.FirstColumn == int.MaxValue )
      {
        m_worksheet.FirstColumn = FirstColumn;
      }

      if( m_worksheet.LastColumn < LastColumn
        || m_worksheet.LastColumn == int.MaxValue )
      {
        m_worksheet.LastColumn = LastColumn;
      }
    }
    /// <summary>
    /// Sets the workbook.
    /// </summary>
    /// <param name="book">The book.</param>
    internal void SetWorkbook(WorkbookImpl book)
    {
        m_book = book;
    }
    /// <summary>
    /// Return outline from the dictionary that corresponds to the specified index,
    /// creates new one if necessary.
    /// </summary>
    /// <param name="groupBy">
    /// Indicates whether row outline or column outline
    /// should be returned from the collection.
    /// </param>
    /// <param name="information">Collection of outlines.</param>
    /// <param name="iIndex">Index of the needed outline.</param>
    /// <param name="bThrowExceptions">
    /// Indicates whether exeption should be thrown when incorrect index passed
    /// or just return Null value.
    /// </param>
    /// <returns>Outline from the collection or newly created one.</returns>
    private IOutline GetOrCreateOutline( ExcelGroupBy groupBy, IDictionary information,
      int iIndex, bool bThrowExceptions )
    {
      if( information == null )
        throw new ArgumentNullException( "information" );

      if( iIndex < 1 )
        throw new ArgumentOutOfRangeException( "iIndex" );

      IOutline outline = null;
      RowRecord row;
      ColumnInfoRecord colInfo;

      if( information.Contains( iIndex ) )
      {
        outline = ( IOutline )information[ iIndex ];
      }
      else
      {
        if( groupBy == ExcelGroupBy.ByRows )
        {
          if( iIndex > m_book.MaxRowCount )
          {
            if( bThrowExceptions )
            {
              throw new ArgumentOutOfRangeException( "iIndex" );
            }

            return null;
          }

          row = ( RowRecord ) BiffRecordFactory.GetRecord( TBIFFRecord.Row );
          row.RowNumber = ( ushort )( iIndex - 1 );
          row.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
          row.Height = ( ushort )( m_worksheet.DefaultRowHeight );
          row.IsBadFontHeight = false;

          outline = ( IOutline )row;
        }
        else
        {
          if( iIndex > m_book.MaxColumnCount )
          {
            if( bThrowExceptions )
              throw new ArgumentOutOfRangeException( "iIndex" );

            return null;
          }

          colInfo = ( ColumnInfoRecord ) BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
          colInfo.LastColumn = colInfo.FirstColumn = ( ushort )( iIndex - 1 );
          colInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;

          outline = ( IOutline )colInfo;
        }
          
        information.Add( iIndex, outline );
      }

      return outline;
    }
    /// <summary>
    /// Gets display text.
    /// </summary>
    /// <returns>Value representing displayed string.</returns>
    protected string GetDisplayString()
    {
      switch( CellType )
      {
        case TCellType.LabelSST:
        case TCellType.RString:
        case TCellType.Label:
          return m_worksheet.GetText( m_iTopRow, m_iLeftColumn );//m_iTopLeftCell );

        case TCellType.Formula:
          string strValue = FormulaStringValue;

          if( strValue != null && strValue.Length != 0 )
            return FormulaStringValue;

          goto default;

        case TCellType.BoolErr:
          return Value;
          
        case TCellType.RK:
          return ParseNumberFormat();
          break;
        default:
          return string.Empty;
      }
    }

    /// <summary>
    /// Parses the number format.
    /// </summary>
    /// <returns></returns>
    private string ParseNumberFormat()
    {
        StringBuilder parsedValue = new StringBuilder();
        string numberFormat = GetNumberFormat();
        string[] formats = numberFormat.Split(new char[] { ';' });
        for (int index=0, len= formats.Length;index< len; index++)
        {
            if (Array.IndexOf(formats[index].ToCharArray(),'@')>=0)
            {
                string[] splitFormats = formats[index-1].Split(new char[] { '\"' });
                foreach (string splitFormat in splitFormats)
                {
                    if (splitFormat.Contains("*"))
                    {
                        parsedValue.Append(DEF_EMPTY_DIGIT);                        
                    }
                    else if (!CheckUnnecessaryChar(splitFormat))
                    {
                        parsedValue.Append(splitFormat);
                    }
                    else if (splitFormat.Contains("?"))
                    {
                        char[] spaceArray = splitFormat.ToCharArray();
                        foreach (char space in spaceArray)
                        {
                            if (space == '?')
                            {
                                parsedValue.Append(DEF_EMPTY_DIGIT);
                                parsedValue.Append(DEF_EMPTY_DIGIT);
                            }
                        }
                    }
                }
            }
        }
        return parsedValue.ToString();
    }
    /// <summary>
    /// Checks the unnecessary char.
    /// </summary>
    /// <param name="splitFormat">The split format.</param>
    /// <returns></returns>
    private bool CheckUnnecessaryChar(string splitFormat)
    {
        bool result=false;
        char[] splitChar = splitFormat.ToCharArray();
        foreach (char split in splitChar)
        {
            if (Array.IndexOf(unnecessaryChar, split) >= 0)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns DataTime value of the record.
    /// </summary>
    /// <returns>DataTime value of the record.</returns>
    protected DateTime GetDateTime()
    {
      double dNumber = GetNumber();
      return UtilityMethods.ConvertNumberToDateTime( dNumber,m_book.Date1904 );
    }

    internal void SetDifferedColumnWidth(RangeImpl sourceRange, RangeImpl destinationRange)
    {
        int columnsCount = sourceRange.Columns.Length;
        for (int i = 0; i < columnsCount; i++)
            destinationRange.Columns[i].ColumnWidth = sourceRange.Columns[i].ColumnWidth;
    }

    internal void SetDifferedRowHeight(RangeImpl sourceRange, RangeImpl destinationRange)
    {
        int rowsCount = sourceRange.Rows.Length;
        for (int i = 0; i < rowsCount; i++)
            destinationRange.Rows[i].RowHeight = sourceRange.Rows[i].RowHeight;
    }
    /// <summary>
    /// Fills internal BiffRecord with data from specified DateTime.
    /// </summary>
    /// <param name="value">DateTime with range value.</param>
    protected void SetDateTime( DateTime value )
    {
      double dNumber = UtilityMethods.ConvertDateTimeToNumber( value );
      if (dNumber >= 0)
          SetNumber(dNumber);
      else
      {
          //If date is set earlier than 1/1/1900, it should be preserved as string.
#if ( WINRT )
          Text = string.Format("MM/dd/yyyy", value);
#else
          Text = value.ToShortDateString();
#endif
          NumberFormat = DEF_DATE_FORMAT ; 
      }

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Fills internal BiffRecord with data from specified DateTime.
    /// </summary>
    /// <param name="time">DateTime with range value.</param>
    protected void SetTimeSpan( TimeSpan time )
    {
      NumberFormat = DEF_TIME_FORMAT;
      
      // NOTE: Excel converts time to DateTime datatype. To indicate that this is time value,
      // date must be minimally supported by Excel - 12/30/1899.
      //DateTime date = new DateTime( 1899, 12, 30, time.Hours, time.Minutes, time.Seconds, time.Milliseconds );
      
      SetNumber( time.Ticks / ( double )TimeSpan.TicksPerDay );

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Returns number value from the cell if possible.
    /// </summary>
    /// <returns>Stored number.</returns>
    protected double GetNumber()
    {
      double dResult = double.NaN;

      if( IsSingleCell )
      {
        if( CellType == TCellType.RK )
        {
          RKRecord rk = ( RKRecord )Record;
          dResult = rk.RKNumber;
        }
        else if( CellType == TCellType.Number )
        {
          NumberRecord number = ( NumberRecord )Record;
          dResult = number.Value;
        }
        else if( CellType == TCellType.Formula )
        {
          FormulaRecord formula = ( FormulaRecord )Record;
          dResult = formula.Value;
        }
      }
      else
      {
        MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
        range.ResetRowColumn( Row, Column );
        dResult = range.GetNumber();

        if( !Double.IsNaN( dResult ) )
        {
          for( int i = Row, iLen = LastRow; i <= iLen; i++ )
          {
            for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
            {
              range.ResetRowColumn( i, j );
              double value = range.GetNumber();

              if( dResult != value )
              {
                dResult = double.NaN;
                break;
              }
            }
          }
        }
      }

      return dResult;
    }
    /// <summary>
    /// Fills internal BiffRecord with data from specified number.
    /// </summary>
    /// <param name="value">Number with range value.</param>
    protected void SetNumber( double value )
    {
      TryRemoveFormulaArrays();

      BiffRecordRaw record = CreateNumberRecord( value );
      Record = record;

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Fills internal BiffRecord with data from specified number.
    /// </summary>
    /// <param name="value">Number with range value.</param>
    private void SetNumberAndFormat( double value,bool isPreserveFormat )
    {
      TryRemoveFormulaArrays();

      BiffRecordRaw record = CreateNumberRecord( value );
      ICellPositionFormat cell = record as ICellPositionFormat;

      int iXFIndex = cell.ExtendedFormatIndex;
      ExtendedFormatImpl xformat = m_book.InnerExtFormats[ iXFIndex ];
      int iFormatIndex = xformat.NumberFormatIndex;
      FormatImpl format = m_book.InnerFormats[ iFormatIndex ];

      if( format.FormatString != DEF_GENERAL_FORMAT )
      {
        ExcelFormatType formatType = format.GetFormatType( value );

        if (formatType != ExcelFormatType.Number && formatType != ExcelFormatType.General && !isPreserveFormat)
        {
          iFormatIndex = m_book.InnerFormats.FindOrCreateFormat( DEF_NUMBER_FORMAT );
          xformat = xformat.Clone() as ExtendedFormatImpl;
          xformat.NumberFormatIndex = iFormatIndex;
          xformat = m_book.InnerExtFormats.Add( xformat );
          cell.ExtendedFormatIndex = ( ushort )xformat.Index;
        }
      }

      Record = record;

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Creates correct record that can store specified number.
    /// </summary>
    /// <param name="value">Value to store.</param>
    /// <returns>Created record with number.</returns>
    private BiffRecordRaw CreateNumberRecord( double value )
    {
      BiffRecordRaw record = m_worksheet.TryCreateRkRecord( m_iTopRow, m_iLeftColumn, value );

      if( record == null )
      {
        NumberRecord number = ( NumberRecord )CreateRecordWithoutAdd( TBIFFRecord.Number );
        number.Value = value;
        record = number;
      }

      return record;
    }
    /// <summary>
    /// Fills internal BiffRecord with data with specified boolean value.
    /// </summary>
    /// <param name="value">Boolean with range value.</param>
    protected void SetBoolean( bool value )
    {
      BoolErrRecord record = ( BoolErrRecord )CreateRecordWithoutAdd( TBIFFRecord.BoolErr );

      record.IsErrorCode = false;
      record.BoolOrError = ( byte )( value ? 1 : 0 );

      Record = record;

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Fills internal BiffRecord with data with specified error value.
    /// </summary>
    /// <param name="strError">String with error value.</param>
    protected void SetError( string strError )
    {
      if( strError == null )
        throw new ArgumentNullException( "strError" );

      if( strError.Length == 0 )
        throw new ArgumentException( "string can't be empty" );

      int iCode = GetErrorCodeByString( strError );

      if( iCode != -1 )
      {
        BoolErrRecord record = ( BoolErrRecord )CreateRecordWithoutAdd( TBIFFRecord.BoolErr );

        record.IsErrorCode = true;
        record.BoolOrError = ( byte )iCode;
        Record = record;
      }
      else
      {
        throw new ArgumentOutOfRangeException( "Not error string" );
      }

      if( m_rtfString != null ) m_rtfString.Clear();
    }
    /// <summary>
    /// Gets error code by error string. 
    /// </summary>
    /// <param name="strError">Represents error string.</param>
    /// <returns>Returns error code.</returns>
    private int GetErrorCodeByString( string strError )
    {
      if( strError == null || strError.Length == 0 )
        throw new ArgumentNullException( "strError" );

      strError = strError.ToUpper();

      if( strError[ 0 ] != '#' )
      {
        strError = '#' + strError;
      }

      int iErrorCode;

      return( FormulaUtil.ErrorNameToCode.TryGetValue( strError, out iErrorCode ) ) ?
        iErrorCode :
        -1;
    }
    /// <summary>
    /// Sets formula value to the current range.
    /// </summary>
    /// <param name="value">Formula value.</param>
    internal protected void SetFormula( string value )
    {
      SetFormula( value, null, false );
    }
    /// <summary>
    /// Sets formula value to the current range.
    /// </summary>
    /// <param name="value">Formula value.</param>
    /// <param name="hashWorksheetNames">
    /// Dictionary with new worksheet names (to copy worksheet's
    /// into workbook's and merging workbooks).
    /// </param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    internal protected void SetFormula( string value, Dictionary<string, string> hashWorksheetNames, bool bR1C1 )
    {
        if (this.Workbook.Version == ExcelVersion.Excel97to2003 && value.Length > FormulaLengthXls)
          throw new ArgumentException("The formula is too long.Formulas length should not be longer then 255");
      else if (value.Length > FormulaLengthXlsX)
          throw new ArgumentException("The formula is too long.Formulas length should not be longer then 8192");
      if( value[ 0 ] == '=' )
      {
        value = value.Substring( 1, value.Length - 1 );
      }

      int iRow = Row - 1;
      int iColumn = Column - 1;
      FormulaUtil formulaUtil = m_book.FormulaUtil;
      Ptg[] ptgs = formulaUtil.ParseString( value, m_worksheet, hashWorksheetNames,
        iRow, iColumn, bR1C1 );

      FormulaRecord formula = ( FormulaRecord )CreateRecordWithoutAdd( TBIFFRecord.Formula );
      formula.ParsedExpression = ptgs;
      if (Parent is IWorksheet && ((IWorksheet)Parent).CalcEngine == null)
      {
          formula.RecalculateAlways = true;
          formula.CalculateOnOpen = true;
      }
      else
      {
          formula.RecalculateAlways = false;
          formula.CalculateOnOpen = false;
      }
      Record = formula;

      //AddRemoveEventListenersForNameX( ( ( FormulaRecord )Record ).ParsedExpression, -1, -1, true );
      FormulaUtil.RaiseFormulaEvaluation( this, new EvaluateEventArgs( this, ptgs ) );
    }
    /// <summary>
    /// Copies formula record.
    /// </summary>
    /// <param name="record">Record to copy.</param>
    [ CLSCompliant( false ) ]
    internal protected void SetFormula( FormulaRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      Record = record;
      //AddRemoveEventListenersForNameX( ( ( FormulaRecord )Record ).ParsedExpression, -1, -1, true );
    }
    /// <summary>
    /// Get / sets type of the format.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected ExcelFormatType FormatType
    {
      get
      {
        return ContainsNumber
          ? InnerNumberFormat.GetFormatType( GetNumber() )
          : InnerNumberFormat.GetFormatType( m_worksheet.GetValue( Record as ICellPositionFormat, false ) );
      }
      set
      {
        if( value == FormatType || ( value != ExcelFormatType.DateTime
          && NumberFormat == DEF_GENERAL_FORMAT ) )
        {
          return;
        }

        switch( value )
        {
          case ExcelFormatType.DateTime:
            NumberFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;//DEF_DATE_FORMAT;
            break;

          case ExcelFormatType.Number:
            NumberFormat = DEF_NUMBER_FORMAT;
            break;

          case ExcelFormatType.Text:
            NumberFormat = DEF_TEXT_FORMAT;
            break;
        }
      }
    }
    /// <summary>
    /// Read-only. Returns FormatRecord for this range.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected FormatRecord Format
    {
      get
      {
        int iFormatIndex = m_book.GetExtFormat( ExtendedFormatIndex ).NumberFormatIndex;
        return m_book.InnerFormats[ iFormatIndex ].Record;
      }
    }
    /// <summary>
    /// This method should be called after any changes in the range.
    /// Sets Saved property of the parent workbook to false.
    /// </summary>
    protected void SetChanged()
    {
      m_worksheet.SetChanged();
    }
    /// <summary>
    /// Checks if specified cell has correct row and column index.
    /// </summary>
    /// <param name="row">Index of the row of the cell.</param>
    /// <param name="column">Index of the column of the cell.</param>
    /// <exception cref="System.ArgumentException">
    /// When row or column is less than 1 or column is greater than maximum possible column index
    /// (it is 256 for Excel 2003, and 16384 for Excel 2007).
    /// </exception>
    protected void CheckRange( int row, int column )
    {
      if( row < 1 || row > m_book.MaxRowCount 
        || column < 1 || column > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException();
    }
    /// <summary>
    /// Searches for specified worksheet in the parent workbook.
    /// </summary>
    /// <param name="sheetName">Name of the worksheet to search.</param>
    /// <returns>Found worksheet.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If there is no such worksheet in the parent workbook.
    /// </exception>
    protected IWorksheet FindWorksheet( string sheetName )
    {
      IWorksheet sheet = m_book.Worksheets[ sheetName ];

      if( sheet == null )
        throw new ArgumentOutOfRangeException( "sheetName" );

      return sheet;
    }
    /// <summary>
    /// Reparses formula.
    /// </summary>
    public void ReparseFormulaString()
    {
      if( IsSingleCell && CellType == TCellType.Formula )
      {
        try
        {
          SetFormula( this.Formula );
        }
        catch( ParseException )
        {
          if( m_book.Loading ) m_book.AddForReparse( this );
          else throw ;
        }
      }
    }
    /// <summary>
    /// Moves cell one row up.
    /// </summary>
    /// <param name="options">Options for cell copying.</param>
    private void MoveCellsUp( ExcelCopyRangeOptions options )
    {
      int iStartRow = this.LastRow + 1;
      int iStartCol = this.FirstColumn;
      int iLastRow = m_worksheet.UsedRange.LastRow;
      int iLastCol = this.LastColumn;

      if( iStartRow > iLastRow ) return;

      IRange destination = m_worksheet.Range[ FirstRow, FirstColumn ];
      IRange rangeToMove = m_worksheet.Range[ iStartRow, iStartCol, iLastRow, iLastCol ];
      m_worksheet.MoveRange( destination, rangeToMove, options, true );
    }
    /// <summary>
    /// Moves cell one column left.
    /// </summary>
    /// <param name="options">Options for cell copying.</param>
    private void MoveCellsLeft( ExcelCopyRangeOptions options )
    {
      int iStartRow = FirstRow;
      int iStartCol = LastColumn + 1;
      int iLastRow = LastRow;
      int iLastCol = m_worksheet.UsedRange.LastColumn;

      if( iStartCol > iLastCol ) return;

      IRange destination = m_worksheet.Range[ FirstRow, FirstColumn ];
      IRange rangeToMove = m_worksheet.Range[ iStartRow, iStartCol, iLastRow, iLastCol ];
      m_worksheet.MoveRange( destination, rangeToMove, options, false );
    }
    /// <summary>
    /// Parses LabelSST record.
    /// </summary>
    /// <param name="label">Record to parse.</param>
    /// <returns>Parsed string without formatting.</returns>
    private string ParseLabelSST( LabelSSTRecord label )
    {
      return m_book.InnerSST.GetStringByIndex( label.SSTIndex );
    }
    /// <summary>
    /// Parses formula record.
    /// </summary>
    /// <param name="formula">Record to parse.</param>
    /// <returns>Parsed string value.</returns>
    private string ParseFormula( FormulaRecord formula )
    {
      return ParseFormula( formula, false );
    }
    /// <summary>
    /// Parses formula record.
    /// </summary>
    /// <param name="formula">Record to parse.</param>
    /// <param name="bR1C1ReferenceStyle">
    /// Indicates whether to return formula string in R1C1 notation.
    /// </param>
    /// <returns>Parsed string value.</returns>
    private string ParseFormula( FormulaRecord formula, bool bR1C1ReferenceStyle )
    {
      try
      {
        FormulaUtil formulaParser = m_book.FormulaUtil;
        ArrayRecord array = m_worksheet.CellRecords.GetArrayRecord( formula.Row + 1, formula.Column + 1 );
        string strResult;

        if( array != null )
        {
          //return formulaParser.ParsePtgArray( array.Formula, FirstRow, FirstColumn, bR1C1ReferenceStyle );
          strResult = formulaParser.ParsePtgArray( array.Formula, array.FirstRow,
            array.FirstColumn, bR1C1ReferenceStyle, false );
        }
        else
        {
          formula.RecalculateAlways = true;
          formula.CalculateOnOpen = true;
          strResult = formulaParser.ParsePtgArray( formula.ParsedExpression,
            Row - 1, Column - 1, bR1C1ReferenceStyle, false );
        }

        return "=" + strResult;
      }
      catch( ParseException )
      {
        if( m_book.Loading ) m_book.AddForReparse( this );
        else throw ;
      }
      catch( Exception ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex, "Exception" );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, Address, "Range coordinates" );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "Stack trace" );
        throw;
      }

      return null;
    }
    /// <summary>
    /// Sets row height.
    /// </summary>
    /// <param name="value">Value to set.</param>
    /// <param name="bIsBadFontHeight">Indicates whether font and row height are not compatible.</param>
    public void SetRowHeight( double value, bool bIsBadFontHeight )
    {
      if( value < 0 || value > RowRecord.DEF_MAX_HEIGHT )
        throw new ArgumentOutOfRangeException( "RowHeight",
          "Row Height must be in range from 0 to 409.5" );
      int FirstRowValue = FirstRow;
      int LastRowValue = LastRow;
      if (((LastRow - FirstRow) > (m_book.MaxRowCount - (LastRow - FirstRow))) && LastRow == m_book.MaxRowCount)
      {
          FirstRowValue = 1;
          LastRowValue = FirstRow - 1;
          m_worksheet.IsZeroHeight = true;
          m_worksheet.IsVisible = true;
      }
     else
     {
         m_worksheet.IsVisible = false;
     }
      for (int i = FirstRowValue, last = LastRowValue; i <= last; i++)
      {
        m_worksheet.InnerSetRowHeight( i, value, bIsBadFontHeight
          , MeasureUnits.Point, true );
      }
    }
    /// <summary>
    /// Creates rich text string.
    /// </summary>
    protected void  CreateRichTextString()
    {
      if( IsSingleCell )
      {
        m_rtfString = new RangeRichTextString( Application, m_worksheet, m_iTopRow, m_iLeftColumn );//m_iTopLeftCell );
      }
      else
      {
        m_rtfString = new RTFStringArray( this );
      }
      //m_rtfString.Text = Value;
    }
    /// <summary>
    /// Tries to create Value2.
    /// </summary>
    /// <returns>Value2 value.</returns>
    private object  TryCreateValue2()
    {
      if( IsBoolean ) return Boolean;
      if( HasNumber ) return Number;

      if( HasDateTime )
      {
        FormatImpl formatImpl = this.InnerNumberFormat;

        if (Number < CultureInfo .CurrentCulture .DateTimeFormat .Calendar .MaxSupportedDateTime .ToOADate ())
            return (formatImpl.IsTimeFormat(Number)) ?
              (object)TimeSpan :
              (object)DateTime;
        else
           return Number;
      }

      return null;
    }
    /// <summary>
    /// Detects whether specified value is error or boolean and tries to parse it.
    /// </summary>
    /// <param name="strValue">String to parse.</param>
    /// <returns>True if value type was detected and value was parsed correctly.</returns>
    private bool    DetectAndSetBoolErrValue( string strValue )
    {
//      string strUpper = strValue.ToUpper();
//
//      if( strUpper == bool.TrueString.ToUpper() )
//      {
//        Boolean = true;
//        return true;
//      }
//      else if( strUpper == bool.FalseString.ToUpper() )
//      {
//        Boolean = false;
//        return true;
//      }
      if( string.Compare( strValue, bool.TrueString, StringComparison.CurrentCultureIgnoreCase ) == 0 )
      {
        Boolean = true;
        return true;
      }
      else if( string.Compare( strValue, bool.FalseString, StringComparison.CurrentCultureIgnoreCase ) == 0 )
      {
        Boolean = false;
        return true;
      }
      else if( FormulaUtil.ErrorNameToCode.ContainsKey( strValue ) )
      {
        Error = strValue;//strUpper;
        return true;
      }

      return false;
    }
    /// <summary>
    /// Sets index in the LabelSST record.
    /// </summary>
    /// <param name="index">New index value.</param>
    protected internal void SetLabelSSTIndex( int index )
    {
      if( index == SSTDictionary.DEF_EMPTY_STRING_INDEX )
      {
        if( CellType != TCellType.Blank )
        {
          Record = CreateRecordWithoutAdd( TBIFFRecord.Blank );
        }
        return;
      }

      if( index < 0 || index >= m_book.InnerSST.Count )
        throw new ArgumentOutOfRangeException( "index" );

      //if( CellType != TCellType.LabelSST )
      {
        LabelSSTRecord labelSST = ( LabelSSTRecord )CreateRecordWithoutAdd( TBIFFRecord.LabelSST );
        labelSST.SSTIndex = index;
        Record = labelSST;
      }
    }
    /// <summary>
    /// Tries to remove all formula arrays from this range.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.InvalidRangeException">
    /// Thrown when it's impossible to remove array formula.
    /// </exception>
    private void TryRemoveFormulaArrays()
    {
      Dictionary<ArrayRecord, object> hashFormulas = FormulaArrays;

      if( hashFormulas == null || hashFormulas.Count == 0 ) return;

      ICollection<ArrayRecord> colKeys = hashFormulas.Keys;

      if( !CheckFormulaArraysNotSeparated( colKeys ) )
        throw new InvalidRangeException( "Can't set value." );

      m_worksheet.RemoveArrayFormulas( colKeys, false );
    }
    /// <summary>
    /// Sets data validation for the range.
    /// </summary>
    /// <param name="dv">Data validation to set.</param>
    public void SetDataValidation( DataValidationImpl dv )
    {
      if( dv == null )
        throw new ArgumentNullException( "dv" );

      m_dataValidation = AppImplementation.CreateDataValidationWrapper( 
        this, dv );
    }
    /// <summary>
    /// Blanks cell.
    /// </summary>
    private void BlankCell()
    {
      Record = CreateRecord( TBIFFRecord.Blank );
    }

    /// <summary>
    /// Adds copy of the comment
    /// </summary>
    /// <param name="comment">Comment to add.</param>
    public void AddComment( ICommentShape comment )
    {
      if( comment == null )
        throw new ArgumentNullException( "comment" );

      CommentShapeImpl curComment = ( CommentShapeImpl )AddComment();
      curComment.CopyFrom( ( CommentShapeImpl )comment, null );
    }
    /// <summary>
    /// Sets new parent.
    /// </summary>
    /// <param name="parent">Parent to set.</param>
    internal protected void SetParent( WorksheetImpl parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      if( Parent == parent ) return;

      m_worksheet = parent;
      m_book = parent.ParentWorkbook;
      //base.SetParent( parent );
    }
    /// <summary>
    /// Updates named ranges indexes.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      FormulaRecord formula = Record as FormulaRecord;

      if( formula == null ) return;

      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      Ptg[] arrExpression = formula.ParsedExpression;

      if( m_book.FormulaUtil.UpdateNameIndex( arrExpression, arrNewIndex ) )
      {
        formula.ParsedExpression = arrExpression;
      }
    }

    /// <summary>
    /// Creates record and sets range data in it.
    /// </summary>
    /// <param name="recordType">Record type to create.</param>
    /// <returns>Newly created record.</returns>
    private BiffRecordRaw CreateRecord( TBIFFRecord recordType )
    {
      BiffRecordRaw record = CreateRecordWithoutAdd( recordType );

      //m_worksheet.InnerSetCell( m_iTopLeftCell, record );
      m_worksheet.InnerSetCell( m_iLeftColumn, m_iTopRow, record );

      return record;
    }
    /// <summary>
    /// Creates record and sets range data in it.
    /// </summary>
    /// <param name="recordType">Record type to create.</param>
    /// <returns>Newly created record.</returns>
    private BiffRecordRaw CreateRecordWithoutAdd( TBIFFRecord recordType )
    {
      return m_worksheet.GetRecord( recordType, m_iTopRow, m_iLeftColumn );
    }
    /// <summary>
    ///  Updates range.
    /// </summary>
    /// <param name="iFirstRow">First row index.</param>
    /// <param name="iFirstColumn">First column index.</param>
    /// <param name="iLastRow">Last row index.</param>
    /// <param name="iLastColumn">Last column index.</param>
    public void UpdateRange( int iFirstRow, int iFirstColumn, int iLastRow, int iLastColumn )
    {
      FirstRow = iFirstRow;
      FirstColumn = iFirstColumn;
      LastRow = iLastRow;
      LastColumn = iLastColumn;
      ResetCells();
    }
    /// <summary>
    /// Indicates whether range contains number. Read-only.
    /// </summary>
    public bool ContainsNumber
    {
      get
      {
        switch( CellType )
        {
          case TCellType.Formula:
            return FormulaStringValue == null;

          case TCellType.Number:
          case TCellType.RK:
            return true;

          default:
            return false;
        }
      }
    }
    /// <summary>
    /// Tries to convert string into datetime value.
    /// </summary>
    /// <param name="value">String to parse.</param>
    /// <param name="dateValue">Converted value.</param>
    /// <returns>True if conversion succeeded, false otherwise.</returns>
    internal bool TryParseDateTime( string value, out DateTime dateValue )
    {
      if( !m_book.DetectDateTimeInValue )
      {
        dateValue = DateTime.MinValue;
        return false;
      }

      return DateTime.TryParse( value, out dateValue );
    }
    /// <summary>
    /// Parses R1C1 reference.
    /// </summary>
    /// <param name="strReference">Reference to parse.</param>
    /// <returns>Range that corresponds to the string.</returns>
    private IRange ParseR1C1Reference( string strReference )
    {
      if( strReference == null )
        throw new ArgumentNullException( "strReference" );

      if( strReference.Length == 0 )
        throw new ArgumentException( "strReference - string cannot be empty." );

      string[] cells = strReference.Split( ':' );
      int iLen = cells.Length;

      if( iLen > 2 )
        throw new ArgumentOutOfRangeException( "strReference" );

      Rectangle result = Rectangle.FromLTRB( 1, 1, m_book.MaxColumnCount
        , m_book.MaxRowCount );

      result = ParseR1C1Expression( cells[ 0 ], result, true );

      if( iLen == 2 )
        result = ParseR1C1Expression( cells[ 1 ], result, false );

      return this[ result.Top, result.Left, result.Bottom, result.Right ];
    }
    /// <summary>
    /// Parses R1C1 expression
    /// </summary>
    /// <param name="strName">String to parse.</param>
    /// <param name="rec">Represents rectangle of coordinates.</param>
    /// <param name="bIsFirst">Indicates is it first expression.</param>
    /// <returns>Returns rectangle with updated coordinates.</returns>
    private Rectangle ParseR1C1Expression( string strName, Rectangle rec, bool bIsFirst )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentOutOfRangeException( "strName is empty." );

      int iColumnStart = strName.IndexOf( DEF_R1C1_COLUMN );
      bool bRowPresent = strName[ 0 ] == DEF_R1C1_ROW;
      bool bColPresent = iColumnStart != -1;

      if( !bRowPresent && !bColPresent )
        throw new ArgumentOutOfRangeException( "strReference", "Can't locate row or column section." );

      string strColumn = bColPresent
        ? strName.Substring( iColumnStart + 1 )
        : null;

      int iRowSectionLen = ( bColPresent ? iColumnStart : strName.Length ) - 1;
      string strRow = bRowPresent
        ? strName.Substring( 1, iRowSectionLen )
        : null;

      int iRowIndex = GetIndexFromR1C1( strRow, true );
      int iColumnIndex = GetIndexFromR1C1( strColumn, false );

      if( bRowPresent )
      {
        if( bIsFirst )
        {
          rec.Y = iRowIndex;
          rec.Height = 0;
        }
        else
        {
          rec.Height = iRowIndex - rec.Y;
        }
      }

      if( bColPresent )
      {
        if( bIsFirst )
        {
          rec.X = iColumnIndex;
          rec.Width = 0;
        }
        else
        {
          rec.Width = iColumnIndex - rec.X;
        }
      }

      return rec;
    }
    /// <summary>
    /// Parses index string in R1C1 style and evaluates absolute row or column index.
    /// </summary>
    /// <param name="strValue">Value to parse.</param>
    /// <param name="bRow">Indicates whether this is row or column index.</param>
    /// <returns>Parsed row or column index.</returns>
    private int GetIndexFromR1C1( string strValue, bool bRow )
    {
      if( strValue == null )
      {
        return ( bRow )
          ? m_book.MaxRowCount
          : m_book.MaxColumnCount;
      }

      int iLength = strValue.Length;

      if( iLength == 0 )
      {
        return ( bRow )
          ? Row
          : Column;
      }

      bool bRelative = false;

      if( strValue[ 0 ] == DEF_R1C1_OPENBRACKET && strValue[ iLength - 1 ] == DEF_R1C1_CLOSEBRACKET )
      {
        strValue = strValue.Substring( 1, iLength - 2 );
        bRelative = true;
      }

      double dResult;

      if( double.TryParse( strValue, NumberStyles.Integer, null, out dResult )
        && dResult >= int.MinValue && dResult <= int.MaxValue )
      {
        int iIndex = ( int )dResult;

        if( bRelative )iIndex += bRow ? Row : Column;

        return iIndex;
      }

      throw new ApplicationException( "Cannot parse expression." );
    }
    /// <summary>
    /// Converts array-entered formula to string..
    /// </summary>
    /// <param name="bR1C1">Indicates whether R1C1 notation must be used.</param>
    /// <returns>String representation of the array-entered formula.</returns>
    private string GetFormulaArray( bool bR1C1 )
    {
      if( CellType != TCellType.Formula && !IsSingleCell ) return null;

      string strResult = null;

      if( IsSingleCell )
      {
        FormulaRecord formula = Record as FormulaRecord;
        if( formula != null && m_worksheet.IsArrayFormula( formula ) )
        {
          strResult = ParseFormula( formula, bR1C1 );
        }
      }
      else
      {
        MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
        range.ResetRowColumn( Row, Column );
        strResult = range.GetFormulaArray( bR1C1 );

        if( strResult != null )
        {
          for( int i = Row, iLen = LastRow; i <= iLen; i++ )
          {
            for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
            {
              range.ResetRowColumn( i, j );
              string value = range.GetFormulaArray( bR1C1 );

              if( strResult != value )
              {
                strResult = null;
                break;
              }
            }
          }
        }
      }

      return strResult;
    }
    /// <summary>
    /// Sets array-entered formula.
    /// </summary>
    /// <param name="value">String representation of the formula.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation is used.</param>
    private void SetFormulaArray( string value, bool bR1C1 )
    {
      if( value == null )
        throw new ArgumentNullException( "FormulaArray" );

      int iLength = value.Length;

      if( iLength == 0 )
        throw new ArgumentException( "FormulaArray can't be empty" );

      if( value.StartsWith( "{=" ) && value[ iLength - 1 ] == '}' )
      {
        value = value.Substring( 2, iLength - 3 );
        iLength -= 3;
      }

      else if( value[ 0 ] == '=' )
        value = value.Substring( 1, iLength - 1 );

      TryRemoveFormulaArrays();
      ExcelParseFormulaOptions options = ExcelParseFormulaOptions.RootLevel
        | ExcelParseFormulaOptions.InArray;

      if( bR1C1 ) options |= ExcelParseFormulaOptions.UseR1C1;

      Ptg[] arrPtgs = m_book.FormulaUtil.ParseString( value, m_worksheet,
        null, 0, null, options, Row - 1, Column - 1 );

      ArrayRecord record = ( ArrayRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Array );
      record.Formula = arrPtgs;
      record.IsRecalculateOnOpen = true;

      record.FirstRow = FirstRow - 1;
      record.FirstColumn = FirstColumn - 1;
      record.LastRow = LastRow - 1;
      record.LastColumn = LastColumn - 1;

      SetFormulaArrayRecord( record );
    }
    /// <summary>
    /// Sets array formula record.
    /// </summary>
    /// <param name="record">Formula array record.</param>
    [ CLSCompliant( false ) ]
    public void SetFormulaArrayRecord( ArrayRecord record )
    {
      SetFormulaArrayRecord( record, ArrayFormulaXFFlag );
    }
    /// <summary>
    /// Sets array formula record.
    /// </summary>
    /// <param name="record">Formula array record.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    [CLSCompliant( false )]
    public void SetFormulaArrayRecord( ArrayRecord record, int iXFIndex )
    {
      Ptg control = FormulaUtil.CreatePtg( FormulaToken.tExp, record.FirstRow, record.FirstColumn );
      //new ControlPtg( FormulaToken.tExp, record.FirstRow, record.FirstCol );
      Ptg[] arrPtgs = new Ptg[] { control };

      FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );
      formula.ParsedExpression = arrPtgs;

      if( iXFIndex != ArrayFormulaXFFlag )
        formula.ExtendedFormatIndex = ( ushort )iXFIndex;

      if( IsSingleCell )
      {
        UpdateRecord( formula, this, iXFIndex );
        Record = formula;
      }
      else
      {
        RangeImpl cell = ( RangeImpl )m_worksheet[ FirstRow, FirstColumn ];
        UpdateRecord( formula, cell, iXFIndex );
        cell.Record = formula;

        for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
        {
          for( int iColumn = FirstColumn; iColumn <= LastColumn; iColumn++ )
          {
            cell = ( RangeImpl )m_worksheet[ iRow, iColumn ];
            formula = ( FormulaRecord )formula.Clone();
            UpdateRecord( formula, cell, iXFIndex );
            cell.SetFormula( formula );
          }
        }
      }

      m_worksheet.CellRecords.SetArrayFormula( record );
    }
    /// <summary>
    /// Updates record with new position and extended format record if necessary.
    /// </summary>
    /// <param name="record">Record to update.</param>
    /// <param name="cell">Cell to get data from.</param>
    /// <param name="iXFIndex">XF index to set.</param>
    private void UpdateRecord( ICellPositionFormat record, RangeImpl cell, int iXFIndex )
    {
      record.Row = cell.FirstRow - 1;
      record.Column = cell.FirstColumn - 1;

      if( iXFIndex == ArrayFormulaXFFlag )
        record.ExtendedFormatIndex = cell.ExtendedFormatIndex;
    }
    /// <summary>
    /// Normalizes row index.
    /// </summary>
    /// <param name="iRow">One-based row index to normalize.</param>
    /// <param name="iStartCol">First column.</param>
    /// <param name="iEndCol">Last column.</param>
    /// <returns>Normalized row index.</returns>
    private int NormalizeRowIndex( int iRow, int iStartCol, int iEndCol )
    {
      switch( iRow )
      {
        case ExcelConstants.MinimumIndex:
          return 1;

        case ExcelConstants.MaximumIndex:
          return m_book.MaxRowCount;

        case ExcelConstants.MinimumUsedIndex:
          return m_worksheet.CellRecords.GetMinimumRowIndex( iStartCol, iEndCol );

        case ExcelConstants.MaximumUsedIndex:
          return m_worksheet.CellRecords.GetMaximumRowIndex( iStartCol, iEndCol );

        default:
          return iRow;
      };
    }

    /// <summary>
    /// Normalizes row index.
    /// </summary>
    /// <param name="iColumn">One-based column index to normalize.</param>
    /// <param name="iStartRow">First row.</param>
    /// <param name="iEndRow">Last row.</param>
    /// <returns>Normalized column index.</returns>
    private int NormalizeColumnIndex( int iColumn, int iStartRow, int iEndRow )
    {
      switch( iColumn )
      {
        case ExcelConstants.MinimumIndex:
          return 1;

        case ExcelConstants.MaximumIndex:
          return m_book.MaxColumnCount;

        case ExcelConstants.MinimumUsedIndex:
          return m_worksheet.CellRecords.GetMinimumColumnIndex( iStartRow, iEndRow );

        case ExcelConstants.MaximumUsedIndex:
          return m_worksheet.CellRecords.GetMaximumColumnIndex( iStartRow, iEndRow );

        default:
          return iColumn;
      };
    }

    /// <summary>
    /// Searches for corresponding data validation.
    /// </summary>
    /// <returns>Found data validation or null.</returns>
    private DataValidationImpl FindDataValidation()
    {
      //if( !IsSingleCell )
      //  throw new ApplicationException( "Can't call this method for ranges that contsist of multiple cells" );

      DataValidationTable dvTable = m_worksheet.DVTable;
      //return dvTable.FindDataValidation( m_iTopLeftCell );
      long lCellIndex = RangeImpl.GetCellIndex( m_iLeftColumn, m_iTopRow );
      return dvTable.FindDataValidation( lCellIndex );
    }
    /// <summary>
    /// Partially clear range.
    /// </summary>
    public void PartialClear()
    {
      m_cells = null;
      m_style = null;
      m_bCells = false;
      m_dataValidation = null;
      m_rtfString = null;
    }
    /// <summary>
    /// Sets border to single cell.
    /// </summary>
    /// <param name="borderIndex">Represents border index.</param>
    /// <param name="borderLine">Represents border line type.</param>
    /// <param name="borderColor">Represents border line color.</param>
    protected void SetBorderToSingleCell( ExcelBordersIndex borderIndex, ExcelLineStyle borderLine
      , ExcelKnownColors borderColor )
    {
      if( !IsSingleCell )
        throw new NotSupportedException( "Supports only for single cell." );

      IBorder border = Borders[ borderIndex ];

      border.LineStyle = borderLine;
      //border.ColorObject.SetIndexed( borderColor );
      border.Color = borderColor;
    }
    /// <summary>
    /// Collapses or expands this group.
    /// </summary>
    /// <param name="groupBy">Should we perform operation on rows or columns?</param>
    /// <param name="isCollapsed">Indicates desired group state. If it is True then group should be collapsed, otherwise expanded.</param>
    /// <param name="flags">Flag indicating collapse or expand settings.</param>
    private void CollapseExpand( ExcelGroupBy groupBy, bool isCollapsed, ExpandCollapseFlags flags )
    {
      int iStartIndex;
      int iEndIndex;
      int iMaxIndex;
      OutlineGetter outlineGetter;
      bool bLastIndex;

      if( groupBy == ExcelGroupBy.ByRows )
      {
        iStartIndex = Row;
        iEndIndex = LastRow;
        iMaxIndex = m_book.MaxRowCount;
        bLastIndex = m_worksheet.PageSetup.IsSummaryRowBelow;
        outlineGetter = GetRowOutline;
      }
      else
      {
        iStartIndex = Column;
        iEndIndex = LastColumn;
        iMaxIndex = m_book.MaxColumnCount;
        bLastIndex = m_worksheet.PageSetup.IsSummaryColumnRight;
        outlineGetter = GetColumnOutline;
      }

      CollapseExpand( isCollapsed, iStartIndex, iEndIndex, iMaxIndex, bLastIndex, outlineGetter, flags );
    }
    /// <summary>
    /// Collapses or expands group.
    /// </summary>
    /// <param name="isCollapsed">Indicates whether group is collapsed.</param>
    /// <param name="iStartIndex">Represents start index.</param>
    /// <param name="iEndIndex">Represents end index.</param>
    /// <param name="iMaxIndex">Represents maximum index.</param>
    /// <param name="bLastIndex">Indicates whether last index.</param>
    /// <param name="outlineGetter">Provides outline information.</param>
    /// <param name="flags">Flag for expand or collapse setting.</param>
    private void CollapseExpand( bool isCollapsed, int iStartIndex, int iEndIndex, int iMaxIndex, bool bLastIndex,
      OutlineGetter outlineGetter, ExpandCollapseFlags flags )
    {
      bool bExpandSubgroups = ( flags & ExpandCollapseFlags.IncludeSubgroups ) != 0;
      int iOutlineIndex = ( bLastIndex ) ? iEndIndex : iStartIndex - 1;
      IOutline outline;// = outlineGetter( iOutlineIndex );

      if( iOutlineIndex <= iMaxIndex && iOutlineIndex > 0 )
      {
        outline = outlineGetter( iOutlineIndex );
        outline.IsCollapsed = isCollapsed;
      }


      IOutline startOutline = outlineGetter( iStartIndex );
      IOutline endOutline = outlineGetter( iEndIndex );
      int iCurrentGroupLevel = Math.Min( startOutline.OutlineLevel, endOutline.OutlineLevel );
      //if( outline.OutlineLevel != 0 )
      {
        // Should we hide items?
        //outline.IsCollapsed = true;
        int iParentStart = iStartIndex;
        int iParentEnd = iEndIndex;
        bool bParentVisible = IsParentGroupVisible( ref iParentStart, ref iParentEnd, iMaxIndex, outlineGetter );

        if( !bParentVisible && ( flags & ExpandCollapseFlags.ExpandParent ) != 0 )
        {
          bParentVisible = true;
          CollapseExpand( isCollapsed, iParentStart, iParentEnd, iMaxIndex, bLastIndex,
            outlineGetter, ExpandCollapseFlags.ExpandParent );
        }

        if( isCollapsed )
        {
          SetHiddenState( iStartIndex, iEndIndex, outlineGetter, true );
        }
        else if( bParentVisible )
        {
          ExpandOutlines( iStartIndex, iEndIndex, outlineGetter, bExpandSubgroups, bLastIndex );
          //for( int i = iStartIndex; i <= iEndIndex; i++ )
          //{
          //  outline = outlineGetter( i );

          //  if( isCollapsed ||
          //    ( bExpandSubgroups || outline.OutlineLevel == iCurrentGroupLevel ) && bParentVisible )
          //  {
          //    outline.IsHidden = isCollapsed;
          //  }

          //  if( !isCollapsed && bExpandSubgroups )
          //    outline.IsCollapsed = false;
          //}
        }
      }
    }
    /// <summary>
    /// Sets hidden state.
    /// </summary>
    /// <param name="iStartIndex">Represents starting index.</param>
    /// <param name="iEndIndex">Represents ending index.</param>
    /// <param name="outlineGetter">Provides Outline information.</param>
    /// <param name="state">Value indicating whether outline is hidden</param>
    private void SetHiddenState( int iStartIndex, int iEndIndex, OutlineGetter outlineGetter, bool state )
    {
      for( int i = iStartIndex; i <= iEndIndex; i++ )
      {
        IOutline outline = outlineGetter( i );
        outline.IsHidden = state;
		outline.IsCollapsed = state;
      }
    }
    /// <summary>
    /// Expands outlines.
    /// </summary>
    /// <param name="iStartIndex">Represents starting index.</param>
    /// <param name="iEndIndex">Represents end index.</param>
    /// <param name="outlineGetter">Provides outline information.</param>
    /// <param name="includeSubgroups">Value indicating whether to include sub groups</param>
    /// <param name="bLastIndex">Value indicating whether last index</param>
    private void ExpandOutlines( int iStartIndex, int iEndIndex, OutlineGetter outlineGetter,
      bool includeSubgroups, bool bLastIndex )
    {
      if( includeSubgroups )
      {
        SetHiddenState( iStartIndex, iEndIndex, outlineGetter, false );
      }
      else
      {
        int iDelta;

        if( bLastIndex )
        {
          SwapValues( ref iStartIndex, ref iEndIndex );
          iDelta = -1;
        }
        else
        {
          iDelta = 1;
        }

        for( int i = iStartIndex, end = iEndIndex + iDelta; i != end; i += iDelta )
        {
          IOutline outline = outlineGetter( i );

          // 1. Locate current group range + collapsed state.
          if( outline.IsCollapsed )
          {
            IOutline outlineGroupStart = outlineGetter( i + iDelta );

            if( outline.OutlineLevel >= outlineGroupStart.OutlineLevel )
            {
              outline.IsCollapsed = false;
              outline.IsHidden = false;
            }
            else
            {
              // skip group.
              i = FindGroupEdge( i + iDelta, iDelta, int.MaxValue, outlineGetter, outlineGroupStart.OutlineLevel );
              outline.IsHidden = false;
            }
          }
          else
          {
            outline.IsHidden = false;
          }
          // 2. if collapsed then skip
          // 3. otherwise show.
        }
      }
    }

    private void SwapValues( ref int iStartIndex, ref int iEndIndex )
    {
      int temp = iEndIndex;
      iEndIndex = iStartIndex;
      iStartIndex = temp;
    }
    /// <summary>
    /// Determines whether parent outline group is visible or not.
    /// </summary>
    /// <param name="iStartIndex">Start index of the child group.</param>
    /// <param name="iEndIndex">End index of the child group.</param>
    /// <param name="iMaxIndex">Maximum possible outline index.</param>
    /// <param name="outlineGetter">Method that is used to get outline by index.</param>
    /// <returns>True if parent group is visible.</returns>
    private bool IsParentGroupVisible( ref int iStartIndex, ref int iEndIndex, int iMaxIndex, OutlineGetter outlineGetter )
    {
      // 1. Locate parent group
      IOutline outline = outlineGetter( iStartIndex );
      int iCurrentGroupLevel = outline.OutlineLevel;

      if( iCurrentGroupLevel <= 1 )
        return true;

      int iParentPossibleStart = FindFirstWithLowerLevel( iStartIndex, -1, iMaxIndex, outlineGetter );
      int iParentPossibleEnd = FindFirstWithLowerLevel( iEndIndex, 1, iMaxIndex, outlineGetter );

      int iParentStartLevel = ( iParentPossibleStart > 0 ) ?
        ( int )outlineGetter( iParentPossibleStart ).OutlineLevel :
        0;

      int iParentEndLevel = ( iParentPossibleEnd > 0 ) ?
        ( int )outlineGetter( iParentPossibleEnd ).OutlineLevel :
        0;

      int iParentGroupLevel = Math.Min( iParentStartLevel, iParentEndLevel );

      if( iParentGroupLevel == 0 )
        return true;

      int iParentStart = FindGroupEdge( iStartIndex, -1, iMaxIndex, outlineGetter, iParentGroupLevel );
      int iParentEnd = FindGroupEdge( iEndIndex, 1, iMaxIndex, outlineGetter, iParentGroupLevel );

      // 2. Check its visibility
      int iOutlineLevel = outlineGetter( iParentStart ).OutlineLevel;

      iStartIndex = iParentStart;
      iEndIndex = iParentEnd;
      return FindVisibleOutline( iStartIndex, iEndIndex, outlineGetter, iOutlineLevel ) != -1;
    }
    /// <summary>
    /// Searches for the first outline with lower level (parent).
    /// </summary>
    /// <param name="startIndex">Start index to search from.</param>
    /// <param name="delta">Delta to add to outline index after each iteration (direction).</param>
    /// <param name="maximum">Maximum possible outline index.</param>
    /// <param name="outlineGetter">Method that is used to get outline by index.</param>
    /// <returns>Found outline index with lower level or -1.</returns>
    private int FindFirstWithLowerLevel( int startIndex, int delta, int maximum, OutlineGetter outlineGetter )
    {
      // 1. Find first with lower level
      int iCurrentLevel = outlineGetter( startIndex ).OutlineLevel;
      int iResult = -1;

      for( int iParentStart = startIndex + delta; iParentStart > 0 && iParentStart <= maximum;
        iParentStart += delta )
      {
        IOutline outline = outlineGetter( iParentStart );

        if( outline.OutlineLevel < iCurrentLevel )
        {
          iResult = iParentStart;
          break;
        }
      }

      return iResult;
    }
    /// <summary>
    /// Searches for the edge of the group.
    /// </summary>
    /// <param name="startIndex">Start index to search from.</param>
    /// <param name="delta">Delta to add to the index at each iteration.</param>
    /// <param name="maximum">Maximum possible outline index.</param>
    /// <param name="outlineGetter">Method that is used to get outline by index.</param>
    /// <param name="parentGroupLevel">Outline level of the group to find edge for.</param>
    /// <returns>Index of the parent group start.</returns>
    private int FindGroupEdge( int startIndex, int delta, int maximum, OutlineGetter outlineGetter, int parentGroupLevel )
    {
      int iParentStart = startIndex;
      IOutline outline;

      do
      {
        iParentStart += delta;
        outline = outlineGetter( iParentStart );
      }
      while( iParentStart > 0 && iParentStart <= maximum && outline.OutlineLevel >= parentGroupLevel );

      //for( ; iParentStart > 0 &&
      //  iParentStart <= maximum &&
      //  outline.OutlineLevel >= parentGroupLevel;
      //  iParentStart += delta )
      //{
      //  outline = outlineGetter( iParentStart );
      //}

      iParentStart -= delta;
      return iParentStart;
    }
    /// <summary>
    /// Searches for the first visible outline of the required level.
    /// </summary>
    /// <param name="startIndex">Start index to search.</param>
    /// <param name="endIndex">End index to search.</param>
    /// <param name="outlineGetter">Method that is used to get outline by index.</param>
    /// <param name="outlineLevel">Outline level to check.</param>
    /// <returns>Index of the first found visible outline.</returns>
    private int FindVisibleOutline( int startIndex, int endIndex, OutlineGetter outlineGetter, int outlineLevel )
    {
      int iOutlineIndex = -1;

      for( int i = startIndex; i <= endIndex; i++ )
      {
        IOutline outline = outlineGetter( i );

        if( outline.OutlineLevel == outlineLevel && !outline.IsHidden )
        {
          iOutlineIndex = i;
          break;
        }
      }

      return iOutlineIndex;
    }
    /// <summary>
    /// Gets the unique values.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <returns>Gets the list of unique values.</returns>
    internal IList<object> GetUniqueValues(ref PivotDataType fieldType)
    {
        bool checkInteger = true;
        object obj = new object();
        Dictionary<object,object> uniqueValues = new Dictionary<object,object>();
        Dictionary<string, object> caseInSensitive = new Dictionary<string, object>();
        int row = Row;
        int column = Column;
        int lastRow = LastRow;
        bool isLongText = false;
        for (int i = row; i <= lastRow; i++)
        {
            TRangeValueType valType = m_worksheet.GetCellType(i, column, false);
            object value = null;
            switch (valType)
            {
                case TRangeValueType.Blank:
                    string strValue = string.Empty;
                    fieldType |= PivotDataType.String;
                    value = strValue;
                    if (!uniqueValues.ContainsKey(value))
                    {
                        caseInSensitive.Add(strValue,obj);
                        uniqueValues.Add(value,obj);
                    }
                    break;
                case TRangeValueType.String:
               
                     strValue = m_worksheet.GetText(i, column);
                    if (strValue != null && strValue.Length > 0 || strValue == string.Empty)
                    {
                        isLongText = false;
                        if (strValue.Length > PivotCacheFieldImpl.MaxStringLength)
                        {
                            fieldType |= PivotDataType.LongText;
                            isLongText = true;
                        }
                        fieldType |= PivotDataType.String;
                    }
               
                    else
                        fieldType |= PivotDataType.Blank;
                    value = strValue;
                    strValue = strValue.ToLower();
                    if (!caseInSensitive.ContainsKey(strValue))
                    {
                        caseInSensitive.Add(strValue,obj);
                        uniqueValues.Add(value,obj);
                    }
                    break;

                case TRangeValueType.Number:
                    double dValue = m_worksheet.GetNumber(i, column);

                    if (!Double.IsNaN(dValue))//value != Double.NaN )
                    {
                        ExcelFormatType formatType = InnerNumberFormat.GetFormatType(dValue);

                        if (formatType == ExcelFormatType.Unknown && dValue == 0)
                        {
                            formatType = InnerNumberFormat.GetFormatType(1); // any positive number.
                        }

                        if (formatType == ExcelFormatType.Number || formatType == ExcelFormatType.General)
                        {
                            value = dValue;
                            if (checkInteger)
                            {
                                fieldType |= PivotDataType.Number;
                                fieldType |= PivotDataType.Integer;
                            }
                            if (dValue - Math.Floor(dValue) > 0)
                            {
                                fieldType &= ~PivotDataType.Integer ;
                                checkInteger = false;

                            }
                        }
                        else
                        {
                            FormatImpl formatImpl = this.InnerNumberFormat;
                            fieldType |= PivotDataType.Date;
                            value = (formatImpl.IsTimeFormat(dValue)) ?
                              (object)TimeSpan.FromDays(dValue) :
                              (object)UtilityMethods.ConvertNumberToDateTime(dValue,m_book.Date1904);
                         
                            DateTime dtime = (DateTime)value;
                            dtime = dtime.AddMilliseconds(-dtime.Millisecond);
                            value = (object)dtime;
                        }
                    }
                    else
                        value = dValue;
                    if (!uniqueValues.ContainsKey(value))
                        uniqueValues.Add(value,obj);
                    break;

                case TRangeValueType.Boolean:
                    value = m_worksheet.GetBoolean(i, column);
                    fieldType |= PivotDataType.Boolean;
                    if (!uniqueValues.ContainsKey(value))
                        uniqueValues.Add(value,obj);
                    break;

                case TRangeValueType.Formula:
                    bool isString = false;
                    value = GetFormulaValue(ref fieldType, i, column,ref isString);
                    if (isString)
                    {
                        strValue = value.ToString().ToLower();
                        if (!caseInSensitive.ContainsKey(strValue))
                        {
                            caseInSensitive.Add(strValue,obj);
                            uniqueValues.Add(value,obj);
                        }
                    }
                    else
                    {
                        if (!uniqueValues.ContainsKey(value))
                            uniqueValues.Add(value,obj);
                    }
                    break;
                default:
                    value = m_worksheet[i, column].DisplayText;
                    fieldType |= PivotDataType.String;
                      strValue = value.ToString().ToLower();
                        if (!caseInSensitive.ContainsKey(strValue))
                        {
                            caseInSensitive.Add(strValue,obj);
                            uniqueValues.Add(value,obj);
                        }
                    break;
            }
        }
        object[] values = new object[uniqueValues.Count];

        uniqueValues.Keys.CopyTo(values,0);
        uniqueValues = null;
        caseInSensitive = null;
        IList<object> list = new List<object>(values);
        return list;
    }
    /// <summary>
    /// Gets the formula value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="row">Represents the row to fetch the value.</param>
    /// <param name="column">Represents the column to fetch the value.</param>
    /// <param name="isString">if set to <c>true</c> [is string].</param>
    /// <returns>Returns the formula value.</returns>
    internal object GetFormulaValue(ref PivotDataType fieldType, int row, int column,ref bool isString)
    {
        object value = null;
        
        TCellType cellType = (m_worksheet[row, column] as RangeImpl).CellType;
        string strValue = m_worksheet.GetFormulaStringValue(row, column);
        TRangeValueType rangeType = m_worksheet.GetCellType(row, column, true);
        if ((rangeType & TRangeValueType.Boolean) == TRangeValueType.Boolean)
        {
            value = m_worksheet.GetFormulaBoolValue(row, column);
            fieldType |= PivotDataType.Boolean;
        }
        else if (strValue == null || cellType == TCellType.RK || cellType == TCellType.Number)
        {
            double formulaNumberValue = m_worksheet.GetFormulaNumberValue(row, column);

            if (!Double.IsNaN(formulaNumberValue))//value != Double.NaN )
            {
                ExcelFormatType formatType = InnerNumberFormat.GetFormatType(formulaNumberValue);

                if (formatType == ExcelFormatType.Unknown && formulaNumberValue == 0)
                {
                    formatType = InnerNumberFormat.GetFormatType(1); // any positive number.
                }

                if (formatType == ExcelFormatType.Number || formatType == ExcelFormatType.General)
                {
                    value = formulaNumberValue;

                    fieldType |= PivotDataType.Number;
                    fieldType |= (formulaNumberValue <= int.MaxValue && formulaNumberValue >= int.MinValue && Math.Round(formulaNumberValue) == formulaNumberValue) ?
         PivotDataType.Integer :
         PivotDataType.Float;
                }
                else
                {
                    FormatImpl formatImpl = this.InnerNumberFormat;
                    fieldType |= PivotDataType.Date;
                    value = (formatImpl.IsTimeFormat(formulaNumberValue)) ?
                      (object)TimeSpan.FromDays(formulaNumberValue) :
                      (object)UtilityMethods.ConvertNumberToDateTime(formulaNumberValue,m_book.Date1904); ;
                }
            }
            else
                value = formulaNumberValue;
        }
        else if (cellType == TCellType.Formula || cellType == TCellType.LabelSST || cellType == TCellType.RString || cellType == TCellType.Label)
        {
            value = m_worksheet.GetFormulaStringValue(row, column);
            fieldType |= PivotDataType.String;
            isString = true;
        }
        else if (cellType == TCellType.Blank)
        {
            value = string.Empty;
            fieldType |= PivotDataType.Blank;
            isString = true;
        }
        else
        {
            value = (m_worksheet[row, column] as RangeImpl).GetDisplayString();
            fieldType |= PivotDataType.String;
            isString = true;
        }
        return value;
    }
    /// <summary>
    /// Gets the formula value.
    /// </summary>
    /// <param name="fieldType">Type of the field.</param>
    /// <param name="row">Represents the row to fetch the value.</param>
    /// <param name="column">Represents the column to fetch the value.</param>
    /// <param name="isString">if set to <c>true</c> [is string].</param>
    /// <returns>Returns the formula value.</returns>
    private string GetFormulaValue(int row, int column, FormatImpl formatImpl)
    {
        string displayText = null;

        TCellType cellType = (m_worksheet[row, column] as RangeImpl).CellType;
        string strFValue = m_worksheet.GetFormulaStringValue(row, column);
        TRangeValueType rangeType = m_worksheet.GetCellType(row, column, true);
        if ((rangeType & TRangeValueType.Boolean) == TRangeValueType.Boolean)
        {
            bool bValue = m_worksheet.GetFormulaBoolValue(row, column);
            displayText = bValue.ToString();
        }
        else if (strFValue == null || cellType == TCellType.RK || cellType == TCellType.Number)
        {
            double formulaNumberValue = m_worksheet.GetFormulaNumberValue(row, column);
            displayText =(Double.IsNaN(formulaNumberValue))?"": formatImpl.ApplyFormat(formulaNumberValue);
        }
        else if (cellType == TCellType.Formula || cellType == TCellType.LabelSST || cellType == TCellType.RString || cellType == TCellType.Label)
        {
            string strValue = m_worksheet.GetFormulaStringValue(row, column);
            strValue = formatImpl.ApplyFormat(strValue);
        }
        else if (cellType == TCellType.Blank)
        {
            displayText = "";
        }
        else
        {
            displayText = (m_worksheet[row, column] as RangeImpl).GetDisplayString();
        }
        return displayText;
    }
    #endregion

    #region Auto format methods
    /// <summary>
    /// Sets auto format pattern.
    /// </summary>
    /// <param name="color">Represents pattern color.</param>
    /// <param name="iRow">Represents first row.</param>
    /// <param name="iLastRow">Represents last row.</param>
    /// <param name="iCol">Represents first column.</param>
    /// <param name="iLastCol">Represents last column.</param>
    private void SetAutoFormatPattern( ExcelKnownColors color, int iRow,
      int iLastRow, int iCol, int iLastCol )
    {
      SetAutoFormatPattern( color, iRow, iLastRow, iCol, iLastCol
        , ExcelKnownColors.Black ,ExcelPattern.Solid );
    }
    /// <summary>
    /// Sets auto format pattern.
    /// </summary>
    /// <param name="color">Represents pattern color.</param>
    /// <param name="iRow">Represents first row.</param>
    /// <param name="iLastRow">Represents last row.</param>
    /// <param name="iCol">Represents first column.</param>
    /// <param name="iLastCol">Represents last column.</param>
    /// <param name="patCol">Represents pattern color.</param>
    /// <param name="pat">Represents cell pattern.</param>
    private void SetAutoFormatPattern( ExcelKnownColors color, int iRow,
      int iLastRow, int iCol, int iLastCol, ExcelKnownColors patCol, ExcelPattern pat )
    {
      MigrantRangeImpl range = new MigrantRangeImpl( Application, Worksheet );

      for( int i = iRow; i <= iLastRow; i++ )
      {
        for( int j = iCol; j <= iLastCol; j++ )
        {
          range.ResetRowColumn( i, j );
          IStyle style = range.CellStyle;

          style.FillPattern = pat;
          style.ColorIndex = color;
          style.PatternColorIndex = patCol;
        }
      }
    }
    /// <summary>
    /// Sets auto format patterns.
    /// </summary>
    /// <param name="type">Represents auto format type.</param>
    private void SetAutoFormatPatterns( ExcelAutoFormat type )
    {
      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      ExcelKnownColors foreCol = ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX;
      ExcelKnownColors backCol = ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX;

      switch( type )
      {
        case ExcelAutoFormat.Classic_2:
          SetAutoFormatPattern( foreCol, iRow + 1, iLastRow, iCol + 1, iLastCol, backCol, ExcelPattern.None );
          SetAutoFormatPattern( ExcelKnownColors.Grey_25_percent, iRow + 1, iLastRow, iCol, iCol );
          SetAutoFormatPattern( ExcelKnownColors.Violet, iRow, iRow, iCol, iLastCol );
          return;

        case ExcelAutoFormat.Classic_3:
          SetAutoFormatPattern( foreCol, iLastRow, iLastRow, iCol, iLastCol, backCol, ExcelPattern.None );
          SetAutoFormatPattern( ExcelKnownColors.Grey_25_percent, iRow + 1, iLastRow - 1, iCol, iLastCol );
          SetAutoFormatPattern( ExcelKnownColors.Dark_blue, iRow, iRow, iCol, iLastCol );
          return;

        case ExcelAutoFormat.Colorful_1:
          SetAutoFormatPattern( ExcelKnownColors.Dark_blue, iRow + 1, iLastRow, iCol, iLastCol );
          SetAutoFormatPattern( ExcelKnownColors.Teal, iRow + 1, iLastRow, iCol + 1, iLastCol );
          SetAutoFormatPattern( ExcelKnownColors.Black, iRow, iRow, iCol, iLastCol );
          return;

        case ExcelAutoFormat.Colorful_2:
          int iIndex = ( iCol == iLastCol ) ? iLastCol : iLastCol - 1;

          SetAutoFormatPattern( ExcelKnownColors.Grey_25_percent, iRow + 1, iLastRow, iLastCol, iLastCol );
          SetAutoFormatPattern( ExcelKnownColors.YellowCustom, iRow + 1, iLastRow, iCol, iIndex
            , ExcelKnownColors.WhiteCustom, ExcelPattern.Percent75Gray );
          SetAutoFormatPattern( ExcelKnownColors.Dark_red, iRow, iRow, iCol, iLastCol );
          return;

        case ExcelAutoFormat.Colorful_3:
          SetAutoFormatPattern( ExcelKnownColors.Black, iRow, iLastRow, iCol, iLastCol );
          return;

        case ExcelAutoFormat.List_1:
          SetListAutoFormatPattern( true, foreCol, backCol );
          return;

        case ExcelAutoFormat.List_2:
          SetListAutoFormatPattern( false, foreCol, backCol );
          return;

        case ExcelAutoFormat.Effect3D_1:
        case ExcelAutoFormat.Effect3D_2:
          SetAutoFormatPattern( ExcelKnownColors.Grey_25_percent, iRow, iLastRow, iCol, iLastCol );
          return;

        default:
          SetAutoFormatPattern( foreCol, iRow, iLastRow, iCol, iLastCol, backCol, ExcelPattern.None );
          return;
      }
    }
    /// <summary>
    /// Sets auto format pattern for list_1 or list_2 types.
    /// </summary>
    /// <param name="bIsList_1">Indicates if it is list_1 auto format type.</param>
    /// <param name="foreCol">Represents default fore color.</param>
    /// <param name="backColor">Represents default back color.</param>
    private void SetListAutoFormatPattern( bool bIsList_1, ExcelKnownColors foreCol
      , ExcelKnownColors backColor )
    {
      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;

      SetAutoFormatPattern( foreCol, iLastRow, iLastRow, iCol, iLastCol, backColor, ExcelPattern.None );

      ExcelKnownColors color = ( bIsList_1 )
        ? ExcelKnownColors.Grey_25_percent
        : ExcelKnownColors.Light_green;

      int div = ( bIsList_1 ) ? 2 : 4;

      for( int i = 0, iLen = iLastRow - iRow - 1; i < iLen; i++ )
      {
        if( ( i % div ) < div / 2 )
        {
          SetAutoFormatPattern( color, i + iRow + 1, i + iRow + 1, iCol, iLastCol );
        }
        else
        {
          SetAutoFormatPattern( foreCol, i + iRow + 1, i + iRow + 1, iCol, iLastCol
            , backColor, ExcelPattern.None );
        }
      }

      if( bIsList_1 )
      {
        SetAutoFormatPattern( color, iRow, iRow, iCol, iLastCol );
      }
      else
      {
        SetAutoFormatPattern( ExcelKnownColors.Green, iRow, iRow, iCol, iLastCol
          , ExcelKnownColors.Teal, ExcelPattern.Percent75Gray );
      }
    }
    /// <summary>
    /// Sets auot format alignment.
    /// </summary>
    /// <param name="type">Represents auto format type.</param>
    private void SetAutoFormatAlignments( ExcelAutoFormat type )
    {
      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;

      if( type == ExcelAutoFormat.None )
      {
        SetAutoFormatAlignment( ExcelHAlign.HAlignGeneral, iRow, iLastRow, iCol, iLastCol );

        return;
      }

      ExcelHAlign align = ExcelHAlign.HAlignLeft;

      SetAutoFormatAlignment( ExcelHAlign.HAlignGeneral, iRow + 1, iLastRow, iCol + 1, iLastCol );
      SetAutoFormatAlignment( ExcelHAlign.HAlignGeneral, iRow, iRow, iCol, iCol );

      if( iRow != iLastRow )
        SetAutoFormatAlignment( ExcelHAlign.HAlignLeft, iLastRow, iLastRow, iCol, iCol );

      if( type == ExcelAutoFormat.List_3 )
        align = ExcelHAlign.HAlignGeneral;

      SetAutoFormatAlignment( align, iRow + 1, iLastRow - 1, iCol, iCol );
      align = ExcelHAlign.HAlignCenter;

      if( Array.IndexOf( DEF_AUTOFORMAT_RIGHT, type ) != -1 )
        align = ExcelHAlign.HAlignRight;

      SetAutoFormatAlignment( align, iRow, iRow, iCol + 1, iLastCol );
    }
    /// <summary>
    /// Sets auto format alignment.
    /// </summary>
    /// <param name="align">Represents align.</param>
    /// <param name="iRow">Represents first row.</param>
    /// <param name="iLastRow">Represents last row.</param>
    /// <param name="iCol">Represents first column.</param>
    /// <param name="iLastCol">Represents last column.</param>
    private void SetAutoFormatAlignment( ExcelHAlign align, int iRow,
      int iLastRow, int iCol, int iLastCol )
    {
      MigrantRangeImpl range = new MigrantRangeImpl( Application, Worksheet );

      for( int i = iRow; i <= iLastRow; i++ )
      {
        for( int j = iCol; j <= iLastCol; j++ )
        {
          range.ResetRowColumn( i, j );
          IStyle style = range.CellStyle;

          style.HorizontalAlignment = align;
          style.VerticalAlignment = ExcelVAlign.VAlignBottom;
          style.Rotation = 0;
          style.IndentLevel = 0;
        }
      }
    }
    /// <summary>
    /// Sets auto format width height.
    /// </summary>
    /// <param name="type">Represents auto format type.</param>
    private void SetAutoFormatWidthHeight( ExcelAutoFormat type )
    {
      if( type == ExcelAutoFormat.None )
        return;

      for( int i = FirstRow, iLen = LastRow; i <= iLen; i++ )
      {
        Worksheet.AutofitRow( i );
      }

      for( int i = FirstColumn, iLen = LastColumn; i <= iLen; i++ )
      {
        Worksheet.AutofitColumn( i );
      }
    }
    /// <summary>
    /// Sets auto format number.
    /// </summary>
    /// <param name="type">Represents auto format type.</param>
    private void SetAutoFormatNumbers( ExcelAutoFormat type )
    {
      bool bIsNone = type == ExcelAutoFormat.None;

      if( !bIsNone && Array.IndexOf( DEF_AUTOFORMAT_NUMBER, type ) == -1 )
        return;

      MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
      int iIndex = DEF_AUTOFORMAT_NUMBER_INDEX;

      for( int i = Row + 1, iLen = LastRow; i <= iLen; i++ )
      {
        for( int j = Column, iCount = LastColumn; j <= iCount; j++ )
        {
          range.ResetRowColumn( i, j );
          IStyle style = range.CellStyle;

          if( !bIsNone )
          {
            iIndex = ( i == Row + 1 )
              ? DEF_AUTOFORMAT_NUMBER_INDEX_2
              : DEF_AUTOFORMAT_NUMBER_INDEX_1;
          }

          style.NumberFormatIndex = iIndex;
        }
      }
    }
    /// <summary>
    /// Sets auto format font border.
    /// </summary>
    /// <param name="type">Represents auto format type.</param>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorder( ExcelAutoFormat type, bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      switch( type )
      {
        case ExcelAutoFormat.Simple:
          SetAutoFormatSimpleFontBorder( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Classic_1:
          SetAutoFormatFontBorderClassic_1( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Classic_2:
          SetAutoFormatFontBorderClassic_2( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Classic_3:
          SetAutoFormatFontBorderClassic_3( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Accounting_1:
          SetAutoFormatFontBorderAccounting_1( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Accounting_2:
          SetAutoFormatFontBorderAccounting_2( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Accounting_3:
          SetAutoFormatFontBorderAccounting_3( bIsFont, bIsBorder );
          return;

        case ExcelAutoFormat.Accounting_4:
          SetAutoFormatFontBorderAccounting_4( bIsFont, bIsBorder );
          return;

        default:
          throw new NotSupportedException( "Unknown auto format type." );
      }
    }
    /// <summary>
    /// Sets auto format simple font border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatSimpleFontBorder( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      bool bIsOneRow = iRow == iLastRow;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )m_book.InnerFonts[ 0 ];
        font = font.Clone( m_book.InnerFonts );

        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );

        int iIndex = ( bIsOneRow )
          ? iLastRow
          : iLastRow - 1;

        SetAutoFormatFont( font, iRow, iIndex, iCol, iCol );

        font = font.Clone( m_book.InnerFonts );
        font.Bold = true;

        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol );

        if( !bIsOneRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol, iCol );
      }
    }
    /// <summary>
    /// Sets auto format Classic_1 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderClassic_1( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      bool bIsOneRow = iRow == iLastRow;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )m_book.InnerFonts[ 0 ];

        font = font.Clone( m_book.InnerFonts );
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );

        int iIndex = ( bIsOneRow )
          ? iLastRow
          : iLastRow - 1;

        SetAutoFormatFont( font, iRow, iIndex, iCol, iCol );

        font = font.Clone( m_book.InnerFonts );
        font.Bold = true;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol );

        if( !bIsOneRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol, iCol );

        if( iCol != iLastCol )
          SetAutoFormatFont( font, iRow, iRow, iLastCol, iLastCol );

        font = font.Clone( m_book.InnerFonts );
        font.Bold = false;
        font.Italic = true;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol - 1 );
      }
    }
    /// <summary>
    /// Sets auto format Classic_2 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderClassic_2( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      bool bIsOneRow = iRow == iLastRow;
      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];

        font = font.Clone( fonts );
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );
        SetAutoFormatFont( font, iRow, iRow, iCol, iCol );

        font = font.Clone( fonts );
        font.Bold = true;
        SetAutoFormatFont( font, iRow + 1, iLastRow - 1, iCol, iCol );

        font = font.Clone( fonts );
        font.Color = ExcelKnownColors.Dark_blue;

        if( iRow != iLastRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol, iCol );

        font = font.Clone( fonts );
        font.Bold = false;
        font.Size = 9;
        font.Color = ExcelKnownColors.White;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol - 1 );

        font = font.Clone( fonts );
        font.Bold = true;

        if( iCol != iLastCol )
          SetAutoFormatFont( font, iRow, iRow, iLastCol, iLastCol );
      }
    }
    /// <summary>
    /// Sets auto format Classic_3 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderClassic_3( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];

        font = font.Clone( fonts );
        font.Color = ExcelKnownColors.Dark_blue;
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );
        SetAutoFormatFont( font, iRow, iRow, iCol, iCol );

        font = font.Clone( fonts );
        font.Bold = true;
        font.Color = ExcelKnownColors.Black;
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol, iCol );

        font = font.Clone( fonts );
        font.Color = ExcelKnownColors.White;
        font.Italic = true;
        font.Size = 9;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol );
      }
    }
    /// <summary>
    /// Sets auto format Accounting_1 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderAccounting_1( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];

        font = font.Clone( fonts );
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );
        SetAutoFormatFont( font, iRow, iRow, iCol, iCol );
        SetAutoFormatFont( font, iRow + 1, iLastRow - 1, iCol, iCol );

        font = font.Clone( fonts );
        font.Italic = true;

        if( iRow != iLastRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol, iCol );

        font = font.Clone( fonts );
        font.Bold = true;
        font.Size = 9;

        if( iCol != iLastCol )
          SetAutoFormatFont( font, iRow, iRow, iLastCol, iLastCol );

        font = font.Clone( fonts );
        font.Color = ExcelKnownColors.Grey_50_percent;

        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol - 1 );
      }
    }
    /// <summary>
    /// Sets auto format Accounting_2 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderAccounting_2( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];
        font = font.Clone( fonts );

        SetAutoFormatFont( font, Row, LastRow, Column, LastColumn );
      }
    }
    /// <summary>
    /// Sets auto format Accounting_3 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderAccounting_3( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];

        font = font.Clone( fonts );
        SetAutoFormatFont( font, iRow, iRow, iCol, iCol );
        SetAutoFormatFont( font, iRow + 1, iLastRow, iCol + 1, iLastCol );

        font = font.Clone( fonts );
        font.Italic = true;
        SetAutoFormatFont( font, iRow + 1, iLastRow - 1, iCol, iCol );

        font = font.Clone( fonts );
        font.Size = 9;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol - 1 );

        font = font.Clone( fonts );
        font.Bold = true;
        font.Italic = true;

        if( iCol != iLastCol )
          SetAutoFormatFont( font, iRow, iRow, iLastCol, iLastCol );

        font = font.Clone( fonts );
        font.Bold = true;
        font.Italic = false;
        font.Size = 10;

        if( iRow != iLastRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol, iCol );
      }
    }
    /// <summary>
    /// Sets auto format Accounting_4 font and border.
    /// </summary>
    /// <param name="bIsFont">Indicates if set font object.</param>
    /// <param name="bIsBorder">Indicates if set borders.</param>
    private void SetAutoFormatFontBorderAccounting_4( bool bIsFont, bool bIsBorder )
    {
      if( !bIsFont && !bIsBorder )
        return;

      int iRow = FirstRow;
      int iLastRow = LastRow;
      int iCol = FirstColumn;
      int iLastCol = LastColumn;
      FontsCollection fonts = m_book.InnerFonts;

      if( bIsFont )
      {
        FontImpl font = ( FontImpl )fonts[ 0 ];

        font = font.Clone( fonts );
        SetAutoFormatFont( font, iRow, iLastRow, iCol, iCol );
        SetAutoFormatFont( font, iRow + 1, iLastRow - 2, iCol + 1, iLastCol );

        font = font.Clone( fonts );
        font.Underline = ExcelUnderline.SingleAccounting;
        SetAutoFormatFont( font, iRow, iRow, iCol + 1, iLastCol );

        if( iLastRow - iRow > 1 )
          SetAutoFormatFont( font, iLastRow - 1, iLastRow - 1, iCol + 1, iLastCol );

        font = font.Clone( fonts );
        font.Underline = ExcelUnderline.DoubleAccounting;

        if( iRow != iLastRow )
          SetAutoFormatFont( font, iLastRow, iLastRow, iCol + 1, iLastCol );
      }
    }
    /// <summary>
    /// Sets auto format font.
    /// </summary>
    /// <param name="font">Represents font object.</param>
    /// <param name="iRow">Represents first row.</param>
    /// <param name="iLastRow">Represents last row.</param>
    /// <param name="iCol">Represents first column.</param>
    /// <param name="iLastCol">Represents last column.</param>
    private void SetAutoFormatFont( IFont font, int iRow,
      int iLastRow, int iCol, int iLastCol )
    {
      if( font == null )
        throw new ArgumentNullException( "font" );

      if( iRow > iLastRow || iCol > iLastCol )
        return;

      MigrantRangeImpl range = new MigrantRangeImpl( Application, Worksheet );
      range.ResetRowColumn( iRow, iCol );

      IFont fontToSet = range.CellStyle.Font;

      fontToSet.BeginUpdate();

      fontToSet.Bold = font.Bold;
      fontToSet.Color = font.Color;
      fontToSet.FontName = font.FontName;
      fontToSet.Italic = font.Italic;
      fontToSet.MacOSOutlineFont = font.MacOSOutlineFont;
      fontToSet.MacOSShadow = font.MacOSShadow;
      fontToSet.Size = font.Size;
      fontToSet.Strikethrough = font.Strikethrough;
      fontToSet.Subscript = font.Subscript;
      fontToSet.Superscript = font.Superscript;
      fontToSet.Strikethrough = font.Strikethrough;
      fontToSet.Underline = font.Underline;

      fontToSet.EndUpdate();

      int iFontIndex = range.m_style.FontIndex;

      for( int i = iRow; i <= iLastRow; i++ )
      {
        for( int j = iCol; j <= iLastCol; j++ )
        {
          range.ResetRowColumn( i, j );

          ExtendedFormatWrapper wraper = ( ExtendedFormatWrapper )range.CellStyle;
          wraper.FontIndex = iFontIndex;
        }
      }
    }
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Creates an object and sets its Application and Parent 
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <exception cref="System.ApplicationException">
    /// The parent worksheet or workbook cannot be found.
    /// </exception>
    public RangeImpl( IApplication application, object parent )
      //: base( application, parent )
    {
      SetParents( parent );
    }
    /// <summary>
    /// Recover Region from stream.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="reader">Stream with range data.</param>
    [ CLSCompliant( false ) ]
    public RangeImpl( IApplication application, object parent, BiffReader reader )
      : this( application, parent )
    {
      Parse( reader );
    }

    /// <summary>
    /// Recover Range from Biff records.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="data">Array of BiffRecordRaws which contains record for the range.</param>
    /// <param name="position">Index of record for the range.</param>
    [ CLSCompliant( false ) ]
    public RangeImpl( IApplication application, object parent, 
      BiffRecordRaw[] data, int position )
      : this( application, parent )
    {
      Parse( data, ref position );
    }
    /// <summary>
    /// Recover Range from Biff records.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="data">
    /// Array of BiffRecordRaws which contains record for the range.
    /// </param>
    /// <param name="position">Index of record for the range.</param>
    [ CLSCompliant( false ) ]
    public RangeImpl( IApplication application, object parent, 
      BiffRecordRaw[] data, ref int position )
      : this( application, parent )
    {
      Parse( data, ref position );
    }
    /// <summary>
    /// Recover Range from Biff records.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="data">
    /// Array of BiffRecordRaws which contains record for the range.
    /// </param>
    /// <param name="position">Index of record for the range.</param>
    /// <param name="ignoreStyles">Indicates whether to ignore styles.</param>
    [ CLSCompliant( false ) ]
    public RangeImpl( IApplication application, object parent, 
      BiffRecordRaw[] data, ref int position, bool ignoreStyles )
      : this( application, parent )
    {
      Parse( data, ref position, ignoreStyles );
    }
    /// <summary>
    /// Recover Range from Biff records.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="data">Array of BiffRecordRaws which contains record for the range.</param>
    /// <param name="position">Index of record for the range.</param>
    /// <param name="ignoreStyles">Indicates whether to ignore styles.</param>
    public RangeImpl( IApplication application, object parent, 
      List<BiffRecordRaw> data, ref int position, bool ignoreStyles )
      : this( application, parent )
    {
      Parse( data, ref position, ignoreStyles );
    }
    /// <summary>
    /// Creates range with specified top-left and bottom-right corners.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="firstRow">First row of the range.</param>
    /// <param name="lastRow">Last row of the range.</param>
    /// <param name="firstCol">First column of the range.</param>
    /// <param name="lastCol">Last column of the range.</param>
    public RangeImpl( IApplication application, object parent, 
      int firstCol, int firstRow, int lastCol, int lastRow )
      : this( application, parent )
    {
      if( firstCol > lastCol )
        throw new ArgumentOutOfRangeException( "firstCol or lastCol" );

      if( firstRow > lastRow )
        throw new ArgumentOutOfRangeException( "firstRow or lastRow" );

      FirstColumn = firstCol;
      FirstRow = firstRow;
      LastColumn = lastCol;
      LastRow = lastRow;
    }
    /// <summary>
    /// Creates range for specified single cell.
    /// </summary>
    /// <param name="application">Application object for the range.</param>
    /// <param name="parent">Parent object for the range.</param>
    /// <param name="row">Row index for the range.</param>
    /// <param name="column">Column index for the range.</param>
    public RangeImpl( IApplication application, object parent, int column, int row )
      : this( application, parent )
    {
      FirstColumn = column;
      LastColumn = column;
      
      FirstRow = row;
      LastRow = row;
    }

    /// <summary>
    /// Creates new range based on the record.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="record">Range record.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    [ CLSCompliant( false ) ]
    public RangeImpl( IApplication application, object parent, BiffRecordRaw record, bool bIgnoreStyles )
      : this( application, parent, new BiffRecordRaw[]{ record }, 0 )
    {
    }
    /// <summary>
    /// Fill internal collection by references on cells.
    /// </summary>
    internal protected void InfillCells()
    {
      if( !m_bCells )
      {
        m_cells = new List<IRange>();

        if( FirstRow > 0 && FirstColumn > 0 )
        {
          for( int iRow = FirstRow, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
          {
            for( int iCol = FirstColumn, iLastCol = LastColumn; iCol <= iLastCol; iCol++ )
            {
              m_cells.Add( m_worksheet.InnerGetCell( iCol, iRow ) );
            }
          }
        }

        m_bCells = true;
      }
    }
    /// <summary>
    /// Clears internal cells array.
    /// </summary>
    internal protected void ResetCells()
    {
      if( m_cells != null )
        m_cells.Clear();
      
      m_cells = null;
      m_bCells = false;
    }
    /// <summary>
    /// This method is called when disposing the object.
    /// </summary>
    public void Dispose()
    {
      if( m_style != null )
      {
        //m_style.Dispose();
        m_style = null;
      }

      if( m_rtfString != null ) m_rtfString.Dispose();

      //base.OnDispose();
    }
    private void CheckDisposed()
    {
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents( object parent )
    {
      m_worksheet = parent as WorksheetImpl;//CommonObject.FindParent( parent, typeof( WorksheetImpl ) ) as WorksheetImpl;
      
      if( m_worksheet == null )
        throw new ApplicationException( "Range object must be a child of worksheet object tree" );

      m_book = m_worksheet.ParentWorkbook;
    }
    #endregion

    #region Class parse/deparse
    /// <summary>
    /// Recover Region from the stream.
    /// </summary>
    /// <param name="reader">BiffReader with range record.</param>
    [ CLSCompliant( false ) ]
    public    void Parse( BiffReader reader )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Recover region from array of Biff Records and position in it.
    /// </summary>
    /// <param name="data">Array of BiffRecordRaws that contains range record.</param>
    /// <param name="position">Position of range record.</param>
    [ CLSCompliant( false ) ]
    public    void Parse( BiffRecordRaw[] data, ref int position )
    {
      Parse( data, ref position, false );
    }

    /// <summary>
    /// Recover region from array of Biff Records and position in it.
    /// </summary>
    /// <param name="data">Array of BiffRecordRaws that contains range record.</param>
    /// <param name="position">Position of range record.</param>
    /// <param name="ignoreStyles">Indicates whether to ignore style.</param>
    public    void Parse( IList data, ref int position, bool ignoreStyles )
    {
      BiffRecordRaw record = ( BiffRecordRaw )data[ position ];
      
      ICellPositionFormat num = ( ICellPositionFormat )record;
      FirstColumn = LastColumn = ( int )num.Column + 1;
      FirstRow = LastRow = ( int )num.Row + 1;
      //Record = record;

      switch( record.TypeCode )
      {
        case TBIFFRecord.Number:
        case TBIFFRecord.RK:
          ParseDouble( ( IDoubleValue )record );
          break;

        case TBIFFRecord.LabelSST: break;//ParseLabelSST( ( LabelSSTRecord )record ); break;
        case TBIFFRecord.Blank: ParseBlank( ( BlankRecord )record ); break;
        case TBIFFRecord.Formula: ParseFormula( ( FormulaRecord )record, data, ref position ); break;
        case TBIFFRecord.BoolErr: ParseBoolError( ( BoolErrRecord )record ); break;
        case TBIFFRecord.RString: ParseRString( ( RStringRecord )record ); break;
        case TBIFFRecord.Label: break;//this.Text = ( ( LabelRecord )record ).Label; break;

        default:
          throw new ArgumentException( "Unknown to Range biff record type" );
      }
    }

    /// <summary>
    /// Parses double value accordingly to format string.
    /// </summary>
    /// <param name="value">Value to parse.</param>
    /// <returns>Parsed value.</returns>
    protected string ParseDouble( IDoubleValue value )
    {
      double dValue = value.DoubleValue;
      FormatImpl format = InnerNumberFormat;

      switch( format.GetFormatType( dValue ) )
      {
          case ExcelFormatType.DateTime:
              {
                  if (dValue < DEF_MAX_DOUBLE)
                      return DateTime.ToString();
                  else
                      return dValue.ToString();
              }

        default:
          return dValue.ToString();

      }
    }

    /// <summary>
    /// Parses BlankRecord.
    /// </summary>
    /// <param name="blank">BlankRecord to parse.</param>
    /// <returns>Blank string.</returns>
    [ CLSCompliant( false ) ]
    protected string ParseBlank( BlankRecord blank )
    {
      return string.Empty;
    }

    /// <summary>
    /// Reparses formula record.
    /// </summary>
    /// <param name="formula">Record to reparse.</param>
    [ CLSCompliant( false ) ]
    protected void ReParseFormula( FormulaRecord formula )
    {
      throw new NotImplementedException();
//      try
//      {
//        if( formula.ParsedExpression[ 0 ].TokenCode == FormulaToken.tExp )
//        {
//          ControlPtg control = ( ControlPtg ) formula.ParsedExpression[ 0 ];
//          int index = RangeImpl.GetCellIndex( control.Column + 1, control.Row + 1);
//
//          if( m_worksheet.SharedFormula.Contains( index ) )
//          {
//            ISharedFormula shared = ( ISharedFormula ) m_worksheet.SharedFormula[ index ];
//
//            string strFormula = m_book.FormulaUtil.ParseSharedFormula( shared,
//              formula.Row, formula.Column );
//
//            Formula = strFormula;
//            FormulaNumberValue = formula.Value;
//          }
//          else if( !m_worksheet.ArrayFormula.Contains( index ) )
//          {
//            throw new ArgumentException( "Cannot find corresponding SharedFormulaRecord "
//              + m_worksheet.Name + "!" + control.Row + ":" + control.Column );
//          }
//        }
//        else
//        {
//          AddRemoveEventListenersForNameX( formula.ParsedExpression, -1, -1, true );
//          Record = formula;
//        }
//      }
//      catch( ParseException )
//      {
//        if( m_book.Loading ) m_book.AddForReparse( this );
//        else throw ;
//      }
    }
    /// <summary>
    /// Parses FormulaRecord.
    /// </summary>
    /// <param name="formula">FormulaRecord to parse.</param>
    /// <param name="data">List with Biff records.</param>
    /// <param name="pos">Position of formula in the list.</param>
    [ CLSCompliant( false ) ]
    protected void ParseFormula( FormulaRecord formula, IList data, ref int pos )
    {
//      int iCount = data.Count;
//
//      if( iCount > pos + 1 && data[ pos + 1 ] is StringRecord )
//      {
//        FormulaStringValue = ( data[ pos + 1 ] as StringRecord ).Value;
//      }
//      else if( iCount > pos + 2
//        && ( data[ pos + 1 ] is ArrayRecord || data[ pos + 1 ] is SharedFormulaRecord )
//        && data[ pos + 2 ] is StringRecord )
//      {
//        FormulaStringValue = ( data[ pos + 2 ] as StringRecord ).Value;
//      }
//
//      try
//      {
//        if( formula.ParsedExpression[ 0 ].TokenCode == FormulaToken.tExp )
//        {
//          ControlPtg control = ( ControlPtg ) formula.ParsedExpression[ 0 ];
//          int index = RangeImpl.GetCellIndex( control.ColumnIndex + 1, control.RowIndex + 1);
//
//          if( m_book.Loading )
//          {
//            m_book.AddForReparse( this );
//            return;
//          }
//          else if( m_worksheet.CellRecords.GetArrayRecord( control.RowIndex + 1, control.ColumnIndex + 1 ) == null )
//          {
//            throw new ArgumentException( "Cannot find corresponding SharedFormulaRecord "
//              + m_worksheet.QuotedName + "!" + control.RowIndex + ":" + control.ColumnIndex );
//          }
//        }
//        else
//        {
//          SetFormula( formula );
//        }
//      }
//      catch( ParseException )
//      {
//        if( m_book.Loading ) m_book.AddForReparse( this );
//        else throw ;
//      }
    }

    /// <summary>
    /// Parses BoolErrRecord.
    /// </summary>
    /// <param name="error">BoolErrRecord to parse.</param>
    /// <returns>Extracted BoolError record.</returns>
    [ CLSCompliant( false ) ]
    public static string ParseBoolError( BoolErrRecord error )
    {
      if( error.IsErrorCode )
      {
        if( FormulaUtil.ErrorCodeToName.ContainsKey( error.BoolOrError ) )
        {
          return FormulaUtil.ErrorCodeToName[ error.BoolOrError ];
        }

        return ErrorPtg.DEF_ERROR_NAME;
      }
      else
      {
        return ( error.BoolOrError == 1 ).ToString().ToUpper();
      }
    }

    /// <summary>
    /// Parses RStringRecord.
    /// </summary>
    /// <param name="rstring">RStringRecord to parse.</param>
    /// <returns>Blank string.</returns>
    [ CLSCompliant( false ) ]
    protected string ParseRString( RStringRecord rstring )
    {
      // TODO: Change parse method.
      return string.Empty;
    }

    /// <summary>
    /// Adds listener for the NameXIndexChanged event
    /// if there is NameXPtg in the formula.
    /// </summary>
    /// <param name="parsedFormula">Parsed formula.</param>
    /// <param name="iBookIndex">Workbook index.</param>
    /// <param name="iNameIndex">Name index.</param>
    /// <param name="bAdd">Indicates whether event handler should be added.</param>
    private void AddRemoveEventListenersForNameX( Ptg[] parsedFormula, int iBookIndex,
      int iNameIndex, bool bAdd )
    {
      if( parsedFormula == null )
        throw new ArgumentNullException( "parsedFormula" );

      NameImpl.NameIndexChangedEventHandler handler = new NameImpl.
        NameIndexChangedEventHandler( OnNameXIndexChanged );

      AttachDetachNameIndexChangedEvent( m_book, handler,
        parsedFormula, iBookIndex, iNameIndex, bAdd );
    }
    /// <summary>
    /// Attaches handler to NameIndexChanged event.
    /// </summary>
    /// <param name="book">Workbook with name.</param>
    /// <param name="handler">Event handler.</param>
    /// <param name="parsedFormula">Parsed formula.</param>
    /// <param name="iBookIndex">Workbook index.</param>
    /// <param name="iNewIndex">New index.</param>
    /// <param name="bAdd">Indicates whether event handler should be added.</param>
    public static void AttachDetachNameIndexChangedEvent( WorkbookImpl book,
      NameImpl.NameIndexChangedEventHandler handler, Ptg[] parsedFormula,
      int iBookIndex, int iNewIndex, bool bAdd )
    {
      try
      {
        NameXPtg namex;
        NamePtg name;
        Dictionary<long, object> m_hashIndecies = new Dictionary<long, object>();
        int iReferenceIndex;

        for( int i = 0, len = parsedFormula.Length; i < len; i++ )
        {
          int index = -1;
          iReferenceIndex = -1;

          if( FormulaUtil.IndexOf( FormulaUtil.NameXCodes, parsedFormula[ i ].TokenCode ) != -1 )
          {
            namex = ( NameXPtg ) parsedFormula[ i ];
            index = namex.NameIndex;
            iReferenceIndex = namex.RefIndex;
            AttachDetachExternNameEvent( book, namex, iBookIndex, iNewIndex,
              handler, m_hashIndecies, bAdd );
          }
          else if( FormulaUtil.IndexOf( FormulaUtil.NameCodes, parsedFormula[ i ].TokenCode ) != -1 )
          {
            name = ( NamePtg ) parsedFormula[ i ];
            index = name.ExternNameIndex;
            AttachDetachLocalNameEvent( book, name, iBookIndex, iNewIndex, handler,
              m_hashIndecies, bAdd );
          }
        }
      }
      catch( Exception ex )
      {
        if( book.Loading )
        {
          throw new ParseException( "Parse exception", ex );
        }
        else
        {
          throw;
        }
      }
    }
    /// <summary>
    /// Attaches index changed event handler to extern name collection.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="namex">NamexPtg that identifies named range.</param>
    /// <param name="iBookIndex">Workbook index.</param>
    /// <param name="iNewIndex">NamedRange index to attach to.</param>
    /// <param name="handler">Event handler.</param>
    /// <param name="indexes">Dictionary with indexes of named range that already have attached event.</param>
    /// <param name="bAdd">Indicates whether event handler should be added.</param>
    private static void AttachDetachExternNameEvent( WorkbookImpl book,
      NameXPtg namex, int iBookIndex, int iNewIndex,
      NameImpl.NameIndexChangedEventHandler handler, Dictionary<long, object> indexes, bool bAdd )
    {
      if( namex == null )
        throw new ArgumentNullException( "namex" );

      if( handler == null )
        throw new ArgumentNullException( "handler" );

      int iNameXBookIndex = book.GetBookIndex( namex.RefIndex );
      bool bLocal = ( namex.RefIndex < 0 ) ? true : book.IsLocalReference( namex.RefIndex );

      long index = GetIndex( bLocal ? -1 : iNameXBookIndex, namex.NameIndex );

      if( indexes.ContainsKey( index ) )
        return;

      bool bAttachAnyway = ( iBookIndex == -1 && iNewIndex == -1 );

      if( bLocal && ( namex.NameIndex == iNewIndex || bAttachAnyway ) )
      {
        ( ( NameImpl )book.Names[ namex.NameIndex - 1 ] ).NameIndexChanged += handler;
        indexes.Add( index, null );
      }
      else if( !bLocal && ( ( iBookIndex == iNameXBookIndex && iBookIndex != -1
        && namex.NameIndex == iNewIndex ) || bAttachAnyway ) )
      {
        ExternWorkbookImpl externBook = book.ExternWorkbooks[ iNameXBookIndex ];
        ExternNamesCollection names = externBook.ExternNames;
        ExternNameImpl name = names[ namex.NameIndex - 1 ];

        if( bAdd )
        {
          name.NameIndexChanged += handler;
        }
        else
        {
          name.NameIndexChanged -= handler;
        }

        indexes.Add( index, null );
      }
    }

    /// <summary>
    /// Attaches index changed event handler to local name collection.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="name">NamePtg that identifies named range.</param>
    /// <param name="iBookIndex">Workbook index.</param>
    /// <param name="iNewIndex">New name index.</param>
    /// <param name="handler">Event handler</param>
    /// <param name="indexes">Dictionary with indexes of named range that already have attached event.</param>
    /// <param name="bAdd">Indicates whether event handler should be added.</param>
    private static void AttachDetachLocalNameEvent( WorkbookImpl book, NamePtg name,
      int iBookIndex, int iNewIndex, NameImpl.NameIndexChangedEventHandler handler,
      Dictionary<long, object> indexes, bool bAdd )
    {
      if( name == null )
        throw new ArgumentNullException( "namex" );

      if( handler == null )
        throw new ArgumentNullException( "handler" );

      long index = GetIndex( -1, name.ExternNameIndex );
      bool bAttachAnyway = ( iBookIndex == -1 && iNewIndex == -1 );

      if( indexes.ContainsKey( index ) )
        return;

      if( iBookIndex == -1 && name.ExternNameIndex == iNewIndex || bAttachAnyway )
      {
        NameImpl curName = ( NameImpl )book.Names[ name.ExternNameIndex - 1 ];

        if( bAdd )
        {
          curName.NameIndexChanged += handler;
        }
        else
        {
          curName.NameIndexChanged -= handler;
        }

        indexes.Add( index, null );
      }
    }

    /// <summary>
    /// Combines book index and name index into one index.
    /// </summary>
    /// <param name="iBookIndex">Book index.</param>
    /// <param name="iNameIndex">Name index.</param>
    /// <returns>Combined index.</returns>
    private static long GetIndex( int iBookIndex, int iNameIndex )
    {
      return iBookIndex << 32 + iNameIndex;
    }
    /// <summary>
    /// This is handler of the NameIndexChanged event
    /// of the formula contained by the range
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    private void OnNameXIndexChanged( object sender, NameIndexChangedEventArgs e )
    {
      ( ( INameIndexChangedEventProvider ) sender).NameIndexChanged -= new 
        NameImpl.NameIndexChangedEventHandler( OnNameXIndexChanged );

      if( sender is NameImpl )
      {
        LocalIndexChanged( ( NameImpl )sender, e );
      }
      else if( sender is ExternNameImpl )
      {
        ExternIndexChanged( ( ExternNameImpl )sender, e );
      }
    }
    /// <summary>
    /// Handler for named range index changed event for local named ranges.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void LocalIndexChanged( NameImpl sender, NameIndexChangedEventArgs e )
    {
      if( CellType == TCellType.Formula )
      {
        FormulaRecord formula = ( FormulaRecord )Record;
        Ptg[] arrExpression = formula.ParsedExpression;

        for( int i = 0, len = arrExpression.Length; i < len; i++ )
        {
          Ptg ptg = arrExpression[ i ];

          if( ptg is NameXPtg )
          {
            NameXPtg nameX = ptg as NameXPtg;
            formula.IsFillFromExpression = true;
            
            if( nameX.RefIndex == 0xFFFF || m_book.IsLocalReference( nameX.RefIndex )
              && e.OldIndex == nameX.NameIndex - 1 )
            {
              nameX.NameIndex = ( ushort )( e.NewIndex + 1 );
            }
          }

          if( ptg is NamePtg )
          {
            NamePtg name = ptg as NamePtg;

            formula.IsFillFromExpression = true;
            
            if( e.OldIndex == name.ExternNameIndex - 1 )
              name.ExternNameIndex = ( ushort )( e.NewIndex + 1 );
          }
        }

        //AddRemoveEventListenersForNameX( formula.ParsedExpression, -1, e.OldIndex + 1, false );
        //AddRemoveEventListenersForNameX( formula.ParsedExpression, -1, e.NewIndex + 1, true );
      }
    }
    /// <summary>
    /// Handler for named range index changed event for external named ranges.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void ExternIndexChanged( ExternNameImpl sender, NameIndexChangedEventArgs e )
    {
      if( this.CellType == TCellType.Formula )
      {
        FormulaRecord formula = ( FormulaRecord )Record;
        Ptg[] arrExpression = formula.ParsedExpression;

        for( int i = 0, len = arrExpression.Length; i < len; i++ )
        {
          Ptg ptg = arrExpression[ i ];

          if( ptg is NameXPtg )
          {
            NameXPtg nameX = ptg as NameXPtg;
            formula.IsFillFromExpression = true;
            
            if( nameX.RefIndex == sender.BookIndex && e.OldIndex == nameX.NameIndex - 1 )
            {
              nameX.NameIndex = ( ushort )( e.NewIndex + 1 );
            }
          }
        }

        //AddRemoveEventListenersForNameX( formula.ParsedExpression, sender.BookIndex, e.OldIndex + 1, false );
        //AddRemoveEventListenersForNameX( formula.ParsedExpression, sender.BookIndex, e.NewIndex + 1, true );
      }
    }
    #endregion

    #region IRange methods
    /// <summary>
    /// Activates a single cell, which must be inside the current selection. 
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <returns>Range representing single active cell.</returns>
    public IRange Activate()
    {
      CheckDisposed();

      if( this.IsSingleCell )
      {
        m_worksheet.SetActiveCell( this );
        return this;
      }

      return null;
    }

    /// <summary>
    /// Activates a single cell, scroll to it and activates the corresponding sheet.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
    /// <returns></returns>
    public virtual IRange Activate( bool scroll )
    {
      this.Activate();

      if( scroll )
      {
        m_worksheet.TopLeftCell = this;
      }

      m_worksheet.Activate();
      return this;
    }

    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether grouping should
    /// be performed by rows or by columns. 
    /// </param>
    /// <param name="bCollapsed">Indicates whether group should be collapsed.</param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy, bool bCollapsed )
    {
      CheckDisposed();
      return ToggleGroup( groupBy, true, bCollapsed );
    }
    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether grouping should be performed by rows or by columns. 
    /// </param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy )
    {
      CheckDisposed();
      return Group( groupBy, false );
    }
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    public void Merge()
    {
      Merge( false );
    }
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    /// <param name="clearCells">Indicates whether to clear unnecessary cells.</param>
    public void Merge( bool clearCells )
    {
      CheckDisposed();

      if( IsSingleCell ) return;

      m_worksheet.MergeCells.AddMerge( this, ExcelMergeOperation.Delete );

        m_worksheet.ClearExceptFirstCell( this , clearCells );

      if (!m_book.IsLoaded)
      {
          bool hasMerged = false;
          int firstColumn = Column;
          int firstRow = Row;
          int lastRow = LastRow;
          int lastColumn = LastColumn;

          if (firstRow == lastRow)
          {
              for (int i = firstColumn, iLastColumn = LastColumn; i <= iLastColumn; i++)
              {
                  if (this[firstRow, i] != null)
                  {
                      RowStorage storage = WorksheetHelper.GetOrCreateRow( (this.Worksheet) as WorksheetImpl, firstRow-1 , false);
                      if (this[firstRow, i].WrapText && !storage .IsBadFontHeight)

                          m_worksheet.AutofitRow(firstRow, i, iLastColumn, true);
                  }
              }

          }
      }
      
    }
    internal void MergeWithoutCheck()
    {
        if (IsSingleCell) return;
        m_worksheet.MergeCells.AddMerge(this, ExcelMergeOperation.Leave);
    }

    /// <summary>
    /// Ungroup the range
    /// </summary>
    /// <param name="groupBy">GroupBy rows or columns.</param>
    /// <returns>Current range after ungrouping.</returns>
    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      CheckDisposed();
      return ToggleGroup( groupBy, false, false );
    }
    /// <summary>
    /// Separates a merged area into individual cells.
    /// </summary>
    public void   UnMerge()
    {
      CheckDisposed();

      Rectangle range = Rectangle.FromLTRB( FirstColumn - 1, FirstRow - 1, LastColumn - 1, LastRow - 1 );
      m_worksheet.MergeCells.DeleteMerge( range );
    }

    /// <summary>
    /// Freezes pane at the current range if it is single cell range.
    /// </summary>
    public void   FreezePanes()
    {
      CheckDisposed();

      if( IsSingleCell )
        m_worksheet.SetPaneCell( this );
      else
      {
          RangeImpl freezeRange = this.Worksheet[this.FirstRow, this.FirstColumn] as RangeImpl;
          m_worksheet.SetPaneCell(freezeRange);
      }
    }
    /// <summary>
    /// Clear the contents of the Range.
    /// </summary>
    public void   Clear()
    {
      CheckDisposed();
      Clear( false );
    }
    /// <summary>
    /// Clear the contents of the Range with formatting.
    /// </summary>
    /// <param name="isClearFormat">True if formatting should also be cleared.</param>
    public void   Clear( bool isClearFormat )
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        BlankCell();
      }
      else
      {
        MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );
        int lastRow = LastRow;
        int lastColumn = LastColumn;

        if (lastRow == m_book.MaxRowCount)
        {
            lastRow = m_worksheet.UsedRange.LastRow;
        }
        if (lastColumn == m_book.MaxColumnCount)
        {
            lastColumn = m_worksheet.UsedRange.LastColumn;
        }

        for (int iRow = FirstRow; iRow <= lastRow; iRow++)
        {
            for (int iCol = FirstColumn; iCol <= lastColumn; iCol++)
            {
                range.ResetRowColumn(iRow, iCol);
                range.BlankCell();
            }
        }
      }

      if( isClearFormat ) CellStyleName = "Normal";
    }
    /// <summary>
    /// Clears range.
    /// </summary>
    public void FullClear()
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        Clear( true );

        CommentsCollection comments = m_worksheet.InnerComments;
        ICommentShape comment = comments[ FirstRow, FirstColumn ];

        if( comment != null )
        {
          comments.Remove( comment );
        }
      }
      else
      {
        MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

        for( int iRow = FirstRow; iRow <= LastRow; iRow++ )
        {
          for( int iCol = FirstColumn; iCol <= LastColumn; iCol++ )
          {
            range.ResetRowColumn( iRow, iCol );
            range.FullClear();
          }
        }
      }
    }
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left
    /// without formula or merged ranges update.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    public void   Clear( ExcelMoveDirection direction )
    {
      CheckDisposed();
      Clear( direction, ExcelCopyRangeOptions.None );
    }
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    /// <param name="options">Cells shifting options.</param>
    public void   Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      CheckDisposed();

      switch( direction )
      {
        case ExcelMoveDirection.None:
          Clear( true );
          break;

        case ExcelMoveDirection.MoveUp:
          Clear( true );
          MoveCellsUp( options );
          break;

        case ExcelMoveDirection.MoveLeft:
          Clear( true );
          MoveCellsLeft( options );
          break;
      }
    }
    /// <summary>
    /// clears the cell formats,comments,contents based on clear option.
    /// </summary>
    /// <param name="option"></param>
    internal void ClearOption(ExcelClearOptions option)
    {
        CheckDisposed();

        switch(option)
        {
            case ExcelClearOptions.ClearFormat:
                {
                    CellStyleName = "Normal";
                    int firstRow = 0;
                    int lastRow = 0;
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        if (range.ConditionalFormats != null)
                            range.ClearConditionalFormats();
                        if (firstRow == 0 && !range.IsBlank)
                            firstRow = range.Row;
                        else if (!range.IsBlank)
                            lastRow = range.Row;
                    }
                    if (this.IsBlankorHasStyle)
                    {
                        if (this.LastRow == m_worksheet.LastRow)
                            m_worksheet.LastRow = this.Row - 1;
                        else if (this.LastRow <= m_worksheet.LastRow)
                        {
                            if (this.Row == m_worksheet.FirstRow)
                                m_worksheet.FirstRow = this.LastRow + 1;
                        }
                    }
                    else
                    {
                        if( firstRow != this.FirstRow && lastRow != this.LastRow)
                        {
                            m_worksheet.FirstRow = firstRow;
                            m_worksheet.LastRow = lastRow;
                        }
                        else if (lastRow == this.LastRow)
                            m_worksheet.FirstRow = firstRow;
                        else
                            m_worksheet.LastRow = lastRow;
                    }
                    break;
                }

            case ExcelClearOptions.ClearContent:
                {
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        range.Value = null;
                    }
                    break;
                }

            case ExcelClearOptions.ClearComment:
                {
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        range.Comments();
                    }
                    break;
                }

            case ExcelClearOptions.ClearConditionalFormats:
                {
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        if (range.ConditionalFormats != null)
                            range.ClearConditionalFormats();
                    }
                    break;
                }
                
            case ExcelClearOptions.ClearDataValidations:
                {
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        if (range.HasDataValidation)
                            range.ClearDataValidations();
                    }
                    break;
                }

            default:
                {
                    List<IRange> list = CellsList;
                    for (int i = 0, len = list.Count; i < len; i++)
                    {
                        RangeImpl range = (RangeImpl)list[i];
                        range.Value = null;
                        range.Comments();
                        if (range.ConditionalFormats != null)
                            range.ClearConditionalFormats();
                        if (range.HasDataValidation)
                            range.ClearDataValidations();
                    }
                    CellStyleName = "Normal";
                    if ((this.Row == m_worksheet.FirstRow) && (this.LastRow == m_worksheet.LastRow))
                    {
                        m_worksheet.Clear();
                    }
                    else if (this.LastRow == m_worksheet.LastRow)
                        m_worksheet.LastRow = this.Row - 1;
                    else if (this.LastRow <= m_worksheet.LastRow)
                    {
                        if (this.Row == m_worksheet.FirstRow)
                            m_worksheet.FirstRow = this.LastRow + 1;
                    }
                    break;
                }            
        }
    }
    /// <summary>
    /// Clears the cell based on clear options.
    /// </summary>
    /// <param name="option"></param>
    public void Clear(ExcelClearOptions option)
    {
        ClearOption(option);
    }
    /// <summary>
    /// clears the comments of the cell.
    /// </summary>
    internal void Comments()
    {
        if (IsSingleCell)
        {
            CommentsCollection comments = m_worksheet.InnerComments;
            ICommentShape comment = comments[FirstRow, FirstColumn];
            if (comment != null)
            {
                comments.Remove(comment);
            }
        }
    }
    /// <summary>
    /// Moves the cells to the specified Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination Range.</param>
    public void   MoveTo( IRange destination )
    {
      CheckDisposed();
      MoveTo( destination, ExcelCopyRangeOptions.All );
    }

    /// <summary>
    /// Moves the cells to the specified Range.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Specifies the options to update formulas and merged ranges during copy range.</param>
    public void   MoveTo( IRange destination, ExcelCopyRangeOptions options )
    {
      CheckDisposed();

      if( this == destination ) return;
      
      m_worksheet.MoveRange( destination, this, options, false );
    }
    /// <summary>
    /// Copies this range into another location and updates formulas.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <returns>Destination range.</returns>
    public IRange CopyTo( IRange destination )
    {
      CheckDisposed();

      if( this == destination ) return destination;
      
      return m_worksheet.CopyRange( destination, this, ExcelCopyRangeOptions.All );
    }
    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>Destination range.</returns>
    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      CheckDisposed();

      if( this == destination ) return destination;
      
      return m_worksheet.CopyRange( destination, this, options );
    }
    /// <summary>
    /// Returns intersection of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range with which to intersect.</param>
    /// <returns>Range intersection. If there is no intersection, NULL is returned.</returns>
    public IRange IntersectWith( IRange range )
    {
      CheckDisposed();
      return m_worksheet.IntersectRanges( this, range );
    }
    /// <summary>
    /// Returns merge of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range to merge with.</param>
    /// <returns>Merged ranges or null if wasn't able to merge ranges.</returns>
    public IRange MergeWith( IRange range )
    {
      CheckDisposed();
      return m_worksheet.MergeRanges( this, range );
    }
    /// <summary>
    /// Adds new comment or returns an old one if was present.
    /// </summary>
    /// <returns>Newly created comment or old one if was present.</returns>
    public ICommentShape AddComment()
    {
      return AddComment( true );
    }
    /// <summary>
    /// Adds new comment or returns an old one if was present.
    /// </summary>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    /// <returns>Newly created comment or old one if was present.</returns>
    public ICommentShape AddComment( bool bIsParseOptions )
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        CommentsCollection comments = m_worksheet.InnerComments;
        ICommentShape comment = comments[ m_iTopRow, m_iLeftColumn ];

        if( comment != null ) return comment;

        return comments.AddComment( m_iTopRow, m_iLeftColumn, bIsParseOptions );
      }

      return null;
    }
    /// <summary>
    /// Measures size of the string.
    /// </summary>
    /// <param name="strMeasure">String to measure.</param>
    /// <returns>Size of the string.</returns>
    public SizeF MeasureString( string strMeasure )
    {
      CheckDisposed();
      return ( CellStyle.Font as FontWrapper ).Wrapped.MeasureString( strMeasure );
    }

    /// <summary>
    /// Autofits rows.
    /// </summary>
    public void AutofitRows()
    {
      CheckDisposed();
      int firstColumn = Column;
      int lastColumn = LastColumn;

      for( int i = FirstRow, iLastRow = LastRow; i <= iLastRow; i++ )
      {
        m_worksheet.AutofitRow( i, firstColumn, lastColumn, true );
      }
    }

    /// <summary>
    /// Autofit columns.
    /// </summary>
    public void AutofitColumns()
    {
      CheckDisposed();
      int firstRow = Row;
      int lastRow = LastRow;
      AutoFitToColumn(FirstColumn, LastColumn);

    }
    /// <summary>
    /// Auto fit the column.
    /// </summary>
    /// <param name="firstColumn">The first column.</param>
    /// <param name="lastColumn">The last column.</param>
    public void AutoFitToColumn(int firstColumn, int lastColumn)
    {
        int firstRow = FirstRow;
        int lastRow = LastRow;
        if (firstRow == 0 || lastRow == 0 || firstRow > lastRow)
            return;

        if (firstColumn < 1 || firstColumn > m_book.MaxColumnCount)
            throw new ArgumentOutOfRangeException("firstColumn");

        if (lastColumn < 1 || lastColumn > m_book.MaxColumnCount)
            throw new ArgumentOutOfRangeException("lastColumn");

        using (AutoFitManager autoFitManager = new AutoFitManager(firstRow, firstColumn, lastRow, lastColumn, this))
        {
            autoFitManager.MeasureToFitColumn();
        }
    }
    /// <summary>
    /// Determines whether the range is Merged or not.
    /// </summary>
    /// <param name="mergedCells">The merged cells.</param>
    /// <param name="iRow">Represents the row.</param>
    /// <param name="iColumn">Represents the column</param>
    /// <param name="isRow">whether method called by AutoFitColumn or AutoFitRow</param>
    /// <param name="num4">Difference of Merged Range.</param>
    /// <returns>
    ///   true, if the Range is merged, else false.
    /// </returns>
    internal static bool IsMergedCell(MergeCellsImpl mergedCells, int iRow, int iColumn, bool isRow, ref int num4)
    {
        if (mergedCells != null)
        {
            Rectangle rect = Rectangle.FromLTRB(iColumn - 1, iRow - 1, iColumn - 1, iRow - 1);

            MergeRegion region = mergedCells[rect];
            
            if (region != null)
            {
                if ((region.RowFrom <= iRow - 1 && region.RowTo >= iRow - 1)
                  && (region.ColumnFrom <= iColumn - 1 && region.ColumnTo >= iColumn - 1))
                {
                    if (isRow)
                    {
                        if (region.RowFrom == region.RowTo)
                            num4 = region.ColumnTo - region.ColumnFrom;
                    }
                    else if (region.ColumnFrom == region.ColumnTo)
                    {
                        num4 = region.RowTo - region.RowFrom;
                    }
                    return true;
                }
            }
        }
        return false;
    }
   
    /// <summary>
    /// Gets the display text.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="formatImpl">The format impl.</param>
    /// <returns></returns>
    internal string GetDisplayText(int row, int column, FormatImpl formatImpl)
    {
        TRangeValueType valType = m_worksheet.GetCellType(row, column, false);
        object value = null;
        string displayText = null;
        switch (valType)
        {
            case TRangeValueType.Blank:
                displayText = string.Empty;
                break;
            case TRangeValueType.String:
                string strValue = m_worksheet.GetText(row, column);
                displayText = formatImpl.ApplyFormat(strValue);
                break;

            case TRangeValueType.Number:
                double dValue = m_worksheet.GetNumber(row, column);

                if (!Double.IsNaN(dValue))//value != Double.NaN )
                {
                    if (dValue == 0 && !m_worksheet.WindowTwo.IsDisplayZeros)
                    {
                        ExcelFormatType formatType1 = formatImpl.GetFormatType(0);

                        if (formatType1 == ExcelFormatType.Number || formatType1 == ExcelFormatType.General)
                            displayText = GetDisplayString();
                    }

                    if (displayText == null)
                        displayText = (double.IsNaN(dValue)) ? dValue.ToString() : formatImpl.ApplyFormat(dValue);
                }
                break;
            case TRangeValueType.Boolean:
                bool bValue = m_worksheet.GetBoolean(row, column);
                displayText = bValue.ToString();
                break;

            case TRangeValueType.Formula:
                bool isString = false;
                TRangeValueType rangeType = m_worksheet.GetCellType(row, column, true);
                if ((rangeType & TRangeValueType.Boolean) == TRangeValueType.Boolean)
                {
                    return m_worksheet.GetFormulaBoolValue(row, column).ToString();
                }
                switch (this.CellType)
                {
                    case TCellType.Label:
                    case TCellType.LabelSST:
                    case TCellType.RString:
                        return m_worksheet.GetFormulaStringValue(row, column);
                        break;
                    case TCellType.Number:
                    case TCellType.RK:
                        return m_worksheet.GetFormulaNumberValue(row, column).ToString();
                        break;
                    default:
                        return "";
                }
                break;
            default:
                displayText = m_worksheet[row, column].DisplayText;
                break;
        }
        return displayText;
    }
    
    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValue">Value to replace by.</param>
    public void Replace( string oldValue, string newValue )
    {
      CheckDisposed();

      if( IsSingleCell && this.Text == oldValue )
      {
        Text = newValue;
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValue">Value to replace by.</param>
    public void Replace( string oldValue, double newValue )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        Number = newValue;
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValue">Value to replace by.</param>
    public void Replace( string oldValue, DateTime newValue )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        DateTime = newValue;
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValues">Array to replace by.</param>
    /// <param name="isVertical">Indicates whether to import values vertically or horizontally.</param>
    public void Replace( string oldValue, string[] newValues, bool isVertical )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        m_worksheet.ImportArray( newValues, Row, Column, isVertical );
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValues">Array to replace by.</param>
    /// <param name="isVertical">Indicates whether to import values vertically or horizontally.</param>
    public void Replace( string oldValue, int[] newValues, bool isVertical )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        m_worksheet.ImportArray( newValues, Row, Column, isVertical );
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValues">Array to replace by.</param>
    /// <param name="isVertical">Indicates whether to import values vertically or horizontally.</param>
    public void Replace( string oldValue, double[] newValues, bool isVertical )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        m_worksheet.ImportArray( newValues, Row, Column, isVertical );
      }
    }

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValues">DataTable to replace by.</param>
    /// <param name="isFieldNamesShown">Indicates whether to import field names.</param>
    public void Replace( string oldValue, DataTable newValues, bool isFieldNamesShown )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        m_worksheet.ImportDataTable( newValues, isFieldNamesShown, Row, Column );
      }
    }

    /// <summary>
    /// Replaces oldValue by newValue.
    /// </summary>
    /// <param name="oldValue">Value to compare.</param>
    /// <param name="newValues">DataTable to replace by.</param>
    /// <param name="isFieldNamesShown">Indicates whether to import field names.</param>
    public void Replace( string oldValue, DataColumn newValues, bool isFieldNamesShown )
    {
      CheckDisposed();

      if( IsSingleCell && Text == oldValue )
      {
        m_worksheet.ImportDataColumn( newValues, isFieldNamesShown, Row, Column );
      }
    }
#endif
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
      CheckDisposed();

      if( findValue == null || findValue.Length == 0 ) return null;
            
      bool bIsFormula = ( ( flags & ExcelFindType.Formula ) == ExcelFindType.Formula );
      bool bIsText = ( ( flags & ExcelFindType.Text ) == ExcelFindType.Text );
      bool bIsFormulaStringValue = ( ( flags & ExcelFindType.FormulaStringValue ) == ExcelFindType.FormulaStringValue );
      bool bIsError = ( ( flags & ExcelFindType.Error ) == ExcelFindType.Error );

      if( !( bIsFormula || bIsText || bIsFormulaStringValue || bIsError ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );
      
      if( IsSingleCell )
      {
        if( bIsError && IsError && Error == findValue )
        {
          return this;
        }

        if( bIsFormula && HasFormula && Formula == findValue )
        {
          return this;
        }

        if( bIsFormulaStringValue  && FormulaStringValue != null && FormulaStringValue == findValue )
        {
          return this;
        }

        if( bIsText && HasString && Text == findValue )
        {
          return this;
        }
      }
      else
      {
        IRange[] arrResult = m_worksheet.Find( this, findValue, flags, true );
        return ( arrResult != null )
          ? arrResult[ 0 ]
          : null;
      }

      return null;
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      CheckDisposed();

      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );


      if( IsSingleCell )
      {
        if( bIsNumber && HasNumber && Number == findValue )
        {
          return this;
        }

        if( bIsFormulaValue && HasFormula  && FormulaNumberValue == findValue )
        {
          return this;
        }
      }
      else
      {
        IRange[] arrResult = m_worksheet.Find( this, findValue, flags, true );
        return ( arrResult != null )
          ? arrResult[ 0 ]
          : null;
      }

      return null;
    }

    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        return( IsBoolean && Boolean == findValue )
          ? this
          : null;
      }
      else
      {
        byte bFindValue = ( byte )( ( findValue ) ? 1 : 0 );

        IRange[] arrResult = m_worksheet.Find( this, bFindValue, false, true );

        return ( arrResult != null )
          ? arrResult[ 0 ]
          : null;
      }
    }

    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        if( HasDateTime && DateTime == findValue )
        {
          return this;
        }
      }
      else
      {
        double dFindValue = findValue.ToOADate();

        return FindFirst( dFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
      }

      return null;
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      CheckDisposed();

      if( IsSingleCell )
      {
        if( HasDateTime && TimeSpan == findValue )
        {
          return this;
        }
      }
      else
      {
        double dFindValue = findValue.TotalDays;

        return FindFirst( dFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
      }

      return null;
  }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
      CheckDisposed();

      if( findValue == null || findValue.Length == 0 ) return null;
      
      bool bIsFormula = ( ( flags & ExcelFindType.Formula ) == ExcelFindType.Formula );
      bool bIsText = ( ( flags & ExcelFindType.Text ) == ExcelFindType.Text );
      bool bIsFormulaStringValue = ( ( flags & ExcelFindType.FormulaStringValue ) == ExcelFindType.FormulaStringValue );
      bool bIsError = ( ( flags & ExcelFindType.Error ) == ExcelFindType.Error );

      if( !( bIsFormula || bIsText || bIsFormulaStringValue || bIsError ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );
      
      if( IsSingleCell )
      {
        return( ( bIsError && IsError && Error == findValue )
          || ( bIsFormula && HasFormula && Formula == findValue )
          || ( bIsFormulaStringValue && FormulaStringValue != null && FormulaStringValue == findValue )
          || ( bIsText && HasString && Text == findValue ) )
          ? new IRange[] { this }
          : null;
      }
      else
      {
        return m_worksheet.Find( this, findValue, flags, false );
      }
    }
    /// <summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      CheckDisposed();

      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      if ( IsSingleCell )
      {
        return( ( bIsNumber && HasNumber && Number == findValue )
          || ( bIsFormulaValue && HasFormula  && FormulaNumberValue == findValue ) )
          ? new IRange[]{ this }
          : null;
      }
      else
      {
        return m_worksheet.Find( this, findValue, flags, false );
      }
    }

    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( bool findValue )
    {
      CheckDisposed();

      if ( IsSingleCell )
      {
        return( IsBoolean && Boolean == findValue )
          ? new IRange[]{ this }
          : null;
      }
      else
      {
        byte bFindValue = ( byte )( ( findValue ) ? 1 : 0 );
        return m_worksheet.Find( this, bFindValue, false, false );
      }
    }

    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      CheckDisposed();

      List<IRange> resultCells = new List<IRange>();
 
      if ( IsSingleCell )
      {
        if( HasDateTime && DateTime == findValue )
        {
          resultCells.Add( this );
        }
      }
      else
      {
        double iFindValue = findValue.ToOADate();
        return FindAll( iFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
      }

      return ( resultCells.Count != 0 ) ?
        resultCells.ToArray() :
        null;
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      CheckDisposed();
      List<IRange> resultCells = new List<IRange>();
 
      if ( IsSingleCell )
      {
        if( HasDateTime && TimeSpan == findValue )
        {
          resultCells.Add( this );
        }
      }
      else
      {
        double iFindValue = findValue.TotalDays;

        return FindAll( iFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
      }

      return ( resultCells.Count != 0 ) ?
        resultCells.ToArray() :
        null;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies range to the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      ClipboardProvider provider = AppImplementation.CreateClipboardProvider( m_worksheet );
      provider.SetClipboard( this );
    }
#endif
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    public void BorderAround()
    {
      BorderAround( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderAround( ExcelLineStyle borderLine )
    {
      BorderAround( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderAround( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = m_book.GetNearestColor( borderColor );

      BorderAround( borderLine, color );
    }
    /// <summary>
    /// Sets around border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderAround( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      if( IsSingleCell )
      {
        SetBorderToSingleCell( ExcelBordersIndex.EdgeLeft, borderLine, borderColor );
        SetBorderToSingleCell( ExcelBordersIndex.EdgeRight, borderLine, borderColor );
        SetBorderToSingleCell( ExcelBordersIndex.EdgeTop, borderLine, borderColor );
        SetBorderToSingleCell( ExcelBordersIndex.EdgeBottom, borderLine, borderColor );

        return;
      }

      int iFirstCol = Column;
      int iLastCol = LastColumn;
      int iFirstRow = Row;
      int iLastRow = LastRow;
      MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

      for( int i = iFirstCol; i <= iLastCol; i++ )
      {
        range.ResetRowColumn( iFirstRow, i );
        range.SetBorderToSingleCell( ExcelBordersIndex.EdgeTop, borderLine, borderColor );

        range.ResetRowColumn( iLastRow, i );
        range.SetBorderToSingleCell( ExcelBordersIndex.EdgeBottom, borderLine, borderColor );
      }

      for( int j = iFirstRow; j <= iLastRow; j++ )
      {
        range.ResetRowColumn( j, iFirstCol );
        range.SetBorderToSingleCell( ExcelBordersIndex.EdgeLeft, borderLine, borderColor );

        range.ResetRowColumn( j, iLastCol );
        range.SetBorderToSingleCell( ExcelBordersIndex.EdgeRight, borderLine, borderColor );
      }
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    public void BorderInside()
    {
      BorderInside( ExcelLineStyle.Thin );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    public void BorderInside( ExcelLineStyle borderLine )
    {
      BorderInside( borderLine, ExcelKnownColors.Black );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color.</param>
    public void BorderInside( ExcelLineStyle borderLine, Color borderColor )
    {
      ExcelKnownColors color = m_book.GetNearestColor( borderColor );

      BorderInside( borderLine, color );
    }
    /// <summary>
    /// Sets inside border for current range.
    /// </summary>
    /// <param name="borderLine">Represents border line.</param>
    /// <param name="borderColor">Represents border color as ExcelKnownColors.</param>
    public void BorderInside( ExcelLineStyle borderLine, ExcelKnownColors borderColor )
    {
      if( IsSingleCell )
        throw new NotSupportedException( "This method doesn't support for single cell." );

      int iFirstCol = Column;
      int iLastCol = LastColumn;
      int iFirstRow = Row;
      int iLastRow = LastRow;
      MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

      for( int i = iFirstCol; i <= iLastCol; i++ )
      {
        for( int j = iFirstRow; j <= iLastRow; j++ )
        {
          if( i != LastColumn )
          {
            range.ResetRowColumn( j, i );
            range.SetBorderToSingleCell( ExcelBordersIndex.EdgeRight, borderLine, borderColor );
          }

          if( j != LastRow )
          {
            range.ResetRowColumn( j, i );
            range.SetBorderToSingleCell( ExcelBordersIndex.EdgeBottom, borderLine, borderColor );
          }
        }
      }
    }
    /// <summary>
    /// Sets none border for current range.
    /// </summary>
    public void BorderNone()
    {
      if( IsSingleCell )
      {
        Borders.LineStyle = ExcelLineStyle.None;

        return;
      }

      for( int i = FirstColumn, iLen = LastColumn; i <= iLen; i++ )
      {
        for( int j = FirstRow, iCount = LastRow; j <= iCount; j++ )
        {
          MigrantRangeImpl range = new MigrantRangeImpl( Application, m_worksheet );

          range.ResetRowColumn( j, i );
          range.Borders.LineStyle = ExcelLineStyle.None;
        }
      }
    }
    /// <summary>
    /// Sets auto format for current range.
    /// </summary>
    /// <param name="format">Represents format to set.</param>
    public void SetAutoFormat( ExcelAutoFormat format )
    {
      SetAutoFormat( format, ExcelAutoFormatOptions.All );
    }
    /// <summary>
    /// Sets auto format for current range.
    /// </summary>
    /// <param name="format">Represents auto format to set.</param>
    /// <param name="options">Represents auto format options.</param>
    public void SetAutoFormat( ExcelAutoFormat format, ExcelAutoFormatOptions options )
    {
      if( IsSingleCell )
        throw new NotSupportedException( "Auto format doesn't suport in single cell." );

      if( options == ExcelAutoFormatOptions.None )
        return;

      bool bIsPatterns = ( options & ExcelAutoFormatOptions.Patterns )
        == ExcelAutoFormatOptions.Patterns;

      bool bIsAlignment = ( options & ExcelAutoFormatOptions.Alignment )
        == ExcelAutoFormatOptions.Alignment;

      bool bIsWidthHeight = ( options & ExcelAutoFormatOptions.Width_Height )
        == ExcelAutoFormatOptions.Width_Height;

      bool bIsNumber = ( options & ExcelAutoFormatOptions.Number )
        == ExcelAutoFormatOptions.Number;

      bool bIsFont = ( options & ExcelAutoFormatOptions.Font )
        == ExcelAutoFormatOptions.Font;

      bool bIsBorder = ( options & ExcelAutoFormatOptions.Border )
        == ExcelAutoFormatOptions.Border;

      if( bIsPatterns )
        SetAutoFormatPatterns( format );

      if( bIsAlignment )
        SetAutoFormatAlignments( format );

      if( bIsWidthHeight )
        SetAutoFormatWidthHeight( format );

      if( bIsNumber )
        SetAutoFormatNumbers( format );

      SetAutoFormatFontBorder( format, bIsFont, bIsBorder );
    }
    /// <summary>
    /// Sets cell value.
    /// </summary>
    /// <param name="value">Value to be set.</param>
    private void SetSingleCellValue2( object value )
    {
        bool preserveFormats = true ;
      if( value != null )
      {
        bool? bStringsPreserved = IsStringsPreserved;

        if( bStringsPreserved == null )
          bStringsPreserved = m_worksheet.IsStringsPreserved;

        if( bStringsPreserved == false )
        {
          if( value is DateTime && (DateTime)value>=DEF_MIN_DATETIME)
          {
            DateTime date = ( DateTime )value;
            this.DateTime = date;
          }
          else if( value is TimeSpan )
          {
            TimeSpan time = ( TimeSpan )value;
            this.TimeSpan = time;
          }
          else if( value is double )
          {
            Number = ( double )value;
          }
          else if( value is int )
          {
              SetNumberAndFormat((int) value,preserveFormats);
              
          }
          else
          {
            Value = value.ToString();
          }
        }
        else
        {
          Value = value.ToString();
        }
      }
      else
      {
        Text = "";
      }
    }
    /// <summary>
    /// Collapses current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      CollapseExpand( groupBy, true, ExpandCollapseFlags.Default );
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      ExpandGroup( groupBy, ExpandCollapseFlags.Default );
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <param name="flags">Additional option flags.</param>
    public void ExpandGroup( ExcelGroupBy groupBy, ExpandCollapseFlags flags )
    {
      CollapseExpand( groupBy, false, flags );
    }
    #endregion

    #region ICombinedRange methods
    /// <summary>
    /// Gets new address of range.
    /// </summary>
    /// <param name="names">Dictionary with Worksheet names.</param>
    /// <param name="strSheetName">String that sets as a worksheet name.</param>
    /// <returns>Returns string with new name.</returns>
    public string GetNewAddress( Dictionary<string, string> names, out string strSheetName )
    {
      strSheetName = m_worksheet.Name;

      if( names == null || !names.ContainsKey( strSheetName ) )
        return Address;

      strSheetName = names[ strSheetName ];

      return "'" + strSheetName.Replace( "'", "''" ) + "'!" + AddressLocal;
    }
    /// <summary>
    /// Clones current IRange.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="hashNewNames">Hash table with new names.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns>Returns clone of current instance.</returns>
    public IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      string strWorkSheetName = m_worksheet.Name;

      if( hashNewNames != null && hashNewNames.ContainsKey( strWorkSheetName ) )
      {
        strWorkSheetName = hashNewNames[ strWorkSheetName ];
      }

      WorksheetImpl sheet = ( WorksheetImpl )book.Worksheets[ strWorkSheetName ];
      IRange cloneRange = null;
      if (sheet != null)
          cloneRange = sheet.Range[FirstRow, FirstColumn, LastRow, LastColumn];
      else
          cloneRange = m_worksheet.Range[FirstRow, FirstColumn, LastRow, LastColumn];

      return cloneRange;
    }
    /// <summary>
    /// Clears conditional formats.
    /// </summary>
    public void ClearConditionalFormats()
    {
      m_worksheet.ConditionalFormats.Remove( GetRectangles() );
    }
    /// <summary>
    /// Clears data validations.
    /// </summary>
    public void ClearDataValidations()
    {
        m_worksheet.DVTable.Remove(GetRectangles());
    }
    /// <summary>
    /// Returns array that contains information about range.
    /// </summary>
    /// <returns>Rectangles that describes range with zero-based coordinates.</returns>
    public Rectangle[] GetRectangles()
    {
      return new Rectangle[]{ Rectangle.FromLTRB( FirstColumn - 1, FirstRow - 1, LastColumn - 1, LastRow - 1 ) };
    }
    /// <summary>
    /// Returns number of rectangles returned by GetRectangles method.
    /// </summary>
    /// <returns>Number of rectangles returned by GetRectangles method.</returns>
    public int GetRectanglesCount()
    {
      return 1;
    }
    /// <summary>
    /// Gets name of the parent worksheet.
    /// </summary>
    public string WorksheetName
    {
      get
      {
        return Worksheet.Name;
      }
    }
    #endregion

    #region Public static methods
    /// <summary>
    /// Update the cell value when calc engine is enabled.
    /// </summary>
    /// <param name="Parent"></param>
    /// <param name="Column"></param>
    /// <param name="Row"></param>
    private static void UpdateCellValue(object Parent , int Column, int Row,bool updateCellVaue)
    {
        if (Parent is IWorksheet && ((IWorksheet)Parent).CalcEngine != null && updateCellVaue )
        {
            string cellRef = Syncfusion.Calculate.RangeInfo.GetAlphaLabel(Column) + Row.ToString();
            ((IWorksheet)Parent).CalcEngine.PullUpdatedValue(cellRef);
        }
    }

    /// <summary>
    /// Gets R1C1 address from cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns R1C1 address.</returns>
    public static string GetR1C1AddresFromCellIndex( long cellIndex )
    {
      int row = GetRowFromCellIndex( cellIndex );
      int column = GetColumnFromCellIndex( cellIndex );

      return GetAddressLocal( row, column, row, column, true );
    }
    /// <summary>
    /// Converts cell name to cell index.
    /// </summary>
    /// <param name="name">Name of the cell.</param>
    /// <returns>Cell index of the specified cell.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When specified cell name is null.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When length of the specified cell name is less than 2.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When length of the alpha part of the name is less 
    /// than 1 or greater than 2.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When length of the number part of the name is less
    /// than 1 or greater than 5.
    /// </exception>
    public static long CellNameToIndex( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length < 2 )
        throw new ArgumentException( "name cannot be less then 2 symbols" );

      int iRow = 0;
      int iColumn = 0;

      CellNameToRowColumn( name, out iRow, out iColumn );

      return GetCellIndex( iColumn, iRow );
    }
    /// <summary>
    /// Converts cell name to row and column index.
    /// </summary>
    /// <param name="name">Name of the cell.</param>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    public static void CellNameToRowColumn( string name, out int iRow, out int iColumn )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length < 2 )
        throw new ArgumentException( "name cannot be less then 2 symbols" );

      int iLetterStart = -1;
      int iLetterCount = 0;
      int iDigitStart = -1;
      int iDigitCount = 0;

      for( int i = 0, len = name.Length; i < len; i++ )
      {
        char ch = name[ i ];

        if( char.IsDigit( ch ) )
        {
          if( iDigitStart < 0 )
            iDigitStart = i;

          iDigitCount++;
        }
        else if( char.IsLetter( ch ) )
        {
          if( iLetterStart < 0 )
            iLetterStart = i;

          iLetterCount++;
        }
        else if( ch != '$' )
        {
          throw new ArgumentOutOfRangeException( "name", "Character " + ch + " was not expected." );
        }
      }

      string strNumber = name.Substring( iDigitStart, iDigitCount );
      string strAlpha = name.Substring( iLetterStart, iLetterCount );

      iRow = int.Parse( strNumber, NumberStyles.None, NumberFormatInfo.InvariantInfo );
      iColumn = GetColumnIndex( strAlpha );
    }
    /// <summary>
    /// Converts column name into index.
    /// </summary>
    /// <param name="columnName">Name to convert.</param>
    /// <returns>Converted value.</returns>
    public static int GetColumnIndex( string columnName )
    {
      //columnName = columnName.ToUpper();
      int iColumn = 0;

      for( int i = 0, len = columnName.Length; i < len; i++ )
      {
        char currentChar = columnName[ i ];
        iColumn *= 26;
        iColumn += 1 + ( ( currentChar >= 'a' ) ?
          ( currentChar - 'a' ):
          ( currentChar - 'A' ) );
      }

      return iColumn;
    }
    /// <summary>
    /// Converts column index into string representation.
    /// </summary>
    /// <param name="iColumn">Column to process.</param>
    /// <returns>String name in excel of the column</returns>
    public static string GetColumnName( int iColumn )
    {
      if( iColumn < 1 )
        throw new ArgumentOutOfRangeException( "iColumn", "Value cannot be less than 1." );

      iColumn--;
      string strColumnName = string.Empty;
      //char[] arrChars = new char[ 3 ];
      //int iCurrentChar = 0;

      do
      {
        int iCurrentDigit = iColumn % 26;
        iColumn = iColumn / 26 - 1;
        //arrChars[ iCurrentChar ] = Convert.ToChar( 'A' + iCurrentDigit );
        strColumnName = Convert.ToChar( 'A' + iCurrentDigit ) + strColumnName;
        //iCurrentChar++;
      }
      while( iColumn >= 0 );

      return strColumnName;//string.Concat( arrChars );
    }
    /// <summary>
    /// Get cell name yy column and row index.
    /// </summary>
    /// <param name="firstColumn">Column index of the cell.</param>
    /// <param name="firstRow">Row index of the cell.</param>
    /// <returns>Cell name.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When firstColumn or firstRow is less than one.
    /// </exception>
    public static string GetCellName( int firstColumn, int firstRow )
    {
      return GetCellName( firstColumn, firstRow, false );
    }
    /// <summary>
    /// Get cell name yy column and row index.
    /// </summary>
    /// <param name="firstColumn">Column index of the cell.</param>
    /// <param name="firstRow">Row index of the cell.</param>
    /// <param name="bR1C1">Indicates whether to use R1C1-style reference mode.</param>
    /// <returns>Cell name.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When firstColumn or firstRow is less than one.
    /// </exception>
    public static string GetCellName( int firstColumn, int firstRow, bool bR1C1 )
    {
      return GetCellName( firstColumn, firstRow, bR1C1, false );
    }
    /// <summary>
    /// Get cell name yy column and row index.
    /// </summary>
    /// <param name="firstColumn">Column index of the cell.</param>
    /// <param name="firstRow">Row index of the cell.</param>
    /// <param name="bR1C1">Indicates whether to use R1C1-style reference mode.</param>
    /// <param name="bUseSeparater">If true adds '$' separator.</param>
    /// <returns>Cell name.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When firstColumn or firstRow is less than one.
    /// </exception>
    public static string GetCellName( int firstColumn, int firstRow, bool bR1C1, bool bUseSeparater )
    {
      if( firstRow < 1 )
        throw new ArgumentOutOfRangeException( "Row index is wrong. It cannot be less then 1" );

      if( bR1C1 )
        return string.Format( DEF_R1C1_FORMAT, firstRow, firstColumn );

      return ( bUseSeparater )
        ? string.Concat( DEF_CELL_NAME_SEPARATER, GetColumnName( firstColumn ), DEF_CELL_NAME_SEPARATER, firstRow )
        : string.Concat( GetColumnName( firstColumn ), firstRow );
    }
    /// <summary>
    /// Returns the range reference for the specified range in the language 
    /// of the user.
    /// </summary>
    /// <param name="iFirstRow">First row of the range.</param>
    /// <param name="iFirstColumn">First column of the range.</param>
    /// <param name="iLastRow">Last row of the range.</param>
    /// <param name="iLastColumn">Last column of the range.</param>
    /// <returns>The range reference for the specified range in the language of the user.</returns>
    public static string GetAddressLocal( int iFirstRow, int iFirstColumn, int iLastRow, int iLastColumn )
    {
      string cell0 = RangeImpl.GetCellName( iFirstColumn, iFirstRow );

      if( iFirstRow == iLastRow && iFirstColumn == iLastColumn )
      {
        return cell0;
      }
      else
      {
        string cell1 =  RangeImpl.GetCellName( iLastColumn, iLastRow );
        return cell0 + ":" + cell1;
      }
    }
    /// <summary>
    /// Returns the range reference for the specified range in the language 
    /// of the user.
    /// </summary>
    /// <param name="iFirstRow">First row of the range.</param>
    /// <param name="iFirstColumn">First column of the range.</param>
    /// <param name="iLastRow">Last row of the range.</param>
    /// <param name="iLastColumn">Last column of the range.</param>
    /// <param name="bR1C1">Indicates whether to use R1C1-style reference mode.</param>
    /// <returns>The range reference for the specified range in the language of the user.</returns>
    public static string GetAddressLocal( int iFirstRow, int iFirstColumn,
      int iLastRow, int iLastColumn, bool bR1C1 )
    {
      string cell0 = RangeImpl.GetCellName( iFirstColumn, iFirstRow, bR1C1 );

      if( iFirstRow == iLastRow && iFirstColumn == iLastColumn )
      {
        return cell0;
      }
      else
      {
        string cell1 =  RangeImpl.GetCellName( iLastColumn, iLastRow, bR1C1 );
        return cell0 + ":" + cell1;
      }
    }
    /// <summary>
    /// Get cell name by column and row index.
    /// </summary>
    /// <param name="firstColumn">Column index of the cell.</param>
    /// <param name="firstRow">Row index of the cell.</param>
    /// <returns>Cell name.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When firstColumn or firstRow is less than one.
    /// </exception>
    public static string GetCellNameWithDollars( int firstColumn, int firstRow )
    {
      if( firstColumn < 1 || firstRow < 1 )
        throw new ArgumentOutOfRangeException( "column or row index is wrong. It cannot be less then 1" );

      string strColumnName = GetColumnName( firstColumn );

      return "$" + strColumnName + "$" + firstRow;
    }
    /// <summary>
    /// Get cell index by column and row index.
    /// </summary>
    /// <param name="firstColumn">Column index of the cell.</param>
    /// <param name="firstRow">Row index of the cell.</param>
    /// <returns>Cell index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When firstRow or firstColumn is less than zero.
    /// </exception>
    public static long GetCellIndex( int firstColumn, int firstRow )
    {
      if (firstColumn == -1 || firstRow == -1)
          return -1;

      if( firstRow < 0 || firstColumn < 0 )
        throw new ArgumentOutOfRangeException( "wrong row or column index" );

      return ( ( long )firstRow << ColumnBitsInCellIndex ) + firstColumn;
    }
    /// <summary>
    /// Gets row index from cell index.
    /// </summary>
    /// <param name="index">Cell index.</param>
    /// <returns>Row index.</returns>
    [DebuggerStepThrough]
    public static int GetRowFromCellIndex( long index )
    {
      ulong ulIndex = ( ulong )index;
      
      int iResult = ( int )( ulIndex >> ColumnBitsInCellIndex );
      return iResult;
    }
    /// <summary>
    /// Gets column index from cell index.
    /// </summary>
    /// <param name="index">Cell index.</param>
    /// <returns>Column index.</returns>
    [DebuggerStepThrough]
    public static int GetColumnFromCellIndex( long index )
    {
      return ( int )( index & 0xFFFFFFFF );
    }
    /// <summary>
    /// Extracts worksheet name from range name.
    /// </summary>
    /// <param name="rangeName">Range name to extract from.</param>
    /// <returns>Worksheet name.</returns>
    public static string GetWorksheetName( ref string rangeName )
    {
      if( rangeName == null )
        throw new ArgumentNullException( "rangeName" );

      if( rangeName.Length == 0 )
        throw new ArgumentException( "rangeName - string cannot be empty" );
      int startPos = 0;
      string tempString1 = rangeName;
      int endPos = 0;
      int FirstStartpos = 0;
      string strSheetName = null;
      string tempString2 = null;
      char[] stringToChar = new char[rangeName.Length];
      stringToChar = rangeName.ToCharArray();
      for (int k = 0; k < stringToChar.Length - 1; k++)
        {
            if (stringToChar[k] == SingleQuote && stringToChar[k + 1] != SingleQuote && FirstStartpos == 0)
            {
                startPos = k;
                FirstStartpos++;
            }


            if (stringToChar[k] == '!' && k > 2 && stringToChar[k - 1] == SingleQuote && stringToChar[k - 2] != SingleQuote)
            {
                endPos = k;
                break;
            }
        }
      int iPos = rangeName.IndexOf( '!' );

        if (iPos != -1)
        {
            strSheetName = rangeName.Substring(startPos, endPos - startPos).Replace("''", "'");
            tempString1 = rangeName.Substring(iPos + 1, rangeName.Length - iPos - 1);
            //rangeName = rangeName.Substring(iPos + 1, rangeName.Length - iPos - 1);
            if (strSheetName == "")
            {
                tempString2 = rangeName.Substring(0, iPos);
                if (tempString2.Contains("("))
                    tempString2 = tempString2.Substring(tempString2.IndexOf('(')+1);             
                strSheetName = tempString2;
                if (!tempString1.Contains(strSheetName))
                    rangeName = rangeName.Substring(iPos + 1, rangeName.Length - iPos - 1);
            }
            else if (!tempString1.Contains(strSheetName))
                rangeName = rangeName.Substring(endPos + 1, rangeName.Length - endPos - 1);
            if (endPos != 0)
            {
                int iLength = strSheetName.Length;
                if (strSheetName[0] == SingleQuote
                  && strSheetName[iLength - 1] == SingleQuote)
                {
                    strSheetName = strSheetName.Substring(1, iLength - 2);
                }
            }       


        }        
        return strSheetName;       
    }
    /// <summary>
    /// Helper methods for WrapText Property.
    /// </summary>
    /// <param name="rangeColection">List of IRange.</param>
    /// <returns>Gets WrapText property value.</returns>
    public static bool GetWrapText( IList rangeColection )
    {
      if( rangeColection == null )
        throw new ArgumentNullException( "rangeColection" );

      bool bResult = true;
      int len = rangeColection.Count;
      int i = 0;

      while( bResult && i < len )
      {
        IRange range = rangeColection[ i ] as IRange;

        if( !range.WrapText )
        {
          bResult = DEF_WRAPTEXT_VALUE;
          break;
        }

        i++;
      }

      return bResult;
    }
    /// <summary>
    /// Helper methods for WrapText Property.
    /// </summary>
    /// <param name="rangeColection">List of IRange.</param>
    /// <param name="wrapText">Value to set.</param>
    public static void SetWrapText( IList rangeColection, bool wrapText )
    {
      for( int i = 0, len = rangeColection.Count; i < len; i++ )
      {
        ( ( IRange )rangeColection[ i ] ).WrapText = wrapText;
      }
    }
    /// <summary>
    /// Helper methods for WrapText Property.
    /// </summary>
    /// <param name="rangeColection">List of IRange.</param>
    /// <returns>Gets WrapText property value.</returns>
    public static string GetNumberFormat( IList rangeColection )
    {
      int iCount = rangeColection.Count;

      if( iCount == 0 ) return null;

      IRange range = ( IRange )rangeColection[ 0 ];
      string strResult = range.NumberFormat;

      for( int i = 1; i < iCount; i++ )
      {
        range = ( IRange )rangeColection[ i ];

        if( strResult != range.NumberFormat ) return null;
      }

      return strResult;
    }
    /// <summary>
    /// Helper methods for CellStyleName Property.
    /// </summary>
    /// <param name="rangeColection">List of IRange.</param>
    /// <returns>Gets CellStyleName property value.</returns>
    public static string GetCellStyleName( IList<IRange> rangeColection )
    {
      int iCount = rangeColection.Count;

      if( iCount == 0 ) return null;

      IRange range = rangeColection[ 0 ];
      string strResult = range.CellStyleName;

      for( int i = 1; i < iCount; i++ )
      {
        range = rangeColection[ i ];

        if( strResult != range.CellStyleName ) return null;
      }

      return strResult;
    }
    /// <summary>
    /// Parses string representation of the range.
    /// </summary>
    /// <param name="range">Range to parse.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="iFirstRow">First row.</param>
    /// <param name="iFirstColumn">First column.</param>
    /// <param name="iLastRow">Last row.</param>
    /// <param name="iLastColumn">Last column.</param>
    /// <returns>Number of parts: 1 - one cell, 2 - range of cells.</returns>
    public static int ParseRangeString( string range, IWorkbook book, out int iFirstRow, out int iFirstColumn,
      out int iLastRow, out int iLastColumn )
    {
      iLastColumn = iLastRow = iFirstColumn = iFirstRow = -1;

      string[] cells = range.Split( ':' );
      int iCount = cells.Length;

      Regex regex = FormulaUtil.FullRowRangeRegex;
      Match match = regex.Match( range );

      if( match.Success && match.Index == 0 && match.Length == range.Length )
      {
        iFirstColumn = 1;
        iLastColumn = book.MaxColumnCount;
        string strFirstRow = UtilityMethods.RemoveFirstCharUnsafe( match.Groups[ FormulaUtil.DEF_GROUP_ROW1 ].Value );
        string strLastRow = UtilityMethods.RemoveFirstCharUnsafe( match.Groups[ FormulaUtil.DEF_GROUP_ROW2 ].Value );
        iFirstRow = Convert.ToInt32( strFirstRow );
        iLastRow = Convert.ToInt32( strLastRow );
        return iCount;
      }

      regex = FormulaUtil.FullColumnRangeRegex;
      match = regex.Match( range );

      if( match.Success && match.Index == 0 && match.Length == range.Length )
      {
        string strFirstColumn = UtilityMethods.RemoveFirstCharUnsafe( match.Groups[ FormulaUtil.DEF_GROUP_COLUMN1 ].Value );
        string strLastColum = UtilityMethods.RemoveFirstCharUnsafe( match.Groups[ FormulaUtil.DEF_GROUP_COLUMN2 ].Value );
        iFirstColumn = RangeImpl.GetColumnIndex( strFirstColumn );
        iLastColumn = RangeImpl.GetColumnIndex( strLastColum );
        iFirstRow = 1;
        iLastRow = book.MaxRowCount;
        return iCount;
      }

      long index = -1;
        
      if( iCount >= 1 )
      {
        index = RangeImpl.CellNameToIndex( cells[ 0 ] );
        iLastRow = iFirstRow = RangeImpl.GetRowFromCellIndex( index );
        iLastColumn = iFirstColumn = RangeImpl.GetColumnFromCellIndex( index );
      }

      if( iCount == 2 )
      {
        long index2 = RangeImpl.CellNameToIndex( cells[ 1 ] );
          
        if( index != index2 )
        {
          iLastRow = RangeImpl.GetRowFromCellIndex( index2 );
          iLastColumn = RangeImpl.GetColumnFromCellIndex( index2 );
        }

      }
      else if( iCount > 2 )
        throw new ArgumentException();

      return iCount;
    }
    /// <summary>
    /// Gets rectangle object, that represents rect of range.
    /// </summary>
    /// <param name="range">Represents current range.</param>
    /// <param name="bThrowExcONNullRange">If true than thrown an exception, if range is null.</param>
    /// <returns>Returns rectangle, that represents borders of range.</returns>
    public static Rectangle GetRectangeOfRange( IRange range, bool bThrowExcONNullRange )
    {
      Rectangle rec = new Rectangle( -1, -1, -1, -1 );

      if( range == null )
      {
        if( bThrowExcONNullRange )
          throw new ArgumentNullException( "range" );

        return rec;
      }

      rec.Y = range.Row;
      rec.Height = range.LastRow - rec.Y;
      rec.X = range.Column;
      rec.Width = range.LastColumn - rec.X;

      return rec;
    }
    #endregion

    #region Style wrapping
    /// <summary>
    /// This method is called when NumberFormat of the range changes.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    internal protected void wrapStyle_OnNumberFormatChanged( object sender, /*ValueChanged*/EventArgs e )
    {
      TCellType oldCellType = this.CellType;

      string strValue = Value;
      OnValueChanged( strValue, strValue );
      OnStyleChanged( oldCellType );
    }
    /// <summary>
    /// Attaches handler to NumberFormatChanged event of parent style.
    /// </summary>
    private void AttachEventToStyle()
    {
      int iXFIndex = ExtendedFormatIndex;
      ExtendedFormatImpl format = ( ExtendedFormatImpl )m_book.InnerExtFormats[ iXFIndex ];

      iXFIndex = format.ParentIndex;
      StyleImpl style = m_book.InnerStyles.GetByXFIndex( iXFIndex );
      AttachEvent( style, new EventHandler( wrapStyle_OnNumberFormatChanged ) );
    }
    /// <summary>
    /// Attaches event to cell styles.
    /// </summary>
    private void AttachEventToCellStyle()
    {
      AttachEvent( m_style, new EventHandler( wrapStyle_OnNumberFormatChanged ) );
    }
    /// <summary>
    /// Attaches handler to NumberFormatChanged of the specified wrapper.
    /// </summary>
    /// <param name="wrapper">Wrapper to attach event to.</param>
    /// <param name="handler">Handler to attach.</param>
    private void AttachEvent( ExtendedFormatWrapper wrapper, EventHandler handler )
    {
      wrapper.NumberFormatChanged += handler;
    }
    /// <summary>
    /// Creates style wrapper.
    /// </summary>
    protected void CreateStyle()
    {
      int index = m_book.DefaultXFIndex;
      //StyleImpl style;
      BiffRecordRaw record = Record;

      if( record != null )
      {
        ICellPositionFormat format = ( ICellPositionFormat )record;
        index = format.ExtendedFormatIndex;
      }

      CreateStyleWrapper( index );
    }
    /// <summary>
    /// Initializes style wrapper.
    /// </summary>
    /// <param name="value">Extended format index.</param>
    protected void CreateStyleWrapper( int value )
    {
        if (!IsSingleCell && !IsEntireRow && !IsEntireColumn)
        throw new ArgumentException( "This method can be used only for single cell not a range" );

      if( m_style != null )
      {
        //m_style.Dispose();
      }

      //m_style = new CellStyle( Application, this, value );
      m_style = new CellStyle( this, value );

//      m_style = new StyleImplWrapper( this, styleToWrap );
//      m_style.OnBeforeChange += new ValueChangedEventHandler( wrapStyle_OnBeforeChange );
//      m_style.OnNumberFormatChanged += new ValueChangedEventHandler( wrapStyle_OnNumberFormatChanged );
    }

    /// <summary>
    /// Initializes style wrapper.
    /// </summary>
    /// <param name="value">Extended format index.</param>
    internal static IStyle CreateTempStyleWrapperWithoutRange(RangeImpl rangeImpl, int value)
    {


        //m_style = new CellStyle( Application, this, value );
        return new CellStyle(rangeImpl, value);

        //      m_style = new StyleImplWrapper( this, styleToWrap );
        //      m_style.OnBeforeChange += new ValueChangedEventHandler( wrapStyle_OnBeforeChange );
        //      m_style.OnNumberFormatChanged += new ValueChangedEventHandler( wrapStyle_OnNumberFormatChanged );
    }

    /// <summary>
    /// Sets index of extended format that defines style for this range..
    /// </summary>
    /// <param name="index">Index to set.</param>
    public void SetXFormatIndex( int index )
    {
        if (!IsSingleCell && !IsEntireRow && !IsEntireColumn)
        throw new ApplicationException( "This method should be called for single range cells only" );

      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0" );

//      BiffRecordRaw record = Record;
//
//      if( Record == null )
//      {
//        Record = record = CreateRecordWithoutAdd( TBIFFRecord.Blank );
//      }
//
      ExtendedFormatImpl format = m_book.InnerExtFormats[ index ];

      if( format.Record.XFType == ExtendedFormatRecord.TXFType.XF_CELL )
      {
        format = format.CreateChildFormat( m_book.InnerExtFormats[ ExtendedFormatIndex ] );
      }
      else
      {
        format = format.CreateChildFormat();
      }

      index = format.Index;
//
//      ICellPositionFormat cellPos = ( ICellPositionFormat )record;
//      cellPos.ExtendedFormatIndex = ( ushort )index;
//      Record = record;
      
      if (IsEntireRow)
      {
          int firstRow = Row;
          int lastRow = LastRow;

          for (int i = firstRow; i <= lastRow; i++)
          {
              m_worksheet.CellRecords.SetCellStyle(i, index);
          }
      }
      else if (IsEntireColumn)
      {
          int firstColumn = FirstColumn;
          int lastColumn = LastColumn;

          for (int i = firstColumn; i <= lastColumn; i++)
          {
              ColumnInfoRecord columnInfo = (ColumnInfoRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ColumnInfo);
              columnInfo.FirstColumn = (ushort)(firstColumn - 1);
              columnInfo.LastColumn = (ushort)(LastColumn - 1);
              columnInfo.ExtendedFormatIndex = (ushort)index;
              m_worksheet.ColumnInformation[i] = columnInfo;
          }
      }
      else
          m_worksheet.CellRecords.SetCellStyle(Row, Column, index);

      if( m_style != null )
      {
        m_style.SetFormatIndex( index );
      }

      if( Array.IndexOf<ExcelLineStyle>( ThinBorders, format.BottomBorderLineStyle ) < 0 )
      {
        RowStorage row = WorksheetHelper.GetOrCreateRow( m_worksheet, Row - 1, false );

        if( row != null )
          row.IsSpaceBelowRow = true;
      }
    }
    /// <summary>
    /// Changes style name.
    /// </summary>
    /// <param name="strNewName">Name to set.</param>
    /// <returns>Style object that corresponds to the name.</returns>
    private StyleImpl ChangeStyleName( string strNewName )
    {
      int iNewIndex = m_book.DefaultXFIndex;
      StyleImpl style = null;

      if( strNewName != null && strNewName.Length > 0 )
      {
        if( !m_book.InnerStyles.ContainsName( strNewName ) &&
          ( m_book.Version !=ExcelVersion.Excel97to2003 ) )
        {
          int index = Array.IndexOf( DefaultStyleNames, strNewName );
          style = m_book.InnerStyles.CreateBuiltInStyle( strNewName );
        }
        else
        {
          style = ( StyleImpl )m_book.Styles[ strNewName ];
        }

        if( style != null )
          iNewIndex = style.Index;
      }
      else
      {
        style = ( StyleImpl )m_book.Styles[ iNewIndex ];
      }

      // Here we have to create corresponding extended format.
//      ExtendedFormatImpl format = ( ExtendedFormatImpl )m_book.CreateExtFormatWithoutRegister( style.Wrapped );
//      format.Record.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
//      format.ParentIndex = iNewIndex;
//      format = ( ExtendedFormatImpl )m_book.RegisterExtFormat( format );

      ExtendedFormatIndex = ( ushort )iNewIndex;//( ushort )format.Index;
      return style;
    }
    /// <summary>
    /// Returns name of the style, applied to the cell.
    /// </summary>
    /// <returns>Name of the style, applied to the cell.</returns>
    private string GetStyleName()
    {
      if( m_style != null )
      {
        return m_style.Name;
      }
      else
      {
        int iXFIndex = ExtendedFormatIndex;
        StyleImpl style = m_book.InnerStyles.GetByXFIndex( iXFIndex );

        if( style == null )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Incorrect situation. Bad indexes", "Warning" );
          ExtendedFormatImpl xFormat = ( ExtendedFormatImpl )m_book.InnerExtFormats[ iXFIndex ];

          style = m_book.InnerStyles.GetByXFIndex( xFormat.ParentIndex );

          //As MS Excel, If parent style not found, Then default style is parent.
          if (style == null)
          {
              style = m_book.InnerStyles[RangeImpl.DEF_DEFAULT_STYLE] as StyleImpl;
              ExtendedFormatIndex = (ushort)style.Index;
          }
        }

        return style.Name;
      }
    }
    /// <summary>
    /// Returns WrapText of the style, applied to the cell.
    /// </summary>
    /// <returns>WrapText of the style, applied to the cell.</returns>
    private bool GetWrapText()
    {
      if( m_style != null )
      {
        return m_style.WrapText;
      }
      else
      {
        return ExtendedFormat.WrapText;
      }
    }
    /// <summary>
    /// Gets format code.
    /// </summary>
    /// <returns>Value representing number format code</returns>
    private string GetNumberFormat()
    {
      if( m_style != null )
      {
        return m_style.NumberFormat;
      }
      else
      {
        return ExtendedFormat.NumberFormat;
      }
    }
    /// <summary>
    /// Gets the date time by culture.
    /// </summary>
    /// <param name="strDateTime">Date in string format.</param>
    /// <param name="dtValue">Result in Datetime.</param>
    /// <returns>Returns true, when the date time is culture based.</returns>
    internal bool TryGetDateTimeByCulture(string strDateTime,bool isUKCulture,out DateTime dtValue)
    {
        if (strDateTime == null)
            throw new ArgumentNullException("strDateTime");
        string format = 
#if ( WINRT )
            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
#else
            System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
#endif
        dtValue = DateTime.Now;
        //In MS Excel, Default DateTime format of 14th Index format is displayed 
        //based on Current culture setting.
        if (ExtendedFormat.NumberFormatIndex == 14)
        {
            if (DateTime.TryParse(strDateTime, 
#if ( WINRT )
                CultureInfo.CurrentCulture
#else
                System.Threading.Thread.CurrentThread.CurrentCulture
#endif
                , DateTimeStyles.NoCurrentDateDefault, out dtValue))
                return true;
            //In US Culture, this date should be refered as mm/dd/yyyy, but MS Excel accepts the UK culture date format (dd/mm/yyyy).
            // in US Culture.
            else if (DateTime.TryParse(strDateTime, new CultureInfo(UKCultureName), DateTimeStyles.None, out dtValue))
                return true;
        }
        else if (isUKCulture)
        {
            if (DateTime.TryParse(strDateTime, new CultureInfo(UKCultureName), DateTimeStyles.None, out dtValue))
                return true;
        }
        return false;
    }
    /// <summary>
    /// Returns extended format for this range. Read-only.
    /// This property should only be used for reading values.
    /// </summary>
    private ExtendedFormatImpl ExtendedFormat
    {
      get
      {
        int iXFIndex = ExtendedFormatIndex;
        return m_book.InnerExtFormats[ iXFIndex ];
      }
    }
    /// <summary>
    /// Updates range information from record.
    /// </summary>
    public void UpdateRecord()
    {
      if( m_style != null )
      {
        ICellPositionFormat record = ( ICellPositionFormat )Record;
        m_style.SetFormatIndex( record.ExtendedFormatIndex );
      }
    }
    /// <summary>
    /// Checks whether formula arrays inside this range are separated or not.
    /// </summary>
    /// <param name="hashToSkip">Dictionary with records to skip.</param>
    /// <returns>True if records are not separated.</returns>
    public bool GetAreArrayFormulasNotSeparated( Dictionary<ArrayRecord, object> hashToSkip )
    {
      if( hashToSkip == null )
        hashToSkip = new Dictionary<ArrayRecord, object>();

      CellRecordCollection cellRecords = m_worksheet.CellRecords;

      for( int iRow = FirstRow, iLastRow = LastRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = FirstColumn, iLastCol = LastColumn; iCol <= iLastCol; iCol++ )
        {
          iCol = cellRecords.FindRecord( TBIFFRecord.Formula, iRow, iCol, iLastCol );

          if( iCol <= iLastCol )
          {
            ArrayRecord array = cellRecords.GetArrayRecord( iRow, iCol );

            if( array != null )
            {
              if( !hashToSkip.ContainsKey( array ) )
              {
                if( array.FirstRow + 1 < FirstRow || array.LastRow + 1 > iLastRow
                  || array.FirstColumn + 1 < FirstColumn || array.LastColumn + 1 > iLastCol )
                {
                  return false;
                }

                hashToSkip.Add( array, null );
              }
            }
          }
        }
      }
      return true;
    }
    #endregion

    #region IReparse Members
    /// <summary>
    /// Reparse cell if parsing wasn't successful when loading the workbook.
    /// </summary>
    public void Reparse()
    {
      TCellType cellType = CellType;

      if( cellType == TCellType.Formula )
      {
        //m_strValue = FormulaUtil.ParseFormulaRecord( ( FormulaRecord ) m_record, m_book );
        ////Debug.WriteLineIf( true/*ApplicationImpl.IsDebugInfoEnabled*/, Address, "Reparsing range" );
        ReParseFormula( ( FormulaRecord )Record );
      }
      else if( cellType == TCellType.LabelSST )
      {
        //ParseLabelSST( Record as LabelSSTRecord );
      }
    }

    #endregion

    #region ICellPositionFormat members
    /// <summary>
    /// Returns type code of the underlying record. Read-only.
    /// </summary>
    TBIFFRecord ICellPositionFormat.TypeCode
    {
      get
      {
        return ( TBIFFRecord )CellType;
      }
    }
    /// <summary>
    /// Gets / sets cell column.
    /// </summary>
    int ICellPositionFormat.Column
    {
      get
      {
        return FirstColumn - 1;
      }
      set
      {
        if( !IsSingleCell )
          throw new ArgumentException( "This property can be called only for single cell ranges" );

        FirstColumn = LastColumn = value + 1;
      }
    }
    /// <summary>
    /// Gets / sets cell row.
    /// </summary>
    int ICellPositionFormat.Row
    {
      get
      {
        return FirstRow - 1;
      }
      set
      {
        if( !IsSingleCell )
          throw new ArgumentException( "This property can be called only for single cell ranges" );

        FirstRow = LastRow = value + 1;
      }
    }
    #endregion

    #region INativePTG methods
    /// <summary>
    /// Gets ptg of current range.
    /// </summary>
    /// <returns>Returns native ptg.</returns>
    public Ptg[] GetNativePtg()
    {
      Ptg result;
      int index = m_book.AddSheetReference( m_worksheet.Name );

      if( IsSingleCell )
      {
        result = FormulaUtil.CreatePtg( FormulaToken.tRef3d1, index
          , FirstRow - 1, FirstColumn - 1, ( byte )0 );
      }
      else
      {
        result = FormulaUtil.CreatePtg( FormulaToken.tArea3d1, index, FirstRow - 1
          , FirstColumn - 1, LastRow - 1, LastColumn - 1, ( byte )0, ( byte )0 );
      }
      
      return new Ptg[ 1 ]{ result };
    }
    #endregion

    #region Class delegates
    /// <summary>
    /// 
    /// </summary>
    private delegate IOutline OutlineGetter( int iOutlineIndex );
    #endregion

    #region IEnumerable<IRange> Members

    public IEnumerator<IRange> GetEnumerator()
    {
        return (this.CellsList as IEnumerable<IRange>).GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (this.CellsList as IEnumerable).GetEnumerator();
    }

    #endregion

    #region CellValueChanged Events and delegates

    internal void OnCellValueChanged(object oldValue, object newValue, IRange range)
    {
        (Worksheet as WorksheetImpl).OnCellValueChanged(oldValue, newValue, range);
    }

    public delegate void CellValueChangedEventHandler(object sender, CellValueChangedEventArgs e);

    #endregion

  }
  /// <summary>
  /// 
  /// </summary>
    public class CellValueChangedEventArgs : EventArgs
  {
      private object m_oldValue;
      private object m_newValue;
      private IRange m_range;

      /// <summary>
      /// Gets or sets the old value.
      /// </summary>
      /// <value>The old value.</value>
     public object OldValue
      {
          get
          {
              return m_oldValue;
          }
          set
          {
              m_oldValue = value;
          }
      }
     /// <summary>
     /// Gets or sets the new value.
     /// </summary>
     /// <value>The new value.</value>
     public object NewValue
      {
         get
          {
              return m_newValue;
          }
          set
          {
             m_newValue = value;
          }
      }
     /// <summary>
     /// Gets or sets the range.
     /// </summary>
     /// <value>The range.</value>
      public IRange Range
      {
          get
          {
              return m_range;
          }
          set
          {
              m_range = value;
          }
      }
  }
   
}
