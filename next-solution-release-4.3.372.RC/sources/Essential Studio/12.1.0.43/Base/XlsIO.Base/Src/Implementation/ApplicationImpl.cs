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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Security.Principal;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.TemplateMarkers;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.Compression;
using Syncfusion.Compression.Zip;
using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO;
#endif


#if  SILVERLIGHT 
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif !(WINRT )
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.XlsIO.Implementation.Clipboard;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// XlsIO Application interface declaration.
  /// </summary>
  public class ApplicationImpl : IApplication
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    /// Quits Microsoft Excel.
    /// </summary>
    public void Quit()
    {
      // TODO:  Add ApplicationImpl.Quit implementation.
      throw new NotImplementedException();
    }
    /// <summary>
    /// For an Application object, returns a Range object that represents all
    /// the rows on the active worksheet. If the active document isn't a worksheet,
    /// the Rows property fails. For a Range object, returns a Range object that
    /// represents the rows in the specified range. For a Worksheet object, returns
    /// a Range object that represents all the rows on the specified worksheet.
    /// Read-only Range object.
    /// </summary>
    public IRange       Rows
    {
      get
      {
        return m_rows;
      }
    }

    /// <summary>
    /// Returns a Range object that represents all the cells on the
    /// active worksheet. If the active document isn't a worksheet, this
    /// property fails. Read-only.
    /// </summary>
    public IRange       Cells
    {
      get
      {
        return m_cells;
      }
    }

    /// <summary>
    /// Returns a Range object that represents all the columns on the
    /// active worksheet. If the active document isn't a worksheet, the
    /// Columns property fails. Read-only.
    /// </summary>
    public IRange       Columns
    {
      get
      {
        return m_columns;
      }
    }

#endif
    #endregion

    #region Class constants
    /// <summary>
    /// Width of the zero character.
    /// </summary>
    private const double  DEF_ZERO_CHAR_WIDTH = 8;
    /// <summary>
    /// Build number.
    /// </summary>
    private const int     DEF_BUILD_NUMBER = 0;
    /// <summary>
    /// Default font size.
    /// </summary>
    private const double  DEF_STANDARD_FONT_SIZE = 10.0;
    /// <summary>
    /// Default quantity of decimal digits after separator.
    /// </summary>
    private const int     DEF_FIXED_DECIMAL_PLACES = 4;
    /// <summary>
    /// Default sheets in the new workbook.
    /// </summary>
    private const int     DEF_SHEETS_IN_NEW_WORKBOOK = 3;
    /// <summary>
    /// Default font name.
    /// </summary>
    private const string  DEF_DEFAULT_FONT = "Arial";
    /// <summary>
    /// Name of the object "Microsoft Excel".
    /// </summary>
    private const string  DEF_VALUE = "Microsoft Excel";
#if !(WINRT )
    /// <summary>
    /// Path separator.
    /// </summary>
    private string  DEF_PATH_SEPARATOR = "" + System.IO.Path.PathSeparator;
#endif
    /// <summary>
    /// Name of the switch.
    /// </summary>
    private const string DEF_SWITCH_NAME = "Syncfusion.XlsIO.DebugInfo";
    /// <summary>
    /// Switch description.
    /// </summary>
    private const string DEF_SWITCH_DESCRIPTION = "Indicates wether to show library debug messages.";
    /// <summary>
    /// Argument separator.
    /// </summary>
    public const char DEF_ARGUMENT_SEPARATOR = ',';
    /// <summary>
    /// Row separator.
    /// </summary>
    public const char DEF_ROW_SEPARATOR = ';';
    #endregion

    #region Class static members
    /// <summary>
    /// Array of Proportions.
    /// </summary>
    private static readonly double[] s_arrProportions;
    /// <summary>
    /// Minimum cell size.
    /// </summary>
    internal static readonly SizeF MinCellSize = new SizeF( 8.43f, 12.75f );
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Represents the Graphic.
    /// </summary>
    private readonly Graphics m_graphics;
    /// <summary>
    /// Switch indicating whether to show debug information.
    /// </summary>
    private static readonly BooleanSwitch m_switch = new BooleanSwitch( DEF_SWITCH_NAME,
      DEF_SWITCH_DESCRIPTION );
#endif
    /// <summary>
    /// Flag for debug message.
    /// </summary>
    private static readonly bool m_bDebugMessage = 
#if !SILVERLIGHT && !WINRT && !WP
      m_switch.Enabled;
#else
      false;
#endif
    /// <summary>
    /// All types in the assembly (in order to increase performance).
    /// </summary>
    internal static Type[] AssemblyTypes =
#if ( WINRT )
        null;
        
#else
        Assembly.GetExecutingAssembly().GetTypes();
#endif
    /// <summary>
    /// Parent object
    /// </summary>
    private object m_parent;
    /// <summary>
    /// Indicates is debug info enabled.
    /// </summary>
    private static bool m_bIsDebugInfoEnabled = false;
    /// <summary>
    /// XlsIO Static Members.
    /// </summary>
    private StringEnumerations m_stringEnum = new StringEnumerations();
    /// <summary>
    /// 
    /// </summary>
    private static ExcelDataProviderType m_dataType =
#if AllowUnsafeCode
      ExcelDataProviderType.Native;
#else
      ExcelDataProviderType.ByteArray;
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Default styles names. Index means outline level value.
    /// </summary>
    private string[] m_defaultStyleNames = new string[]
    {
      "Normal",
      "RowLevel_",
      "ColLevel_",
      "Comma",
      "Currency",
      "Percent",
      "Comma [0]",
      "Currency [0]",
      "Hyperlink",
      "Followed Hyperlink",
      "Note",
      "Warning Text",
      "Emphasis 1",
      "Emphasis 2",
      "",
      "Title",
      "Heading 1",
      "Heading 2",
      "Heading 3",
      "Heading 4",
      "Input",
      "Output",
      "Calculation",
      "Check Cell",
      "Linked Cell",
      "Total",
      "Good",
      "Bad",
      "Neutral",
      "Accent1",
      "20% - Accent1",
      "40% - Accent1",
      "60% - Accent1",
      "Accent2",
      "20% - Accent2",
      "40% - Accent2",
      "60% - Accent2",
      "Accent3",
      "20% - Accent3",
      "40% - Accent3",
      "60% - Accent3",
      "Accent4",
      "20% - Accent4",
      "40% - Accent4",
      "60% - Accent4",
      "Accent5",
      "20% - Accent5",
      "40% - Accent5",
      "60% - Accent5",
      "Accent6",
      "20% - Accent6",
      "40% - Accent6",
      "60% - Accent6",
      "Explanatory Text",
    };
    /// <summary>
    /// Table with paper sizes.
    /// </summary>
    private Dictionary<int, PageSetupBaseImpl.PaperSizeEntry> m_dicPaperSizeTable;
    /// <summary>
    /// Range representing active cell of the active worksheet of the active workbook.
    /// </summary>
    private IRange m_ActiveCell;
    /// <summary>
    /// Active worksheet.
    /// </summary>
    private WorksheetBaseImpl m_ActiveSheet;
    /// <summary>
    /// Active workbook.
    /// </summary>
    private IWorkbook m_ActiveBook;
    /// <summary>
    /// Collection of all workbooks in the application.
    /// </summary>
    private WorkbooksCollection m_workbooks;
    /// <summary>
    /// All data entered after this field is set to True will be formatted
    /// with the number of fixed decimal places set by the FixedDecimalPlaces
    /// property.
    /// </summary>
    private bool m_bFixedDecimal;
    /// <summary>
    /// True (default) if the system separators of Microsoft Excel are
    /// enabled.
    /// </summary>
    private bool m_bUseSystemSep;
    /// <summary>
    /// Standard font size, in points.
    /// </summary>
    private double m_dbStandardFontSize;
    /// <summary>
    /// Number of fixed decimal places used when the FixedDecimal
    /// property is set to True.
    /// </summary>
    private int m_iFixedDecimalPlaces;
    /// <summary>
    /// The number of sheets that Microsoft Excel automatically
    /// inserts into new workbooks.
    /// </summary>
    private int m_iSheetsInNewWorkbook;
    /// <summary>
    /// The character used for the decimal separator as a String.
    /// </summary>
    private string m_strDecimalSeparator;
    /// <summary>
    /// Name of the standard font.
    /// </summary>
    private string m_strStandardFont;
    /// <summary>
    /// The character used for the thousands separator as a String.
    /// </summary>
    private string m_strThousandsSeparator;
    /// <summary>
    /// The name of the current user.
    /// </summary>
    private string m_strUserName;
    /// <summary>
    /// If this value is True and if some cells have reference 
    /// to the same style, changes will influence all these cells.
    /// False otherwise
    /// </summary>

    private bool   m_bChangeStyle;
    /// <summary>
    /// Storage of SkipExtendedRecords property value.
    /// </summary>
    private SkipExtRecords m_enSkipExtRecords = SkipExtRecords.None;
    /// <summary>
    /// Standard row height - height of the rows that do not have specified row height.
    /// </summary>
    private int             m_iStandardRowHeight = 255;
    /// <summary>
    /// Standard (default) height option flag, which defines that standard (default)
    /// row height and book default font height do not match.
    /// </summary>
    private bool            m_bStandartRowHeightFlag = false;
    /// <summary>
    /// Standard column width.
    /// </summary>
    private double          m_dStandardColWidth = 8.43;
    /// <summary>
    /// Indicates whether to optimize fonts count. This option will
    /// take effect only on workbooks that will be added after setting
    /// this property.
    /// WARNING: Setting this property to True can decrease performance significantly,
    /// but will reduce resulting file size.
    /// </summary>
    private bool            m_bOptimizeFonts;
    /// <summary>
    /// Indicates whether to optimize Import data. This option will
    /// take effect only on Import methods that are available with the worksheet
    /// WARNING: Setting this property to True can decrease memory significantly,
    /// but will increase the performance of data import .
    /// </summary> 
    private bool           m_bOptimizeImport;
    /// <summary>
    /// Row separator for array parsing.
    /// </summary>
    private char m_chRowSeparator = DEF_ROW_SEPARATOR;
    /// <summary>
    /// Formula arguments separator.
    /// </summary>
    private char m_chArgumentSeparator = DEF_ARGUMENT_SEPARATOR;
    /// <summary>
    /// Represents CSV Separator.
    /// </summary>
    private string m_strCSVSeparator = ",";
    /// <summary>
    /// Indicates whether to try fast record parsing.
    /// </summary>
    private bool m_bUseFastRecordParsing;
    /// <summary>
    /// Memory allocation block for single row.
    /// </summary>
    private int m_iRowStorageBlock = 128;
    /// <summary>
    /// Indicates whether XlsIO should delete destination file before saving into it.
    /// Default value is TRUE.
    /// </summary>
    private bool m_bDeleteDestinationFile = true;

    private CultureInfo m_standredCulture;
    private CultureInfo m_currentCulture;
     
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Represents the Graphic.
    /// </summary>
    private readonly Graphics m_autoFilterManagerGraphics;
