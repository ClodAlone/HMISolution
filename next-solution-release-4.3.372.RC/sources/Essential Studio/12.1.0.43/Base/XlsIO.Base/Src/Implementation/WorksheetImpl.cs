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

//#define MEASURE_PERFORMANCE

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using NameIndexChangedEventHandler = Syncfusion.XlsIO.Implementation.NameImpl.NameIndexChangedEventHandler;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Tables;
using Syncfusion.Calculate;
using System.Reflection;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Syncfusion.CompoundFile.XlsIO.Native;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents a worksheet. The Worksheet object is a member of the
  /// Worksheets collection. The Worksheets collection contains all the
  /// Worksheet objects in a workbook.
  /// </summary>
  public class WorksheetImpl
    : WorksheetBaseImpl
    , IWorksheet
    , ISerializableNamedObject
    , IParseable
    , ICloneParent
    , IInternalWorksheet, ISheetData
  {
      #region ICalcData & Calculate methods


      
      private Syncfusion.Calculate.CalcEngine m_calcEngine;
      internal bool m_hasSheetCalculation;
      /// <summary>
      /// Indicates whether this instance has Alernate Content.
      /// </summary>
      private bool m_hasAlternateContent;
      internal int unknown_formula_name = 9;
      /// <summary>
      /// Event raised when an unknown function is encountered.
      /// </summary>
      public event MissingFunctionEventHandler MissingFunction;
      public event Syncfusion.XlsIO.Implementation.RangeImpl.CellValueChangedEventHandler CellValueChanged;

      /// <summary>
      /// Returns or sets the a CalcEngine object associated with this ICalcData implementation.
      /// </summary>
      public Syncfusion.Calculate.CalcEngine CalcEngine 
      { 
          get          
         {
          return m_calcEngine;
         }
          set
         {
          m_calcEngine = value;
         }
      }
      /// <summary>
      /// Gets a value indicating whether this instance has sheet calculation.
      /// </summary>
      /// <value>
      /// 	<c>true</c> if this instance has sheet calculation; otherwise, <c>false</c>.
      /// </value>
      internal bool HasSheetCalculation
      {
          get
          {
              return m_hasSheetCalculation;
          }
      }
      /// <summary>
      /// Gets a value indicating whether this instance has Alernate Content.
      /// </summary>
      /// <value>
      /// 	<c>true</c> if this instance has sheet AlernateContent; otherwise, <c>false</c>.
      /// </value>
      internal bool HasAlternateContent
      {
          get
          {
              return m_hasAlternateContent;
          }
          set
          {
              m_hasAlternateContent = value;
          }
      }
      /// <summary>
      /// Enables calculation support. If you want to be able to retrieve calculated values of formulas
      /// based on values you have changed in the workbook, then call this method once for any any worksheet
      /// in the workbook. Your can then used worksheet[row, column].CalculatedValue to access the proper
      /// calculated value of a cell.
      /// </summary>
      public void EnableSheetCalculations()
      {
          m_book.EnabledCalcEngine = true;
          if (CalcEngine == null)
          {
              Syncfusion.Calculate.CalcEngine.ParseArgumentSeparator = this.AppImplementation.ArgumentsSeparator;
              Syncfusion.Calculate.CalcEngine.ParseDecimalSeparator = Convert.ToChar(this.AppImplementation.DecimalSeparator);

              CalcEngine = new Syncfusion.Calculate.CalcEngine(this);
              CalcEngine.ExcelLikeComputations = true;
              CalcEngine.UseDatesInCalculations = true;
              CalcEngine.UseNoAmpersandQuotes = true;
           lock (CalcEngine)
           {
              
              int sheetFamilyID = Syncfusion.Calculate.CalcEngine.CreateSheetFamilyID();

              string nameList = "!";

              //register the sheet names with calculate
              foreach (IWorksheet sheet in this.ParentWorkbook.Worksheets)
              {
                  if (sheet.CalcEngine == null)
                  {
                      sheet.CalcEngine = new Syncfusion.Calculate.CalcEngine(sheet);
                      sheet.CalcEngine.UseDatesInCalculations = true;
                      sheet.CalcEngine.UseNoAmpersandQuotes = true;
                      sheet.CalcEngine.ExcelLikeComputations = true;
                  }
                  CalcEngine.RegisterGridAsSheet(sheet.Name, sheet, sheetFamilyID);
                    sheet.CalcEngine.UnknownFunction += new Syncfusion.Calculate.UnknownFunctionEventHandler(CalcEngine_UnknownFunction);
                  nameList += sheet.Name + "!";
              }

              //get the named ranges into calculate
              Dictionary<string, string> ranges = new Dictionary<string, string>();
              foreach (IName name in this.ParentWorkbook.Names)
              {
                  //ranges.Add(name.Scope + ":" + name.Name, name.Value.Replace("'", ""));
                  if (name.Scope.Length > 0 && nameList.IndexOf("!" + name.Scope + "!") > -1 && name .Value != null )
                  {
                      ranges.Add((name.Scope + "!" + name.Name).ToUpper(), name.Value.Replace("'", ""));
                  }
                  else
                  {
                      if ((name.Name != null) && (name.Value != null) && (!ranges.ContainsKey(name.Name.ToUpper())))
                          ranges.Add(name.Name.ToUpper(), name.Value.Replace("'", ""));
                  }
              }

#if  (SILVERLIGHT) || (WINRT) || (WP)
            Dictionary<object, object> namedRanges1 = new Dictionary<object, object>();
#else
              Hashtable namedRanges1 = new Hashtable();
#endif
              if (ranges != null)
              {
                  foreach (string s in ranges.Keys)
                  {
                      namedRanges1.Add(s.ToUpper(System.Globalization.CultureInfo.InvariantCulture), ranges[s]);
                  }
              }

              foreach (IWorksheet st in this.ParentWorkbook.Worksheets)
              {
                      st.CalcEngine.NamedRanges = namedRanges1;
                  
              }
           }
         
      }
          m_hasSheetCalculation = true;
      }

        void CalcEngine_UnknownFunction(object sender, Syncfusion.Calculate.UnknownFunctionEventArgs args)
        {
            if (MissingFunction != null && CalcEngine != null)
            {
                MissingFunctionEventArgs e = new MissingFunctionEventArgs();
                e.MissingFunctionName = args.MissingFunctionName;
                e.CellLocation = args.CellLocation;
                MissingFunction(this, e);
            }
        }

      /// <summary>
      /// Disables calculation support in this workbook and disposes of the associative CalcEngine objects.
      /// </summary>
      public void DisableSheetCalculations()
      {
          m_book.EnabledCalcEngine = false;
          if (CalcEngine != null && this.ParentWorkbook != null && this.ParentWorkbook.Worksheets != null)
          {
              foreach (IWorksheet st in this.ParentWorkbook.Worksheets)
              {
                  if (st.CalcEngine != null)
                  {
                        st.CalcEngine.UnknownFunction -= new Syncfusion.Calculate.UnknownFunctionEventHandler(CalcEngine_UnknownFunction);
                      st.CalcEngine.Dispose();
                  }
                  st.CalcEngine = null;
              }
              m_hasSheetCalculation = false;
          }
      }
 
      #region ICalcData Members

      /// <summary>
      /// Returns the formula string if the cell contains a formula, or the value if
      /// the cell cantains anything other than a formula.
      /// </summary>
      /// <param name="row">The row of the cell.</param>
      /// <param name="col">The column of the cell.</param>
      /// <returns>The formula string or value.</returns>
      public object GetValueRowCol(int row, int col)
      {
          IRange r = this[row, col];
          if (r.HasFormula)
              return r.Formula;
          else
              return r.Value;
      }

      /// <summary>
      /// Sets the value of a cell.
      /// </summary>
      /// <param name="value">The value to be set.</param>
      /// <param name="row">The row of the cell.</param>
      /// <param name="col">The column of the cell.</param>
      public void SetValueRowCol(object value, int row, int col)
      {
            if (value != null)
            {
              this.SetValue(row, col, value.ToString());
            }
        }

      /// <summary>
      /// Not implemented.
      /// </summary>
      public void WireParentObject()
      {
          // throw new NotImplementedException();
      }

      /// <summary>
      /// An event raised on the IWorksheet whenever a value changes.
      /// </summary>
      public event Syncfusion.Calculate.ValueChangedEventHandler ValueChanged;

      /// <summary>
      /// Raises the <see cref="ValueChanged"/> event.
      /// </summary>
      /// <param name="row">The row of the change.</param>
      /// <param name="col">The column of the change.</param>
      /// <param name="value">The changed value.</param>
      public void OnValueChanged(int row, int col, string value)
      {
          if (ValueChanged != null)
          {
              Syncfusion.Calculate.ValueChangedEventArgs e = new Syncfusion.Calculate.ValueChangedEventArgs(row, col, value);
              ValueChanged(this, e);
          }
      }

      #endregion

      #endregion
    #region Skipped
#if SKIPPED
/*
    /// <summary>
    ///
    /// </summary>
    private HPageBreaksCollection m_hpageBreaks;
    /// <summary>
    ///
    /// </summary>
    private VPageBreaksCollection m_vpageBreaks;
    /// <summary>
    /// Returns a Range object that represents all the columns on the
    /// specified worksheet. Read-only.
    /// </summary>
    public IRange       Columns
    {
      get
      {
        return UsedRange.Columns;
      }
    }

    /// <summary>
    /// Copies the sheet to another location in the workbook.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    public void Copy( object Before, object After )
    {
      // TODO: Add WorksheetImpl.Copy implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes the object.
    /// </summary>
    public void Delete()
    {
      // TODO: Add WorksheetImpl.Delete implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Pastes the contents of the Clipboard onto the sheet.
    /// </summary>
    /// <param name="Destination"></param>
    /// <param name="Link"></param>
    public void Paste( object Destination, object Link )
    {
      // TODO: Add WorksheetImpl.Paste implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Pastes the contents of the Clipboard onto the sheet, using a
    /// specified format. Use this method to paste data from other
    /// applications or to paste data in a specific format.
    /// </summary>
    /// <param name="Format"></param>
    /// <param name="Link"></param>
    /// <param name="DisplayAsIcon"></param>
    /// <param name="IconFileName"></param>
    /// <param name="IconIndex"></param>
    /// <param name="IconLabel"></param>
    /// <param name="NoHTMLFormatting"></param>
    public void PasteSpecial( object Format, object Link,
      object DisplayAsIcon, object IconFileName,
      object IconIndex, object IconLabel,
      object NoHTMLFormatting )
    {
      // TODO: Add WorksheetImpl.PasteSpecial implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Resets all page breaks on the specified worksheet.
    /// </summary>
    public void ResetAllPageBreaks()
    {
      // TODO: Add WorksheetImpl.ResetAllPageBreaks implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Saves changes to the chart or worksheet in a different file.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="FileFormat"></param>
    /// <param name="Password"></param>
    /// <param name="WriteResPassword"></param>
    /// <param name="ReadOnlyRecommended"></param>
    /// <param name="CreateBackup"></param>
    /// <param name="AddToMru"></param>
    /// <param name="TextCodepage"></param>
    /// <param name="TextVisualLayout"></param>
    /// <param name="Local"></param>
    public void SaveAs( string Filename, object FileFormat,
      object Password, object WriteResPassword,
      object ReadOnlyRecommended, object CreateBackup,
      object AddToMru, object TextCodepage,
      object TextVisualLayout, object Local )
    {
      // TODO: Add WorksheetImpl.SaveAs implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Selects the object.
    /// </summary>
    /// <param name="Replace"></param>
    public void Select( object Replace )
    {
      // TODO: Add WorksheetImpl.Select implementation.
      throw new NotImplementedException();
    }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the next
    /// sheet or cell. Read-only.
    /// </summary>
    public object       Next
    {
      get
      {
        return m_rngUsed.Next;
      }
    }
    /// <summary>
    /// Returns a Chart, Range, or Worksheet object that represents the previous
    /// sheet or cell. Read-only.
    /// </summary>
    public object       Previous
    {
      get
      {
        return m_rngUsed.Previous;
      }
    }
*/
#endif
    #endregion

    #region Class constants
    /// <summary>
    /// Default character (for width measuring).
    /// </summary>
    internal const char DEF_STANDARD_CHAR = '0';
    /// <summary>
    /// One degree in radians.
    /// </summary>
    private const float DEF_AXE_IN_RADIANS = ( float )Math.PI / 180;
    /// <summary>
    /// Maximum column width.
    /// </summary>
    private const int DEF_MAX_COLUMN_WIDTH = 255;
    /// <summary>
    /// Width of the zero character.
    /// </summary>
    private const double DEF_ZERO_CHAR_WIDTH = 8;
    /// <summary>
    /// Default size for the dictionary of ranges.
    /// </summary>
    private const int DEF_ARRAY_SIZE = 100;
    /// <summary>
    /// Default size of autofilter arrow width.
    /// </summary>
    private const int DEF_AUTO_FILTER_WIDTH = 16;
    /// <summary>
    /// Represents indent width.
    /// </summary>
    private const int DEF_INDENT_WIDTH = 12;
    /// Default OleDateValue
    /// </summary>
    private const double DEF_OLE_DOUBLE = 2958465.9999999884;
    /// <summary>
    /// Maximum OleDateValue
    /// </summary>
    private const double DEF_MAX_DOUBLE = 2958466.0;
    /// <summary>
    /// Represents the Carriage Return character.
    /// </summary>
    private const char CarriageReturn = '\r';
    /// <summary>
    /// Represents the Carriage new line character.
    /// </summary>
    private const char NewLine = '\n';
    /// <summary>
    /// Defines which property of IRange should be used.
    /// </summary>
    private enum RangeProperty
    {
      Value2,
      Text,
      DateTime,
      TimeSpan,
      Double,
      Int,
    }
    /// <summary>
    /// Represents range value type.
    /// </summary>
    [ Flags ]
    public enum TRangeValueType
    {
      /// <summary>
      /// Represents Blank type.
      /// </summary>
      Blank = 0,
      /// <summary>
      /// Represents Error type.
      /// </summary>
      Error = 1,
      /// <summary>
      /// Represents Boolean type.
      /// </summary>
      Boolean = 2,
      /// <summary>
      /// Represents Number type.
      /// </summary>
      Number = 4,
      /// <summary>
      /// Represents Formula type.
      /// </summary>
      Formula = 8,
      /// <summary>
      /// Represents String type.
      /// </summary>
      String = 16
    }
    #endregion

    #region Class delegates
    /// <summary>
    /// Delegate for outline creation.
    /// </summary>
    private delegate IOutline OutlineDelegate( int iIndex );
    #endregion

    #region Class static members
    //    /// <summary>
    //    /// Record codes for AutoFilter.
    //    /// </summary>
    //    private static readonly TBIFFRecord[] AUTOFILTER_RECORDS = new TBIFFRecord[]
    //    {
    //      TBIFFRecord.AutoFilter,
    //      TBIFFRecord.AutoFilterInfo,
    //      TBIFFRecord.FilterMode,
    //    };
    /// <summary>
    /// Array with autofilter record types.
    /// </summary>
    private static readonly TBIFFRecord[] s_arrAutofilterRecord = new TBIFFRecord[]
    {
      TBIFFRecord.AutoFilter, TBIFFRecord.AutoFilterInfo, TBIFFRecord.FilterMode,
    };

    #endregion

    #region Class members
    private Dictionary<int, int> m_indexAndLevels;
    /// <summary>
    /// Represents to parse sheet on demand
    /// </summary>
    private bool m_bParseDataOnDemand;
    /// <summary>
    /// Range that contains all used cells.
    /// </summary>
    private RangeImpl       m_rngUsed;
    /// <summary>
    /// The dictionary holds a records objects, representing each cell.
    /// Holds information about used cells only.
    /// Key - cell index.
    /// Value - corresponding BiffRecordRaw.
    /// </summary>
    private CellRecordCollection m_dicRecordsCells;
    /// <summary>
    /// In the dictionary store, where ColumnIndex-to-ColumnInfoRecord.
    /// Column Index is started from 1.
    /// </summary>
    private ColumnInfoRecord[] m_arrColumnInfo;
    /// <summary>
    /// Indicates whether page breaks should be displayed.
    /// </summary>
    private bool            m_bDisplayPageBreaks;
    /// <summary>
    /// Object that contains information about page setup, i.e.
    /// paper size, paper orientation, footers, headers, etc.
    /// </summary>
    private PageSetupImpl   m_pageSetup;
    /// <summary>
    /// Standard column width.
    /// </summary>
    //    private int             m_iStandardColWidth = ( int )(256 * DEF_ZERO_CHAR_WIDTH);
    private double          m_dStandardColWidth = 8.43;
    /// <summary>
    /// Object that contains all merged regions of the worksheet.
    /// </summary>
    private MergeCellsImpl  m_mergedCells;
    /// <summary>
    /// Array store containing all selection records.
    /// </summary>
    private List<SelectionRecord>       m_arrSelections;
    /// <summary>
    /// 
    /// </summary>
    private PaneRecord      m_pane;
    /// <summary>
    /// Collection of all names defined in the worksheet (like named ranges).
    /// </summary>
    private WorksheetNamesCollection m_names;
    /// <summary>
    /// Type of the worksheet.
    /// </summary>
    private ExcelSheetType m_sheetType = ExcelSheetType.Worksheet;
    /// <summary>
    /// Indicates if values are preserved as strings.
    /// </summary>
    private bool m_bStringsPreserved;
    /// <summary>
    /// Array of all records for autofilter.
    /// </summary>
    private List<BiffRecordRaw> m_arrAutoFilter;
    /// <summary>
    /// SortedList with all NoteRecords.
    /// </summary>
    private SortedList<int, NoteRecord> m_arrNotes;
    /// <summary>
    /// Notes sorted by cell index, key - cell index, value - note record.
    /// </summary>
    private SortedList<long, NoteRecord> m_arrNotesByCellIndex;
    /// <summary>
    /// 
    /// </summary>
    private NameIndexChangedEventHandler m_nameIndexChanged;
    /// <summary>
    /// Collection of all data validations in the worksheet.
    /// </summary>
    private DataValidationTable m_dataValidation;
    /// <summary>
    /// Collection of worksheet's autofilters.
    /// </summary>
    private AutoFiltersCollection m_autofilters;
    /// <summary>
    /// Collection of worksheet's pivot tables.
    /// </summary>
    private PivotTableCollection m_pivotTables;
    /// <summary>
    /// Collection of all hyperlinks in the current worksheet.
    /// </summary>
    private HyperLinksCollection m_hyperlinks;
    /// <summary>
    /// Contains all worksheet's sort records.
    /// </summary>
    private List<BiffRecordRaw> m_arrSortRecords;
    /// <summary>
    /// Start index for pivot table records.
    /// </summary>
    private int m_iPivotStartIndex = -1;
    /// <summary>
    /// Start index for hyperlinks records.
    /// </summary>
    private int m_iHyperlinksStartIndex = -1;
    /// <summary>
    /// Start index for conditional formatting records.
    /// </summary>
    private int m_iCondFmtPos = -1;
    /// <summary>
    /// Start index for data validation formatting records.
    /// </summary>
    private int m_iDValPos = -1;
    /// <summary>
    /// Start index of custom properties block.
    /// </summary>
    private int m_iCustomPropertyStartIndex = -1;
    /// <summary>
    /// DCon records.
    /// </summary>
    private List<BiffRecordRaw> m_arrDConRecords;
    /// <summary>
    /// List with all conditional format collections.
    /// </summary>
    private WorksheetConditionalFormats m_arrConditionalFormats;
    /// <summary>
    /// Collection of custom properties.
    /// </summary>
    private WorksheetCustomProperties m_arrCustomProperties;
    /// <summary>
    /// Migrant range - row and column of this range object can be changed by user.
    /// </summary>
    private IMigrantRange m_migrantRange;
    /// <summary>
    /// Worksheet's index record. This member is used for parsing only.
    /// </summary>
    private IndexRecord m_index;
    /// <summary>
    /// There are two different algorithms to create UsedRange object:
    /// 1) Default. This property = true. The cell is included into UsedRange when
    /// it has some record created for it even if data is empty (maybe some formatting
    /// changed, maybe not - cell was accessed and record was created).
    /// 2) This property = false. In this case XlsIO tries to remove empty rows and
    /// columns from all sides to make UsedRange smaller.
    /// </summary>
    private bool m_bUsedRangeIncludesFormatting = true;
    /// <summary>
    /// Contains settings of string preservation for ranges.
    /// </summary>
    private RangeTrueFalse m_stringPreservedRanges = new RangeTrueFalse();
    /// <summary>
    /// Object used for shape coordinates evaluation on loading.
    /// </summary>
    private ItemSizeHelper m_rowHeightHelper;
    /// <summary>
    /// Reresents collection of all list objects in the worksheet.
    /// </summary>
    private ListObjectCollection m_listObjects;
    /// <summary>
    /// List with preserved table object's records.
    /// </summary>
    private List<BiffRecordRaw> m_tableRecords;
  /// <summary>
   /// To set Ishidden property
    /// </summary>
    private bool m_isRowHeightSet;
    /// <summary>
    /// For Zeroheight attribute to enable or disable
    /// </summary>
    private bool m_isZeroHeight;
       /// <summary>
        /// Specifies the number of characters of the maximum digit width of the normal style's font.
        /// </summary>
        private int m_baseColumnWidth;
        /// <summary>
        /// 'True' if rows have a thick bottom border by default.
        /// </summary>
        private bool m_isThickBottom;
        /// <summary>
        /// 'True' if rows have a thick top border by default.
        /// </summary>
        private bool m_isThickTop;
        /// <summary>
        /// Highest number of outline levels for columns in this sheet.
        /// </summary>
        private byte m_outlineLevelColumn;
        ///<summary >
        /// Highest number of outline level for rows in this sheet.
        /// </summary>
        private byte m_outlineLevelRow;
    private ColumnInfoRecord m_rawColRecord;
    private bool m_bOptimizeImport;
    private SheetView m_view = SheetView.Normal;
    internal List<Stream> preservedStreams;
    /// <summary>
    /// List of CondFMT records.
    /// </summary>
    internal Dictionary<int, CondFMTRecord> m_dictCondFMT = new Dictionary<int, CondFMTRecord>();
    /// <summary>
    /// List of CFEx Records.
    /// </summary>
    internal Dictionary<int, CFExRecord> m_dictCFExRecords = new Dictionary<int, CFExRecord>();
    private AutoFitManager m_autoFitManager;
    
    /// <summary>
    /// List of Outline Wrappers collection
    /// </summary>
    internal List<IOutlineWrapper> m_outlineWrappers;
    /// <summary>
    /// Represents the Column group outline dictionary collection
    /// </summary>
    private Dictionary<int, List<Point>> m_columnOutlineLevels;
    /// <summary>
    /// Represents the row group outline dictionary collection
    /// </summary>
    private Dictionary<int, List<Point>> m_rowOutlineLevels;
   
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Represents the Collection of OleObject
    /// </summary>
    private OleObjects m_oleObjects;
#endif
        /// <summary>
        /// Represents the Collection of SparklineGroups
        /// </summary>
        private SparklineGroups m_sparklineGroups;
        /// <summary>
        /// Represents the Collection of InlineStrings
        /// </summary>
        private Dictionary<string, string> m_inlineStrings;
    /// <summary>
    /// Preserves the External connection setting in the worksheet.
    /// </summary>
    private List<BiffRecordRaw> m_preserveExternalConnection;
    /// <summary>
    /// Preserves the pivot tables.
    /// </summary>
    private List<Stream> m_preservePivotTables;
    
    /// <summary>
    /// Stream to preserve the worksheet slicer
    /// </summary>
    internal Stream m_worksheetSlicer;
    /// <summary>
    /// Represents a formula string for external links
    /// </summary>
    internal string m_formulaString;
    private ColumnCollection columnCollection;
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static WorksheetImpl()
    {
      //s_hashAutofilterRecord.Add( ( int )TBIFFRecord.AutoFilter, null );
      //s_hashAutofilterRecord.Add( ( int )TBIFFRecord.AutoFilterInfo, null );
      //s_hashAutofilterRecord.Add( ( int )TBIFFRecord.FilterMode, null );
    }
    /// <summary>
    /// Creates worksheet and set its Application and Parent
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the worksheet.</param>
    /// <param name="parent">Parent object for the worksheet.</param>
    public WorksheetImpl( IApplication application, object parent )
      : base( application, parent )
    {
        //CalcEngine = new Syncfusion.Calculate.CalcEngine(this);
    }
    /// <summary>
    /// Creates worksheet from the stream.
    /// </summary>
    /// <param name="application">Application object for the worksheet.</param>
    /// <param name="parent">Parent object for the worksheet.</param>
    /// <param name="reader">BiffReader with worksheet data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes used in ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public WorksheetImpl( IApplication application, object parent, BiffReader reader
      , ExcelParseOptions options, bool bSkipParsing, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
      : base( application, parent, reader, options, bSkipParsing, hashNewXFormatIndexes, decryptor )
    {
    }
    /// <summary>
    /// Initializes all inner data such as Comments collection,
    /// Page setup, etc.
    /// </summary>
    protected override void InitializeCollections()
    {
      base.InitializeCollections();

      m_nameIndexChanged = new NameIndexChangedEventHandler( OnNameIndexChanged );

      m_dicRecordsCells = new CellRecordCollection( Application, this );
      m_pageSetup = new PageSetupImpl( Application, this );  
        if (this.Application.DefaultVersion != ExcelVersion.Excel97to2003)
      {
          m_pageSetup.DefaultRowHeight = (int)Application.StandardHeight * 20;
      }
      m_names = new WorksheetNamesCollection( Application, this );
      m_autofilters = new AutoFiltersCollection( Application, this );
      m_arrConditionalFormats = new WorksheetConditionalFormats( Application, this );
      m_errorIndicators = new ErrorIndicatorsCollection( Application, this );
      m_arrColumnInfo = new ColumnInfoRecord[ m_book.MaxColumnCount + 2 ];
      m_bOptimizeImport = Application.OptimizeImport;
      Index = m_book.Worksheets.Count;

      m_arrSelections = new List<SelectionRecord>();

      StandardWidth = Application.StandardWidth;
      StandardHeight = Application.StandardHeight;
      StandardHeightFlag = Application.StandardHeightFlag;
      AttachEvents();
    }
    /// <summary>
    /// Clear all internal collections.
    /// </summary>
    protected void ClearAll()
    {
      ClearAll( ExcelWorksheetCopyFlags.CopyAll );
    }
    /// <summary>
    /// Clear all internal collections.
    /// </summary>
    /// <param name="flags">Allows to avoid clearing of some properties.</param>
    protected override void ClearAll( ExcelWorksheetCopyFlags flags )
    {
      m_dicRecordsCells.Clear();
      m_arrSelections.Clear();

      if( ( flags & ExcelWorksheetCopyFlags.CopyNames ) != 0 )
        m_names.Clear();

      base.ClearAll( flags );

      //if( m_hashAttachedNames != null )m_hashAttachedNames.Clear();
      if( m_autofilters != null ) m_autofilters.Clear();

      if( m_hyperlinks != null ) m_hyperlinks.Clear();

      if( m_arrCustomProperties != null ) m_arrCustomProperties.Clear();
    }
    /// <summary>
    /// Copies names from another worksheet.
    /// </summary>
    /// <param name="basedOn">Worksheet to copy names from.</param>
    /// <param name="hashNewSheetNames">Dictionary with new worksheet names.</param>
    /// <param name="hashNewNameIndexes">Dictionary, key - old name index, value - new name index.</param>
    /// <param name="hashExternSheetIndexes">Represents hash table with new extern sheet indexes.</param>
    protected void CopyNames( WorksheetImpl basedOn, Dictionary<string, string> hashNewSheetNames
      , Dictionary<int, int> hashNewNameIndexes, Dictionary<int, int> hashExternSheetIndexes )
    {
      if( basedOn == null )
        throw new ArgumentNullException( "basedOn" );

      for( int i = m_names.Count - 1; i >= 0; i-- )
      {
        m_names[ i ].Delete();
      }

      if( hashNewSheetNames == null )
      {
        hashNewSheetNames = new Dictionary<string, string>();
        hashNewSheetNames.Add( Name, basedOn.Name );
      }

      m_names.FillFrom( basedOn.m_names, hashNewSheetNames, hashNewNameIndexes,
        ExcelNamesMergeOptions.MakeLocal, hashExternSheetIndexes );

      // Here we have to copy all global names and change them into local names if necessary.
      // Also we have to remember old and new name indexes.
      WorkbookNamesCollection globalSourceNames = basedOn.Workbook.Names as WorkbookNamesCollection;
      WorkbookNamesCollection globalDestNames = Workbook.Names as WorkbookNamesCollection;
      NameImpl name;
      IName newName;

      for( int i = 0, len = globalSourceNames.Count; i < len; i++ )
      {
        name = ( NameImpl )globalSourceNames[ i ];

        if( !name.IsLocal )
        {
          IRange range = name.RefersToRange;
          int iOldNameIndex = name.Index;

          if( range == null )
          {
            if( !globalDestNames.Contains( name.Name ) )
            {
              NameRecord record = ( NameRecord )( ( NameImpl )name ).Record.Clone();
              WorksheetNamesCollection.UpdateReferenceIndexes( record, name.Workbook,
                hashNewSheetNames, hashExternSheetIndexes, m_book );
              newName = globalDestNames.Add( record );
              hashNewNameIndexes[ iOldNameIndex ] = newName.Index;
            }
          }
          else if( range.Worksheet == basedOn )
          {
              try
              {
                  // In this case we have to try to copy this name.
                  newName = globalDestNames.AddCopy(name, this, hashExternSheetIndexes, hashNewSheetNames);
                  hashNewNameIndexes[iOldNameIndex] = newName.Index;
              }
              catch (Exception e)
              {
                  if (!globalDestNames.Contains(name.Name))
                  {
                      NameRecord record = (NameRecord)((NameImpl)name).Record.Clone();
                      WorksheetNamesCollection.UpdateReferenceIndexes(record, name.Workbook,
                        hashNewSheetNames, hashExternSheetIndexes, m_book);
                      newName = globalDestNames.Add(record);
                      hashNewNameIndexes[iOldNameIndex] = newName.Index;
                  }
              }
          }
        }
      }

      Dictionary<int, object> usedNames = basedOn.GetUsedNames();

      foreach( int nameIndex in usedNames.Keys )
      {
        if( !hashNewNameIndexes.ContainsKey( nameIndex ) )
        {
          name = ( NameImpl )globalSourceNames[ nameIndex ];
          newName = globalDestNames.AddCopy( name, this, hashExternSheetIndexes, hashNewSheetNames );
          hashNewNameIndexes[ nameIndex ] = newName.Index;
        }
      }
    }
    /// <summary>
    /// Searches for all used named range objects.
    /// </summary>
    /// <returns>Dictionary where key means named range index.</returns>
    private Dictionary<int, object> GetUsedNames()
    {
      ArrayListEx rows = m_dicRecordsCells.Table.Rows;
      Dictionary<int, object> result = new Dictionary<int, object>();

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = rows[ i ];

        if( row != null )
          row.GetUsedNames( result );
      }

      return result;
    }
    /// <summary>
    /// Copies row height.
    /// </summary>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    protected void CopyRowHeight( WorksheetImpl sourceSheet, Dictionary<int, int> hashExtFormatIndexes )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      //m_arrRow = new Dictionary( sourceSheet.m_arrRow );
      //m_arrRow = CloneUtils.CloneHash( ( Dictionary )sourceSheet.m_arrRow );
      //UpdateIndexes( m_arrRow.Values, sourceSheet, hashExtFormatIndexes, false );
    }
    /// <summary>
    /// Copies conditional formats.
    /// </summary>
    /// <param name="sourceSheet">Base worksheet.</param>
    protected void CopyConditionalFormats( WorksheetImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      sourceSheet.ParseSheetCF();
      this.ParseSheetCF();  
      m_arrConditionalFormats.CopyFrom( sourceSheet.m_arrConditionalFormats );
    }
    /// <summary>
    /// Copies autofilters.
    /// </summary>
    /// <param name="sourceSheet">Base worksheet.</param>
    protected void CopyAutoFilters( WorksheetImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      List<BiffRecordRaw> arrList = sourceSheet.m_arrAutoFilter;

      if( arrList != null )
      {
        List<BiffRecordRaw> arrAutoFilter = AutoFilterRecords;

        for( int i = 0, iLen = arrList.Count; i < iLen; i++ )
        {
          arrAutoFilter.Add( ( BiffRecordRaw )CloneUtils.CloneCloneable( ( ICloneable )arrList[ i ] ) );
          //          AutoFilterInfoRecord toClone = ( AutoFilterInfoRecord )arrList[ i ];
          //
          //          if( toClone != null )
          //          {
          //            object o = toClone.Clone();
          //            arrAutoFilter.Add( o );
          //          }
        }
      }

      if( sourceSheet.m_arrAutoFilter != null || sourceSheet .m_autofilters .Count > 0)
      {
        m_autofilters = sourceSheet.m_autofilters.Clone( this );
      }
    }
    /// <summary>
    /// Copies data validations.
    /// </summary>
    /// <param name="sourceSheet">Base worksheet.</param>
    protected void CopyDataValidations( WorksheetImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      DataValidationTable toClone = sourceSheet.m_dataValidation;

      if( toClone != null )
      {
        m_dataValidation = ( DataValidationTable )toClone.Clone( this );
      }
    }
    /// <summary>
    /// Copies column width.
    /// </summary>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashExtFormatIndexes">Array with new extended format indexes.</param>
    protected void CopyColumnWidth( WorksheetImpl sourceSheet, Dictionary<int, int> hashExtFormatIndexes )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      ColumnInfoRecord[] arrColumnInfo = CloneUtils.CloneArray( sourceSheet.m_arrColumnInfo );//sourceSheet.m_arrColumnInfo.CloneAll();
      int iCount = Math.Min( arrColumnInfo.Length, m_arrColumnInfo.Length );
      Array.Copy( arrColumnInfo, m_arrColumnInfo, iCount );

      //UpdateIndexes( m_arrColumnInfo.Values, sourceSheet, hashExtFormatIndexes );
      UpdateIndexes( m_arrColumnInfo, sourceSheet, hashExtFormatIndexes );

      if( hashExtFormatIndexes != null )
      {
        int iDefaultXFIndex = sourceSheet.ParentWorkbook.DefaultXFIndex;
        //object normalStyleIndex = hashExtFormatIndexes[ iDefaultXFIndex ];
        int iNormalStyleIndex;

        if( hashExtFormatIndexes.TryGetValue( iDefaultXFIndex, out iNormalStyleIndex ) )
        {
          List<int> arrIsDefaultColumnWidth = null;

          if( iNormalStyleIndex != iDefaultXFIndex )
            arrIsDefaultColumnWidth = CreateColumnsOnUpdate( m_arrColumnInfo, iNormalStyleIndex );

          // TODO: here we have to update column width according to the new normal style keeping column width in pixels.
          double dRatio = -1;
          int iDefaultWidth = -1;
          int iCurrentIndex = 0;

          for( int i = 1, len = m_arrColumnInfo.Length; i < len; i++ )
          {
            ColumnInfoRecord colInfo = m_arrColumnInfo[ i ];

            if( colInfo != null )
            {
              if( IsDefaultColumnWidth( arrIsDefaultColumnWidth, ref iCurrentIndex, i ) )
              {
                if( iDefaultWidth < 0 )
                {
                  int iOldColumnWidth = colInfo.ColumnWidth;
                  int iWidthInPixels = sourceSheet.GetColumnWidthInPixels( i );
                  SetColumnWidthInPixels( i, iWidthInPixels );
                  iDefaultWidth = colInfo.ColumnWidth;
                }
                else
                {
                  colInfo.ColumnWidth = ( ushort )iDefaultWidth;
                }
              }
              else if( dRatio < 0 )
              {
                int iOldColumnWidth = colInfo.ColumnWidth;
                int iWidthInPixels = sourceSheet.GetColumnWidthInPixels( i );
                SetColumnWidthInPixels( i, iWidthInPixels );
                dRatio = colInfo.ColumnWidth / ( double )iOldColumnWidth;
              }
              else
              {
                colInfo.ColumnWidth = ( ushort )( colInfo.ColumnWidth * dRatio );
              }
            }
          }
        }
      }
    }
    /// <summary>
    /// Returns true if specified column index can be found inside list of columns with default width.
    /// </summary>
    /// <param name="arrIsDefaultColumnWidth">List of columns with default width (sorted).</param>
    /// <param name="startIndex">Start index in the list.</param>
    /// <param name="columnIndex">Column index to check.</param>
    /// <returns>True if specified column index can be found inside list if columns.</returns>
    private bool IsDefaultColumnWidth( List<int> arrIsDefaultColumnWidth, ref int startIndex, int columnIndex )
    {
      if( arrIsDefaultColumnWidth == null )
        return false;

      int iLength = arrIsDefaultColumnWidth.Count;

      if( iLength == 0 )
        return false;

      if( startIndex >= iLength )
        return false;

      int iCurrentItem = arrIsDefaultColumnWidth[ startIndex ];

      while( iCurrentItem < columnIndex )
      {
        startIndex++;

        if( startIndex >= iLength )
          return false;

        iCurrentItem = arrIsDefaultColumnWidth[ startIndex ];
      }

      if( iCurrentItem == columnIndex )
        return true;

      return false;
    }
    /// <summary>
    /// Updates indexes of extended formats.
    /// </summary>
    /// <param name="collection">Collection with IOutline elements that should be updated.</param>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    private void UpdateIndexes( ICollection collection, WorksheetImpl sourceSheet,
      Dictionary<int, int> hashExtFormatIndexes )
    {
      UpdateIndexes( collection, sourceSheet, hashExtFormatIndexes, true );
    }
    /// <summary>
    /// Updates indexes of extended formats.
    /// </summary>
    /// <param name="collection">Collection with IOutline elements that should be updated.</param>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="bUpdateDefault">Indicates whether update default format index.</param>
    private void UpdateIndexes( ICollection collection, WorksheetImpl sourceSheet,
      Dictionary<int, int> hashExtFormatIndexes, bool bUpdateDefault )
    {
      if( collection == null )
        throw new ArgumentNullException( "collection" );

      // If worksheets are in same workbook, everything is ok.
      if( sourceSheet.Workbook == Workbook ) return;

      WorkbookImpl book = sourceSheet.ParentWorkbook;
      int iDefaultXFIndex = m_book.DefaultXFIndex;

      // Otherwise, update indexes.
      foreach( IOutline info in collection )
      {
        if( info == null ) continue;

        int index = info.ExtendedFormatIndex;
        if (hashExtFormatIndexes.ContainsKey(index))
        {
            index = hashExtFormatIndexes[index];

            if (bUpdateDefault || index == iDefaultXFIndex)
                info.ExtendedFormatIndex = (ushort)index;
        }
      }
    }
    /// <summary>
    /// Updates indexes of extended formats.
    /// </summary>
    /// <param name="collection">Collection with IOutline elements that should be updated.</param>
    /// <param name="extFormatIndexes">Array with new extended format indexes.</param>
    private void UpdateOutlineIndexes( ICollection collection, int[] extFormatIndexes )
    {
      if( collection == null )
        throw new ArgumentNullException( "collection" );

      // Otherwise, update indexes.
      foreach( IOutline info in collection )
      {
        if( info == null )
          continue;

        int index = info.ExtendedFormatIndex;
        index = extFormatIndexes[ index ];
        info.ExtendedFormatIndex = ( ushort )index;
      }
    }
    /// <summary>
    /// Creates columns on update.
    /// </summary>
    /// <param name="columns">Represents column collection.</param>
    /// <param name="iXFIndex">Represents new XF indexes.</param>
    /// <returns>List of columns that had default column width and were create by this method.
    /// Items in this list are placed in ascending order.</returns>
    private List<int> CreateColumnsOnUpdate( ColumnInfoRecord[] columns, int iXFIndex )
    {
      if( columns == null )
        throw new ArgumentNullException( "columns" );

      List<int> arrResult = new List<int>();

      for( int i = 1; i <= m_book.MaxColumnCount; i++ )
      {
        if( columns[ i ] == null )
        {
          ColumnInfoRecord record = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );

          record.FirstColumn = record.LastColumn = ( ushort )( i - 1 );
          record.ExtendedFormatIndex = ( ushort )iXFIndex;

          columns[ i ] = record;
          arrResult.Add( i );
        }
      }

      return arrResult;
    }
    /// <summary>
    /// Copies merged regions.
    /// </summary>
    /// <param name="sourceSheet">Source worksheet.</param>
    protected void CopyMerges( WorksheetImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      MergeCellsImpl merged = sourceSheet.MergeCells;

      if( merged != null )
        m_mergedCells = ( MergeCellsImpl )CloneUtils.CloneCloneable( merged, this );

      //      IRange[] merges = sourceSheet.MergedCells;
      //
      //      for( int i = 0, len = merges.Length; i < len; i++ )
      //      {
      //        string address = merges[ i ].AddressLocal;
      //        Range[ address ].Merge();
      //      }
    }

    /// <summary>
    /// 
    /// </summary>
    protected void AttachEvents()
    {
      // styles can be parsed after worksheet creation.
      if( !m_book.Styles.Contains( "Normal" ) && m_book.Loading )
        return;

      ( m_book.Styles[ "Normal" ].Font as FontWrapper ).AfterChangeEvent
        //+= new ValueChangedEventHandler( NormalFont_OnAfterChange );
        += new EventHandler( NormalFont_OnAfterChange );
    }
    /// <summary>
    /// 
    /// </summary>
    protected void DetachEvents()
    {

      if (m_book != null && m_book.Styles != null && m_book.Styles.Contains("Normal"))
      {
        ( m_book.Styles[ "Normal" ].Font as FontWrapper ).AfterChangeEvent
          //-= new ValueChangedEventHandler( NormalFont_OnAfterChange );
          -= new EventHandler( NormalFont_OnAfterChange );
      }
    }
    /// <summary>
    /// This method is called during dispose operation.
    /// </summary>
    protected override void OnDispose()
    {
      if( !m_bIsDisposed )
      {
          //base.Dispose();
          DetachEvents();
          m_arrAutoFilter = null;
          m_arrColumnInfo = null;
          m_arrConditionalFormats = null;
          m_arrCustomProperties = null;
          m_arrDConRecords = null;
          m_arrNotes = null;
          m_arrNotesByCellIndex = null;
          m_arrRecords = null;
          m_arrSelections = null;
          m_arrSortRecords = null;
          m_autofilters = null;
          if (m_autoFitManager != null)
          {
              m_autoFitManager.Dispose();
          }
          m_bof = null;

          if (m_dataHolder != null && m_book == null)
          {
              m_dataHolder.Dispose();
              m_dataHolder = null;
          }

          m_dataValidation = null;
            if(m_dicRecordsCells!=null)
          m_dicRecordsCells.Dispose();
            m_dataValidation = null;
            m_dictCFExRecords = null;
            m_dictCondFMT = null;
            m_errorIndicators = null;
            m_hyperlinks = null;
            if (m_inlineStrings != null)
            {
                m_inlineStrings.Clear();
                m_inlineStrings = null;
            }
            if (m_listObjects != null)
            {
                m_listObjects.Dispose();
                m_listObjects = null;
            }
            if (m_mergedCells != null)
            {
                m_mergedCells.Dispose();
                m_mergedCells = null;
            }
          m_migrantRange=null;
          m_nameIndexChanged = null;
#if !SILVERLIGHT && !WINRT && !WP
          if (m_oleObjects != null)
              m_oleObjects.Clear();
#endif
          m_pane = null;
          if(m_pivotTables!=null)
            m_pivotTables.Clear();
          m_preserveExternalConnection=null;

          if(m_preservePivotTables!=null)
            m_preservePivotTables.Clear();

          m_rawColRecord = null;
          m_rowHeightHelper = null;
          if(m_sparklineGroups!=null)
            m_sparklineGroups.Clear();
          if(m_tableRecords!=null)
              m_tableRecords.Clear();
          m_worksheetSlicer = null;
          
          if (m_calcEngine != null)
          {
              m_calcEngine.Dispose();
              m_calcEngine = null;
              ValueChanged = null;
          }
          if (m_book != null)
          {
              m_book.EnabledCalcEngine = false;
          }

        if( m_dicRecordsCells != null )
        {
          m_dicRecordsCells.Dispose();
          m_dicRecordsCells = null;
        }
        if (m_pageSetup != null)
        {
            m_pageSetup.Dispose();
        }
        //m_arrDefaultColumnStyle.Dispose();
        //m_arrDefaultRowStyle.Dispose();
        RowHeightChanged = null;
        ColumnWidthChanged = null;

        //m_book = null;

        GC.SuppressFinalize(this);
      }
    }
    /// <summary>
    /// Copies page setup from another worksheet.
    /// </summary>
    /// <param name="sourceSheet">Worksheet to copy from.</param>
    protected void CopyPageSetup( WorksheetImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      m_pageSetup = sourceSheet.m_pageSetup.Clone( this );
    }
    /// <summary>
    /// Imports extended format from anther worksheet.
    /// </summary>
    /// <param name="iXFIndex">Extended format to import.</param>
    /// <param name="basedOn">Source workbook.</param>
    /// <param name="hashExtFormatIndexes">Dictionary key - old xf index, value - new xf index.</param>
    /// <returns>Index of the new format.</returns>
    protected int ImportExtendedFormat( int iXFIndex, WorkbookImpl basedOn, Dictionary<int, int> hashExtFormatIndexes )
    {
      return m_book.InnerExtFormats.Import( basedOn.InnerExtFormats[ iXFIndex ], hashExtFormatIndexes );
    }
    /// <summary>
    /// Updates style indexes.
    /// </summary>
    /// <param name="styleIndexes">Array with changed style indexes.</param>
    protected internal override void UpdateStyleIndexes( int[] styleIndexes )
    {
      UpdateOutlineIndexes( m_arrColumnInfo, styleIndexes );
      m_dicRecordsCells.UpdateExtendedFormatIndex( styleIndexes );
    }
    #endregion

    #region Class properties
    internal Dictionary<int,int> IndexAndLevels
    {
        get
        {
            if (m_indexAndLevels == null)
                m_indexAndLevels = new Dictionary<int,int>();
            return m_indexAndLevels;
        }
    }
    /// <summary>
    ///  Stream to preserve the worksheet slicer
    /// </summary>
    internal Stream WorksheetSlicerStream
    {
        get
        {
            return m_worksheetSlicer;
        }
        set
        {
            m_worksheetSlicer = value;
        }
    }

    /// <summary>
    /// Read-only. Access to merged cells.
    /// </summary>
    public MergeCellsImpl MergeCells
    {
      get
      {
        ParseData();

        if( m_mergedCells == null )
          m_mergedCells = new MergeCellsImpl( Application, this );

        return m_mergedCells;
      }
    }
    /// <summary>
    /// Read-only. Access to column info records.
    /// </summary>
    [ CLSCompliant( false ) ]
    public /*IDictionary*/ColumnInfoRecord[]      ColumnInformation
    {
      get
      {
        ParseData();

        return m_arrColumnInfo;
      }
    }
    /// <summary>
    /// Gets or sets position of vertical split.
    /// </summary>
    public int VerticalSplit
    {
      get
      {
        ParseData();

        if( m_pane == null ) return 0;// int.MinValue;

        return m_pane.VerticalSplit;
      }
      set
      {
        ParseData();

        if( m_pane == null ) CreateEmptyPane();

        m_pane.VerticalSplit = ( ushort )value;
      }
    }
    /// <summary>
    /// Gets or sets position of horizontal split.
    /// </summary>
    public int HorizontalSplit
    {
      get
      {
        ParseData();

        if( m_pane == null ) return 0;// int.MinValue;

        return m_pane.HorizontalSplit;
      }
      set
      {
        ParseData();

        if( m_pane == null ) CreateEmptyPane();

        m_pane.HorizontalSplit = ( ushort )value;
      }
    }
    /// <summary>
    /// Gets or sets first visible row in bottom pane.
    /// </summary>
    public int FirstVisibleRow
    {
      get
      {
        ParseData();

        if( m_pane == null )
          return 0;

        return m_pane.FirstRow;//+1;
      }
      set
      {
        //if( value < 1 )
        //  throw new ArgumentOutOfRangeException();

        ParseData();

        if( m_pane == null ) CreateEmptyPane();

        m_pane.FirstRow = ( ushort )( value /*- 1*/ );
      }
    }
    /// <summary>
    /// Max_Coloumn width
    /// </summary>
    internal int MaxColumnWidth
    {
        get
        {
            return DEF_MAX_COLUMN_WIDTH;
        }
    }
    /// <summary>
    /// Gets or sets first visible column in right pane.
    /// </summary>
    public int FirstVisibleColumn
    {
      get
      {
        ParseData();

        if( m_pane == null )
          return 0;

        return m_pane.FirstColumn;
        //+1;
      }
      set
      {
        //if( value < 1 )
        //  throw new ArgumentOutOfRangeException();

        ParseData();

        if( m_pane == null ) CreateEmptyPane();

        m_pane.FirstColumn = ( ushort )( value /*- 1*/ );
      }
    }
    /// <summary>
    /// Worksheet's print area.
    /// </summary>
    public IRange PrintArea
    {
      get
      {
        // TODO: Change implementation.
        return UsedRange;
      }
    }
    /// <summary>
    /// Gets a value indicating selection count of pane.
    /// </summary>
    public int SelectionCount
    {
      get
      {
        ParseData();

        int result = 1;

        if( m_pane != null )
        {
          if( m_pane.VerticalSplit != 0 ) result *= 2;

          if( m_pane.HorizontalSplit != 0 ) result *= 2;
        }

        return result;
      }
    }
    /// <summary>
    /// Gets data validation collection.
    /// </summary>
    public DataValidationTable DVTable
    {
      get
      {
        ParseData();

        if( m_dataValidation == null )
          m_dataValidation = new DataValidationTable( Application, this );

        return m_dataValidation;
      }
    }
    /// <summary>
    /// Returns collection of worksheet's autofilters. Read-only.
    /// </summary>
    public IAutoFilters AutoFilters
    {
      get
      {
        ParseData();

        return m_autofilters;
      }
    }
    /// <summary>
    /// Collection of all hyperlinks in the current worksheet.
    /// </summary>
    public HyperLinksCollection InnerHyperLinks
    {
      get
      {
        ParseData();

        if( m_hyperlinks == null )
          m_hyperlinks = new HyperLinksCollection( Application, this );

        return m_hyperlinks;
      }
    }
    /// <summary>
    /// Collection of all hyperlinks in the current worksheet.
    /// </summary>
    public HyperLinksCollection InnerHyperLinksOrNull
    {
      get
      {
        ParseData();

        return m_hyperlinks;
      }
    }
    /// <summary>
    /// Gets or sets the view setting of the sheet.
    /// </summary>
    /// <value></value>
    public SheetView View
    {
        get
        {
            return m_view;
        }
        set
        {
            m_view = value;
        }
    }

    /// <summary>
    /// Return default row height in pixel.
    /// </summary>
    public int DefaultRowHeight
    {
      get
      {
        ParseData();

        return m_pageSetup.DefaultRowHeight;
      }
      set
      {
        ParseData();

        if( m_pageSetup.DefaultRowHeight != value )
        {
          if( StandardHeight != m_book.StandardRowHeight )
            m_pageSetup.DefaultRowHeightFlag = true;

          int oldValue = m_pageSetup.DefaultRowHeight;

          if( m_iFirstRow >= 0 && m_iLastRow >= 0 )
          {
            for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
            {
                RowStorage row = WorksheetHelper.GetOrCreateRow(this, i, false);

              if (row != null && !row.IsBadFontHeight && row.Height == oldValue)
              {
                  row.Height = (ushort)value;
                  row.IsBadFontHeight = true;
              }
            }
          }

          m_pageSetup.DefaultRowHeight = value;
        }
      }
    }
    /// <summary>
    /// Returns inner names collection. Read-only.
    /// </summary>
    public WorksheetNamesCollection InnerNames
    {
      get
      {
        return m_names;
      }
    }
    /// <summary>
    /// Returns inner data validation table. Read-only.
    /// </summary>
    public DataValidationTable InnerDVTable
    {
      get
      {
        return m_dataValidation;
      }
    }

    /// <summary>
    /// Returns collection of cell records. Read-only.
    /// </summary>
    public CellRecordCollection CellRecords
    {
      [DebuggerStepThrough]
      get
      {
        ParseData();

        return m_dicRecordsCells;
      }
    }
    /// <summary>
    /// Returns a PageSetup object that contains all the page setup settings
    /// for the specified object. Read-only.
    /// </summary>
    public override PageSetupBaseImpl PageSetupBase
    {
      get
      {
        ParseData();

        return m_pageSetup;
      }
    }
    internal bool m_parseCondtionalFormats = true;
    /// <summary>
    /// Returns collection with all conditional formats in the worksheet. Read-only.
    /// </summary>
    public WorksheetConditionalFormats ConditionalFormats
    {
      get
      {
        ParseData();
        if (m_parseCondtionalFormats)
            this.ParseSheetCF();
        return m_arrConditionalFormats;
      }
    }
    /// <summary>
    /// Gets pain record or null. Read-only.
    /// </summary>
    [CLSCompliant( false )]
    public PaneRecord Pane
    {
      get
      {
        ParseData();
        if (m_pane == null)
            m_pane = (PaneRecord)BiffRecordFactory.GetRecord(TBIFFRecord.Pane);
        return m_pane;
      }
    }
    /// <summary>
    /// Gets array with selection records or null. Read-only.
    /// </summary>
    [CLSCompliant( false )]
    public List<SelectionRecord> Selections
    {
      get
      {
        ParseData();
        return m_arrSelections;
      }
    }
    /// <summary>
    /// Returns collection of the custom properties. Read-only.
    /// </summary>
    public WorksheetCustomProperties InnerCustomProperties
    {
      get
      {
        ParseData();
        return m_arrCustomProperties;
      }
    }
    /// <summary>
    /// Indicates whether all created range objects should be cached. Default value is true.
    /// </summary>
    public bool UseRangesCache
    {
      get
      {
        ParseData();

        return m_dicRecordsCells.UseCache;
      }
      set
      {
        ParseData();

        m_dicRecordsCells.UseCache = value;
      }
    }
    /// <summary>
    /// Returns all autofilter records. Read-only.
    /// </summary>
    private List<BiffRecordRaw> AutoFilterRecords
    {
      get
      {
        ParseData();

        if( m_arrAutoFilter == null )
          m_arrAutoFilter = new List<BiffRecordRaw>();

        return m_arrAutoFilter;
      }
    }
    /// <summary>
    /// Returns all DCon records that were met in the source document. Read-only.
    /// </summary>
    private List<BiffRecordRaw> DConRecords
    {
      get
      {
        ParseData();

        if( m_arrDConRecords == null )
          m_arrDConRecords = new List<BiffRecordRaw>();

        return m_arrDConRecords;
      }
    }
    /// <summary>
    /// Returns all Sort records that were met in the source document. Read-only.
    /// </summary>
    private List<BiffRecordRaw> SortRecords
    {
      get
      {
        ParseData();

        if( m_arrSortRecords == null )
          m_arrSortRecords = new List<BiffRecordRaw>();

        return m_arrSortRecords;
      }
    }
    /// <summary>
    /// Represents error indicators.
    /// </summary>
    public ErrorIndicatorsCollection ErrorIndicators
    {
      get
      {
        ParseData();

        return m_errorIndicators;
      }
    }
    /// <summary>
    /// Returns quoted name of the worksheet.
    /// </summary>
    public string QuotedName
    {
      get
      {
        ParseData();
        return "'" + Name.Replace("'", "''") + "'";
      }
    }
    /// <summary>
    /// Gets or sets excel version.
    /// </summary>
    public ExcelVersion Version
    {
      get
      {
        //return m_dicRecordsCells.Version;
        return m_book.Version;
      }
      set
      {
        if( m_iLastRow != DEF_MIN_ROW_INDEX )
        {
          m_iLastRow = Math.Min( m_iLastRow, m_book.MaxRowCount );
        }

        if( m_iFirstRow != DEF_MIN_ROW_INDEX )
        {
          m_iFirstRow = Math.Min( m_iFirstRow, m_book.MaxRowCount );
        }

        if( m_iFirstColumn != DEF_MIN_COLUMN_INDEX )
        {
          m_iFirstColumn = Math.Min( m_iFirstColumn, m_book.MaxColumnCount );
        }

        if( m_iLastColumn != DEF_MIN_COLUMN_INDEX )
        {
          m_iLastColumn = Math.Min( m_iLastColumn, m_book.MaxColumnCount );
        }

        ColumnInfoRecord[] arrOldInfo = m_arrColumnInfo;

        m_arrColumnInfo = new ColumnInfoRecord[ m_book.MaxColumnCount + 2 ];

        Array.Copy( arrOldInfo, 0, m_arrColumnInfo, 0, Math.Min( arrOldInfo.Length, m_arrColumnInfo.Length ) );
        if (m_book.IsConverted && arrOldInfo[arrOldInfo.Length-1] !=null && m_rawColRecord !=null)
        {
            ColumnInfoRecord record = null;
            for (int i = arrOldInfo.Length; i < m_arrColumnInfo.Length; i++)
            {
                record = (m_rawColRecord.Clone() as ColumnInfoRecord);
                record.FirstColumn = (ushort)i;
                record.LastColumn = (ushort)i;
                m_arrColumnInfo[i] = record;
            }
        }

        m_dicRecordsCells.Version = value;

        HPageBreaksCollection hPagebreaks = ( HPageBreaksCollection )HPageBreaks;

        if( hPagebreaks != null && value == ExcelVersion.Excel97to2003 )
        {
          hPagebreaks.ChangeToExcel97to03Version();
        }

        VPageBreaksCollection vPagebreaks = ( VPageBreaksCollection )VPageBreaks;

        if( vPagebreaks != null && value == ExcelVersion.Excel97to2003 )
        {
          vPagebreaks.ChangeToExcel97to03Version();
        }

        if( value == ExcelVersion.Excel97to2003 && m_mergedCells != null )
        {
          m_mergedCells.SetNewDimensions( m_book.MaxRowCount, m_book.MaxColumnCount );
        }

        if( AutoFilters.Count != 0 )
        {
          AutoFiltersCollection autoFilters = ( AutoFiltersCollection )AutoFilters;
          autoFilters.ChangeVersions( m_book.MaxRowCount, m_book.MaxColumnCount, value );
        }

        FileDataHolder dataHolder = ( ( WorkbookImpl )Workbook ).DataHolder;

        if( value == ExcelVersion.Excel97to2003 && dataHolder != null )
        {
          ParseCFFromExcel2007( dataHolder );
          WorksheetConditionalFormats sheetCF = ConditionalFormats;

          foreach( ConditionalFormats conFormats in sheetCF )
          {
            conFormats.ConvertToExcel97to03Version();
          }
        }

        WorksheetNamesCollection names = InnerNames;

        if( names != null )
        {
          names.ConvertFullRowColumnNames( value );
        }

        if( m_pane != null &&
          ( m_pane.FirstRow > m_book.MaxRowCount - 1 ||
          m_pane.FirstColumn > m_book.MaxColumnCount - 1 ) )
        {
          m_pane = null;
        }

        ShapesCollection shapes = InnerShapes;

        if( shapes != null )
          shapes.SetVersion( value );

        if(Version== ExcelVersion.Excel97to2003)
            ClearPivotTables();
      }
    }
    /// <summary>
    /// Clears all pivot tables.
    /// </summary>
    private void ClearPivotTables()
    {
      if( m_pivotTables != null )
        m_pivotTables.Clear();
    }
    /// <summary>
    /// Returns object used for records creation/extraction from data provider. Read-only.
    /// </summary>
    public RecordExtractor RecordExtractor
    {
      get
      {
        return CellRecords.RecordExtractor;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    internal ItemSizeHelper RowHeightHelper
    {
      get
      {
        if( m_rowHeightHelper == null )
          m_rowHeightHelper = new ItemSizeHelper( GetRowHeightInPixels );

        return m_rowHeightHelper;
      }
    }
    /// <summary>
    /// Indicates whether IsHidden property is set.
    /// </summary>
    internal bool IsVisible
    {
        get
        {
            return m_isRowHeightSet;
        }
        set
        {
            m_isRowHeightSet = (bool)value;
        }
    }
    /// <summary>
    /// Indicates whether ZeroHeight property has enabled or not.
    /// </summary>
    internal bool IsZeroHeight
    {
        get
        {
            return m_isZeroHeight;
        }
        set
        {
            m_isZeroHeight = (bool)value;
        }
    }
        ///<summary>
        /// Specifies the number of characters of the maximum digit width of the normal style's font.
        ///</summary>
        internal int BaseColumnWidth
         {
            get
             {
                return m_baseColumnWidth;
             }
            set
            {
                m_baseColumnWidth = value;
            }
         }
        
      ///<summary>
        ///'True' if rows have a thick bottom border by default.
        /// </summary>
        internal bool IsThickBottom
         {
            get
            {
                return m_isThickBottom;
            }
            set
            {
                m_isThickBottom = value;
            }
         }
      ///<summary>
        /// 'True' if rows have a thick top border by default.
        /// </summary>
        internal bool IsThickTop
         {
            get
            {
                return m_isThickTop;
            }
            set
            {
                m_isThickTop = value;
            }
         }

      ///<summary>
        ///Highest number of outline levels for columns in this sheet.
        ///</summary>
        internal byte OutlineLevelColumn
         {
            get
            {
                return m_outlineLevelColumn;
            }
            set
            {
                m_outlineLevelColumn = value;
            }
         }
        ///<summary>
        ///Highest number of outline level for rows in this sheet.
        /// </summary>
        internal byte OutlineLevelRow
         {
            get
            {
                return m_outlineLevelRow;
            }
            set
            {
                m_outlineLevelRow = value;
            }
         }

        ///<summary>
        ///Highest number of outline level for rows in this sheet.
        /// </summary>
        internal bool CustomHeight
        {
            get
            {
                return m_isCustomHeight ;
            }
            set
            {
                m_isCustomHeight  = value;
            }
        }
      /// <summary>
      /// Returns the rows outline level count
      /// </summary>
      public int RowsOutlineLevel
      {
          get
          {
              return (int)OutlineLevelRow;
          }
      }
      /// <summary>
      /// Returns the columns outline level count
      /// </summary>      
      public int ColumnsOutlineLevel
      {
          get
          {
              return (int)OutlineLevelColumn;          
          }
      }
      /// <summary>
      /// Returns or sets the list of outline wrapper collection
      /// </summary>
      public List<IOutlineWrapper> OutlineWrappers
      {
          get
          {
              if (m_outlineWrappers == null)
              {
                  OutlineWrappers = new List<IOutlineWrapper>();
                  
                  CreateOutlineWrappers(ColumnOutlineLevels, ExcelGroupBy.ByColumns);
                  CreateOutlineWrappers(RowOutlineLevels, ExcelGroupBy.ByRows);
              }
              return m_outlineWrappers;
          }
          set
          {
              m_outlineWrappers = value;
          }
      }

     
    /// <summary>
    /// Indicates whether worksheet has merged cells. Read-only.
    /// </summary>
    public bool HasMergedCells
    {
      get
      {
        return ( m_mergedCells != null && m_mergedCells.MergeCount > 0 );
      }
    }
    /// <summary>
    /// Gets collection of all list objects in the worksheet.
    /// </summary>
    public ListObjectCollection InnerListObjects
    {
      get
      {
        return m_listObjects;
      }
    }
    /// <summary>
    /// Gets default protection options for the worksheet.
    /// </summary>
    protected override ExcelSheetProtection DefaultProtectionOptions
    {
      get
      {
        return ExcelSheetProtection.LockedCells | ExcelSheetProtection.UnLockedCells;
      }
    }
    protected override ExcelSheetProtection UnprotectedOptions
    {
      get
      {
        return ExcelSheetProtection.Content;
      }
    }
    /// <summary>
    /// Gets the inline strings.
    /// </summary>
    /// <value>The inline strings.</value>
    internal Dictionary<string, string> InlineStrings
    {
        get
        {
            if (m_inlineStrings == null)
                m_inlineStrings = new Dictionary<string, string>();

            return m_inlineStrings;
        }
    }
    /// <summary>
    /// Preserves the External connection setting in the worksheet.
    /// </summary>
    internal List<BiffRecordRaw> PreserveExternalConnection
    {
        get
        {
            if (m_preserveExternalConnection == null)
                m_preserveExternalConnection = new List<BiffRecordRaw>();
            return m_preserveExternalConnection;
        }
    }
    /// <summary>
    /// Preserves the pivot tables.
    /// </summary>
    internal List<Stream> PreservePivotTables
    {
        get
        {
            if (m_preservePivotTables == null)
                m_preservePivotTables = new List<Stream>();
            return m_preservePivotTables;
        }
    }
    /// <summary>
    /// Gets or sets the boolean value to load worksheets on demand
    /// </summary>
    internal override bool ParseDataOnDemand
    {
        get
        {
            return m_bParseDataOnDemand;
        }
        set
        {
            m_bParseDataOnDemand = value;
        }
    }

    /// <summary>
    /// Return or sets the columns outline levels collection
    /// </summary>
    internal Dictionary<int, List<Point>> ColumnOutlineLevels
    {
        get
        {
            if (m_columnOutlineLevels == null)
                m_columnOutlineLevels = new Dictionary<int, List<Point>>();
            return m_columnOutlineLevels;
        }
        set
        {
            m_columnOutlineLevels = value;
        }
    }
    /// <summary>
    /// Returns the row outline levels collection
    /// </summary>
    internal Dictionary<int, List<Point>> RowOutlineLevels
    {
        get
        {
            if (m_rowOutlineLevels == null)
                m_rowOutlineLevels = new Dictionary<int, List<Point>>();
            return m_rowOutlineLevels;
        }
        set
        {          
            m_rowOutlineLevels = value;
        }
    }
    
    #endregion

    #region IWorksheet Properties

    /// <summary>
    /// Get cell by row and index.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        return this.Range[ row, column ];
      }
    }
    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        return this.Range[ row, column, lastRow, lastColumn ];
      }
    }
    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name ]
    {
      get
      {
        return this[ name, false ];
      }
    }
    /// <summary>
    /// Get cell range.
    /// </summary>
    public IRange this[ string name, bool IsR1C1Notation ]
    {
      get
      {
        return this.Range[ name, IsR1C1Notation ];
      }
    }
    /// <summary>
    /// Gets / sets index of the active pane.
    /// </summary>
    public int ActivePane
    {
      get
      {
        ParseData();

        if( m_pane == null ) return int.MinValue;

        return m_pane.ActivePane;
      }
      set
      {
        ParseData();

        if( m_pane == null ) CreateEmptyPane();

        m_pane.ActivePane = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns all used cells in the worksheet. Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        return UsedRange.Cells;
      }
    }
    internal ColumnCollection Columnss
    {
        get
        {
            if (this.columnCollection != null)
                return this.columnCollection;
            else
            {
                int num = (this.GetAppImpl().GetFontCalc2() * 8) + this.GetAppImpl().GetFontCalc3();
                int num2 = ((num / 8) + 1) * 8;
                double defaultWidth = 8.0 + (((num2 - num) * 1.0) / ((double)this.GetAppImpl().GetFontCalc2()));
                this.columnCollection = new ColumnCollection(this, defaultWidth);
                return this.columnCollection;
            }
        }
    }

    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all used columns on the specified worksheet. Read-only Range object.
    /// </summary>
    public IRange[] Columns
    {
      get
      {
        return UsedRange.Columns;
      }
    }

    /// <summary>
    /// True if page breaks (both automatic and manual) on the specified
    /// worksheet are displayed. Read / write Boolean.
    /// </summary>
    public bool DisplayPageBreaks
    {
      get
      {
        ParseData();

        return m_bDisplayPageBreaks;
      }
      set
      {
        ParseData();

        if( m_bDisplayPageBreaks != value )
        {
          SetChanged();
          m_bDisplayPageBreaks = value;
        }
      }
    }
    internal AutoFitManager AutoFitManagerImpl
     {
         get
         {
             if (m_autoFitManager == null)
                 m_autoFitManager = new AutoFitManager();
             return m_autoFitManager;
         }
 
         set { m_autoFitManager = value; }
     }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Gets the OLE objects.
    /// </summary>
    /// <value>The OLE objects.</value>
    public IOleObjects OleObjects
    {
      get
      {
        if( m_oleObjects == null )
        {
          m_oleObjects = new OleObjects( this );
        }

        return m_oleObjects;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is OLE object.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is OLE object; otherwise, <c>false</c>.
    /// </value>
    public bool HasOleObject
    {
      get
      {
        return m_oleObjects != null && m_oleObjects.Count > 0 ;
      }
    }
#endif
    /// <summary>
        /// Gets the sparkline groups.
        /// </summary>
        /// <value>The sparkline groups.</value>
        public ISparklineGroups SparklineGroups
        {
            get
            {
              if( m_book.Loading && Version == ExcelVersion.Excel2007 )
              {
                m_book.Version = ExcelVersion.Excel2010;
              }

                if (Version == ExcelVersion.Excel2010)
                {
                    if (m_sparklineGroups == null)
                        m_sparklineGroups = new SparklineGroups( m_book );

                    return m_sparklineGroups;
                }
                else
                {
                    throw new NotSupportedException("Sparkline is not supported for the Current Version");
                }
            }
           
        }
        /// <summary>
    /// Returns an HPageBreaks collection that represents the horizontal
    /// page breaks on the sheet. Read-only.
    /// </summary>
    public IHPageBreaks HPageBreaks
    {
      get
      {
        ParseData();

        return m_pageSetup.HPageBreaks;
      }
    }
    /// <summary>
    /// Collection of all worksheet's hyperlinks.
    /// </summary>
    public IHyperLinks HyperLinks
    {
      get
      {
        return InnerHyperLinks;
      }
    }
    /// <summary>
    /// True if zero values to be displayed
    /// False otherwise.
    /// </summary>
    public bool IsDisplayZeros
    {
      get
      {
        ParseData();
        return WindowTwo.IsDisplayZeros;
      }
      set
      {
        ParseData();
        WindowTwo.IsDisplayZeros = value;
      }
    }
    /// <summary>
    /// True if gridlines are visible;
    /// False otherwise.
    /// </summary>
    public bool IsGridLinesVisible
    {
      get
      {
        ParseData();

        return WindowTwo.IsDisplayGridlines;
      }
      set
      {
        ParseData();

        WindowTwo.IsDisplayGridlines = value;
      }
    }
    /// <summary>
    /// True if row and column headers are visible.
    /// False otherwise.
    /// </summary>
    public bool IsRowColumnHeadersVisible
    {
      get
      {
        ParseData();

        return WindowTwo.IsDisplayRowColHeadings;
      }
      set
      {
        ParseData();

        WindowTwo.IsDisplayRowColHeadings = value;
      }
    }
    /// <summary>
    /// Indicates if all values in the workbook are preserved as strings.
    /// </summary>
    public bool IsStringsPreserved
    {
      get
      {
        ParseData();

        return m_bStringsPreserved;
      }
      set
      {
        ParseData();
        m_stringPreservedRanges.Clear();
        m_bStringsPreserved = value;
      }
    }
    /// <summary>
    /// Returns all merged ranges. Read-only.
    /// </summary>
    public IRange[] MergedCells
    {
      get
      {
        ParseData();

        int iMergeCount = ( m_mergedCells != null )
          ? m_mergedCells.MergeCount
          : 0;

        IRange[] result = ( iMergeCount > 0 )
          ? new IRange[ iMergeCount ]
          : null;

        if( result != null )
        {
          List<Rectangle> lstRanges = m_mergedCells.MergedRegions;

          for( int i = 0; i < iMergeCount; i++ )
          {
            Rectangle curRectangle = lstRanges[ i ];
            RangeImpl range = AppImplementation.CreateRange( this,
              curRectangle.X + 1, curRectangle.Y + 1, curRectangle.Right + 1, curRectangle.Bottom + 1 );
            result[ i ] = range;
          }
        }

        return result;
      }
    }
    /// <summary>
    /// Name used by macros to access to workbook items.
    /// </summary>
    public INames Names
    {
      get
      {
        return m_names;
      }
    }
    /// <summary>
    /// Returns a PageSetup object that contains all the page setup settings
    /// for the specified object. Read-only.
    /// </summary>
    public IPageSetup PageSetup
    {
      get
      {
        ParseData();

        return m_pageSetup;
      }
    }

    /// <summary>
    /// Gets or sets range indicating first visible row and column.
    /// </summary>
    public IRange PaneFirstVisible
    {
      get
      {
        ParseData();

        return AppImplementation.CreateRange( this, FirstVisibleColumn + 1, FirstVisibleRow + 1 );
      }
      set
      {
        ParseData();

        FirstVisibleRow = value.Row - 1;
        FirstVisibleColumn = value.Column - 1;
      }
    }
    /// <summary>
    /// Read-only. Returns a Range object that represents a cell or a range of cells.
    /// </summary>
    public IRange Range
    {
      [DebuggerStepThrough]
      get
      {
        return UsedRange;
      }
    }

    /// <summary>
    /// For a Worksheet object, returns an array of Range objects that represents
    /// all the rows on the specified worksheet. Read-only Range object.
    /// </summary>
    public IRange[] Rows
    {
      get
      {
        return UsedRange.Rows;
      }
    }
    /// <summary>
    /// Defines whether freeze panes are applied.
    /// </summary>
    public bool IsFreezePanes
    {
      get
      {
        return WindowTwo.IsFreezePanes;
      }
    }
    /// <summary>
    /// Gets or sets range for vertical and horizontal split
    /// </summary>
    public IRange SplitCell
    {
      get
      {
        ParseData();

        return AppImplementation.CreateRange( this, VerticalSplit + 1, HorizontalSplit + 1 );
      }
      set
      {
        ParseData();

        VerticalSplit = value.Column - 1;
        HorizontalSplit = value.Row - 1;
        WindowTwo.IsFreezePanes = true;
        WindowTwo.IsFreezePanesNoSplit = true;
      }
    }
    /// <summary>
    /// Gets or sets standard ( default ) height of all the rows in the worksheet,
    /// in points. Double.
    /// </summary>
    public double StandardHeight
    {
      get
      {
        return DefaultRowHeight / 20.0;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "Standard Row Height" );

        DefaultRowHeight = ( int )( value * 20 );
      }
    }

    /// <summary>
    /// Gets or sets the standard (default) height option flag, which defines that
    /// standard (default) row height and book default font height do not match. Bool.
    /// </summary>
    public bool StandardHeightFlag
    {
      get
      {
        ParseData();

        return m_pageSetup.DefaultRowHeightFlag;
      }
      set
      {
        ParseData();

        m_pageSetup.DefaultRowHeightFlag = value;
      }
    }

    /// <summary>
    /// Returns or sets the standard ( default ) width of all the columns in the
    /// worksheet. Read/write Double.
    /// </summary>
    public double StandardWidth
    {
      get
      {
        ParseData();

        //return m_iStandardColWidth / 256.0;
        return m_dStandardColWidth;
        //return Application.StandardWidth;
      }
      set
      {
        ParseData();

        m_dStandardColWidth = value;
        //        Application.StandardWidth = value;
        //        int newValue = ( int )( value * 256 );
        //        if( m_iStandardColWidth != newValue )
        //        {
        //          m_iStandardColWidth = newValue;
        //          SetChanged();
        //        }
      }
    }

    /// <summary>
    /// Returns or sets the worksheet type. Read-only ExcelSheetType.
    /// </summary>
    public ExcelSheetType Type
    {
      get
      {
        return m_sheetType;
      }
      set
      {
        m_sheetType = value;

        if( !IsSupported && m_sheetType == ExcelSheetType.Worksheet )
          IsSupported = true;
      }
    }

    /// <summary>
    /// Returns a Range object that represents the used range on the
    /// specified worksheet. Read-only.
    /// </summary>
    public IRange UsedRange
    {
      //[ DebuggerStepThrough() ]
      get
      {
        ParseData();
        // If no cells in worksheet.
        if( m_iFirstColumn == m_iLastColumn && m_iFirstColumn == DEF_MIN_COLUMN_INDEX
          || m_iFirstRow == m_iLastRow && m_iFirstRow < 0 )
        {
          if( m_rngUsed != null ) m_rngUsed.Dispose();

          m_rngUsed = AppImplementation.CreateRange( this );
        }
        else
        {
          int iFirstRow = m_iFirstRow;
          int iFirstColumn = m_iFirstColumn;
          int iLastRow = m_iLastRow;
          int iLastColumn = m_iLastColumn;

          GetRangeCoordinates( ref iFirstRow, ref iFirstColumn, ref iLastRow, ref iLastColumn );
          CreateUsedRange( iFirstRow, iFirstColumn, iLastRow, iLastColumn );
        }

        return m_rngUsed;
      }
    }
    /// <summary>
    /// Returns all not empty or accessed cells. Read-only.
    /// WARNING: This property creates Range object for each cell in the worksheet
    /// and creates new array each time user calls to it. It can cause huge memory
    /// usage especially if called frequently.
    /// </summary>
    public IRange[] UsedCells
    {
      get
      {
        ParseData();

        //IRange[] arrResult = new IRange[ m_dicRecordsCells.Count ];
        List<IRange> arrResult = new List<IRange>();
        int i = 0;

        foreach( DictionaryEntry entry in m_dicRecordsCells )
        {
			if(entry .Value != null)
			{
				ICellPositionFormat cell = entry.Value as ICellPositionFormat;
				arrResult.Add( InnerGetCell( cell.Column + 1, cell.Row + 1 ) );
				i++;
			}
        }
        return arrResult.ToArray();
      }
    }
    /// <summary>
    /// Returns a VPageBreaks collection that represents the vertical page
    /// breaks on the sheet. Read-only.
    /// </summary>
    public IVPageBreaks VPageBreaks
    {
      get
      {
        ParseData();
        return m_pageSetup.VPageBreaks;
      }
    }
    /// <summary>
    /// Indicates whether worksheet is empty. Read-only.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        ParseData();
        return m_iFirstRow == DEF_MIN_ROW_INDEX;
      }
    }
    /// <summary>
    /// Returns collection of custom properties. Read-only.
    /// </summary>
    public IWorksheetCustomProperties CustomProperties
    {
      get
      {
        ParseData();

        if( m_arrCustomProperties == null )
        {
          m_arrCustomProperties = new WorksheetCustomProperties();
        }

        return m_arrCustomProperties;
      }
    }
    /// <summary>
    /// Returns instance of migrant range - row and column of this range
    /// object can be changed by user. Read-only.
    /// </summary>
    public IMigrantRange MigrantRange
    {
      get
      {
        ParseData();

        if( m_migrantRange == null )
          CreateMigrantRange();

        return m_migrantRange;
      }
    }
    /// <summary>
    /// There are two different algorithms to create UsedRange object:
    /// 1) Default. This property = true. The cell is included into UsedRange when
    /// it has some record created for it even if data is empty (maybe some formatting
    /// changed, maybe not - cell was accessed and record was created).
    /// 2) This property = false. In this case XlsIO tries to remove empty rows and
    /// columns from all sides to make UsedRange smaller.
    /// </summary>
    public bool UsedRangeIncludesFormatting
    {
      get
      {
        return m_bUsedRangeIncludesFormatting;
      }
      set
      {
        m_bUsedRangeIncludesFormatting = value;
      }
    }
    /// <summary>
    /// Returns pivot tables collection.
    /// </summary>
    public IPivotTables PivotTables
    {
      get
      {
        if( m_pivotTables == null )
          m_pivotTables = new PivotTableCollection( Application, this );

        return m_pivotTables;
      }
    }
    /// <summary>
    /// Retruns internal  pivot table collection. Read-only.
    /// </summary>
    public PivotTableCollection InnerPivotTables
    {
      get
      {
        return m_pivotTables;
      }
    }
    /// <summary>
    /// Gets collection of all list objects in the worksheet.
    /// </summary>
    public IListObjects ListObjects
    {
      get
      {
        if( m_listObjects == null )
          m_listObjects = new ListObjectCollection(this);

        return m_listObjects;
      }
    }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    public override bool ProtectContents
    {
      get
      {
        return ( InnerProtection & ExcelSheetProtection.Content ) == 0;
        //return m_bIsProtected;
      }
      internal set
      {
        //m_bIsProtected = value;
        if( !value )
        {
          InnerProtection |= ExcelSheetProtection.Content;
        }
        else
        {
          InnerProtection &= ~ExcelSheetProtection.Content;
        }
      }
    }
    #endregion

    #region Class Events
    /// <summary>
    /// This event is raised after column width changed. 
    /// </summary>
    public event ValueChangedEventHandler ColumnWidthChanged;
    /// <summary>
    /// This event is raised after column height changed. 
    /// </summary>
    public event ValueChangedEventHandler RowHeightChanged;
    #endregion

    #region Class helper methods
   /// <summary>
   /// Gets outline levels dictionary collection create outline wrapper collection
   /// </summary>
   /// <param name="m_outlineRanges"></param>
   /// <param name="groupBy"></param>
   /// <param name="workSheet"></param>
    private void CreateOutlineWrappers(Dictionary<int, List<Point>> outlineLevels, ExcelGroupBy groupBy)
    {
        if (outlineLevels != null)
        {
            foreach (int key in outlineLevels.Keys)
            {
                List<Point> points = outlineLevels[key];

                for (int i = 0; i < points.Count; i++)
                {
                    OutlineWrapper m_outlineWrapper = new OutlineWrapper();

                    m_outlineWrapper.OutlineLevel = (ushort)key;
                    m_outlineWrapper.FirstIndex = points[i].X;
                    m_outlineWrapper.LastIndex = points[i].Y;
                    m_outlineWrapper.GroupBy = groupBy;

                    IOutline outline;

                    if (groupBy == ExcelGroupBy.ByRows)
                    {
                        m_outlineWrapper.OutlineRange = this[points[i].X, 1, points[i].Y, 1];
                        outline = (m_outlineWrapper.OutlineRange as RangeImpl).GetRowOutline(m_outlineWrapper.LastIndex);
                        // outline.IsCollapsed = outline.IsHidden;
                    }
                    else
                    {
                        m_outlineWrapper.OutlineRange = this[1, points[i].X, 1, points[i].Y];
                        outline = (m_outlineWrapper.OutlineRange as RangeImpl).GetColumnOutline(m_outlineWrapper.LastIndex);
                        // outline.IsCollapsed = outline.IsHidden;                     
                    }

                    m_outlineWrapper.Outline = outline;
                    m_outlineWrappers.Add(m_outlineWrapper);                   
                }
            }
        }
    }

   
    /// <summary>
    /// Gets object that is clone of current worksheet in the specified workbook.
    /// </summary>
    /// <param name="hashNewNames">Dictionary with update worksheet names.</param>
    /// <param name="book">New workbook object.</param>
    /// <returns>Object that is clone of the current worksheet.</returns>
    public IInternalWorksheet GetClonedObject( Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
      string strName = Name;

      if( hashNewNames != null )
      {
        string newName;

        if( hashNewNames.TryGetValue( strName, out newName ) )
        {
          strName = newName;
        }
      }

      return book.Worksheets[ strName ] as IInternalWorksheet;
    }
    /// <summary>
    /// Parses Excel 2007 conditional formatting.
    /// </summary>
    /// <param name="dataHolder">Workbook file data holder.</param>
    public void ParseCFFromExcel2007( FileDataHolder dataHolder )
    {
      if( dataHolder != null )
      {
        List<DxfImpl> lstDxfs = dataHolder.ParseDxfsCollection();
        
        WorksheetDataHolder holder = null;

        if( lstDxfs != null )
            holder = DataHolder;

        if( holder != null )
          holder.ParseConditionalFormatting( lstDxfs, this );
      }
    }
    /// <summary>
    /// Parses worksheet conditional formats in Excel2007 version.
    /// </summary>
    public void ParseSheetCF()
    {
      if( Version !=ExcelVersion.Excel97to2003 )
      {
          m_book.AppImplementation.IsFormulaParsed = false;          
          FileDataHolder dataHolder = m_book.DataHolder;
          ParseCFFromExcel2007( dataHolder );
          m_book.AppImplementation.IsFormulaParsed = true;
      }
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    public override void UpdateExtendedFormatIndex( Dictionary<int, int> dictFormats )
    {
      ParseData();

      base.UpdateExtendedFormatIndex( dictFormats );
      m_dicRecordsCells.UpdateExtendedFormatIndex( dictFormats );
      //UpdateOutlineAfterXFRemove( m_arrRow, dictFormats );
      UpdateOutlineAfterXFRemove( m_arrColumnInfo, dictFormats );
      //UpdateOutlineAfterXFRemove( m_arrColumnInfo.Values, dictFormats );
    }
    /// <summary>
    /// This method updates indexes to the extended formats after version change.
    /// </summary>
    /// <param name="maxCount">New restriction for maximum possible XF index.</param>
    public void UpdateExtendedFormatIndex( int maxCount )
    {
      ParseData();

      if( maxCount <= 0 )
        throw new ArgumentOutOfRangeException( "maxCount" );

      m_dicRecordsCells.UpdateExtendedFormatIndex( maxCount );
      int iDefaultXFIndex = m_book.DefaultXFIndex;

      for( int i = 0, len = m_arrColumnInfo.Length; i < len; i++ )
      {
        ColumnInfoRecord columnInfo = m_arrColumnInfo[ i ];

        if( columnInfo != null && columnInfo.ExtendedFormatIndex >= maxCount )
        {
          columnInfo.ExtendedFormatIndex = ( ushort )iDefaultXFIndex;
        }
      }
    }
    /// <summary>
    /// Creates Rtf string for LabelSST record.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Created rtf string.</returns>
    public RangeRichTextString CreateLabelSSTRTFString( long cellIndex )
    {
      ParseData();

      RangeRichTextString result = null;
      IRange range = m_dicRecordsCells.GetRange( cellIndex );

      if( range != null )
      {
        result = ( RangeRichTextString )range.RichText;
      }
      else
      {
        result = new RangeRichTextString( Application, this, cellIndex );
      }

      return result;
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="bIsError">If true - finds as error; otherwise as bool value.</param>
    /// <param name="bIsFindFirst">If true - finds first value; otherwise - all values.</param>
    /// <returns>If findfirst - true then returns range; otherwise - array with all found values.</returns>
    public IRange[] Find( IRange range, byte findValue, bool bIsError, bool bIsFindFirst )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      ParseData();

      List<long> arrIndexes = m_dicRecordsCells.Find( range, findValue, bIsError, bIsFindFirst );
      return ConvertCellListIntoRange( arrIndexes );
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="flags">Finding flags.</param>
    /// <param name="bIsFindFirst">If true - finds first value; otherwise - all values.</param>
    /// <returns>If findfirst - true then returns range; otherwise - array with all found values.</returns>
    public IRange[] Find( IRange range, double findValue
      , ExcelFindType flags, bool bIsFindFirst )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      ParseData();

      List<long> arrIndexes = m_dicRecordsCells.Find( range, findValue, flags, bIsFindFirst );
      return ConvertCellListIntoRange( arrIndexes );
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>Range array with found values.</returns>
    public IRange[] Find( IRange range, string findValue
      , ExcelFindType flags, bool bIsFindFirst )
    {
        return Find(range, findValue, flags, ExcelFindOptions.None, bIsFindFirst);
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>Range array with found values.</returns>
    public IRange[] Find(IRange range, string findValue
      , ExcelFindType flags,ExcelFindOptions findOptions, bool bIsFindFirst)
    {
        if (range == null)
            throw new ArgumentNullException("range");

        if (findValue == null || findValue.Length == 0)
            return null;

        ParseData();
        
        List<long> arrIndexes = m_dicRecordsCells.Find(range, findValue, flags, findOptions,bIsFindFirst);
        return ConvertCellListIntoRange(arrIndexes);
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies worksheet into the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      ParseData();

      m_book.CopyToClipboard( this );
    }
#endif
    /// <summary>
    /// Moves range to new position.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="options">Move options.</param>
    /// <param name="bUpdateRowRecords">
    /// Indicates whether row information such as row height, default style, etc.
    /// must be copied from cache.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// When source and destination ranges have different sizes.
    /// </exception>
    public void MoveRange( IRange destination, IRange source, ExcelCopyRangeOptions options,
      bool bUpdateRowRecords )
    {
      MoveRange( destination, source, options, bUpdateRowRecords, null );
    }
    /// <summary>
    /// Moves range to new position.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="options">Move options.</param>
    /// <param name="bUpdateRowRecords">
    /// Indicates whether row information such as row height, default style, etc.
    /// must be copied from cache.
    /// </param>
    /// <param name="beforeMove">Represents before move operation.</param>
    /// <exception cref="System.ArgumentException">
    /// When source and destination ranges have different sizes.
    /// </exception>
    private void MoveRange( IRange destination, IRange source, ExcelCopyRangeOptions options,
      bool bUpdateRowRecords, IOperation beforeMove )
    {
      ParseData();

      if( destination == source ) return;

      if( CanMove( ref destination, source ) )
      {
        WorksheetImpl destSheet = ( WorksheetImpl )destination.Worksheet;
        WorksheetImpl sourceSheet = ( WorksheetImpl )source.Worksheet;

        if( beforeMove != null )
          beforeMove.Do();

        int iSourceIndex = m_book.AddSheetReference( sourceSheet );
        int iDestIndex = m_book.AddSheetReference( destSheet );
        int iCurIndex = m_book.AddSheetReference( sourceSheet );

        Rectangle rectSource = Rectangle.FromLTRB( source.Column - 1, source.Row - 1,
          source.LastColumn - 1, source.LastRow - 1 );

        Rectangle rectDest = Rectangle.FromLTRB( destination.Column - 1, destination.Row - 1,
          destination.LastColumn - 1, destination.LastRow - 1 );

        int iRowDelta = destination.Row - source.Row;
        int iColDelta = destination.Column - source.Column;

        bool bUpdateFormula = ( options & ExcelCopyRangeOptions.UpdateFormulas ) != 0;
        RangeImpl destRange = ( RangeImpl )destination;

        int iMaxRow = 0;
        int iMaxColumn = 0;

        RecordTable cachedRecords = CacheAndRemoveFromParent( source, destination,
          ref iMaxRow, ref iMaxColumn, sourceSheet.m_dicRecordsCells );

        if( ( options & ExcelCopyRangeOptions.UpdateMerges ) != 0 )
          CopyRangeMerges( destination, source, true );

        //destSheet.PartialClearRange( rectDest );
        sourceSheet.PartialClearRange( rectSource );

        destSheet.CellRecords.ClearRange( rectDest );

        if (this.HyperLinks.Count > 0)
        {
            CopyMoveHyperlinks(rectSource.Y + 1, rectSource.X + 1, rectSource.Height + 1
             , rectSource.Width + 1, rectDest.Y + 1, rectDest.X + 1, destSheet, true);
        }   
        CopyCacheInto( cachedRecords, destSheet.m_dicRecordsCells.Table, bUpdateRowRecords );

        if( cachedRecords != null )
          cachedRecords.Dispose();
        //destSheet.m_dicRecordsCells.Table = cachedRecords;

        //destSheet.AddArrayFormulas( arrFormula );
        WorksheetHelper.AccessRow( destSheet, destRange.FirstRow );
        WorksheetHelper.AccessColumn( destSheet, destRange.FirstColumn );

        if( iMaxColumn > destRange.FirstColumn )
          WorksheetHelper.AccessColumn( destSheet, iMaxColumn );

        if( iMaxRow > destRange.FirstRow )
          WorksheetHelper.AccessRow( destSheet, iMaxRow );

        //m_shapes.MoveRange( rectSource, rectDest );
        //MoveShapes( sourceSheet.InnerShapes, rectSource, destSheet.InnerShapes, rectDest );

       

        if( ( options & ExcelCopyRangeOptions.CopyErrorIndicators ) != 0 )
        {
          sourceSheet.CopyMoveErrorIndicators( rectSource.Y + 1, rectSource.X + 1, rectSource.Height + 1
            , rectSource.Width + 1, rectDest.Y + 1, rectDest.X + 1, destSheet, true );
        }

        if( ( options & ExcelCopyRangeOptions.CopyConditionalFormats ) == ExcelCopyRangeOptions.CopyConditionalFormats )
        {
          sourceSheet.CopyMoveConditionalFormatting( rectSource.Y + 1, rectSource.X + 1, rectSource.Height + 1,
            rectSource.Width + 1, rectDest.Y + 1, rectDest.X + 1, destSheet, true );
        }
        if (bUpdateFormula)
        {
            m_book.UpdateFormula(iSourceIndex, rectSource, iDestIndex, rectDest);
        }

        if( ( options & ExcelCopyRangeOptions.CopyDataValidations ) == ExcelCopyRangeOptions.CopyDataValidations )
        {
          sourceSheet.CopyMoveDataValidations( rectSource.Y + 1, rectSource.X + 1, rectSource.Height + 1,
            rectSource.Width + 1, rectDest.Y + 1, rectDest.X + 1, destSheet, true );
        }

        if( ( options & ExcelCopyRangeOptions.CopyShapes ) != 0 )
        {
          rectSource.X += 1;
          rectSource.Y += 1;
          rectDest.X += 1;
          rectDest.Y += 1;

          ( ( ShapesCollection )Shapes ).CopyMoveShapeOnRangeCopy( destSheet, rectSource, rectDest, false );
        }
        //cachedRecords.CreateRangesIfNeccessary( destSheet );
      }
      else
      {
        //        sourceSheet.AddArrayFormulas( arrSourceFormulas );
        //        destSheet.AddArrayFormulas( ( ( RangeImpl )destination ).FormulaArrays );
        throw new InvalidRangeException();
      }
     // This method only useful for perform the used range move operation with in the worksheet.
     // UpdateHyperlinks((RangeImpl)source,(RangeImpl)destination);
    }
    /// <summary>
    /// Copies range from one range into another with formulas update.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <returns>Range into which source was copied.</returns>
    public IRange CopyRange( IRange destination, IRange source )
    {
      return CopyRange( destination, source, ExcelCopyRangeOptions.UpdateMerges );
    }
    /// <summary>
    /// Copies range from one range into another.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="options">Copy options.</param>
    /// <returns>Range into which source was copied.</returns>
    public IRange CopyRange( IRange destination, IRange source, ExcelCopyRangeOptions options )
    {
      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      ParseData();

      if( source.Worksheet != this )
      {
        WorksheetImpl sourceSheet = ( WorksheetImpl )source.Worksheet;
        return sourceSheet.CopyRange( destination, source );
      }

      RangeImpl sourceRange = ( RangeImpl )source;
      RangeImpl destRange = ( RangeImpl )destination;
      if (sourceRange.IsEntireRow)
      {
          if (source.RowHeight < 0)
              sourceRange.SetDifferedRowHeight(sourceRange, destRange);
          else
              destination.RowHeight = source.RowHeight;
      }

      if (sourceRange.IsEntireColumn)
      {
          if (source.ColumnWidth < 0)
              sourceRange.SetDifferedColumnWidth(sourceRange, destRange);
          else
              destination.ColumnWidth = source.ColumnWidth;
      }

      int iDestStartRow = destination.Row;
      int iDestStartCol = destination.Column;

      if( destRange.IsSingleCell && !sourceRange.IsSingleCell )
      {
        int iNewRow = iDestStartRow + sourceRange.LastRow - sourceRange.Row;
        int iNewColumn = iDestStartCol + sourceRange.LastColumn - sourceRange.Column;
        destRange = ( RangeImpl )destRange[ iDestStartRow, iDestStartCol,
          iNewRow, iNewColumn ];
        destination = destRange;
      }
      //      else
      //      {
      //        CheckRangesSizes( destination, source );
      //      }

      int iDestinationRows = destination.LastRow - iDestStartRow + 1;
      int iDestinationCols = destination.LastColumn - iDestStartCol + 1;
      int iSourceRows = source.LastRow - source.Row + 1;
      int iSourceCols = source.LastColumn - source.Column + 1;

      int iRowCount = 1;
      int iColCount = 1;

      if( iDestinationRows % iSourceRows == 0 && iDestinationCols % iSourceCols == 0 )
      {
        iRowCount = iDestinationRows / iSourceRows;
        iColCount = iDestinationCols / iSourceCols;
      }

      for( int iCurRow = 0; iCurRow < iRowCount; iCurRow++, iDestStartRow += iSourceRows )
      {
        for( int iCurCol = 0, iCurStartCol = iDestStartCol; iCurCol < iColCount;
          iCurCol++, iCurStartCol += iSourceCols )
        {
          destRange = ( RangeImpl )destination[ iDestStartRow, iCurStartCol,
            iDestStartRow + iSourceRows - 1, iCurStartCol + iSourceCols - 1 ];

          if( !destRange.AreFormulaArraysNotSeparated )
            throw new InvalidRangeException();

          CopyRangeWithoutCheck( sourceRange, destRange, options );
        }
      }

      return destination;
    }
    /// <summary>
    /// Copies range without checking range sizes.
    /// </summary>
    /// <param name="source">Source range to copy.</param>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy options.</param>
    public void CopyRangeWithoutCheck( RangeImpl source, RangeImpl destination,
      ExcelCopyRangeOptions options )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( !destination.AreFormulaArraysNotSeparated )
        throw new InvalidRangeException( "Can't copy to destination range." );

      ParseData();

      int iRowCount = source.LastRow - source.Row + 1;
      int iColumnCount = source.LastColumn - source.Column + 1; // check if +1 is needed
      Rectangle rectIntersection;

      RecordTable intersection = m_dicRecordsCells.CacheIntersection( destination, source, out rectIntersection );
      bool bUpdateFormula = ( options & ExcelCopyRangeOptions.UpdateFormulas ) != 0;

      Dictionary<ArrayRecord, object> hashFormulaToRemove = destination.FormulaArrays;
      WorksheetImpl destSheet = ( WorksheetImpl )destination.Worksheet;

      if( ( options & ExcelCopyRangeOptions.UpdateMerges ) != 0 )
        CopyRangeMerges( destination, source );

      if( hashFormulaToRemove != null && hashFormulaToRemove.Count > 0 )
      {
        destSheet.RemoveArrayFormulas( hashFormulaToRemove.Keys, true );
      }

      int iDestColumn = destination.Column;
      int iDestRow = destination.Row;
      int iSourceRow = source.Row;
      int iSourceColumn = source.Column;

      if( ( options & ExcelCopyRangeOptions.CopyShapes ) != 0 )
      {
        Rectangle sourceRec = new Rectangle( iSourceColumn, iSourceRow, iColumnCount - 1, iRowCount - 1 );
        Rectangle destRec = sourceRec;
        destRec.X = iDestColumn;
        destRec.Y = iDestRow;
        ShapesCollection shapes = ( ShapesCollection )Shapes;
        shapes.CopyMoveShapeOnRangeCopy( destSheet, sourceRec, destRec, true );
      }

      destination.Clear();

      if( ( options & ExcelCopyRangeOptions.CopyErrorIndicators ) == ExcelCopyRangeOptions.CopyErrorIndicators )
      {
        CopyMoveErrorIndicators( iSourceRow, iSourceColumn, iRowCount, iColumnCount, iDestRow, iDestColumn
          , destSheet, false );
      }

      if( ( options & ExcelCopyRangeOptions.CopyConditionalFormats ) == ExcelCopyRangeOptions.CopyConditionalFormats )
      {
        CopyMoveConditionalFormatting( iSourceRow, iSourceColumn, iRowCount, iColumnCount,
          iDestRow, iDestColumn, destSheet, false );
      }

      if( ( options & ExcelCopyRangeOptions.CopyDataValidations ) != 0 )
      {
        CopyMoveDataValidations( iSourceRow, iSourceColumn, iRowCount, iColumnCount, iDestRow, iDestColumn
          , destSheet, false );
      }

      CopyRange( iSourceRow, iSourceColumn, iRowCount, iColumnCount, iDestRow,
        iDestColumn, destSheet, intersection, rectIntersection, options );

      CopyMoveHyperlinks(iSourceRow, iSourceColumn, iRowCount, iColumnCount,
        iDestRow, iDestColumn, destSheet, false);

      if( intersection != null )
        intersection.Dispose();
    }
    /// <summary>
    /// Copies or moves data validations.
    /// </summary>
    /// <param name="iSourceRow">Represents source row index.</param>
    /// <param name="iSourceColumn">Represents source column index.</param>
    /// <param name="iRowCount">Represents row count.</param>
    /// <param name="iColumnCount">Represents column count.</param>
    /// <param name="iDestRow">Represents destination row index.</param>
    /// <param name="iDestColumn">Represents destination column index.</param>
    /// <param name="destSheet">Represents destination sheet.</param>
    /// <param name="bIsMove">Represents is move data validations.</param>
    private void CopyMoveDataValidations( int iSourceRow, int iSourceColumn, int iRowCount, int iColumnCount,
      int iDestRow, int iDestColumn, WorksheetImpl destSheet, bool bIsMove )
    {
      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      destSheet.ParseData();
      ParseData();

      if( iSourceColumn == DEF_MIN_COLUMN_INDEX || iDestColumn == DEF_MIN_COLUMN_INDEX
        || iSourceRow == DEF_MIN_ROW_INDEX || iDestRow == DEF_MIN_ROW_INDEX )
      {
        return;
      }

      DataValidationTable destDataValidation = destSheet.m_dataValidation;
      bool bSourceAbsent = ( m_dataValidation == null || m_dataValidation.Count == 0 );
      bool bDestinationAbsent = ( destDataValidation == null || destDataValidation.Count == 0 );

      if( bSourceAbsent && bDestinationAbsent )
      {
        return;
      }
      else if( bSourceAbsent )
      {
        Rectangle rect = new Rectangle( iDestColumn - 1, iDestRow - 1, iColumnCount, iRowCount );
        destDataValidation.Remove( new Rectangle[] { rect } );
      }
      else if( bDestinationAbsent )
      {
        destDataValidation = destSheet.DVTable;
      }
        
      // Here both source and destination data validation tables are present.
      if (m_dataValidation != null)
      {
          m_dataValidation.CopyMoveTo(destDataValidation,
            iSourceRow, iSourceColumn,
            iDestRow, iDestColumn,
            iRowCount, iColumnCount,
            bIsMove);

          if (m_dataValidation.Count == 0)
              m_dataValidation = null;
      }
      if( destDataValidation.Count == 0 )
        destSheet.m_dataValidation = null;

      //for( int i = 0, len = m_arrConditionalFormats.Count; i < len; i++ )
      //{
      //  ConditionalFormats sourceFormats = m_arrConditionalFormats[ i ];
      //  ConditionalFormats destFormats = sourceFormats.GetPart( iSourceRow, iSourceColumn,
      //    iRowCount, iColumnCount, bIsMove, iDestRow - iSourceRow, iDestColumn - iSourceColumn, destSheetCondFormats );


      //  if( sourceFormats.IsEmpty )
      //  {
      //    m_arrConditionalFormats.RemoveItem( sourceFormats );
      //    i--;
      //    len--;
      //  }

      //  if( destFormats != null )
      //    destSheetCondFormats.Add( destFormats );
      //}
    }
    /// <summary>
    /// Copies or moves conditional formats.
    /// </summary>
    /// <param name="iSourceRow">Represents source row index.</param>
    /// <param name="iSourceColumn">Represents source column index.</param>
    /// <param name="iRowCount">Represents row count.</param>
    /// <param name="iColumnCount">Represents column count.</param>
    /// <param name="iDestRow">Represents destination row index.</param>
    /// <param name="iDestColumn">Represents destination column index.</param>
    /// <param name="destSheet">Represents destination sheet.</param>
    /// <param name="bIsMove">Represents is move conditional formats.</param>
    private void CopyMoveConditionalFormatting( int iSourceRow, int iSourceColumn, int iRowCount, int iColumnCount,
      int iDestRow, int iDestColumn, WorksheetImpl destSheet, bool bIsMove )
    {
      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      destSheet.ParseSheetCF();
      ParseSheetCF();

      if( iSourceColumn == DEF_MIN_COLUMN_INDEX || iDestColumn == DEF_MIN_COLUMN_INDEX
        || iSourceRow == DEF_MIN_ROW_INDEX || iDestRow == DEF_MIN_ROW_INDEX )
      {
        return;
      }

      WorksheetConditionalFormats destSheetCondFormats = destSheet.ConditionalFormats;

      for( int i = 0, len = m_arrConditionalFormats.Count; i < len; i++ )
      {
        ConditionalFormats sourceFormats = m_arrConditionalFormats[ i ];
        int columnIncrement = iDestColumn - iSourceColumn;
        int rowIncrement=iDestRow - iSourceRow;
        int sheetIndex = destSheet.Index;
        ConditionalFormats destFormats = sourceFormats.GetPart( iSourceRow, iSourceColumn,
          iRowCount, iColumnCount, bIsMove, rowIncrement, columnIncrement, destSheetCondFormats );
        if (!bIsMove)
        {
            Rectangle srcRect = new Rectangle(iSourceColumn - 1, iSourceRow - 1, iColumnCount, iRowCount);
            Rectangle destRect = new Rectangle(iSourceColumn - 1, iSourceRow - 1, 0, 0);
            destRect.Offset(columnIncrement, rowIncrement);
            if (destFormats != null && destFormats[0].FormatType == ExcelCFType.Formula)
                destFormats.UpdateFormula(sheetIndex, sheetIndex, srcRect, sheetIndex, destRect);
        }
        if( sourceFormats.IsEmpty )
        {
          m_arrConditionalFormats.RemoveItem( sourceFormats );
          i--;
          len--;
        }

        if( destFormats != null )
          destSheetCondFormats.Add( destFormats );
      }
    }
    /// <summary>
    /// Copies or moves Hyperlinks.
    /// </summary>
    /// <param name="iSourceRow">Represents source row index.</param>
    /// <param name="iSourceColumn">Represents source column index.</param>
    /// <param name="iRowCount">Represents row count.</param>
    /// <param name="iColumnCount">Represents column count.</param>
    /// <param name="iDestRow">Represents destination row index.</param>
    /// <param name="iDestColumn">Represents destination column index.</param>
    /// <param name="destSheet">Represents destination sheet.</param>
    /// <param name="bIsMove">Represents is move Hyperlinks.</param>
    private void CopyMoveHyperlinks(int iSourceRow, int iSourceColumn, int iRowCount, int iColumnCount,
    int iDestRow, int iDestColumn, WorksheetImpl destSheet, bool bIsMove)
    {
        if (destSheet == null)
            throw new ArgumentNullException("destSheet");

        if (iSourceColumn == DEF_MIN_COLUMN_INDEX || iDestColumn == DEF_MIN_COLUMN_INDEX
          || iSourceRow == DEF_MIN_ROW_INDEX || iDestRow == DEF_MIN_ROW_INDEX)
        {
            return;
        }        

        for (int i = 0, len = iRowCount; i < len; i++)
        {
            for (int j = 0; j < iColumnCount; j++)
            {
                IRange sourceRange = Range[iSourceRow + i, iSourceColumn + j];
                IRange destRange = destSheet[iDestRow + i, iDestColumn + j];
                if (sourceRange.Hyperlinks.Count != 0)
                {                    
                    IHyperLink desthyper = destSheet.HyperLinks.Add(destRange);
                    IHyperLink sourcehyper = sourceRange.Hyperlinks[0];
                    desthyper.Type = sourcehyper.Type;
                    desthyper.Address = sourcehyper.Address;
                    desthyper.TextToDisplay = sourcehyper.TextToDisplay;
                    desthyper.ScreenTip = sourcehyper.ScreenTip;
                    if (bIsMove)
                    {
                        RemoveHyperlink(sourceRange);
                    }
                }
                else if (bIsMove && destRange.Hyperlinks.Count>0)
                {
                    RemoveHyperlink(destRange);
                }
            }           
        }
    }
    /// <summary>
    /// Remove the Hyperlink from the specific range.
    /// </summary>
    /// <param name="range"></param>
    private void RemoveHyperlink(IRange range)
    {
        bool isDeleted = false;
        for (int count = 0; count < this.HyperLinks.Count && !isDeleted; count++)
        {
            if (this.HyperLinks[count].Address == range.Hyperlinks[0].Address)
            {
                this.HyperLinks.RemoveAt(count);
                isDeleted = true;
            }
        }
    }
    /// <summary>
    /// Copies or move error indicators.
    /// </summary>
    /// <param name="iSourceRow">Represents source row index.</param>
    /// <param name="iSourceColumn">Represents source column index.</param>
    /// <param name="iRowCount">Represents row count.</param>
    /// <param name="iColumnCount">Represents column count.</param>
    /// <param name="iDestRow">Represents destination row index.</param>
    /// <param name="iDestColumn">Represents destination column index.</param>
    /// <param name="destSheet">Represents destination sheet.</param>
    /// <param name="bIsMove">Represents is move error indicators.</param>
    private void CopyMoveErrorIndicators( int iSourceRow, int iSourceColumn, int iRowCount, int iColumnCount
      , int iDestRow, int iDestColumn, WorksheetImpl destSheet, bool bIsMove )
    {
      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      ErrorIndicatorsCollection errorIndicators = destSheet.m_errorIndicators;

      for( int i = 0; i < iRowCount; i++ )
      {
        for( int j = 0; j < iColumnCount; j++ )
        {
          Rectangle rectangle = new Rectangle( iSourceColumn + j - 1, iSourceRow + i - 1, 0, 0 );
          Rectangle[] rectangles = new Rectangle[ 1 ] { rectangle };
          ErrorIndicatorImpl errorIndicator = m_errorIndicators.Find( rectangles );

          ErrorIndicatorImpl dest_ErrorIndicator = errorIndicators.Find(rectangles);

          if (errorIndicator != null)
          {
              if (bIsMove && dest_ErrorIndicator == null)
            {
              errorIndicator.Remove( rectangles );
            }

            errorIndicator = new ErrorIndicatorImpl( errorIndicator.IgnoreOptions );
            List<Rectangle> list = new List<Rectangle>();
            Rectangle newRectangle = new Rectangle( iDestColumn + j - 1, iDestRow + i - 1, 0, 0 );
            list.Add( newRectangle );

            
            ErrorIndicatorImpl existingErrorIndicator = errorIndicators.Find( new Rectangle[ 1 ] { newRectangle } );

            if( existingErrorIndicator != null && ( existingErrorIndicator.IgnoreOptions != errorIndicator.IgnoreOptions ) )
            {
              existingErrorIndicator.AddCells( list );
            }
            else
            {
              errorIndicator.AddCells( list );
              errorIndicators.Add( errorIndicator );
            }
          }
        }
      }
    }
    /// <summary>
    /// Copies cell into another worksheet.
    /// </summary>
    /// <param name="cell">Cell to copy.</param>
    /// <param name="strFormulaValue">Formula string value of the cell.</param>
    /// <param name="dicXFIndexes">
    /// Dictionary with updated extended format indexes,
    /// or Null if indexes were not updated.
    /// </param>
    /// <param name="lNewIndex">New cell index</param>
    /// <param name="book">Source workbook.</param>
    /// <param name="dicFontIndexes">
    /// Dictionary with updated font indexes or Null if indexes were not updated.
    /// </param>
    /// <param name="options">Copy options.</param>
    [CLSCompliant( false )]
    public void CopyCell( ICellPositionFormat cell, string strFormulaValue,
      IDictionary dicXFIndexes, long lNewIndex, WorkbookImpl book,
      Dictionary<int, int> dicFontIndexes, ExcelCopyRangeOptions options )
    {
      ParseData();

      bool bCreateRange = m_dicRecordsCells.CopyCell( cell, strFormulaValue, dicXFIndexes,
        lNewIndex, book, dicFontIndexes, options );

      int iRow = cell.Row;
      int iColumn = cell.Column;

      //      AccessRow( iRow + 1 );
      //      AccessColumn( iColumn + 1 );

      RangeImpl range = m_dicRecordsCells.GetRange( lNewIndex );

      //      if( bCreateRange && range == null )
      //      {
      //        range = Range[ iRow + 1, iColumn + 1 ] as RangeImpl;
      //      }

      if( range != null )
      {
        range.UpdateRecord();
      }
    }
    /// <summary>
    /// Copies cells from one range into another.
    /// </summary>
    /// <param name="iSourceRow">One-based index of the source row.</param>
    /// <param name="iSourceColumn">One-based index of the source column.</param>
    /// <param name="iRowCount">Number of rows to copy.</param>
    /// <param name="iColumnCount">Number of columns to copy.</param>
    /// <param name="iDestRow">One-based index of the destination row.</param>
    /// <param name="iDestColumn">One-based index of the destination column.</param>
    /// <param name="destSheet">Destination worksheet.</param>
    /// <param name="intersection">DictionaryEntry with records and strings intersection part.</param>
    /// <param name="rectIntersection">Rectangle with intersection information.</param>
    /// <param name="options">Copy options.</param>
    public void CopyRange( int iSourceRow, int iSourceColumn,
      int iRowCount, int iColumnCount, int iDestRow, int iDestColumn,
      WorksheetImpl destSheet, RecordTable intersection, Rectangle rectIntersection,
      ExcelCopyRangeOptions options )
    {
      ParseData();

      Dictionary<int, int> dicFontIndexes = null;
      Dictionary<int, int> dicXFIndexes = null;

      bool bCopyStyles = ( ( options & ExcelCopyRangeOptions.CopyStyles ) != 0 );

      if( bCopyStyles )
      {
        dicXFIndexes = GetUpdatedXFIndexes( iSourceRow, iSourceColumn,
          iRowCount, iColumnCount, destSheet, out dicFontIndexes );
      }

      CellRecordCollection destCells = destSheet.CellRecords;

      int iColDelta = iDestColumn - iSourceColumn;
      int iRowDelta = iDestRow - iSourceRow;
      Dictionary<long, long> hashArrayIndexes = new Dictionary<long, long>();
      RecordTable destTable = destSheet.CellRecords.Table;
      int iSourceBlockSize = Application.RowStorageAllocationBlockSize;

      // Now we should copy range cells.
      for( int row = 0; row < iRowCount; row++ )
      {
        int iNewRow = iDestRow + row;
        int iOldRow = iSourceRow + row;

        for( int column = 0; column < iColumnCount; column++ )
        {
          int iNewColumn = iDestColumn + column;
          int iOldColumn = iSourceColumn + column;
          long lDestIndex = RangeImpl.GetCellIndex( iNewColumn, iNewRow );
          //long lSourceIndex = RangeImpl.GetCellIndex( iOldColumn, iOldRow );

          RecordTable table = GetRecordTable( iOldRow, iOldColumn, rectIntersection,
            intersection, m_dicRecordsCells.Table );

          RowStorage arrSourceRow = ( RowStorage )table.Rows[ iOldRow - 1 ];

          ICellPositionFormat cell = ( arrSourceRow != null )
            ? arrSourceRow.GetRecord( iOldColumn - 1, iSourceBlockSize )
            : null;

          if( cell != null )
          {
            string strValue = arrSourceRow.GetFormulaStringValue( iOldColumn - 1 );
            destSheet.CopyCell( cell, strValue, dicXFIndexes, lDestIndex, m_book, dicFontIndexes, options );

            if( cell.TypeCode == TBIFFRecord.Formula )
            {
              ArrayRecord array = table.GetArrayRecord( cell );
              //ArrayRecord array = arrSourceRow.GetArrayRecord( iOldColumn - 1 );

              if( array != null )
              {
                long lCellIndex = RangeImpl.GetCellIndex( array.FirstColumn, array.FirstRow );
                int iNewArrayRowIndex = 0;
                int iNewArrayColumnIndex = 0;

                if( hashArrayIndexes.ContainsKey( lCellIndex ) )
                {
                  long lNewArrayIndex = hashArrayIndexes[ lCellIndex ];
                  iNewArrayRowIndex = RangeImpl.GetRowFromCellIndex( lNewArrayIndex );
                  iNewArrayColumnIndex = RangeImpl.GetColumnFromCellIndex( lNewArrayIndex );
                }
                else
                {
                  iNewArrayRowIndex = iNewRow - 1;
                  iNewArrayColumnIndex = iNewColumn - 1;
                  long lNewArrayIndex = RangeImpl.GetCellIndex( iNewArrayColumnIndex, iNewArrayRowIndex );
                  hashArrayIndexes[ lCellIndex ] = lNewArrayIndex;

                  array.FirstColumn = Math.Max( array.FirstColumn, iSourceColumn - 1 ) + iColDelta;
                  array.FirstRow = Math.Max( array.FirstRow, iSourceRow - 1 ) + iRowDelta;

                  array.LastColumn = Math.Min( array.LastColumn, iSourceColumn + iColumnCount - 2 );
                  array.LastColumn += iColDelta;

                  array.LastRow = Math.Min( array.LastRow, iSourceRow + iRowCount - 2 );
                  array.LastRow += iRowDelta;

                  if( ( options & ExcelCopyRangeOptions.UpdateFormulas ) != 0 )
                  {
                    UpdateArrayFormula( array, destSheet, iRowDelta, iColDelta );
                  }

                  RowStorage arrDestRow = ( RowStorage )destTable.Rows[ iNewRow - 1 ];
                  arrDestRow.SetArrayRecord( iNewColumn - 1, array, Application.RowStorageAllocationBlockSize );
                }

                // Here we have to update Formula tokens.
                RowStorage arrRow = ( RowStorage )destTable.Rows[ iNewRow - 1 ];

                if( arrRow != null )
                {
                  arrRow.SetArrayFormulaIndex( iNewColumn - 1, iNewArrayRowIndex,
                    iNewArrayColumnIndex, Application.RowStorageAllocationBlockSize );
                }
              }
            }
          }
          else
          {
            //destCells.ClearCell( iDestIndex );
            destCells.Remove( lDestIndex );
          }
        }
      }
    }
    /// <summary>
    /// Removes all formulas in colRemove from internal ArrayFormula collection.
    /// </summary>
    /// <param name="colRemove">Formulas to remove.</param>
    /// <param name="bClearRange">Indicates whether to clear range before remove operation.</param>
    public void RemoveArrayFormulas( ICollection<ArrayRecord> colRemove, bool bClearRange )
    {
      if( colRemove == null )
        throw new ArgumentNullException( "colRemove" );

      ParseData();

      foreach( ArrayRecord record in colRemove )
      {
        RemoveArrayFormula( record, bClearRange );
      }
    }

    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="arrFormula">Formula to update.</param>
    /// <param name="iRowOffset">Row offset.</param>
    /// <param name="iColOffset">Column offset.</param>
    /// <returns></returns>
    public Ptg[] UpdateFormula( Ptg[] arrFormula, int iRowOffset, int iColOffset )
    {
      if( arrFormula == null )
        throw new ArgumentNullException( "arrFormula" );

      ParseData();

      bool bUpdateFormula = iRowOffset != 0 || iColOffset != 0;

      Ptg[] arrResult = new Ptg[ arrFormula.Length ];

      for( int i = 0, len = arrFormula.Length; i < len; i++ )
      {
        if( bUpdateFormula )
        {
          arrResult[ i ] = arrFormula[ i ].Offset( iRowOffset, iColOffset, m_book );
        }
        else
        {
          arrResult[ i ] = ( Ptg )arrFormula[ i ].Clone();
        }
      }

      return arrResult;
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public override void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect, int iDestIndex,
      Rectangle destRect )
    {
      ParseData();

      m_dicRecordsCells.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      base.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );

      if( m_arrConditionalFormats != null )
        m_arrConditionalFormats.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }

    /// <summary>
    /// Autofits row.
    /// </summary>
    /// <param name="rowIndex">Row index.</param>
    public void AutofitRow( int rowIndex )
    {
      int iFirstColumn = UsedRange.Column;
      int iLastColumn = UsedRange.LastColumn;
      AutofitRow( rowIndex, iFirstColumn, iLastColumn, true );
    }
    /// <summary>
    /// Autofits column.
    /// </summary>
    /// <param name="colIndex">Column index.</param>
    public void AutofitColumn( int colIndex )
    {
       RangeImpl rangeImpl = this[UsedRange.Row,UsedRange.Column, UsedRange.LastRow,UsedRange.LastColumn] as RangeImpl;
         rangeImpl.AutoFitToColumn(colIndex, colIndex);

    }
    /// <summary>
    /// Autofits column.
    /// </summary>
    /// <param name="colIndex">Column index.</param>
    /// <param name="firstRow">One-based index of the first row to be used for autofit operation.</param>
    /// <param name="lastRow">One-based index of the last row to be used for autofit operation.</param>
    public void AutofitColumn( int colIndex, int firstRow, int lastRow )
    {
        RangeImpl rangeImpl = this[firstRow,colIndex ,lastRow,colIndex] as RangeImpl;
        rangeImpl.AutoFitToColumn( colIndex, colIndex);
    }

    /// <summary>
    /// Copies all data from another worksheet.
    /// </summary>
    /// <param name="worksheet">Parent worksheet.</param>
    /// <param name="hashStyleNames">Dictionary with style names.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="flags">Copy flags.</param>
    public void CopyFrom( WorksheetImpl worksheet, Dictionary<string, string> hashStyleNames,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicFontIndexes, ExcelWorksheetCopyFlags flags )
    {
      Dictionary<int, int> dictEmpty1 = new Dictionary<int, int>();
      Dictionary<int, int> dictEmpty2 = new Dictionary<int, int>();
      Dictionary<int, int> dictEmpty3 = new Dictionary<int, int>();
      CopyFrom( worksheet, hashStyleNames, hashWorksheetNames, dicFontIndexes, flags, dictEmpty1, dictEmpty2, dictEmpty3 );
    }
    /// <summary>
    /// Copies all data from another worksheet.
    /// </summary>
    /// <param name="worksheet">Parent worksheet.</param>
    /// <param name="hashStyleNames">Dictionary with style names.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="flags">Copy flags.</param>
    /// <param name="hashExtFormatIndexes">
    /// Dictionary with new extended format indexes.
    /// </param>
    /// <param name="hashNameIndexes">Dictionary with new name indexes.</param>
    public void CopyFrom( WorksheetImpl worksheet, Dictionary<string, string> hashStyleNames,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicFontIndexes, ExcelWorksheetCopyFlags flags,
      Dictionary<int, int> hashExtFormatIndexes, Dictionary<int, int> hashNameIndexes )
    {
      CopyFrom( worksheet, hashStyleNames, hashWorksheetNames, dicFontIndexes, flags
        , hashExtFormatIndexes, hashNameIndexes, new Dictionary<int, int>( 0 ) );
    }
    /// <summary>
    /// Copies all data from another worksheet.
    /// </summary>
    /// <param name="worksheet">Parent worksheet.</param>
    /// <param name="hashStyleNames">Dictionary with style names.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="flags">Copy flags.</param>
    /// <param name="hashExtFormatIndexes">
    /// Dictionary with new extended format indexes.
    /// </param>
    /// <param name="hashNameIndexes">Dictionary with new name indexes.</param>
    /// <param name="hashExternSheets">Represents collection with extern sheets indexes.</param>
    public void CopyFrom( WorksheetImpl worksheet, Dictionary<string, string> hashStyleNames,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicFontIndexes, ExcelWorksheetCopyFlags flags,
      Dictionary<int, int> hashExtFormatIndexes, Dictionary<int, int> hashNameIndexes, Dictionary<int, int> hashExternSheets )
    {
      ParseData();

      if (worksheet.ParseDataOnDemand || worksheet.ParseOnDemand)
      {
          if (worksheet.m_dataHolder != null && worksheet.ParseDataOnDemand)
              worksheet.m_dataHolder.ParseWorksheetData(worksheet, null, worksheet.ParseDataOnDemand);
          else if (worksheet.m_dataHolder == null && worksheet.ParseOnDemand && !worksheet.IsParsed && (worksheet.Parent is Syncfusion.XlsIO.Implementation.Collections.WorksheetsCollection))
              worksheet.ParseData(null);

          if (this.Parent is WorksheetsCollection)
          {
              foreach (WorksheetImpl sourceSheet in (this.Parent as WorksheetsCollection))
              {
                  if (sourceSheet.m_dataHolder != null && sourceSheet.ParseDataOnDemand)
                      sourceSheet.m_dataHolder.ParseWorksheetData(sourceSheet, null, sourceSheet.ParseDataOnDemand);
                  else if (sourceSheet.m_dataHolder == null && sourceSheet.ParseOnDemand && !sourceSheet.IsParsed && (sourceSheet.Parent is Syncfusion.XlsIO.Implementation.Collections.WorksheetsCollection))
                      sourceSheet.ParseData(null);
              }
          }
      }

      if( ( flags & ExcelWorksheetCopyFlags.ClearBefore ) != 0 )
      {
        ClearAll();
        flags &= ~ExcelWorksheetCopyFlags.ClearBefore;
      }

      if( ( flags & ExcelWorksheetCopyFlags.CopyColumnHeight ) != 0 )
      {
        CopyColumnWidth( worksheet, hashExtFormatIndexes );
        flags &= ~ExcelWorksheetCopyFlags.CopyColumnHeight;
      }

      if( ( flags & ExcelWorksheetCopyFlags.CopyRowHeight ) != 0 )
      {
        CopyRowHeight( worksheet, hashExtFormatIndexes );
        flags &= ~ExcelWorksheetCopyFlags.CopyRowHeight;
      }

      this.CustomHeight = worksheet.CustomHeight;

      if( ( flags & ExcelWorksheetCopyFlags.CopyNames ) != 0 )
      {
        CopyNames( worksheet, hashWorksheetNames, hashNameIndexes, hashExternSheets );
        flags &= ~ExcelWorksheetCopyFlags.CopyNames;
      }

      // We had to copy row height, column width in order to keep shape size correct.
      // Otherwise shapes could be stretched.
      base.CopyFrom( worksheet, hashStyleNames, hashWorksheetNames,
        dicFontIndexes, flags, hashExtFormatIndexes );

      if( ( flags & ExcelWorksheetCopyFlags.CopyCells ) != 0 )
      {
        //m_arrDefaultRowStyle.CopyFrom( worksheet.m_arrDefaultRowStyle );
        //m_arrDefaultColumnStyle.CopyFrom( worksheet.m_arrDefaultColumnStyle );

        m_iFirstRow = worksheet.m_iFirstRow;
        m_iLastRow = worksheet.m_iLastRow;
        m_iFirstColumn = worksheet.m_iFirstColumn;
        Zoom = worksheet.Zoom;
        m_iLastColumn = worksheet.m_iLastColumn;
        m_dicRecordsCells.CopyCells( worksheet.m_dicRecordsCells, hashStyleNames,
          hashWorksheetNames, hashExtFormatIndexes, hashNameIndexes, dicFontIndexes, hashExternSheets );

        CopyErrorIndicators( worksheet.m_errorIndicators );
        CopyHyperlinks( worksheet.m_hyperlinks );
      }

      if( ( flags & ExcelWorksheetCopyFlags.CopyMerges ) != 0 )
        CopyMerges( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyConditionlFormats ) != 0 )
        CopyConditionalFormats( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyAutoFilters ) != 0 )
        CopyAutoFilters( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyDataValidations ) != 0 )
        CopyDataValidations( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyPageSetup ) != 0 )
        CopyPageSetup( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyTables ) != 0 )
        CopyTables( worksheet, hashWorksheetNames );

      if( ( flags & ExcelWorksheetCopyFlags.CopyPivotTables ) != 0 )
        CopyPivotTables( worksheet, hashWorksheetNames );
    }
    /// <summary>
    /// Copies all pivot table objects.
    /// </summary>
    /// <param name="worksheet">Worksheet to copy pivot table objects from.</param>
    private void CopyPivotTables( WorksheetImpl worksheet, Dictionary<string, string> hashWorksheetNames )
    {
      if( worksheet == null )
        throw new ArgumentNullException( "worksheet" );

      if( worksheet.m_pivotTables == null || worksheet.m_pivotTables.Count == 0 )
        return;

      m_pivotTables = worksheet.m_pivotTables.Clone( this, hashWorksheetNames );
    }
    /// <summary>
    /// Copies all table objects.
    /// </summary>
    /// <param name="worksheet">Worksheet to copy table objects from.</param>
    private void CopyTables( WorksheetImpl worksheet, Dictionary<string, string> hashWorksheetNames )
    {
      if( worksheet == null )
        throw new ArgumentNullException( "worksheet" );

      if( worksheet.m_listObjects == null || worksheet.m_listObjects.Count == 0 )
        return;

      m_listObjects = worksheet.m_listObjects.Clone( this, hashWorksheetNames );
    }
    /// <summary>
    /// Copies error indicators.
    /// </summary>
    /// <param name="sourceErrors">Represents source error indicators.</param>
    private void CopyErrorIndicators( ErrorIndicatorsCollection sourceErrors )
    {
      if( sourceErrors == null )
        throw new ArgumentNullException( "sourceErrors" );

      ParseData();

      if( sourceErrors.Count > 0 )
      {
        m_errorIndicators = ( ErrorIndicatorsCollection )sourceErrors.Clone( this );
      }
    }
    /// <summary>
    /// Copies hyperlinks.
    /// </summary>
    /// <param name="source">Source hyperlink collection.</param>
    private void CopyHyperlinks( HyperLinksCollection source )
    {
      ParseData();

      m_hyperlinks = ( HyperLinksCollection )CloneUtils.CloneCloneable( ( ICloneParent )source, this );
    }
    /// <summary>
    /// Indicates whether source range can be moved into new location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <returns> True if source range can be moved.</returns>
    public bool CanMove( ref IRange destination, IRange source )
    {
      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      ParseData();

      RangeImpl destRange = ( RangeImpl )destination;
      RangeImpl sourceRange = ( RangeImpl )source;

      //if( destRange.IsSingleCell && !sourceRange.IsSingleCell )
      {
        int iNewRow = destRange.FirstRow + sourceRange.LastRow - sourceRange.FirstRow;
        int iNewColumn = destRange.FirstColumn + sourceRange.LastColumn - sourceRange.FirstColumn;
        destination = destRange = ( RangeImpl )destRange.InnerWorksheet.Range[ destRange.Row,
          destRange.Column, iNewRow, iNewColumn ];
      }

      if( destRange == sourceRange ) return true;

      Dictionary<ArrayRecord, object> hashToSkip = new Dictionary<ArrayRecord, object>();
      bool bResult = sourceRange.GetAreArrayFormulasNotSeparated( hashToSkip );

      if( bResult )
      {
        if( destRange.Worksheet != sourceRange.Worksheet )
          hashToSkip.Clear();

        bResult = destRange.GetAreArrayFormulasNotSeparated( hashToSkip );
      }

      return bResult;
    }

    /// <summary>
    /// Checks whether it is possible insert row into iRowIndex.
    /// </summary>
    /// <param name="iRowIndex">Index of row to insert.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="options">Insert options.</param>
    /// <returns>True if it is possible to insert row.</returns>
    public bool CanInsertRow( int iRowIndex, int iRowCount, ExcelInsertOptions options )
    {
      ParseData();

      if( iRowIndex < 1 || iRowIndex > m_book.MaxRowCount )
        return false;
      //throw new ArgumentOutOfRangeException( "iRowIndex" );

      if( iRowCount <= 0 )
        return false;
      //throw new ArgumentOutOfRangeException( "iRowCount" );

      if( m_iLastRow <= iRowIndex ) return true;

      // Check if we won't separate an array formula.
      if( iRowIndex >= m_iFirstRow && m_iLastColumn <= m_book.MaxColumnCount )
      {
        RangeImpl range = ( RangeImpl )Range[ iRowIndex, m_iFirstColumn, m_iLastRow, m_iLastColumn ];

        if( !range.AreFormulaArraysNotSeparated ) return false;

      }

      // Check if some cells won't be moved out of worksheet.
      int iDelta = m_iLastRow + iRowCount - m_book.MaxRowCount;

      if( iDelta > 0 )
      {
        int iStartRow = Math.Max( m_iLastRow - iDelta, m_iFirstRow );

        for( int i = m_iLastRow; i >= iStartRow; i-- )
        {
          if( !IsRowEmpty( i ) )
          {
            return false;
          }
          else
          {
            m_iLastRow--;
          }
        }

        // Check if there is no rows at all.
        if( m_iFirstRow > m_iLastRow )
        {
          m_iLastRow = m_iFirstRow = -1;
          m_iLastColumn = m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
        }
      }

      return true;
    }
    /// <summary>
    /// Checks whether it is possible to insert column into iRowIndex.
    /// </summary>
    /// <param name="iColumnIndex">Index of column to insert.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    /// <param name="options">Insert options.</param>
    /// <returns>True if it is possible to insert column.</returns>
    public bool CanInsertColumn( int iColumnIndex, int iColumnCount, ExcelInsertOptions options )
    {
      ParseData();

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        return false;

      if( m_iLastColumn < iColumnIndex || m_iFirstColumn == DEF_MIN_COLUMN_INDEX )
        return true;

      // Check if we won't separate some array formula.
      if( iColumnIndex >= m_iFirstColumn )
      {
        RangeImpl range = ( RangeImpl )Range[ m_iFirstRow, iColumnIndex, m_iLastRow, m_iLastColumn ];

        if( !range.AreFormulaArraysNotSeparated ) return false;
      }

      // check if some cells won't be moved out of worksheet.
      int iDelta = m_iLastColumn + iColumnCount - m_book.MaxColumnCount;

      if( iDelta > 0 )
      {
        int iStartColumn = Math.Max( m_iLastColumn - iDelta, m_iFirstColumn );

        for( int i = m_iLastColumn; i >= iStartColumn; i-- )
        {
          if( !IsColumnEmpty( i ) )
          {
            return false;
          }
          else
          {
            m_iLastColumn--;
          }
        }

        // Check if there is no rows at all.
        if( m_iFirstColumn > m_iLastColumn )
        {
          m_iLastRow = m_iFirstRow = -1;
          m_iLastColumn = m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
        }
      }
      //      if( m_usLastColumn == m_book.MaxColumnCount )
      //      {
      //        for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      //        {
      //          int index = RangeImpl.GetCellIndex( m_usLastColumn, i );
      //          if( m_dicRangeCells.Contains( index ) )
      //          {
      //#if USE_TYPED_HASH
      //            RangeImpl range = m_dicRangeCells[ index ];
      //#else
      //            RangeImpl range = ( RangeImpl )m_dicRangeCells[ index ];
      //#endif
      //
      //            if( range.IsInitialized ) return false;
      //          }
      //        }
      //      }

      return true;
    }
    /// <summary>
    /// Gets range from string value.
    /// </summary>
    /// <param name="strRangeValue">Range value represented in string.</param>
    /// <returns>Extracted Range.</returns>
    public IRange GetRangeByString(string strRangeValue, bool hasFormula)
    {
      if( strRangeValue == null || strRangeValue.Length == 0 )
        return null;

      FormulaUtil formulaUtil = ( m_book.Loading ) ?
        new FormulaUtil( Application, m_book, NumberFormatInfo.InvariantInfo,
          Application.ArgumentsSeparator, Application.RowSeparator) :
        m_book.FormulaUtil;

      Ptg[] arrPtgs = formulaUtil.ParseString( strRangeValue );
      Stack<object> stack = new Stack<object>();
      List<IRange> listFree = new List<IRange>();

      for( int i = 0, len = arrPtgs.Length; i < len; i++ )
      {
        if( arrPtgs[ i ] is IRangeGetter )
        {
          List<IRange> list = ( listFree != null ) ? listFree : new List<IRange>();

          IRange range = ( ( IRangeGetter )arrPtgs[ i ] ).GetRange( Workbook, this );
          list.Add( range );

          stack.Push( list );
          listFree = null;
        }
        else if( arrPtgs[ i ].TokenCode == FormulaToken.tCellRangeList )
        {
          listFree = ( List<IRange> )stack.Pop();
          List<IRange> list = ( List<IRange> )stack.Peek();
          list.AddRange( listFree );
          listFree.Clear();
        }
      }

      if (hasFormula && stack.Count != 1)
      {
          //Only the Supported formulas of calcengine has been handled as of now. 
          try
          {
              this.EnableSheetCalculations();
              string computedValue= CalcEngine.ParseAndComputeFormula(strRangeValue);
              IRange computedRange= this[computedValue.Substring(computedValue.LastIndexOf('!')+1)];
              this.DisableSheetCalculations();
              return computedRange;
          }                   
          catch (Exception e)
          {
              return null;
          }
      }       
        

      listFree = ( List<IRange> )stack.Pop();
      int iCount = listFree.Count;

      if( iCount == 1 )
        return listFree[ 0 ];

      IRanges result = listFree[ 0 ].Worksheet.CreateRangesCollection();

      for( int i = 0; i < iCount; i++ )
      {
        result.Add( listFree[ i ] );
      }
      
      return result;
    }

    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      ParseData();

      m_dicRecordsCells.UpdateNameIndexes( m_book, arrNewIndex );

      if( m_dataValidation != null )
        m_dataValidation.UpdateNamedRangeIndexes( arrNewIndex );

      ShapesCollection shapes = InnerShapes;

      if( shapes != null )
        shapes.UpdateNamedRangeIndexes( arrNewIndex );
    }

    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="dicNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      ParseData();

      if(m_dicRecordsCells !=null)
        m_dicRecordsCells.UpdateNameIndexes( m_book, dicNewIndex );

      if( m_dataValidation != null )
        m_dataValidation.UpdateNamedRangeIndexes( dicNewIndex );

      ShapesCollection shapes = InnerShapes;

      if( shapes != null )
        shapes.UpdateNamedRangeIndexes( dicNewIndex );
    }

    /// <summary>
    /// Returns string index of the specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index to locate.</param>
    /// <returns>String index of the specified cell.</returns>
    public int GetStringIndex( long cellIndex )
    {
      ParseData();

      LabelSSTRecord label = m_dicRecordsCells.GetCellRecord( cellIndex ) as LabelSSTRecord;

      return ( label != null )
        ? label.SSTIndex
        : -1;
    }
    /// <summary>
    /// Returns TextWithFormat object corresponding to the specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>TextWithFormat object corresponding to the specified cell.</returns>
    public TextWithFormat GetTextWithFormat( long cellIndex )
    {
      ParseData();
      ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord(cellIndex);
      if (cell is LabelRecord)
      {
          LabelRecord labelRecord = cell as LabelRecord;
          SetString(labelRecord.Row + 1, labelRecord.Column + 1, labelRecord.Label);
      }
      LabelSSTRecord label = m_dicRecordsCells.GetCellRecord( cellIndex ) as LabelSSTRecord;

      if( label == null ) return null;

      int iSSTIndex = label.SSTIndex;
      return m_book.InnerSST[ iSSTIndex ];
    }
    /// <summary>
    /// Returns TextWithFormat object corresponding to the specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Object corresponding to the specified cell.</returns>
    public object GetTextObject( long cellIndex )
    {
      ParseData();

      LabelSSTRecord label = m_dicRecordsCells.GetCellRecord( cellIndex ) as LabelSSTRecord;

      if( label == null )
        return null;

      int iSSTIndex = label.SSTIndex;
      return m_book.InnerSST[ iSSTIndex ];
    }
    /// <summary>
    /// Returns extended format for specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Extended format for specified cell.</returns>
    public ExtendedFormatImpl GetExtendedFormat( long cellIndex )
    {
      ParseData();

      ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( cellIndex );

      if( cell == null ) return null;

      int iXFIndex = cell.ExtendedFormatIndex;
      return m_book.InnerExtFormats[ iXFIndex ];
    }
    /// <summary>
    /// Sets string index in the specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index to set sst index in.</param>
    /// <param name="iSSTIndex">SST index to set.</param>
    public void SetLabelSSTIndex( long cellIndex, int iSSTIndex )
    {
      ParseData();

      //BiffRecordRaw cell = m_dicRecordsCells[ iCellIndex ];
      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex );

      ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( iRow, iColumn );

      if( iSSTIndex == SSTDictionary.DEF_EMPTY_STRING_INDEX )
      {
        if( cell == null || cell.TypeCode != TBIFFRecord.Blank )
        {
          m_dicRecordsCells.SetCellRecord( iRow, iColumn, ( ICellPositionFormat )
            GetRecord( TBIFFRecord.Blank, iRow, iColumn ) );
        }

        return;
      }

      if( iSSTIndex < 0 || iSSTIndex >= m_book.InnerSST.Count )
        throw new ArgumentOutOfRangeException( "iSSTIndex" );

      if( cell == null || cell.TypeCode != TBIFFRecord.LabelSST )
      {
        cell = ( ICellPositionFormat )GetRecord( TBIFFRecord.LabelSST, iRow, iColumn );
      }

      ( ( LabelSSTRecord )cell ).SSTIndex = iSSTIndex;
      
      if (iRow != 0 || iColumn != 0)
          m_dicRecordsCells.SetCellRecord( iRow, iColumn, cell );
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      if( arrNewIndexes == null )
        throw new ArgumentNullException( "arrNewIndexes" );

      ParseData();

      m_dicRecordsCells.UpdateStringIndexes( arrNewIndexes );
    }
    /// <summary>
    /// Removes merged cells.
    /// </summary>
    /// <param name="range">Represent range of merged cells.</param>
    public void RemoveMergedCells( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      ParseData();

      int iStartRow = range.Row;
      int iStartCol = range.Column;
      int iEndRow = range.LastRow;
      int iEndCol = range.LastColumn;

      for( int iRow = iStartRow; iRow <= iEndRow; iRow++ )
      {
        for( int iCol = iStartCol; iCol <= iEndCol; iCol++ )
        {
          if( iRow != iStartRow || iCol != iStartCol )
          {
            long lCellIndex = RangeImpl.GetCellIndex( iCol, iRow );
            m_dicRecordsCells.Remove( lCellIndex );
          }
        }
      }
    }
    /// <summary>
    /// Sets active cell
    /// </summary>
    /// <param name="range">Cell to activate.</param>
    public void SetActiveCell( IRange range )
    {
      SetActiveCell( range, true );
    }
    /// <summary>
    /// Sets active cell
    /// </summary>
    /// <param name="range">Cell to activate.</param>
    public void SetActiveCell( IRange range, bool updateApplication )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      ParseData();

      if( updateApplication )
        AppImplementation.SetActiveCell( range );

      CreateAllSelections();

      
      SelectionRecord selection = GetActiveSelection();

      int iColumn = range.Column - 1;
      int iRow = range.Row - 1;

      selection.ColumnActiveCell = ( ushort )iColumn;
      selection.RowActiveCell = ( ushort )iRow;
      SelectionRecord.TAddr addr = new SelectionRecord.TAddr( ( ushort )iRow,
        ( ushort )iRow, ( byte )iColumn, ( byte )iColumn );

      selection.SetSelection( 0, addr );

      //if( m_pane != null )
      //  m_pane.ActivePane = selection.Pane;
    }

    private void ActivatePane( IRange range )
    {
      if( WindowTwo.IsFreezePanes )
      {
        IRange splitCell = SplitCell;
        int iActivePane;

        //  1 3
        //  0 2

        if( range.Row < splitCell.Row )
        {
          if( range.Column >= splitCell.Column )
          {
            iActivePane = 2;
          }
          else
          {
            iActivePane = 0;
          }
        }
        else
        {
          if( range.Column >= splitCell.Column )
          {
            iActivePane = 3;
          }
          else
          {
            iActivePane = 1;
          }
        }
 
      m_pane.ActivePane = ( ushort )iActivePane;
     }
    }
    /// <summary>
    /// Gets selection with active cell.
    /// </summary>
    /// <returns></returns>
    private SelectionRecord GetActiveSelection()
    {
      SelectionRecord selection = null;
      int iPaneIndex = ( m_pane != null ) ? ( int )m_pane.ActivePane : 0;

      for( int i = 0, len = m_arrSelections.Count; i < len; i++ )
      {
        SelectionRecord currentSelection = m_arrSelections[ i ];

        if( currentSelection.Pane == iPaneIndex )
        {
          selection = currentSelection;
          break;
        }
      }

      if( selection == null && m_arrSelections.Count == 1 )
        selection = m_arrSelections[ 0 ];

      return selection;
    }
    /// <summary>
    /// Returns active cell.
    /// </summary>
    /// <returns>Currently active cell.</returns>
    public IRange GetActiveCell()
    {
      ParseData();
      SelectionRecord selection = GetActiveSelection();
      int iRow = 0;
      int iColumn = 0;

      if( selection != null )
      {
        iRow = selection.RowActiveCell;
        iColumn = selection.ColumnActiveCell;
      }

      return this[ iRow + 1, iColumn + 1 ];
    }
    /// <summary>
    /// Tells whether specific FormulaRecord is array-entered formula.
    /// </summary>
    /// <param name="formula">FormulaRecord to check.</param>
    /// <returns>True if it is array-entered formula.</returns>
    [CLSCompliant( false )]
    public bool IsArrayFormula( FormulaRecord formula )
    {
      if( formula == null || formula.ParsedExpression == null
        || formula.ParsedExpression.Length == 0 ) return false;

      Ptg ptg = formula.ParsedExpression[ 0 ];

      if( ptg.TokenCode == FormulaToken.tExp )
      {
        return CellRecords.GetArrayRecord( formula.Row + 1, formula.Column + 1 ) != null;
      }

      return false;
    }
    /// <summary>
    /// Indicates whether cell contains array-entered formula.
    /// </summary>
    /// <param name="cellIndex">Cell index to check.</param>
    /// <returns>True if cell contains array-entered formula.</returns>
    public bool IsArrayFormula( long cellIndex )
    {
      ParseData();

      FormulaRecord formula = m_dicRecordsCells.GetCellRecord( cellIndex ) as FormulaRecord;

      if( formula != null )
      {
        return IsArrayFormula( formula );
      }

      return false;
    }
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRow">One-based index of the row</param>
    /// <param name="bRaiseEvents">Indicates whether to raise events on row autofitting.</param>
    /// <returns>
    /// Height from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public double InnerGetRowHeight( int iRow, bool bRaiseEvents )
    {
      if (iRow < 1 || iRow > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("Value cannot be less 1 and greater than max row index.");

        RowStorage row = WorksheetHelper.GetOrCreateRow(this, iRow - 1, false);
        bool hasMaxHeight = false;     
   
        if (row != null)
        {            
            if (row.IsHidden)
                return 0;
            else if (row.IsBadFontHeight || CustomHeight || (row.IsWrapText && !this.Rows[iRow - FirstRow].IsMerged))
                return row.Height / 20.0;
            else if(FirstColumn <= m_book.MaxColumnCount && LastColumn <= m_book.MaxColumnCount)
            {                
                double standardFontSize = AppImplementation.StandardFontSize;
                for (int iColumn = FirstColumn; iColumn <= LastColumn; iColumn++)
                {
                    double fontSize = Range[iRow, iColumn].CellStyle.Font.Size;
                    if (fontSize > standardFontSize)
                    {
                        hasMaxHeight = true;
                        break;
                    }
                }
            }
        }

        if (hasMaxHeight)
            return row.Height / 20.0;
        else
            return StandardHeight;
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <param name="cloneShapes">Indicates whether we should clone shapes or not.</param>
    /// <returns>Copy of the current object.</returns>
    public override object Clone( object parent, bool cloneShapes )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ParseData();

      WorksheetImpl result = ( WorksheetImpl )base.Clone( parent, cloneShapes );
      result.m_rngUsed = null;
      result.m_migrantRange = null;

      result.m_pane = ( PaneRecord )CloneUtils.CloneCloneable( m_pane );
      result.m_arrSortRecords = CloneUtils.CloneCloneable( m_arrSortRecords );
      result.m_arrDConRecords = CloneUtils.CloneCloneable( m_arrDConRecords );
      result.m_arrAutoFilter = CloneUtils.CloneCloneable( m_arrAutoFilter );
      result.m_arrSelections = CloneUtils.CloneCloneable( m_arrSelections );

      if( m_arrCustomProperties != null )
        result.m_arrCustomProperties = ( WorksheetCustomProperties )m_arrCustomProperties.CloneAll();

      if( m_arrNotes != null )
      {
        result.m_arrNotes = CloneUtils.CloneCloneable( m_arrNotes );
        result.m_arrNotesByCellIndex = CloneUtils.CloneCloneable( m_arrNotesByCellIndex );
      }

      result.m_arrColumnInfo = CloneUtils.CloneArray( m_arrColumnInfo );

      result.m_dataValidation = ( DataValidationTable )CloneUtils.CloneCloneable( ( ICloneParent )m_dataValidation, result );
      // NOTE: names collection shouldn't be copied since it should be done in workbook,
      // because otherwise workbook and worksheet would contain different named ranges.
      result.m_names = new WorksheetNamesCollection( Application, this );//( WorksheetNamesCollection )m_names.Clone( parent );
      result.m_pageSetup = ( PageSetupImpl )m_pageSetup.Clone( result );
      result.m_mergedCells = ( MergeCellsImpl )CloneUtils.CloneCloneable( m_mergedCells, result );
      result.m_autofilters = m_autofilters.Clone( result );
      result.m_pivotTables = ( PivotTableCollection )CloneUtils.CloneCloneable( ( ICloneParent )m_pivotTables, result );
      result.m_hyperlinks = ( HyperLinksCollection )CloneUtils.CloneCloneable( ( ICloneParent )m_hyperlinks, result );

      result.m_arrConditionalFormats = ( WorksheetConditionalFormats )
        CloneUtils.CloneCloneable( ( ICloneParent )m_arrConditionalFormats, result );

      result.m_dicRecordsCells = m_dicRecordsCells.Clone( result );

      // We have to add worksheet into worksheets collection.
      result.m_book.InnerWorksheets.InnerAdd( result );
      return result;
    }
    /// <summary>
    /// Looks through all records and calls AddIncrease for each LabelSST record.
    /// </summary>
    public void ReAddAllStrings()
    {
      ParseData();

      m_dicRecordsCells.ReAddAllStrings();
    }
    /// <summary>
    /// Gets string preservation option for the range.
    /// </summary>
    /// <param name="range">Range to get value for.</param>
    /// <returns>True if strings are preserved for all cells of the range,
    /// false - if it is not preserved,
    /// null - undefined or different cells have different values.</returns>
    public bool? GetStringPreservedValue( ICombinedRange range )
    {
      return m_stringPreservedRanges.GetRangeValue( range );
    }
    /// <summary>
    /// Sets value indicating whether string should be preserved for range.
    /// </summary>
    /// <param name="range">Range to set value for.</param>
    /// <param name="value">Value to set.</param>
    public void SetStringPreservedValue( ICombinedRange range, bool? value )
    {
      m_stringPreservedRanges.SetRange( range, value );
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public override void MarkUsedReferences( bool[] usedItems )
    {
      m_dicRecordsCells.MarkUsedReferences( usedItems );

      if( m_dataValidation != null )
        m_dataValidation.MarkUsedReferences( usedItems );

      if( m_arrConditionalFormats != null )
        m_arrConditionalFormats.MarkUsedReferences( usedItems );

      IChartShapes charts = Charts;

      for( int i = 0, len = charts.Count; i < len; i++ )
      {
        ( charts[ i ] as ChartImpl ).MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public override void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      m_dicRecordsCells.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_dataValidation != null )
        m_dataValidation.UpdateReferenceIndexes( arrUpdatedIndexes );

      if( m_arrConditionalFormats != null )
        m_arrConditionalFormats.UpdateReferenceIndexes( arrUpdatedIndexes );

      IChartShapes charts = Charts;

      for( int i = 0, len = charts.Count; i < len; i++ )
      {
        ( charts[ i ] as ChartImpl ).UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    /// <summary>
    /// Creates default pane.
    /// </summary>
    protected void CreateEmptyPane()
    {
      m_pane = ( PaneRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Pane );
    }
    /// <summary>
    /// Copies cell value from sourceCell into destCell.
    /// DOES NOT COPY FORMULAARRAYS (THIS SHOULD BE DONE IN DIFFERENT PLACE).
    /// </summary>
    /// <param name="destCell">Destination cell.</param>
    /// <param name="sourceCell">Source cell.</param>
    protected void CopyCell( IRange destCell, IRange sourceCell )
    {
      CopyCell( destCell, sourceCell, ExcelCopyRangeOptions.None );
    }
    /// <summary>
    /// Copies cell value from sourceCell into destCell.
    /// DOES NOT COPY FORMULAARRAYS (THIS SHOULD BE DONE IN DIFFERENT PLACE).
    /// </summary>
    /// <param name="destCell">Destination cell.</param>
    /// <param name="sourceCell">Source cell.</param>
    /// <param name="options">Options for coping.</param>
    protected void CopyCell( IRange destCell, IRange sourceCell, ExcelCopyRangeOptions options )
    {
      if( destCell == null )
        throw new ArgumentNullException( "destCell" );

      if( sourceCell == null )
        throw new ArgumentNullException( "sourceCell" );

      RangeImpl dest = ( RangeImpl )destCell;
      RangeImpl source = ( RangeImpl )sourceCell;

      if( !( dest.IsSingleCell && source.IsSingleCell ) )
        throw new ArgumentException( "Each range argument should contain a single cell" );

      dest.ExtendedFormatIndex = source.ExtendedFormatIndex;

      if( source.Record != null && source.Record is FormulaRecord )
      {
        if( sourceCell.HasFormulaArray ) return;

        FormulaRecord formulaSource = ( FormulaRecord )source.Record;
        FormulaRecord formulaDest = ( FormulaRecord )formulaSource.Clone();
        //( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );
        formulaDest.Row = ( ushort )( destCell.Row - 1 );
        formulaDest.Column = ( ushort )( destCell.Column - 1 );

        bool bUpdateFormula = ( options & ExcelCopyRangeOptions.UpdateFormulas ) != 0;
        int iRowOffset = bUpdateFormula ? destCell.Row - sourceCell.Row : 0;
        int iColOffset = bUpdateFormula ? destCell.Column - sourceCell.Column : 0;

        formulaDest.ParsedExpression = UpdateFormula( formulaSource.ParsedExpression,
          iRowOffset, iColOffset );

        dest.SetFormula( formulaDest );
      }
      else
      {
        destCell.Value = sourceCell.Value;
      }

      CopyComment( source, dest );      
    }

    /// <summary>
    /// Updates the hyperlinks.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dest">The dest.</param>
    private void UpdateHyperlinks(RangeImpl source, RangeImpl dest)
    {
        if (source == null)
            throw new ArgumentNullException("source");

        if (dest == null)
            throw new ArgumentNullException("dest");

        int rowCount = GetRowCount(source, dest) ;
        int colCount=GetColumnCount(source,dest);
       
        if (source.Hyperlinks != null)
        {
            IHyperLinks sheetHyperlinks = source .Hyperlinks ;
            HyperLinksCollection hyperLinksCollection = (HyperLinksCollection)this.HyperLinks;
            for (int index = 0, len = sheetHyperlinks.Count; index < len; index++)
            {
                IRange range = sheetHyperlinks[index].Range;
                long removingIndex = RangeImpl.GetCellIndex(range .Column,range.Row );
                hyperLinksCollection.m_dicCellToList.Remove(removingIndex );
            }

            for (int index = 0, len =sheetHyperlinks .Count; index < len; index++)
            {
                IRange range = sheetHyperlinks[index].Range;
                ((HyperLinkImpl)sheetHyperlinks[index]).Range = this[range.Row + rowCount, range.Column + colCount];
                HyperLinkImpl link = ((HyperLinkImpl)sheetHyperlinks[index]);
                hyperLinksCollection.AddToHash(link );
            }
        }
    }
    /// <summary>
    /// Gets the column count.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dest">The dest.</param>
    /// <returns></returns>
    private int GetColumnCount(RangeImpl source, RangeImpl dest)
    {
        int colCount = 0;
        if (source.Row != dest.Row)
        {
            colCount = dest.Column - source.Column;
        }
        return colCount;
    }

    /// <summary>
    /// Gets the row count.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="dest">The dest.</param>
    /// <returns></returns>
    private int GetRowCount(RangeImpl source, RangeImpl dest)
    {        
        int rowCount = 0;
        if (source.Row != dest.Row)
        {
            rowCount = dest.Row - source.Row;
        }
        return rowCount;
    }
    /// <summary>
    /// Copies comment from one cell into another.
    /// </summary>
    /// <param name="source">Source cell.</param>
    /// <param name="dest">Destination cell.</param>
    private void CopyComment( RangeImpl source, RangeImpl dest )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( dest == null )
        throw new ArgumentNullException( "dest" );

      if( !source.IsSingleCell || !dest.IsSingleCell )
        throw new ArgumentException( "Ranges should be single cells." );

      if( source.Comment != null )
      {
        dest.AddComment( source.Comment );
      }
    }
    /// <summary>
    /// Removes last row from the worksheet.
    /// </summary>
    /// <param name="bUpdateFormula">Indicates whether to update formulas after row remove.</param>
    private void RemoveLastRow( bool bUpdateFormula )
    {
      RemoveLastRow( bUpdateFormula, 1 );
    }
    /// <summary>
    /// Removes required number of last rows from the worksheet.
    /// </summary>
    /// <param name="bUpdateFormula">Indicates whether to update formulas after row remove.</param>
    /// <param name="count">Number of rows to remove.</param>
    private void RemoveLastRow( bool bUpdateFormula, int count )
    {
      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      if( count == 0 )
        return;

      ParseData();

      int rowIndex = m_dicRecordsCells.Table.LastRow + 1;

      for( int i = 0, curRow = rowIndex; i < count; i++, curRow-- )
      {
        if( curRow < 0 )
        {
          count = i;
          break;
        }

        m_dicRecordsCells.RemoveRow( curRow );
      }

      m_iLastRow = m_dicRecordsCells.Table.LastRow + 1;

      // Update operation is performed inside MoveRange method call?
      //if( bUpdateFormula )
      //{
      //  Rectangle rectSource = Rectangle.FromLTRB( 0, rowIndex, m_book.MaxColumnCount - 1,
      //    m_book.MaxRowCount - 1 );

      //  Rectangle rectDest = Rectangle.FromLTRB( 0, rowIndex - count, m_book.MaxColumnCount - 1,
      //    m_book.MaxRowCount - 1 );

      //  int iCurIndex = m_book.AddSheetReference( this );
      //  m_book.UpdateFormula( iCurIndex, rectSource, iCurIndex, rectDest );
      //}
    }
    /// <summary>
    /// Removes last column from the worksheet.
    /// </summary>
    /// <param name="bUpdateFormula">Indicates whether update formulas after removing.</param>
    private void RemoveLastColumn( bool bUpdateFormula )
    {
      ParseData();

      int colIndex = m_iLastColumn;

      m_iLastColumn = colIndex - 1;
      m_dicRecordsCells.RemoveLastColumn( colIndex );

      if( m_iFirstColumn > m_iLastColumn )
      {
        m_iLastColumn = m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
      }

      if( bUpdateFormula )
      {
        Rectangle rectSource = Rectangle.FromLTRB( colIndex, 0, m_book.MaxColumnCount - 1,
          m_book.MaxRowCount - 1 );

        Rectangle rectDest = Rectangle.FromLTRB( colIndex - 1, 0, m_book.MaxColumnCount - 1,
          m_book.MaxRowCount - 1 );

        int iCurIndex = m_book.AddSheetReference( this );
        m_book.UpdateFormula( iCurIndex, rectSource, iCurIndex, rectDest );
      }
    }
    /// <summary>
    /// Removes last column from the worksheet.
    /// </summary>
    /// <param name="bUpdateFormula">Indicates whether update formulas after removing.</param>
    /// <param name="count">Number of columns to remove.</param>
    private void RemoveLastColumn( bool bUpdateFormula, int count )
    {
      ParseData();

      int colIndex = m_iLastColumn;
      m_iLastColumn = colIndex - 1;

      for( int i = 0; i < count && m_iLastColumn >= 0; i++, m_iLastColumn-- )
      {
        m_dicRecordsCells.RemoveLastColumn( m_iLastColumn + 1 );
      }

      m_iLastColumn = m_dicRecordsCells.LastColumn + 1;
      if( m_iFirstColumn > m_iLastColumn )
      {
        m_iLastColumn = m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
      }

      if( bUpdateFormula )
      {
        Rectangle rectSource = Rectangle.FromLTRB( colIndex + count - 1, 0, m_book.MaxColumnCount - 1,
          m_book.MaxRowCount - 1 );

        Rectangle rectDest = Rectangle.FromLTRB( colIndex - 1, 0, m_book.MaxColumnCount - 1,
          m_book.MaxRowCount - 1 );

        int iCurIndex = m_book.AddSheetReference( this );
        m_book.UpdateFormula( iCurIndex, rectSource, iCurIndex, rectDest );
      }
    }
    /// <summary>
    /// Partially clears cells if necessary.
    /// </summary>
    /// <param name="rect">Range to clear.</param>
    private void PartialClearRange( Rectangle rect )
    {
      ParseData();

      if( m_dicRecordsCells.UseCache )
      {
        int iFirstRow = rect.Top + 1;
        int iFirstColumn = rect.Left + 1;

        int iLastRow = rect.Bottom + 1;
        int iLastColumn = rect.Right + 1;

        for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
        {
          //SetRowHeight( iRow, AppImplementation.StandardHeight );
          //InnerSetRowHeight( iRow, AppImplementation.StandardHeight, true, MeasureUnits.Point, false );

          for( int iColumn = iFirstColumn; iColumn <= iLastColumn; iColumn++ )
          {
            //long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
            RangeImpl range = m_dicRecordsCells.GetRange( iRow, iColumn );//lCellIndex );

            if( range != null )
            {
              range.PartialClear();
            }
          }
        }
      }
    }
    /// <summary>
    /// Caches cells from the source range and removes them from it.
    /// </summary>
    /// <param name="source">Source range.</param>
    /// <param name="destination">Destination range.</param>
    /// <param name="iMaxRow">Maximum row after moving into destination range.</param>
    /// <param name="iMaxColumn">Maximum column after moving into destination range.</param>
    /// <param name="tableSource">Source records collection.</param>
    /// <returns>Returns hashtable (index-RangeImpl) with all cached cells.</returns>
    private RecordTable CacheAndRemoveFromParent( IRange source, IRange destination,
      ref int iMaxRow, ref int iMaxColumn, CellRecordCollection tableSource )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( tableSource == null )
        throw new ArgumentNullException( "tableSource" );

      WorksheetImpl destSheet = ( WorksheetImpl )destination.Parent;
      WorksheetImpl sourceSheet = ( WorksheetImpl )source.Worksheet;

      int iDeltaRow = destination.Row - source.Row;
      int iDeltaCol = destination.Column - source.Column;
      int iLastColumn = source.LastColumn - 1;
      int iStartColumn = source.Column - 1;

      return tableSource.CacheAndRemove( ( RangeImpl )source, iDeltaRow, iDeltaCol, ref iMaxRow, ref iMaxColumn );
    }
    /// <summary>
    /// Copies source dictionary into destination.
    /// </summary>
    /// <param name="source">Source table.</param>
    /// <param name="destination">Destination table.</param>
    /// <param name="bUpdateRowRecords">
    /// Indicates whether row information such as row height, default style, etc.
    /// must be copied from cache.
    /// </param>
    private void CopyCacheInto( RecordTable source, RecordTable destination, bool bUpdateRowRecords )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source.FirstRow < 0 )
        return;

      for( int i = source.FirstRow, iLastRow = source.LastRow; i <= iLastRow; i++ )
      {
        RowStorage row = ( RowStorage )source.Rows[ i ];
        RowStorage destRow = destination.Rows[ i ];

        if( row == null )
        {
          if( destRow != null && destRow.UsedSize == 0 && bUpdateRowRecords )
          {
            destination.RemoveRow( i );
          }

          continue;
        }

        destRow = ( RowStorage )destination.Rows[ i ];

        WorksheetHelper.AccessRow( this, i + 1 );

        if( destRow == null )
        {
          destRow = new RowStorage( i, AppImplementation.StandardHeightInRowUnits,
            destination.Workbook.DefaultXFIndex );
          destRow.IsFormatted = false;
          destination.SetRow( i, destRow );
        }

        if( bUpdateRowRecords )
        {
          destRow.CopyRowRecordFrom( row );
        }

        if( row.UsedSize > 0 )
        {
          WorksheetHelper.AccessColumn( this, row.FirstColumn + 1 );
          WorksheetHelper.AccessColumn( this, row.LastColumn + 1 );
          destRow.InsertRowData( row, Application.RowStorageAllocationBlockSize,
            destination.Workbook.HeapHandle );
        }

        //destRow.RowInformation = ( RowRecord )row.RowInformation.Clone();
      }
    }
    /// <summary>
    /// Clears range in the dictionary that corresponds to the specified range.
    /// </summary>
    /// <param name="dictionary">Dictionary to clear.</param>
    /// <param name="rect">Rectangle to clear.</param>
    private static void ClearRange( IDictionary dictionary, Rectangle rect )
    {
      if( dictionary == null )
        throw new ArgumentNullException( "dictionary" );

      int iFirstRow = rect.Top + 1;
      int iFirstColumn = rect.Left + 1;

      int iLastRow = rect.Bottom + 1;
      int iLastColumn = rect.Right + 1;

      // NOTE: this code segment can be optimized.
      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        for( int iColumn = iFirstColumn; iColumn <= iLastColumn; iColumn++ )
        {
          long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
          dictionary.Remove( lCellIndex );
        }
      }
    }
    /// <summary>
    /// Updates formula array after copy range operation.
    /// </summary>
    /// <param name="array">ArrayRecord to update.</param>
    /// <param name="destSheet">Destination worksheet.</param>
    /// <param name="iDeltaRow">Row difference.</param>
    /// <param name="iDeltaColumn">Column difference.</param>
    private void UpdateArrayFormula( ArrayRecord array, IWorksheet destSheet, int iDeltaRow, int iDeltaColumn )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      // TODO: Maybe this update is not enough?
      WorkbookImpl book = ( WorkbookImpl )destSheet.Workbook;
      array.Formula = book.FormulaUtil.UpdateFormula( array.Formula, iDeltaRow, iDeltaColumn );
    }
    /// <summary>
    /// Returns record table that should contain cell information.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <param name="rectIntersection">Intersection rectangle.</param>
    /// <param name="intersection">RecordTable with intersection data.</param>
    /// <param name="rectSource">Another RecordTable that contains cell that are not included into intersection.</param>
    /// <returns>RecordTable that should contain cell information.</returns>
    private RecordTable GetRecordTable( int iRow, int iColumn, Rectangle rectIntersection,
      RecordTable intersection, RecordTable rectSource )
    {
      return UtilityMethods.Contains( rectIntersection, iColumn, iRow )
        ? intersection
        : rectSource;
    }
    /// <summary>
    /// Adds all necessary styles into collection.
    /// </summary>
    /// <param name="iRow">Start row of the range to copy styles from.</param>
    /// <param name="iColumn">Start column of the range to copy styles from.</param>
    /// <param name="iRowCount">Number of rows in the range.</param>
    /// <param name="iColCount">Number of columns in the range.</param>
    /// <param name="destSheet">Destination worksheet.</param>
    /// <param name="dicFontIndexes">Dictionary that will get updated indexes.</param>
    /// <returns>Dictionary with updated extended format indexes</returns>
    private Dictionary<int, int> GetUpdatedXFIndexes( int iRow, int iColumn,
      int iRowCount, int iColCount, WorksheetImpl destSheet, out Dictionary<int, int> dicFontIndexes )
    {
      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      dicFontIndexes = null;

      if( m_book == destSheet.Workbook )
        return null;

      ParseData();

      dicFontIndexes = new Dictionary<int, int>();
      Dictionary<int, object> hashXFIndexes = new Dictionary<int, object>();
      IList<ExtendedFormatImpl> arrXFormats = new List<ExtendedFormatImpl>();
      ExtendedFormatsCollection arrCurFormats = m_book.InnerExtFormats;

      for( int i = iRow, iRowLen = iRow + iRowCount; i < iRowLen; i++ )
      {
        for( int j = iColumn, iColLen = iColumn + iColCount; j < iColLen; j++ )
        {
          long lCellIndex = RangeImpl.GetCellIndex( j, i );
          ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( lCellIndex );

          if( cell != null )
          {
            int iXFIndex = cell.ExtendedFormatIndex;
            arrCurFormats.AddIndex( hashXFIndexes, arrXFormats, iXFIndex );
          }
        }
      }

      WorkbookImpl destBook = destSheet.ParentWorkbook;
      ExtendedFormatsCollection destXFormats = destBook.InnerExtFormats;
      return destXFormats.Merge( arrXFormats, out dicFontIndexes );
    }

    /// <summary>
    /// Clear specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell to clear.</param>
    private void ClearCell( long cellIndex )
    {
      ParseData();

      m_dicRecordsCells.Remove( cellIndex );
      //m_dicStringCells.Remove( iCellIndex );
      RangeImpl range = m_dicRecordsCells.GetRange( cellIndex );

      if( range != null ) range.Clear();
    }

    /// <summary>
    /// Sets range values accordingly to an ArrayRecord.
    /// </summary>
    /// <param name="array">ArrayRecord that has cells values.</param>
    private void SetArrayFormulaRanges( ArrayRecord array )
    {
      ParseData();

      // Create control token.
      Ptg controlToken = FormulaUtil.CreatePtg( FormulaToken.tExp, array.FirstRow,
        array.FirstColumn );
      int iLastRow = array.LastRow;
      int iLastCol = array.LastColumn;
      int iFirstRow = array.FirstRow;
      int iFirstCol = array.FirstColumn;

      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = iFirstCol; iCol <= iLastCol; iCol++ )
        {
          //RangeImpl range = ( RangeImpl )Range[ iRow + 1, iCol + 1 ];
          long lCellIndex = RangeImpl.GetCellIndex( iCol + 1, iRow + 1 );

          FormulaRecord formula = ( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );
          formula.Row = iRow;
          formula.Column = iCol;
          //formula.RecalculateAlways = true;
          //formula.CalculateOnOpen = true;
          formula.ParsedExpression = new Ptg[] { ( Ptg )controlToken.Clone() };
          m_dicRecordsCells.SetCellRecord( lCellIndex, formula );
          //range.SetFormula( formula );

          // Force range creation or update value (formula still requires range object).
          RangeImpl range = ( RangeImpl )Range[ iRow + 1, iCol + 1 ];
          range.UpdateRecord();
        }
      }

      UpdateFirstLast( iFirstRow + 1, iFirstCol + 1 );
      UpdateFirstLast( iLastRow + 1, iLastCol + 1 );
    }

    /// <summary>
    /// Removes all formulas in colRemove from internal ArrayFormula collection.
    /// </summary>
    /// <param name="record">Formula to remove.</param>
    /// <param name="bClearRange">Indicates whether to clear range before remove operation.</param>
    [CLSCompliant( false )]
    protected void RemoveArrayFormula( ArrayRecord record, bool bClearRange )
    {
      ParseData();

      if( record == null )
        throw new ArgumentNullException( "record" );

      int iStartRow = record.FirstRow + 1;
      int iStartCol = record.FirstColumn + 1;
      int iLastRow = record.LastRow + 1;
      int iLastCol = record.LastColumn + 1;
      //      int index = RangeImpl.GetCellIndex( iStartCol, iStartRow );
      //
      //      if( !m_dicArrayFormula.Contains( index ) )
      //        throw new ApplicationException( "Can't remove formula - wrong column and row index" );
      //
      //      ArrayRecord sourceRecord = ( ArrayRecord )m_dicArrayFormula[ index ];
      //
      //      if( sourceRecord.FirstRow != iStartRow - 1 || sourceRecord.FirstCol != iStartCol - 1
      //        || sourceRecord.LastCol != iLastCol - 1 || sourceRecord.LastRow != iLastRow - 1 )
      //        throw new ApplicationException( "Can't remove formula - wrong column and row index" );

      //      if( bClearRange )
      //        Range[ iStartRow, iStartCol, iLastRow, iLastCol ].Text = "";

      for( int iRow = iStartRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = iStartCol; iCol <= iLastCol; iCol++ )
        {
          //int iRemoveIndex = RangeImpl.GetCellIndex( iCol, iRow );
          m_dicRecordsCells.SetCellRecord( iRow, iCol, null );
          //m_dicArrayFormula.Remove( iRemoveIndex );
        }
      }

      //m_arrArrayFormula.Remove( sourceRecord );
    }
    /// <summary>
    /// Creates new ArrayFormula based on specified ArrayFormula.
    /// Used in copy and move range operations.
    /// </summary>
    /// <param name="arraySource">Source ArrayFormula.</param>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="iRow">Row (IN THE SOURCE range) of the array formula.</param>
    /// <param name="iColumn">Column (IN THE SOURCE range) of the array formula.</param>
    /// <param name="bUpdateFormula">Indicates whether to update formulas.</param>
    /// <returns>New array formula.</returns>
    private ArrayRecord CreateArrayFormula( ArrayRecord arraySource,
      IRange destination, IRange source, int iRow, int iColumn, bool bUpdateFormula )
    {
      if( arraySource == null )
        throw new ArgumentNullException( "arraySource" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      ParseData();

      Rectangle rectSourceFormula = Rectangle.FromLTRB( arraySource.FirstRow,
        arraySource.FirstColumn, arraySource.LastRow, arraySource.LastColumn );

      // Row and Column are one-based but in ArrayRecord they are zero-based.
      Rectangle rectSourceRange = Rectangle.FromLTRB( source.Row - 1,
        source.Column - 1, source.LastRow - 1, source.LastColumn - 1 );

      Rectangle rectIntersection = Rectangle.Intersect( rectSourceFormula, rectSourceRange );

      if( rectIntersection.IsEmpty )
      {
        ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Intersection is empty - this should never happen" );
        throw new ArgumentNullException( "Intersection is empty" );
      }

      rectIntersection.Offset( destination.Row - source.Row, destination.Column - source.Column );

      if( rectIntersection.Left < 0 )
        throw new ArgumentOutOfRangeException();

      if( rectIntersection.Top < 0 )
        throw new ArgumentOutOfRangeException();

      ArrayRecord result = ( ArrayRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Array );
      result.FirstRow = iRow + destination.Row - source.Row - 1;
      result.FirstColumn = iColumn + destination.Column - source.Column - 1;

      result.LastRow = result.FirstRow - rectIntersection.Left + rectIntersection.Right;
      result.LastColumn = result.FirstColumn - rectIntersection.Top + rectIntersection.Bottom;

      result.IsRecalculateAlways = true;
      result.IsRecalculateOnOpen = true;

      int iRowOffset = bUpdateFormula ? result.FirstRow - arraySource.FirstRow : 0;
      int iColOffset = bUpdateFormula ? result.FirstColumn - arraySource.FirstColumn : 0;

      result.Formula = UpdateFormula( arraySource.Formula, iRowOffset, iColOffset );

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="source"></param>
    protected void CheckRangesSizes( IRange destination, IRange source )
    {
      if( destination.LastRow - destination.Row != source.LastRow - source.Row ||
        destination.LastColumn - destination.Column != source.LastColumn - source.Column )
        throw new ArgumentException( "Ranges do not fit each other" );
    }

    /// <summary>
    /// Copies merged regions.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    private void CopyRangeMerges( IRange destination, IRange source )
    {
      CopyRangeMerges( destination, source, false );
    }
    /// <summary>
    /// Copies merged regions.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="bDeleteSource">Indicates whether to delete source merges after copy.</param>
    private static void CopyRangeMerges( IRange destination, IRange source, bool bDeleteSource )
    {
      RangeImpl sourceRange = ( RangeImpl )source;
      RangeImpl destRange = ( RangeImpl )destination;
      MergeCellsImpl sourceMerge = sourceRange.InnerWorksheet.MergeCells;
      MergeCellsImpl destMerge = destRange.InnerWorksheet.MergeCells;

      if( sourceMerge != null )
      {
        if( sourceMerge == destMerge )
        {
            Rectangle range = Rectangle.FromLTRB(destination.Column - 1, destination.Row - 1,
              destination.LastColumn - 1, destination.LastRow - 1);

            destMerge.DeleteMerge(range);
          sourceMerge.CopyMoveMerges( destRange, sourceRange, bDeleteSource );
        }
        else
        {
          int iRowDelta = destination.Row - source.Row;
          int iColumnDelta = destination.Column - source.Column;

          List<MergeRegion> lstRegions = sourceMerge.FindMergesToCopyMove( sourceRange, bDeleteSource );

          Rectangle range = Rectangle.FromLTRB( destination.Column - 1, destination.Row - 1,
            destination.LastColumn - 1, destination.LastRow - 1 );

          destMerge.DeleteMerge( range );

          destMerge.AddCache( lstRegions, iRowDelta, iColumnDelta );
        }
      }
    }
    /// <summary>
    /// Returns note record by object index.
    /// </summary>
    /// <param name="index">Object index to find NoteRecord for.</param>
    /// <returns>Corresponding NoteRecord, or Null if not found.</returns>
    [CLSCompliant( false )]
    protected internal NoteRecord GetNoteByObjectIndex( int index )
    {
      if( index < 0 )
        throw new ArgumentOutOfRangeException( "index < 0" );

      ParseData();
      NoteRecord result = null;

      if( m_arrNotes != null )
        m_arrNotes.TryGetValue( index, out result );

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="note"></param>
    [CLSCompliant( false )]
    protected internal void AddNote( NoteRecord note )
    {
      ParseData();

      int iKey = ( int )note.ObjId;
      long lCellIndex;
      bool bArrNotesNull = m_arrNotes == null;

      if( !bArrNotesNull && m_arrNotes.ContainsKey( iKey ) )
      {
        NoteRecord oldNote = m_arrNotes[ iKey ];

        lCellIndex = RangeImpl.GetCellIndex( oldNote.Column, oldNote.Row );
        m_arrNotesByCellIndex.Remove( lCellIndex );
      }
      else if( bArrNotesNull )
      {
        m_arrNotes = new SortedList<int, NoteRecord>();
        m_arrNotesByCellIndex = new SortedList<long, NoteRecord>();
      }

      m_arrNotes[ iKey ] = note;
      lCellIndex = RangeImpl.GetCellIndex( note.Column, note.Row );
      m_arrNotesByCellIndex[ lCellIndex ] = note;
    }
    /// <summary>
    /// Autofits row.
    /// </summary>
    /// <param name="rowIndex">Row index.</param>
    /// <param name="firstColumn">One-based index of the first column to be used for autofit operation.</param>
    /// <param name="lastColumn">One-based index of the last column to be used for autofit operation.</param>
    /// <param name="bRaiseEvents">If true then raise events.</param>
    public void AutofitRow( int rowIndex, int firstColumn, int lastColumn, bool bRaiseEvents )
    {
      ParseData();

      RichTextString richText = new RichTextString( Application, this, false, true );

      if( firstColumn == 0 || lastColumn == 0 || firstColumn > lastColumn )
        return;

      SizeF maxSize = new SizeF( 0, 0 );
      SizeF curSize;

      bool isMergedAndWrapped = false;
      for( int j = firstColumn; j <= lastColumn; j++ )
      {
        long lCellIndex = RangeImpl.GetCellIndex( j, rowIndex );

        if( !m_dicRecordsCells.Contains( lCellIndex ) )
          continue;

        curSize = MeasureCell(lCellIndex, true, richText, false, out isMergedAndWrapped);

        if( maxSize.Height < curSize.Height ) maxSize.Height = curSize.Height;
      }

      if( maxSize.Height == 0 )
      {
        maxSize.Height = ( m_book.Styles[ "Normal" ].Font as FontWrapper )
          .Wrapped.MeasureString( DEF_STANDARD_CHAR.ToString() ).Height;
      }

      double newHeight = ApplicationImpl.ConvertFromPixel( maxSize.Height,
        MeasureUnits.Point );

      if( newHeight > RowRecord.DEF_MAX_HEIGHT )
        newHeight = RowRecord.DEF_MAX_HEIGHT;

      (UsedRange[rowIndex, firstColumn] as RangeImpl).SetRowHeight(newHeight, isMergedAndWrapped);
      //( UsedRange[ rowIndex, iFirstColumn ] as RangeImpl ).SetRowHeight( newHeight, true );
    }

    /// <summary>
    /// Sets inner row height.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="bIsBadFontHeight">If true then bad row height.</param>
    /// <param name="units">Current units.</param>
    /// <param name="bRaiseEvents">if true then raise events.</param>
    internal void InnerSetRowHeight( int iRowIndex, double value, bool bIsBadFontHeight
      , MeasureUnits units, bool bRaiseEvents )
    {
      value = Application.ConvertUnits( value, units, MeasureUnits.Point );

      RowStorage rowStorage = WorksheetHelper.GetOrCreateRow( this, iRowIndex - 1, true );

      if (value == 0)
      {
          rowStorage.IsHidden = true;
      }
      else
      {
          ushort setValue = (ushort)(Math.Round(value * 20));
          if (rowStorage.Height != setValue)
          {
              rowStorage.Height = setValue;
              rowStorage.IsBadFontHeight = bIsBadFontHeight;
              WorksheetHelper.AccessRow(this, iRowIndex);
              SetChanged();
          }

          if (bRaiseEvents)
              RaiseRowHeightChangedEvent(iRowIndex, value);
      }
    }
    /// <summary>
    /// Returns True if row is empty;Checking only for styles.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to check.</param>
    /// <returns>True if row is empty.</returns>
    private bool IsRowEmpty( int iRowIndex )
    {
      return IsRowEmpty( iRowIndex, true );
    }
    /// <summary>
    /// Returns True if row is empty
    /// </summary>
    /// <param name="iRowIndex">One-based row index to check.</param>
    /// <param name="bCheckStyle">If true - checking for styles and value, otherwise - for value only.</param>
    /// <returns>True if row is empty.</returns>
    private bool IsRowEmpty( int iRowIndex, bool bCheckStyle )
    {
      ParseData();

      if( iRowIndex < m_iFirstRow || iRowIndex > m_iLastRow ) return true;

      int iDefaultXFIndex = m_book.DefaultXFIndex;

      for( int i = m_iFirstColumn; i <= m_iLastColumn; i++ )
      {
        long index = RangeImpl.GetCellIndex( i, iRowIndex );

        if( m_dicRecordsCells.Contains( index ) )
        {
          bool flag = true;
          ICellPositionFormat format = m_dicRecordsCells.GetCellRecord( index );

          if( bCheckStyle && format.TypeCode == TBIFFRecord.Blank )
          {
            int iXFindex = format.ExtendedFormatIndex;

            if( iXFindex == iDefaultXFIndex || iXFindex == 0 )
              flag = false;
          }

          if( flag ) return false;
        }
      }

      return true;
    }
    /// <summary>
    /// Returns True if column is empty.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index to check.</param>
    /// <returns>True if column is empty.</returns>
    private bool IsColumnEmpty( int iColumnIndex )
    {
      return IsColumnEmpty( iColumnIndex, true );
    }
    /// <summary>
    /// Indicate if column is empty.
    /// </summary>
    /// <param name="iColumnIndex">One-based Column index.</param>
    /// <param name="bIgnoreStyles">If true - ignore styles.</param>
    /// <returns>If true - column is empty.</returns>
    private bool IsColumnEmpty( int iColumnIndex, bool bIgnoreStyles )
    {
      ParseData();

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Value cannot be less 1 and greater than max column index." );

      if( iColumnIndex < m_iFirstColumn || iColumnIndex > m_iLastColumn ) return true;

      int iDefaultXFIndex = m_book.DefaultXFIndex;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        long index = RangeImpl.GetCellIndex( iColumnIndex, i );

        if( m_dicRecordsCells.Contains( index ) )
        {
          bool flag = true;
          ICellPositionFormat format = m_dicRecordsCells.GetCellRecord( index );

          if( bIgnoreStyles && format.TypeCode == TBIFFRecord.Blank )
          {
            int iXFindex = format.ExtendedFormatIndex;

            if( iXFindex == iDefaultXFIndex || iXFindex == 0 )
              flag = false;
          }

          if( flag ) return false;
        }
      }

      return true;
    }
    /// <summary>
    /// Parses Range.
    /// </summary>
    /// <param name="range">Current Range.</param>
    /// <param name="strRowString">String where parsing is.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="i">Current index of parsing string.</param>
    /// <returns>Index of parsed range.</returns>
    private int ParseRange( IRange range, string strRowString, string separator, int i )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      if( strRowString == null )
        throw new ArgumentNullException( "strRowString" );

      if( separator == null )
        throw new ArgumentNullException( "separator" );

      int iLen = strRowString.Length;
      int iSeparatorLen = separator.Length;

      bool bStringEndFound = true;
      int iRecordStart = i;
      int iRecordEnd = i;
      char character;

      while( bStringEndFound && iRecordEnd < iLen )
      {
        character = strRowString[ iRecordEnd ];

        if( character == '"' && iRecordEnd + 1 < iLen )
        {
          int iNextCharacter = strRowString.IndexOf( '"', iRecordEnd + 1 );

          if( iNextCharacter != -1 )
          {
            iRecordEnd = iNextCharacter + 1;
          }
          else
          {
            iRecordEnd++;
          }
        }
        else if( String.CompareOrdinal( strRowString, iRecordEnd, separator, 0, iSeparatorLen ) == 0 )
        {
          bStringEndFound = false;
        }
        else
        {
          iRecordEnd++;
        }
      }

      int iResultLength = iRecordEnd - iRecordStart;
      string result = strRowString.Substring( iRecordStart, iResultLength );

      if( iResultLength > 1 && result[ 0 ] == '"' && result[ iResultLength - 1 ] == '"' )
        result = result.Substring( 1, iResultLength - 2 );

      result = result.Replace( "\"\"", "\"" );

      if( result.IndexOf( '\n' ) >= 0 )
        range.WrapText = true;

          range.Value = result;
          range.Text = result;

      if (this.Application.PreserveCSVDataTypes)
      {
          DateTime dateResult;
          double doubleNum;
          if (DateTime.TryParse(result, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateResult))
              range.DateTime = dateResult;

          if (double.TryParse(result, out doubleNum))
              range.Number = doubleNum;

      }
      return Math.Min(iLen, iRecordEnd + iSeparatorLen - 1);
    }
    /// <summary>
    /// Gets size of string that contain cell found by cellindex.
    /// </summary>
    /// <param name="cell">Cell to measure.</param>
    /// <param name="bAutoFitRows">If true then autofit Rows, otherwise - columns.</param>
    /// <param name="ignoreRotation">Indicates whether rotation must be ignored.</param>
    /// <returns>Returns new size of string.</returns>
    internal SizeF MeasureCell( IRange cell, bool bAutoFitRows, bool ignoreRotation )
    {
      long lCellIndex = RangeImpl.GetCellIndex( cell.Column, cell.Row );
      return MeasureCell( lCellIndex, bAutoFitRows, ignoreRotation );
    }
    /// <summary>
    /// Gets size of string that contain cell found by cellindex.
    /// </summary>
    /// <param name="cellIndex">Cell index to Autofit.</param>
    /// <param name="bAutoFitRows">If true then autofit Rows, otherwise - columns.</param>
    /// <param name="ignoreRotation">Indicates whether rotation must be ignored.</param>
    /// <returns>Returns new size of string.</returns>
    internal SizeF MeasureCell( long cellIndex, bool bAutoFitRows, bool ignoreRotation )
    {
      RichTextString richText = new RichTextString( Application, this, false, true );
      bool isMergedAndWrapped = false;
      return MeasureCell( cellIndex, bAutoFitRows, richText, ignoreRotation, out isMergedAndWrapped );
    }
    /// <summary>
    /// Gets size of string that contain cell found by cellindex.
    /// </summary>
    /// <param name="cellIndex">Cell index to Autofit.</param>
    /// <param name="bAutoFitRows">If true then autofit Rows, otherwise - columns.</param>
    /// <param name="richText">RichTextString object to use for text measuring -
    /// to reduce time and memory consumption.</param>
    /// <param name="ignoreRotation">Indicates whether rotation must be ignored.</param>
    /// <returns>Returns new size of string.</returns>
    private SizeF MeasureCell( long cellIndex, bool bAutoFitRows, RichTextString richText, bool ignoreRotation,out bool bIsMergedAndWrapped)
    {
      ParseData();

      //RichTextString rtfString = m_dicRecordsCells.GetRTFString( cellIndex, bAutoFitRows );
      m_dicRecordsCells.FillRTFString( cellIndex, bAutoFitRows, richText );
      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex );
      bool isMerged = false;
      string strText = richText.Text;

      if( strText == null || strText.Length == 0 )
      {
          bIsMergedAndWrapped = false;
        return new SizeF( 0, 0 );
      }

      if( m_mergedCells != null )
      {
        Rectangle rect = Rectangle.FromLTRB( iColumn - 1, iRow - 1, iColumn - 1, iRow - 1 );

        MergeRegion region = m_mergedCells[ rect ];

        if( region != null )
        {
          if( bAutoFitRows && ( region.RowFrom <= iRow - 1 || region.RowTo >= iRow - 1 )
            || !bAutoFitRows && ( region.ColumnFrom <= iColumn - 1 || region.ColumnTo >= iColumn - 1 ) )
          {
              isMerged = true;
          }
        }
      }

      FontImpl font = ( m_book.Styles[ "Normal" ].Font as FontWrapper ).Wrapped;

      ExtendedFormatImpl format = GetExtendedFormat( cellIndex );
      int rotation = format.Rotation;
      SizeF curSize = richText.StringSize;
      bool bIndentRight = format.HorizontalAlignment == ExcelHAlign.HAlignRight;

      if( bAutoFitRows )
      {
        if(!isMerged && format.WrapText )
        {
           int value = AutoFitManagerImpl.CalculateWrappedCell(format, strText, this.GetColumnWidthInPixels(iColumn),this.AppImplementation);
           curSize.Height = value;
		   //curSize = WrapLine( this, richText,iColumn );
        }

        //if( !ignoreRotation )
        //  curSize.Height = UpdateTextWidthOrHeightByRotation( curSize, //rotation, true );
      }
      else
      {
        curSize = UpdateAutofitByIndent( curSize, format );

        if( !ignoreRotation )
          curSize.Width = UpdateTextWidthOrHeightByRotation( curSize, rotation, false );

        IRange filterRange = m_autofilters.FilterRange;

        if( filterRange != null && filterRange.Row == iRow && iColumn >= filterRange.Column
          && iColumn <= filterRange.LastColumn )
        {
          curSize = UpdateAutoFitByAutoFilter( curSize, format, m_dicRecordsCells, cellIndex );
        }
      }
      bIsMergedAndWrapped = isMerged && format.WrapText;
      //int spaceLimit=84;
      //if (format.WrapText && richText.Text.Length>spaceLimit)
     // {
          //TODO: Need to calculate properly.
      //    FontImpl fontImpl = richText.GetFontObject(0);
      //    string strValue = " ";
      //    SizeF tempSizeF = fontImpl.MeasureStringSpecial(strValue);
      //    curSize.Height += ((int)tempSizeF.Height / 3);
      //    curSize.Width += ((int)tempSizeF.Width / 3);
      //}
      return curSize;
    }
    private Size WrapLine( IWorksheet sheet, IRichTextString rtf,int columnIndex)
    {
      // TODO: this set of methods requires optimization. Currently it is placed to research autofit and string measurement.
      RichTextString rtfString = ( rtf as RichTextString );
      // 1. split by \n
      string[] lines = rtfString.Text.Split( '\n' );
      int startPos = 0;
      Size resultSize = Size.Empty;
      int availableWidth = sheet.GetColumnWidthInPixels(columnIndex);

      foreach( string line in lines )
      {
        RichTextString stringPart = rtfString.Clone( rtfString.Parent ) as RichTextString;
        string trimmedLine = line.TrimEnd( '\r' );
        stringPart.Substring( startPos, trimmedLine.Length );

        Size lineSize = WrapSingleLine( line, availableWidth, stringPart );
        resultSize.Height += lineSize.Height;
        resultSize.Width = Math.Max( lineSize.Width, resultSize.Width );
        startPos += line.Length + 1;
      }

      return resultSize;
    }
    private Size WrapSingleLine( string line, int availableWidth, RichTextString stringPart )
    {
      Size resultSize = Size.Empty;

      SizeF stringPartSize = stringPart.StringSize;
      int totalWidth = ( int )stringPartSize.Width;

      // 2. if it fits - great
      if( totalWidth > availableWidth )
      {
        stringPartSize = FitByWords( stringPart, availableWidth );
      }

      resultSize.Height += ( int )stringPartSize.Height;
      resultSize.Width = Math.Max( ( int )stringPartSize.Width, resultSize.Width );

      return resultSize;
    }

    private Size FitByWords( RichTextString stringPart, int availableWidth )
    {
      RichTextString originalString = ( RichTextString )stringPart.Clone( stringPart.Parent );
      int currentIndex = 0;
      int startIndex = 0;
      int totalLen = stringPart.Text.Length;
      Size resultSize = Size.Empty;

      while( startIndex < totalLen )
      {
        stringPart = AddNextWord( originalString, currentIndex, ref currentIndex );
        RichTextString prevString = null;
        int prevIndex = -1;

        // 1. first word cannot be placed inside available width - then split it by chars
        SizeF currentSize = stringPart.StringSize;

        while( currentSize.Width < availableWidth && currentIndex < originalString.Text.Length )
        {
          prevString = stringPart;
          prevIndex = currentIndex;
          stringPart = AddNextWord( originalString, startIndex, ref currentIndex );
          currentSize = stringPart.StringSize;
        }

        if( currentSize.Width > availableWidth )
        {
          stringPart = prevString;
          currentIndex = prevIndex;
        }

        SizeF partSize;

        if( stringPart != null )
        {
          // we have successfully splitted it by words. Process the next part of this string
          partSize = stringPart.StringSize;
        }
        else
        {
          // string cannot be split by words
          currentIndex = startIndex;
          partSize = SplitByChars( originalString, startIndex, ref currentIndex, availableWidth );
        }

        resultSize.Width = Math.Max( ( int )partSize.Width, resultSize.Width );
        resultSize.Height += ( int )partSize.Height;
        startIndex = currentIndex;
        if (currentIndex == 0) startIndex++;
      }

      return resultSize;

      //if( firstWordSize.Width > availableWidth )
      //{
      //}
      //// 2. first word can be placed inside available width - then try to add next ones.
      //else
      //{
      //  RichTextString prevString = stringPart;
      //  int prevIndex = currentIndex;
      //  stringPart = AddNextWord( originalString, ref currentIndex );
      //}
    }

    private SizeF SplitByChars( RichTextString originalString, int startIndex, ref int currentIndex, int availableWidth )
    {
      int totalLen = originalString.Text.Length;
      Size resultSize = Size.Empty;
      Size prevSize = Size.Empty;

      while( currentIndex < totalLen && resultSize.Width < availableWidth )
      {
        RichTextString stringPart = ( RichTextString )originalString.Clone( originalString.Parent );
        stringPart.Substring( startIndex, currentIndex - startIndex + 1 );
        prevSize = resultSize;
        resultSize = stringPart.StringSize.ToSize();

        if( resultSize.Width > availableWidth )
        {
          resultSize = prevSize;
          break;
        }

        currentIndex++;
      }

      return resultSize;
    }

    private RichTextString AddNextWord( RichTextString originalString, int startIndex, ref int currentIndex )
    {
      RichTextString result = ( RichTextString )originalString.Clone( originalString.Parent );
      int delimIndex = result.Text.IndexOfAny( new char[] { '-', ' ' }, currentIndex );

      if( delimIndex < 0 )
        delimIndex = result.Text.Length - 1;

      result.Substring( startIndex, delimIndex - startIndex + 1 );
      currentIndex = delimIndex + 1;
      return result;
    }
    /// <summary>
    /// Updates indent size.
    /// </summary>
    /// <param name="curSize">Represents current size.</param>
    /// <param name="format">Represents extended format.</param>
    /// <returns>Returns updated size by indent value.</returns>
    private SizeF UpdateAutofitByIndent( SizeF curSize, ExtendedFormatImpl format )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      bool bFlag = format.HorizontalAlignment != ExcelHAlign.HAlignLeft && format.HorizontalAlignment
        != ExcelHAlign.HAlignRight;

      if( bFlag && format.Rotation != 0 && format.IndentLevel == 0 )
        return curSize;

      curSize.Width += format.IndentLevel * DEF_INDENT_WIDTH;

      return curSize;
    }
    /// <summary>
    /// Updates text width by rotation.
    /// </summary>
    /// <param name="size">String size without rotation.</param>
    /// <param name="rotation">Current rotation.</param>
    /// <param name="bUpdateHeight">If true then update height otherwise - width.</param>
    /// <returns>Updated width or height.</returns>
    private float UpdateTextWidthOrHeightByRotation( SizeF size, int rotation,
      bool bUpdateHeight )
    {
      if( rotation == 0 )
      {
        return bUpdateHeight ? size.Height : size.Width;
      }

      if( rotation == 90 || rotation == 180 )
      {
        return bUpdateHeight ? size.Width : size.Height;
      }

      if( rotation > 90 ) rotation -= 90;

      if( bUpdateHeight ) rotation = 90 - rotation;

      float fPart = ( float )Math.Sin( DEF_AXE_IN_RADIANS * rotation ) * size.Height;
      float fResult = ( float )Math.Cos( DEF_AXE_IN_RADIANS * rotation ) * size.Width;

      return fResult + fPart;

    }
    /// <summary>
    /// Gets font by extended format index.
    /// </summary>
    /// <param name="cellFormat">Record that contain extended format index.</param>
    /// <param name="rotation">Represents the rotation. Out Parameter.</param>
    /// <returns>Returns font, rotation by extended format.</returns>
    private FontImpl GetFontByExtendedFormatIndex( ICellPositionFormat cellFormat, out int rotation )
    {
      ExtendedFormatsCollection arrExtFormats = m_book.InnerExtFormats;

      if( arrExtFormats.Count <= cellFormat.ExtendedFormatIndex )
        throw new ArgumentException( "cellFormat" );

      ExtendedFormatImpl extFormat = arrExtFormats[ cellFormat.ExtendedFormatIndex ];

      rotation = extFormat.Rotation;

      return ( FontImpl )m_book.InnerFonts[ extFormat.FontIndex ];
    }
    /// <summary>
    /// Copies different sheet options.
    /// </summary>
    /// <param name="sourceSheet">Source sheet.</param>
    protected override void CopyOptions( WorksheetBaseImpl sourceSheet )
    {
      base.CopyOptions( sourceSheet );
      WorksheetImpl sheet = ( WorksheetImpl )sourceSheet;

      IsRowColumnHeadersVisible = sheet.IsRowColumnHeadersVisible;
      IsStringsPreserved = sheet.IsStringsPreserved;
      IsGridLinesVisible = sheet.IsGridLinesVisible;

      m_pane = ( PaneRecord )CloneUtils.CloneCloneable( sheet.m_pane );
    }

    /// <summary>
    /// This method is called after RealIndex property change.
    /// </summary>
    /// <param name="iOldIndex">Old value.</param>
    protected override void OnRealIndexChanged( int iOldIndex )
    {
      if( m_names != null )
        m_names.SetSheetIndex( RealIndex );
    }

    /// <summary>
    /// This method is called after insert row or column operation is complete
    /// and it fires event handlers if necessary.
    /// </summary>
    /// <param name="iRowIndex">Row or column index to insert at.</param>
    /// <param name="iRowCount">Number of inserted rows or columns.</param>
    /// <param name="bRow">Indicates whether rows were inserted.</param>
    private void OnInsertRowColumnComplete( int iRowIndex, int iRowCount, bool bRow )
    {
      //      if( RowColumnInserted != null )
      //      {
      //        RowColumnInsertEventArgs args = new RowColumnInsertEventArgs( iRowIndex, iRowCount, bRow );
      //        RowColumnInserted( this, args );
      //      }
    }
    /// <summary>
    /// Updates fit size by autofilter arrow.
    /// </summary>
    /// <param name="size">Represents current size.</param>
    /// <param name="format">Represents extended format.</param>
    /// <param name="col">Represents cell records collection.</param>
    /// <param name="cellIndex">Represents cell index.</param>
    /// <returns>Returns new size, that contain size of arrow.</returns>
    private SizeF UpdateAutoFitByAutoFilter( SizeF size, ExtendedFormatImpl format,
      CellRecordCollection col, long cellIndex )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( col == null )
        throw new ArgumentNullException( "col" );

      ExcelHAlign align = format.HorizontalAlignment;
      int iRotation = format.Rotation;

      if( align == ExcelHAlign.HAlignCenterAcrossSelection || align == ExcelHAlign.HAlignFill
        || align == ExcelHAlign.HAlignRight )
      {
        return size;
      }

      if( align == ExcelHAlign.HAlignJustify || align == ExcelHAlign.HAlignDistributed )
      {
        if( iRotation > 0 && iRotation < 90 )
          size.Width += DEF_AUTO_FILTER_WIDTH;

        return size;
      }

      if( align == ExcelHAlign.HAlignLeft || align == ExcelHAlign.HAlignCenter )
      {
        size.Width += ( align == ExcelHAlign.HAlignLeft )
          ? DEF_AUTO_FILTER_WIDTH
          : 2 * DEF_AUTO_FILTER_WIDTH;

        return size;
      }

      return UpdateAutoFilterForGeneralAllignment( size, iRotation, col, cellIndex );
    }
    /// <summary>
    /// Updates size for general alignment for autofilter arrow.
    /// </summary>
    /// <param name="size">Represents current size.</param>
    /// <param name="iRot">Represents rotation.</param>
    /// <param name="col">Represents cells collection.</param>
    /// <param name="cellIndex">Represents cell index.</param>
    /// <returns>Returns updated size.</returns>
    private SizeF UpdateAutoFilterForGeneralAllignment( SizeF size, int iRot
      , CellRecordCollection col, long cellIndex )
    {
      if( col == null )
        throw new ArgumentNullException( "col" );

      if( m_dicRecordsCells.ContainFormulaBoolOrError( cellIndex )
        || m_dicRecordsCells.ContainBoolOrError( cellIndex ) )
      {
        size.Width += 2 * DEF_AUTO_FILTER_WIDTH;

        return size;
      }

      if( ( iRot > 0 && iRot < 90 ) || iRot >= 180 )
      {
        size.Width += DEF_AUTO_FILTER_WIDTH;

        return size;
      }

      if( !m_dicRecordsCells.ContainFormulaNumber( cellIndex )
        && !m_dicRecordsCells.ContainNumber( cellIndex ) && iRot == 0 )
      {
        size.Width += DEF_AUTO_FILTER_WIDTH;
      }

      return size;
    }
    /// <summary>
    /// Creates migrant range.
    /// </summary>
    private void CreateMigrantRange()
    {
      m_migrantRange = new MigrantRangeImpl( Application, this );
      //m_migrantRange.ResetRowColumn( m_iFirstRow, m_iFirstColumn );
    }
    /// <summary>
    /// Returns default outline style.
    /// </summary>
    /// <param name="dicOutlines">Dictionary with outlines.</param>
    /// <param name="iIndex">Outline index.</param>
    /// <returns>Extracted outline style.</returns>
    private IStyle GetDefaultOutlineStyle( IDictionary dicOutlines, int iIndex )
    {
      if( dicOutlines == null )
        throw new ArgumentNullException( "dicOutlines" );

      IOutline outline = ( IOutline )dicOutlines[ iIndex ];

      int iXFIndex = ( outline != null )
        ? ( int )outline.ExtendedFormatIndex
        : m_book.DefaultXFIndex;

      return new ExtendedFormatWrapper( m_book, iXFIndex );
    }
    /// <summary>
    /// Sets row or column default style.
    /// </summary>
    /// <param name="iIndex">Row or column index.</param>
    /// <param name="iEndIndex">End row or column index.</param>
    /// <param name="defaultStyle">Style to set.</param>
    /// <param name="dicOutlines">Collection of outlines that contains style info.</param>
    /// <param name="createOutline">Delegate used for outline creation.</param>
    /// <param name="bIsRow">Indicates is in row or column.</param>
    /// <returns>XF index that was set.</returns>
    private int SetDefaultRowColumnStyle( int iIndex, int iEndIndex, IStyle defaultStyle,
      IDictionary dicOutlines, OutlineDelegate createOutline, bool bIsRow )
    {
      ParseData();

      int iXFIndex = ConvertStyleToCorrectIndex( defaultStyle );
      IOutline outline;

      for( int i = iIndex; i <= iEndIndex; i++ )
      {
        outline = dicOutlines.Contains( i )
          ? ( IOutline )dicOutlines[ i ]
          : createOutline( i );

        outline.ExtendedFormatIndex = ( ushort )iXFIndex;
      }

      return iXFIndex;
    }
    /// <summary>
    /// Sets row or column default style.
    /// </summary>
    /// <param name="iIndex">Row or column index.</param>
    /// <param name="iEndIndex">End row or column index.</param>
    /// <param name="defaultStyle">Style to set.</param>
    /// <param name="outlines">Collection of outlines that contains style info.</param>
    /// <param name="createOutline">Delegate used for outline creation.</param>
    /// <param name="bIsRow">Indicates is in row or column.</param>
    /// <returns>XF index that was set.</returns>
    private int SetDefaultRowColumnStyle( int iIndex, int iEndIndex, IStyle defaultStyle,
      IList outlines, OutlineDelegate createOutline, bool bIsRow )
    {
      ParseData();

      int iXFIndex = ConvertStyleToCorrectIndex( defaultStyle );
      IOutline outline;

      for( int i = iIndex; i <= iEndIndex; i++ )
      {
        outline = ( outlines[ i ] != null )
          ? ( IOutline )outlines[ i ]
          : createOutline( i );

        outline.ExtendedFormatIndex = ( ushort )iXFIndex;
		SetCellStyle(i, (ushort )iXFIndex);
      }

      return iXFIndex;
    }
    /// <summary>
    /// Converts style object into XF index that can be assigned to cell or ColumnInfo, Row records.
    /// </summary>
    /// <param name="style">Style to convert.</param>
    /// <returns>Converted style.</returns>
    private int ConvertStyleToCorrectIndex( IStyle style )
    {
      if( style == null )
        throw new ArgumentNullException( "defaultStyle" );

      int iXFIndex = ( ( IXFIndex )style ).XFormatIndex;

      if( iXFIndex == int.MinValue )
        throw new ArgumentException( "defaultStyle" );

      ExtendedFormatImpl format = m_book.InnerExtFormats[ iXFIndex ];
      format = format.CreateChildFormat();
      return format.Index;
    }
    /// <summary>
    /// Creates new column record.
    /// </summary>
    /// <param name="iColumnIndex">Column index to create record for.</param>
    /// <returns>Created row.</returns>
    private IOutline CreateColumnOutline( int iColumnIndex )
    {
      ParseData();

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
      {
        throw new ArgumentOutOfRangeException( "iColumnIndex"
          , "Column index is out of range." );
      }

      ColumnInfoRecord columnRecord = BiffRecordFactory.GetRecord(
        TBIFFRecord.ColumnInfo ) as ColumnInfoRecord;

      columnRecord.FirstColumn = ( ushort )( iColumnIndex - 1 );
      columnRecord.LastColumn = ( ushort )( iColumnIndex - 1 );
      columnRecord.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
      WorksheetHelper.AccessColumn( this, iColumnIndex );

      //m_arrColumnInfo.Add( iColumnIndex, columnRecord );
      m_arrColumnInfo[ iColumnIndex ] = columnRecord;

      return columnRecord;
    }
    /// <summary>
    /// Copies conditional formats after row/column insert.
    /// </summary>
    /// <param name="iIndex">Row/column index.</param>
    /// <param name="iCount">Rows/columns count.</param>
    /// <param name="options">Excel insert option.</param>
    /// <param name="bIsRow">True if row was inserted, false if column was inserted.</param>
    private void CopyConditionalFormatsAfterInsert( int iIndex, int iCount, ExcelInsertOptions options, bool bIsRow )
    {
      if( options == ExcelInsertOptions.FormatDefault )
        return;

      if( bIsRow )
      {
        int iSourceRow = ( options == ExcelInsertOptions.FormatAsBefore ) ? iIndex - 1 : iIndex + iCount;
        int iColumnCount = m_iLastColumn - m_iFirstColumn + 1;

        for( int iCurrentRow = iIndex, iRowCount = iIndex + iCount; iCurrentRow < iRowCount; iCurrentRow++ )
        {
          CopyMoveConditionalFormatting( iSourceRow, m_iFirstColumn, 1, iColumnCount, iCurrentRow, m_iFirstColumn,
            this, false );
        }
      }
      else
      {
        int iSourceColumn = ( options == ExcelInsertOptions.FormatAsBefore ) ? iIndex - 1 : iIndex + iCount;

        for( int iCurrentColumn = iIndex, iColumnCount = iIndex + iCount; iCurrentColumn < iColumnCount; iCurrentColumn++ )
        {
          CopyMoveConditionalFormatting( m_iFirstRow, iSourceColumn, m_iLastRow, 1, m_iFirstRow, iCurrentColumn,
            this, false );
        }
      }
    }
    /// <summary>
    /// Copies style from above/below/left/right after insert row/column operation.
    /// </summary>
    /// <param name="iIndex">Index where insert operation took place.</param>
    /// <param name="iCount">Number of inserted rows/columns.</param>
    /// <param name="options">Insert options.</param>
    /// <param name="bRow">Indicates whether rows where inserted.</param>
    private void CopyStylesAfterInsert( int iIndex, int iCount, ExcelInsertOptions options, bool bRow )
    {
      int iIndexWithSourceStyle = GetIndexForStyleCopy( iIndex, iCount, options );

      int iSecondStartIndex;
      int iSecondEndIndex;

      if( !bRow )
      {
        iSecondStartIndex = m_iFirstRow;
        iSecondEndIndex = m_iLastRow;
      }
      else if( m_iFirstColumn == DEF_MIN_COLUMN_INDEX )
      {
        iSecondStartIndex = -1;
        iSecondEndIndex = -1;
      }
      else
      {
        iSecondStartIndex = m_iFirstColumn;
        iSecondEndIndex = m_iLastColumn;
      }

      RowStorage sourceRow = null;
      ColumnInfoRecord sourceColumn = null;

      if( iIndexWithSourceStyle != -1 )
      {
        if( bRow )
        {
          sourceRow = WorksheetHelper.GetOrCreateRow( this, iIndexWithSourceStyle - 1, false );
        }
        else
        {
          sourceColumn = m_arrColumnInfo[ iIndexWithSourceStyle ];
        }

        if( iSecondStartIndex > 0 )
        {
          for( int j = iSecondStartIndex; j <= iSecondEndIndex; j++ )
          {
            long lCellIndex = bRow
              ? RangeImpl.GetCellIndex( j, iIndexWithSourceStyle )
              : RangeImpl.GetCellIndex( iIndexWithSourceStyle, j );

            ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( lCellIndex );

            if( cell == null || cell.ExtendedFormatIndex == m_book.DefaultXFIndex || (sourceRow!=null && cell.ExtendedFormatIndex==sourceRow.ExtendedFormatIndex) )
              continue;

            for( int i = iIndex, iLast = iIndex + iCount; i < iLast; i++ )
            {
              lCellIndex = bRow
                ? RangeImpl.GetCellIndex( j, i )
                : RangeImpl.GetCellIndex( i, j );

              BlankRecord blank = ( BlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Blank );
              blank.Row = ( bRow ? i : j ) - 1;
              blank.Column = ( bRow ? j : i ) - 1;
              blank.ExtendedFormatIndex = cell.ExtendedFormatIndex;
              m_dicRecordsCells.SetCellRecord( lCellIndex, blank );
            }
          }
        }
      }

      if( bRow )
      {
        for( int i = iIndex, iLast = iIndex + iCount; i < iLast; i++ )
        {
          CopyRowColumnSettings( sourceRow, sourceColumn, bRow, iIndexWithSourceStyle, i, options );
        }
      }
    }
    /// <summary>
    /// Copies row and column settings
    /// </summary>
    /// <param name="sourceRow">Represents source row.</param>
    /// <param name="sourceColumn">Represents source column.</param>
    /// <param name="bRow">Represents row or column</param>
    /// <param name="iSourceIndex">Source index</param>
    /// <param name="iCurIndex">Current index</param>
    /// <param name="options">Insert option</param>
    private void CopyRowColumnSettings( RowStorage sourceRow, ColumnInfoRecord sourceColumn, bool bRow,
      int iSourceIndex, int iCurIndex, ExcelInsertOptions options )
    {
      if( options == ExcelInsertOptions.FormatDefault )
      {
        if( bRow )
        {
          RowStorage curRow = WorksheetHelper.GetOrCreateRow( this, iCurIndex - 1, false );

          if( curRow != null )
          {
            curRow.SetDefaultRowOptions();
            curRow.Height = (ushort)AppImplementation.StandardHeightInRowUnits;
            curRow.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
          }
        }
        else
        {
          ColumnInfoRecord columnInfo = m_arrColumnInfo[ iCurIndex ];

          if( columnInfo != null )
          {
            columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( iCurIndex - 1 );
            columnInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
            columnInfo.SetDefaultOptions();
          }
        }
      }
      else if( iSourceIndex != -1 )
      {
        if( bRow )
        {
          RowStorage curRow = WorksheetHelper.GetOrCreateRow( this, iCurIndex - 1, sourceRow != null );

          if( sourceRow == null && curRow != null )
          {
            // Set default row settings
            //throw new NotImplementedException();
            curRow.Height = ( ushort )AppImplementation.StandardHeightInRowUnits;
            curRow.SetDefaultRowOptions();
          }
          else if( sourceRow != null )
          {
            curRow.CopyRowRecordFrom( sourceRow );
          }
        }
        else
        {
          ColumnInfoRecord result = ( ColumnInfoRecord )CloneUtils.CloneCloneable( sourceColumn );
          m_arrColumnInfo[ iCurIndex ] = result;

          if( result != null )
          {
            result.FirstColumn = result.LastColumn = ( ushort )( iCurIndex - 1 );
          }
        }
      }
    }
    /// <summary>
    /// Calculates row or column index from which style must be copied
    /// into inserted area.
    /// </summary>
    /// <param name="iIndex">Row or column index whether insert operation was called.</param>
    /// <param name="iCount">Number of rows/columns to insert.</param>
    /// <param name="options">Insert options.</param>
    /// <returns>Update row or column index.</returns>
    private int GetIndexForStyleCopy( int iIndex, int iCount, ExcelInsertOptions options )
    {
      switch( options )
      {
        case ExcelInsertOptions.FormatAsBefore:
          iIndex--;
          break;

        case ExcelInsertOptions.FormatAsAfter:
          iIndex += iCount;
          break;

        case ExcelInsertOptions.FormatDefault:
        default:
          iIndex = -1;
          break;
      }

      return iIndex;
    }

    /// <summary>
    /// Returns format type for specified column.
    /// </summary>
    /// <param name="iRow">One-based row index for exported cell.</param>
    /// <param name="iColumn">One-based column index for exported cell.</param>
    /// <param name="bUseDefaultStyle">Indicates whether to use default style.</param>
    /// <returns>Format type for specified column.</returns>
    private ExcelFormatType GetFormatType( int iRow, int iColumn, bool bUseDefaultStyle )
    {
      int iXFIndex;
      if( bUseDefaultStyle )
      {
        ColumnInfoRecord columnInfo = m_arrColumnInfo[ iColumn ] as ColumnInfoRecord;

        if( columnInfo == null )
          return ExcelFormatType.General;

        iXFIndex = columnInfo.ExtendedFormatIndex;
      }
      else
      {
        ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( iRow, iColumn );

        iXFIndex = ( cell != null )
          ? ( int )cell.ExtendedFormatIndex
          : m_book.DefaultXFIndex;
      }

      ExtendedFormatImpl xf = m_book.InnerExtFormats[ iXFIndex ];
      int iNumberFormat = xf.NumberFormatIndex;
      FormatImpl format = m_book.InnerFormats[ iNumberFormat ];
      return format.GetFormatType( 1 );
    }
    /// <summary>
    /// Returns type of the elements based on format type.
    /// </summary>
    /// <param name="formatType">Format type that must be converted into System.Type.</param>
    /// <param name="row">Represents cell's row index.</param>
    /// <param name="column">Represents cell's column index.</param>
    /// <param name="maxRows">Represents maximum numbre of rows to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>Type of the elements based on format type.</returns>
    private ExcelExportType GetExportType(ExcelFormatType formatType, int row, int column,
     int maxRows, ExcelExportDataTableOptions options, out Type formulaDataType)
    {
        ParseData();
        bool bExportFormulaValues = ((options & ExcelExportDataTableOptions.ComputedFormulaValues) != 0);
        bool preserveOLEDate = ((options & ExcelExportDataTableOptions.PreserveOleDate) != 0);
        formulaDataType = null;
        ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord(row, column);
        ExcelExportType result = ExcelExportType.Text;
        int iLastRow = row + maxRows;
        bool bUseDefaultStyles = ((options & ExcelExportDataTableOptions.DefaultStyleColumnTypes) != 0);
        while (row <= iLastRow && (cell == null || (cell != null && cell.TypeCode == TBIFFRecord.Blank)))
        {
            row++;
            formatType = GetFormatType(row, column, bUseDefaultStyles);
            cell = m_dicRecordsCells.GetCellRecord(row, column);
        }

        if (cell != null)
        {
            switch (cell.TypeCode)
            {
                case TBIFFRecord.BoolErr:
                    if (formatType != ExcelFormatType.Text)
                    {
                        BoolErrRecord boolErr = (BoolErrRecord)cell;

                        result = boolErr.IsErrorCode
                          ? ExcelExportType.Error
                          : ExcelExportType.Bool;
                    }
                    break;

                case TBIFFRecord.Number:
                case TBIFFRecord.RK:
                    if (formatType != ExcelFormatType.Text)
                    {
                        result = (formatType == ExcelFormatType.DateTime)
                          ? ExcelExportType.DateTime
                          : ExcelExportType.Number;
                    }
                    break;

                case TBIFFRecord.LabelSST:
                    result = ExcelExportType.Text;
                    break;

                case TBIFFRecord.Formula:
                    bool bComputedValue = (options & ExcelExportDataTableOptions.ComputedFormulaValues) != 0;

                    if (bComputedValue)
                    {
                        if (formatType == ExcelFormatType.General)
                        {
                            if (bExportFormulaValues)
                            {
                                FormulaRecord formula = null;
                                formula = cell as FormulaRecord;
                                IRange range = this.Range[row, column];
                                if (bExportFormulaValues)
                                {
                                    object a = this.Range[row, column].CalculatedValue;
                                }
                                cell = m_dicRecordsCells.GetCellRecord(row, column);
                                formula = cell as FormulaRecord;

                                if (range.HasFormulaStringValue)
                                {
                                    result = ExcelExportType.Text;
                                }
                                else if (range.HasFormulaBoolValue)
                                {
                                    result = ExcelExportType.Bool;
                                }
                                else if (range.HasFormulaDateTime)
                                {
                                    result = ExcelExportType.DateTime;
                                }
                                else if (range.HasFormulaNumberValue)
                                    result = ExcelExportType.Number ;
                            }
                            else
                                result = ExcelExportType.Text;
                        }
                        else if (formatType != ExcelFormatType.Text)
                        {
                            result = (formatType == ExcelFormatType.DateTime) ?
                              ExcelExportType.DateTime :
                              ExcelExportType.Number;
                        }
                    }
                    else
                    {
                        result = ExcelExportType.Formula;
                    }
                    break;

                default:
                    result = ExcelExportType.Text;
                    break;
            }
        }

        return result;
    }
    /// <summary>
    /// Returns type of the elements based on format type.
    /// </summary>
    /// <param name="exportType">Export type that must be converted into System.Type.</param>
    /// <returns>Type of the elements based on format type.</returns>
    private Type GetType( ExcelExportType exportType, bool preserveOLEDate )
    {
      switch( exportType )
      {
        case ExcelExportType.DateTime:
          return preserveOLEDate == true ?
            typeof(double) :
            typeof(DateTime);                

        case ExcelExportType.Bool:
          return typeof( bool );

        case ExcelExportType.Number:
          return typeof( double );

        case ExcelExportType.Text:
        case ExcelExportType.Error:
        case ExcelExportType.Formula:
          return typeof( string );
      }

      throw new ArgumentOutOfRangeException( "exportType" );
    }
    /// <summary>
    /// Returns cell value.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to get value from.</param>
    /// <param name="iColumn">One-based column index of the cell to get value from.</param>
    /// <param name="formatType">Format type.</param>
    /// <param name="bExportFormulaValues">Indicates whether to export formula values.</param>
    /// <returns>Cell value.</returns>
    private object GetValue( int iRow, int iColumn, ExcelExportType formatType,
      bool bExportFormulaValues, bool preserveOLEDate )
    {
      // TODO: optimize this function.
      ParseData();

      ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( iRow, iColumn );

      if( cell == null || cell.TypeCode == TBIFFRecord.Blank )
        return DBNull.Value;

      FormulaRecord formula = null;

      if( cell.TypeCode == TBIFFRecord.Formula )
      {
          formula = cell as FormulaRecord;
          if (bExportFormulaValues && formula.IsBlank)
              return DBNull.Value;
          if (bExportFormulaValues)
          {
              object a = this.Range[iRow, iColumn].CalculatedValue;
          }
          cell = m_dicRecordsCells.GetCellRecord(iRow, iColumn);
          formula = cell as FormulaRecord;
      }

      //IRange range = Range[ iRow, iColumn ];
      //IMigrantRange range = this.MigrantRange;
      //range.ResetRowColumn( iRow, iColumn );

      object objResult = null;
      IDoubleValue doubleValue;

      if( formatType == ExcelExportType.Text )
      {
        if( !bExportFormulaValues || cell.TypeCode != TBIFFRecord.Formula )//!range.HasFormula )
        {
          //range.ResetRowColumn( iRow, iColumn );
          objResult = GetValue( cell, preserveOLEDate);//range.Value;
#if !WINRT
          if (objResult.ToString().Contains("/"))
          {
              DateTime date;
              if (DateTime.TryParse(objResult.ToString(), out date))
              {
                  objResult = date.ToShortDateString();
              }
          }
#endif
        }
        else
        {
          double dFormulaNumberValue = formula.Value;
          string strFormulaStringValue = GetFormulaStringValue( iRow, iColumn );
          ExtendedFormatImpl format = m_book.InnerExtFormats[ cell.ExtendedFormatIndex ];
          FormatImpl numberFormat = format.NumberFormatSettings as FormatImpl;

          if( strFormulaStringValue != null )
          {
            objResult = strFormulaStringValue;
          }
          else if( bExportFormulaValues && formula.IsError )
          {
            byte errorValue= formula.ErrorValue;
            objResult = GetErrorValueToString(errorValue, iRow);              
          }
          else if (bExportFormulaValues && formula.IsBool)
          {
              IRange range = Range[ iRow, iColumn ];
              objResult = range.FormulaBoolValue;
          }
          else if (numberFormat.GetFormatType(dFormulaNumberValue) == ExcelFormatType.DateTime)
          {
              if (!double.IsNaN(dFormulaNumberValue))
              {
                  DateTime date = UtilityMethods.ConvertNumberToDateTime(dFormulaNumberValue,m_book.Date1904);

                  if (!preserveOLEDate)
                  {
                      objResult = date;
                  }
                  else
                  {
                      objResult = date.ToOADate();
                  }
              }
              else
              {
                  objResult = GetFormulaBoolValue(iRow, iColumn);
              }
          }
          else
          {
            objResult = dFormulaNumberValue;
          }
        }
      }
      else
      {
        switch( formatType )
        {
          case ExcelExportType.DateTime:
            //range.ResetRowColumn( iRow, iColumn );
            doubleValue = cell as IDoubleValue;
            double dValue = ( doubleValue != null ) ?
              doubleValue.DoubleValue :
              double.NaN;

            if (preserveOLEDate == true)
            {
              objResult = dValue;
            }
            else
            {
              objResult = UtilityMethods.ConvertNumberToDateTime( dValue ,m_book.Date1904);
            }
            break;

          case ExcelExportType.Number:
            doubleValue = cell as IDoubleValue;
            objResult = ( doubleValue != null ) ?
              doubleValue.DoubleValue :
              double.NaN;
            //GetNumber( iRow, iColumn );//range.Number;
            break;

          case ExcelExportType.Bool:
            BoolErrRecord boolErr = cell as BoolErrRecord;
            objResult = ( boolErr != null && !boolErr.IsErrorCode ) ?
              boolErr.BoolOrError != 0 :
              false;
            break;

          case ExcelExportType.Error:
            objResult = GetError( iRow, iColumn );//range.Error;
            break;

          case ExcelExportType.Formula:
            objResult = GetFormulaStringValue(iRow, iColumn);
            break;

          default:
            objResult = GetText( iRow, iColumn );//range.Text;
            break;
        }
      }

      return objResult;
    }
    /// <summary>
    /// Gets value from the cell record.
    /// </summary>
    /// <param name="cell">Cell to get value from.</param>
    /// <returns>String representation of the cell's value.</returns>
    internal string GetValue( ICellPositionFormat cell, bool preserveOLEDate )
    {
      if( cell == null )
        return string.Empty;//throw new ArgumentNullException( "cell" );

      //TRangeValueType type = m_worksheet.GetCellType( Row, Column, false );
      object result;

      switch( cell.TypeCode )
      {
        case TBIFFRecord.Blank:
          result = string.Empty;
          break;

        case TBIFFRecord.BoolErr:
          BoolErrRecord boolErr = ( BoolErrRecord )cell;
          int iValue = ( int )boolErr.BoolOrError;
          result = ( boolErr.IsErrorCode ) ?
            FormulaUtil.ErrorCodeToName[ iValue ] :
            ( object )( ( boolErr.BoolOrError != 0 ).ToString().ToUpper() );
          break;

        case TBIFFRecord.Formula:
          // TODO: maybe this can be optimized or moved inside GetFormula method.
          FormulaRecord formula = ( FormulaRecord )cell;
          Ptg[] tokens = formula.ParsedExpression;
          if( HasArrayFormula( tokens ) )
          {
            result = GetFormulaArray( formula ); //FormulaArray;
          }
          else
          {
            result = GetFormula( cell.Row, cell.Column, tokens, false, m_book.FormulaUtil, false );
          }
          break;

        case TBIFFRecord.Number:
        case TBIFFRecord.RK:
          double dNumber = ( ( IDoubleValue )cell ).DoubleValue;
          int iXFIndex = cell.ExtendedFormatIndex;
          int iFormatIndex = m_book.InnerExtFormats[ iXFIndex ].NumberFormatIndex;
          FormatImpl format = m_book.InnerFormats[ iFormatIndex ];

          if( format.FormatType == ExcelFormatType.DateTime )
          {
#if ( WINRT )
              DateTime dt = DateTimeExtension.FromOADate(dNumber);
              result = (preserveOLEDate == true) ?
                  (object)dt.ToOADate() :
                  (object)dt;

#else
              result = (preserveOLEDate == true) ? (object)DateTime.FromOADate(dNumber).ToOADate() :
                       (format.IsTimeFormat(dNumber)) ? (object)DateTime.FromOADate(dNumber).ToLongTimeString() :
                                                        (object)DateTime.FromOADate(dNumber);
#endif
          }
          else
          {
            result = dNumber;
          }
          break;

        case TBIFFRecord.LabelSST:
          LabelSSTRecord labelSST = ( LabelSSTRecord )cell;
          object sstItem = m_book.InnerSST.GetSSTContentByIndex( labelSST.SSTIndex );
          TextWithFormat text = sstItem as TextWithFormat;
          result = ( text != null ) ? text.Text : sstItem;
          break;

        case TBIFFRecord.String:
          StringRecord stringRecord = ( StringRecord )cell;
          result = stringRecord.Value;
          break;

        case TBIFFRecord.Label:
          LabelRecord label = ( LabelRecord )cell;
          result = label.Label;
          break;

        default:
          throw new ArgumentException( "Cannot recognize cell type." );
      }

      return result.ToString();

    }
    /// <summary>
    /// Updates dictionary with outlines after extended format removal.
    /// </summary>
    /// <param name="dictOutline">Dictionary to update.</param>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    private void UpdateOutlineAfterXFRemove( ICollection dictOutline, IDictionary dictFormats )
    {
      foreach( IOutline outline in dictOutline )
      {
        if( outline == null ) continue;

        int iOldXFIndex = outline.ExtendedFormatIndex;

        if( dictFormats.Contains( iOldXFIndex ) )
        {
          iOldXFIndex = ( int )dictFormats[ iOldXFIndex ];
          outline.ExtendedFormatIndex = ( ushort )iOldXFIndex;
        }
      }
    }
    /// <summary>
    /// Converts list of cell indexes into ranges array.
    /// </summary>
    /// <param name="arrIndexes">List of cell indexes.</param>
    /// <returns>Array with ranges.</returns>
    private IRange[] ConvertCellListIntoRange( List<long> arrIndexes )
    {
      if( arrIndexes == null || arrIndexes.Count == 0 ) return null;

      int iCount = arrIndexes.Count;
      IRange[] arrResult = new IRange[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        long lIndex = arrIndexes[ i ];
        int iRow = RangeImpl.GetRowFromCellIndex( lIndex );
        int iColumn = RangeImpl.GetColumnFromCellIndex( lIndex );
        arrResult[ i ] = this[ iRow, iColumn ];
      }

      return arrResult;
    }
    /// <summary>
    /// Finds value for number.
    /// </summary>
    /// <param name="record">Record that represents current cell.</param>
    /// <param name="findValue">Value for find.</param>
    /// <param name="bIsNumber">If true - find as number.</param>
    /// <param name="bIsFormulaValue">If true find as formula value.</param>
    /// <returns>Returns cell or null.</returns>
    private IRange FindValueForNumber( BiffRecordRaw record, double findValue
      , bool bIsNumber, bool bIsFormulaValue )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      double dResult = double.MinValue;
      ICellPositionFormat pos = ( ICellPositionFormat )record;

      if( bIsNumber )
      {
        if( record is NumberRecord )
        {
          NumberRecord numberRecord = ( NumberRecord )record;
          dResult = numberRecord.Value;
        }

        if( record is RKRecord )
        {
          RKRecord rkRecord = ( RKRecord )record;
          dResult = rkRecord.RKNumber;
        }
      }

      if( bIsFormulaValue && record is FormulaRecord )
      {
        FormulaRecord formulaRecord = ( FormulaRecord )record;
        dResult = formulaRecord.Value;
      }

      return ( dResult == findValue ) ? Range[ pos.Row + 1, pos.Column + 1 ] : null;
    }
    /// <summary>
    /// Finds bool or error.
    /// </summary>
    /// <param name="boolError">BoolError record that represents current cell.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="bIsError">If true - finds error; otherwise bool</param>
    /// <returns>Returns cell or null.</returns>
    private IRange FindValueForByteOrError( BoolErrRecord boolError
      , byte findValue, bool bIsError )
    {
      if( boolError == null )
        throw new ArgumentNullException( "boolError" );

      if( bIsError == boolError.IsErrorCode && boolError.BoolOrError == findValue )
        return Range[ boolError.Row + 1, boolError.Column + 1 ];

      return null;
    }
    /// <summary>
    /// Returns Range which represents specified cell.
    /// </summary>
    /// <param name="column">Column index of the cell.</param>
    /// <param name="row">Row index of the cell.</param>
    /// <returns>Range which represents specified cell.</returns>
    internal protected IRange InnerGetCell( int column, int row )
    {
      return InnerGetCell( column, row, GetXFIndex( row, column ) );
    }

    /// <summary>
    /// Returns Range which represents specified cell.
    /// </summary>
    /// <param name="column">Column index of the cell.</param>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="iXFIndex">Index to extended format for new range.</param>
    /// <returns>Range which represents specified cell.</returns>
    internal protected IRange InnerGetCell( int column, int row, int iXFIndex )
    {
      ParseData();

      IRange range = m_dicRecordsCells.GetRange( row, column );

      if( range == null )
      {
        BiffRecordRaw record = m_dicRecordsCells.GetCellRecord( row, column ) as BiffRecordRaw;

        if( record == null )
        {
          RangeImpl newRange = AppImplementation.CreateRange( this, column, row, column, row );

          if( newRange.ExtendedFormatIndex != iXFIndex )
          {
            newRange.ExtendedFormatIndex = ( ushort )iXFIndex;
          }

          m_dicRecordsCells.SetRange( row, column, newRange );
          range = newRange;
        }
        else
        {
          range = ConvertRecordToRange( record );
        }
      }

      return range;
    }
    /// <summary>
    /// Returns Range which represents specified cell.
    /// </summary>
    /// <param name="column">Column index of the cell.</param>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="iXFIndex">Index to extended format for new range.</param>
    /// <returns>Range which represents specified cell.</returns>
    internal protected IStyle InnerGetCellStyle(int column, int row, int iXFIndex,RangeImpl rangeImpl)
    {
        IStyle cellStyle= RangeImpl.CreateTempStyleWrapperWithoutRange(rangeImpl, iXFIndex);
        return cellStyle;
    }
    /// <summary>
    /// Converts biff record into range.
    /// </summary>
    /// <param name="record">Record to convert.</param>
    /// <returns>Created range.</returns>
    private IRange ConvertRecordToRange( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      ParseData();

      RangeImpl range = AppImplementation.CreateRange( this, record, false );
      long lCellIndex = range.CellIndex;
      m_dicRecordsCells.SetRange( lCellIndex, range );

      return range;
    }
    /// <summary>
    /// Updates first cell and last cell if necessary.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iColumnIndex">Column index.</param>
    protected void UpdateFirstLast( int iRowIndex, int iColumnIndex )
    {
      ParseData();

      m_iFirstColumn = ( m_iFirstColumn > iColumnIndex || m_iFirstColumn == DEF_MIN_COLUMN_INDEX )
        ? ( ushort )iColumnIndex
        : m_iFirstColumn;
      m_iLastColumn = ( m_iLastColumn < iColumnIndex || m_iLastColumn == DEF_MIN_COLUMN_INDEX )
        ? ( ushort )iColumnIndex
        : m_iLastColumn;

      m_iFirstRow = ( m_iFirstRow > iRowIndex || m_iFirstRow < 0 ) ? iRowIndex : m_iFirstRow;
      m_iLastRow = ( m_iLastRow < iRowIndex || m_iLastRow < 0 ) ? iRowIndex : m_iLastRow;
    }
    /// <summary>
    /// Sets Range which represents specified cell.
    /// </summary>
    /// <param name="column">Column index of the cell.</param>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="range">Range which represents specified cell.</param>
    internal protected void InnerSetCell( int column, int row, RangeImpl range )
    {
      if( !range.IsSingleCell )
        throw new ArgumentException( "Range must represent single cell" );

      ParseData();

      m_dicRecordsCells.SetRange( row, column, range );
    }
    /// <summary>
    /// Sets cell value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="record">Record to set into cell.</param>
    [CLSCompliant( false )]
    internal protected void InnerSetCell( long cellIndex, BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      ParseData();

      ICellPositionFormat cell = ( ICellPositionFormat )record;
      WorksheetHelper.AccessColumn( this, cell.Column + 1 );
      WorksheetHelper.AccessRow( this, cell.Row + 1 );

      m_dicRecordsCells.SetCellRecord( cellIndex, cell );
    }
    /// <summary>
    /// Sets cell value.
    /// </summary>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="record">Record to set into cell.</param>
    [CLSCompliant( false )]
    internal protected void InnerSetCell( int iColumn, int iRow, BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      ParseData();

      m_dicRecordsCells.SetCellRecord( iRow, iColumn, record as ICellPositionFormat );
    }
    /// <summary>
    /// Returns dimensions of the worksheet.
    /// </summary>
    /// <param name="left">Variable that receives index of the first used column.</param>
    /// <param name="top">Variable that receives index of the first used row.</param>
    /// <param name="right">Variable that receives index of the last used column.</param>
    /// <param name="bottom">Variable that receives index of the last used row.</param>
    internal protected void InnerGetDimensions( out int left, out int top, out int right, out int bottom )
    {
      ParseData();

      left = ( int )m_iFirstColumn;
      right = ( int )m_iLastColumn;
      top = m_iFirstRow;
      bottom = m_iLastRow;
    }
    /// <summary>
    /// Calculates dimensions of the specified column.
    /// </summary>
    /// <param name="column">Column dimension of which will be calculated.</param>
    /// <param name="top">
    /// Variable that will receive first used row in the specified column.
    /// </param>
    /// <param name="bottom">
    /// Variable that will receive last used row in the specified column.
    /// </param>
    internal protected void InnerGetColumnDimensions( int column, out int top, out int bottom )
    {
      ParseData();

      int iMax = -1, iMin = -1;

      for( int iRow = FirstRow, last = LastRow; iRow <= last; iRow++ )
      {
        long lCellIndex = RangeImpl.GetCellIndex( column, iRow );

        if( !m_dicRecordsCells.Contains( lCellIndex ) ) continue;

        if( iMax < iRow ) iMax = iRow;
        if( iMin == -1 ) iMin = iRow;
      }

      top = iMin;
      bottom = iMax;
    }
    /// <summary>
    /// Updates LabelSST indexes after SST record parsing.
    /// </summary>
    /// <param name="dictUpdatedIndexes">Dictionary with indexes to update, key - old index, value - new index.</param>
    internal void UpdateLabelSSTIndexes( Dictionary<int, int> dictUpdatedIndexes, IncreaseIndex method )
    {
      ParseData();

      m_dicRecordsCells.UpdateLabelSSTIndexes( dictUpdatedIndexes, method );
    }
    /// <summary>
    /// Insert into columns.
    /// </summary>
    /// <param name="iColumnIndex">Represents column index.</param>
    /// <param name="iColumnCount">Represents number of columns to be inserted.</param>
    /// <param name="insertOptions">Represents insert options.</param>
    private void InsertIntoDefaultColumns( int iColumnIndex, int iColumnCount,
      ExcelInsertOptions insertOptions )
    {
      ParseData();

      ColumnInfoRecord columnInfo = null;

      for( int i = m_book.MaxColumnCount; i > iColumnIndex + iColumnCount - 1; i-- )
      {
        int iOldIndex = i - iColumnCount;
        columnInfo = m_arrColumnInfo[ iOldIndex ];

        if( columnInfo != null )
        {
          columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( i - 1 );
        }

        m_arrColumnInfo[ i ] = columnInfo;
        m_arrColumnInfo[ iOldIndex ] = null;
      }

      columnInfo = null;
      if( insertOptions == ExcelInsertOptions.FormatAsBefore )
      {
        columnInfo = m_arrColumnInfo[ iColumnIndex - 1 ];
      }
      else if( insertOptions == ExcelInsertOptions.FormatAsAfter )
      {
        columnInfo = m_arrColumnInfo[ iColumnIndex + iColumnCount ];
      }

      if( columnInfo != null )
      {
        for( int i = iColumnIndex, len = iColumnIndex + iColumnCount; i < len; i++ )
        {
          columnInfo = ( ColumnInfoRecord )columnInfo.Clone();
          columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( i - 1 );
          m_arrColumnInfo[ i ] = columnInfo;
        }
      }

      //m_arrColumnInfo[ iColumnIndex ] = columnInfo;
    }
    /// <summary>
    /// Remove from column.
    /// </summary>
    /// <param name="iColumnIndex">Represents column index.</param>
    /// <param name="iColumnCount">Represents number of columns to remove.</param>
    /// <param name="insertOptions">Insert Options</param>
    private void RemoveFromDefaultColumns( int iColumnIndex, int iColumnCount,
      ExcelInsertOptions insertOptions )
    {
      ParseData();

      ColumnInfoRecord columnInfo = null;

      for( int i = iColumnIndex; i <= m_book.MaxColumnCount - iColumnCount; i++ )
      {
        int iOldIndex = i + iColumnCount;
        columnInfo = m_arrColumnInfo[ iOldIndex ];

        if( columnInfo != null )
        {
          columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( i - 1 );
        }

        m_arrColumnInfo[ i ] = columnInfo;
        //m_arrColumnInfo[ iOldIndex ] = null;
      }

      // Add last column.
      int iMaxCount = m_book.MaxColumnCount;
      columnInfo = ( ColumnInfoRecord )CloneUtils.CloneCloneable( m_arrColumnInfo[ iMaxCount - 1 ] );
      m_arrColumnInfo[ iMaxCount ] = columnInfo;

      if( columnInfo != null )
        columnInfo.FirstColumn = columnInfo.LastColumn = ( ushort )( iMaxCount - 1 );
    }
    /// <summary>
    /// Updates coordinates for used range, by removing empty rows and columns if necessary.
    /// </summary>
    /// <param name="firstRow">First row to start looking from.</param>
    /// <param name="firstColumn">First column to start looking from.</param>
    /// <param name="lastRow">Last row to finish looking at.</param>
    /// <param name="lastColumn">Last column to finish looking at.</param>
    private void GetRangeCoordinates( ref int firstRow, ref int firstColumn, ref int lastRow, ref int lastColumn )
    {
      if( !m_bUsedRangeIncludesFormatting )
      {
        for( ; firstRow <= lastRow; firstRow++ )
        {
          if( !IsRowBlankOnly( firstRow ) )
            break;
        }

        for( ; lastRow >= firstRow; lastRow-- )
        {
          if( !IsRowBlankOnly( lastRow ) )
            break;
        }

        for( ; firstColumn <= lastColumn; firstColumn++ )
        {
          if( !IsColumnBlankOnly( firstColumn ) )
            break;
        }

        for( ; lastColumn >= firstColumn; lastColumn-- )
        {
          if( !IsColumnBlankOnly( lastColumn ) )
            break;
        }
      }
    }
    /// <summary>
    /// Checks whether row is empty.
    /// </summary>
    /// <param name="rowIndex">Row index to check.</param>
    /// <returns>True if the whole row is empty.</returns>
    private bool IsRowBlankOnly( int rowIndex )
    {
      bool bResult = true;

      for( int i = m_iFirstColumn; i <= m_iLastColumn; i++ )
      {
        if( GetCellType( rowIndex, i, false ) != TRangeValueType.Blank )
        {
          bResult = false;
          break;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Checks whether column is empty.
    /// </summary>
    /// <param name="columnIndex">Column index to check.</param>
    /// <returns>True if the whole column is empty.</returns>
    private bool IsColumnBlankOnly( int columnIndex )
    {
      bool bResult = true;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        if( GetCellType( i, columnIndex, false ) != TRangeValueType.Blank )
        {
          bResult = false;
          break;
        }
      }

      return bResult;
    }
    /// <summary>
    /// Creates used range.
    /// </summary>
    /// <param name="firstRow">Represents first row of the range.</param>
    /// <param name="firstColumn">Represents first column of the range.</param>
    /// <param name="lastRow">Represents last row of the range.</param>
    /// <param name="lastColumn">Represents last column of the range.</param>
    private void CreateUsedRange( int firstRow, int firstColumn, int lastRow, int lastColumn )
    {
      if( m_rngUsed != null &&
        m_rngUsed.FirstColumn == firstColumn &&
        m_rngUsed.FirstRow == firstRow &&
        m_rngUsed.LastColumn == lastColumn &&
        m_rngUsed.LastRow == lastRow )
      {
        m_rngUsed.ResetCells();
      }
      else
      {
        if( m_rngUsed != null )
          m_rngUsed.Dispose();

        m_rngUsed = AppImplementation.CreateRange( this,
          firstColumn, firstRow,
          lastColumn, lastRow );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rangeImpl"></param>
    internal bool ClearExceptFirstCell( RangeImpl rangeImpl, bool isClearCells )
    {
      bool bFirstCellFound = false;
      bool bFirstCellValue = false;
      int iColumn = rangeImpl.Column - 1;
      int iLastColumn = rangeImpl.LastColumn - 1;

      for( int i = rangeImpl.Row - 1, lastRow = rangeImpl.LastRow; i < lastRow; i++ )
      {
        RowStorage rowStorage = m_dicRecordsCells.Table.Rows[ i ];

        if( rowStorage != null )
        {
          if( !bFirstCellFound )
          {
            int iStartColumn = rowStorage.FindFirstCell( iColumn, iLastColumn );

            if( iStartColumn <= iLastColumn )
            {
              bFirstCellFound = true;

             if( isClearCells )
                 rowStorage.Remove( iStartColumn + 1, iLastColumn, AppImplementation.RowStorageAllocationBlockSize );

             iStartColumn = Math.Max( rowStorage.FirstColumn, iStartColumn + 1 );

              if ( !bFirstCellValue )
              {
                  if ( rangeImpl[ rangeImpl.Row, rangeImpl.Column].Value != rangeImpl[i + 1, iStartColumn].Value)
                  {
                      rangeImpl[rangeImpl.Row, rangeImpl.Column].Value = rangeImpl[i + 1, iStartColumn].Value;
                      rangeImpl[rangeImpl.Row, rangeImpl.Column].CellStyle = rangeImpl[i + 1, iStartColumn].CellStyle;
                      rangeImpl[i + 1, iStartColumn].Value = null;
                      bFirstCellValue = true;
                      if (!isClearCells)
                          return true;
                  }
              }
            }
          }
          else if( isClearCells )
          {
            rowStorage.Remove( iColumn, iLastColumn, AppImplementation.RowStorageAllocationBlockSize );
          }
        }
      }

      return bFirstCellFound;
    }
    /// <summary>
    /// Prepares protection options before setting protection.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected override ExcelSheetProtection PrepareProtectionOptions( ExcelSheetProtection options )
    {
      return options &= ~ExcelSheetProtection.Content;
    }
    #endregion

    #region Recover from stream
    /// <summary>
    /// Method extracts biff records belonging to the worksheet.
    /// </summary>
    /// <param name="reader">BiffReader that contains worksheet's records.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [CLSCompliant( false )]
    protected internal void Parse( BiffReader reader, IDecryptor decryptor )
    {
      Parse( reader, ExcelParseOptions.Default, false, null, decryptor );
    }

    /// <summary>
    /// Prepares variables to worksheet parsing.
    /// </summary>
    protected override void PrepareVariables( ExcelParseOptions options, bool bSkipParsing )
    {
      base.PrepareVariables( options, bSkipParsing );

      if( m_arrAutoFilter != null ) m_arrAutoFilter.Clear();
      if( m_arrDConRecords != null ) m_arrDConRecords.Clear();

      m_iDValPos = -1;
      m_iCondFmtPos = -1;
      m_iPivotStartIndex = -1;
      m_iHyperlinksStartIndex = -1;
    }

    /// <summary>
    /// Parses single record.
    /// </summary>
    /// <param name="raw">Record to parse.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles information.</param>
    /// <param name="hashNewXFormatIndexes">Dictionary with new extended format index.</param>
    [CLSCompliant( false )]
    protected override void ParseRecord( BiffRecordRaw raw, bool bIgnoreStyles,
      Dictionary<int, int> hashNewXFormatIndexes )
    {
      if( m_book.HasDuplicatedNames && raw.TypeCode == TBIFFRecord.Formula )
      {
        FormulaRecord formula = ( FormulaRecord )raw;
        UpdateDuplicatedNameIndexes( formula );
      }

      if( !IsSkipParsing )
      {
        // NOTE: Here we must add code which set worksheet properties
        // or configure worksheet behaviour.
        //if( s_hashAutofilterRecord.Contains( raw.RecordCode ) )
        if( UtilityMethods.IndexOf( s_arrAutofilterRecord, raw.TypeCode ) >= 0 )
        {
          AutoFilterRecords.Add( raw );
        }

        ICellPositionFormat cell = raw as ICellPositionFormat;

        if( cell != null && bIgnoreStyles )
        {
          cell.ExtendedFormatIndex = ( ushort )GetNewXFormatIndex( cell.ExtendedFormatIndex,
            hashNewXFormatIndexes );
        }

        switch( raw.TypeCode )
        {
          case TBIFFRecord.Index:
            m_index = ( IndexRecord )raw;
            break;

          case TBIFFRecord.ColumnInfo:
            ParseColumnInfo( ( ColumnInfoRecord )raw, bIgnoreStyles );
            break;

          case TBIFFRecord.Row:
            ParseRowRecord( ( RowRecord )raw, bIgnoreStyles );
            break;

          case TBIFFRecord.DefaultColWidth:
            //m_dStandardColWidth = ( DefaultColWidthRecord )raw ).Width// ) * 256;
            DefaultColWidthRecord defaultWidth = ( DefaultColWidthRecord )raw;
            if( defaultWidth.Width != 8 )
            {
              m_dStandardColWidth = m_book.WidthToFileWidth( defaultWidth.Width );
            }
            break;

          case TBIFFRecord.MergeCells:
            MergeCells.AddMerge( ( MergeCellsRecord )raw );
            break;

          case TBIFFRecord.Pane:
            m_pane = ( PaneRecord )raw;
            break;

          case TBIFFRecord.Selection:
            m_arrSelections.Add( ( SelectionRecord )raw );
            break;

          case TBIFFRecord.CondFMT:
            if( !KeepRecord )
            {
              KeepRecord = true;
              m_arrRecords.Add( raw );
            }

            if( m_iCondFmtPos < 0 ) m_iCondFmtPos = m_arrRecords.Count - 1;
            break;

          case TBIFFRecord.CondFMT12:
            if (!KeepRecord)
            {
                KeepRecord = true;
                m_arrRecords.Add(raw);
            }

            if (m_iCondFmtPos < 0) m_iCondFmtPos = m_arrRecords.Count - 1;
            break;

          case TBIFFRecord.DVal:
            if( !KeepRecord )
            {
              KeepRecord = true;
              m_arrRecords.Add( raw );
            }

            DValRecord dval = ( DValRecord )raw;
            dval.IsDataCached = false;

            if( m_iDValPos < 0 )
              m_iDValPos = m_arrRecords.Count - 1;
            break;

          case PivotTableImpl.DEF_FIRSTRECORD_CODE:
            if( !KeepRecord )
            {
              KeepRecord = true;
              m_arrRecords.Add( raw );
            }

            if( m_iPivotStartIndex < 0 ) m_iPivotStartIndex = m_arrRecords.Count - 1;
            break;

          case TBIFFRecord.HLink:
            if( !KeepRecord )
            {
              KeepRecord = true;
              m_arrRecords.Add( raw );
            }

            if( m_iHyperlinksStartIndex < 0 ) m_iHyperlinksStartIndex = m_arrRecords.Count - 1;
            break;

          case TBIFFRecord.Sort:
            SortRecords.Add( raw );
            break;

          case TBIFFRecord.Note:
            AddNote( raw as NoteRecord );
            break;

          //          case TBIFFRecord.Array:
          //            ArrayRecord array = ( ArrayRecord )raw;
          //            m_dicRecordsCells.SetArrayFormula( array );
          //            //AddArrayFormula( ( ISharedFormula )raw );
          //            break;

          case TBIFFRecord.DCON:
            DConRecords.Add( raw );
            break;

          case TBIFFRecord.RangeProtection:
            ParseErrorIndicators( ( RangeProtectionRecord )raw );
            break;

          case TBIFFRecord.CustomProperty:
            if( !KeepRecord )
            {
              KeepRecord = true;
              m_arrRecords.Add( raw );
            }
            if( m_iCustomPropertyStartIndex < 0 ) m_iCustomPropertyStartIndex = m_arrRecords.Count - 1;
            break;

          case TBIFFRecord.Qsi:
          case TBIFFRecord.Qsif:
          case TBIFFRecord.QsiSXTag:
          case TBIFFRecord.Feature12:
          case TBIFFRecord.ExternalSourceInfo:
          case TBIFFRecord.PivotString:
          case TBIFFRecord.DBQueryExt:
          case TBIFFRecord.Qsir:
          case TBIFFRecord.OleDbConn:
          case TBIFFRecord.ExtString:
          case TBIFFRecord.PivotViewAdditionalInfo:
          case TBIFFRecord.TextQuery:
            PreserveExternalConnection.Add(raw);
            break;

          case ( TBIFFRecord )2167:
          case ( TBIFFRecord )2161:
          case ( TBIFFRecord )2162:
            if( m_tableRecords == null )
              m_tableRecords = new List<BiffRecordRaw>();

            m_tableRecords.Add( raw );
            break;
        }
      }
    }
    /// <summary>
    /// Parses error indicators.
    /// </summary>
    /// <param name="record">Represents </param>
    private void ParseErrorIndicators( RangeProtectionRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      ErrorIndicatorImpl indicator = record.ErrorIndicator;

      if( indicator == null )
        return;

      if( m_errorIndicators == null )
        m_errorIndicators = new ErrorIndicatorsCollection( Application, this );

      m_errorIndicators.Add( record.ErrorIndicator );
    }
    /// <summary>
    /// Updates duplicated name indexes.
    /// </summary>
    /// <param name="formula">Represents formula.</param>
    private void UpdateDuplicatedNameIndexes( FormulaRecord formula )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      Ptg[] arrPtgs = formula.ParsedExpression;

      for( int i = 0, len = arrPtgs.Length; i < len; i++ )
      {
        Ptg token = arrPtgs[ i ];

        if( FormulaUtil.IndexOf( FormulaUtil.NameXCodes, token.TokenCode ) != -1 )
        {
          NameXPtg name = ( NameXPtg )token;
          int iRefIndex = name.RefIndex;
          int iNameIndex = name.NameIndex;

          if( !m_book.IsLocalReference( iRefIndex ) )
          {
            ExternWorkbookImpl externBook = m_book.ExternWorkbooks[ iRefIndex ];
            name.NameIndex = ( ushort )( externBook.GetNewIndex( iNameIndex - 1 ) + 1 );
          }
        }
      }
    }
    /// <summary>
    /// Returns new index of the extended format in ignore styles mode.
    /// </summary>
    /// <param name="iXFIndex">Old extended format index.</param>
    /// <param name="hashNewXFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <returns>New index of the extended format.</returns>
    private int GetNewXFormatIndex( int iXFIndex, Dictionary<int, int> hashNewXFormatIndexes )
    {
      if( hashNewXFormatIndexes == null )
        throw new ArgumentNullException( "hashNewXFormatIndexes" );

      ExtendedFormatRecord record = ( ExtendedFormatRecord )m_book.InnerExtFormatRecords[ iXFIndex ];
      int iNumberFormat = record.FormatIndex;
      return hashNewXFormatIndexes[ iNumberFormat ];
    }
    /// <summary>
    /// Method opens excel file using separator.
    /// </summary>
    /// <param name="streamToRead">Stream to reading.</param>    
    /// <param name="separator">Current separator.</param>
    /// <param name="row">First row to write.</param>
    /// <param name="column">First column to write.</param>
    /// <param name="isValid">boolen value for the valid document</param>
    public void Parse( TextReader streamToRead, string separator, int row, int column,bool isValid )
    {
      if( streamToRead == null )
        throw new ArgumentNullException( "streamToRead" );

      if( separator == null )
        throw new ArgumentNullException( "separator" );

      int iSeparatorLen = separator.Length;

      if( iSeparatorLen == 0 )
        throw new ArgumentException( "separator" );

      int iCurRow = row;
      StringBuilder builder = new StringBuilder();
      int iCurColumn = column;
      this.CustomHeight = false;

      while( streamToRead.Peek() >= 0 )
      {
       
        string strRowString = ReadCellValue( streamToRead, separator, builder,isValid );
        bool bNewRow = strRowString.EndsWith( "\n" );

        if( bNewRow )
          strRowString = strRowString.Remove( strRowString.Length - 1 );

        if( strRowString.Length > 0 )
          ParseRange( Range[ iCurRow, iCurColumn ], strRowString, separator, 0 );

        if( bNewRow )
        {
          iCurRow++;
          iCurColumn = column;
        }
        else
        {
          iCurColumn++;
        }
      }
    }
    /// <summary>
    /// Read single cell value plus ending separator or new line character if present.
    /// </summary>
    /// <param name="reader">Read to get data from.</param>
    /// <param name="separator">Separator between cell values.</param>
    /// <param name="builder">Builder to store temporary results.</param>
    /// <param name="isValid">Boolean value for the Valid Document.</param>
    /// <returns>Extracted cell value.</returns>
    private static string ReadCellValue( TextReader reader, string separator, StringBuilder builder,bool isValid )
    {
      builder.Length = 0;

      while( true )
      {
        int iCurrentChar = reader.Read();

        if( iCurrentChar < 0 )
          break;

        char currentChar = ( char )iCurrentChar;

        if( currentChar == '"' )
        {
          builder.Append( currentChar );
          ReadToChar( reader, currentChar, builder,separator,isValid );
        }
        else if (currentChar == CarriageReturn)
        {
          // Skip it for easier manipulations.
        }
       else if( currentChar == NewLine )
        {
          builder.Append( currentChar );
          break;
        }
        else
        {
          builder.Append( currentChar );

          if( EndsWith( builder, separator ) )
            break;
        }
      }

      return builder.ToString();
    }
    /// <summary>
    /// Checks whether data inside specified string builder ends with specified string.
    /// </summary>
    /// <param name="builder">Builder to check.</param>
    /// <param name="separator">Separator to locate.</param>
    /// <returns>True if it ends with specified value.</returns>
    private static bool EndsWith( StringBuilder builder, string separator )
    {
      if( String.IsNullOrEmpty( separator ) )
        throw new ArgumentException( "separator" );

      int iBuilderLen = builder.Length;
      int iSeparatorLen = separator.Length;
      bool result = false;

      if( iBuilderLen >= iSeparatorLen )
      {
        result = true;

        for( int i = iBuilderLen - 1, j = iSeparatorLen - 1; j >= 0; i--, j-- )
        {
          if( builder[ i ] != separator[ j ] )
          {
            result = false;
            break;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Read data from reader until it find specified character.
    /// </summary>
    /// <param name="reader">Reader to read data from.</param>
    /// <param name="endChar">Character to locate.</param>
    /// <param name="builder">Builder to put extracted data into.</param>
    /// <param name="isValid">BoolenValue for the ValidDocument.</param>
    private static void ReadToChar( TextReader reader, char endChar, StringBuilder builder,string separator,bool isValid )
    {
        if (isValid)
        {
            ReadToChar(reader, endChar, builder);
        }
        else
        {
            RemoveJunkChar(reader, endChar, builder, separator);
        }
    }

    private static void RemoveJunkChar(TextReader reader, char endChar, StringBuilder builder, string separator)
    {
        int iCurrentChar;
        char ch;
        bool isValue = true;

        do
        {
            iCurrentChar = reader.Read();
            ch = (char)iCurrentChar;

            if (ch == endChar)
            {
                char nextChar = (char)reader.Peek();
                if (nextChar == Convert.ToChar(separator) || nextChar == '\r' || nextChar == '\n')
                {
                    isValue = false;
                    builder.Append(ch);
                }
            }
            else
            {
                builder.Append(ch);
            }
        }

        while (isValue && iCurrentChar > 0);
    }

    private static void ReadToChar(TextReader reader, char endChar, StringBuilder builder)
    {
        int iCurrentChar;
        char ch;

        do
        {
            iCurrentChar = reader.Read();
            ch = (char)iCurrentChar;
            builder.Append(ch);
        }
        while (ch != endChar && iCurrentChar > 0 && ch != NewLine && ch != CarriageReturn);
    }
    /// <summary>
    /// Calculates number of specified character in the specified string.    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="ch">Character to count.</param>
    /// <returns>Number of found characters.</returns>
    private static int CharCount( string value, char ch )
    {
      int iCount = 0;

      for( int i = value.Length - 1; i >= 0; i-- )
      {
        if( value[ i ] == ch )
          iCount++;
      }

      return iCount;
    }
    /// <summary>
    /// Parses worksheet's data.
    /// </summary>
    protected internal override void ParseData( Dictionary<int, int> dictUpdatedSSTIndexes )
    {
#if MEASURE_PERFORMANCE
      DateTime start = DateTime.Now;
#endif

      if ((IsParsed || IsParsing || m_book.Saving) && (!ParseDataOnDemand)) return;
      
      IsParsing = true;

      if( m_dataHolder == null )
      {
        if( !IsSkipParsing )
        {
            bool isBookLoading = m_book.Loading;
            
            if (ParseOnDemand)
            {
                if (!isBookLoading)
                    m_book.Loading = true;

                Stream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                foreach (BiffRecordRaw raw in m_arrRecords)
                {
                    int code = raw.RecordCode;
                    int count = 0;
                    byte[] data = raw.Data;
                    if (data != null)
                        count = data.Length;
                    writer.Write((short)code);
                    writer.Write((short)count);
                    if (data != null)
                        writer.Write(data, 0, count);
                }
                writer.Flush();
                if (stream.Length > 0)
                {
                    m_arrRecords.Clear();
                    ParseOnDemand = false;
                    stream.Position = 0;
                    Parser.BiffReader reader = new Parser.BiffReader(stream);
                    Parse(reader, ExcelParseOptions.Default, false, null, null);
                    if (m_bParseMSODrawings)
                        Parse();

                    m_book.ParseWorksheetsOnDemand();
                }
            }

          int iPos = ExtractCalculationOptions();
          ReplaceSharedFormula();
          ExtractPageSetup( iPos );
          ExtractPivotTables( m_iPivotStartIndex );
          ExtractHyperLinks( m_iHyperlinksStartIndex );

          if( m_iCondFmtPos >= 0 ) ExtractConditionalFormats( m_iCondFmtPos );
          if( m_iDValPos >= 0 ) ExtractDataValidation( m_iDValPos );
          if( m_iCustomPropertyStartIndex >= 0 ) ExtractCustomProperties( m_iCustomPropertyStartIndex );

          m_book.Loading = isBookLoading;
        }
      }
      else
      {
        AttachEvents();
        m_dataHolder.ParseWorksheetData( this, dictUpdatedSSTIndexes, ParseDataOnDemand );
      }

      if (!IsParsed)
          IsSaved = true;
      IsParsed = true;
      IsParsing = false;

#if MEASURE_PERFORMANCE
      TimeSpan parseDataTime = DateTime.Now - start;
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, parseDataTime, "Time to extract ranges" );
      Console.WriteLine( "ParseData time: {0}, worksheet: {1}", parseDataTime, Name );
#endif
    }
    /// <summary>
    /// Replaces all shared formula with ordinary formula.
    /// </summary>
    private void ReplaceSharedFormula()
    {
      m_dicRecordsCells.ReplaceSharedFormula();
    }
    /// <summary>
    /// Parses ColumnInfo record.
    /// </summary>
    /// <param name="columnInfo">Record to parse.</param>
    /// <param name="bIgnoreStyles">Indicates whether we should ignore styles settings.</param>
    internal void ParseColumnInfo( ColumnInfoRecord columnInfo, bool bIgnoreStyles )
    {
      if( columnInfo == null )
        throw new ArgumentNullException( "columnInfo" );

      if( bIgnoreStyles )
      {
        columnInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
      }

      columnInfo.ColumnWidth = ( ushort )EvaluateRealColumnWidth( columnInfo.ColumnWidth );

      ColumnInfoRecord newColInfo;
      short sXFIndex = ( short )columnInfo.ExtendedFormatIndex;

      if( columnInfo.FirstColumn != columnInfo.LastColumn )
      {
        if (columnInfo.LastColumn == m_book.MaxColumnCount)
            m_rawColRecord = (columnInfo.Clone() as ColumnInfoRecord);
        for( int i = columnInfo.FirstColumn; i <= columnInfo.LastColumn; i++ )
        {
          int iColumnIndex = i + 1;
          newColInfo = ( ColumnInfoRecord )columnInfo.Clone();
          newColInfo.FirstColumn = ( ushort )i;
          newColInfo.LastColumn = ( ushort )i;
          m_arrColumnInfo[ iColumnIndex ] = newColInfo;
        }
      }
      else
      {
        m_arrColumnInfo[ columnInfo.FirstColumn + 1 ] = columnInfo;
      }
    }
    /// <summary>
    /// Parses row record.
    /// </summary>
    /// <param name="row">Record to parse.</param>
    /// <param name="bIgnoreStyles">Indicates whether we should ignore styles settings.</param>
    internal void ParseRowRecord( RowRecord row, bool bIgnoreStyles )
    {
      if( row == null )
        throw new ArgumentNullException( "row" );

      if( bIgnoreStyles )
      {
        row.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
        row.IsFormatted = false;
      }

      RowStorage rowStorage = WorksheetHelper.GetOrCreateRow( this, row.RowNumber, true );
      rowStorage.UpdateRowInfo( row, AppImplementation.UseFastRecordParsing );

      int iRowIndex = row.RowNumber + 1;

      if( iRowIndex < FirstRow ) FirstRow = iRowIndex;
      if( iRowIndex > LastRow ) LastRow = iRowIndex;

      if( FirstColumn == DEF_MIN_COLUMN_INDEX ) FirstColumn = 0;
      if( LastColumn == DEF_MIN_COLUMN_INDEX ) LastColumn = 1;

      //WorksheetHelper.AccessColumn( this, row.LastColumn + 1 );
      //WorksheetHelper.AccessColumn( this, row.FirstColumn + 1 );

      // Fixing styles.
      int xfIndex = rowStorage.ExtendedFormatIndex;

      if( xfIndex > m_book.InnerExtFormats.Count )
      {
        rowStorage.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
      }
      else
      {
        ExtendedFormatImpl format = m_book.InnerExtFormats[ xfIndex ];

        if( !format.HasParent )
        {
          format = ( ExtendedFormatImpl )format.Clone();
          format.ParentIndex = xfIndex;
          format.Record.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
          format = m_book.InnerExtFormats.Add( format );
          rowStorage.ExtendedFormatIndex = ( ushort )format.Index;
        }
      }
    }
    /// <summary>
    /// Extracts hyperlinks from internal array of biff records.
    /// </summary>
    /// <param name="iLinkIndex">
    /// Index to the first hyperlink in the array of biff records,
    /// less then zero if there are no hyperlinks.
    /// </param>
    protected void ExtractHyperLinks( int iLinkIndex )
    {
      if( iLinkIndex < 0 ) return;

      InnerHyperLinks.Clear();
      InnerHyperLinks.Parse( m_arrRecords, iLinkIndex );
    }
    /// <summary>
    /// Extracts calculation options.
    /// </summary>
    /// <returns>Position in the records array after extraction.</returns>
    protected int ExtractCalculationOptions()
    {
      for( int i = 0, len = m_arrRecords.Count; i < len; i++ )
      {
        BiffRecordRaw raw = ( BiffRecordRaw )m_arrRecords[ i ];
        if( Array.IndexOf( CalculationOptionsImpl.DEF_CORRECT_CODES, raw.TypeCode ) != -1 )
        {
          //i = m_calculation.Parse( m_arrRecords, i );
          i = m_book.InnerCalculation.Parse( m_arrRecords, i );
          return i;
        }
      }

      return 0;
    }
    /// <summary>
    /// Extracts page setup from biff records array
    /// </summary>
    /// <param name="iStartIndex">Start index of the first pagesetup record.</param>
    protected void ExtractPageSetup( int iStartIndex )
    {
      if( iStartIndex < 0 )
        throw new ArgumentOutOfRangeException( "iStartIndex" );

      int i = iStartIndex;
      for( int len = m_arrRecords.Count; i < len; i++ )
      {
        BiffRecordRaw record = ( BiffRecordRaw )m_arrRecords[ i ];
        TBIFFRecord code = record.TypeCode;
        if( code == TBIFFRecord.PrintHeaders || code == TBIFFRecord.DefaultRowHeight )
        {
          m_pageSetup = new PageSetupImpl( Application, this, m_arrRecords, i );
          break;
        }
      }
    }
    /// <summary>
    /// Extracts conditional formats from biff records array
    /// </summary>
    /// <param name="iCondFmtPos">Position of the first conditional format</param>
    protected void ExtractConditionalFormats( int iCondFmtPos )
    {
      if( iCondFmtPos < 0 )
        throw new ArgumentOutOfRangeException( "iCondFmtPos" );

      bool bIsCF = true;
      int condFMTIndex = 0;

      CondFMTRecord curFormat = null;
      List<CFRecord> listCF = new List<CFRecord>();

      CondFmt12Record cur12Format = null;
      List<CF12Record> listCF12 = new List<CF12Record>();
      List<CFExRecord> listCFEx = new List<CFExRecord>();

      while( bIsCF )
      {
        // Use 'as' to increase performance.
        BiffRecordRaw record = m_arrRecords[ iCondFmtPos ] as BiffRecordRaw;

        switch( record.TypeCode )
        {
          case TBIFFRecord.CondFMT:
            if( curFormat != null )
            {
              CreateFormatsCollection(curFormat, listCF, listCFEx, false);
              curFormat = null;
              listCF.Clear();
            }
            if (cur12Format != null)
            {
                CreateCF12RecordCollection(cur12Format, listCF12);
                listCF12.Clear();
                cur12Format = null;
            }

            curFormat = ( CondFMTRecord )record;
            condFMTIndex++;
            if (curFormat.Index == 0)
            {
                curFormat.Index = (ushort)condFMTIndex;
            }

            if (!m_dictCondFMT.ContainsKey(curFormat.Index))
                m_dictCondFMT.Add(curFormat.Index, curFormat);
            else
            {
                condFMTIndex++;
                curFormat.Index = (ushort)condFMTIndex;
                m_dictCondFMT.Add(curFormat.Index, curFormat);
            }
            break;

          case TBIFFRecord.CF:
            listCF.Add( ( CFRecord )record );
            break;

          case TBIFFRecord.CondFMT12:
            if (cur12Format != null)
            {
                CreateCF12RecordCollection(cur12Format, listCF12);
                cur12Format = null;
                listCF12.Clear();
            }
            if (curFormat != null)
            {
                CreateFormatsCollection(curFormat, listCF, listCFEx, false);
                listCF.Clear();
                curFormat = null;
            }

            cur12Format = (CondFmt12Record)record;
            break;

          case TBIFFRecord.CF12:
            if (m_dictCFExRecords.Count > 0)
            {
                CFExRecord cfexRecord = m_dictCFExRecords[m_dictCFExRecords.Count - 1];
                if (cfexRecord.IsCF12Extends == 1)
                {
                    cfexRecord.CF12RecordIfExtends = (CF12Record)record;
                }
                else
                {
                    listCF12.Add((CF12Record)record);
                }
            }
            else
            {
                listCF12.Add((CF12Record)record);
            }
            break;

          case TBIFFRecord.CFEx:
            if (curFormat != null)
            {
                CreateFormatsCollection(curFormat, listCF, listCFEx, false);
                listCF.Clear();
                curFormat = null;
            }
            if (cur12Format != null)
            {
                CreateCF12RecordCollection(cur12Format, listCF12);
                listCF12.Clear();
                cur12Format = null;
            }

            CFExRecord cfEx = (CFExRecord)record;
            for (int i = 0; i < m_arrConditionalFormats.Count; i++)
            {
                if(m_arrConditionalFormats[i].CondFMTRecord!=null)
                {
                if (m_arrConditionalFormats[i].CondFMTRecord.Index == cfEx.CondFmtIndex)
                {
                    m_dictCFExRecords.Add(m_dictCFExRecords.Count, cfEx);
                }
                }
            }            
            break;

          default:
            bIsCF = false;
            break;
        }

        iCondFmtPos++;
      }

      if( curFormat != null )
      {
        CreateFormatsCollection(curFormat, listCF, listCFEx, false);
        listCF.Clear();
      }
      if (cur12Format != null)
      {
          CreateCF12RecordCollection(cur12Format, listCF12);
          listCF12.Clear();
      }           
      m_dictCondFMT.Clear();
      m_dictCFExRecords.Clear();
    }

    /// <summary>
    /// Extracts data validation data from internal records array.
    /// </summary>
    /// <param name="iDValPos">Position of the first data validation record.</param>
    protected void ExtractDataValidation( int iDValPos )
    {
      if( iDValPos < 0 )
        throw new ArgumentOutOfRangeException( "iDValPos" );

      m_dataValidation = new DataValidationTable( Application, this, m_arrRecords, ref iDValPos );
    }
    /// <summary>
    /// Extracts custom properties from the records array.
    /// </summary>
    /// <param name="iCustomPropertyPos">Position of the first custom property record.</param>
    protected void ExtractCustomProperties( int iCustomPropertyPos )
    {
      if( iCustomPropertyPos < 0 )
        throw new ArgumentOutOfRangeException( "iCustomPropertyPos" );

      m_arrCustomProperties = new WorksheetCustomProperties( m_arrRecords, iCustomPropertyPos );
    }
    /// <summary>
    /// Creates collection of conditional formats.
    /// </summary>
    /// <param name="format">Record that describes formats collection.</param>
    /// <param name="lstConditions">
    /// Conditional formats that will be inserted into the collection.
    /// </param>
    private void CreateFormatsCollection(CondFMTRecord format, IList lstConditions, IList CFExRecords, bool isFutureRecord)
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( lstConditions == null && CFExRecords == null)
        throw new ArgumentNullException( "Conditions" );

      ConditionalFormats condFormat = AppImplementation.CreateConditionalFormats(this, format, lstConditions, CFExRecords);
      condFormat.IsFutureRecord = isFutureRecord;
      m_arrConditionalFormats.Add( condFormat );
    }
    /// <summary>
    /// Creates collection of conditional formats.
    /// </summary>
    /// <param name="format">Record that describes formats collection.</param>
    /// <param name="conditions">
    /// Conditional formats that will be inserted into the collection.
    /// </param>
    private void CreateCF12RecordCollection(CondFmt12Record format, IList conditions)
    {
        if (format == null)
            throw new ArgumentNullException("format");

        if (conditions == null)
            throw new ArgumentNullException("conditions");

        ConditionalFormats condFormat = AppImplementation.CreateConditionalFormats(this, format, conditions);
        m_arrConditionalFormats.Add(condFormat);
    }
    /// <summary>
    /// Returns width from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumn">One-based index of the column.</param>
    /// <returns>Width of the specified column.</returns>
    public double InnerGetColumnWidth( int iColumn )
    {
      if( iColumn < 1 )
        throw new ArgumentOutOfRangeException( "iColumn can't be less then 1" );

      ParseData();

      ColumnInfoRecord column = ( ColumnInfoRecord )m_arrColumnInfo[ iColumn ];

      double dResult;

      if( column == null )
      {
        dResult = StandardWidth;
      }
      else
      {
        dResult = ( column.IsHidden ) ? 0 : column.ColumnWidth / 256.0;
      }

      return dResult;
    }
    /// <summary>
    /// Converts column width into pixels.
    /// </summary>
    /// <param name="widthInChars">Column width to convert.</param>
    /// <returns>Column width in pixels.</returns>
    public int ColumnWidthToPixels( double widthInChars )
    {
      double dFileWidth = m_book.WidthToFileWidth( widthInChars );
      return ( int )m_book.FileWidthToPixels( dFileWidth );
      //int result = 0;

      //int firstSize = m_book.FirstCharSize;
      //int secondSize = m_book.SecondCharSize;

      //if( firstSize < 0 )
      //{
      //  FontImpl normalFont = ( m_book.Styles[ "Normal" ].Font as FontWrapper ).Wrapped;
      //  m_book.FirstCharSize = firstSize = ( int )Math.Round( normalFont.MeasureString( DEF_STANDARD_CHAR.ToString() ).Width );
      //  m_book.SecondCharSize = secondSize = ( int )Math.Round( normalFont.MeasureCharacter( DEF_STANDARD_CHAR ).Width );
      //}

      //if( widthInChars > 1 )
      //{
      //  result = ( int )Math.Round( ( widthInChars - 1 ) * secondSize + firstSize );
      //}
      //else
      //{
      //  result = ( int )Math.Round( widthInChars * firstSize );
      //}

      //return result;
    }
    /// <summary>
    /// Converts pixels count into column width value.
    /// </summary>
    /// <param name="pixels">Column width in pixels.</param>
    /// <returns>Column width.</returns>
    public double PixelsToColumnWidth( int pixels )
    {
      return m_book.PixelsToWidth( pixels );
      ////      // NOTE: for some reason we setting width that is 1 pixel larger
      ////      // maybe ms excel includes border into column width.
      ////      pixels--;
      //      FontImpl normalFont = ( m_book.Styles[ "Normal" ].Font as FontWrapper ).Wrapped;
      //      double iFirstSize = normalFont.MeasureString( DEF_STANDARD_CHAR.ToString() ).Width;
      //      double iSecondSize = normalFont.MeasureCharacter( DEF_STANDARD_CHAR ).Width;

      //      if( pixels > iFirstSize )
      //      {
      //        return ( pixels - iFirstSize ) / ( double )iSecondSize + 1;
      //      }
      //      else
      //      {
      //        return pixels / ( double )iFirstSize;
      //      }
    }
    /// <summary>
    /// Returns width displayed by Excel.
    /// </summary>
    /// <param name="fileWidth">Width written in file.</param>
    /// <returns>Width displayed by Excel.</returns>
    internal int EvaluateRealColumnWidth( int fileWidth )
    {
      double dPixels = m_book.FileWidthToPixels( fileWidth / 256.0 );
      return ( int )( m_book.PixelsToWidth( dPixels ) * 256 );
      //int firstSize = m_book.FirstCharSize;
      //int secondSize = m_book.SecondCharSize;

      //if( firstSize < 0 )
      //{
      //  FontImpl normalFont = ( m_book.Styles[ "Normal" ].Font as FontWrapper ).Wrapped;
      //  m_book.FirstCharSize = firstSize = ( int )Math.Round( normalFont.MeasureString( DEF_STANDARD_CHAR.ToString() ).Width );
      //  m_book.SecondCharSize = secondSize = ( int )Math.Round( normalFont.MeasureCharacter( DEF_STANDARD_CHAR ).Width );
      //}

      //int firstFileSize = firstSize * 256 / secondSize;

      //if( fileWidth > firstFileSize )
      //{
      //  return fileWidth - ( ( firstSize - secondSize ) * 256 )/ secondSize;
      //}
      //else
      //{
      //  return fileWidth * 256 / firstFileSize;
      //}
    }
    /// <summary>
    /// Converts width displayed by Excel to width that should be written into file.
    /// </summary>
    /// <param name="realWidth">Width displayed by Excel.</param>
    /// <returns>width written into file.</returns>
    internal int EvaluateFileColumnWidth( int realWidth )
    {
      return ( int )( m_book.WidthToFileWidth( realWidth / 256.0 ) * 256 );
      //int firstSize = m_book.FirstCharSize;
      //int secondSize = m_book.SecondCharSize;

      //if( firstSize < 0 )
      //{
      //  FontImpl normalFont = ( m_book.Styles[ "Normal" ].Font as FontWrapper ).Wrapped;
      //  m_book.FirstCharSize = firstSize = ( int )Math.Round( normalFont.MeasureString( DEF_STANDARD_CHAR.ToString() ).Width );
      //  m_book.SecondCharSize = secondSize = ( int )Math.Round( normalFont.MeasureCharacter( DEF_STANDARD_CHAR ).Width );
      //}

      //// Size that corresponds column width 1.0.
      //if( realWidth < 256 )
      //{
      //  int iSize1 = 256 + ( ( firstSize - secondSize ) * 256 ) / secondSize;
      //  double dResult = realWidth * iSize1 / ( double )256;
      //  return ( int )Math.Round( dResult );
      //}

      //return realWidth + ( ( firstSize - secondSize ) * 256 ) / secondSize;
    }
    //    /// <summary>
    //    /// Adds new shared formula to the dictionary of shared formulas.
    //    /// </summary>
    //    /// <param name="dict">Dictionary to add into.</param>
    //    /// <param name="shared">New record to add.</param>
    //    [ CLSCompliant( false ) ]
    //    public static void  AddSharedFormula( IDictionary dict, ISharedFormula shared )
    //    {
    //      if( dict == null )
    //        throw new ArgumentNullException( "dict" );
    //
    //      if( shared == null )
    //        throw new ArgumentNullException( "shared" );
    //
    //      for( int iRow = shared.FirstRow + 1, last = shared.LastRow + 1; iRow <= last; iRow++ )
    //      {
    //        int index = RangeImpl.GetCellIndex( shared.FirstCol + 1, iRow );
    //                
    //        for( int j = index, iCurJ = shared.FirstCol, iLastJ = shared.LastCol;
    //          iCurJ <= iLastJ; iCurJ++, j++ )
    //        {
    //          dict[ j ] = shared;
    //        }
    //      }
    //    }
    //    /// <summary>
    //    /// Adds new array formula into inner collection.
    //    /// </summary>
    //    /// <param name="shared">Formula to add.</param>
    //    [ CLSCompliant( false ) ]
    //    protected internal void  AddArrayFormula( ISharedFormula shared )
    //    {
    //      if( shared == null )
    //        throw new ArgumentNullException( "shared" );
    //
    //      m_arrArrayFormula.Add( shared );
    //      AddSharedFormula( ArrayFormula, shared );
    //    }
    /// <summary>
    /// Handler for NameIndexChanged event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="args">Event arguments.</param>
    private void OnNameIndexChanged( object sender, NameIndexChangedEventArgs args )
    {
      throw new NotImplementedException();
      //      for( int i = 0, iArrayLen = m_arrArrayFormula.Count; i < iArrayLen; i++ )
      //      {
      //        ArrayRecord array = ( ArrayRecord )m_arrArrayFormula[ i ];
      //        Ptg[] arrPtgs = array.Formula;
      //        
      //        if( arrPtgs == null || arrPtgs.Length == 0 ) return;
      //
      //        int iNewIndex = args.NewIndex;
      //        int iOldIndex = args.OldIndex;
      //
      //        for( int j = 0, iPtgsLen = arrPtgs.Length; j < iPtgsLen; j++ )
      //        {
      //          FormulaUtil.UpdateNameIndex( arrPtgs[ i ], iOldIndex, iNewIndex );
      //        }
      //      }
    }
    /// <summary>
    /// Attaches events to Named ranges used in array-entered formulas.
    /// </summary>
    internal void AttachNameIndexChangedEvent()
    {
      AttachNameIndexChangedEvent( 0 );
    }
    /// <summary>
    /// Attaches events to Named ranges used in array-entered formulas.
    /// </summary>
    /// <param name="iStartIndex">Start index of named ranges.</param>
    internal void AttachNameIndexChangedEvent( int iStartIndex )
    {
      throw new NotImplementedException();
      //      if( iStartIndex < 0 ) iStartIndex = 0;
      //
      //      for( int i = iStartIndex, iArrayLen = m_arrArrayFormula.Count; i < iArrayLen; i++ )
      //      {
      //        ArrayRecord array = ( ArrayRecord )m_arrArrayFormula[ i ];
      //        Ptg[] arrPtgs = array.Formula;
      //        
      //        if( arrPtgs == null || arrPtgs.Length == 0 ) return;
      //
      //        for( int j = 0, iPtgsLen = arrPtgs.Length; j < iPtgsLen; j++ )
      //        {
      //          int iNameIndex = -1;
      //
      //          if( FormulaUtil.IndexOf( FormulaUtil.NameCodes, arrPtgs[ j ].TokenCode ) != -1 )
      //          {
      //            NamePtg name = ( NamePtg )arrPtgs[ j ];
      //
      //            iNameIndex = name.ExternNameIndex - 1;
      //          }
      //          else if( FormulaUtil.IndexOf( FormulaUtil.NameXCodes, arrPtgs[ j ].TokenCode ) != -1 )
      //          {
      //            NameXPtg nameX = ( NameXPtg )arrPtgs[ j ];
      //            
      //            iNameIndex = nameX.NameIndex - 1;
      //          }
      //
      //          if( iNameIndex > 0 && !m_hashAttachedNames.Contains( iNameIndex ) )
      //          {
      //            NameImpl nameImpl = ( NameImpl )m_book.Names[ iNameIndex ];
      //            nameImpl.NameIndexChanged += m_nameIndexChanged;
      //            m_hashAttachedNames.Add( iNameIndex, null );
      //          }
      //        }
      //      }
    }
    /// <summary>
    /// Parses autofilters.
    /// </summary>
    public void ParseAutoFilters()
    {
      if( m_arrAutoFilter != null && m_arrAutoFilter.Count > 0 )
        m_autofilters.Parse( m_arrAutoFilter );
    }
    /// <summary>
    /// Extracts pivot tables from records array.
    /// </summary>
    /// <param name="iStartIndex">Index to the first pivot table record.</param>
    protected void ExtractPivotTables( int iStartIndex )
    {
      if( iStartIndex < 0 ) return;

      if( m_pivotTables == null )
        m_pivotTables = new PivotTableCollection( Application, this );

      m_pivotTables.Parse( m_arrRecords, iStartIndex );
    }
    /// <summary>
    /// Returns record at specified cell index.
    /// </summary>
    /// <param name="cellIndex">Record's cell index.</param>
    /// <returns>Record at specified cell index.</returns>
    [CLSCompliant( false )]
    protected internal ICellPositionFormat GetRecord( long cellIndex )
    {
      return /*( BiffRecordRaw )*/m_dicRecordsCells.GetCellRecord( cellIndex );
      //return m_dicRecordsCells[ iCellIndex ];
    }
    /// <summary>
    /// Returns record at specified cell index.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Record at specified cell index.</returns>
    [CLSCompliant( false )]
    protected internal ICellPositionFormat GetRecord( int iRow, int iColumn )
    {
      return m_dicRecordsCells.GetCellRecord( iRow, iColumn );
    }
    /// <summary>
    /// Extracts next record from reader and parses it.
    /// </summary>
    /// <param name="reader">Reader to extract record from.</param>
    /// <param name="iBOFCounter">Number of BOF records without closing EOF record.</param>
    /// <param name="bSkipStyles">Indicates whether styles information must be skipped.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object that decrypts data if necessary.</param>
    /// <returns>Updated number of BOF records without closing EOF record.</returns>
    [CLSCompliant( false )]
    protected override int ParseNextRecord( BiffReader reader, int iBOFCounter,
      ExcelParseOptions options, bool bSkipStyles, Dictionary<int, int> hashNewXFormatIndexes,
      IDecryptor decryptor )
    {
      TBIFFRecord recordType = reader.PeekRecordType();

      if( iBOFCounter == 1)
      {
        switch( recordType )
        {
          case TBIFFRecord.MulRK:
          case TBIFFRecord.MulBlank:
          case TBIFFRecord.Blank:
          case TBIFFRecord.LabelSST:
          case TBIFFRecord.RK:
          case TBIFFRecord.BoolErr:
          case TBIFFRecord.Formula:
          case TBIFFRecord.Label:
          case TBIFFRecord.RString:
          case TBIFFRecord.Array:
          case TBIFFRecord.String:
          case TBIFFRecord.Number:
          case TBIFFRecord.Row:
            if( !Application.UseFastRecordParsing || decryptor != null
              || !m_dicRecordsCells.ExtractRangesFast( m_index, reader, bSkipStyles, hashNewXFormatIndexes ) )
            {
              m_dicRecordsCells.ExtractRanges( reader, bSkipStyles, hashNewXFormatIndexes, decryptor );
            }
            return iBOFCounter;

          default:
            return base.ParseNextRecord( reader, iBOFCounter, options,
              bSkipStyles, hashNewXFormatIndexes, decryptor );
        }
      }
      else
      {
        return base.ParseNextRecord( reader, iBOFCounter, options,
          bSkipStyles, hashNewXFormatIndexes, decryptor );
      }
    }

    /// <summary>
    /// Parses dimensions record.
    /// </summary>
    /// <param name="dimensions">Record to parse.</param>
    [CLSCompliant( false )]
    protected override void ParseDimensions( DimensionsRecord dimensions )
    {
      base.ParseDimensions( dimensions );
      m_dicRecordsCells.Table.EnsureSize( m_iLastRow );
    }
    /// <summary>
    /// Sets cell at which panes are frozen.
    /// </summary>
    public void SetPaneCell( IRange range )
    {
      if( range.Row != range.LastRow || range.Column != range.LastColumn )
        throw new ArgumentOutOfRangeException( "range" );

      SplitCell = range;
      PaneFirstVisible = range;
      CreateAllSelections();
    }
    /// <summary>
    /// Creates all necessary selection records.
    /// </summary>
    private void CreateAllSelections()
    {
      int iSelectionCount = SelectionCount;
      Dictionary<int, object> usedIndexes = new Dictionary<int, object>();

      for( int i = m_arrSelections.Count - 1; i >= 0; i-- )
      {
        usedIndexes[ m_arrSelections[ i ].Pane ] = null;
      }

      int currentIndex = 0;
      for( int i = m_arrSelections.Count; i < iSelectionCount; i++ )
      {
        SelectionRecord selection = ( SelectionRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Selection );
        currentIndex = selection.Pane = ( byte )GetFreeIndex( currentIndex, usedIndexes );
        m_arrSelections.Add( selection );
      }

      int iRemoveCount = m_arrSelections.Count - iSelectionCount;

      if( iRemoveCount > 0 )
        m_arrSelections.RemoveRange( iSelectionCount, iRemoveCount );

      ReIndexSelections( usedIndexes );
    }
    /// <summary>
    /// Re-indexes selection records.
    /// </summary>
    /// <param name="usedIndexes">Dictionary with currently used selection indexes.</param>
    private void ReIndexSelections( Dictionary<int, object> usedIndexes )
    {
      int verticalSplit = 0;
      int horizontalSplit = 0;

      if( m_pane != null )
      {
        verticalSplit = m_pane.VerticalSplit;
        horizontalSplit = m_pane.HorizontalSplit;
      }

      // Panes that must be added.
      List<int> panes = new List<int>();

      // Panes that must be present.
      Dictionary<int, object> mustPresent = new Dictionary<int, object>();

      if( verticalSplit != 0 && horizontalSplit != 0 )
      {
        TryAdd( mustPresent, panes, usedIndexes, 0 );
        TryAdd( mustPresent, panes, usedIndexes, 1 );
        TryAdd( mustPresent, panes, usedIndexes, 2 );
        TryAdd( mustPresent, panes, usedIndexes, 3 );
      }
      else if( verticalSplit != 0 )
      {
        TryAdd( mustPresent, panes, usedIndexes, 3 );
        TryAdd( mustPresent, panes, usedIndexes, 1 );
      }
      else if( horizontalSplit != 0 )
      {
        TryAdd( mustPresent, panes, usedIndexes, 3 );
        TryAdd( mustPresent, panes, usedIndexes, 2 );
      }
      else
      {
        TryAdd( mustPresent, panes, usedIndexes, 3 );
      }

      for( int i = 0, j = 0, len = m_arrSelections.Count, jLen = panes.Count; i < len && j < jLen; i++ )
      {
        SelectionRecord selection = m_arrSelections[ i ];
        int currentPane = selection.Pane;

        if( !mustPresent.ContainsKey( currentPane ) )
        {
          selection.Pane = ( byte )panes[ j ];
          j++;
        }
      }

      if( m_pane != null && !mustPresent.ContainsKey( m_pane.ActivePane ) )
        m_pane.ActivePane = 3;
    }
    /// <summary>
    /// Tries to add next pane index if necessary.
    /// </summary>
    /// <param name="mustPresent">Dictionary with pane indexes that must be present in the file.</param>
    /// <param name="panes">Panes that are absent.</param>
    /// <param name="usedIndexes">Indexes that are currently present.</param>
    /// <param name="paneIndex">Pane index to add.</param>
    private void TryAdd( Dictionary<int, object> mustPresent, List<int> panes,
      Dictionary<int, object> usedIndexes, int paneIndex )
    {
      mustPresent.Add( paneIndex, null );

      if( !usedIndexes.ContainsKey( paneIndex ) )
        panes.Add( paneIndex );
    }
    /// <summary>
    /// Gets free index for the selection.
    /// </summary>
    /// <param name="currentIndex">Start index to try.</param>
    /// <param name="usedIndexes">Dictionary with used indexes.</param>
    /// <returns>Unused index that can be used as pane index.</returns>
    private int GetFreeIndex( int currentIndex, Dictionary<int, object> usedIndexes )
    {
      while( usedIndexes.ContainsKey( currentIndex ) )
        currentIndex++;

      usedIndexes[ currentIndex ] = null;
      return currentIndex;
    }
    #endregion

    #region IWorksheet methods
    /// <summary>
    /// Clears the worksheet. All the data including formatting and merges are removed.
    /// </summary>
    public void Clear()
    {
      ParseData();

      //m_arrDefaultColumnStyle.Clear();
      //m_arrDefaultRowStyle.Clear();
      base.ClearAll( ExcelWorksheetCopyFlags.CopyAll );
      ClearData();

        // remove all cells
        if (m_dicRecordsCells != null) m_dicRecordsCells.Clear();
        //m_dicArrayFormula.Clear();
        if(m_arrConditionalFormats!=null)
        m_arrConditionalFormats.Clear();
        m_rngUsed = null;

      m_arrColumnInfo = new ColumnInfoRecord[m_book.MaxColumnCount + 2];//.Clear();     

      if( m_hyperlinks != null ) m_hyperlinks.Clear();
      
      // reset dimensions
      m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
      m_iLastColumn = DEF_MIN_COLUMN_INDEX;
      m_iFirstRow = -1;
      m_iLastRow = -1;

      // remove merges
      if( m_mergedCells != null ) m_mergedCells.Clear();      
    }
    /// <summary>
    /// Clears all data.
    /// </summary>
    internal void ClearAllData()
    {
        //ParseData();

        //m_arrDefaultColumnStyle.Clear();
        //m_arrDefaultRowStyle.Clear();
        base.ClearAll(ExcelWorksheetCopyFlags.CopyAll);
        ClearData();
        if (this.PivotTables.Count>0)
        {
            for (int index = 0; index < this.PivotTables.Count; index++)
            {
                PivotTableImpl pivotTable = PivotTables [index ] as PivotTableImpl ;
                PivotCacheImpl cache = pivotTable.Cache;
                if (cache!= null && cache.Consolidation != null)
                {
                    cache.Consolidation.Dispose();
                    cache.Consolidation = null;
                }
            }
        }
        // remove all cells
        if (m_dicRecordsCells != null) m_dicRecordsCells.Clear();
        //m_dicArrayFormula.Clear();
        if (m_arrConditionalFormats != null)
            m_arrConditionalFormats.Clear();
        m_rngUsed = null;

        if (m_arrColumnInfo != null) m_arrColumnInfo = null;


        if (m_hyperlinks != null) m_hyperlinks.Clear();

        if (m_dataHolder != null) m_dataHolder.Dispose();
        // reset dimensions
        m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
        m_iLastColumn = DEF_MIN_COLUMN_INDEX;
        m_iFirstRow = -1;
        m_iLastRow = -1;

        // remove merges
        if (m_mergedCells != null) m_mergedCells.Clear();
        if (m_sparklineGroups != null) m_sparklineGroups.Clear();

        //TODO: Need to dispose m_names
        //if (m_names != null)
        //{
        //    foreach (NameImpl name in m_names)
        //    {
        //        name.Record.ClearData();
        //    }
        //    m_names.Clear();
        //    m_names = null;
        //}

        if (m_autoFitManager != null)
        {
            m_autoFitManager.Dispose();
            m_autoFitManager = null;
        }
        if (m_dataValidation != null) m_dataValidation.Clear();
        if (m_listObjects != null) m_listObjects.Dispose();
        if (m_book != null)
            m_book = null;
        if (m_pageSetup != null)
        {
            m_pageSetup.Dispose();
            m_pageSetup = null;
    }
    }
    /// <summary>
    /// Clears the data in the worksheet. Only the data in the cells are removed.
    /// The formatting and merges are not removed.
    /// </summary>
    public void ClearData()
    {
      //ParseData();

      // TODO: Finish implementation
      if (m_dicRecordsCells != null)
      {
          m_dicRecordsCells.ClearData();
      }
    }
    /// <summary>
    /// Indicates whether a cell was initialized or accessed by the user.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <returns>Value indicating whether the cell was initialized or accessed by the user.</returns>
    public bool Contains( int iRow, int iColumn )
    {
      ParseData();

      long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
      return m_dicRecordsCells.Contains( lCellIndex );
    }
    /// <summary>
    /// Creates new instance of IRanges.
    /// </summary>
    /// <returns>New instance of ranges collection.</returns>
    public IRanges CreateRangesCollection()
    {
      return AppImplementation.CreateRangesCollection( this );
    }

    /// <summary>
    /// Create Named Ranges
    /// </summary>
    /// <param name="namedRange">Names to create</param>
    /// <param name="referRange">Refers to range</param>
    /// <param name="vertical">True if data are vertically placed in the sheet.</param>
    public void CreateNamedRanges(string namedRange, string referRange, bool vertical)
    {
        IWorksheet sheet = this;
        
        IRanges refersColl = sheet.CreateRangesCollection();
        if (!vertical)
        {
            for (int i = sheet[referRange].Row; i < sheet[referRange].LastRow + 1; i++)
            {
                refersColl.Add(sheet[i, sheet[referRange].Column, i, sheet[referRange].LastColumn]);
            }
        }
        else
        {
            for (int i = sheet[referRange].Column; i < sheet[referRange].LastColumn + 1; i++)
            {
                refersColl.Add(sheet[sheet[referRange].Row, i, sheet[referRange].LastRow, i]);
            }
        }

        int count = 0; INames namesCollection = sheet.Names;

        try
        {
            foreach (IRange range in sheet[namedRange])
            {
                IName name = namesCollection.Add(range.Text);
                name.RefersToRange = refersColl[count];
                count++;
            }
        }
        catch (Exception ex)
        {
            throw new Exceptions.InvalidRangeException("NamedRange and data count mismatch");
        }
    }
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <returns>Object that can be used for template markers processing.</returns>
    public ITemplateMarkersProcessor CreateTemplateMarkersProcessor()
    {
      return AppImplementation.CreateTemplateMarkers( this );
    }
    /// <summary>
    /// Shows / Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">One-based column index to show or hide.</param>
    /// <param name="isVisible">TRUE - show column, FALSE - hide column.</param>
    public void ShowColumn( int columnIndex, bool isVisible )
    {
      ParseData();

      if( columnIndex < 0 || columnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "columnIndex", "Value cannot be less than 0 and greater than 255" );

      ColumnInfoRecord columnInfo = m_arrColumnInfo[ columnIndex ];

      if( columnInfo == null )
      {
        columnInfo = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
        columnInfo.FirstColumn = ( ushort )( columnIndex - 1 );
        columnInfo.LastColumn = ( ushort )( columnIndex - 1 );
        columnInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
        m_arrColumnInfo[ columnIndex ] = columnInfo;
      }
      else if( isVisible && columnInfo.ColumnWidth == 0 )
      {
        SetColumnWidth( columnIndex, StandardWidth );
      }

      columnInfo.IsHidden = !isVisible;
      UpdateShapes();
    }
    /// <summary>
    /// Hides the specified column.
    /// </summary>
    /// <param name="columnIndex">One-based column index to hide.</param>
    public void HideColumn(int columnIndex)
    {
        ShowColumn(columnIndex, false);
    }
    /// <summary>
    /// Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index to hide.</param>
    public void HideRow(int rowIndex)
    {
        ShowRow(rowIndex, false);
    }
    /// <summary>
    /// Shows / Hides the specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index to show or hide.</param>
    /// <param name="isVisible">TRUE - show row, FALSE - hide row.</param>
    public void ShowRow( int rowIndex, bool isVisible )
    {
      if( rowIndex < 1 || rowIndex > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "rowIndex" );

      RowStorage storage = WorksheetHelper.GetOrCreateRow( this, rowIndex - 1, true );
      storage.IsHidden = !isVisible;
      UpdateShapes();
    }
    private void UpdateShapes()
    {
        if (this.Shapes.Count == 0)
            return;

        for (int index = 0; index < this.Shapes.Count; index++)
        {
            if (!this.Shapes[index].IsSizeWithCell)
            {
                ((ShapeImpl)this.Shapes[index]).UpdateAnchorPoints();
            }
        }
    }
    /// <summary>
    /// Shows / Hides the specified range.
    /// </summary>
    /// <param name="range">Range specifies the particular range to show / hide</param>
    /// <param name="isVisible">True - Range is visible; false - hidden.</param>
    public void ShowRange(IRange range, bool isVisible)
    {
        bool colCheck = false; 
        bool rowCheck = false;
        if (range.Row < 1 || range.Row > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("Row");

        int FirstRow = range.Row;
        int LastRow = range.LastRow;
        if (((range.LastRow - range.Row) > (m_book.MaxRowCount - (range.LastRow - range.Row))) && range.LastRow == m_book.MaxRowCount && !isVisible)
        {
            colCheck = true;
            FirstRow = 1;
            LastRow = range.Row - 1;
             if (LastRow < this.UsedRange.LastRow)
                {
                    FirstRow = range.Row;
                    LastRow = this.UsedRange.LastRow;
                    rowCheck = true;
                }
            IsZeroHeight = true;
            isVisible = true;
        }
       
        for (int iRow = FirstRow, iLastRow = LastRow; iRow <= iLastRow; iRow++)
        {
            RowStorage storage = WorksheetHelper.GetOrCreateRow(this, iRow  - 1, true);
             
            storage.IsHidden = (rowCheck) ? isVisible : !isVisible ;
            
            ParseData();
            
        }        
        if (range.Column < 0 || range.Column > m_book.MaxColumnCount)
            throw new ArgumentOutOfRangeException("Column", "Value cannot be less than 0 and greater than 255");
        
        for (int iCol = range.Column, iLastCol = range.LastColumn; iCol <= iLastCol; iCol++)
        {
            ColumnInfoRecord columnInfo = m_arrColumnInfo[iCol];

            if (columnInfo == null)
            {
                columnInfo = (ColumnInfoRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ColumnInfo);
                columnInfo.FirstColumn = (ushort)(iCol - 1);
                columnInfo.LastColumn = (ushort)(iLastCol-1);
                columnInfo.ExtendedFormatIndex = (ushort)m_book.DefaultXFIndex;
                m_arrColumnInfo[iCol ] = columnInfo;
            }
            else if (isVisible && columnInfo.ColumnWidth == 0)
            {
                SetColumnWidth(iCol, StandardWidth);
            }

            
                 columnInfo.IsHidden = colCheck ? isVisible : !isVisible ;

        }
        UpdateShapes();
        
        
    }
    /// <summary>
    /// Shows/ Hides the collection of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range collection.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(RangesCollection ranges, bool isVisible)
    {
        if (ranges.Count == 0)
            return;

        foreach (IRange range in ranges)
        {
            ShowRange(range, isVisible);
        }
    }
    /// <summary>
    /// Shows/ Hides an array of range.
    /// </summary>
    /// <param name="ranges">Ranges specifies the range array.</param>
    /// <param name="isVisible">True - Row is visible; false - hidden.</param>
    public void ShowRange(IRange[] ranges, bool isVisible)
    {
        if (ranges.Length == 0)
            return;

        RangesCollection collection = new RangesCollection(Application, this);
        foreach (IRange range in ranges)
        {
            collection.Add(range);
        }
        ShowRange(collection, isVisible);
    }

    /// <summary>
    /// Method check is Column with specified index visible to end user or not
    /// </summary>
    /// <param name="columnIndex">Index of column </param>
    /// <returns>True - column is visible, otherwise False</returns>
    public bool IsColumnVisible( int columnIndex )
    {
      if( columnIndex < 1 || columnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "columnIndex", "Value cannot be less than 0 and greater than 255" );

      ParseData();

      ColumnInfoRecord columnInfo = /*( ColumnInfoRecord )*/m_arrColumnInfo[ columnIndex ];

      return ( columnInfo != null )
        ? !columnInfo.IsHidden
        : true;
    }
    /// <summary>
    /// Method check is Row with specified index visible to user or not
    /// </summary>
    /// <param name="rowIndex">Index of row visibility of each must be checked</param>
    /// <returns>True - row is visible to user, otherwise False</returns>
    public bool IsRowVisible( int rowIndex )
    {
      if( rowIndex < 1 || rowIndex > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "rowIndex" );

      RowStorage row = WorksheetHelper.GetOrCreateRow( this, rowIndex - 1, false );
      return ( row != null )
        ? !row.IsHidden
        : true;
    }
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted</param>
    public void InsertRow( int iRowIndex )
    {
      InsertRow( iRowIndex, 1, ExcelInsertOptions.FormatDefault );
    }
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    public void InsertRow( int iRowIndex, int iRowCount )
    {
      InsertRow( iRowIndex, iRowCount, ExcelInsertOptions.FormatDefault );
    }
    /// <summary>
    /// Inserts an empty row with default formatting.
    /// </summary>
    /// <param name="iRowIndex">Index at which new row should be inserted.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    public void InsertRow( int iRowIndex, int iRowCount,
      ExcelInsertOptions insertOptions )
    {
      ParseData();

      if( iRowIndex < 1 || iRowIndex > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex" );

      if( !CanInsertRow( iRowIndex, iRowCount, insertOptions )
        || !InnerShapes.CanInsertRowColumn( iRowIndex, iRowCount, true, m_book.MaxRowCount ) )
      {
        throw new ArgumentException( "Can't insert row" );
      }

      if( m_mergedCells != null ) m_mergedCells.InsertRow( iRowIndex, iRowCount );

      m_book.InnerNamesColection.InsertRow( iRowIndex, iRowCount, Name );
      bool isInRange = iRowIndex <= m_iLastRow;
      bool isLastRow = (iRowIndex + iRowCount) >= m_book.MaxRowCount;
      if (!isLastRow)
      {
          if(!isInRange)
            m_iLastRow = iRowIndex;

          if (m_iFirstColumn < m_book.MaxColumnCount)
          {
              int iFirstRow = iRowIndex;//m_iFirstRow;

              //if( iRowIndex >= m_iFirstRow ) iFirstRow = iRowIndex;

              IRange range = Range[iFirstRow, m_iFirstColumn, m_iLastRow, m_iLastColumn];

              ExcelCopyRangeOptions options = ExcelCopyRangeOptions.UpdateFormulas |  ExcelCopyRangeOptions.CopyErrorIndicators|
                ExcelCopyRangeOptions.CopyConditionalFormats;// | ExcelCopyRangeOptions.CopyShapes;

              IRange destRange = Range[iFirstRow + iRowCount, m_iFirstColumn,
                m_iLastRow + iRowCount, m_iLastColumn];

              MoveRange(destRange, range, options, true);
          }
          else
          {
              m_iLastRow += iRowCount;
              m_dicRecordsCells.Table.InsertIntoDefaultRows(iRowIndex - 1, iRowCount);
          }
      }
      if( isInRange)
      {
       

        CopyStylesAfterInsert( iRowIndex, iRowCount, insertOptions, true );
        CopyConditionalFormatsAfterInsert( iRowIndex, iRowCount, insertOptions, true );
        CopyDataValidationAfterInsert( iRowIndex, iRowCount, insertOptions, true );
        //        if( m_iFirstColumn <= m_book.MaxColumnCount && m_usLastColumn <= m_book.MaxColumnCount )
        //        {
        //          Range[ iRowIndex, m_iFirstColumn, iRowIndex, m_usLastColumn ].CellStyleName = "Normal";
        //        }
      }
      else if( insertOptions != ExcelInsertOptions.FormatDefault )
      {
        CopyStylesAfterInsert( iRowIndex, iRowCount, insertOptions, true );
      }

      InnerShapes.InsertRemoveRowColumn( iRowIndex, iRowCount, true, false );
      m_book.UpdatePivotCachesAfterInsertRemove( this, iRowIndex, iRowCount, true, false );
      //      UpdateErrorIndicators( iRowIndex, iRowCount, true, true );
      (this.HPageBreaks as HPageBreaksCollection).InsertRows(iRowIndex - 1, iRowCount);
    }
    /// <summary>
    /// Inserts an empty column with default formatting (without updating any formula).
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted</param>
    public void InsertColumn( int iColumnIndex )
    {
      InsertColumn( iColumnIndex, 1, ExcelInsertOptions.FormatDefault );
    }
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount )
    {
      InsertColumn( iColumnIndex, iColumnCount,
        ExcelInsertOptions.FormatDefault );
    }
    /// <summary>
    /// Inserts an empty column with default formatting.
    /// </summary>
    /// <param name="iColumnIndex">Index at which new column should be inserted</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    /// <param name="insertOptions">Insert options.</param>
    public void InsertColumn( int iColumnIndex, int iColumnCount,
      ExcelInsertOptions insertOptions )
    {
      ParseData();

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "Value cannot be less 1 and greater than max column index." );

      if( !CanInsertColumn( iColumnIndex, iColumnCount, insertOptions )
        || !InnerShapes.CanInsertRowColumn( iColumnIndex, iColumnCount,
        false, m_book.MaxColumnCount ) )
      {
        throw new ArgumentException( "Can't insert column" );
      }

      if( iColumnCount < 1 || iColumnCount > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumnCount", "Value cannot be less 1 and greater than max column index" );

      int iFirstCol = m_iFirstColumn;

      if( m_mergedCells != null ) m_mergedCells.InsertColumn( iColumnIndex, iColumnCount );

      m_book.InnerNamesColection.InsertColumn( iColumnIndex, iColumnCount, Name );

      if( iColumnIndex <= m_iLastColumn && m_iFirstRow > 0 && m_iFirstRow <= m_book.MaxRowCount )
      {
        if( iColumnIndex >= iFirstCol ) iFirstCol = iColumnIndex;

        IRange range = Range[ m_iFirstRow, iFirstCol, m_iLastRow, m_iLastColumn ];
        ExcelCopyRangeOptions options = ExcelCopyRangeOptions.UpdateFormulas | ExcelCopyRangeOptions.CopyConditionalFormats |ExcelCopyRangeOptions.CopyErrorIndicators;// | ExcelCopyRangeOptions.CopyShapes;

        MoveRange( Range[ m_iFirstRow, iFirstCol + iColumnCount ], range, options, false );
        //      range.MoveTo( Range[ m_iFirstRow, iFirstCol + 1 ], bUpdateFormula );
        InsertIntoDefaultColumns( iColumnIndex, iColumnCount, insertOptions );
      }

      CopyStylesAfterInsert( iColumnIndex, iColumnCount, insertOptions, false );
      CopyConditionalFormatsAfterInsert( iColumnIndex, iColumnCount, insertOptions, false );
      m_book.UpdatePivotCachesAfterInsertRemove( this, iColumnIndex, iColumnCount, false, false );
      CopyDataValidationAfterInsert( iColumnIndex, iColumnCount, insertOptions, false );
      InnerShapes.InsertRemoveRowColumn( iColumnIndex, iColumnCount, false, false );
      //      UpdateErrorIndicators( iColumnIndex, iColumnCount, true, false );
      (this.VPageBreaks as VPageBreaksCollection).InsertColumns(iColumnIndex - 1, iColumnCount);
    }
    /// <summary>
    /// Moves DataValidation of the succeeding cells
    /// </summary>
    /// <param name="iIndex">Row / Column to start with</param>
    /// <param name="bIsRow">Toggles between insertion of Row or Column</param>
    private void CopyDataValidationAfterInsert( int iIndex, int iCount, ExcelInsertOptions insertOptions, bool bIsRow )
    {
      int iRowCount;
      int iColumnCount;

      if( bIsRow )
      {
        iRowCount = m_book.MaxRowCount - iIndex + 1;
        iColumnCount = m_book.MaxColumnCount - m_iFirstColumn + 1;
        CopyMoveDataValidations( iIndex, m_iFirstColumn, iRowCount, iColumnCount, iIndex + iCount, m_iFirstColumn, this, true );
      }
      else
      {
        iRowCount = m_book.MaxRowCount - m_iFirstRow + 1;
        iColumnCount = m_book.MaxColumnCount - iIndex + 1;
        CopyMoveDataValidations( m_iFirstRow, iIndex, iRowCount, iColumnCount, m_iFirstRow, iIndex + iCount, this, true );
      }

      UpdateDataValidationOnInsertOption( iIndex, iCount, insertOptions, bIsRow );
    }
    /// <summary>
    /// Update data validation object based on insert options.
    /// </summary>
    /// <param name="iIndex">Index where row or column was (were) inserted.</param>
    /// <param name="iCount">Number of inserted rows/columns.</param>
    /// <param name="insertOptions">Insert options.</param>
    /// <param name="bIsRow">Indicates whether rows were inserted.</param>
    private void UpdateDataValidationOnInsertOption( int iIndex, int iCount, ExcelInsertOptions insertOptions, bool bIsRow )
    {
      //if( insertOptions == ExcelInsertOptions.FormatDefault )
      //  return;

      // 1. Init variables
      int iRowDelta;
      int iColumnDelta;
      int iDestRow;
      int iDestColumn;
      int iSourceRow;
      int iSourceColumn;
      int iRowCount;
      int iColumnCount;

      if( bIsRow )
      {
        iRowDelta = 1;
        iColumnDelta = 0;
        iDestRow = iIndex;
        iDestColumn = 1;
        iRowCount = 1;
        iColumnCount = m_book.MaxColumnCount;
      }
      else
      {
        iRowDelta = 0;
        iColumnDelta = 1;
        iDestRow = 1;
        iDestColumn = iIndex;
        iRowCount = m_book.MaxRowCount;
        iColumnCount = 1;
      }

      if( insertOptions == ExcelInsertOptions.FormatAsBefore || insertOptions == ExcelInsertOptions.FormatDefault )
      {
        if( bIsRow )
        {
          iSourceRow = iIndex - 1;
          iSourceColumn = 1;
        }
        else
        {
          iSourceRow = 1;
          iSourceColumn = iIndex - 1;
        }
      }
      else
      {
        if( bIsRow )
        {
          iSourceRow = iIndex + iCount;
          iSourceColumn = 1;
        }
        else
        {
          iSourceRow = 1;
          iSourceColumn = iIndex + iCount;
        }
      }

      // 2. process inserted items
      for( int i = 0; i < iCount; i++, iDestRow += iRowDelta, iDestColumn += iColumnDelta )
      {
        CopyMoveDataValidations( iSourceRow, iSourceColumn, iRowCount, iColumnCount, iDestRow, iDestColumn, this, false );
      }
    }
    /// <summary>
    /// Removes specified row.
    /// </summary>
    /// <param name="index">One-based row index to remove</param>
    public void DeleteRow( int index )
    {
      DeleteRow( index, 1 );
    }
    /// <summary>
    /// Removes specified row.
    /// </summary>
    /// <param name="index">One-based row index to remove</param>
    /// <param name="count">Number of rows to delete.</param>
    public void DeleteRow( int index, int count )
    {
      ParseData();

      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      if( index < 1 || index > m_book.MaxRowCount - count + 1 )
        throw new ArgumentOutOfRangeException( "row index" );


      int firstRow = 0, lastRow = 0, firstColumn = 0, lastColumn = 0, deleteStartRange = 0, deleteEndRange = 0, tableInDeleteRange = 0;
      int tableCount = ListObjects.Count;
      for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
      {
          IRange tableRange = ListObjects[tableIndex].Location;
          firstRow = tableRange.Row;
          lastRow = tableRange.LastRow;
          deleteStartRange = index + count - 1 - firstRow;
          deleteEndRange = lastRow - index;
          if (deleteStartRange >= 0 && deleteEndRange >= 0)
              tableInDeleteRange += 1;
          if (tableInDeleteRange > 1)
              return;
      }

      for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
      {
          IRange tableRange = ListObjects[tableIndex].Location;
          firstRow = tableRange.Row;
          lastRow = tableRange.LastRow;
          firstColumn = tableRange.Column;
          lastColumn = tableRange.LastColumn;

          deleteStartRange = index + count - 1 - firstRow;
          deleteEndRange = lastRow - index;
          if (deleteStartRange >= 0 && deleteEndRange >= 0)
          {
              if (index <= firstRow)
                  return;
              else if (index + count - 1 > lastRow)
              {
                  int rowRange = lastRow - index + 1;
                  lastRow -= rowRange;
              }
              else
                  lastRow -= count;

              ListObjects[tableIndex].Location = Range[firstRow, firstColumn, lastRow, lastColumn];
          }
          else if (deleteStartRange < 0)
          {
              firstRow -= count;
              lastRow -= count;
              ListObjects[tableIndex].Location = Range[firstRow, firstColumn, lastRow, lastColumn];
          }
      }

      RecordTable table = m_dicRecordsCells.Table;
      int iFirstRow = table.FirstRow + 1;//UsedRange.Row;
      int iLastRow = table.LastRow + 1;//UsedRange.LastRow;
      int iColumn = m_iFirstColumn;
      int iFirstCol = ( iColumn > 0 ) ? iColumn : 1;

      iColumn = m_iLastColumn;
      int iLastCol = ( iColumn > 0 ) ? iColumn : 1;

      if( iFirstRow > 0 )
      {
        RangeImpl range = ( RangeImpl )Range[ index, iFirstCol, index + count - 1, iLastCol ];

        // This is not necessary since we have similar check in the MoveRange method.
        //if( !range.AreFormulaArraysNotSeparated )
        //  throw new InvalidRangeException();

        iFirstRow = index + count;
        Rectangle removedRow = Rectangle.FromLTRB( FirstColumn - 1, index - 1, LastColumn - 1, index + count - 2 );
        m_arrConditionalFormats.Remove( new Rectangle[] { removedRow } );

        if( iFirstRow <= iLastRow )
        {
          IRange sourceRange = Range[ iFirstRow, iFirstCol, iLastRow, iLastCol ];
          IRange destRange = Range[ index, iFirstCol ];

          ExcelCopyRangeOptions options = ExcelCopyRangeOptions.UpdateFormulas | ExcelCopyRangeOptions.CopyConditionalFormats;
          // | ExcelCopyRangeOptions.CopyShapes;

          IOperation operation = new RowsClearer( this, index, count );
          MoveRange( destRange, sourceRange, options, true, operation );
        }

        if( m_mergedCells != null )
          m_mergedCells.RemoveRow( index, count );
      }

      m_book.InnerNamesColection.RemoveRow( index, Name, count );
      m_book.UpdatePivotCachesAfterInsertRemove( this, index, count, true, true );
      InnerShapes.InsertRemoveRowColumn( index, count, true, true );
      //      UpdateErrorIndicators( index, 1, false, true );
      (this.HPageBreaks as HPageBreaksCollection).DeleteRows(index - 1, count);

      int iLastRowsToRemove = Math.Min( iLastRow - index + 1, count );

      if( iLastRowsToRemove > 0 )
      {
        RemoveLastRow( true, iLastRowsToRemove );
      }
    }
    /// <summary>
    /// Copies row.
    /// </summary>
    /// <param name="iDestRowIndex">Zero-based destination row index.</param>
    /// <param name="iSourceRowIndex">Zero-based source row index.</param>
    private void CopyRowRecord( int iDestRowIndex, int iSourceRowIndex )
    {
      RowStorage destRow = WorksheetHelper.GetOrCreateRow( this, iDestRowIndex, true );
      RowStorage sourceRow = WorksheetHelper.GetOrCreateRow( this, iSourceRowIndex, true );
      destRow.CopyRowRecordFrom( sourceRow );
    }
    /// <summary>
    /// Removes specified column.
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    public void DeleteColumn( int index )
    {
      DeleteColumn( index, 1 );
    }
    /// <summary>
    /// Removes specified column.
    /// </summary>
    /// <param name="index">One-based column index to remove.</param>
    /// <param name="count">Number of columns to remove.</param>
    public void DeleteColumn( int index, int count )
    {
      ParseData();

      if( index < 1 || index > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "column index" );

      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      if( index + count > m_book.MaxColumnCount )
        count = m_book.MaxColumnCount - index;

      if( count == 0 )
        return;

      //IRange usedRange = UsedRange;
      int iFirstRow = m_iFirstRow;
      int iLastRow = m_iLastRow;
      int iFirstCol = m_iFirstColumn;
      int iLastCol = m_iLastColumn;

      if( iFirstRow > 0 )
      {
        RangeImpl range = ( RangeImpl )Range[ iFirstRow, index, iLastRow, index + count - 1 ];

        if( !range.AreFormulaArraysNotSeparated )
          throw new InvalidRangeException();

        int firstColumn = 0, lastColumn = 0, firstRow = 0, lastRow = 0, deleteStartRange = 0, deleteEndRange = 0, tableInDeleteRange = 0;
        int tableCount = ListObjects.Count;
        for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
        {
            IRange tableRange = ListObjects[tableIndex].Location;
            firstColumn = tableRange.Column;
            lastColumn = tableRange.LastColumn;
            deleteStartRange = index + count - 1 - firstColumn;
            deleteEndRange = lastColumn - index;
            if (deleteStartRange >= 0 && deleteEndRange >= 0)
                tableInDeleteRange += 1;
            if (tableInDeleteRange > 1)
                return;
        }

        for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
        {
            IRange tableRange = ListObjects[tableIndex].Location;

            firstColumn = tableRange.Column;
            lastColumn = tableRange.LastColumn;
            firstRow = tableRange.Row;
            lastRow = tableRange.LastRow;

            deleteStartRange = index + count - 1 - firstColumn;
            deleteEndRange = lastColumn - index;

            if (deleteStartRange >= 0 && deleteEndRange >= 0)
            {
                int columnIndex = 0;
                int currentColumnIndex = 0;
                int columnCount = count;

                if (index >= firstColumn)
                {
                    columnIndex = index - firstColumn;
                    if (index + count - 1 > lastColumn)
                        columnCount = tableCount - columnIndex;
                }
                else
                {
                    currentColumnIndex = firstColumn - index;
                    if (lastColumn >= index + count - 1)
                        columnCount -= currentColumnIndex;
                    else
                        columnCount = tableCount;
                }
                for (int columnIndexCount = 0; columnIndexCount < columnCount; columnIndexCount++)
                {
                    ListObjects[tableIndex].Columns.RemoveAt(columnIndex);
                    lastColumn -= 1;
                }

                if (ListObjects[tableIndex].Columns.Count == 0)
                {
                    ListObjects.RemoveAt(tableIndex);
                    tableIndex -= 1;
                    tableCount -= 1;
                }
                else if (index < firstColumn)
                {
                    firstColumn -= currentColumnIndex;
                    lastColumn -= currentColumnIndex;
                }
                else
                    ListObjects[tableIndex].Location = Range[firstRow, firstColumn, lastRow, lastColumn];
            }
            else if (deleteStartRange < 0)
            {
                firstColumn -= count;
                lastColumn -= count;
                ListObjects[tableIndex].Location = Range[firstRow, firstColumn, lastRow, lastColumn];
            }
        }

        Rectangle removedRow = Rectangle.FromLTRB( index - 1, FirstRow - 1, index + count - 2, LastRow - 1 );
        Rectangle[] removedItems = new Rectangle[] { removedRow };
        m_arrConditionalFormats.Remove( removedItems );

        if( m_dataValidation != null )
          m_dataValidation.Remove( removedItems );

        if( index < iLastCol )
        {
          iFirstCol = index + count;

          if( iFirstCol <= iLastCol )
          {
            IRange sourceRange = Range[ iFirstRow, iFirstCol, iLastRow, iLastCol ];
            IRange destRange = Range[ iFirstRow, index ];

            ExcelCopyRangeOptions options = ExcelCopyRangeOptions.UpdateFormulas |
              ExcelCopyRangeOptions.CopyConditionalFormats |
              ExcelCopyRangeOptions.CopyDataValidations;// | ExcelCopyRangeOptions.CopyShapes;

            MoveRange( destRange, sourceRange, options, false );
          }
        }

        if( m_mergedCells != null )
          m_mergedCells.RemoveColumn( index, count );
      }

      m_book.InnerNamesColection.RemoveColumn( index, Name, count );
      m_book.UpdatePivotCachesAfterInsertRemove( this, index, count, false, true );
      RemoveFromDefaultColumns( index, count, ExcelInsertOptions.FormatDefault );

      InnerShapes.InsertRemoveRowColumn( index, count, false, true );
      //      UpdateErrorIndicators( index, 1, false, false );
      (this.VPageBreaks as VPageBreaksCollection).DeleteColumns(index - 1, count);

      if( UsedRange.LastColumn >= index )
      {
        count = Math.Min( count, iLastCol - index + 1 );
        RemoveLastColumn( true, count );
      }
    }
    /// <summary>
    /// Returns width in pixels from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width in Excel units of the specified column.</returns>
    public double GetColumnWidth( int iColumnIndex )
    {
      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "Value cannot be less 1 and greater than max column index." );

      double widthInChars = InnerGetColumnWidth( iColumnIndex );
      return widthInChars;
    }
    /// <summary>
    /// Returns width in pixels from ColumnInfoRecord if there is corresponding ColumnInfoRecord
    /// or StandardWidth if not.
    /// </summary>
    /// <param name="iColumnIndex">One-based index of the column.</param>
    /// <returns>Width in pixels of the specified column.</returns>
    public int GetColumnWidthInPixels( int iColumnIndex )
    {
      if ( iColumnIndex > m_book.MaxColumnCount )
          iColumnIndex = m_book.MaxColumnCount;

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "Value cannot be less 1 and greater than max column index." );

      double widthInChars = InnerGetColumnWidth( iColumnIndex );
      return ColumnWidthToPixels( widthInChars );
    }
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRow">One-based index of the row</param>
    /// <returns>
    /// Height from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public double GetRowHeight( int iRow )
    {
      return InnerGetRowHeight( iRow, true );
    }
    internal double GetInnerRowHeight(int iRow)
    {
        return InnerGetRowHeight(iRow + 1, true);
    }
    /// <summary>
    /// Returns height from RowRecord if there is a corresponding RowRecord.
    /// Otherwise returns StandardHeight. 
    /// </summary>
    /// <param name="iRowIndex">One-based index of the row.</param>
    /// <returns>
    /// Height in pixels from RowRecord if there is corresponding RowRecord.
    /// Otherwise returns StandardHeight.
    /// </returns>
    public int GetRowHeightInPixels( int iRowIndex )
    {
      double height = GetRowHeight( iRowIndex );
      return ( int )ApplicationImpl.ConvertToPixels( ( float )height, MeasureUnits.Point );
    }
    internal int GetInnerRowHeightInPixels(int iRowIndex)
    {
        double height = GetInnerRowHeight(iRowIndex);
        return (int)ApplicationImpl.ConvertToPixels((float)height, MeasureUnits.Point);
    }
    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    private int ImportArray<T>( T[] arrObject, int firstRow, int firstColumn
      , bool isVertical )
    {
      if( arrObject == null )
        throw new ArgumentNullException( "arrObject" );

      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentNullException( "firstColumn" );

      ParseData();
      bool preserveString = false;
      int i = 0;
      int elementsToImport;

      if( isVertical )
      {
        elementsToImport = Math.Min( firstRow + arrObject.Length - 1, m_book.MaxRowCount )
          - firstRow + 1;
      }
      else
      {
        elementsToImport = Math.Min( firstColumn + arrObject.Length - 1, m_book.MaxColumnCount )
          - firstColumn + 1;
      }

      int iXFIndex = m_book.DefaultXFIndex;
      IRange range;

      if( elementsToImport > 0 )
      {
        range = InnerGetCell( firstColumn, firstRow );
        if (arrObject[i] == null)
        {
            range.Value2 = null;
        }
        else
        {
            preserveString = this.IsStringsPreserved;
            if (arrObject[i].GetType() == typeof(string) && !CheckIsFormula(arrObject[i]) && this.IsStringsPreserved)
                this.IsStringsPreserved = true;
            else
                this.IsStringsPreserved = false;

            range.Value2 = arrObject[i];
            this.IsStringsPreserved = preserveString;
        }
        RangeImpl rangeImpl = ( RangeImpl )range;
        iXFIndex = rangeImpl.ExtendedFormatIndex;
      }

      for( i = 1; i < elementsToImport; i++ )
      {
        if( !isVertical )
        {
          range = InnerGetCell( firstColumn + i, firstRow, iXFIndex );
        }
        else
        {
          range = InnerGetCell( firstColumn, firstRow + i, iXFIndex );
        }
        if (arrObject[i] != null)
        {
            preserveString = this.IsStringsPreserved;
            if (arrObject[i].GetType() == typeof(string) && !CheckIsFormula(arrObject[i]) && this.IsStringsPreserved)
                this.IsStringsPreserved = true;
            else
                this.IsStringsPreserved = false;
        } 
        range.Value2 = arrObject[ i ];
        this.IsStringsPreserved = preserveString;
      }

      return i;
    }

    /// <summary>
    /// Checks the string object is a formula.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    private bool CheckIsFormula(object value)
    {
        if (value.ToString().StartsWith("="))
            return true;

        return false;
    }
    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( object[] arrObject, int firstRow, int firstColumn
      , bool isVertical )
    {
      return ImportArray<object>( arrObject, firstRow, firstColumn, isVertical );
    }
    /// <summary>
    /// Imports an array of strings into a worksheet.
    /// </summary>
    /// <param name="arrString">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( string[] arrString, int firstRow, int firstColumn
      , bool isVertical )
    {
      return ImportArray<string>( arrString, firstRow, firstColumn, isVertical );
    }
    /// <summary>
    /// Imports an array of integers into a worksheet.
    /// </summary>
    /// <param name="arrInt">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( int[] arrInt, int firstRow, int firstColumn
      , bool isVertical )
    {
      return ImportArray<int>( arrInt, firstRow, firstColumn, isVertical );
    }
    /// <summary>
    /// Imports an array of doubles into a worksheet.
    /// </summary>
    /// <param name="arrDouble">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( double[] arrDouble, int firstRow, int firstColumn
      , bool isVertical )
    {
      return ImportArray<double>( arrDouble, firstRow, firstColumn, isVertical );
    }
    /// <summary>
    /// Imports an array of DateTimes into worksheet.
    /// </summary>
    /// <param name="arrDateTime">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <param name="isVertical">
    /// TRUE if array should be imported vertically; FALSE - horizontally.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( DateTime[] arrDateTime, int firstRow, int firstColumn
      , bool isVertical )
    {
      if( arrDateTime == null )
        throw new ArgumentNullException( "arrObject" );

      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentNullException( "firstColumn" );

      ParseData();
      this.IsStringsPreserved = false;
      int i = 0;
      int elementsToImport;

      if( isVertical )
      {
        elementsToImport = Math.Min( firstRow + arrDateTime.Length - 1, m_book.MaxRowCount )
          - firstRow + 1;
      }
      else
      {
        elementsToImport = Math.Min( firstColumn + arrDateTime.Length - 1, m_book.MaxColumnCount )
          - firstColumn + 1;
      }

      //IStyle styleOfArray = null;
      int iXFIndex = m_book.DefaultXFIndex;
      IRange range;

      if( elementsToImport > 0 )
      {
        if( !isVertical )
        {
          range = InnerGetCell( firstColumn, firstRow );
        }
        else
        {
          range = InnerGetCell( firstColumn, firstRow );
        }

        range.DateTime = arrDateTime[ i ];
        RangeImpl rangeImpl = ( RangeImpl )range;
        iXFIndex = rangeImpl.ExtendedFormatIndex;
      }

      for( i = 1; i < elementsToImport; i++ )
      {
        if( !isVertical )
        {
          range = InnerGetCell( firstColumn + i, firstRow, iXFIndex );
        }
        else
        {
          range = InnerGetCell( firstColumn, firstRow + i, iXFIndex );
        }

        range.DateTime = arrDateTime[ i ];
      }

      return i;
    }


    /// <summary>
    /// Imports an array of objects into a worksheet.
    /// </summary>
    /// <param name="arrObject">Array to import.</param>
    /// <param name="firstRow">
    /// Row of the first cell where array should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where array should be imported.
    /// </param>
    /// <returns>Number of imported elements.</returns>
    public int ImportArray( object[ , ] arrObject, int firstRow, int firstColumn )
    {
      if( arrObject == null )
        throw new ArgumentNullException( "arrObject" );

      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentNullException( "firstColumn" );

      ParseData();

      int iMaxRows = Math.Min( firstRow + arrObject.GetLength( 0 ) - 1, m_book.MaxRowCount )
        - firstRow + 1;
      int iMaxColumns = Math.Min( firstColumn + arrObject.GetLength( 1 ) - 1, m_book.MaxColumnCount )
        - firstColumn + 1;

      //int iXFIndex  = m_book.DefaultXFIndex;
      IRange range;

      int[] arrXFIndex = new int[ iMaxColumns ];

      if( iMaxColumns > 0 && iMaxRows > 0 )
      {
        for( int i = 0; i < iMaxColumns; i++ )
        {
          range = InnerGetCell( i + firstColumn, firstRow );
          if (arrObject[0, i] == null)
          {
              range.Value2 = null;
          }
          else
          {
              if (arrObject[0, i].GetType() == typeof(string) && !CheckIsFormula(arrObject[0, i]) && this.IsStringsPreserved )
                  this.IsStringsPreserved = true;
              else
                  this.IsStringsPreserved = false;

              range.Value2 = arrObject[0, i];
          }
          RangeImpl rangeImpl = ( RangeImpl )range;
          arrXFIndex[ i ] = rangeImpl.ExtendedFormatIndex;
        }
      }
      else
      {
        return 0;
      }

      int iRow = 1;
      for( ; iRow < iMaxRows; iRow++ )
      {
        for( int iColumn = 0; iColumn < iMaxColumns; iColumn++ )
        {
          range = InnerGetCell( firstColumn + iColumn, iRow + firstRow, arrXFIndex[ iColumn ] );
          if (arrObject[iRow, iColumn] == null)
          {
              range.Value2 = null;
          }
          else
          {
              if (arrObject[iRow, iColumn].GetType() == typeof(string) && !CheckIsFormula(arrObject[iRow, iColumn]) && this.IsStringsPreserved)
                  this.IsStringsPreserved = true;
              else
                  this.IsStringsPreserved = false;

              range.Value2 = arrObject[iRow, iColumn];
          }
        }
      }

      return iRow;
    }

#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Imports data from a DataTable into worksheet
    /// </summary>
    /// <param name="dataTable">DataTable with desired data</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataTable should be imported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataTable should be imported
    /// </param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn )
    {
      return ImportDataTable( dataTable, isFieldNameShown, firstRow, firstColumn, -1, -1 );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet
    /// </summary>
    /// <param name="dataTable">DataTable with desired data</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataTable should be imported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataTable should be imported
    /// </param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, bool preserveTypes )
    {
      return ImportDataTable( dataTable, isFieldNameShown, firstRow, firstColumn, -1, -1, preserveTypes );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet
    /// </summary>
    /// <param name="dataTable">DataTable with desired data</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataTable should be imported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataTable should be imported
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import</param>
    /// <param name="maxColumns">Maximum number of columns to import</param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      return ImportDataTable( dataTable, isFieldNameShown, firstRow, firstColumn
        , maxRows, maxColumns, null, false );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet
    /// </summary>
    /// <param name="dataTable">DataTable with desired data</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataTable should be imported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataTable should be imported
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import</param>
    /// <param name="maxColumns">Maximum number of columns to import</param>
    /// <param name="preserveTypes">
    /// Indicates whether XlsIO should try to preserve types in DataTable,
    /// i.e. if it is set to False (default) and in DataTable we have in string column
    /// value that contains only numbers, it would be converted to number.
    /// </param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataTable( DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns, bool preserveTypes )
    {
      return ImportDataTable( dataTable, isFieldNameShown, firstRow, firstColumn
        , maxRows, maxColumns, null, preserveTypes );
    }
    /// <summary>
    /// Imports data from a DataTable into worksheet
    /// </summary>
    /// <param name="dataTable">DataTable with desired data</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataTable should be imported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataTable should be imported
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import</param>
    /// <param name="maxColumns">Maximum number of columns to import</param>
    /// <param name="arrColumns">Array of columns to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows</returns>
    public int ImportDataTable(DataTable dataTable, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns, DataColumn[] arrDataColumns
      , bool bPreserveTypes)
    {
        if (dataTable == null)
            throw new ArgumentNullException("dataTable");

        if (firstRow < 1 || firstRow > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("firstRow");

        if (firstColumn < 1 || firstColumn > m_book.MaxColumnCount)
            throw new ArgumentOutOfRangeException("firstColumn");

        ParseData();

        m_dicRecordsCells.UpdateRows(dataTable.Rows.Count);
        m_book.MaxImportColumns = dataTable.Columns.Count * 18; //18 bytes for each column

        int i = 0;

        if (arrDataColumns == null || arrDataColumns.Length == 0)
        {
            arrDataColumns = new DataColumn[dataTable.Columns.Count];
            dataTable.Columns.CopyTo(arrDataColumns, 0);
        }

        int tableRowsCount = dataTable.Rows.Count;
        if (maxRows < 0 || maxRows > tableRowsCount) maxRows = tableRowsCount;

        int tableColsCount = arrDataColumns.Length;
        if (maxColumns < 0 || maxColumns > tableColsCount) maxColumns = tableColsCount;

        maxColumns = Math.Min(maxColumns, m_book.MaxColumnCount - firstColumn + 1);
        maxRows = Math.Min(maxRows, m_book.MaxRowCount - firstRow);
        
        if (isFieldNameShown)
        {
            for (i = 0; i < maxColumns; i++)
            {
                SetText(firstRow, firstColumn + i, arrDataColumns[i].Caption);
            }
            firstRow++;
        }


        if (!bPreserveTypes)
        {
            ImportDataTableWithoutCheck(dataTable, firstRow, firstColumn,
              maxRows, maxColumns, arrDataColumns, m_bOptimizeImport);
        }
        else
        {
            ImportDataTableWithoutCheckPreserve(dataTable, firstRow, firstColumn,
              maxRows, maxColumns, arrDataColumns);
        }
        m_book.MaxImportColumns = 0;
        return maxRows;
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown, 0, 0 );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown
        , rowOffset, columnOffset, -1, -1 );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol )
    {
      return ImportDataTable( dataTable, namedRange, isFieldNameShown
        , rowOffset, columnOffset, iMaxRow, iMaxCol, false );
    }
    /// <summary>
    /// Imports data from a DataTable into namedRange.
    /// </summary>
    /// <param name="dataTable">DataTable with desired data.</param>
    /// <param name="namedRange">Represents named range.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="rowOffset">Represents row offset into named range to import.</param>
    /// <param name="columnOffset">Represents column offset into named range to import.</param>
    /// <param name="iMaxRow">Represents count of rows to import.</param>
    /// <param name="iMaxCol">Represents count of rows to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataTable( DataTable dataTable, IName namedRange, bool isFieldNameShown
      , int rowOffset, int columnOffset, int iMaxRow, int iMaxCol, bool bPreserveTypes )
    {
      if( dataTable == null )
        throw new ArgumentNullException( "dataTable" );

      if( namedRange == null )
        throw new ArgumentNullException( "namedRange" );

      if( rowOffset < 0 )
        throw new ArgumentOutOfRangeException( "rowOffset" );

      if( columnOffset < 0 )
        throw new ArgumentOutOfRangeException( "columnOffset" );

      ParseData();

      IRange range = namedRange.RefersToRange;

      if( !( range is RangeImpl ) )
        throw new NotSupportedException( "Doesnot support range collection as named range." );

      int iLength = dataTable.Rows.Count;
      if( iMaxRow < 0 || iMaxRow > iLength )
        iMaxRow = iLength;

      iLength = dataTable.Columns.Count;
      if( iMaxCol < 0 || iMaxCol > iLength )
        iMaxCol = iLength;

      int iColCount = range.LastColumn - range.Column + 1 - columnOffset - iMaxCol;
      int iRowCount = range.LastRow - range.Row + 1 - rowOffset - iMaxRow;

      if( isFieldNameShown )
        iRowCount--;

      if( iRowCount < 0 || iColCount < 0 )
        throw new NotSupportedException( "Bounds of data table is greatfull than bounds of named range." );

      WorksheetImpl sheet = ( WorksheetImpl )range.Worksheet;

      DataColumn[] arrColumns = new DataColumn[ iLength ];
      dataTable.Columns.CopyTo( arrColumns, 0 );

      if( isFieldNameShown )
      {
        for( int i = 0; i < iMaxCol; i++ )
        {
          range[ range.Row + rowOffset, range.Column + columnOffset + i ].Value2 = arrColumns[ i ].Caption;
        }

        rowOffset++;
      }

      if( bPreserveTypes )
      {
        sheet.ImportDataTableWithoutCheckPreserve( dataTable, range.Row + rowOffset, range.Column + columnOffset
          , iMaxRow, iMaxCol, arrColumns );
      }
      else
      {
        sheet.ImportDataTableWithoutCheck( dataTable, range.Row + rowOffset, range.Column + columnOffset
          , iMaxRow, iMaxCol, arrColumns, m_bOptimizeImport);
      }

      return iMaxRow;
    }
    /// <summary>
    /// Imports data column.
    /// </summary>
    /// <param name="dataColumn">Data column to import.</param>
    /// <param name="isFieldNameShown">Indicates whether to import field names.</param>
    /// <param name="firstRow">Index of the first row.</param>
    /// <param name="firstColumn">Index of the first column</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataColumn( DataColumn dataColumn, bool isFieldNameShown
      , int firstRow, int firstColumn )
    {
      if( dataColumn == null )
        throw new ArgumentNullException( "dataColumn" );

      return ImportDataColumns( new DataColumn[] { dataColumn }, isFieldNameShown
        , firstRow, firstColumn );
    }
    /// <summary>
    /// Imports array of data columns.
    /// </summary>
    /// <param name="arrDataColumns">Data columns to import.</param>
    /// <param name="isFieldNameShown">Indicates whether to import field names.</param>
    /// <param name="firstRow">Index to the first row.</param>
    /// <param name="firstColumn">Index to the first column.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataColumns( DataColumn[] arrDataColumns, bool isFieldNameShown
      , int firstRow, int firstColumn )
    {
      if( arrDataColumns == null )
        throw new ArgumentNullException( "arrDataColumns" );

      if( arrDataColumns.Length == 0 )
        throw new ArgumentException( "arrDataColumns can't be empty" );

      return ImportDataTable( arrDataColumns[ 0 ].Table, isFieldNameShown, firstRow, firstColumn
        , -1, -1, arrDataColumns, false );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn )
    {
      return ImportDataView( dataView, isFieldNameShown, firstRow, firstColumn, false );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn, bool bPreserveTypes )
    {
      if( dataView == null )
        throw new ArgumentNullException( "dataView" );

      return ImportDataView( dataView, isFieldNameShown, firstRow, firstColumn,
        dataView.Count, dataView.Table.Columns.Count, bPreserveTypes );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( DataView dataView, bool isFieldNameShown
      , int firstRow, int firstColumn, int maxRows, int maxColumns )
    {
      return ImportDataView( dataView, isFieldNameShown, firstRow, firstColumn,
        maxRows, maxColumns, false );
    }
    /// <summary>
    /// Imports data from a DataView into worksheet.
    /// </summary>
    /// <param name="dataView">DataView with desired data.</param>
    /// <param name="isFieldNameShown">TRUE if column names must also be imported.</param>
    /// <param name="firstRow">
    /// Row of the first cell where DataView should be imported.
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell where DataView should be imported.
    /// </param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="bPreserveTypes">Indicates whether to preserve column types.</param>
    /// <returns>Number of imported rows.</returns>
    public int ImportDataView( DataView dataView, bool isFieldNameShown,
      int firstRow, int firstColumn, int maxRows, int maxColumns,
      bool bPreserveTypes )
    {
      if( dataView == null )
        throw new ArgumentNullException( "dataView" );

      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "firstColumn" );

      ParseData();

      int i = 0;

      DataColumnCollection arrColumns = dataView.Table.Columns;

      int iLength = dataView.Count;

      if( maxRows < 0 || maxRows > iLength )
        maxRows = iLength;

      iLength = arrColumns.Count;

      if( maxColumns < 0 || maxColumns > iLength )
        maxColumns = iLength;

      maxColumns = Math.Min( maxColumns, m_book.MaxColumnCount - firstColumn + 1 );

      if( isFieldNameShown )
      {
        for( i = 0; i < maxColumns; i++ )
        {
          Range[ firstRow, firstColumn + i ].Value2 = arrColumns[ i ].Caption;
        }

        firstRow++;
      }

      maxRows = Math.Min( maxRows, m_book.MaxRowCount - firstRow + 1 );

      if( !bPreserveTypes )
      {
        ImportDataViewWithoutCheck( dataView, firstRow, firstColumn,
          maxRows, maxColumns );
      }
      else
      {
        ImportDataViewWithoutCheckPreserve( dataView, firstRow, firstColumn,
          maxRows, maxColumns );
      }

      return maxRows;
    }
    /// <summary>
    /// Exports worksheet data into a DataTable
    /// </summary>
    /// <param name="firstRow">
    /// Row of the first cell from where DataTable should be exported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell from where DataTable should be exported
    /// </param>
    /// <param name="maxRows">Maximum number of rows to export</param>
    /// <param name="maxColumns">Maximum number of columns to export</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data</returns>
    public DataTable ExportDataTable( int firstRow, int firstColumn
      , int maxRows, int maxColumns, ExcelExportDataTableOptions options )
    {
      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "firstColumn" );

      ParseData();

      bool bExportColumnNames = ( ( options & ExcelExportDataTableOptions.ColumnNames ) != 0 );
      bool bExportFormulaValues = ( ( options & ExcelExportDataTableOptions.ComputedFormulaValues ) != 0 );
      bool bDetectTypes = ( ( options & ExcelExportDataTableOptions.DetectColumnTypes ) != 0 );
      bool bUseDefaultStyles = ( ( options & ExcelExportDataTableOptions.DefaultStyleColumnTypes ) != 0 );
      bool preserveOLEDate = ( ( options & ExcelExportDataTableOptions.PreserveOleDate ) != 0 );

      DataTable result = new DataTable( this.Name );

      // Declare DataColumn and DataRow variables.
      DataColumn dataColumn;
      DataRow dataRow;

      maxColumns = Math.Min( maxColumns, m_book.MaxColumnCount - firstColumn + 1 );
      maxRows = Math.Min( maxRows + ( ( bExportColumnNames ) ? 1 : 0 ), m_book.MaxRowCount - firstRow
        + ( ( bExportColumnNames ) ? 2 : 1 ) );

      ExcelExportType[] arrFormatType = bDetectTypes ? new ExcelExportType[ maxColumns ] : null;
      int iFirstDataRow = bExportColumnNames ? firstRow + 1 : firstRow;

      for( int i = 0; i < maxColumns; i++ )
      {
        dataColumn = new DataColumn();
        Type dataType = typeof( string );

            if (bDetectTypes)
            {
				Type formulaDataType;
                ExcelFormatType formatType = GetFormatType(iFirstDataRow, firstColumn + i, bUseDefaultStyles);
                ExcelExportType exportType = GetExportType(formatType, iFirstDataRow, firstColumn + i, maxRows, options, out formulaDataType);
                dataType = GetType(exportType, preserveOLEDate);
                arrFormatType[i] = exportType;
            }

        dataColumn.DataType = dataType;

        IRange range = Range[ firstRow, firstColumn + i ];
        if (result.Columns.Contains(range.Value))
            dataColumn.ColumnName = range.Value + i;
        else if( bExportColumnNames )
          dataColumn.ColumnName = range.Value;

        result.Columns.Add( dataColumn );
      }

      firstRow = iFirstDataRow;

      if( bExportColumnNames ) maxRows--;

      //IMigrantRange migrantRange = new MigrantRangeImpl( Application, this );
      this.EnableSheetCalculations();
      this.CalcEngine.UseNoAmpersandQuotes = true;
      for( int i = 0; i < maxRows; i++ )
      {
        dataRow = result.NewRow();
        if (Range[firstRow + i, firstColumn].RowHeight > 0)
        {
            for (int j = 0; j < maxColumns; j++)
            {
                ExcelExportType exportType = (arrFormatType != null)
                  ? arrFormatType[j]
                  : ExcelExportType.Text;

                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, i, "Current Row" );
                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, j, "Current Column" );
                dataRow[j] = GetValue(firstRow + i, firstColumn + j, exportType, bExportFormulaValues, preserveOLEDate);
            }


            result.Rows.Add(dataRow);
        }
      }
      //this.DisableSheetCalculations();
      return result;
    }

    /// <summary>
    /// Exports worksheet data into a DataTable
    /// </summary>
    /// <param name="firstRow">
    /// Row of the first cell from where DataTable should be exported
    /// </param>
    /// <param name="firstColumn">
    /// Column of the first cell from where DataTable should be exported
    /// </param>
    /// <param name="maxRows">Maximum number of rows to export</param>
    /// <param name="maxColumns">Maximum number of columns to export</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data</returns>
    public DataTable ExportDataTable(int firstRow, int firstColumn
     , int maxRows, int maxColumns, ExcelExportDataTableOptions options, PivotTableImpl pivotTable)
    {
        if (firstRow < 1 || firstRow > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("firstRow");

        if (firstColumn < 1 || firstColumn > m_book.MaxColumnCount)
            throw new ArgumentOutOfRangeException("firstColumn");

        ParseData();

        bool bExportColumnNames = ((options & ExcelExportDataTableOptions.ColumnNames) != 0);
        bool bExportFormulaValues = ((options & ExcelExportDataTableOptions.ComputedFormulaValues) != 0);
        bool bDetectTypes = ((options & ExcelExportDataTableOptions.DetectColumnTypes) != 0);
        bool bUseDefaultStyles = ((options & ExcelExportDataTableOptions.DefaultStyleColumnTypes) != 0);
        bool preserveOLEDate = ((options & ExcelExportDataTableOptions.PreserveOleDate) != 0);

        DataTable result = new DataTable(this.Name);

        // Declare DataColumn and DataRow variables.
        DataColumn dataColumn;
        DataRow dataRow;
        List<string> checkingList = new List<string>();
        maxColumns = Math.Min(maxColumns, m_book.MaxColumnCount - firstColumn + 1);
        maxRows = Math.Min(maxRows + ((bExportColumnNames) ? 1 : 0), m_book.MaxRowCount - firstRow
          + ((bExportColumnNames) ? 2 : 1));

        ExcelExportType[] arrFormatType = bDetectTypes ? new ExcelExportType[maxColumns] : null;
        int iFirstDataRow = bExportColumnNames ? firstRow + 1 : firstRow;

        for (int i = 0; i < maxColumns; i++)
        {
            dataColumn = new DataColumn();
            Type dataType = typeof(string);

            if (bDetectTypes)
            {
                Type formulaDataType;
                ExcelFormatType formatType = GetFormatType(iFirstDataRow, firstColumn + i, bUseDefaultStyles);
                ExcelExportType exportType = GetExportType(formatType, iFirstDataRow, firstColumn + i, maxRows, options, out formulaDataType);
                dataType = GetType(exportType, preserveOLEDate);
                arrFormatType[i] = exportType;
            }

            dataColumn.DataType = dataType;

            if (bExportColumnNames)
            {
                dataColumn.ColumnName = Range[firstRow, firstColumn + i].Value;
                int incrementColumnName = 0;
                string originalName = dataColumn.ColumnName;
                while (checkingList.Contains(dataColumn.ColumnName))
                {
                    incrementColumnName++;
                    dataColumn.ColumnName = originalName + incrementColumnName.ToString();
                }
                checkingList.Add(dataColumn.ColumnName);
            }

            result.Columns.Add(dataColumn);
        }
        for (int dataFieldsCount = 0; dataFieldsCount < pivotTable.DataFields.Count; dataFieldsCount++)
        {
            if (pivotTable.DataFields[dataFieldsCount].Subtotal == PivotSubtotalTypes.Count)
            {
                int cacheIndex = pivotTable.DataFields[dataFieldsCount].Field.CacheField.Index;
                if (result.Columns[cacheIndex].DataType == typeof(double))
                    result.Columns[cacheIndex].DataType = typeof(int);
            }
        }
        firstRow = iFirstDataRow;

        if (bExportColumnNames) maxRows--;

        //IMigrantRange migrantRange = new MigrantRangeImpl( Application, this );

        for (int i = 0; i < maxRows; i++)
        {
            dataRow = result.NewRow();

            for (int j = 0; j < maxColumns; j++)
            {
                ExcelExportType exportType = (arrFormatType != null)
                  ? arrFormatType[j]
                  : ExcelExportType.Text;

                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, i, "Current Row" );
                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, j, "Current Column" );
                dataRow[j] = GetValue(firstRow + i, firstColumn + j, exportType, bExportFormulaValues, preserveOLEDate);
            }

            result.Rows.Add(dataRow);
        }

        return result;
    }
    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="range">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public DataTable ExportDataTable( IRange range, ExcelExportDataTableOptions options )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      bool bExportColumnNames = ( ( options & ExcelExportDataTableOptions.ColumnNames ) != 0 );
      int increment = bExportColumnNames ? 0 : 1;

      int iRow = range.Row;
      int iColumn = range.Column;

      if( iRow == 0 || iColumn == 0 )
        return null;

      return ExportDataTable( iRow, iColumn, range.LastRow - iRow + increment
        , range.LastColumn - iColumn + 1, options );
    }

    /// <summary>
    /// Exports worksheet data into a DataTable.
    /// </summary>
    /// <param name="range">Range to export.</param>
    /// <param name="options">Export options.</param>
    /// <returns>DataTable with worksheet data.</returns>
    public DataTable PEExportDataTable(IRange range, ExcelExportDataTableOptions options,PivotTableImpl pivotTable)
    {
        if (range == null)
            throw new ArgumentNullException("range");

        bool bExportColumnNames = ((options & ExcelExportDataTableOptions.ColumnNames) != 0);
        int increment = bExportColumnNames ? 0 : 1;

        int iRow = range.Row;
        int iColumn = range.Column;

        if (iRow == 0 || iColumn == 0)
            return null;

        return ExportDataTable(iRow, iColumn, range.LastRow - iRow + increment
          , range.LastColumn - iColumn + 1, options,pivotTable);
    }
#endif
    #region CustomObject Implementation
    public int ImportData(IEnumerable arrObject, int firstRow, int firstColumn, bool includeHeader)
    {
        if( arrObject == null )
        throw new ArgumentNullException( "arrObject" );

      if( firstRow < 1 || firstRow > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "firstRow" );

      if( firstColumn < 1 || firstColumn > m_book.MaxColumnCount )
        throw new ArgumentNullException( "firstColumn" );

        IEnumerator valueEnum = arrObject.GetEnumerator();
        if (valueEnum == null)
            return 0;

        bool isCustomObject = false;
        List<PropertyInfo> propertyInfoCollection = null;
        List<TypeCode> propertyTypeCodeCollection = null;
        object obj;
        int i = 0;

        valueEnum.MoveNext();
        obj = valueEnum.Current;
        if (obj == null)
            return 0;
        Type objType = obj.GetType();

        if (objType.Namespace == null || (objType.Namespace != null && !objType.Namespace.Contains("System")))
        {
            isCustomObject = true;
        }
        if (!isCustomObject)
            return 0;

        propertyTypeCodeCollection = GetObjectMembersInfo(obj, out propertyInfoCollection);
        if (includeHeader)
        {
            for (int j = 0; j < propertyInfoCollection.Count; j++)
            {
                this.SetText(firstRow, firstColumn + j, propertyInfoCollection[j].Name);
            }
            firstRow++;
        }
        IMigrantRange migrantRange = this.MigrantRange;
        while (valueEnum.MoveNext())
        {
            obj = valueEnum.Current;
            if (obj == null)
                continue;
            for (int j = 0; j < propertyInfoCollection.Count; j++)
            {
                PropertyInfo propertyInfo = propertyInfoCollection[j];
                migrantRange.ResetRowColumn(firstRow + i, firstColumn + j);
                switch (propertyTypeCodeCollection[j])
                {
                    case TypeCode.String:
                        string value = (string)GetValueFromProperty(obj, propertyInfo);
                        if (value == null || value.Length == 0)
                            continue;
                        migrantRange.SetValue(value);
                        break;
                    case TypeCode.Int32:
                        migrantRange.SetValue((int)GetValueFromProperty(obj, propertyInfo));
                        break;
                    case TypeCode.Int16:
                        migrantRange.SetValue(Convert.ToInt16(GetValueFromProperty(obj, propertyInfo)));
                        break;
                    case TypeCode.Double:
                        migrantRange.SetValue((double)GetValueFromProperty(obj, propertyInfo));
                        break;
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                        migrantRange.SetValue(Convert.ToDouble(GetValueFromProperty(obj, propertyInfo)));
                        break;
                    case TypeCode.Boolean:
                        migrantRange.SetValue((bool)GetValueFromProperty(obj, propertyInfo));
                        break;
                    case TypeCode.DateTime:
                        migrantRange.SetValue((DateTime)GetValueFromProperty(obj, propertyInfo));
                        break;
                    default:
                        migrantRange.SetValue(GetValueFromProperty(obj, propertyInfo).ToString());
                        break;
                }
            }
            i++;
        }


        return i;
    }
    private List<TypeCode> GetObjectMembersInfo(object obj, out List<PropertyInfo> propertyInfo)
    {
        Type type = obj.GetType();
        List<TypeCode> propertyTypeCode = new List<TypeCode>();
        propertyInfo = new List<PropertyInfo>();

        PropertyInfo[] propertiesInfo = type.GetProperties();
        foreach (PropertyInfo property in propertiesInfo)
        {
            propertyInfo.Add(property);
#if WINRT
            propertyTypeCode.Add(GetTypeCode(property.PropertyType));
#else
            propertyTypeCode.Add(System.Type.GetTypeCode(property.PropertyType));
#endif
        }
        return propertyTypeCode;
    }
    private object GetValueFromProperty(object value, PropertyInfo strProperty)
    {

        if (strProperty == null)
        {
            throw new ArgumentOutOfRangeException("Can't find property");
        }

        value = strProperty.GetValue(value, null);

        return value;
    }

    #if WINRT
    private static readonly Dictionary<Type, TypeCode> _typeCodeTable =
          new Dictionary<Type, TypeCode>()
            {
                { typeof( Boolean ), TypeCode.Boolean },
                { typeof( Char ), TypeCode.Char },
                { typeof( Byte ), TypeCode.Byte },
                { typeof( Int16 ), TypeCode.Int16 },
                { typeof( Int32 ), TypeCode.Int32 },
                { typeof( Int64 ), TypeCode.Int64 },
                { typeof( SByte ), TypeCode.SByte },
                { typeof( UInt16 ), TypeCode.UInt16 },
                { typeof( UInt32 ), TypeCode.UInt32 },
                { typeof( UInt64 ), TypeCode.UInt64 },
                { typeof( Single ), TypeCode.Single },
                { typeof( Double ), TypeCode.Double },
                { typeof( DateTime ), TypeCode.DateTime },
                { typeof( Decimal ), TypeCode.Decimal },
                { typeof( String ), TypeCode.String },
            };

      internal static TypeCode GetTypeCode(Type type)
      {
          if (type == null)
          {
              return TypeCode.Empty;
          }

          TypeCode result;
          if (!_typeCodeTable.TryGetValue(type, out result))
          {
              result = TypeCode.Object;
          }

          return result;
      }
      internal enum TypeCode
      {
          Byte,
          Int16,
          Int32,
          Int64,
          SByte,
          UInt16,
          UInt32,
          UInt64,
          Single,
          Double,
          Char,
          Boolean,
          String,
          DateTime,
          Decimal,
          Empty,
          DBNull, // Never used
          Object
      }
    #endif

    #endregion

    /// <summary>
    /// Removes panes from a worksheet.
    /// </summary>
    public void RemovePanes()
    {
      ParseData();

      WindowTwo.IsFreezePanes = false;
      WindowTwo.IsFreezePanesNoSplit = false;
      m_pane = null;
    }
    /// <summary>
    /// Intersects two ranges.
    /// </summary>
    /// <param name="range1">First range to intersect.</param>
    /// <param name="range2">Second range to intersect.</param>
    /// <returns>Intersection of two ranges or null if there is no ranges intersection.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is null.
    /// </exception>
    public IRange IntersectRanges( IRange range1, IRange range2 )
    {
      if( range1 == null )
        throw new ArgumentNullException( "range1" );

      if( range1 == null )
        throw new ArgumentNullException( "range2" );

      if( range1.Parent != range2.Parent ) return null;

      Rectangle rect1 = Rectangle.FromLTRB( range1.Column, range1.Row
        , range1.LastColumn, range1.LastRow );

      Rectangle rect2 = Rectangle.FromLTRB( range2.Column, range2.Row
        , range2.LastColumn, range2.LastRow );

      Rectangle rectResult = Rectangle.Intersect( rect1, rect2 );

      if( rectResult == Rectangle.Empty ) return null;

      return range1[ rectResult.Top, rectResult.Left, rectResult.Bottom, rectResult.Right ];
    }

    /// <summary>
    /// Merges two ranges.
    /// </summary>
    /// <param name="range1">First range to merge.</param>
    /// <param name="range2">Second range to merge.</param>
    /// <returns>Merged ranges or null if wasn't able to merge ranges.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When range1 or range2 is null.
    /// </exception>
    public IRange MergeRanges( IRange range1, IRange range2 )
    {
      if( range1 == null )
        throw new ArgumentNullException( "range1" );

      if( range2 == null )
        throw new ArgumentNullException( "range2" );

      if( range1.Parent != range2.Parent ) return null;

      int width1 = range1.LastColumn - range1.Column + 1;
      int width2 = range2.LastColumn - range2.Column + 1;

      int height1 = range1.LastRow - range1.Row + 1;
      int height2 = range2.LastRow - range2.Row + 1;

      if( width1 != width2 && height1 != height2 ) return null;
      IRange temp;

      if( width1 == width2 && range1.Column == range2.Column )
      {
        if( range2.Row < range1.Row ) // sort ranges by row
        {
          temp = range1;
          range1 = range2;
          range2 = temp;
        }

        if( range2.Row >= range1.Row && range2.Row <= range1.LastRow + 1 )
        {
          return range1[ range1.Row, range1.Column, Math.Max( range1.LastRow,
            range2.LastRow ), range1.LastColumn ];
        }
      }

      if( height1 == height2 && range1.Row == range2.Row )
      {
        if( range2.Column < range1.Column ) // sort ranges by column
        {
          temp = range1;
          range1 = range2;
          range2 = temp;
        }

        if( range2.Column >= range1.Column && range2.Column <= range1.LastColumn + 1 )
        {
          return range1[ range1.Row, range1.Column, range1.LastRow, Math.Max(
            range1.LastColumn, range2.LastColumn ) ];
        }
      }

      return null;
    }
    /// <summary>
    /// Find a range with the given value.
    /// </summary>
    /// <param name="value">Value to find.</param>
    /// <returns>Range array that contains the given value.</returns>
    private IRange[] Find( string value )
    {
      ParseData();

      Dictionary<int, object> dictIndexes = m_book.InnerSST.GetStringIndexes( value );
      return ConvertCellListIntoRange( m_dicRecordsCells.Find( dictIndexes ) );
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, string newValue )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        IRange range = arrRanges[ i ];
        string cellValue=range.Text.ToLower();
          oldValue=oldValue.ToLower();
         range.Text=cellValue.Replace(oldValue, newValue);
      }
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, DateTime newValue )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        IRange range = arrRanges[ i ];
        range.DateTime = newValue;
      }
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, double newValue )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        IRange range = arrRanges[ i ];
        range.Number = newValue;
      }
    }

    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, string[] newValues, bool isVertical )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        RangeImpl range = ( RangeImpl )arrRanges[ i ];
        range.Replace( oldValue, newValues, isVertical );
      }
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, int[] newValues, bool isVertical )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        RangeImpl range = ( RangeImpl )arrRanges[ i ];
        range.Replace( oldValue, newValues, isVertical );
      }
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, double[] newValues, bool isVertical )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        RangeImpl range = ( RangeImpl )arrRanges[ i ];
        range.Replace( oldValue, newValues, isVertical );
      }
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, DataTable newValues, bool isFieldNamesShown )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        RangeImpl range = ( RangeImpl )arrRanges[ i ];
        range.Replace( oldValue, newValues, isFieldNamesShown );
      }
    }
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="column">Data column with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    /// <remarks>
    /// This can be long operation (needs iteration through all cells
    /// in the worksheet). Better use named ranges instead and call
    /// Import function instead of placeholders.
    /// </remarks>
    public void Replace( string oldValue, DataColumn column, bool isFieldNamesShown )
    {
      IRange[] arrRanges = Find( oldValue );
      int iRangesLength = ( arrRanges != null ) ?
        arrRanges.Length :
        0;

      for( int i = 0; i < iRangesLength; i++ )
      {
        RangeImpl range = ( RangeImpl )arrRanges[ i ];
        range.Replace( oldValue, column, isFieldNamesShown );
      }
    }
#endif

    /// <summary>
    /// Removes worksheet from parent worksheets collection.
    /// </summary>
    public void Remove()
    {
      ParseData();

      if( m_dataValidation != null ) m_dataValidation.Clear();
      if( m_arrConditionalFormats != null ) m_arrConditionalFormats.Clear();
      if (m_pivotTables != null)
      {
          m_pivotTables.Clear();
          m_book.RemoveUnusedCaches();
      }
      m_book.InnerWorksheets.InnerRemove( Index );
      m_names.Clear();
        if (m_listObjects!=null && m_listObjects.Count > 0)
      {
          for (int i = 0; i < m_listObjects.Count; i++)
          {
              if (m_listObjects[i].TableType == ExcelTableType.queryTable)
                  m_book.Connections.Remove(m_listObjects[i].QueryTable.ExternalConnection);
          }
      }
      if (m_listObjects!=null)
      m_listObjects.Dispose();
      Dispose();
    }

    /// <summary>
    /// Moves worksheet into new position.
    /// </summary>
    /// <param name="iNewIndex">
    /// New index in the workbook's objects collection.
    /// IT IS NOT ALWAYS INDEX IN WOKRSHEETS COLLECTION.
    /// </param>
    public void Move( int iNewIndex )
    {
      int iOldIndex = RealIndex;
      int iNewIndexInWorksheets = FindWorksheetNotBefore( iNewIndex );

      m_book.Objects.Move( iOldIndex, iNewIndex );

      WorksheetsCollection worksheets = m_book.InnerWorksheets;
      worksheets.Move( Index, iNewIndexInWorksheets );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iNewIndex"></param>
    private int FindWorksheetNotBefore( int iNewIndex )
    {
      for( int i = iNewIndex, len = m_book.ObjectCount; i < len; i++ )
      {
        IWorksheet sheet = m_book.Objects[ i ] as IWorksheet;

        if( sheet != null )
        {
          return sheet.Index;
        }
      }

      IWorksheets worksheets = m_book.Worksheets;
      return worksheets.Count - 1;
    }
    /// <summary>
    /// Sets column width.
    /// </summary>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="value">Width to set.</param>
    public void SetColumnWidth( int iColumn, double value )
    {
      if( iColumn < 1 || iColumn > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "Column",
          "Column index cannot be larger then 256 or less then one" );

      double iOldValue = InnerGetColumnWidth( iColumn );

      if( iOldValue != value )
      {
        ColumnInfoRecord colInfo = ( ColumnInfoRecord )m_arrColumnInfo[ iColumn ];

        if( colInfo == null )
        {
          colInfo = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
          colInfo.FirstColumn = colInfo.LastColumn = ( ushort )( iColumn - 1 );
          colInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
          colInfo.ColumnWidth = ( ushort )( Application.StandardWidth * 256.0 );
          m_arrColumnInfo[ iColumn ] = colInfo;
        }

        if( value == 0 )
        {
          colInfo.IsHidden = true;
        }
        else
        {
          if (value > DEF_MAX_COLUMN_WIDTH)
              value = DEF_MAX_COLUMN_WIDTH;
          colInfo.ColumnWidth = ( ushort )( value * 256 );
          WorksheetHelper.AccessColumn( this, iColumn );

          RaiseColumnWidthChangedEvent( iColumn, value );
        }

        SetChanged();
      }
    }
    /// <summary>
    /// Sets column width in pixels.
    /// </summary>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="value">Width in pixels to set.</param>
    public void SetColumnWidthInPixels( int iColumn, int value )
    {
      ParseData();

      double dColumnWidth = PixelsToColumnWidth( value );
      SetColumnWidth( iColumn, dColumnWidth );
    }
      /// <summary>
    /// Set Column Width from Start Column index and End Column index
      /// </summary>
    /// <param name="iStartColumnIndex">Start Column index</param>
    /// <param name="iCount">No of Column to be set width</param>
      /// <param name="value">Value to set</param>
    public void SetColumnWidthInPixels(int iStartColumnIndex, int iCount, int value)
    {
        ParseData();

        double dColumnWidth = PixelsToColumnWidth(value);
        for(int i=0;i<iCount ;i++)
        SetColumnWidth(iStartColumnIndex++, dColumnWidth);
    }
    /// <summary>
    /// Sets row height.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="value">Height to set.</param>
    public void SetRowHeight( int iRow, double value )
    {
      InnerSetRowHeight( iRow, value, true, MeasureUnits.Point, true );
      //      IRange range = this[ iRow, m_iFirstColumn ];
      //      SetRowHeightInPixels( iRow, value );
    }
    /// <summary>
    /// Sets row height in pixels.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to set height.</param>
    /// <param name="value">Value in pixels to set.</param>
    public void SetRowHeightInPixels( int iRowIndex, double value )
    {
      if( iRowIndex < 1 || iRowIndex > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex", "Value cannot be less 1 and greater than max row index." );

      if( value < 0 )
        throw new ArgumentOutOfRangeException( "value" );

      InnerSetRowHeight( iRowIndex, value, true, MeasureUnits.Pixel, true );
    }
      /// <summary>
    /// Set Row height from Start Row index to End Row index
      /// </summary>
    /// <param name="iStartRowIndex">Start Row index</param>
    /// <param name="iCount">No. of rows</param>
      /// <param name="value">Value in pixels to set</param>
    public void SetRowHeightInPixels(int iStartRowIndex, int iCount, double value)
    {
        if (iStartRowIndex < 1 || iStartRowIndex > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("Row Index", "value cannot be less than 1 and greater than max row index");
        if ((iStartRowIndex + iCount) > m_book.MaxRowCount)
            throw new ArgumentOutOfRangeException("End Row Index","Value cannot be greater than max row index");
        if (value < 0)
            throw new ArgumentOutOfRangeException("value");
        for(int i=0;i<iCount ;i++)
        {
            InnerSetRowHeight(iStartRowIndex++, value, true, MeasureUnits.Pixel, true);
        }
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
        return FindFirst(findValue, flags, ExcelFindOptions.None);
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags)
    {
        return FindStringStartsWith(findValue, flags, false);
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        this.m_book.IsStartsOrEndsWith = true;
        ExcelFindOptions findOptions = ignoreCase ?
            ExcelFindOptions.None :
            ExcelFindOptions.MatchCase;
        return FindFirst( findValue,  flags, findOptions);
    }
    /// <summary>
    /// This method searches for the first cell that ends  with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringEndsWith(string findValue, ExcelFindType flags)
    {
        return FindStringEndsWith(findValue, flags,false);
    }
    /// <summary>
    /// This method searches for the first cell that ends with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringEndsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        this.m_book.IsStartsOrEndsWith = false;
        ExcelFindOptions findOptions = ignoreCase ?
            ExcelFindOptions.None :
            ExcelFindOptions.MatchCase;
        return FindFirst(findValue, flags, findOptions);
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst(string findValue, ExcelFindType flags,ExcelFindOptions findOptions)
    {
        IRange[] arrResult = Find(UsedRange, findValue, flags,findOptions, true);
        return (arrResult != null)
          ? arrResult[0]
          : null;
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      IRange[] arrResult = Find( UsedRange, findValue, flags, true );
      return ( arrResult != null )
        ? arrResult[ 0 ]
        : null;
    }
    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      IRange[] arrResult = Find( UsedRange, ( byte )( findValue ? 1 : 0 ), false, true );
      return ( arrResult != null )
        ? arrResult[ 0 ]
        : null;
    }
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      double dNumber = UtilityMethods.ConvertDateTimeToNumber( findValue );
      IRange[] arrResult = Find( UsedRange, dNumber, ExcelFindType.Number, true );
      return ( arrResult != null )
        ? arrResult[ 0 ]
        : null;
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      double value = findValue.Days + ( findValue.Hours * 360000 + findValue.Minutes * 6000
        + findValue.Seconds * 100 + findValue.Milliseconds ) / ( 24.0 * 360000 );
      IRange[] arrResult = Find( UsedRange, value, ExcelFindType.Number, true );
      return ( arrResult != null )
        ? arrResult[ 0 ]
        : null;
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
        return FindAll(findValue, flags, ExcelFindOptions.None);
    }
    /// <summary>
    /// This method searches for the all cells with specified string value based on the Excel find options.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search.</param>
    /// <returns>
    /// All found cells, or Null if value was not found.
    /// </returns>
    public IRange[] FindAll(string findValue, ExcelFindType flags, ExcelFindOptions findOptions)
    {
        if (findValue == null || findValue.Length == 0) return null;

        return Find(UsedRange, findValue, flags,findOptions, false);
    }
    ///<summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flags is not valid.", "flags" );

      return Find( UsedRange, findValue, flags, false );
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    public IRange[] FindAll( bool findValue )
    {
      return Find( UsedRange, ( byte )( findValue ? 1 : 0 ), false, false );
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      double iFindValue = UtilityMethods.ConvertDateTimeToNumber( findValue );

      return FindAll( iFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      double iFindValue = findValue.TotalDays;
      return FindAll( iFindValue, ExcelFindType.FormulaValue | ExcelFindType.Number );
    }
#if !(WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( string fileName, string separator )
    {
      SaveAs( fileName, separator, Encoding.Unicode );
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="fileName">File to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs( string fileName, string separator, Encoding encoding )
    {
      if( fileName == null )
        throw new ArgumentNullException( "Filename" );

      if( separator == null || separator == string.Empty )
        throw new ArgumentNullException( "separator" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "FileName cannot be empty." );

      string tmpFullPath = Path.GetFullPath( fileName );
      string dir = Path.GetDirectoryName( tmpFullPath );

#if !SILVERLIGHT && !WINRT && !WP
      if( File.Exists( tmpFullPath ) )
      {
        FileAttributes attrib = File.GetAttributes( tmpFullPath );

        if( ( attrib & FileAttributes.ReadOnly ) != 0 )
        {
          //RaiseReadOnlyFileEvent( tmpFullPath );
          throw new IOException( "Cannot save. File is readonly." );
        }
      }
#endif

      // Create directory if it does not exist.
      if( dir != null && dir.Length > 0 && !Directory.Exists( dir ) )
      {
        Directory.CreateDirectory( dir );
      }

      using( FileStream streamToWrite = new FileStream( tmpFullPath, FileMode.Create ) )
      {
        SaveAs( streamToWrite, separator, encoding );
        streamToWrite.Close();
      }
    }
#endif
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save.</param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( Stream stream, string separator )
    {
#if ( WINRT )
        SaveAs(stream, separator, Encoding.UTF8);
#else
      SaveAs( stream, separator, Encoding.Unicode );
#endif
    }
#if ( WINRT )
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="storageFile">Storage file to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <returns></returns>
    public Task<bool> SaveAsAsync(StorageFile storageFile, string separator)
    {
        return SaveAsAsync(storageFile, separator, Encoding.UTF8);
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save.</param>
    /// <param name="separator">Current separator.</param>
    public Task<bool> SaveAsAsync(Stream stream, string separator)
    {
        return SaveAsAsync(stream, separator, Encoding.UTF8);
    }
#endif
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAsInternal( Stream stream, string separator, Encoding encoding )
    {

      ParseData();

      StreamWriter streamToWrite = new StreamWriter( stream, encoding );

      //stream writing
      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        if( !IsRowEmpty( i, false ) )
        {
          for( int j = m_iFirstColumn; j <= m_iLastColumn; j++ )
          {
            long lCellIndex = RangeImpl.GetCellIndex( j, i );
            TRangeValueType valueType = m_dicRecordsCells.GetCellType( i, j );
            string result = string.Empty;

            if( valueType != TRangeValueType.Blank )
            {
                result = m_dicRecordsCells.GetValue(lCellIndex, i, j, Range, separator);

              streamToWrite.Write( result );
            }

            if( j != m_iLastColumn )
              streamToWrite.Write( separator );
          }
        }

        streamToWrite.WriteLine();
      }

      streamToWrite.Flush();
      stream.Flush();
      //streamToWrite.Close();
    }
    /// <summary>
    /// Save tabsheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="encoding">Encoding to use.</param>
    public void SaveAs(Stream stream, string separator, Encoding encoding)
    {
        if (stream == null) throw new ArgumentException("stream");

        if (separator == null || separator.Length == 0)
            throw new ArgumentException("separator");
        SaveAsInternal(stream, separator, encoding);
    }
#if ( WINRT )
    public async Task<bool> SaveAsAsync(StorageFile storageFile, string separator, Encoding encoding)
    {
        Stream stream = await storageFile.OpenStreamForWriteAsync();
        stream.Position = 0;
        stream.SetLength(0);
        
        bool task = await SaveAsAsync(stream, separator,encoding);
        stream.Flush();
        stream.Dispose();
        return task;
    }

       public Task<bool> SaveAsAsync(Stream stream, string separator, Encoding encoding)
    {
        if (stream == null) throw new ArgumentException("stream");

        if (separator == null || separator.Length == 0)
            throw new ArgumentException("separator");
        return SaveAsAsyncInternal(stream, separator, encoding);
    }
    private async Task<bool> SaveAsAsyncInternal(Stream stream, string separator, Encoding encoding)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        await Task.Run(() =>
        {
            try
            {
                SaveAsInternal(stream, separator,encoding);
                taskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }
#endif
#if !SILVERLIGHT && !WINRT && !WP

    public void SaveAsHtml( string filename )
    {
      SaveAsHtml( filename, HtmlSaveOptions.Default );
    }

    public void SaveAsHtml( Stream stream )
    {
      SaveAsHtml( stream, HtmlSaveOptions.Default );
    }

    /// <summary>
    /// Saves as HTML.
    /// </summary>
    /// <param name="fileName">The filename</param>
    /// <param name="option">The option</param>
    public void SaveAsHtml( string fileName, HtmlSaveOptions saveOption )
    {
      if( fileName == null )
        throw new ArgumentNullException( "Filename" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "FileName cannot be empty." );

      string fullPath = Path.GetFullPath( fileName );
      if( File.Exists( fullPath ) )
        File.Delete( fullPath );

      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension( fullPath );
      string outputDirectoryPath = Path.Combine( Path.GetDirectoryName( fullPath ), string.Format( "{0}_files", fileNameWithoutExtension ) );

      // checks if sheet contains pictures
      if( this.HasPictures )
      {
        // Creates default directory for storing image if not provided by user
        if( saveOption.ImagePath == null || saveOption.ImagePath.Equals( string.Empty ) )
        {
          if( System.IO.Directory.Exists( outputDirectoryPath ) )
          {
            System.IO.Directory.Delete( outputDirectoryPath, true );
            System.IO.Directory.CreateDirectory( outputDirectoryPath );
          }
          else
            System.IO.Directory.CreateDirectory( outputDirectoryPath );

          string directory = new DirectoryInfo( outputDirectoryPath ).Name;
          saveOption.ImagePath = directory;
        }
      }

      using( FileStream stream = new FileStream( fileName, FileMode.CreateNew ) )
      {
        ExcelToHtmlConverter converter = new ExcelToHtmlConverter();
        converter.ConvertToHtml( stream, this, outputDirectoryPath, saveOption );
        stream.Close();
      }
    }

    /// <summary>
    /// Saves as HTML.
    /// </summary>
    /// <param name="stream">The stream</param>
    /// <param name="saveOption">The option</param>
    public void SaveAsHtml( Stream stream, HtmlSaveOptions saveOption )
    {
      ExcelToHtmlConverter converter = new ExcelToHtmlConverter();
      string outputDirectoryPath = null;

      if( !Directory.Exists( saveOption.ImagePath ) && saveOption.ImagePath != null )
        throw new ArgumentException( "Image Path doesn't exist" );

      if( saveOption.ImagePath != null )
        outputDirectoryPath = Path.GetFullPath( saveOption.ImagePath );

      converter.ConvertToHtml( stream, this, outputDirectoryPath, saveOption );
      stream.Flush();
      //stream.Close();
    }
#endif
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iColumnIndex, IStyle defaultStyle )
    {
      SetDefaultRowColumnStyle( iColumnIndex, iColumnIndex, defaultStyle, m_arrColumnInfo,
        new OutlineDelegate( CreateColumnOutline ), false );

      WorksheetHelper.AccessRow( this, iColumnIndex );
    }
    /// <summary>
    /// Sets by column index default style for column.
    /// </summary>
    /// <param name="iStartColumnIndex">Start column index.</param>
    /// <param name="iEndColumnIndex">End column index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultColumnStyle( int iStartColumnIndex, int iEndColumnIndex,
      IStyle defaultStyle )
    {
      ParseData();

      ushort usXFIndex = ( ushort )ConvertStyleToCorrectIndex( defaultStyle );

      for( int i = iStartColumnIndex; i <= iEndColumnIndex; i++ )
      {
        IOutline outline = m_arrColumnInfo[ i ];

        if( outline == null )
        {
          outline = CreateColumnOutline( i );
        }

        outline.ExtendedFormatIndex = usXFIndex;
		SetCellStyle(i, usXFIndex);
      }

      WorksheetHelper.AccessColumn( this, iStartColumnIndex );
      WorksheetHelper.AccessColumn( this, iEndColumnIndex );
    }
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iRowIndex, IStyle defaultStyle )
    {
      ushort usXFIndex = ( ushort )ConvertStyleToCorrectIndex( defaultStyle );
      WorksheetHelper.AccessRow( this, iRowIndex );
      iRowIndex--;

      RowStorage storage = WorksheetHelper.GetOrCreateRow( this, iRowIndex, true );
         if (this.Rows.Length > 0)
      {
          foreach(IRange cell in this.Rows[iRowIndex].Cells)
          {
              storage.SetCellStyle(iRowIndex, cell.Column-1, usXFIndex, Application.RowStorageAllocationBlockSize);
          }
      }
      storage.ExtendedFormatIndex = usXFIndex;
    }
    /// <summary>
    /// Sets by column index default style for row.
    /// </summary>
    /// <param name="iStartRowIndex">Start row index.</param>
    /// <param name="iEndRowIndex">End row index.</param>
    /// <param name="defaultStyle">Default style.</param>
    public void SetDefaultRowStyle( int iStartRowIndex, int iEndRowIndex,
      IStyle defaultStyle )
    {
      //      short sXFIndex = ( short )SetDefaultRowColumnStyle( iStartRowIndex, iEndRowIndex, defaultStyle, m_arrRow,
      //        new OutlineDelegate( CreateRowOutline ), true );
      ushort usXFIndex = ( ushort )ConvertStyleToCorrectIndex( defaultStyle );
      WorksheetHelper.AccessRow( this, iStartRowIndex );
      WorksheetHelper.AccessRow( this, iEndRowIndex );

      iStartRowIndex--;
      iEndRowIndex--;

      for( int i = iStartRowIndex; i <= iEndRowIndex; i++ )
      {
        RowStorage storage = WorksheetHelper.GetOrCreateRow( this, i, true );
        if (this.Rows.Length > 0)
        {
            foreach (IRange cell in this.Rows[i].Cells)
            {
                storage.SetCellStyle(i, cell.Column - 1, usXFIndex, Application.RowStorageAllocationBlockSize);
            }
        }
        storage.ExtendedFormatIndex = usXFIndex;
        //m_arrDefaultRowStyle.SetInt16( i - 1, sXFIndex );
      }
    }
	/// <summary>
    /// Sets by row index default style for cell.
    /// </summary>  
    /// <param name="iColndex">Column index.</param>
    /// <param name="dXFIndex">Default style index.</param>
    private void SetCellStyle(int iColIndex, ushort XFindex)
    {
        for (int iRow = CellRecords.FirstRow-1; iRow <= CellRecords.LastRow; iRow++)
        {
            RowStorage storage = WorksheetHelper.GetOrCreateRow(this, iRow, false);
            if (storage != null && storage.ExtendedFormatIndex != 0)
            {
                ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord(iRow + 1, iColIndex);
                if (cell != null)
                {
                    cell.ExtendedFormatIndex = XFindex;
                    m_dicRecordsCells.AddRecord(cell, false);
                }
                else
                {
                    cell = m_dicRecordsCells.CreateCell(iRow + 1, iColIndex, TBIFFRecord.Blank);
                    cell.ExtendedFormatIndex = XFindex;
                    m_dicRecordsCells.AddRecord(cell, false);
                }
            }
        }
    }
    /// <summary>
    /// Returns default column style.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Default column style or null if style wasn't set.</returns>
    public IStyle GetDefaultColumnStyle( int iColumnIndex )
    {
      ParseData();

      if( iColumnIndex < 1 || iColumnIndex > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Value cannot be less than 1 and greater than m_book.MaxColumnCount." );

      IOutline outline = ( IOutline )m_arrColumnInfo[ iColumnIndex ];

      int iXFIndex = ( outline != null )
        ? ( int )outline.ExtendedFormatIndex
        : m_book.DefaultXFIndex;

      return new ExtendedFormatWrapper( m_book, iXFIndex );
    }

    /// <summary>
    /// Returns default row style.
    /// </summary>
    /// <param name="iRowIndex">One-based row index.</param>
    /// <returns>Default row style or null if style wasn't set.</returns>
    public IStyle GetDefaultRowStyle( int iRowIndex )
    {
      if( iRowIndex < 1 || iRowIndex > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex", "Value cannot be less than 1 and greater than m_book.MaxColumnCount." );

      RowStorage row = WorksheetHelper.GetOrCreateRow( this, iRowIndex - 1, false );

      int iXFIndex = ( row != null && m_book.IsFormatted( row.ExtendedFormatIndex ) )
        ? ( int )row.ExtendedFormatIndex
        : m_book.DefaultXFIndex;

      return new ExtendedFormatWrapper( m_book, iXFIndex );
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="range">Range to remove from internal cache.</param>
    public void FreeRange( IRange range )
    {
      for( int iRow = range.Row, iLastRow = range.LastRow; iRow <= iLastRow; iRow++ )
      {
        for( int iCol = range.Column, iLastCol = range.LastColumn; iCol <= iLastCol; iCol++ )
        {
          FreeRange( iRow, iCol );
        }
      }
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="iRow">One-based row index of the range object to remove from internal cache.</param>
    /// <param name="iColumn">One-based column index of the range object to remove from internal cache.</param>
    public void FreeRange( int iRow, int iColumn )
    {
      ParseData();

      CellRecords.FreeRange( iRow, iColumn );
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Converts range into image (Bitmap).
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <returns>Created image.</returns>
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn )
    {
      return ConvertToImage( firstRow, firstColumn, lastRow, LastColumn, ImageType.Bitmap, null );
    }
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream )
    {
      return ConvertToImage( firstRow, firstColumn, lastRow, lastColumn, imageType, outputStream, EmfType.EmfOnly );
    }
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <returns>Created image.</returns>
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      EmfType emfType, Stream outputStream )
    {
      return ConvertToImage( firstRow, firstColumn, lastRow, lastColumn, ImageType.Metafile, outputStream, emfType );
    }
    /// <summary>
    /// Converts range into image.
    /// </summary>
    /// <param name="firstRow">One-based index of the first row to convert.</param>
    /// <param name="firstColumn">One-based index of the first column to convert.</param>
    /// <param name="lastRow">One-based index of the last row to convert.</param>
    /// <param name="lastColumn">One-based index of the last column to convert.</param>
    /// <param name="imageType">Type of the image to create.</param>
    /// <param name="outputStream">Output stream. It is ignored if null.</param>
    /// <param name="emfType">Metafile EmfType.</param>
    /// <returns>Created image.</returns>
    public Image ConvertToImage( int firstRow, int firstColumn, int lastRow, int lastColumn,
      ImageType imageType, Stream outputStream, EmfType emfType )
    {
      WorksheetImageConverter converter = new WorksheetImageConverter();
      return converter.ConvertToImage( this, firstRow, firstColumn, lastRow,
        lastColumn, imageType, outputStream, emfType );
    }
#endif
    /// <summary>
    /// Gets top left cell of the worksheet.
    /// </summary>
    /// <returns></returns>
    public IRange TopLeftCell
    {
      get
      {
        int row;
        int column;

        if( IsFreezePanes )
        {
          row = FirstVisibleRow + 1;
          column = FirstVisibleColumn + 1;
        }
        else
        {
          row = TopVisibleRow;
          column = LeftVisibleColumn;
        }

        return this[ row, column ];
      }
      set
      {
        if( IsFreezePanes )
        {
          if( value.Row > PaneFirstVisible.Row && value.Column > PaneFirstVisible.Column )
          {
            FirstVisibleRow = value.Row - 1;
            FirstVisibleColumn = value.Column - 1;
          }
        }
        else
        {
          TopVisibleRow = value.Row;
          LeftVisibleColumn = value.Column;
        }
      }
    }

    #endregion

    #region Import data helper methods
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Imports data table without checking arguments for correctness.
    /// </summary>
    /// <param name="dataTable">Data table to import.</param>
    /// <param name="firstRow">Index of the first row to import.</param>
    /// <param name="firstColumn">Index of the first column to import.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="arrColumns">Array of columns to import.</param>
    private void ImportDataTableWithoutCheck(DataTable dataTable, int firstRow, int firstColumn,
      int maxRows, int maxColumns, DataColumn[] arrColumns, bool isOptimized)
    {
        DataColumn curColumn;
        if (isOptimized)
        {
            for (int i = 0; i < maxRows; i++)
            {
                DataRow row = dataTable.Rows[i];
                for (int j = 0; j < maxColumns; j++)
                {
                    curColumn = arrColumns[j];
                    //SetString(firstRow + i, firstColumn + j, row[curColumn].ToString());
                    switch (curColumn.DataType.Name)
                    {
                        default:
                            SetString(firstRow + i, firstColumn + j, (string)row[curColumn]);
                            break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < maxRows; i++)
            {
                DataRow row = dataTable.Rows[i];

                for (int j = 0; j < maxColumns; j++)
                {
                    curColumn = arrColumns[j];

                    object tableValue = row[curColumn];

                    if (tableValue == System.DBNull.Value)
                    {
                        SetString(firstRow + i, firstColumn + j, tableValue.ToString());
                    }
                    else
                    {
                        switch (curColumn.DataType.Name)
                        {
                            case "String":
                                MigrantRangeImpl migrantRange = new MigrantRangeImpl(Application, this);
                                migrantRange.ResetRowColumn(firstRow + i, firstColumn + j);
                                migrantRange.Value2 = row[curColumn];
                                break;

                            case "DateTime":
                                if (tableValue is DateTime && (DateTime)tableValue >= RangeImpl.DEF_MIN_DATETIME)
                                    SetDateTime(firstRow + i, firstColumn + j, (DateTime)tableValue);
                                else
                                    SetString(firstRow + i, firstColumn + j, tableValue.ToString());
                                break;

                            case "Double":
                                SetNumber(firstRow + i, firstColumn + j, (double)tableValue);
                                break;

                            case "Int32":
                                SetNumber(firstRow + i, firstColumn + j, (int)tableValue);
                                break;

                            default:
                                SetValueRowCol(tableValue, firstRow + i, firstColumn + j);
                                break;
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Imports data table with type preservation, but without checking arguments for correctness.
    /// </summary>
    /// <param name="dataTable">Data table to import.</param>
    /// <param name="firstRow">Index of the first row to import.</param>
    /// <param name="firstColumn">Index of the first column to import.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    /// <param name="arrColumns">Array of columns to import.</param>
    private void ImportDataTableWithoutCheckPreserve( DataTable dataTable, int firstRow, int firstColumn,
      int maxRows, int maxColumns, DataColumn[] arrColumns )
    {
        DataColumn curColumn;

        for (int i = 0; i < maxRows; i++)
        {
            DataRow row = dataTable.Rows[i];

            for (int j = 0; j < maxColumns; j++)
            {
                curColumn = arrColumns[j];

                object tableValue = row[curColumn];
    
                if (tableValue == System.DBNull.Value)
                {
                    SetString(firstRow + i, firstColumn + j, tableValue.ToString());
                }
                else
                {
                    switch (curColumn.DataType.Name)
                    {
                        case "String":
                            SetString(firstRow + i, firstColumn + j, (string)tableValue);
                            break;

                        case "DateTime":
                            if (tableValue is DateTime && (DateTime)tableValue >= RangeImpl.DEF_MIN_DATETIME)
                                SetDateTime(firstRow + i, firstColumn + j, (DateTime)tableValue);
                            else
                                SetString(firstRow + i, firstColumn + j, tableValue.ToString());
                            break;

                        case "Double":
                            SetNumber(firstRow + i, firstColumn + j, (double)tableValue);
                            break;

                        case "Int32":
                            SetNumber(firstRow + i, firstColumn + j, (int)tableValue);
                            break;

                        default:
                            SetValueRowCol(tableValue, firstRow + i, firstColumn + j);
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Imports DataView without checking arguments for correctness.
    /// </summary>
    /// <param name="dataView">DataView to import.</param>
    /// <param name="firstRow">Index of the first row to import.</param>
    /// <param name="firstColumn">Index of the first column to import.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    private void ImportDataViewWithoutCheck( DataView dataView, int firstRow, int firstColumn,
      int maxRows, int maxColumns )
    {
      IRange range;

      for( int i = 0; i < maxRows; i++ )
      {
        DataRowView row = dataView[ i ];

        for( int j = 0; j < maxColumns; j++ )
        {
          range = InnerGetCell( firstColumn + j, firstRow + i );
          range.Value2 = row[ j ];
        }
      }
    }

    /// <summary>
    /// Imports DataView with type preservation, but without checking arguments for correctness.
    /// </summary>
    /// <param name="dataView">DataView to import.</param>
    /// <param name="firstRow">Index of the first row to import.</param>
    /// <param name="firstColumn">Index of the first column to import.</param>
    /// <param name="maxRows">Maximum number of rows to import.</param>
    /// <param name="maxColumns">Maximum number of columns to import.</param>
    private void ImportDataViewWithoutCheckPreserve( DataView dataView, int firstRow, int firstColumn,
      int maxRows, int maxColumns )
    {
      IRange range;
      // column type 0 - Value2, 1 - Text, 2 - DateTime, 3 - TimeSpan
      Dictionary<int, RangeProperty> hashColumnTypes = new Dictionary<int, RangeProperty>( maxColumns );

      for( int i = 0; i < maxRows; i++ )
      {
        DataRowView row = dataView[ i ];

        for( int j = 0; j < maxColumns; j++ )
        {
          object value = row[ j ];

          if( value == null || value is System.DBNull ) continue;

          RangeProperty property = GetValueType( value, j, hashColumnTypes );

          range = InnerGetCell( firstColumn + j, firstRow + i );

          switch( property )
          {
            case RangeProperty.Text:
              range.Text = ( string )value;
              break;

            case RangeProperty.DateTime:
              range.DateTime = ( DateTime )value;
              break;

            case RangeProperty.TimeSpan:
              range.TimeSpan = ( TimeSpan )value;
              break;

            default:
              range.Value2 = value;
              break;
          }
        }
      }
    }

    /// <summary>
    /// Converts object value into RangeProperty enum.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="iColumnIndex">Column index in DataRowView.</param>
    /// <param name="hashColumnTypes">Dictionary to cache results.</param>
    /// <returns>Corresponding RangeProperty.</returns>
    private RangeProperty GetValueType( object value, int iColumnIndex, Dictionary<int, RangeProperty> hashColumnTypes )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( hashColumnTypes == null )
        throw new ArgumentNullException( "hashColumnTypes" );

      if( hashColumnTypes.ContainsKey( iColumnIndex ) )
      {
        return hashColumnTypes[ iColumnIndex ];
      }
      RangeProperty result = RangeProperty.Value2;

      if( value is string )
      {
        result = RangeProperty.Text;
      }
      else if( value is DateTime )
      {
        result = RangeProperty.DateTime;
      }
      else if( value is TimeSpan )
      {
        result = RangeProperty.TimeSpan;
      }

      hashColumnTypes.Add( iColumnIndex, result );
      return result;
    }
#endif
    #endregion

    #region Save Worksheet as Biff Records
    /// <summary>
    /// Saves worksheet into specified OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all of the worksheet's records.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When records is null
    /// </exception>
    [CLSCompliant( false )]
    public override void Serialize( OffsetArrayList records )
    {
        if (ParseOnDemand)
        {
            for( int i = 0; (i < m_arrRecords.Count); i++ )
            {
              IBiffStorage raw = m_arrRecords [ i ] as IBiffStorage;
              records.Add(raw);

              if (raw.RecordCode == 512) //DimensionRecord
              {
                  // NOTE: index record must contains stream positions of all
                  // DBCells in worksheet. Here we must simply reserve space for stream
                  // offsets which will be calculated later by IndexRecord.
                  //index.DbCells = new int[ SerializeRows( records ) ];
                  List<DBCellRecord> arrDBCells = new List<DBCellRecord>();
                  /*int*/
                  int iDBCellsCount = m_dicRecordsCells.Serialize(records, arrDBCells);
              }
            }    
        }
        else
        {
            Serialize(records, false);    
        }
    }
    protected override bool ContainsProtection
    {
      get
      {
        return base.ContainsProtection || m_errorIndicators.Count > 0;
      }
    }
    /// <summary>
    /// Serialize error indicators.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    private void SerializeErrorIndicators( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_errorIndicators == null || m_errorIndicators.Count == 0 )
        return;

      SheetProtectionRecord sheetProtection = SheetProtection;

      if( sheetProtection != null && sheetProtection.ContainProtection &&
        ( sheetProtection.ProtectedOptions | ( int )ExcelSheetProtection.LockedCells ) != 0 )
      {
        for( int i = 0, len = m_errorIndicators.Count; i < len; i++ )
        {
          ErrorIndicatorImpl indicator = m_errorIndicators[ i ];

          if( ( indicator.IgnoreOptions & ExcelIgnoreError.UnlockedFormulaCells ) != 0 )
          {
            m_errorIndicators.Remove( indicator );
            i--;
            len--;
          }
        }
      }

      if( m_errorIndicators.Count == 0 )
        return;

      sheetProtection = ( SheetProtectionRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.SheetProtection );
      sheetProtection.Type = SheetProtectionRecord.ErrorIndicatorType;
        
      records.Add( sheetProtection.Clone() );

      for( int i = 0, iCount = m_errorIndicators.Count; i < iCount; i++ )
      {
        RangeProtectionRecord record;

        if (m_rangeProtectionRecord != null)
            record = m_rangeProtectionRecord;
        else
            record = ( RangeProtectionRecord )BiffRecordFactory.GetRecord( TBIFFRecord.RangeProtection );

        ErrorIndicatorImpl errorIndicator = m_errorIndicators[ i ];
        errorIndicator.OptimizeStorage();

        record.IgnoreOptions = errorIndicator.IgnoreOptions;
        record.ErrorIndicator = errorIndicator;

        if( record.GetStoreSize( ExcelVersion.Excel97to2003 ) > BiffRecordRaw.DEF_RECORD_MAX_SIZE )
        {
          if( m_book.IsLoaded )
          {
            errorIndicator.SetLength( ErrorIndicatorImpl.MaximumIndicatorsInRecord );
          }
          else
          {
            throw new ArgumentOutOfRangeException( "Too many regions with error indicators. Please reduce them before saving." );
          }
        }

        records.Add( record );

          if (record.m_continueRecords != null && record.m_continueRecords.Count > 0)
          {
              foreach (UnknownRecord continueRecord in record.m_continueRecords)
                  records.Add(continueRecord);
          }
      }
    }
    /// <summary>
    /// Serializes worksheet if it wasn't parsed.
    /// </summary>
    /// <param name="records">Record list to serialize into.</param>
    private void SerializeNotParsedWorksheet( OffsetArrayList records )
    {
      throw new NotImplementedException();
      //      if( IsParsed )
      //        throw new ArgumentOutOfRangeException( "Worksheet was parsed." );
      //
      //      if( records == null )
      //        throw new ArgumentNullException( "records" );
      //
      //      if( IsSaved || m_iAfterMsoIndex < 0 )
      //      {
      //        records.AddList(m_arrRecords );
      //      }
      //      else
      //      {
      //        for( int i = 0; i < m_iMsoStartIndex; i++ )
      //        {
      //          records.Add( ( BiffRecordRaw )m_arrRecords[ i ] );
      //        }
      //
      //        SerializeMsoDrawings( records );
      //
      //        for( int i = m_iAfterMsoIndex, len = m_arrRecords.Count; i < len; i++ )
      //        {
      //          records.Add( ( BiffRecordRaw )m_arrRecords[ i ] );
      //        }
      //      }
    }
    /// <summary>
    /// Saves worksheet into specified OffsetArrayList in Clipboard format
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all worksheet's records
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When records is null
    /// </exception>
    [CLSCompliant( false )]
    public void SerializeForClipboard( OffsetArrayList records )
    {
      Serialize( records, true );
    }
    /// <summary>
    /// Saves ColumnInfoRecords into specified OffsetArrayList
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all ColumnInfoRecords
    /// </param>
    /// <returns>Number of ColumnInfoRecords</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When records is null
    /// </exception>
    [CLSCompliant( false )]
    protected void SerializeColumnInfo( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      int iLast = SerializeGroupColumnInfo( records );//, values );

      //if( iLast < 256 )
      if( iLast < 255 )
      {
        // add outline groups info for other columns
        ColumnInfoRecord clInfo = ( ColumnInfoRecord )BiffRecordFactory.GetRecord(
          TBIFFRecord.ColumnInfo );
        clInfo.FirstColumn = ( ushort )( iLast + 1 );
        clInfo.LastColumn = 255;//256;
        clInfo.ColumnWidth = ( ushort )EvaluateFileColumnWidth( ( int )( StandardWidth * 256 ) );//clInfo.ColumnWidth );
        clInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
        records.Add( clInfo );
      }
    }
    /// <summary>
    /// Saves specified ColumnInfoRecords into specified OffsetArrayList
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all ColumnInfoRecords
    /// </param>
    /// <returns>Index of last column</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When records or values is null
    /// </exception>
    [CLSCompliant( false )]
    protected int SerializeGroupColumnInfo( OffsetArrayList records )
    {
      int iCurColumn = 1;
      int iLastColumn = 1;
      ColumnInfoRecord curColumn = null;
      ColumnInfoRecord endColumn = null;

      while( iCurColumn <= 256 )
      {
        // Step 1. Locate not null ColumnInfo
        for( ; iCurColumn <= 256; iCurColumn++ )
        {
          curColumn = m_arrColumnInfo[ iCurColumn ];

          if( curColumn != null )
            break;
        }

        if( curColumn == null ) break;
        // Step 2. while next item is not null and equal to the found one we should increment iLastColumn
        iLastColumn = iCurColumn;

        do
        {
          iLastColumn++;
          endColumn = m_arrColumnInfo[ iLastColumn ];

          if( curColumn.CompareTo( endColumn ) != 0 ) endColumn = null;
        }
        while( iLastColumn <= 256 && endColumn != null );

        if( endColumn == null )
        {
          iLastColumn--;
          endColumn = m_arrColumnInfo[ iLastColumn ];
        }

        // Step 3. Serialize group
        ColumnInfoRecord toSerialize;

        if( iCurColumn == iLastColumn )
        {
          toSerialize = ( ColumnInfoRecord )curColumn.Clone();
        }
        else
        {
          toSerialize = ( ColumnInfoRecord )curColumn.Clone();
          toSerialize.LastColumn = endColumn.LastColumn;
        }

        toSerialize.ColumnWidth = ( ushort )EvaluateFileColumnWidth( toSerialize.ColumnWidth );
        records.Add( toSerialize );
        iCurColumn = iLastColumn + 1;
      }
      //      if( records == null )
      //        throw new ArgumentNullException( "records" );
      //
      //      int iLastColumn = 0;
      //      SortedList values = m_arrColumnInfo;
      //
      //      // here we group columns info records
      //      int iPos = 0;
      //      int iLen = 0;
      //      int iCount = values.Count;
      //      ICloneable toClone;
      //
      //      for( int i = 1; i < iCount; i++ )
      //      {
      //        // Use 'as' to increase performance.
      //        toClone = values.GetByIndex( iPos ) as ICloneable;
      //        ColumnInfoRecord info1 = toClone.Clone() as ColumnInfoRecord;
      //        toClone = values.GetByIndex( i ) as ICloneable;
      //        ColumnInfoRecord info2 = toClone.Clone() as ColumnInfoRecord;
      //        
      //        info1.ColumnWidth = ( ushort )EvaluateFileColumnWidth( info1.ColumnWidth );
      //        info2.ColumnWidth = ( ushort )EvaluateFileColumnWidth( info2.ColumnWidth );
      //
      //        int iCheck;
      //
      //        if( info1.LastColumn == info2.FirstColumn - 1 - iLen )
      //        {
      //          iCheck = info1.CompareTo( info2 );
      //        }
      //        else
      //        {
      //          iCheck = -1;
      //        }
      //
      //        if( iCheck == 0 && i != iCount - 1 )
      //        {
      //          iLen++;
      //        }
      //        else
      //        {
      //          ColumnInfoRecord clmn = null;
      //
      //          // if last record in values array equal then increase iLen counter.
      //          if( iCheck == 0 ) iLen++;
      //
      //          if( iLen > 0 )
      //          {
      //            ColumnInfoRecord info3 = values.GetByIndex( iPos + iLen ) as ColumnInfoRecord;
      //            clmn = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );
      //
      //            clmn.FirstColumn = info1.FirstColumn;
      //            clmn.LastColumn = info3.LastColumn;
      //            clmn.ExtendedFormatIndex = info1.ExtendedFormatIndex;
      //            clmn.IsCollapsed = info1.IsCollapsed;
      //            clmn.IsHidden = info1.IsHidden;
      //            clmn.OutlineLevel = info1.OutlineLevel;
      //            clmn.ColumnWidth = info1.ColumnWidth;
      //          }
      //
      //          if( clmn != null ) records.Add( clmn );
      //          else records.Add( info1 );
      //
      //          if( iCheck != 0 && i == iCount - 1 )
      //            records.Add( info2 );
      //
      //          iPos = i;
      //          iLen = 0;
      //        }
      //      }
      //
      //      // if array contains only one element then add it to output array
      //      if( iCount == 1 )
      //      {
      //        ColumnInfoRecord record = values.GetByIndex( 0 ) as ColumnInfoRecord;
      //        record.ColumnWidth = ( ushort )EvaluateFileColumnWidth( record.ColumnWidth );
      //        records.Add( record );
      //      }
      //
      //      iCount = records.Count;
      //      if( iCount > 0 )
      //      {
      //        if( records[ iCount - 1 ] is ColumnInfoRecord )
      //        {
      //          iLastColumn = ( ( ColumnInfoRecord )records[ iCount - 1 ] ).LastColumn;
      //        }
      //      }
      //
      return iLastColumn - 1;
    }
    /// <summary>
    /// Serializes conditional formats.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive format records.</param>
    [CLSCompliant( false )]
    protected void SerializeConditionalFormatting( OffsetArrayList records )
    {
      m_arrConditionalFormats.Serialize( records );
    }
    /// <summary>
    /// Serializes data validation table.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive data validation records.</param>
    [CLSCompliant( false )]
    protected void SerializeDataValidation( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_dataValidation == null ) return;

      for( int i = 0, len = m_dataValidation.Count; i < len; i++ )
      {
        DataValidationCollection dvCollection = m_dataValidation[ i ];
        dvCollection.Serialize( records );
      }

    }
    /// <summary>
    /// Compares two DVRecords ignoring ranges information.
    /// </summary>
    /// <param name="curDV">First DVRecord to compare.</param>
    /// <param name="dvToAdd">Second DVRecord to compare.</param>
    /// <returns>TRUE if they are equal; FALSE otherwise.</returns>
    private bool CompareDVWithoutRanges( DVRecord curDV, DVRecord dvToAdd )
    {
      if( curDV == null )
      {
        return dvToAdd == null;
      }

      return ( curDV.Condition == dvToAdd.Condition
        && curDV.DataType == dvToAdd.DataType
        && curDV.ErrorBoxText == dvToAdd.ErrorBoxText
        && curDV.ErrorBoxTitle == dvToAdd.ErrorBoxTitle
        && curDV.ErrorStyle == dvToAdd.ErrorStyle
        && curDV.IsEmptyCell == dvToAdd.IsEmptyCell
        && curDV.IsShowErrorBox == dvToAdd.IsShowErrorBox
        && curDV.IsShowPromptBox == dvToAdd.IsShowPromptBox
        && curDV.IsStrListExplicit == dvToAdd.IsStrListExplicit
        && curDV.IsSuppressArrow == dvToAdd.IsSuppressArrow
        && curDV.PromtBoxText == dvToAdd.PromtBoxText
        && curDV.PromtBoxTitle == dvToAdd.PromtBoxTitle
        && Ptg.CompareArrays( curDV.FirstFormulaTokens, dvToAdd.FirstFormulaTokens )
        && Ptg.CompareArrays( curDV.SecondFormulaTokens, dvToAdd.SecondFormulaTokens ) );
    }

    /// <summary>
    /// Merges DVRecords.
    /// </summary>
    /// <param name="curDv">Destination DVRecord.</param>
    /// <param name="dvToAdd">DVRecord to add regions from.</param>
    private void MergeDVRanges( DVRecord curDv, DVRecord dvToAdd )
    {
      if( curDv == null )
        throw new ArgumentNullException( "curDv" );

      if( dvToAdd == null )
        throw new ArgumentNullException( "dvToAdd" );

      curDv.AddRange( dvToAdd.AddrList );
    }
    /// <summary>
    /// Saves all shapes.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [CLSCompliant( false )]
    protected override void SerializeMsoDrawings( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      base.SerializeMsoDrawings( records );

      if( ( Application.SkipOnSave & SkipExtRecords.Drawings ) != SkipExtRecords.Drawings )
      {
        if( m_arrNotesByCellIndex != null )
        {
          foreach( NoteRecord record in m_arrNotesByCellIndex.Values )
          {
            records.Add( record );
          }
        }
      }
    }
    /// <summary>
    /// Saves worksheet into specified OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all of the worksheet's records.
    /// </param>
    /// <param name="bClipboard">Indicates whether we need to serialize all records to be able to copy them into clipboard.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When records is null
    /// </exception>
    private void Serialize( OffsetArrayList records, bool bClipboard )
    {
      if( m_arrNotes != null )
      {
        m_arrNotes.Clear();
        m_arrNotesByCellIndex.Clear();
      }

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Serializing " + Name );

      if( records == null )
        throw new ArgumentNullException( "records" );

      // if Worksheet not supported by us then save it as is
      if( IsSupported == false )
      {
        records.AddList( m_arrRecords );
        return;
      }
      else if( !IsParsed )
      {
        SerializeNotParsedWorksheet( records );
        return;
      }

#if MEASURE_PERFORMANCE
      DateTime start = DateTime.Now;
#endif

      m_bof.Type = BOFRecord.TType.TYPE_WORKSHEET;
      records.Add( m_bof );

      IndexRecord index = null;

      int iRowCount = ( m_iLastRow - m_iFirstRow + 1 );
      int iDBCellsCount = 0;

      if( iRowCount > 0 )
      {
        int mod = iRowCount % 32;
        iDBCellsCount = iRowCount / 32;

        if( mod != 0 ) iDBCellsCount++;
      }

      if( !bClipboard )
      {
        index = ( IndexRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Index );

        index.DbCells = new int[ iDBCellsCount ];
        index.FirstRow = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 ) ? 0 : m_iFirstRow - 1;
        index.LastRow = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 ) ? 0 : m_iLastRow;

        records.Add( index );
      }

      m_book.InnerCalculation.Serialize( records );

      records.Add( m_pageSetup );
      //m_pageSetup.Serialize( records );

      if( m_arrCustomProperties != null ) m_arrCustomProperties.Serialize( records );

      SerializeProtection( records, false );

      DefaultColWidthRecord defColWidth =
        ( DefaultColWidthRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DefaultColWidth );
      defColWidth.Width = ( ushort )m_dStandardColWidth;//( m_iStandardColWidth / 256 );
      records.Add( defColWidth );

      SerializeColumnInfo( records );

      m_autofilters.Serialize( records );

      if( m_arrSortRecords != null ) records.AddList( m_arrSortRecords );

      #region Insert sheet Dimenssion information
      // set worksheet dimensions
      DimensionsRecord dims = ( DimensionsRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.Dimensions );
      dims.LastRow = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 )
        ? 0
        : m_iLastRow;

      dims.LastColumn = ( m_iLastColumn == m_iFirstColumn && m_iFirstColumn == DEF_MIN_COLUMN_INDEX )
        ? ( ushort )0
        : ( ushort )( m_iLastColumn );

      dims.FirstRow = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 )
        ? 0
        : m_iFirstRow - 1;

      dims.FirstColumn = ( ushort )( ( m_iLastColumn == m_iFirstColumn
        && m_iFirstColumn == DEF_MIN_COLUMN_INDEX ) ? 0 : ( int )m_iFirstColumn - 1 );

      records.Add( dims );
      #endregion

      // NOTE: index record must contains stream positions of all
      // DBCells in worksheet. Here we must simply reserve space for stream
      // offsets which will be calculated later by IndexRecord.
      //index.DbCells = new int[ SerializeRows( records ) ];
      List<DBCellRecord> arrDBCells = new List<DBCellRecord>();
      /*int*/
      iDBCellsCount = m_dicRecordsCells.Serialize( records, arrDBCells );
      //      index.DbCells = new int[ iDBCellsCount ];
      //      index.FirstRow = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 ) ? 0 : m_iFirstRow - 1;
      //      index.LastRow  = ( m_iLastRow == m_iFirstRow && m_iFirstRow == -1 ) ? 0 : m_iLastRow;

      if( !bClipboard )
        index.DbCellRecords = arrDBCells;

      SerializeMsoDrawings( records );
      if( m_arrDConRecords != null ) records.AddList( m_arrDConRecords );

      if( m_pivotTables != null )
        m_pivotTables.Serialize( records );

      SerializeHeaderFooterPictures( records );
      SerializeWindowTwo( records );
      SerializePageLayoutView(records);
      SerializeWindowZoom( records );

      if( m_pane != null )
      {
        if( VerticalSplit == 0 && HorizontalSplit == 0 )
        {
          m_pane.ActivePane = 3;
        }
        else if( VerticalSplit == 0 )
        {
          m_pane.ActivePane = 2;
        }
        else if( HorizontalSplit == 0 )
        {
          m_pane.ActivePane = 1;
        }

        records.Add( m_pane );
      }

      CreateAllSelections();
      records.AddList( m_arrSelections );

      if( m_mergedCells != null )
        m_mergedCells.Serialize( records );

      records.AddList(PreserveExternalConnection);
            
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.UnkMarker ) );

      if( m_hyperlinks != null )
        m_hyperlinks.Serialize( records );

      SerializeConditionalFormatting( records );
      SerializeDataValidation( records );
      SerializeMacrosSupport( records );
      SerializeSheetLayout( records );
      SerializeSheetProtection( records );
      SerializeErrorIndicators( records );

      if( m_tableRecords != null )
        records.AddRange( m_tableRecords );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.EOF ) );

      if( m_arrNotes != null )
      {
        m_arrNotes.Clear();
        m_arrNotesByCellIndex.Clear();
      }

#if MEASURE_PERFORMANCE
      Debug.WriteLine( DateTime.Now - start, "Serialization time of " + Name );
#endif
    }
    #endregion

    #region Class Event raisers
    /// <summary>
    /// Raises ColumnWidthChanged event.
    /// </summary>
    /// <param name="iColumn">Zero-based column index.</param>
    /// <param name="dNewValue">New width value.</param>
    protected void RaiseColumnWidthChangedEvent( int iColumn, double dNewValue )
    {
      if( ColumnWidthChanged != null )
      {
        ValueChangedEventArgs args = new ValueChangedEventArgs( iColumn, dNewValue, "ColumnWidth" );
        ColumnWidthChanged( this, args );
      }
    }
    /// <summary>
    /// Raises RowHeightChanged event.
    /// </summary>
    /// <param name="iRow">Zero-based row index.</param>
    /// <param name="dNewValue">New height.</param>
    protected void RaiseRowHeightChangedEvent( int iRow, double dNewValue )
    {
      if( RowHeightChanged != null )
      {
        ValueChangedEventArgs args = new ValueChangedEventArgs( iRow, dNewValue, "RowHeight" );
        RowHeightChanged( this, args );
      }
    }
    #endregion

    #region Class static methods
    #endregion

    #region Class event handlers
    /// <summary>
    /// This method is called when normal font changes.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    //private void NormalFont_OnAfterChange(object sender, ValueChangedEventArgs e)
    private void NormalFont_OnAfterChange( object sender, EventArgs e )
    {
      if( m_iFirstRow > 0 )
      {
        for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
        {
          RowStorage row = WorksheetHelper.GetOrCreateRow( this, i, false );

          if( row != null && !row.IsBadFontHeight )
          {
            AutofitRow( i );
          }
        }
      }
    }
    #endregion

    #region Optimize Set Values Methods
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetFormulaValue(int iRow, int iColumn, string value)
    {
        if (value == null)
            throw new ArgumentNullException("value");

        if (value[0] == '#')
        {
            SetFormulaErrorValue(iRow, iColumn, value);
        }
        else
        {
            double doubleValue;
            bool boolValue;
            IRange range = Range [iRow ,iColumn ];
            if (Double.TryParse(value, out doubleValue) && !(range.NumberFormat.Contains("@") && range.NumberFormat.Length == 1))
                SetFormulaNumberValue(iRow, iColumn, doubleValue);
            else if (Boolean.TryParse(value, out boolValue))
                SetFormulaBoolValue(iRow, iColumn, boolValue);
            else
                SetFormulaStringValue(iRow, iColumn, value);
      }
    }
    /// <summary>
    /// Fills internal BiffRecord with data from specified DateTime.
    /// </summary>
    /// <param name="value">DateTime with range value.</param>
    protected void SetDateTime(int iRow, int iCol, DateTime value)
    {
        double dNumber = UtilityMethods.ConvertDateTimeToNumber(value);
        if (dNumber >= 0)
            SetNumber(iRow, iCol, dNumber);
        else
        {
#if WINRT
            //If date is set earlier than 1/1/1900, it should be preserved as string.
            SetString(iRow, iCol, value.ToString());
#else
            SetString(iRow, iCol, value.ToShortDateString());
#endif
        }
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetValue( int iRow, int iColumn, string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( value.Length == 0 )
      {
        SetBlankRecord( iRow, iColumn );
      }
      else if( value[ 0 ] == '=' )
      {
        SetFormula( iRow, iColumn, value.Substring( 1 ) );
      }
      else if( value[ 0 ] == '#' )
      {
          SetFormulaErrorValue(iRow, iColumn, value);
      }
      else
      {
          if (this[iRow, iColumn].HasFormula)
              SetFormulaValue(iRow, iColumn, value);
          else
          {
              double doubleValue;
              RangeImpl range = this.Range[iRow, iColumn] as RangeImpl;
              DateTime dateValue;
              bool bDateTime = range.TryParseDateTime(value, out dateValue);

              if (Double.TryParse(value, out doubleValue) && !bDateTime)
                  SetNumber(iRow, iColumn, doubleValue);
              else if (bDateTime)
                  range.Value = value;
              else
                  SetString(iRow, iColumn, value);
          }

      }
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetNumber( int iRow, int iColumn, double value )
    {
      int iXFIndex = RemoveString( iRow, iColumn );

      RKRecord rk = TryCreateRkRecord( iRow, iColumn, value, iXFIndex );

      if( rk != null )
      {
        InnerSetCell( iColumn, iRow, rk );
      }
      else
      {
        SetNumberRecord( iRow, iColumn, value, iXFIndex );
      }
    }
    /// <summary>
    /// Sets value in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Value to set.</param>
    public void SetBoolean( int iRow, int iColumn, bool value )
    {
      int iXFIndex = RemoveString( iRow, iColumn );

      BoolErrRecord boolRecord = ( BoolErrRecord )GetRecord( TBIFFRecord.BoolErr, iRow, iColumn, iXFIndex );
      boolRecord.IsErrorCode = false;

      boolRecord.BoolOrError = ( value )
        ? ( byte )1
        : ( byte )0;

      InnerSetCell( iColumn, iRow, boolRecord );
    }
    /// <summary>
    /// Sets text in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Text to set.</param>
    public void SetText( int iRow, int iColumn, string value )
    {
      if( value == null || value.Length == 0 )
        throw new ArgumentOutOfRangeException( "Text value cannot be null or empty" );

      SetString( iRow, iColumn, value );
    }
    /// <summary>
    /// Sets formula in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Formula to set.</param>
    public void SetFormula( int iRow, int iColumn, string value )
    {
      SetFormula( iRow, iColumn, value, false );
    }
    /// <summary>
    /// Sets formula in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Formula to set.</param>
    /// <param name="bIsR1C1">Indicates is formula in R1C1 notation.</param>
    public void SetFormula( int iRow, int iColumn, string value, bool bIsR1C1 )
    {
      if( value == null || value.Length == 0 || value[ 0 ] == '=' )
        throw new ArgumentOutOfRangeException( "Text value cannot be null or empty. First symbol of formula cannot be '='" );

      SetFormulaValue( iRow, iColumn, value, bIsR1C1 );
    }
    /// <summary>
    /// Sets error in the specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    /// <param name="value">Error to set.</param>
    public void SetError( int iRow, int iColumn, string value )
    {
      if( value == null || value.Length == 0 || value[ 0 ] != '#' )
        throw new ArgumentOutOfRangeException( "Text value cannot be null or empty. First symbol must be '#'" );

      SetError( iRow, iColumn, value, false );
    }
    /// <summary>
    /// Sets blank in specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index  of the cell to set value.</param>
    /// <param name="iColumn">One-based column index of the cell to set value.</param>
    public void SetBlank( int iRow, int iColumn )
    {
      SetBlankRecord( iRow, iColumn );
    }
    /// <summary>
    /// Sets blank record into cell with specified row and column.
    /// </summary>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    private void SetBlankRecord( int iRow, int iColumn )
    {
      int iXFIndex = RemoveString( iRow, iColumn );

      BiffRecordRaw blank = GetRecord( TBIFFRecord.Blank, iRow, iColumn, iXFIndex );
      InnerSetCell( iColumn, iRow, blank );
    }
    /// <summary>
    /// Sets number record into cell with specified row and column.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="value">Value to set.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    private void SetNumberRecord( int iRow, int iColumn, double value, int iXFIndex )
    {
      NumberRecord number = ( NumberRecord )GetRecord( TBIFFRecord.Number, iRow, iColumn, iXFIndex );
      number.Value = value;
      InnerSetCell( iColumn, iRow, number );
    }
    /// <summary>
    /// Sets rk record into cell with specified row and column.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="value">Value to be set.</param>
    private void SetRKRecord( int iRow, int iColumn, double value )
    {
      RKRecord rk = ( RKRecord )GetRecord( TBIFFRecord.RK, iRow, iColumn );
      rk.RKNumber = value;
      InnerSetCell( iColumn, iRow, rk );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column.</param>
    /// <param name="value">Formula value to set.</param>
    /// <param name="bIsR1C1">Indicates whether range is represented as R1C1 notation.</param>
    private void SetFormulaValue( int iRow, int iColumn, string value, bool bIsR1C1 )
    {
      int iXFIndex = RemoveString( iRow, iColumn );

      FormulaRecord formula = ( FormulaRecord )GetRecord( TBIFFRecord.Formula, iRow, iColumn, iXFIndex );
      formula.ParsedExpression = m_book.FormulaUtil.ParseString( value, this, null,
        iRow - 1, iColumn - 1, bIsR1C1 );

      InnerSetCell( iColumn, iRow, formula );
    }
    /// <summary>
    /// Sets formula number value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula number value for set.</param>
    public void SetFormulaNumberValue( int iRow, int iColumn, double value )
    {
      TRangeValueType type = GetCellType( iRow, iColumn, false );

      if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula )
        throw new ArgumentException( "Cannot sets formula value in cell that doesn't contain formula" );

      SetFormulaValue( iRow, iColumn, value );
    }
    /// <summary>
    /// Sets formula error value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula error value for set.</param>
    public void SetFormulaErrorValue( int iRow, int iColumn, string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( !FormulaUtil.ErrorNameToCode.ContainsKey( value ) )
        throw new ArgumentOutOfRangeException( "Value does not valid error string." );

      TRangeValueType type = GetCellType( iRow, iColumn, false );

      if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula )
        throw new ArgumentException( "Cannot sets formula value in cell that doesn't contain formula" );

      byte bErrCode = ( byte )FormulaUtil.ErrorNameToCode[ value ];
      double dFormulaValue = FormulaRecord.GetBoolErrorValue( bErrCode, true );

      SetFormulaValue( iRow, iColumn, dFormulaValue );
    }
    /// <summary>
    /// Sets formula bool value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula bool value for set.</param>
    public void SetFormulaBoolValue( int iRow, int iColumn, bool value )
    {
      TRangeValueType type = GetCellType( iRow, iColumn, false );

      if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula )
        throw new ArgumentException( "Cannot sets formula value in cell that doesn't contain formula" );

      byte bVal = ( value ) ? ( byte )1 : ( byte )0;
      double dFormulaValue = FormulaRecord.GetBoolErrorValue( bVal, false );

      SetFormulaValue( iRow, iColumn, dFormulaValue );
    }
    /// <summary>
    /// Sets formula string value.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents formula string value for set.</param>
    public void SetFormulaStringValue( int iRow, int iColumn, string value )
    {
      TRangeValueType type = GetCellType( iRow, iColumn, false );

      if( ( type & TRangeValueType.Formula ) != TRangeValueType.Formula )
        throw new ArgumentException( "Cannot sets formula value in cell that doesn't contain formula" );

      StringRecord strRecord = ( StringRecord )RecordExtractor.GetRecord( ( int )TBIFFRecord.String );

      strRecord.Value = value;

      double dFormulaValue = FormulaRecord.DEF_STRING_VALUE;

      SetFormulaValue( iRow, iColumn, dFormulaValue, strRecord );
    }
    /// <summary>
    /// Sets error value.
    /// </summary>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <param name="value">Value representing error name.</param>
    /// <param name="isSetText">Indicates whether to set text.</param>
    public void SetError( int iRow, int iColumn, string value, bool isSetText )
    {
      int iCode;

      if( !FormulaUtil.ErrorNameToCode.TryGetValue( value, out iCode ) )
      {
        if( isSetText )
        {
          SetString( iRow, iColumn, value );
        }
        else
        {
          throw new ArgumentOutOfRangeException( "Cannot parse error code." );
        }
      }
      else
      {
        int iXFIndex = RemoveString( iRow, iColumn );

        BoolErrRecord boolErr = ( BoolErrRecord )GetRecord( TBIFFRecord.BoolErr, iRow, iColumn, iXFIndex );
        boolErr.IsErrorCode = true;
        boolErr.BoolOrError = ( byte )iCode;
        InnerSetCell( iColumn, iRow, boolErr );
      }
    }
    /// <summary>
    /// Sets string to a range.
    /// </summary>
    /// <param name="iRow">Row index</param>
    /// <param name="iColumn">Column index</param>
    /// <param name="value">String value to set.</param>
    private void SetString( int iRow, int iColumn, string value )
    {
      int iXFIndex = RemoveString( iRow, iColumn );
      int index = m_book.InnerSST.AddIncrease( value );

      LabelSSTRecord labelSST = ( LabelSSTRecord )GetRecord( TBIFFRecord.LabelSST, iRow, iColumn, iXFIndex );
      labelSST.SSTIndex = index;

      InnerSetCell( iColumn, iRow, labelSST );
    }
    /// <summary>
    /// Removes string from a cell.
    /// </summary>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <returns>Index to extended format</returns>
    private int RemoveString( int iRow, int iColumn )
    {
      ParseData();

      ICellPositionFormat record = m_dicRecordsCells.GetCellRecord( iRow, iColumn );
      int iXFIndex = m_book.DefaultXFIndex;

      if( record != null )
      {
        iXFIndex = record.ExtendedFormatIndex;
      }
      else
      {
        RowStorage row = WorksheetHelper.GetOrCreateRow( this, iRow - 1, false );

        if( row != null )
          iXFIndex = row.ExtendedFormatIndex;

        if( iXFIndex == 0 || iXFIndex == m_book.DefaultXFIndex )
        {
          //iXFIndex = m_arrDefaultColumnStyle.GetInt16( iColumn - 1 );
          ColumnInfoRecord column = m_arrColumnInfo[ iColumn ];
          if( column != null )
          {
            iXFIndex = column.ExtendedFormatIndex;
          }
        }
      }

      LabelSSTRecord label = record as LabelSSTRecord;

      if( label != null )
      {
        int iIndex = label.SSTIndex;
        m_book.InnerSST.RemoveDecrease( iIndex );
      }

      return iXFIndex;
    }
    /// <summary>
    /// Returns index of an extended format for specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Index to the extended format.</returns>
    internal int GetXFIndex( int iRow, int iColumn )
    {
      ParseData();

      //ICellPositionFormat cell = m_dicRecordsCells.GetCellRecord( iRow, iColumn );

      int iXFIndex = m_dicRecordsCells.GetExtendedFormatIndex( iRow, iColumn );

      if( iXFIndex < 0 )
      {
        RowStorage row = WorksheetHelper.GetOrCreateRow( this, iRow - 1, false );
        int iStyleIndex = ( row != null && m_book.IsFormatted( row.ExtendedFormatIndex ) )
          ? ( int )row.ExtendedFormatIndex
          : 0;
        //m_arrDefaultRowStyle.GetInt16( iRow - 1 );
        if( iStyleIndex != 0 && iStyleIndex != m_book.DefaultXFIndex )
        {
          iXFIndex = row.ExtendedFormatIndex;
        }
        else
        {
          ColumnInfoRecord column = m_arrColumnInfo[ iColumn ];

          if( column != null )
          {
            iXFIndex = column.ExtendedFormatIndex;
          }
        }
      }

      return ( iXFIndex < 0 ) ? m_book.DefaultXFIndex : iXFIndex;
    }
    /// <summary>
    /// Returns index of an extended format for specified Row.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <returns>Index to the extended format.</returns>
    internal int GetXFIndex(int iRow)
    {
        ParseData();

        int iXFIndex = m_dicRecordsCells.GetExtendedFormatIndexByRow(iRow);

        if (iXFIndex < 0)
        {
            RowStorage row = WorksheetHelper.GetOrCreateRow(this, iRow - 1, false);
            int iStyleIndex = (row != null && m_book.IsFormatted(row.ExtendedFormatIndex))
              ? (int)row.ExtendedFormatIndex
              : 0;
            //m_arrDefaultRowStyle.GetInt16( iRow - 1 );
            if (iStyleIndex != 0 && iStyleIndex != m_book.DefaultXFIndex)
            {
                iXFIndex = row.ExtendedFormatIndex;
            }
        }

        return (iXFIndex < 0) ? m_book.DefaultXFIndex : iXFIndex;
    }
    /// <summary>
    /// Returns index of an extended format for specified Column.
    /// </summary>
    /// <param name="firstColumn">first column index.</param>
    /// <param name="lastColumn">last column index.</param>
    /// <returns>Index to the extended format.</returns>
    internal int GetColumnXFIndex(int firstColumn)
    {
        ParseData();

        int iXFIndex = m_dicRecordsCells.GetExtendedFormatIndexByColumn(firstColumn);

        if (iXFIndex < 0)
        {
            ColumnInfoRecord columnInfo = (ColumnInfoRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ColumnInfo);
            columnInfo.FirstColumn = (ushort)(firstColumn - 1);

            int iStyleIndex = (columnInfo != null && m_book.IsFormatted(columnInfo.ExtendedFormatIndex))
              ? (int)columnInfo.ExtendedFormatIndex
              : 0;

            if (iStyleIndex != 0 && iStyleIndex != m_book.DefaultXFIndex)
            {
                iXFIndex = columnInfo.ExtendedFormatIndex;
            }            
        }

        return (iXFIndex < 0) ? m_book.DefaultXFIndex : iXFIndex;
    }
    /// <summary>
    /// Tries to create Rk record from double value.
    /// </summary>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <param name="value">Double that should be converted to RkRecord.</param>
    /// <returns>Created RkRecord if succeeded, null otherwise.</returns>
    [CLSCompliant( false )]
    protected internal RKRecord TryCreateRkRecord( int iRow, int iColumn, double value )
    {
      ParseData();

      int rkNumber = RKRecord.ConvertToRKNumber( value );

      if( rkNumber != int.MaxValue )
      {
        RKRecord rk = ( RKRecord )GetRecord( TBIFFRecord.RK, iRow, iColumn );
        rk.SetConvertedNumber( rkNumber );
        return rk;
      }

      return null;
    }
    /// <summary>
    /// Tries to create Rk record from double value.
    /// </summary>
    /// <param name="iRow">Row index.</param>
    /// <param name="iColumn">Column index.</param>
    /// <param name="value">Double that should be converted to RkRecord.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    /// <returns>Created RkRecord if succeeded, null otherwise.</returns>
    [CLSCompliant( false )]
    protected internal RKRecord TryCreateRkRecord( int iRow, int iColumn, double value, int iXFIndex )
    {
      ParseData();

      int rkNumber = RKRecord.ConvertToRKNumber( value );

      if( rkNumber != int.MaxValue )
      {
        RKRecord rk = ( RKRecord )GetRecord( TBIFFRecord.RK, iRow, iColumn, iXFIndex );
        rk.SetConvertedNumber( rkNumber );
        return rk;
      }

      return null;
    }
    /// <summary>
    /// Creates record.
    /// </summary>
    /// <param name="recordCode">Record to create.</param>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Created biff record.</returns>
    [CLSCompliant( false )]
    public BiffRecordRaw GetRecord( TBIFFRecord recordCode, int iRow, int iColumn )
    {
      return GetRecord( recordCode, iRow, iColumn, GetXFIndex( iRow, iColumn ) );
    }
    /// <summary>
    /// Creates record.
    /// </summary>
    /// <param name="recordCode">Record to create.</param>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    /// <returns>Created biff record.</returns>
    private BiffRecordRaw GetRecord( TBIFFRecord recordCode, int iRow, int iColumn, int iXFIndex )
    {
      ICellPositionFormat cell = (RecordExtractor.GetRecord((int)recordCode) as ICellPositionFormat);

      cell.Row = iRow - 1;
      cell.Column = iColumn - 1;
      cell.ExtendedFormatIndex = ( ushort )iXFIndex;

      return cell as BiffRecordRaw;
    }
    /// <summary>
    /// Sets formula number. Use for setting FormulaError, FormulaBoolean, FormulaNumber values.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents value for set.</param>
    private void SetFormulaValue( int iRow, int iColumn, double value )
    {
      SetFormulaValue( iRow, iColumn, value, null );
    }
    /// <summary>
    /// Sets formula value. Use for setting FormulaError, FormulaBoolean, FormulaNumber, FormulaString values.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents value for set.</param>
    /// <param name="strRecord">Represents string record as formula string value. Can be null.</param>
    private void SetFormulaValue( int iRow, int iColumn, double value, StringRecord strRecord )
    {
      ParseData();

      m_dicRecordsCells.Table.SetFormulaValue( iRow, iColumn, value, strRecord );
    }
    #endregion

    #region Optimize Get Values Methods
    /// <summary>
    /// Returns formula corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>Formula contained by the cell.</returns>
    public string GetFormula( int row, int column, bool bR1C1 )
    {
      return GetFormula( row, column, bR1C1, false );
    }
    /// <summary>
    /// Returns formula corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <returns>Formula contained by the cell.</returns>
    public string GetFormula( int row, int column, bool bR1C1, bool isForSerialization )
    {
      return GetFormula( row, column, bR1C1, m_book.FormulaUtil, isForSerialization );
    }
    /// <summary>
    /// Returns formula corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <param name="formulaUtil">Formula utilities to use for parsing.</param>
    /// <returns>Formula contained by the cell.</returns>
    public string GetFormula( int row, int column, bool bR1C1, FormulaUtil formulaUtil, bool isForSerialization )
    {
      ParseData();

      Ptg[] arr = m_dicRecordsCells.Table.GetFormulaValue( row, column );

      row--;
      column--;
      return GetFormula( row, column, arr, bR1C1, formulaUtil, isForSerialization );
    }
    /// <summary>
    /// Returns formula corresponding to the cell.
    /// </summary>
    /// <param name="row">Zero-based row index of the cell to get value from.</param>
    /// <param name="column">Zero-based column index of the cell to get value from.</param>
    /// <param name="arrTokens">Array with formula tokens.</param>
    /// <param name="bR1C1">Indicates whether R1C1 notation should be used.</param>
    /// <param name="formulaUtil">Formula utilities to use for parsing.</param>
    /// <returns>Formula contained by the cell.</returns>
    private string GetFormula( int row, int column, Ptg[] arrTokens, bool bR1C1,
      FormulaUtil formulaUtil,
      bool isForSerialization )
    {
      return ( arrTokens != null ) ?
        "=" + formulaUtil.ParsePtgArray( arrTokens, row, column, bR1C1, null, false, isForSerialization, this ) :
        null;
    }
    /// <summary>
    /// Gets formula array.
    /// </summary>
    /// <param name="formula">Represents formula.</param>
    /// <returns>Formula array.</returns>
    private string GetFormulaArray( FormulaRecord formula )
    {
      ArrayRecord array = CellRecords.GetArrayRecord( formula.Row + 1, formula.Column + 1 );
      return ( array != null ) ?
        m_book.FormulaUtil.ParsePtgArray( array.Formula, array.FirstRow, array.FirstColumn, false, null, false, false, this )
        : null;
    }
    /// <summary>
    /// Returns string value corresponding to the cell.
    /// </summary>
    /// <param name="cellIndex">Cell index to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetStringValue( long cellIndex )
    {
      ParseData();

      return GetText( RangeImpl.GetRowFromCellIndex( cellIndex )
        , RangeImpl.GetColumnFromCellIndex( cellIndex ) );
    }
    /// <summary>
    /// Returns string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetText( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetStringValue( row, column, m_book.InnerSST );
    }
    /// <summary>
    /// Returns formula string value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>String contained by the cell.</returns>
    public string GetFormulaStringValue( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetFormulaStringValue( row, column, m_book.InnerSST );
    }
    /// <summary>
    /// Returns number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetNumber( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetNumberValue( row, column );
    }
    /// <summary>
    /// Returns formula number value corresponding to the cell.
    /// </summary>
    /// <param name="row">One-based row index of the cell to get value from.</param>
    /// <param name="column">One-based column index of the cell to get value from.</param>
    /// <returns>Number contained by the cell.</returns>
    public double GetFormulaNumberValue( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetFormulaNumberValue( row, column );
    }
    /// <summary>
    /// Gets error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetError( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetErrorValue( row, column );
    }
    /// <summary>
    /// Gets the error value to string.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="row">Row index.</param>
    /// <returns>Returns error string or null.</returns>
    internal string GetErrorValueToString(byte value,int row)
    {
        return m_dicRecordsCells.Table.GetErrorValue(value, row);
    }
    /// <summary>
    /// Gets formula error value from cell.
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Returns error value or null.</returns>
    public string GetFormulaErrorValue( int row, int column )
    {
      ParseData();

      return m_dicRecordsCells.Table.GetFormulaErrorValue( row, column );
    }
    /// <summary>
    /// Gets bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetBoolean( int row, int column )
    {
      ParseData();

      int iResult = m_dicRecordsCells.Table.GetBoolValue( row, column );

      return iResult > 0;
    }
    /// <summary>
    /// Gets formula bool value from cell.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Returns found bool value. If cannot found returns false.</returns>
    public bool GetFormulaBoolValue( int row, int column )
    {
      ParseData();

      int iResult = m_dicRecordsCells.Table.GetFormulaBoolValue( row, column );

      return iResult > 0;
    }
    /// <summary>
    /// Indicates is has array formula.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>Indicates is contain array formula record.</returns>
    public bool HasArrayFormulaRecord( int row, int column )
    {
      ParseData();

      Ptg[] arr = m_dicRecordsCells.Table.GetFormulaValue( row, column );
      return HasArrayFormula( arr );
    }
    /// <summary>
    /// Indicates whether tokens array contains array formula reference.
    /// </summary>
    /// <param name="arrTokens">Tokens to check.</param>
    /// <returns>True if it refers to </returns>
    public bool HasArrayFormula( Ptg[] arrTokens )
    {
      if( arrTokens == null || arrTokens.Length != 1 )
        return false;

      Ptg token = arrTokens[ 0 ];

      if( token.TokenCode != FormulaToken.tExp )
        return false;

      ControlPtg control = token as ControlPtg;

      return m_dicRecordsCells.Table.HasFormulaArrayRecord( control.RowIndex, control.ColumnIndex );//column - 1 );
    }
    /// <summary>
    /// Gets cell type from current column.
    /// </summary>
    /// <param name="row">Indicates row.</param>
    /// <param name="column">Indicates column.</param>
    /// <param name="bNeedFormulaSubType">Indicates is need to indentify formula sub type.</param>
    /// <returns>Returns cell type.</returns>
    public TRangeValueType GetCellType( int row, int column, bool bNeedFormulaSubType )
    {
      ParseData();

        if (m_dicRecordsCells != null && m_dicRecordsCells.Table != null)
            return m_dicRecordsCells.Table.GetCellType( row, column, bNeedFormulaSubType );

        return TRangeValueType.Error;
    }
    /// <summary>
    /// Indicates is formula in cell is formula to external workbook.
    /// </summary>
    /// <param name="row">Represents row index.</param>
    /// <param name="column">Represents column index.</param>
    /// <returns>If contain extern formula returns true; otherwise false.</returns>
    public bool IsExternalFormula( int row, int column )
    {
      ParseData();

      Ptg[] arr = m_dicRecordsCells.Table.GetFormulaValue( row, column );

      if( arr != null )
      {
        for( int i = 0, iLen = arr.Length; i < iLen; i++ )
        {
          ISheetReference sheetRef = arr[ i ] as ISheetReference;

          if( sheetRef != null )
          {
            int iIndex = sheetRef.RefIndex;

            if( m_book.IsExternalReference( iIndex ) )
              return true;
          }
        }
      }

      return false;
    }
     internal void OnCellValueChanged(object oldValue, object newValue, IRange range)
    {
        if (CellValueChanged != null)
        {
            Syncfusion.XlsIO.Implementation.CellValueChangedEventArgs args = new Syncfusion.XlsIO.Implementation.CellValueChangedEventArgs();
            args.OldValue = oldValue;
            args.NewValue = newValue;
            args.Range = range;
            CellValueChanged(this, args);
        }
    }
    #endregion

     #region ISheetData Members

     /// <summary>
     /// Get the idex of the first row in UsedRange
     /// </summary>
     /// <returns> index of first row</returns>
     public int GetFirstRow()
     {
         return this.Rows[0].Row;
     }
     /// <summary>
     /// get the index of the last row in UsedRange
     /// </summary>
     /// <returns>index of last row</returns>
     public int GetLastRow()
     {
         return this.Rows[Rows.Length - 1].Row;
     }

     /// <summary>
     /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
     /// </summary>
     /// <returns></returns>
     public int GetRowCount()
     {
         return this.Rows.Length;
     }

     /// <summary>
     /// Gets the first column index.
     /// </summary>
     /// <returns>Index of first column</returns>
     public int GetFirstColumn()
     {
         return this.Columns[0].Column;
     }

     /// <summary>
     /// Gets the last column index / column count.
     /// </summary>
     /// <returns>Index of last column</returns>
     public int GetLastColumn()
     {
         return this.Columns[Columns.Length - 1].Column;
     }

     /// <summary>
     /// This API supports the .NET Framework infrastructure and is not intended to be used directly from your code
     /// </summary>
     /// <returns></returns>
     public int GetColumnCount()
     {
         return this.Columns.Length;
     }
     #endregion

     internal ApplicationImpl GetAppImpl()
     {
         return this.AppImplementation;
     }
     internal int GetViewColumnWidthPixel(int column)
     {
         int num3;
         WorksheetImpl.CheckColumnIndex(column);
         double columnWidth = this.GetColumnWidth(column + 1);
         double num2 = (this.View ==SheetView.PageLayout) ? 1.05 : 1.0;
         if (columnWidth > 1.0)
         {
             num3 = (int)((columnWidth * this.GetAppImpl().GetFontCalc2()) + 0.5);
             int num4 = (int)((((double)(this.GetAppImpl().GetFontCalc2() * this.GetAppImpl().GetFontCalc1())) / 256.0) + 0.5);
             return (int)(((num3 + num4) * num2) + 0.5);
         }
         num3 = (int)((columnWidth * (this.GetAppImpl().GetFontCalc2() + ((int)((((double)(this.GetAppImpl().GetFontCalc2() * this.GetAppImpl().GetFontCalc1())) / 256.0) + 0.5)))) + 0.5);
         return (int)((num3 * num2) + 0.5);
     }

     internal double CharacterWidth(double width)
     {
         ApplicationImpl appImpl = this.GetAppImpl();
         int num = (int)((width * appImpl.GetFontCalc2()) + 0.5);
         int num2 = appImpl.GetFontCalc2();
         int num3 = appImpl.GetFontCalc1();
         int num4 = appImpl.GetFontCalc3();
         if (num < (num2 + num4))
         {
             return ((1.0 * num) / ((double)(num2 + num4)));
         }
         double num5 = ((double)((int)((((num - ((int)((((double)(num2 * num3)) / 256.0) + 0.5))) * 100.0) / ((double)num2)) + 0.5))) / 100.0;
         if (num5 > 255.0)
         {
             num5 = 255.0;
         }
         return num5;
     }
     internal static int CharacterWidth(double width, ApplicationImpl application)
     {
         if (width > 1.0)
         {
             int num = (int)((width * application.GetFontCalc2()) + 0.5);
             int num2 = (int)((((double)(application.GetFontCalc2() * application.GetFontCalc1())) / 256.0) + 0.5);
             return (num + num2);
         }
         return (int)((width * (application.GetFontCalc2() + ((int)((((double)(application.GetFontCalc2() * application.GetFontCalc1())) / 256.0) + 0.5)))) + 0.5);
     }
     internal static void CheckColumnIndex(int columnIndex)
     {
         if ((columnIndex < 0) || (columnIndex > 0x3fff))
         {
             throw new ArgumentException("Invalid column index.");
         }
     }
     internal static void CheckRowIndex(int rowIndex)
     {
         if ((rowIndex < 0) || (rowIndex > 0xfffff))
         {
             throw new ArgumentException("Invalid row index.");
         }
     }
  }


  #region Sort Classes
  /// <summary>
  /// This class is used for sorting cell indexes by row.
  /// </summary>
  internal class SortByRow : IComparer
  {
    /// <summary>
    /// Compares two cell indexes by row value
    /// </summary>
    /// <param name="x">First cell index to compare</param>
    /// <param name="y">Second cell index to compare</param>
    /// <returns>
    /// 0  - if rows are equal;
    /// -1 - when the second row is greater than the first;
    /// 1  - when the first row is greater than the second;
    ///  </returns>
    public int Compare( object x, object y )
    {
      int iRow1 = RangeImpl.GetRowFromCellIndex( ( int )x );
      int iRow2 = RangeImpl.GetRowFromCellIndex( ( int )y );
      return iRow1 - iRow2;
    }
  }

  /// <summary>
  /// This class is used for sorting cell indexes by column
  /// </summary>
  internal class SortByColumn : IComparer
  {
    /// <summary>
    /// Compares two cell indexes by column value
    /// </summary>
    /// <param name="x">First cell index to compare</param>
    /// <param name="y">Second cell index to compare</param>
    /// <returns>
    /// 0  - if columns are equal;
    /// -1 - when the second column is greater than the first;
    /// 1  - when the first column is greater than the second;
    ///  </returns>
    public int Compare( object x, object y )
    {
      int iCol1 = RangeImpl.GetColumnFromCellIndex( ( int )x );
      int iCol2 = RangeImpl.GetColumnFromCellIndex( ( int )y );
      return iCol1 - iCol2;
    }
  }

  /// <summary>
  /// This class is used for sorting ranges by row index
  /// </summary>
  internal class SortRangeByRow : IComparer
  {
    /// <summary>
    /// Compares two ranges by row index
    /// </summary>
    /// <param name="x">First range to compare</param>
    /// <param name="y">Second range to compare</param>
    /// <returns>
    /// 0  - if rows are equal;
    /// -1 - when the second row is greater than the first;
    /// 1  - when the first row is greater than the second;
    ///  </returns>
    public int Compare( object x, object y )
    {
      RangeImpl ran1 = x as RangeImpl;
      RangeImpl ran2 = y as RangeImpl;
      return ran1.Row - ran2.Row;
    }
  }

  /// <summary>
  /// This class is used for sorting ranges by column index
  /// </summary>
  internal class SortRangeByColumn : IComparer
  {
    /// <summary>
    /// Compares two ranges by column index
    /// </summary>
    /// <param name="x">First range to compare</param>
    /// <param name="y">Second range to compare</param>
    /// <returns>
    /// 0  - if rows are equal;
    /// -1 - when the second column is greater than the first;
    /// 1  - when the first column is greater than the second;
    ///  </returns>
    public int Compare( object x, object y )
    {
      RangeImpl ran1 = x as RangeImpl;
      RangeImpl ran2 = y as RangeImpl;
      return ran1.Column - ran2.Column;
    }
  }
  #endregion

  ///<exclude/>
  /// <summary>
  /// Represents the method that will return array of biff records for MsoDrawing record.
  /// </summary>
  [CLSCompliant( false )]
  public delegate BiffRecordRaw[] GetNextMsoDrawingData();

#if DEBUG || MEASURE_PERFORMANCE
  /// <summary>
  /// This structure allows to calculate the speed of code implementation
  /// with one of the most exact methods. Actually calculations are made in
  /// times of processor and then transferred in a fraction of a second
  /// ( decimal part  is a split second ).
  /// </summary>
  public struct WorksheetPerfCounter
  {
    Int64 _start;

    /// <summary>
    /// Starts to count the time of implementation.
    /// </summary>
    public void Start()
    {
      _start = 0;
      QueryPerformanceCounter( ref _start );
    }

    /// <summary>
    /// Completes to count the time of execution and returns time in seconds.
    /// </summary>
    /// <returns>Time in seconds spent on implementation of code area.
    /// Decimal part reflects the splits of second.</returns>
    public float Finish()
    {
      Int64 finish = 0;
      QueryPerformanceCounter( ref finish );

      Int64 freq = 0;
      QueryPerformanceFrequency( ref freq );
      return ( ( ( float )( finish - _start ) / ( float )freq ) );
    }

    [System.Runtime.InteropServices.DllImport( "kernel32.dll" )]
    static extern bool QueryPerformanceCounter( ref Int64 performanceCount );
    [System.Runtime.InteropServices.DllImport( "kernel32.dll" )]
    static extern bool QueryPerformanceFrequency( ref Int64 frequency );
  }
#endif


    #region MissingFunction event support
    /// <summary>
    /// Event delegate for MissingFunction event.
    /// </summary>
    /// <param name="sender">The CalcEngine.</param>
    /// <param name="e">The <see cref="MissingFunctionEventArgs"/> for this event.</param>
    public delegate void MissingFunctionEventHandler(object sender, MissingFunctionEventArgs e);

    /// <summary>
    /// The event args for the MissingFunction event which is raised whenever the CalcEngine encounters a function
    /// that is does not know.
    /// </summary>
    public class MissingFunctionEventArgs : EventArgs
    {
        private string m_missingFunctionName;
        private string m_cellLocation;
        /// <summary>
        /// Gets the name of the unknown function.
        /// </summary>
        public string MissingFunctionName
        {
            get
            {
                return m_missingFunctionName;
            }
            internal set
            {
                m_missingFunctionName = value;
            }
        }
        /// <summary>
        /// Gets the location of the missing function.
        /// </summary>
        public string CellLocation 
        {
            get
            {
                return m_cellLocation;
            }
            internal set
            {
                m_cellLocation = value;
            }
        }
    }

    #endregion

}