#endif
    /// <summary>
    /// Default excel version for new workbooks.
    /// </summary>
    private ExcelVersion m_defaultVersion = ExcelVersion.Excel97to2003;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bNetStorage =
#if AllowUnsafeCode
      false;
#else
      true;
#endif
    /// <summary>
    /// Indicates whether evaluation license expired.
    /// </summary>
    private bool m_bEvalExpired;
    private CompressionLevel? m_compressionLevel;
    private bool m_preserveTypes;
    private bool m_isFormulaparsed = true;
    private StyleImpl.StyleSettings[] m_builtInStyleInfo;
    //Default row height for xlsx file.
    private const int DefaultRowHeightXlsx = 300;
#if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    private IChartToImageConverter m_chartToImageConverter;
#endif
    #endregion

    #region Class native properties
    public string[] DefaultStyleNames
    {
        get
        {
            return m_defaultStyleNames;
        }
    }
    internal StyleImpl.StyleSettings[] BuiltInStyleInfo
    {
        get
        {
            return m_builtInStyleInfo;
        }
    }
    internal Dictionary<int, PageSetupBaseImpl.PaperSizeEntry> DicPaperSizeTable
    {
        get
        {
            return m_dicPaperSizeTable;
        }
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Switch indicating whether to show debug information. Read-only.
    /// </summary>
    public static BooleanSwitch DebugInfo
    {
      get
      {
        return m_switch;
      }
    }
#endif
    /// <summary>
    /// Indicates whether debug info is enabled. Read-only.
    /// </summary>
    public static bool IsDebugInfoEnabled
    {
      get
      {
        return m_bIsDebugInfoEnabled;
        //return true;
        //return m_bDebugMessage;
      }
      set
      {
        m_bIsDebugInfoEnabled = value;
      }
    }
    /// <summary>
    /// Indicates is use unsafe code. Static property.
    /// </summary>
    [ Obsolete() ]
    public static bool UseUnsafeCodeStatic
    {
      get
      {
        return m_dataType == ExcelDataProviderType.Unsafe;//m_bUseUnsafeCode;
      }
      set
      {
        if( value )
        {
          m_dataType = ExcelDataProviderType.Unsafe;
        }
        else
        {
          m_dataType = ExcelDataProviderType.Native;
        }
      }
    }
    /// <summary>
    /// Changes data provider type for all operations after it. Static property.
    /// </summary>
    public static ExcelDataProviderType DataProviderTypeStatic
    {
      get
      {
        return m_dataType;
      }
      set
      {
        m_dataType = value;
      }
    }
    /// <summary>
    /// If True, no changes were made since last save.
    /// </summary>
    public bool IsSaved
    {
      get
      {
        IWorkbooks books = Workbooks;

        for( int i = 0, len = books.Count; i < len; i++ )
        {
          IWorkbook book = books[ i ];
          
          if( !book.Saved ) return false;
        }

        return true;
      }
    }
    public bool IsFormulaParsed
    {
        get { return m_isFormulaparsed; }
        set { m_isFormulaparsed = value; }

    }
    /// <summary>
    /// Returns standard row height in units used by RowRecord.
    /// </summary>
    public int StandardHeightInRowUnits
    {
      get
      {
        return m_iStandardRowHeight;
      }
    }
    /// <summary>
    /// Indicates whether evaluation license expired.
    /// </summary>
    internal bool EvalExpired
    {
      get
      {
        return m_bEvalExpired;
      }
      set
      {
        m_bEvalExpired = value;
      }
    }
    #endregion

    #region IApplication Properties
    /// <summary>
    /// Returns a Range object that represents the active cell in the
    /// active window (the window on top) or in the specified window.
    /// If the window isn't displaying a worksheet, this property fails.
    /// Read-only.
    /// </summary>
    public IRange       ActiveCell
    {
      get
      {
        return m_ActiveCell;
      }
    }
    /// <summary>
    /// Returns an object that represents the active sheet (the sheet on
    /// top) in the active workbook or in the specified window or workbook.
    /// Returns NULL (Nothing in VB) if no sheet is active. Read-only.
    /// </summary>
    public IWorksheet   ActiveSheet
    {
      get
      {
        return m_ActiveSheet as IWorksheet;
      }
    }
    /// <summary>
    /// Returns a Workbook object that represents the workbook in the active
    /// window (the window on top). Read-only. Returns NULL (Nothing in VB) if there are
    /// no windows open or if either the Info window or the Clipboard window
    /// is the active window.
    /// </summary>
    public IWorkbook    ActiveWorkbook
    {
      get
      {
        return m_ActiveBook;
      }
    }
    /// <summary>
    /// Used without an object qualifier, this property returns an Application
    /// object that represents the Microsoft Excel application. Used with an
    /// object qualifier, this property returns an Application object that
    /// represents the creator of the specified object (you can use this property
    /// with an OLE Automation object to return that object's application).
    /// Read-only.
    /// </summary>
    public IApplication Application
    {
      get
      {
        return this;
      }
    }
    /// <summary>
    /// Returns a Workbooks collection that represents all the open workbooks.
    /// Read-only.
    /// </summary>
    public IWorkbooks   Workbooks
    {
      [DebuggerStepThrough]
      get
      {
        return m_workbooks;
      }
    }
    /// <summary>
    /// For an Application object, returns a Sheets collection that represents
    /// all worksheets in the active workbook. For a Workbook object,
    /// returns a Sheets collection that represents all the worksheets in the
    /// specified workbook. Read-only Sheets object.
    /// </summary>
    public IWorksheets  Worksheets
    {
      get
      {
        if( ActiveWorkbook != null )
          return ActiveWorkbook.Worksheets;

        return null;
      }
    }
    /// <summary>
    /// Returns the parent object for the specified object. Read-only.
    /// </summary>
    public object       Parent
    {
      [DebuggerStepThrough]
      get
      {
        return null;
      }
    }
    /// <summary>
    /// Returns a Range object that represents a cell or a range of cells.
    /// Read-only.
    /// </summary>
    public IRange       Range
    {
      get
      {
        return CreateRange( this );
      }
    }
    /// <summary>
    /// All data entered after this property is set to True will be formatted
    /// with the number of fixed decimal places set by the FixedDecimalPlaces
    /// property. Read/write Boolean.
    /// </summary>
    public bool         FixedDecimal
    {
      get
      {
        return m_bFixedDecimal;
      }
      set
      {
        m_bFixedDecimal = value;
      }
    }
    /// <summary>
    /// True (default) if the system separators of Microsoft Excel are
    /// enabled. Read/write Boolean.
    /// </summary>
    public bool         UseSystemSeparators
    {
      get
      {
        return m_bUseSystemSep;
      }
      set
      {
        m_bUseSystemSep = value;
      }
    }
    /// <summary>
    /// Returns or sets the standard font size, in points. Read/write.
    /// </summary>
    public double       StandardFontSize
    {
      get
      {
        return m_dbStandardFontSize;
      }
      set
      {
        m_dbStandardFontSize = value;
      }
    }
    /// <summary>
    /// Returns the Microsoft Excel build number. Read-only.
    /// </summary>
    public int          Build
    {
      [DebuggerStepThrough]
      get
      {
        return DEF_BUILD_NUMBER;
      }
    }
    /// <summary>
    /// Returns or sets the number of fixed decimal places used when
    /// the FixedDecimal property is set to True. Read/write.
    /// </summary>
    public int          FixedDecimalPlaces
    {
      get
      {
        return m_iFixedDecimalPlaces;
      }
      set
      {
        m_iFixedDecimalPlaces = value;
      }
    }
    /// <summary>
    /// Returns or sets the number of sheets that Microsoft Excel
    /// automatically inserts into new workbooks. Read/write Long.
    /// </summary>
    public int          SheetsInNewWorkbook
    {
      get
      {
        return m_iSheetsInNewWorkbook;
      }
      set
      {
        if( value < 1 )
          throw new ArgumentException( "Sheets in workbook cannot be less then 1" );

        m_iSheetsInNewWorkbook = value;
      }
    }
    /// <summary>
    /// Sets or returns the character used for the decimal separator as a
    /// String. Read/write.
    /// </summary>
    public string       DecimalSeparator
    {
      get
      {
        return m_strDecimalSeparator;
      }
      set
      {
        m_strDecimalSeparator = value;
        m_currentCulture.NumberFormat.NumberDecimalSeparator = m_strDecimalSeparator;
        m_currentCulture.NumberFormat.PercentDecimalSeparator = m_strDecimalSeparator;
        m_currentCulture.NumberFormat.CurrencyDecimalSeparator = m_strDecimalSeparator;
      }
    }
    /// <summary>
    /// Returns or sets the default path that Microsoft Excel uses when it
    /// opens files. Read/write String.
    /// </summary>
    public string       DefaultFilePath
    {
      get
      {
#if ( WINRT )
          return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
        return Environment.CurrentDirectory;
#endif
      }
#if !SILVERLIGHT && !WINRT && !WP
      set
      {
        Environment.CurrentDirectory = value;
      }
#endif
    }
    ///// <summary>
    ///// Returns the complete path to the application, excluding the
    ///// final separator and name of the application. Read-only String.
    ///// </summary>
    //public string       Path
    //{
    //  get
    //  {
    //    string strPath = System.IO.Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location );
    //    return strPath;
    //  }
    //}
#if !(WINRT )
    /// <summary>
    /// Returns the path separator character ("\"). Read-only String.
    /// </summary>
    public string       PathSeparator
    {
      get
      {
        return DEF_PATH_SEPARATOR;
      }
    }
#endif
    /// <summary>
    /// Returns or sets the name of the standard font. Read/write String.
    /// </summary>
    public string       StandardFont
    {
      get
      {
        return m_strStandardFont;
      }
      set
      {
        m_strStandardFont = value;
      }
    }
    /// <summary>
    /// Sets or returns the character used for the thousands separator
    /// as a String. Read/write
    /// </summary>
    public string       ThousandsSeparator
    {
      get
      {
        return m_strThousandsSeparator;
      }
      set
      {
        m_strThousandsSeparator = value;
        m_currentCulture.NumberFormat.NumberGroupSeparator = m_strThousandsSeparator;
        m_currentCulture.NumberFormat.PercentGroupSeparator = m_strThousandsSeparator;
        m_currentCulture.NumberFormat.CurrencyGroupSeparator = m_strThousandsSeparator;
      }
    }
    /// <summary>
    /// Returns or sets the name of the current user. Read/write String.
    /// </summary>
    public string       UserName
    {
      get
      {
        return m_strUserName;
      }
      set
      {
        m_strUserName = value;
      }
    }
    /// <summary>
    /// For the Application object, it always returns "Microsoft Excel". For
    /// the CubeField object, the name of the specified field. For the Style
    /// object, the name of the specified style. Read-only String.
    /// </summary>
    public string       Value
    {
      [DebuggerStepThrough]
      get
      {
        return DEF_VALUE;
      }
    }
    /// <summary>
    /// When this property is set to True, if some cells have reference 
    /// to the same style, changes will influence all these cells.
    /// Default value: FALSE.
    /// </summary>
    public bool         ChangeStyleOnCellEdit
    {
      get
      {
        return m_bChangeStyle;
      }
      set
      {
        if( value != m_bChangeStyle )
        {
          if( m_workbooks.Count > 0 )
          {
            throw new ArgumentException( "ChangeStyleOnCellEdit property can be changed " +
              "only when Application does not contains any workbook" );
          }

          m_bChangeStyle = value;
        }
      }
    }
    /// <summary>
    /// Flags that control behavior of workbook save methods. Each flag controls 
    /// one aspect of save code. Can be set one or more flags influencing
    /// the output produced.
    /// </summary>
    public SkipExtRecords SkipOnSave
    {
      get
      {
        return m_enSkipExtRecords;
      }
      set
      {
        m_enSkipExtRecords = value;
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) height of all the rows in the worksheet,
    /// in points. Read/write Double.
    /// </summary>
    public double       StandardHeight
    {
      get
      {
        return m_iStandardRowHeight / 20.0;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "StandardHeight" );

        m_iStandardRowHeight = ( int )( value * 20 );
        m_bStandartRowHeightFlag = true;
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) height option flag, which defines that
    /// standard (default) row height and book default font height do not match.
    /// Read/write Bool.
    /// </summary>
    public bool         StandardHeightFlag
    {
      get
      {
        return m_bStandartRowHeightFlag;
      }
      set
      {
        m_bStandartRowHeightFlag = value;
      }
    }
    /// <summary>
    /// Returns or sets the standard (default) width of all the columns in the
    /// worksheet. Read/write Double.
    /// </summary>
    public double       StandardWidth
    {
      get
      {
        return m_dStandardColWidth;
      }
      set
      {
        if( m_dStandardColWidth != value )
        {
          m_dStandardColWidth = value;
          //          SetChanged();
        }
      }
    }
    /// <summary>
    /// Indicates whether to optimize fonts count. This option will
    /// take effect only on workbooks that will be added after setting
    /// this property.
    /// WARNING: Setting this property to True can decrease performance significantly,
    /// but will reduce resulting file size.
    /// </summary>
    public bool         OptimizeFonts
    {
      get
      {
        return m_bOptimizeFonts;
      }
      set
      {
        m_bOptimizeFonts = value;
      }
    }
    /// <summary>
    /// Indicates whether to optimize Import data. This option will
    /// take effect only on Import methods that are available with the worksheet
    /// WARNING: Setting this property to True can decrease memory significantly,
    /// but will increase the performance of data import .
    /// </summary>
    public bool OptimizeImport
    {
        get
        {
            return m_bOptimizeImport;
        }
        set
        {
            m_bOptimizeImport = value;
        }
    }
    /// <summary>
    /// Gets / sets row separator for array parsing.
    /// </summary>
    public char         RowSeparator
    {
      get
      {
        return m_chRowSeparator;
      }
      set
      {
        m_chRowSeparator = value;
      }
    }
    /// <summary>
    /// Formula arguments separator.
    /// </summary>
    public char         ArgumentsSeparator
    {
      get
      {
        return m_chArgumentSeparator;
      }
      set
      {
        m_chArgumentSeparator = value;
        m_currentCulture.TextInfo.ListSeparator = Convert.ToString(m_chArgumentSeparator);
      }
    }
    /// <summary>
    /// Represents CSV Separator.
    /// </summary>
    public string       CSVSeparator
    {
      get
      {
        return m_strCSVSeparator;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty" );

        m_strCSVSeparator = value;
      }
    }
    /// <summary>
    /// Indicates is use unsafe code.
    /// </summary>
    [ Obsolete() ]
    public bool         UseNativeOptimization
    {
      get
      {
        return UseUnsafeCodeStatic;
      }
      set
      {
        UseUnsafeCodeStatic = value;
      }
    }
    /// <summary>
    /// Indicates whether to try fast record parsing.
    /// </summary>
    public bool UseFastRecordParsing
    {
      get
      {
        return m_bUseFastRecordParsing;
      }
      set
      {
        m_bUseFastRecordParsing = value;
      }
    }
    /// <summary>
    /// Gets / sets memory allocation block for single row. Each row will allocate memory block
    /// that can be divided on this number. Smaller value means smaller memory usage but slower
    /// speed when changing cell's value. Default value is 128. That is enough to allocate 9 string records,
    /// or 9 integer numbers (or floating numbers with 1 or 2 digits after decimal point) or 7 double numbers.
    /// </summary>
    public int RowStorageAllocationBlockSize
    {
      get
      {
        return m_iRowStorageBlock;
      }
      set
      {
        if( value <= 0 )
          throw new ArgumentOutOfRangeException( "RowStorageAllocationBlock", "Property must be larger than zero." );

        m_iRowStorageBlock = value;
      }
    }
    /// <summary>
    /// Indicates whether XlsIO should delete destination file before saving into it.
    /// Default value is TRUE.
    /// </summary>
    public bool DeleteDestinationFile
    {
      get
      {
        return m_bDeleteDestinationFile;
      }
      set
      {
        m_bDeleteDestinationFile = value;
      }
    }
    /// <summary>
    /// Gets / sets default excel version. This value is used in create methods.
    /// </summary>
    public ExcelVersion DefaultVersion
    {
      get
      {
        return m_defaultVersion;
      }
      set
      {
        m_defaultVersion = value;
        if (value != ExcelVersion.Excel97to2003)
        {
            m_iStandardRowHeight = DefaultRowHeightXlsx;
            m_bStandartRowHeightFlag = true;
        }
      }
    }
    /// <summary>
    /// Indicates whether we should use native storage (standard windows COM object)
    /// or our .Net implementation to open excel 97-2003 files.
    /// </summary>
    public bool UseNativeStorage
    {
      get
      {
        return !m_bNetStorage;
      }
      set
      {
        m_bNetStorage = !value;
      }
    }
    /// <summary>
    /// Changes data provider type for all operations after it. Static property.
    /// </summary>
    public ExcelDataProviderType DataProviderType
    {
      get
      {
        return m_dataType;
      }
      set
      {
        m_dataType = value;
      }
    }
    /// <summary>
    /// Compression level for workbooks serialization.
    /// </summary>
    public CompressionLevel? CompressionLevel
    {
      get
      {
        return m_compressionLevel;
      }
      set
      {
        m_compressionLevel = value;
      }
    }
    /// <summary>
    /// Indicates whether to preserve the datatypes for the CSV file formats.
    /// </summary>
    public bool PreserveCSVDataTypes
    {
        get
        {
            return m_preserveTypes;
        }
        set
        {
            m_preserveTypes = value;
        }
    }
    internal StringEnumerations StringEnum
    {
        get
        {            
            return m_stringEnum;
        }
    }
#if ((SyncfusionFramework4_0 || SyncfusionFramework4_5) && !SILVERLIGHT && !WINRT && !WP)
    /// <summary>
    /// Represents the Chart to Image Converter instance.
	/// Returns null if it is not instantiated.
    /// </summary>
    public IChartToImageConverter ChartToImageConverter
    {
        get
        {
            return m_chartToImageConverter;
        }
        set
        {
            m_chartToImageConverter = value;
        }
    }
#endif
    #endregion

    #region Class Initialize/Finalize methods
      #if ( WINRT )
      internal static void InitAssemblyTypes()
        {
         IEnumerator<TypeInfo> typeInfos= typeof(ApplicationImpl).GetTypeInfo().Assembly.DefinedTypes.GetEnumerator();
         IList<Type> types = new List<Type>();
         while (typeInfos.MoveNext())
         {
             types.Add(typeInfos.Current.AsType());
         }
         AssemblyTypes = types.ToArray<Type>();
        }
#endif
    /// <summary>
    /// Static constructor.
    /// </summary>
    static ApplicationImpl()
    {
#if !SILVERLIGHT && !WINRT && !WP
      Bitmap   _bmp = new Bitmap( 1, 1 );
      Graphics m_graphics = Graphics.FromImage( _bmp );

      PointF[] points = new PointF[]{ new PointF( 1, 1 ) };

      GraphicsContainer cont = m_graphics.BeginContainer(
        new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ),
        GraphicsUnit.Pixel );

      m_graphics.PageUnit = GraphicsUnit.Inch;
      m_graphics.TransformPoints( CoordinateSpace.Device, CoordinateSpace.Page, points );
      m_graphics.EndContainer( cont );

      float DEF_INCH_PIXEL = points[ 0 ].X;

      s_arrProportions = new double[]
      {
        DEF_INCH_PIXEL / 75.0d,          // Display
        DEF_INCH_PIXEL / 300.0d,         // Document
        DEF_INCH_PIXEL,               // Inch
        DEF_INCH_PIXEL / 25.4d,       // Millimeter
        DEF_INCH_PIXEL / 2.54d,       // Centimeter
        1,                            // Pixel
        DEF_INCH_PIXEL / 72.0d,          // Point
        DEF_INCH_PIXEL / 72.0d / 12700,  // EMU
      };

      m_graphics.Dispose();
      _bmp.Dispose();
      m_graphics = null;
      _bmp = null;
#else
      s_arrProportions = new double[]
      {
        1.28,                   // Display
        0.32,                   // Document
        96,                     // Inch
        3.7795275590551185,     // Millimeter
        37.795275590551178,     // Centimeter
        1,                      // Pixel
        4 / 3.0,                // Point
        0.00010498687664041994, // EMU
      };

#endif

      MinCellSize.Height = ( float )ConvertToPixels( MinCellSize.Height, MeasureUnits.Point );


    }
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ApplicationImpl(ExcelEngine excelEngine)
    {
        m_parent = excelEngine;
      // Set system User name.
#if !SILVERLIGHT && !WINRT && !WP
      if( ExcelEngine.IsSecurityGranted )
        m_strUserName = Environment.UserName;
      Bitmap _bmp = new Bitmap(1, 1);
      m_graphics = Graphics.FromImage(_bmp);
      this.dpiX = (int)m_graphics.DpiX;
      this.dpiY = (int)m_graphics.DpiY;
#endif

      m_builtInStyleInfo = new StyleImpl.StyleSettings[m_defaultStyleNames.Length];
      // Set default font size.
      m_dbStandardFontSize = DEF_STANDARD_FONT_SIZE;

      // Default number of decimal digits after separator.
      m_iFixedDecimalPlaces = DEF_FIXED_DECIMAL_PLACES;

      // Number of sheets that are automatically created for new workbook.
      m_iSheetsInNewWorkbook = DEF_SHEETS_IN_NEW_WORKBOOK;


        m_standredCulture = CultureInfo.InvariantCulture;

        m_currentCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);

        

      // Set system default Decimal separator.
      m_strDecimalSeparator = m_currentCulture.NumberFormat.NumberDecimalSeparator;

      // Set system default font name.
      m_strStandardFont =
        "Tahoma";
        //SystemInformation.MenuFont.Name;

      // Thousands separator also known as Group separator.
      m_strThousandsSeparator = m_currentCulture.NumberFormat.NumberGroupSeparator;

      m_chArgumentSeparator = Convert.ToChar(m_currentCulture.TextInfo.ListSeparator);
#if !SILVERLIGHT && !WINRT && !WP
      m_graphics.PageUnit = GraphicsUnit.Pixel;
#endif
      InitializeCollection();
      InitializeStyleCollections();
      InitializePageSetup();
    }

    private void InitializePageSetup()
    {
        m_dicPaperSizeTable = new Dictionary<int, PageSetupBaseImpl.PaperSizeEntry>();
        m_dicPaperSizeTable.Add(1, new PageSetupBaseImpl.PaperSizeEntry(8.5, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(2, new PageSetupBaseImpl.PaperSizeEntry(8.5, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(3, new PageSetupBaseImpl.PaperSizeEntry(11, 17, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(4, new PageSetupBaseImpl.PaperSizeEntry(17, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(5, new PageSetupBaseImpl.PaperSizeEntry(8.5, 14, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(6, new PageSetupBaseImpl.PaperSizeEntry(5.5, 8.5, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(7, new PageSetupBaseImpl.PaperSizeEntry(7.25, 10.5, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(8, new PageSetupBaseImpl.PaperSizeEntry(297, 420, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(9, new PageSetupBaseImpl.PaperSizeEntry(210, 297, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(10, new PageSetupBaseImpl.PaperSizeEntry(210, 297, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(11, new PageSetupBaseImpl.PaperSizeEntry(148, 210, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(12, new PageSetupBaseImpl.PaperSizeEntry(257, 368, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(13, new PageSetupBaseImpl.PaperSizeEntry(182, 257, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(14, new PageSetupBaseImpl.PaperSizeEntry(8.5, 13, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(15, new PageSetupBaseImpl.PaperSizeEntry(215, 275, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(16, new PageSetupBaseImpl.PaperSizeEntry(10, 14, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(17, new PageSetupBaseImpl.PaperSizeEntry(11, 17, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(18, new PageSetupBaseImpl.PaperSizeEntry(8.5, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(19, new PageSetupBaseImpl.PaperSizeEntry(3.875, 8.875, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(20, new PageSetupBaseImpl.PaperSizeEntry(4.125, 9.5, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(21, new PageSetupBaseImpl.PaperSizeEntry(4.5, 10.375, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(22, new PageSetupBaseImpl.PaperSizeEntry(4.75, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(23, new PageSetupBaseImpl.PaperSizeEntry(5, 11.5, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(24, new PageSetupBaseImpl.PaperSizeEntry(17, 22, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(25, new PageSetupBaseImpl.PaperSizeEntry(22, 34, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(26, new PageSetupBaseImpl.PaperSizeEntry(34, 44, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(27, new PageSetupBaseImpl.PaperSizeEntry(110, 220, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(28, new PageSetupBaseImpl.PaperSizeEntry(162, 229, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(29, new PageSetupBaseImpl.PaperSizeEntry(324, 458, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(30, new PageSetupBaseImpl.PaperSizeEntry(229, 324, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(31, new PageSetupBaseImpl.PaperSizeEntry(114, 162, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(32, new PageSetupBaseImpl.PaperSizeEntry(114, 229, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(33, new PageSetupBaseImpl.PaperSizeEntry(250, 353, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(34, new PageSetupBaseImpl.PaperSizeEntry(176, 250, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(35, new PageSetupBaseImpl.PaperSizeEntry(125, 176, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(36, new PageSetupBaseImpl.PaperSizeEntry(110, 230, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(37, new PageSetupBaseImpl.PaperSizeEntry(3.875, 7.5, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(38, new PageSetupBaseImpl.PaperSizeEntry(3.625, 6.5, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(39, new PageSetupBaseImpl.PaperSizeEntry(14.875, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(40, new PageSetupBaseImpl.PaperSizeEntry(8.5, 12, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(41, new PageSetupBaseImpl.PaperSizeEntry(8.5, 13, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(42, new PageSetupBaseImpl.PaperSizeEntry(250, 353, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(43, new PageSetupBaseImpl.PaperSizeEntry(100, 148, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(44, new PageSetupBaseImpl.PaperSizeEntry(9, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(45, new PageSetupBaseImpl.PaperSizeEntry(10, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(46, new PageSetupBaseImpl.PaperSizeEntry(15, 11, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(47, new PageSetupBaseImpl.PaperSizeEntry(220, 220, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(50, new PageSetupBaseImpl.PaperSizeEntry(9.5, 12, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(51, new PageSetupBaseImpl.PaperSizeEntry(9.5, 15, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(52, new PageSetupBaseImpl.PaperSizeEntry(11.6875, 18, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(53, new PageSetupBaseImpl.PaperSizeEntry(235, 322, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(54, new PageSetupBaseImpl.PaperSizeEntry(8.5, 11, MeasureUnits.Inch));
        m_dicPaperSizeTable.Add(55, new PageSetupBaseImpl.PaperSizeEntry(210, 297, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(56, new PageSetupBaseImpl.PaperSizeEntry(9.5, 12, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(57, new PageSetupBaseImpl.PaperSizeEntry(227, 356, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(58, new PageSetupBaseImpl.PaperSizeEntry(305, 487, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(59, new PageSetupBaseImpl.PaperSizeEntry(8.5, 12.6875, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(60, new PageSetupBaseImpl.PaperSizeEntry(210, 330, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(61, new PageSetupBaseImpl.PaperSizeEntry(148, 210, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(62, new PageSetupBaseImpl.PaperSizeEntry(182, 257, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(63, new PageSetupBaseImpl.PaperSizeEntry(322, 445, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(64, new PageSetupBaseImpl.PaperSizeEntry(174, 235, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(65, new PageSetupBaseImpl.PaperSizeEntry(201, 276, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(66, new PageSetupBaseImpl.PaperSizeEntry(420, 594, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(67, new PageSetupBaseImpl.PaperSizeEntry(297, 420, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(68, new PageSetupBaseImpl.PaperSizeEntry(322, 445, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(69, new PageSetupBaseImpl.PaperSizeEntry(200, 148, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(70, new PageSetupBaseImpl.PaperSizeEntry(105, 148, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(75, new PageSetupBaseImpl.PaperSizeEntry(11, 8.5, MeasureUnits.Inch));

        m_dicPaperSizeTable.Add(76, new PageSetupBaseImpl.PaperSizeEntry(420, 297, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(77, new PageSetupBaseImpl.PaperSizeEntry(297, 210, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(78, new PageSetupBaseImpl.PaperSizeEntry(210, 148, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(79, new PageSetupBaseImpl.PaperSizeEntry(364, 257, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(80, new PageSetupBaseImpl.PaperSizeEntry(257, 182, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(81, new PageSetupBaseImpl.PaperSizeEntry(148, 100, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(82, new PageSetupBaseImpl.PaperSizeEntry(148, 200, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(83, new PageSetupBaseImpl.PaperSizeEntry(148, 105, MeasureUnits.Millimeter));

        m_dicPaperSizeTable.Add(88, new PageSetupBaseImpl.PaperSizeEntry(128, 182, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(89, new PageSetupBaseImpl.PaperSizeEntry(182, 128, MeasureUnits.Millimeter));
        m_dicPaperSizeTable.Add(90, new PageSetupBaseImpl.PaperSizeEntry(12, 11, MeasureUnits.Inch));
    }

    private void InitializeStyleCollections()
    {
       
    
      #region Excel 97 Styles
      int index = 0;
      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( null, null ); //"Normal",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"RowLevel_",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"ColLevel_",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Comma",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Currency",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Percent",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Comma [0]",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Currency [0]",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Hyperlink",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Followed Hyperlink",
      index++;
      #endregion

      FillImpl fill;
      StyleImpl.FontSettings font;
      StyleImpl.BorderSettings borders;

      #region Note
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xFF, 0xFF, 0xCC ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0xB2, 0xB2, 0xB2 ),
        ExcelLineStyle.Thin );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(fill, font, borders); //"Note",
      index++;
      #endregion

      #region Warning Text
      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0xFF, 0, 0 ) );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font); //"Warning Text",
      index++;
      #endregion

      #region Unknown styles
      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Emphasis 1",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"Emphasis 2",
      index++;

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, null); //"",
      index++;
      #endregion

      #region Title
      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 3 ),
        18,
        FontStyle.Bold,
        "Cambria" );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font); //"Title",
      index++;
      #endregion

      #region Heading 1
      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 3 ),
        15,
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        new ColorObject( ColorType.Theme, 4 ),
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.Thick );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font, borders); //"Heading 1",
      index++;
      #endregion

      #region Heading 2
      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 3 ),
        13,
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        new ColorObject( ColorType.Theme, 4, 0.499984740745262 ),
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.Thick );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font, borders); //"Heading 2",
      index++;
      #endregion

      #region Heading 3
      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(
        null,
        new StyleImpl.FontSettings(new ColorObject(ColorType.Theme, 3), FontStyle.Bold),
        new StyleImpl.BorderSettings(new ColorObject(ColorType.Theme, 4, 0.39997558519241921),
          ExcelLineStyle.None, ExcelLineStyle.None, ExcelLineStyle.None, ExcelLineStyle.Medium ) ); //"Heading 3",
      index++;
      #endregion

      #region Heading 4
      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 3 ),
        FontStyle.Bold );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font); //"Heading 4",
      index++;
      #endregion

      #region Input
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xFF, 0xCC, 0x99 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(Color.FromArgb(0xFF, 0x3F, 0x3F, 0x76));

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0x7F, 0x7F, 0x7F ),
        ExcelLineStyle.Thin );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(fill, font, borders); //"Input",
      index++;
      #endregion

      #region Output
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xF2, 0xF2, 0xF2 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0x3F, 0x3F, 0x3F ),
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0x3F, 0x3F, 0x3F ),
        ExcelLineStyle.Thin );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(fill, font, borders); //"Output",
      index++;
      #endregion

      #region Calculation
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xF2, 0xF2, 0xF2 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0xFA, 0x7D, 0 ),
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0x7F, 0x7F, 0x7F ),
        ExcelLineStyle.Thin );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(fill, font, borders); //"Calculation",
      index++;
      #endregion

      #region Check Cell
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xA5, 0xA5, 0xA5 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ),
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0x3F, 0x3F, 0x3F ),
        ExcelLineStyle.Double );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(fill, font, borders); //"Check Cell",
      index++;
      #endregion

      #region Linked Cell
      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0xFA, 0x7D, 0x00 ) );

      borders = new StyleImpl.BorderSettings(
        Color.FromArgb( 0xFF, 0xFF, 0x80, 0x01 ),
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.Double );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font, borders); //"Linked Cell",
      index++;
      #endregion

      #region Total
      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ),
        FontStyle.Bold );

      borders = new StyleImpl.BorderSettings(
        new ColorObject( ColorType.Theme, 4 ),
        ExcelLineStyle.None,
        ExcelLineStyle.None,
        ExcelLineStyle.Thin,
        ExcelLineStyle.Double );

      BuiltInStyleInfo[index] = new StyleImpl.StyleSettings(null, font, borders); //"Total",
      index++;
      #endregion

      #region Good
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xC6, 0xEF, 0xCE ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0x00, 0x61, 0x00 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Good",
      index++;
      #endregion

      #region Bad
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xFF, 0xC7, 0xCE ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0x9C, 0x00, 0x06 ) );
      
      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Bad",
      index++;
      #endregion

      #region Neutral
      fill = new FillImpl(
        ExcelPattern.Solid,
        Color.FromArgb( 0xFF, 0xFF, 0xEB, 0x9C ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0x9C, 0x65, 0x00 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Neutral",
      index++;
      #endregion

      #region Accent1
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 4 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font );//"Accent1"
      index++;
      #endregion

      #region 20% - Accent1
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 4, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"20% - Accent1",
      index++;
      #endregion

      #region 40% - Accent1
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 4, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );
      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent1",
      index++;
      #endregion

      #region 60% - Accent1
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 4, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent1",
      index++;
      #endregion

      #region Accent2
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 5 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Accent2",
      index++;
      #endregion

      #region 20% - Accent2
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 5, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings( new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"20% - Accent2",
      index++;
      #endregion

      #region 40% - Accent2
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 5, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent2",
      index++;
      #endregion

      #region 60% - Accent2
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 5, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent2",
      index++;
      #endregion

      #region Accent3
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 6 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Accent3",
      index++;
      #endregion

      #region 20% - Accent3
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 6, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //20% - Accent3",
      index++;
      #endregion

      #region 40% - Accent3
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 6, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent3",
      index++;
      #endregion

      #region 60% - Accent3
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 6, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent3",
      index++;
      #endregion

      #region Accent4
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 7 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Accent4",
      index++;
      #endregion

      #region 20% - Accent4
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 7, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"20% - Accent4",
      index++;
      #endregion

      #region 40% - Accent4
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 7, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent4",
      index++;
      #endregion

      #region 60% - Accent4
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 7, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent4",
      index++;
      #endregion

      #region Accent5
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 8 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Accent5",
      index++;
      #endregion

      #region 20% - Accent5
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 8, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"20% - Accent5",
      index++;
      #endregion

      #region 40% - Accent5
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 8, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent5",
      index++;
      #endregion

      #region 60% - Accent5
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 8, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent5",
      index++;
      #endregion

      #region Accent6
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 9 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"Accent6",
      index++;
      #endregion

      #region 20% - Accent6
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 9, 0.79998168889431442 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings( new ColorObject( ColorType.Theme, 1 ) );
      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"20% - Accent6",
      index++;
      #endregion

      #region 40% - Accent6
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 9, 0.59999389629810485 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 1 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"40% - Accent6",
      index++;
      #endregion

      #region 60% - Accent6
      fill = new FillImpl(
        ExcelPattern.Solid,
        new ColorObject( ColorType.Theme, 9, 0.39997558519241921 ),
        ColorExtension.Empty );

      font = new StyleImpl.FontSettings(
        new ColorObject( ColorType.Theme, 0 ) );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( fill, font ); //"60% - Accent6",
      index++;
      #endregion

      #region Explanatory Text
      font = new StyleImpl.FontSettings(
        Color.FromArgb( 0xFF, 0x7F, 0x7F, 0x7F ),
        FontStyle.Italic );

      BuiltInStyleInfo[ index ] = new StyleImpl.StyleSettings( null, font ); //"Explanatory Text",
      index++;
      #endregion
    }
    /// <summary>
    /// This method initializes inner collections of workbooks.
    /// </summary>
    protected void InitializeCollection()
    {
      m_workbooks = new WorkbooksCollection( Application, this );
    }
    #endregion

    #region IApplication methods implementation
    /// <summary>
    /// Converts a measurement from centimeters to points
    /// (one point equals 0.035 centimeters).
    /// </summary>
    /// <param name="Centimeters">Value in centimeters to convert.</param>
    /// <returns>Converted value.</returns>
    public double CentimetersToPoints( double Centimeters )
    {
      return ConvertUnits( ( float )Centimeters, MeasureUnits.Centimeter, MeasureUnits.Point );
    }

    /// <summary>
    /// Converts a measurement from inches to points.
    /// </summary>
    /// <param name="Inches">Value in inches.</param>
    /// <returns>Converted value in points.</returns>
    public double InchesToPoints( double Inches )
    {
      return ConvertUnits( ( float )Inches, MeasureUnits.Inch, MeasureUnits.Point );
    }
#if !(WINRT || WP)
    /// <summary>
    /// Saves changes to the active workbook.
    /// </summary>
    /// <param name="Filename">File name of result file.</param>
    public void Save( string Filename )
    {
      if( ActiveWorkbook != null )
      {
        ActiveWorkbook.SaveAs( Filename );
      }
    }
#endif
    #endregion

    #region Create methods

    #region Workbook
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, ExcelVersion version )
    {
      WorkbookImpl book = new WorkbookImpl( this, parent, version );
      CheckDefaultFont(book);
      return book;
    }
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="stream">Stream to read.</param>
    /// <param name="separator">Current separator</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="version">Excel version.</param>
    /// <param name="fileName">Filename is used to generate worksheet name</param>
    /// <returns>Created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, Stream stream, string separator,
      int row, int column, ExcelVersion version, string fileName, Encoding encoding )
    {
      return new WorkbookImpl( this, parent, stream, separator, row, column, version, fileName, encoding );
    }
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, Stream stream, ExcelVersion version, ExcelParseOptions options )
    {
      return new WorkbookImpl( this, parent, stream, options, version );
    }
    
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, Stream stream,
      ExcelParseOptions options, ExcelVersion version )
    {
      return new WorkbookImpl( this, parent, stream, options, version );
    }
#if !(WINRT )
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="strTemplateFile">Name of the file with workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bReadOnly">Indicates whether to open workbook in read-only mode.</param>
    /// <param name="password">Password to decrypt.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, string strTemplateFile,
      ExcelParseOptions options, bool bReadOnly, string password, ExcelVersion version )
    {
      return new WorkbookImpl( this, parent, strTemplateFile, options, bReadOnly, password, version );
    }
#endif
    /// <summary>
    /// Creates the workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="stream">Stream with workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bReadOnly">Indicates whether to open workbook in read-only mode.</param>
    /// <param name="password">Password to decrypt.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook(object parent, Stream stream,
    ExcelParseOptions options, bool bReadOnly, string password, ExcelVersion version)
    {
        return new WorkbookImpl(this, parent, stream, options, bReadOnly, password, version);
    }
    
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="sheetsQuantity">Number of sheets in the new workbook.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, int sheetsQuantity, ExcelVersion version )
    {
      WorkbookImpl book = new WorkbookImpl( this, parent, sheetsQuantity, version );
      CheckDefaultFont(book);
      return book;
    }
#if !(WINRT )
    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="strTemplateFile">Name of the file with workbook.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, string strTemplateFile, ExcelVersion version )
    {
      return new WorkbookImpl( this, parent, strTemplateFile, version );
    }

    /// <summary>
    /// Creates a new Workbook.
    /// </summary>
    /// <param name="parent">Parent object for the new workbook.</param>
    /// <param name="strTemplateFile">Name of the file with workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Newly created workbook.</returns>
    virtual public WorkbookImpl CreateWorkbook( object parent, string strTemplateFile
      , ExcelParseOptions options, ExcelVersion version )
    {
      return new WorkbookImpl( this, parent, strTemplateFile, options, version );
    }
#endif
    #endregion

    #region Worksheet
    /// <summary>
    /// Creates a new  Worksheet.
    /// </summary>
    /// <param name="parent">Parent object for the new worksheet.</param>
    /// <returns>Newly created worksheet.</returns>
    virtual public WorksheetImpl CreateWorksheet( object parent )
    {
      return new WorksheetImpl( this, parent );
    }
    
    /// <summary>
    /// Creates a new Worksheet.
    /// </summary>
    /// <param name="parent">Parent object for the new worksheet.</param>
    /// <param name="reader">Reader with worksheet data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes used in ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <returns>Newly created worksheet.</returns>
    [ CLSCompliant( false ) ]
    virtual public WorksheetImpl CreateWorksheet( object parent, BiffReader reader
      , ExcelParseOptions options, bool bSkipParsing, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      return new WorksheetImpl( this, parent, reader, options, bSkipParsing,
        hashNewXFormatIndexes, decryptor );
    }
    #endregion

    #region Range
    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <returns>Newly created range.</returns>
    virtual public RangeImpl CreateRange( object parent )
    {
      return new RangeImpl( this, parent );
    }

    /// <summary>
    /// Creates new range for a single cell.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="col">Column index for the new range.</param>
    /// <param name="row">Row index for the new range.</param>
    /// <returns>Newly created range.</returns>
    virtual public RangeImpl CreateRange( object parent, int col, int row )
    {
      return new RangeImpl( this, parent, col, row );
    }
    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="data">Array of BiffRecordRaws that contains range data.</param>
    /// <param name="i">Index of the range record.</param>
    /// <returns>Newly created range.</returns>
    [ CLSCompliant( false ) ]
    virtual public RangeImpl CreateRange( object parent, BiffRecordRaw[] data, ref int i )
    {
      return new RangeImpl( this, parent, data, i );
    }
    
    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="data">Array of BiffRecordRaws that contains range data.</param>
    /// <param name="i">Index of the range record.</param>
    /// <param name="ignoreStyles">Indicates whether cell styles should be ignored.</param>
    /// <returns>Newly created range.</returns>
    [ CLSCompliant( false ) ]
    virtual public RangeImpl CreateRange( object parent,  BiffRecordRaw[] data, ref int i
      , bool ignoreStyles )
    {
      return new RangeImpl( this, parent, data, ref i, ignoreStyles );
    }
    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="data">Array of BiffRecordRaws that contains range data.</param>
    /// <param name="i">Index of the range record.</param>
    /// <param name="ignoreStyles">Indicates whether cell styles should be ignored.</param>
    /// <returns>Newly created range.</returns>
    virtual public RangeImpl CreateRange( object parent,  List<BiffRecordRaw> data, ref int i
      , bool ignoreStyles )
    {
      return new RangeImpl( this, parent, data, ref i, ignoreStyles );
    }
    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="firstCol">First column of the range.</param>
    /// <param name="firstRow">First row of the range.</param>
    /// <param name="lastCol">Last column of the range.</param>
    /// <param name="lastRow">Last row of the range.</param>
    /// <returns>Newly created range.</returns>
    virtual public RangeImpl CreateRange( object parent,
      int firstCol, int firstRow, int lastCol, int lastRow )
    {
      return new RangeImpl( this, parent, firstCol, firstRow, lastCol, lastRow );
    }

    /// <summary>
    /// Creates a new Range.
    /// </summary>
    /// <param name="parent">Parent object for the new range.</param>
    /// <param name="record">Range record to parse.</param>
    /// <param name="bIgnoreStyles">Indicates whether styles should be ignored.</param>
    /// <returns>Newly created range.</returns>
    [ CLSCompliant( false ) ]
    virtual public RangeImpl CreateRange( object parent, BiffRecordRaw record, bool bIgnoreStyles )
    {
      return new RangeImpl( this, parent, record, bIgnoreStyles );
    }
    #endregion

    #region Style
    /// <summary>
    /// Creates a new Style.
    /// </summary>
    /// <param name="parent">Parent object for the new style object.</param>
    /// <param name="name">Name of the new style.</param>
    /// <returns>Newly created style.</returns>
    virtual public StyleImpl CreateStyle( WorkbookImpl parent, string name )
    {
      //return new StyleImpl( this, parent, name );
      return new StyleImpl( parent, name );
    }
    /// <summary>
    /// Creates a new Style.
    /// </summary>
    /// <param name="parent">Parent object for the new style object.</param>
    /// <param name="name">Name of the new style.</param>
    /// <param name="basedOn">Base style for this style.</param>
    /// <returns>Newly created style.</returns>
    virtual public StyleImpl CreateStyle( WorkbookImpl parent, string name, StyleImpl basedOn )
    {
      return new StyleImpl( parent, name, basedOn );
    }
    /// <summary>
    /// Creates a new Style.
    /// </summary>
    /// <param name="parent">Parent object for the new style object.</param>
    /// <param name="style">Style record with style information.</param>
    /// <returns>Newly created style.</returns>
    [ CLSCompliant( false ) ]
    virtual public StyleImpl CreateStyle( WorkbookImpl parent, StyleRecord style )
    {
      return new StyleImpl( parent, style );
    }
    /// <summary>
    /// Creates a new Style.
    /// </summary>
    /// <param name="parent">Parent object for the new style object.</param>
    /// <param name="name">Name of the new style.></param>
    /// <param name="bIsBuildIn">Indicates whether built in style should be created.</param>
    /// <returns></returns>
    public virtual StyleImpl CreateStyle( WorkbookImpl parent, string name, bool bIsBuildIn )
    {
      return new StyleImpl( parent, name, null, bIsBuildIn );
    }
    #endregion

    #region Font
    /// <summary>
    /// Creates new font.
    /// </summary>
    /// <param name="parent">Parent object for the new font.</param>
    /// <returns>Newly created font.</returns>
    virtual public FontImpl CreateFont( object parent )
    {
      return new FontImpl( this, parent );
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Method creates a font object based on native font and register it in the workbook.
    /// </summary>]
    /// <param name="parent">Parent object for the new font.</param>
    /// <param name="nativeFont">Native font to get settings from.</param>
    /// <returns>Newly created font.</returns>
    virtual public FontImpl CreateFont( object parent, Font nativeFont )
    {
      return new FontImpl( this, parent, nativeFont );
    }
#endif
    /// <summary>
    /// Creates new font.
    /// </summary>
    /// <param name="basedOn">Base font for the new one.</param>
    /// <returns>Newly created font.</returns>
    virtual public FontImpl CreateFont( IFont basedOn )
    {
      return new FontImpl( basedOn );
    }
    /// <summary>
    /// Creates new font.
    /// </summary>
    /// <param name="parent">Parent object for the new font.</param>
    /// <param name="font">Font record that contains font information.</param>
    /// <returns>Newly created font.</returns>
    [ CLSCompliant( false ) ]
    virtual public FontImpl CreateFont( object parent, FontRecord font )
    {
      return new FontImpl( this, parent, font );
    }
    /// <summary>
    /// Creates new font.
    /// </summary>
    /// <param name="parent">Parent object for the new font.</param>
    /// <param name="font">Font object that contains font information.</param>
    /// <returns>Newly created font.</returns>
    [CLSCompliant(false)]
    virtual public FontImpl CreateFont(object parent, FontImpl font)
    {
        return new FontImpl(this, parent, font);
    }
    /// <summary>
    /// This method should be called before creating font.
    /// Sets default font name and size.
    /// </summary>
    public void CheckDefaultFont(WorkbookImpl workbook)
    {
        if (workbook != null)
        {
            switch (workbook.Version)
            {
                case ExcelVersion.Excel97to2003:
                    StandardFont = "Tahoma";
                    StandardFontSize = DEF_STANDARD_FONT_SIZE;
                    break;
                default:
                    StandardFont = "Calibri";
                    StandardFontSize = 11.00;
                    break;
            }
        }
    }
    #endregion

    #region ClipboardProvider
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Creates clipboard provider.
    /// </summary>
    /// <returns>Created provider.</returns>
    virtual public ClipboardProvider CreateClipboardProvider()
    {
      return new Biff8ClipboardProvider( new DelimiterClipboardProvider() );
    }
    /// <summary>
    /// Creates clipboard provider.
    /// </summary>
    /// <param name="sheet">Sheet for which provider will be created.</param>
    /// <returns>Created provider.</returns>
    virtual public ClipboardProvider CreateClipboardProvider( IWorksheet sheet )
    {
      return new Biff8ClipboardProvider( sheet, new DelimiterClipboardProvider( sheet ) );
    }
#endif
    #endregion

    #region Charts
    /// <summary>
    /// Creates a new chart.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns>The created chart object.</returns>
    public virtual ChartImpl CreateChart( object parent )
    {
      return new ChartImpl( this, parent );
    }
    /// <summary>
    /// Creates a chart series object.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns>The created chart series object.</returns>
    public virtual ChartSerieImpl CreateSerie( object parent )
    {
      return new ChartSerieImpl( this, parent );
    }
    #endregion

    #region RangesCollection
    /// <summary>
    /// Creates new instance of RangesCollection.
    /// </summary>
    /// <param name="parent">Parent object for the collection.</param>
    /// <returns>Newly created collection.</returns>
    public RangesCollection CreateRangesCollection( object parent )
    {
      return new RangesCollection( this, parent );
    }
    #endregion

    #region HyperLinks
    /// <summary>
    /// Initializes new hyperlink.
    /// </summary>
    /// <param name="parent">Parent object for the new hyperlink.</param>
    /// <returns>Newly created hyperlink.</returns>
    public virtual HyperLinkImpl CreateHyperLink( object parent )
    {
      return new HyperLinkImpl( this, parent );
    }
    /// <summary>
    /// Initializes new hyperlink.
    /// </summary>
    /// <param name="parent">Parent object for the new hyperlink.</param>
    /// <param name="range">Hyperlink range.</param>
    /// <returns>Newly created hyperlink.</returns>
    public virtual HyperLinkImpl CreateHyperLink( object parent, IRange range )
    {
      return new HyperLinkImpl( this, parent, range );
    }
    #endregion
    
    #region CommentsCollection
    /// <summary>
    /// Initialize new ComentsRange.
    /// </summary>
    /// <param name="parentRange">Parent Range for the new CommentRange.</param>
    /// <returns>Newly created ComentsRange.</returns>
    public virtual CommentsRange CreateCommentsRange( IRange parentRange )
    {
      return new CommentsRange( this, parentRange );
    }
    /// <summary>
    /// Initialize new CommentShapeImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new CommentShapeImpl.</param>
    /// <returns>Newly created CommentShapeImpl.</returns>
    public virtual CommentShapeImpl CreateCommentShapeImpl( object parent )
    {
      return CreateCommentShapeImpl( parent, true );
    }
    /// <summary>
    /// Initialize new CommentShapeImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new CommentShapeImpl.</param>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    /// <returns>Newly created CommentShapeImpl.</returns>
    public virtual CommentShapeImpl CreateCommentShapeImpl( object parent, bool bIsParseOptions )
    {
      return new CommentShapeImpl( this, parent, bIsParseOptions );
    }
    /// <summary>
    /// Initialize new CommentShapeImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new CommentShapeImpl.</param>
    /// <param name="container">Parent MsofbtSpContainer for the new CommentShapeImpl.</param>
    /// <returns>Newly created CommentShapeImpl.</returns>
    [ CLSCompliant( false ) ]
    public virtual CommentShapeImpl CreateCommentShapeImpl( object parent, MsofbtSpContainer container )
    {
      return new CommentShapeImpl( this, parent, container );
    }
    /// <summary>
    /// Initialize new CommentShapeImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new CommentShapeImpl.</param>
    /// <param name="container">Parent MsofbtSpContainer for the new CommentShapeImpl.</param>
    /// <param name="options">Parse options.</param>
    /// <returns>Newly created CommentShapeImpl.</returns>
    [ CLSCompliant( false ) ]
    public virtual CommentShapeImpl CreateCommentShapeImpl( object parent,
      MsofbtSpContainer container, ExcelParseOptions options )
    {
      return new CommentShapeImpl( this, parent, container, options );
    }
    #endregion

    #region DataValidation
    /// <summary>
    /// Initialize new DataValidationArrayImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new DataValidationArrayImpl.</param>
    /// <returns>Newly created DataValidationArrayImpl.</returns>
    public virtual DataValidationArray CreateDataValidationArrayImpl( IRange parent )
    {
      return new DataValidationArray( parent );
    }
    /// <summary>
    /// Initialize new DataValidationWrapper.
    /// </summary>
    /// <param name="range">Parent Range for the new DataValidationWrapper.</param>
    /// <param name="wrap">Parent DataValidationImpl for the new DataValidationWrapper.</param>
    /// <returns>Newly created DataValidationWrapper.</returns>
    public virtual DataValidationWrapper CreateDataValidationWrapper( RangeImpl range
      , DataValidationImpl wrap )
    {
      return new DataValidationWrapper( range, wrap );
    }
    /// <summary>
    /// Initialize new DataValidationImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new DataValidationImpl.</param>
    /// <returns>Newly created DataValidationImpl.</returns>
    public virtual DataValidationImpl CreateDataValidationImpl( DataValidationCollection parent )
    {
      return new DataValidationImpl( parent );
    }
    /// <summary>
    /// Initialize new DataValidationImpl.
    /// </summary>
    /// <param name="parent">Parent object for the new DataValidationImpl.</param>
    /// <param name="dv">Base DVRecord for the new DataValidationImpl.</param>
    /// <returns>Newly created DataValidationImpl.</returns>
    [ CLSCompliant( false ) ]
    public virtual DataValidationImpl CreateDataValidationImpl( DataValidationCollection parent
      , DVRecord dv )
    {
      return new DataValidationImpl( parent, dv );
    }
    #endregion

    #region ConditionalFormats
    /// <summary>
    /// Creates collection with specified argument.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual CondFormatCollectionWrapper CreateCondFormatCollectionWrapper( ICombinedRange range )
    {
      return new CondFormatCollectionWrapper( range );
    }
    /// <summary>
    /// Creates ConditionalFormats collection from array of BiffRecords.
    /// </summary>
    /// <param name="parent">Parent object for the collection.</param>
    /// <param name="format">Main conditional format record.</param>
    /// <param name="formats">Array of CFRecords with conditional formats.</param>
    [ CLSCompliant( false ) ]
    public virtual ConditionalFormats CreateConditionalFormats( object parent,
      CondFMTRecord format, IList formats, IList CFExRecords)
    {
        return new ConditionalFormats(this, parent, format, formats, CFExRecords);
    }
    /// <summary>
    /// Creates ConditionalFormats collection from array of BiffRecords.
    /// </summary>
    /// <param name="parent">Parent object for the collection.</param>
    /// <param name="format">Main conditional format12 record.</param>
    /// <param name="formats">Array of CF12Records with conditional formats.</param>
    [CLSCompliant(false)]
    public virtual ConditionalFormats CreateConditionalFormats(object parent,
      CondFmt12Record format, IList formats)
    {
        return new ConditionalFormats(this, parent, format, formats);
    }
    #endregion

    #region Template markers
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <param name="parent">Parent object for the new instance.</param>
    /// <returns>Object that can be used for template markers processing.</returns>
    public TemplateMarkersImpl CreateTemplateMarkers( object parent )
    {
      return new TemplateMarkersImpl( this, parent );
    }
    #endregion

    #region Unmanaged arrays
    /// <summary>
    /// Creates intptr data provider.
    /// </summary>
    /// <returns>New instance of created data provider.</returns>
    public static DataProvider CreateDataProvider( IntPtr heapHandle )
    {
      DataProvider result = null;

      switch( m_dataType )
      {
        case ExcelDataProviderType.ByteArray:
          result = new ByteArrayDataProvider();
          break;

#if !SILVERLIGHT && !WINRT && !WP
        case ExcelDataProviderType.Unsafe:
          // NOTE: order is important, if unsafe code is not allowed, then we use Native data provider.
#if AllowUnsafeCode
          result = new UnsafeDataProvider( heapHandle );
          break;
#endif

        case ExcelDataProviderType.Native:
          result = new IntPtrDataProvider( heapHandle );
          break;
#endif
      }

      return result;
    }
    ///// <summary>
    ///// Creates intptr data provider.
    ///// </summary>
    ///// <param name="ptrData">Array to read data from.</param>
    ///// <returns>New instance of created data provider.</returns>
    //public static IntPtrDataProvider CreateDataProvider( IntPtr ptrData )
    //{
    //  return ( m_bUseUnsafeCode )
    //    ? new UnsafeDataProvider( ptrData )
    //    : new IntPtrDataProvider( ptrData );
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal static DataProvider CreateDataProvider()
    {
      DataProvider result = null;

      switch( m_dataType )
      {
        case ExcelDataProviderType.ByteArray:
          result = new ByteArrayDataProvider();
          break;

#if !SILVERLIGHT && !WINRT && !WP
        case ExcelDataProviderType.Unsafe:
        // NOTE: order is important, if unsafe code is not allowed, then we use Native data provider.
#if AllowUnsafeCode
          result = new UnsafeDataProvider( IntPtr.Zero );
          break;
#endif

        case ExcelDataProviderType.Native:
          result = new IntPtrDataProvider( IntPtr.Zero );
          break;
#endif
      }

      return result;
    }
    #endregion

    #region CompoundStorage
    /// <summary>
    /// Creates compound file based on stream object.
    /// </summary>
    /// <param name="stream">Stream to create compound file for.</param>
    /// <returns>Created compound file object.</returns>
    internal ICompoundFile CreateCompoundFile( Stream stream )
    {
      return
#if !SILVERLIGHT && !WINRT && !WP
        ( !m_bNetStorage ) ?
        ( ICompoundFile )new Syncfusion.CompoundFile.XlsIO.Native.CompoundFile( stream ) :
#endif
        ( ICompoundFile )new Syncfusion.CompoundFile.XlsIO.Net.CompoundFile( stream );
    }
    /// <summary>
    /// Creates compound file object from file.
    /// </summary>
    /// <param name="fileName">Name of the file to open.</param>
    /// <param name="storageOptions">Storage flags to use.</param>
    /// <returns>Created compound file object.</returns>
    internal ICompoundFile CreateCompoundFile( string fileName, STGM storageOptions )
    {
      bool bCreate = ( storageOptions & STGM.STGM_CREATE ) != 0;
      ICompoundFile result;

#if !SILVERLIGHT && !WINRT && !WP
      if( m_bNetStorage )
#endif
      {
        Syncfusion.CompoundFile.XlsIO.Net.CompoundFile file =
          new Syncfusion.CompoundFile.XlsIO.Net.CompoundFile( fileName, bCreate );

        file.DirectMode = true;
        result = file;
      }
#if !SILVERLIGHT && !WINRT && !WP
      else
      {
        result = ( ICompoundFile )new Syncfusion.CompoundFile.XlsIO.Native.CompoundFile( fileName, storageOptions );
      }
#endif

      return result;
    }
    /// <summary>
    /// Creates compound file object.
    /// </summary>
    /// <returns>Created compound file object.</returns>
    internal ICompoundFile CreateCompoundFile()
    {
      return
#if !SILVERLIGHT && !WINRT && !WP
        ( !m_bNetStorage ) ?
        ( ICompoundFile )new Syncfusion.CompoundFile.XlsIO.Native.CompoundFile() :
#endif
        ( ICompoundFile )new Syncfusion.CompoundFile.XlsIO.Net.CompoundFile();
    }
    #endregion

    #region Image
    /// <summary>
    /// Create image from stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
#if (SILVERLIGHT || WP)
    public static System.Drawing.Image CreateImage( Stream stream )
       {
      return System.Drawing.Image.FromStream( stream, true,
#else
    public static Image CreateImage( Stream stream )

    {
      return Image.FromStream( stream, true,
#endif
#if AllowUnsafeCode
        false );
#else
 true );
#endif
    }
    #endregion

    #region TextBox
    /// <summary>
    /// Creates new instance of the textbox shape.
    /// </summary>
    /// <param name="shapesCollection">Parent shapes collection.</param>
    /// <returns>Created textbox shape.</returns>
    internal TextBoxShapeImpl CreateTextBoxShapeImpl( ShapesCollection shapesCollection, WorksheetImpl sheet )
    {
      return new TextBoxShapeImpl( this, shapesCollection, sheet );
    }
    #endregion

    #region CheckBox
    /// <summary>
    /// Creates new instance of the checkbox shape.
    /// </summary>
    /// <param name="shapesCollection">Parent shapes collection.</param>
    /// <returns>Created checkbox shape.</returns>
    public CheckBoxShapeImpl CreateCheckBoxShapeImpl( object shapesCollection )
    {
      return new CheckBoxShapeImpl( this, shapesCollection );
    }
    #endregion

    #region OptionButton
    /// <summary>
    /// Creates new instance of the OptionButton shape.
    /// </summary>
    /// <param name="shapesCollection">Parent shapes collection.</param>
    /// <returns>Created OptionButton shape.</returns>
    public OptionButtonShapeImpl CreateOptionButtonShapeImpl(object shapesCollection)
    {
        return new OptionButtonShapeImpl(this, shapesCollection);
    }
    #endregion

    #region Combobox
    /// <summary>
    /// Creates new instance of the combobox shape.
    /// </summary>
    /// <param name="shapesCollection">Parent shapes collection.</param>
    /// <returns>Created checkbox shape.</returns>
    public ComboBoxShapeImpl CreateComboBoxShapeImpl( object shapesCollection )
    {
      return new ComboBoxShapeImpl( this, shapesCollection );
    }
    #endregion

    #region Compression
    public virtual Stream CreateCompressor( Stream outputStream )
    {
      Stream result = null;
#if !(WINRT )
#if !(SILVERLIGHT || WP)
      if( m_compressionLevel == null )
      {
        result = new System.IO.Compression.DeflateStream( outputStream, System.IO.Compression.CompressionMode.Compress, true );
      }
      else
      {
#endif
        CompressionLevel compressionLevel = ( m_compressionLevel != null ) ?
          ( CompressionLevel )m_compressionLevel :
          Syncfusion.Compression.CompressionLevel.Normal;

        result = new NetCompressor( compressionLevel, outputStream );
#if !(SILVERLIGHT || WP)
      }
#endif
#else
      if (m_compressionLevel == null)
      {
          result = new System.IO.Compression.DeflateStream(outputStream, System.IO.Compression.CompressionMode.Compress, true);
      }
#endif
      return result;
    }
    #endregion

    #endregion

    #region Class public methods
    /// <summary>
    /// Checks and apply the seperators to the current culture.
    /// (ie.. DecimalSeperator, ThousandSeperators).
    /// </summary>
    /// <returns>Current Culture with the new Seperators.</returns>
    internal CultureInfo CheckAndApplySeperators()
    {
        if (!IsFormulaParsed)
        {
            return m_standredCulture;
        }
        else
        {
            return m_currentCulture;
        }
    }
    /// <summary>
    /// Sets active workbook.
    /// </summary>
    /// <param name="book">Workbook that becomes active.</param>
    public void SetActiveWorkbook( IWorkbook book )
    {
      m_ActiveBook = book;
    }
    /// <summary>
    /// Sets active worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet that becomes active.</param>
    public void SetActiveWorksheet( WorksheetBaseImpl sheet )
    {
      m_ActiveSheet = sheet;
    }
    /// <summary>
    /// Sets active cell.
    /// </summary>
    /// <param name="cell">Range that becomes active.</param>
    public void SetActiveCell( IRange cell )
    {
      m_ActiveCell = cell;
    }
    /// <summary>
    /// Converts to pixels.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="from">MeasureUnits.</param>
    /// <returns>Returns converted result.</returns>
    internal static double ConvertToPixels( double value, MeasureUnits from )
    {
      return value * s_arrProportions[ ( int )from ];
    }

    /// <summary>
    /// Converts from pixel.
    /// </summary>
    /// <param name="value">Pixel to convert.</param>
    /// <param name="to">Convert options.</param>
    /// <returns>Returns converted result.</returns>
    internal static double ConvertFromPixel( double value, MeasureUnits to )
    {
      return value / s_arrProportions[ ( int )to ];
    }

    /// <summary>
    /// Converts units.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="from">Form option.</param>
    /// <param name="to">To option.</param>
    /// <returns>Returns converted result.</returns>
    public static double ConvertUnitsStatic( double value, MeasureUnits from, MeasureUnits to )
    {
      return value * s_arrProportions[ ( int )from ] / s_arrProportions[ ( int )to ];
    }

    /// <summary>
    /// Converts units.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="from">Form option.</param>
    /// <param name="to">To option.</param>
    /// <returns>Returns converted result.</returns>
    public double ConvertUnits( double value, MeasureUnits from, MeasureUnits to )
    {
      return ( from == to )
        ? value
        : value * s_arrProportions[ ( int )from ] / s_arrProportions[ ( int )to ];
    }

    /// <summary>
    /// Raising progress event.
    /// </summary>
    /// <param name="curPos">Position.</param>
    /// <param name="fullSize">Full size.</param>
    public void RaiseProgressEvent( long curPos, long fullSize )
    {
      if( ProgressEvent != null )
      {
        ProgressEvent( this, new ProgressEventArgs( curPos, fullSize ) );
      }
    }
    /// <summary>
    /// Measures string.
    /// </summary>
    /// <param name="strToMeasure">String to measure.</param>
    /// <param name="font">Font to measure.</param>
    /// <param name="rectSize">Rect size.</param>
    /// <returns>Returns new size.</returns>
    public SizeF MeasureString( string strToMeasure, FontImpl font, SizeF rectSize )
    {
#if !SILVERLIGHT && !WINRT && !WP
      StringFormat format = new StringFormat( StringFormatFlags.NoWrap );
      lock (m_graphics)
      {
          return m_graphics.MeasureString(strToMeasure, font.GenerateNativeFont(), rectSize, format);
      }
#else
      throw new NotImplementedException();
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal bool RaiseOnPasswordRequired( object sender, PasswordRequiredEventArgs e )
    {
      bool bResult = false;

      if( OnPasswordRequired != null )
      {
        OnPasswordRequired( sender, e );
        bResult = true;
      }

      return bResult;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal bool RaiseOnWrongPassword( object sender, PasswordRequiredEventArgs e )
    {
      bool bResult = false;

      if( OnWrongPassword != null )
      {
        OnWrongPassword( sender, e );
        bResult = true;
      }

      return bResult;
    }
    #endregion

    #region Class events
    /// <summary>
    /// Progress event handler. 
    /// </summary>
    public event ProgressEventHandler ProgressEvent;
    /// <summary>
    /// This event is fired when user tries to open password protected workbook
    /// without specifying password. It is used to obtain password.
    /// </summary>
    public event PasswordRequiredEventHandler OnPasswordRequired;
    /// <summary>
    /// This event is fired when user specified wrong password when trying to open
    /// password protected workbook. It is used to obtain correct password.
    /// </summary>
    public event PasswordRequiredEventHandler OnWrongPassword;
    #endregion

    internal void Dispose()
    {
        WorksheetImpl wsheet = (this.ActiveSheet as WorksheetImpl);
        if (wsheet != null)
        {
            if (wsheet.Parent != null)
            {
            List<IWorksheet> sheets = ((WorksheetsCollection)wsheet.Parent).InnerList;

            foreach (IWorksheet sheet in sheets)
            {
                (sheet as WorksheetImpl).ClearAllData();
            }
                if(wsheet!=null && wsheet.ParentWorkbook!=null)
            wsheet.ParentWorkbook.DisposeAll();
        }
        }
#if !SILVERLIGHT && !WINRT && !WP
        if (m_graphics != null)
        {
            m_graphics.Dispose();
        }
#endif
        m_defaultStyleNames = null;
        m_workbooks = null;
        RemoveStylesCollection();
        RemovePageSetupCollection();

    }
    internal void RemoveStylesCollection()
    {
        int length = m_builtInStyleInfo.Length;
        for (int i = 0; i < length; i++)
        {
            m_builtInStyleInfo[i].Clear();
            m_builtInStyleInfo[i] = null;
        }

        m_builtInStyleInfo = null;
    }
    internal void RemovePageSetupCollection()
    {
        m_dicPaperSizeTable.Clear();
        m_dicPaperSizeTable = null;
    }
    #region ShapeImplementation

    private int dpiX = 96;
    private int dpiY = 96;



    internal int GetFontCalc1()
    {
        return 182;
    }
    internal int GetFontCalc2()
    {
        return 7;
    }
    internal int GetFontCalc3()
    {
        return 5;
    }
    internal int GetdpiX()
    {
        return this.dpiX;
    }
    internal int GetdpiY()
    {
        return this.dpiY;
    }
    #endregion
  }
}
