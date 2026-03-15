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
using System.Diagnostics;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using System.Collections.Generic;


#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents named range in the excel.
  /// </summary>
  public class NameImpl
    : CommonObject
    , IName
    , INameIndexChangedEventProvider
    , IParseable
    , IRange
    , INativePTG
    , ICloneParent
    , ICombinedRange, IDisposable
  {
    #region Skipped
#if SKIPPED
    public object RefersTo
    {
      get
      {
        // TODO: Add NameImpl.RefersTo getter implementation.
        return null;
      }
      set
      {
        // TODO: Add NameImpl.RefersTo setter implementation.
      }
    }

    public object RefersToLocal
    {
      get
      {
        // TODO: Add NameImpl.RefersToLocal getter implementation.
        return null;
      }
      set
      {
        // TODO: Add NameImpl.RefersToLocal setter implementation.
      }
    }

    public object RefersToR1C1
    {
      get
      {
        // TODO: Add NameImpl.RefersToR1C1 getter implementation.
        return null;
      }
      set
      {
        // TODO: Add NameImpl.RefersToR1C1 setter implementation.
      }
    }

    public object RefersToR1C1Local
    {
      get
      {
        // TODO: Add NameImpl.RefersToR1C1Local getter implementation.
        return null;
      }
      set
      {
        // TODO: Add NameImpl.RefersToR1C1Local setter implementation.
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public string ShortcutKey
    {
      get
      {
        return m_name.KeyboardShortcut;
      }
      set
      {
        m_name.KeyboardShortcut = value;
      }
    }

#endif
    #endregion

    #region Class constants
    /// <summary>
    /// Represents default sheet name separator.
    /// </summary>
    private const string DEF_SHEETNAME_SEPARATER = "!";
    /// <summary>
    /// String format for cell range.
    /// </summary>
    private const string DEF_RANGE_FORMAT = "{2}!{0}:{1}";
    /// <summary>
    /// Represents removed sheet index.
    /// </summary>
    public const int DEF_NAME_SHEET_INDEX = 65534;
    /// <summary>
    /// Represents valid symbols.
    /// </summary>
    private static readonly char[] DEF_VALID_SYMBOL =
    {
      '_',
      '?',
      '\\',
      '\x2116',
      '.',
      '#'
    };
    /// <summary>
    /// String representation of the workbook scope value.
    /// </summary>
    private const string WorkbookScope = "Workbook";
    #endregion

    #region Delegates
    /// <summary>
    /// 
    /// </summary>
    public delegate void NameIndexChangedEventHandler( object sender, NameIndexChangedEventArgs data );
    #endregion

    #region Class members
    /// <summary>
    /// Name record with info about this Name object.
    /// </summary>
    private NameRecord m_name;
    /// <summary>
    /// Parent workbook for this object.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Parent worksheet for this object.
    /// (but NameRecord still must be in Workbook).
    /// </summary>
    private WorksheetImpl m_worksheet;
    /// <summary>
    /// Index of the Name object in the Workbook's Names collection.
    /// </summary>
    private int m_index = -1;
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
    private bool m_isQueryRange;
    private int m_sheetindex;
    private bool m_isCommon;
    //    /// <summary>
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates a new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    public NameImpl( IApplication application, object parent )
      : base( application, parent )
    {
      m_name = ( NameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Name );
      SetParents();
    }
    /// <summary>
    /// Creates a new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">
    /// NameRecord that contains information about the new Name object.
    /// </param>
    /// <param name="index">
    /// Index of the Name object in the workbook's Names collection.
    /// </param>
    [ CLSCompliant( false ) ]
    public NameImpl( IApplication application, object parent, NameRecord name
      , int index )
      : base( application, parent )
    {
      m_index = index;
      SetParents();
      Parse( name );
    }
    /// <summary>
    /// Creates a new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">
    /// NameRecord that contains information about the new Name object.
    /// </param>
    [ CLSCompliant( false ) ]
    public NameImpl( IApplication application, object parent, NameRecord name )
      : this( application, parent, name, -1 )
    {
    }
    /// <summary>
    /// Creates a new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">Name of the new Name object.</param>
    /// <param name="range">
    /// Range that will be associated with the specified name.
    /// </param>
    /// <param name="index"></param>
    public NameImpl( IApplication application, object parent, string name, IRange range
      , int index )
      : this( application, parent, name, range, index, false )
    {
    }
    /// <summary>
    /// Creates new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">Name of the new Name object.</param>
    /// <param name="index">Current index.</param>
    public NameImpl( IApplication application, object parent, string name, int index )
      : this( application, parent, name, index, false )
    {
    }
    /// <summary>
    /// Creates new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">Name of the new Name object.</param>
    /// <param name="index">Current index.</param>
    /// <param name="bIsLocal">Indicates whether name is local.</param>
    public NameImpl( IApplication application, object parent, string name, int index, bool bIsLocal )
      : this( application, parent )
    {
      m_index = index;
      Name = name;
      SetIndexOrGlobal( bIsLocal );
    }
    /// <summary>
    /// Creates a new Name object.
    /// </summary>
    /// <param name="application">Application object for the new Name object.</param>
    /// <param name="parent">Parent object for the new Name object.</param>
    /// <param name="name">Name of the new Name object.</param>
    /// <param name="range">
    /// Range that will be associated with the specified name.
    /// </param>
    /// <param name="index"></param>
    /// <param name="bIsLocal"></param>
    public NameImpl( IApplication application, object parent, string name, IRange range
      , int index, bool bIsLocal )
      : this( application, parent )
    {
      m_index = index;
      m_name = ( NameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Name );
      SetIndexOrGlobal( bIsLocal );
      SetParents();
      Name = name;
      RefersToRange = range;
    }
    /// <summary>
    /// Sets index or global depending on IsLocal property value.
    /// </summary>
    /// <param name="bIsLocal">Indicates whether name should be local.</param>
    private void SetIndexOrGlobal( bool bIsLocal )
    {
      m_name.IndexOrGlobal = bIsLocal 
        ? ( ushort )( m_worksheet.RealIndex + 1 )
        : ( ushort )0;
    }
    #endregion

    #region IName Properties
    /// <summary>
    /// Index of the Name object in the workbook's Names collection.
    /// </summary>
    public int    Index
    {
      get
      {
        return m_index;
      }
    }
    /// <summary>
    /// Name of the Name object.
    /// </summary>
    public string Name
    {
      get
      {
        return m_name.Name;
      }
      set
      {
        if( value == null )
        {
          throw new ArgumentNullException( "value" );
        }

        //        if( !IsValidName( value ) )
        //          throw new ArgumentException( "That name is not valid" );

        if( value != m_name.Name )
        {
          string strOldName = m_name.Name;

          m_name.IsBuinldInName = NameRecord.IsPredefinedName( value );
          m_name.Name = value;
          m_book.InnerNamesColection.IsWorkbookNamesChanged = true;

          if( m_worksheet != null )
          {
            WorksheetNamesCollection names = Worksheet.InnerNames;
            names.Rename( this, strOldName );
          }
        }
      }
    }
    /// <summary>
    /// Same as Name.
    /// </summary>
    public string NameLocal
    {
      get
      {
        return m_name.Name;
      }
      set
      {
        m_name.Name = value;
      }
    }
    /// <summary>
    /// Gets / sets Range associated with the Name object.
    /// </summary>
    public IRange RefersToRange
    {
      get
      {
        IRange result = null;

        string strValue = Value;
        // TODO: this operation can be optimized.
        if( strValue != null && strValue.Length != 0 )
        {
          if( m_worksheet != null )
          {
            result = m_worksheet.GetRangeByString( strValue,true );
          }
          else
          {
            string value = strValue;
            string strSheetName = RangeImpl.GetWorksheetName( ref value );
            WorksheetImpl sheet = ( strSheetName != null ) ?
              ( WorksheetImpl )m_book.Worksheets[ strSheetName ] :
              null;

            if( sheet != null )
              result = sheet.GetRangeByString( value,true );
          }
        }

        return result;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        Value = value.AddressGlobal;
      }
    }
    /// <summary>
    /// For the Name object, a string containing the formula that the name is
    /// defined to refer to. The string is in A1-style notation in the language
    /// of the macro, without an equal sign.
    /// </summary>
    public string Value
    {
      get
      {
        string strResult = null;
        try
        {
          strResult = m_book.FormulaUtil.ParsePtgArray( m_name.FormulaTokens );
        }
        catch( ParseException ex )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message, "Exception" );
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "Stack trace" );
        }

        return strResult;
      }
      set
      {
        SetValue( value, false );
      }
    }
    /// <summary>
    /// Gets/sets named range Value in R1C1 style. Read-only.
    /// </summary>
    public string ValueR1C1
    {
      get
      {
        try
        {
          return m_book.FormulaUtil.ParsePtgArray( m_name.FormulaTokens, 0, 0, true, false );
        }
        catch
        {
          return null;
        }
      }
      set
      {
        SetValue( value, true );
      }
    }
    /// <summary>
    /// Gets/sets named range RefersTo. Read-only.
    /// </summary>
    public string RefersTo
    {
        get
    {
        return '='+Value;
    }
        set
        {
            SetValue(value, false);
        }
        
    }
    /// <summary>
    /// Gets/sets named range RefersToR1C1 in R1C1 style. Read-only.
    /// </summary> 
    public string RefersToR1C1
    {
        get
        {
            return '=' +ValueR1C1;
        }
        set
        {
            SetValue(value, true);
        }

    }
    /// <summary>
    /// Determines whether the object is visible. Read/write Boolean.
    /// </summary>
    public bool   Visible
    {
      get
      {
        return !m_name.IsNameHidden;
      }
      set
      {
        m_name.IsNameHidden = !value;
      }
    }
    /// <summary>
    /// Indicates whether current name is locally defined name. Read-only.
    /// </summary>
    public bool   IsLocal
    {
      get
      {
        return ( m_name.IndexOrGlobal != 0 );
      }
    }
    /// <summary>
    /// Returns parent worksheet. Read-only.
    /// </summary>
    IWorksheet IName.Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    public bool IsQueryTableRange
    {
        get
        {
            return m_isQueryRange;
        }
        set
        {
            m_isQueryRange = value;
        }
    }
    public int SheetIndex
    {
        get
        {
            return m_sheetindex;
        }
        set
        {
            m_sheetindex = value;
        }
    }
    /// <summary>
    /// Returns string representation of the name's scope. Read-only.
    /// </summary>
    public string Scope
    {
      get
      {
        return ( IsLocal ) ?
          m_worksheet.Name :
          WorkbookScope;
      }
    }
    #endregion

    #region IRange Properties
    /// <summary>
    /// Returns the range reference in the language of the macro.
    /// Read-only String.
    /// </summary>
    public String Address
    {
      get
      {
        return ( m_worksheet != null ) ?
          string.Format( "'{0}'!{1}", m_worksheet.Name, Name ) :
          Name;//RefersToRange.Address;
      }
    }
    /// <summary>
    /// Returns the range reference for the specified range in the language
    /// of the user. Read-only String.
    /// </summary>
    public String AddressLocal
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ? range.AddressLocal : null;
      }
    }
    /// <summary>
    /// Returns range Address in format "'Sheet1'!$A$1".
    /// </summary>
    public String AddressGlobal
    {
      get
      {
        return ( m_worksheet != null ) ?
          string.Format( "'{0}'!{1}", m_worksheet.Name, Name ) :
          Name;
        //return RefersToRange.AddressGlobal;
      }
    }
    /// <summary>
    /// Returns range address in format "$A$1".
    /// </summary>
    public String AddressGlobalWithoutSheetName
    {
      get
      {
        return ( ( RangeImpl )RefersToRange ).AddressGlobalWithoutSheetName;
      }
    }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public String AddressR1C1
    {
      get
      {
        return RefersToRange.AddressR1C1;
      }
    }
    /// <summary>
    /// Returns the range reference using R1C1 notation.
    /// Read-only String.
    /// </summary>
    public String AddressR1C1Local
    {
      get
      {
        return RefersToRange.AddressR1C1Local;
      }
    }
    /// <summary>
    /// Gets / sets boolean value that is contained by this range.
    /// </summary>
    public Boolean Boolean
    {
      get
      {
        return RefersToRange.Boolean;
      }
      set
      {
        RefersToRange.Boolean = value;
      }
    }
    /// <summary>
    /// Returns a  Borders collection that represents the borders of a style
    /// or a range of cells (including a range defined as part of a
    /// conditional format).
    /// </summary>
    public IBorders Borders
    {
      get
      {
        return RefersToRange.Borders;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the cells in the specified range.
    /// Read-only.
    /// </summary>
    public IRange[] Cells
    {
      get
      {
        return RefersToRange.Cells;
      }
    }
    /// <summary>
    /// Returns the number of the first column in the first area in the specified
    /// range. Read-only.
    /// </summary>
    public Int32 Column
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ?
          range.LastColumn :
          -1;
      }
    }
    /// <summary>
    /// Column group level. Read-only.
    /// -1 - Not all columns in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public Int32 ColumnGroupLevel
    {
      get
      {
        return RefersToRange.ColumnGroupLevel;
      }
    }
    /// <summary>
    /// Returns or sets the width of all columns in the specified range.
    /// Read/write Double.
    /// </summary>
    public Double ColumnWidth
    {
      get
      {
        return RefersToRange.ColumnWidth;
      }
      set
      {
        RefersToRange.ColumnWidth = value;
      }
    }
    /// <summary>
    /// Returns the number of objects in the collection. Read-only.
    /// </summary>
    public Int32 Count
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ?
          range.Count :
          1;
      }
    }
    /// <summary>
    /// Gets / sets DateTime contained by this cell. Read-write DateTime.
    /// </summary>
    public DateTime DateTime
    {
      get
      {
        return RefersToRange.DateTime;
      }
      set
      {
        RefersToRange.DateTime = value;
      }
    }
    /// <summary>
    /// Returns cell value after number format application. Read-only.
    /// </summary>
    public String DisplayText
    {
      get
      {
        return RefersToRange.DisplayText;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the cell at the end of the
    /// region that contains the source range.
    /// </summary>
    public IRange End
    {
      get
      {
        return RefersToRange.End;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the entire column (or
    /// columns) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireColumn
    {
      get
      {
        return RefersToRange.EntireColumn;
      }
    }
    /// <summary>
    /// Returns a Range object that represents the entire row (or
    /// rows) that contains the specified range. Read-only.
    /// </summary>
    public IRange EntireRow
    {
      get
      {
        return RefersToRange.EntireRow;
      }
    }
    /// <summary>
    /// Gets / sets error value that is contained by this range.
    /// </summary>
    public String Error
    {
      get
      {
        return RefersToRange.Error;
      }
      set
      {
        RefersToRange.Error = value;
      }
    }
    /// <summary>
    /// Returns or sets the object's formula in A1-style notation and in
    /// the language of the macro. Read/write Variant.
    /// </summary>
    public String Formula
    {
      get
      {
        return RefersToRange.Formula;
      }
      set
      {
        RefersToRange.Formula = value;
      }
    }
    /// <summary>
    /// Represents array-entered formula.
    /// </summary>
    public String FormulaArray
    {
      get
      {
        return RefersToRange.FormulaArray;
      }
      set
      {
        RefersToRange.FormulaArray = value;
      }
    }
    /// <summary>
    /// Returns or sets the formula array for the range, using R1C1-style notation.
    /// </summary>
    public String FormulaArrayR1C1
    {
      get
      {
        return RefersToRange.FormulaArrayR1C1;
      }
      set
      {
        RefersToRange.FormulaArrayR1C1 = value;
      }
    }
    /// <summary>
    /// True if the formula will be hidden when the worksheet is protected.
    /// False if at least part of formula in the range is not hidden.
    /// </summary>
    public Boolean FormulaHidden
    {
      get
      {
        return RefersToRange.FormulaHidden;
      }
      set
      {
        RefersToRange.FormulaHidden = value;
      }
    }
    /// <summary>
    /// Get / set formula DateTime value contained by this cell.
    /// DateTime.MinValue if not all cells of the range have same DateTime value.
    /// </summary>
    public DateTime FormulaDateTime
    {
      get
      {
        return RefersToRange.FormulaDateTime;
      }
      set
      {
        RefersToRange.FormulaDateTime = value;
      }
    }
    /// <summary>
    /// Returns or sets the formula for the range, using R1C1-style notation.
    /// </summary>
    public String FormulaR1C1
    {
      get
      {
        return RefersToRange.FormulaR1C1;
      }
      set
      {
        RefersToRange.FormulaR1C1 = value;
      }
    }
    /// <summary>
    /// Indicates whether specified range object has data validation.
    /// If Range is not single cell, then returns true only if all cells have data validation. Read-only.
    /// </summary>
    public Boolean HasDataValidation
    {
      get
      {
        return RefersToRange.HasDataValidation;
      }
    }
    /// <summary>
    /// Indicates whether range contains bool value. Read-only.
    /// </summary>
    public bool HasBoolean
    {
      get
      {
        return RefersToRange.HasBoolean;
      }
    }
    /// <summary>
    /// Indicates whether range contains DateTime value. Read-only.
    /// </summary>
    public Boolean HasDateTime
    {
      get
      {
        return RefersToRange.HasDateTime;
      }
    }
    /// <summary>
    /// Indicates if current range has formula bool value. Read-only.
    /// </summary>
    public bool HasFormulaBoolValue
    {
      get
      {
        return RefersToRange.HasFormulaBoolValue;
      }
    }
    /// <summary>
    /// Indicates if current range has formula error value. Read-only.
    /// </summary>
    public bool HasFormulaErrorValue
    {
      get
      {
        return RefersToRange.HasFormulaErrorValue;
      }
    }
    /// <summary>
    /// Indicates if current range has formula value formatted as DateTime. Read-only.
    /// </summary>
    public bool HasFormulaDateTime
    {
      get
      {
        return RefersToRange.HasFormulaDateTime;
      }
    }

    public bool HasFormulaNumberValue
    {
      get
      {
        return RefersToRange.HasFormulaNumberValue;
      }
    }

    public bool HasFormulaStringValue
    {
      get
      {
        return RefersToRange.HasFormulaStringValue;
      }
    }
    /// <summary>
    /// True if all cells in the range contain formulas; False if
    /// at least one of the cells in the range doesn't contain a formula.
    /// Read-only Boolean.
    /// </summary>
    public Boolean HasFormula
    {
      get
      {
        return RefersToRange.HasFormula;
      }
    }
    /// <summary>
    /// Indicates whether range contains array-entered formula. Read-only.
    /// </summary>
    public Boolean HasFormulaArray
    {
      get
      {
        return RefersToRange.HasFormulaArray;
      }
    }
    /// <summary>
    /// Indicates whether the range contains number. Read-only.
    /// </summary>
    public Boolean HasNumber
    {
      get
      {
        double dResult;

        return ( RefersToRange != null ) ?
          RefersToRange.HasNumber :
          double.TryParse( Value, out dResult );
      }
    }
    /// <summary>
    /// Indicates whether cell contains formatted rich text string.
    /// </summary>
    public Boolean HasRichText
    {
      get
      {
        return RefersToRange.HasRichText;
      }
    }
    /// <summary>
    /// Indicates whether the range contains String. Read-only.
    /// </summary>
    public Boolean HasString
    {
      get
      {
        return ( RefersToRange != null ) ?
          RefersToRange.HasString :
          false;
      }
    }
    /// <summary>
    /// Indicates whether range has default style. False means default style.
    /// Read-only.
    /// </summary>
    public Boolean HasStyle
    {
      get
      {
        return RefersToRange.HasStyle;
      }
    }
    /// <summary>
    /// Returns hyperlinks for this name.
    /// </summary>
    public IHyperLinks Hyperlinks
    {
      get
      {
        return RefersToRange.Hyperlinks;
      }
    }
    /// <summary>
    /// Returns or sets the horizontal alignment for the specified object.
    /// Read/write ExcelHAlign.
    /// </summary>
    public ExcelHAlign HorizontalAlignment
    {
      get
      {
        return RefersToRange.HorizontalAlignment;
      }
      set
      {
        RefersToRange.HorizontalAlignment = value;
      }
    }
    /// <summary>
    /// Returns or sets the indent level for the cell or range. Can be an
    /// integer from 0 to 15. Read/write Integer.
    /// </summary>
    public Int32 IndentLevel
    {
      get
      {
        return RefersToRange.IndentLevel;
      }
      set
      {
        RefersToRange.IndentLevel = value;
      }
    }
    /// <summary>
    /// Indicates whether the range is blank. Read-only.
    /// </summary>
    public Boolean IsBlank
    {
      get
      {
        return ( RefersToRange != null )?
          RefersToRange.IsBlank :
          !string.IsNullOrEmpty( Value );
      }
    }
    /// <summary>
    /// Indicates whether range contains boolean value. Read-only.
    /// </summary>
    public Boolean IsBoolean
    {
      get
      {
        return RefersToRange.IsBoolean;
      }
    }
    /// <summary>
    /// Indicates whether range contains error value.
    /// </summary>
    public Boolean IsError
    {
      get
      {
        return RefersToRange.IsError;
      }
    }
    /// <summary>
    /// Indicates whether this range is grouped by column. Read-only.
    /// </summary>
    public Boolean IsGroupedByColumn
    {
      get
      {
        return RefersToRange.IsGroupedByColumn;
      }
    }
    /// <summary>
    /// Indicates whether this range is grouped by row. Read-only.
    /// </summary>
    public Boolean IsGroupedByRow
    {
      get
      {
        return RefersToRange.IsGroupedByRow;
      }
    }
    /// <summary>
    /// Indicates whether cell is initialized. Read-only.
    /// </summary>
    public Boolean IsInitialized
    {
      get
      {
        return RefersToRange.IsInitialized;
      }
    }
    /// <summary>
    /// Returns last column of the range. Read-only.
    /// </summary>
    public Int32 LastColumn
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ?
          range.LastColumn :
          -1;
      }
    }
    /// <summary>
    /// Returns last row of the range. Read-only.
    /// </summary>
    public Int32 LastRow
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ?
          range.LastRow :
          -1;
      }
    }
    /// <summary>
    /// Gets / sets double value of the range.
    /// </summary>
    public Double Number
    {
      get
      {
        return RefersToRange.Number;
      }
      set
      {
        if(RefersToRange !=null )
            RefersToRange.Number = value;
      }
    }
    /// <summary>
    /// Format of current cell. Analog of Style.NumberFormat property.
    /// </summary>
    public String NumberFormat
    {
      get
      {
        return RefersToRange.NumberFormat;
      }
      set
      {
        RefersToRange.NumberFormat = value;
      }
    }
    /// <summary>
    /// Returns the number of the first row of the first area in
    /// the range. Read-only Long.
    /// </summary>
    public Int32 Row
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ?
          range.LastRow :
          -1;
      }
    }
    /// <summary>
    /// Row group level. Read-only.
    /// -1 - Not all rows in the range have same group level.
    /// 0 - No grouping,
    /// 1 - 7 - Group level.
    /// </summary>
    public Int32 RowGroupLevel
    {
      get
      {
        return RefersToRange.RowGroupLevel;
      }
    }
    /// <summary>
    /// Returns the height of all the rows in the range specified,
    /// measured in points. Returns Double.MinValue if the rows in the specified range
    /// aren't all the same height. Read / write Double.
    /// </summary>
    public Double RowHeight
    {
      get
      {
        return RefersToRange.RowHeight;
      }
      set
      {
        RefersToRange.RowHeight = value;
      }
    }
    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// rows in the specified range.
    /// </summary>
    public IRange[] Rows
    {
      get
      {
        return RefersToRange.Rows;
      }
    }
    /// <summary>
    /// For a Range object, returns an array of Range objects that represent the
    /// columns in the specified range.
    /// </summary>
    public IRange[] Columns
    {
      get
      {
        return RefersToRange.Columns;
      }
    }
    /// <summary>
    /// Returns a Style object that represents the style of the specified
    /// range. Read/write IStyle.
    /// </summary>
    public IStyle CellStyle
    {
      get
      {
        return RefersToRange.CellStyle;
      }
      set
      {
        RefersToRange.CellStyle = value;
      }
    }
    /// <summary>
    /// Returns name of the Style object that represents the style of the specified
    /// range. Read/write String.
    /// </summary>
    public String CellStyleName
    {
      get
      {
        return RefersToRange.CellStyleName;
      }
      set
      {
        RefersToRange.CellStyleName = value;
      }
    }
    /// <summary>
    /// Gets / sets string value of the range.
    /// </summary>
    public String Text
    {
      get
      {
        return RefersToRange.Text;
      }
      set
      {
        RefersToRange.Text = value;
      }
    }
    /// <summary>
    /// Gets / sets time value of the range.
    /// </summary>
    public TimeSpan TimeSpan
    {
      get
      {
        return RefersToRange.TimeSpan;
      }
      set
      {
        RefersToRange.TimeSpan = value;
      }
    }
    /// <summary>
    /// Returns or sets the value of the specified range.
    /// Read/write Variant.
    /// </summary>
    string IRange.Value
    {
      get
      {
        return RefersToRange.Value;
      }
      set
      {
        RefersToRange.Value = value;
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
    public Object Value2
    {
      get
      {
        return RefersToRange.Value2;
      }
      set
      {
        RefersToRange.Value2 = value;
      }
    }
    /// <summary>
    /// Returns or sets the vertical alignment of the specified object.
    /// Read/write ExcelVAlign.
    /// </summary>
    public ExcelVAlign VerticalAlignment
    {
      get
      {
        return RefersToRange.VerticalAlignment;
      }
      set
      {
        RefersToRange.VerticalAlignment = value;
      }
    }
    /// <summary>
    /// Returns a Worksheet object that represents the worksheet
    /// containing the specified range. Read-only.
    /// </summary>
    IWorksheet IRange.Worksheet
    {
      get
      {
        IRange range = RefersToRange;
        return ( range != null ) ? range.Worksheet : null;
      }
    }
    /// <summary>
    /// Collection of conditional formats.
    /// </summary>
    public IConditionalFormats ConditionalFormats
    {
      get
      {
        return RefersToRange.ConditionalFormats;
      }
    }
    /// <summary>
    /// Data validation for the range.
    /// </summary>
    public IDataValidation DataValidation
    {
      get
      {
        return RefersToRange.DataValidation;
      }
    }
    /// <summary>
    /// Gets / sets string value evaluated by formula.
    /// </summary>
    public String FormulaStringValue
    {
      get
      {
        return RefersToRange.FormulaStringValue;
      }
      set
      {
        RefersToRange.FormulaStringValue = value;
      }
    }
    /// <summary>
    /// Gets / sets number value evaluated by formula.
    /// </summary>
    public Double FormulaNumberValue
    {
      get
      {
        return RefersToRange.FormulaNumberValue;
      }
      set
      {
        RefersToRange.FormulaNumberValue = value;
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a boolean.
    /// </summary>
    public bool FormulaBoolValue
    {
      get
      {
        return RefersToRange.FormulaBoolValue;
      }
      set
      {
        RefersToRange.FormulaBoolValue = value;
      }
    }
    /// <summary>
    /// Returns the calculated value of the formula as a string.
    /// </summary>
    public string FormulaErrorValue
    {
      get
      {
        return RefersToRange.FormulaErrorValue;
      }
      set
      {
        RefersToRange.FormulaErrorValue = value;
      }
    }
    /// <summary>
    /// Comment assigned to the range. Read-only.
    /// </summary>
    public ICommentShape Comment
    {
      get
      {
        return RefersToRange.Comment;
      }
    }
    /// <summary>
    /// String with rich text formatting. Read-only.
    /// </summary>
    public IRichTextString RichText
    {
      get
      {
        return RefersToRange.RichText;
      }
    }
    /// <summary>
    /// Indicates whether this range is part of merged range. Read-only.
    /// </summary>
    public Boolean IsMerged
    {
      get
      {
        return RefersToRange.IsMerged;
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
        return RefersToRange.MergeArea;
      }
    }
    /// <summary>
    /// True if Microsoft Excel wraps the text in the object.
    /// Read/write Boolean.
    /// </summary>
    public Boolean WrapText
    {
      get
      {
        return RefersToRange.WrapText;
      }
      set
      {
        RefersToRange.WrapText = value;
      }
    }
    /// <summary>
    /// Gets / sets cell by row and column index. Row and column indexes are one-based.
    /// </summary>
    public IRange this[ int row, int column ]
    {
      get
      {
        return RefersToRange[ row, column ];
      }
      set
      {
        RefersToRange[ row, column ] = value;
      }
    }
    /// <summary>
    /// Get cell range. Row and column indexes are one-based. Read-only.
    /// </summary>
    public IRange this[ int row, int column, int lastRow, int lastColumn ]
    {
      get
      {
        return RefersToRange[ row, column, lastRow, lastColumn ];
      }
    }
    /// <summary>
    /// Get cell range. Read-only.
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
        return RefersToRange[ name, IsR1C1Notation ];
      }
    }
    /// <summary>
    /// Indicates is current range has external formula. Read-only.
    /// </summary>
    public bool HasExternalFormula
    {
      get
      {
        return RefersToRange.HasExternalFormula;
      }
    }
    /// <summary>
    /// Represents ignore error options. If not single cell returs concatenated flags.
    /// </summary>
    public ExcelIgnoreError IgnoreErrorOptions
    {
      get
      {
        return RefersToRange.IgnoreErrorOptions;
      }
      set
      {
        RefersToRange.IgnoreErrorOptions = value;
      }
    }
    /// <summary>
    /// Indicates whether all values in the range are preserved as strings.
    /// </summary>
    public bool? IsStringsPreserved
    {
      get
      {
        ICombinedRange range = RefersToRange as ICombinedRange;

        return ( range != null ) ?
          m_worksheet.GetStringPreservedValue( range ):
          null;
      }
      set
      {
        ICombinedRange range = RefersToRange as ICombinedRange;

        if( range != null )
          m_worksheet.SetStringPreservedValue( range, value );
      }
    }
    /// <summary>
    /// Gets/sets built in style.
    /// </summary>
    public BuiltInStyles? BuiltInStyle
    {
      get
      {
        return RefersToRange.BuiltInStyle;
      }
      set
      {
        RefersToRange.BuiltInStyle = value;
      }
    }
    #endregion

    #region IRange Methods
    /// <summary>
    /// Copies range to the clipboard.
    /// </summary>
    public void CopyToClipboard()
    {
      throw new NotImplementedException();
      //RefersToRange.CopyToClipboard();
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      return RefersToRange.FindAll(findValue);
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      return RefersToRange.FindAll(findValue);
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( Boolean findValue )
    {
      return RefersToRange.FindAll(findValue);
    }
    /// <summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    public IRange[] FindAll( Double findValue, ExcelFindType flags )
    {
      return RefersToRange.FindAll(findValue, flags);
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>    
    public IRange[] FindAll( String findValue, ExcelFindType flags )
    {
      return RefersToRange.FindAll(findValue, flags);
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      return RefersToRange.FindFirst(findValue);
    }
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      return RefersToRange.FindFirst(findValue);
    }
    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( Boolean findValue )
    {
      return RefersToRange.FindFirst(findValue);
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns>First found cell, or Null if value was not found. </returns>
    public IRange FindFirst( Double findValue, ExcelFindType flags )
    {
      return RefersToRange.FindFirst(findValue, flags);
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Flag that represent type of search.</param>
    /// <returns></returns>
    public IRange FindFirst( String findValue, ExcelFindType flags )
    {
      return RefersToRange.FindFirst(findValue, flags);
    }
    /// <summary>
    /// Adds comment to the range.
    /// </summary>
    /// <returns>Range's comment.</returns>
    public ICommentShape AddComment()
    {
      return RefersToRange.AddComment();
    }
    /// <summary>
    /// Autofits all columns in the range.
    /// </summary>
    public void AutofitColumns()
    {
      RefersToRange.AutofitColumns();
    }
    /// <summary>
    /// Autofits all rows in the range.
    /// </summary>
    public void AutofitRows()
    {
      RefersToRange.AutofitRows();
    }
    /// <summary>
    /// Returns merge of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range to merge with.</param>
    /// <returns>Merged ranges or NULL if wasn't able to merge ranges.</returns>
    public IRange MergeWith( IRange range )
    {
      return RefersToRange.MergeWith(range);
    }
    /// <summary>
    /// Returns intersection of this range with the specified one.
    /// </summary>
    /// <param name="range">The Range with which to intersect.</param>
    /// <returns>Range intersection; if there is no intersection, NULL is returned.</returns>
    public IRange IntersectWith( IRange range )
    {
      return RefersToRange.IntersectWith(range);
    }
    /// <summary>
    /// Copies this range into another location.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>Destination range.</returns>
    public IRange CopyTo( IRange destination, ExcelCopyRangeOptions options )
    {
      return RefersToRange.CopyTo(destination, options);
    }
    /// <summary>
    /// Copies the range to the specified destination Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <returns>Range were this range was copied.</returns>
    public IRange CopyTo( IRange destination )
    {
      return RefersToRange.CopyTo( destination );
    }
    /// <summary>
    /// Moves the cells to the specified Range (without updating formulas).
    /// </summary>
    /// <param name="destination">Destination Range.</param>
    public void MoveTo( IRange destination )
    {
      RefersToRange.MoveTo( destination );
    }
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    /// <param name="options">Cells shifting options.</param>
    public void Clear( ExcelMoveDirection direction, ExcelCopyRangeOptions options )
    {
      RefersToRange.Clear(direction, options);
    }
    /// <summary>
    /// Clears the Content, formats, comments based on clear option.
    /// </summary>
    /// <param name="option"></param>
    public void Clear(ExcelClearOptions option)
    {
        RefersToRange.Clear(option);
    }
    /// <summary>
    /// Clear the contents of the Range and shifts the cells Up or Left
    /// without formula or merged ranges update.
    /// </summary>
    /// <param name="direction">Cells shift direction Up/Left.</param>
    public void Clear( ExcelMoveDirection direction )
    {
      RefersToRange.Clear(direction);
    }
    /// <summary>
    /// Clear the contents of the Range with formatting.
    /// </summary>
    /// <param name="isClearFormat">True if formatting should also be cleared.</param>
    public void Clear( Boolean isClearFormat )
    {
      RefersToRange.Clear(isClearFormat);
    }
    /// <summary>
    /// Clear the contents of the Range.
    /// </summary>
    public void Clear()
    {
      RefersToRange.Clear();
    }
    /// <summary>
    /// Freezes pane at the current range.
    /// </summary>
    public void FreezePanes()
    {
      RefersToRange.FreezePanes();
    }
    /// <summary>
    /// Separates a merged area into individual cells.
    /// </summary>
    public void UnMerge()
    {
      RefersToRange.UnMerge();
    }
    /// <summary>
    /// Ungroups current range.
    /// </summary>
    /// <param name="groupBy">Indicates type of ungrouping. Ungroup by columns or by rows.</param>
    /// <returns>Current range after ungrouping.</returns>
    public IRange Ungroup( ExcelGroupBy groupBy )
    {
      return RefersToRange.Ungroup(groupBy);
    }
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    public void Merge()
    {
      RefersToRange.Merge();
    }
    /// <summary>
    /// Creates a merged cell from the specified Range object.
    /// </summary>
    /// <param name="clearCells">Indicates whether to clear unnecessary cells.</param>
    public void Merge( bool clearCells )
    {
      RefersToRange.Merge( clearCells );
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
    public IRange Group( ExcelGroupBy groupBy, Boolean bCollapsed )
    {
      return RefersToRange.Group(groupBy, bCollapsed);
    }
    /// <summary>
    /// This method groups current range.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    /// <returns>Current range after grouping.</returns>
    public IRange Group( ExcelGroupBy groupBy )
    {
      return RefersToRange.Group(groupBy);
    }
    /// <summary>
    /// Creates Subtotal for the corresponding ranges
    /// </summary>
    /// <param name="groupBy">GroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList)
    {
        RefersToRange.SubTotal(groupBy, function, totalList);
    }
    /// <summary>
    /// Creates SubTotal for the corresponding Ranges
    /// </summary>
    /// <param name="groupBy">GroupByGroupBy</param>
    /// <param name="function">ConsolidationFunction</param>
    /// <param name="totalList">TotalList</param>
    /// <param name="replace">Replace exisiting SubTotal</param>
    /// <param name="pageBreaks">Insert PageBreaks</param>
    /// <param name="summaryBelowData">SummaryBelowData</param>
    public void SubTotal(int groupBy, ConsolidationFunction function, int[] totalList, bool replace, bool pageBreaks, bool summaryBelowData)
    {
        RefersToRange.SubTotal(groupBy, function, totalList, replace, pageBreaks, summaryBelowData);
    }
    /// <summary>
    /// Activates a single cell, which must be inside the current selection.
    /// To select a range of cells, use the Select method.
    /// </summary>
    /// <returns></returns>
    public IRange Activate()
    {
      return RefersToRange.Activate();
    }
      /// <summary>
      /// Activates a single cell, scroll to it and activates the corresponding sheet.
      /// To select a range of cells, use the Select method.
      /// </summary>
    /// <param name="scroll">True to scroll to the cell</param>
      /// <returns></returns>
    public IRange Activate(bool scroll)
    {
        return RefersToRange.Activate(scroll);
    }
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
      RefersToRange.BorderAround( borderLine, borderColor );
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
      RefersToRange.BorderInside( borderLine, borderColor );
    }
    /// <summary>
    /// Sets none border for current range.
    /// </summary>
    public void BorderNone()
    {
      RefersToRange.BorderNone();
    }
    /// <summary>
    /// Collapses current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void CollapseGroup( ExcelGroupBy groupBy )
    {
      RefersToRange.CollapseGroup( groupBy );
    }
    /// <summary>
    /// Expands current group.
    /// </summary>
    /// <param name="groupBy">
    /// This parameter specifies whether the grouping should be performed by rows or by columns. 
    /// </param>
    public void ExpandGroup( ExcelGroupBy groupBy )
    {
      RefersToRange.ExpandGroup( groupBy );
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
      RefersToRange.ExpandGroup( groupBy, flags );
    }
    #endregion

    #region Implementation properties
    /// <summary>
    /// Get NameRecord to which point current object.
    /// </summary>
    [ CLSCompliant( false ) ]
    public NameRecord Record
    {
      get
      {
        return m_name;
      }
    }
    /// <summary>
    /// Get worksheet of Name Object.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Get workbook of Name Object.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Indicates whether the name is extern name.
    /// </summary>
    public bool IsExternName
    {
      get
      {
        if( m_name == null || m_name.FormulaTokens == null )
          return false;

        for( int i = 0, len = m_name.FormulaTokens.Length; i < len; i++ )
        {
          if( m_name.FormulaTokens[ i ] is IReference )
          {
            int index = ( m_name.FormulaTokens[ i ] as IReference ).RefIndex;
            if( m_book.IsExternalReference( index ) ) return true;
          }
        }

        return false;
      }
    }
    /// <summary>
    /// Name region.
    /// </summary>
    [ CLSCompliant( false ) ]
    public MergeRegion Region
    {
      get
      {
        string strAddressLocal = AddressLocal;

        if( strAddressLocal == null )
          return null;

        string[] arrCells = strAddressLocal.Split( ':' );
        long lStartIndex = 0;
        long lEndIndex = 0;

        if( arrCells.Length > 2 )
          return null;
          
        if( arrCells.Length >= 1 )
        {
          try
          {
            lStartIndex = RangeImpl.CellNameToIndex( arrCells[ 0 ] );
            lEndIndex = lStartIndex;
          }
          catch( ArgumentException )
          {
            return null;
          }
        }

        if( arrCells.Length == 2 )
        {
          try
          {
            lEndIndex = RangeImpl.CellNameToIndex( arrCells[ 1 ] );
          }
          catch( ArgumentException )
          {
            return null;
          }
        }

        // zero-based name dimensions.
        ushort usFirstRow = ( ushort )( RangeImpl.GetRowFromCellIndex( lStartIndex ) - 1 );
        ushort usFirstCol = ( ushort )( RangeImpl.GetColumnFromCellIndex( lStartIndex ) - 1 );
        
        ushort usLastRow = ( ushort )( RangeImpl.GetRowFromCellIndex( lEndIndex ) - 1 );
        ushort usLastCol = ( ushort )( RangeImpl.GetColumnFromCellIndex( lEndIndex ) - 1 );

        return new MergeRegion( usFirstRow, usLastRow, usFirstCol, usLastCol );
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "Region" );

        if( Region == value ) return;

        int iFirstRow = value.RowFrom + 1;
        int iFirstCol = value.ColumnFrom + 1;
        int iLastRow = value.RowTo + 1;
        int iLastCol = value.ColumnTo + 1;

        string strFirstCell = RangeImpl.GetCellName( iFirstCol, iFirstRow, false, true );
        string strLastCell = RangeImpl.GetCellName( iLastCol, iLastRow, false, true );
        string strAddress = Value;
        int iIndex = strAddress.IndexOf( DEF_SHEETNAME_SEPARATER );

        if( iIndex < 1 )
          throw new NotSupportedException( "Cannot find sheet name separater." );

        string strSheetName = strAddress.Substring( 0, iIndex );

        if( strFirstCell == strLastCell )
        {
          Value = strSheetName + DEF_SHEETNAME_SEPARATER + strFirstCell;
        }
        else
        {
          Value = string.Format( DEF_RANGE_FORMAT, strFirstCell, strLastCell, strSheetName );
        }
      }
    }
    /// <summary>
    /// Indicates whether name is built-in or not.
    /// </summary>
    public bool   IsBuiltIn
    {
      get
      {
        return m_name.IsBuinldInName;
      }
      set
      {
        m_name.IsBuinldInName = value;
      }
    }
    /// <summary>
    /// Returns count of event handlers for NameIndexChanged event. Read-only.
    /// </summary>
    public int NameIndexChangedHandlersCount
    {
      get
      {
        if( NameIndexChanged != null )
        {
          return NameIndexChanged.GetInvocationList().Length;
        }

        return 0;
      }
    }
    /// <summary>
    /// Indicates whether this is function.
    /// </summary>
    public bool IsFunction
    {
      get
      {
        return m_name.IsNameFunction;
      }
      set
      {
        m_name.IsNameFunction = value;
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
    /// It's used to identify the common workbook Names
    /// </summary>
    internal bool IsCommon
    {
        get
        {
            return m_isCommon;
        }
        set
        {
            m_isCommon = value;
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Utility event. Raised on Name object index property change.
    /// </summary>
    public event NameIndexChangedEventHandler NameIndexChanged;
    #endregion

    #region IName Methods
    /// <summary>
    /// Removes this Name object from the workbook's Names collection.
    /// </summary>
    public void Delete()
    {
      if( m_worksheet != null )
      {
        m_worksheet.Names.Remove( Name );
      }
      else
      {
        m_book.Names.RemoveAt( Index );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets parent workbook and worksheet.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// When parent workbook or worksheet cannot be found.
    /// </exception>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_worksheet != null )
      {
        m_book = m_worksheet.Workbook as WorkbookImpl;
      }
      else
      {
        m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

        if( m_book == null )
          throw new ArgumentNullException( "IName has no parent workbook" );
      }
    }
    /// <summary>
    /// Reads information from the NameRecord.
    /// </summary>
    /// <param name="name">NameRecord to parse</param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified NameRecord is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Parse( NameRecord name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      m_name = ( NameRecord )name.Clone();
    }
    /// <summary>
    /// This method is called when Value property was changed.
    /// </summary>
    /// <param name="oldValue">Old value of the property.</param>
    /// <param name="newValue">New value of the property.</param>
    /// <param name="useR1C1">Is Value in R1C1 style.</param>
    private void OnValueChanged( string oldValue, string newValue, bool useR1C1 )
    {
      if( oldValue != newValue )
      {
        Dictionary<Type, ReferenceIndexAttribute> indexes = new Dictionary<Type, ReferenceIndexAttribute>();
        indexes.Add( typeof( Area3DPtg ), new ReferenceIndexAttribute( 1 ) );
        indexes.Add( typeof( Ref3DPtg ), new ReferenceIndexAttribute( 1 ) );
        ExcelParseFormulaOptions options = ExcelParseFormulaOptions.RootLevel
          | ExcelParseFormulaOptions.InName;
        
        if( useR1C1 )
          options |= ExcelParseFormulaOptions.UseR1C1;

        m_name.FormulaTokens = m_book.FormulaUtil.ParseString( newValue, m_worksheet,
          indexes, 0, null, options, 0, 0 );
        RaiseNameIndexChangedEvent( new NameIndexChangedEventArgs( Index, Index ) );
      }
    }
    /// <summary>
    /// Sets name value.
    /// </summary>
    /// <param name="parsedExpression">Parsed expression value to set.</param>
    public void SetValue( Ptg[] parsedExpression )
    {
      m_name.FormulaTokens = parsedExpression;
      RaiseNameIndexChangedEvent( new NameIndexChangedEventArgs( Index, Index ) );
    }
    /// <summary>
    /// This method raises NameIndexChanged event.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    private void RaiseNameIndexChangedEvent( NameIndexChangedEventArgs e )
    {
      if( NameIndexChanged != null )
      {
        int iLength = NameIndexChanged.GetInvocationList().Length;
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Format( "Name {0} has {1} event handlers" ,Name, iLength ),
        //  "Before Name Index changed" );

        NameIndexChanged( this, e );
      }
    }
    /// <summary>
    /// Checks for valid name.
    /// </summary>
    /// <param name="str">String to check.</param>
    /// <returns>If true - valid name; otherwise - not valid.</returns>
    private bool IsValidName( string str )
    {
      if( str == null || str.Length == 0 )
        return false;

      for( int i = 0, iLen = str.Length; i < iLen; i++ )
      {
        char charToCheck = str[ i ];

        if( !char.IsLetterOrDigit( charToCheck ) && Array.IndexOf( DEF_VALID_SYMBOL, charToCheck ) == -1 )
        {
          int iIndex = ( int )charToCheck;

          if( iIndex > NameRecord.PREDEFINED_NAMES.Length )
            return false;
        }
      }

      return true;
    }
    /// <summary>
    /// Sets Value.
    /// </summary>
    /// <param name="strValue">New value of the property.</param>
    /// <param name="useR1C1">Is Value in R1C1 style.</param>
    private void SetValue( string strValue, bool useR1C1 )
    {
      if( strValue != null && strValue.Length > 0 && strValue[ 0 ] == '=' )
      {
        strValue = strValue.Substring( 1 );
      }

      string strOldValue = Value;

      if( strOldValue != strValue )
      {
        OnValueChanged( strOldValue, strValue, useR1C1 );
      }
    }
    /// <summary>
    /// Converts full row or column tokens between versions.
    /// </summary>
    /// <param name="version">Version to convert into.</param>
    public void ConvertFullRowColumnName( ExcelVersion version )
    {
      FormulaRecord.ConvertFormulaTokens( m_name.FormulaTokens,
        ( version == ExcelVersion.Excel97to2003) );
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// A string containing the formula that the name is defined to refer to.
    /// </summary>
    /// <param name="formulaUtil">Formula util to take setting from.</param>
    /// <returns>Formula string.</returns>
    public string GetValue( FormulaUtil formulaUtil )
    {
      return formulaUtil.ParsePtgArray( m_name.FormulaTokens );
    }
    /// <summary>
    /// Sets index of the named range and raise event.
    /// </summary>
    /// <param name="index">New index.</param>
    public void SetIndex( int index )
    {
      SetIndex( index, true );
    }
    /// <summary>
    /// Sets index of the named range.
    /// </summary>
    /// <param name="index">New index.</param>
    /// <param name="bRaiseEvent">Indicates whether events should be raised.</param>
    public void SetIndex( int index, bool bRaiseEvent )
    {
      if( index != m_index )
      {
        int oldIndex = m_index;
        m_index = index;

        if( bRaiseEvent )
        {
          RaiseNameIndexChangedEvent( new NameIndexChangedEventArgs( oldIndex, index ) );
        }
      }
    }
    /// <summary>
    /// Saves named range into list of biff records.
    /// </summary>
    /// <param name="records">List of biff records to save into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_name );
    }
    /// <summary>
    /// This method should be called after worksheet index change.
    /// </summary>
    /// <param name="iSheetIndex">New sheet index.</param>
    public void SetSheetIndex( int iSheetIndex )
    {
      m_name.IndexOrGlobal = ( ushort )( iSheetIndex + 1 );
    }
    #endregion

    #region IParseable Members
    /// <summary>
    /// Parses named range.
    /// </summary>
    void Syncfusion.XlsIO.Interfaces.IParseable.Parse()
    {
      //      int iReferenceIndex = -1;
      //      Ptg[] arrPtgs = m_name.FormulaTokens;
      //
      //      for( int i = 0, len = arrPtgs.Length; i < len; i++ )
      //      {
      //        IReference reference = arrPtgs[ i ] as IReference;
      //
      //        if( reference != null )
      //        {
      //          iReferenceIndex = reference.RefIndex;
      //          break;
      //        }
      //      }
      //
      //      if( m_book.IsLocalReference( iReferenceIndex ) )
      //      {
      //        try
      //        {
      //          m_strValue = m_book.FormulaUtil.ParsePtgArray( m_name.FormulaTokens );
      //        }
      //        catch( ParseException ex )
      //        {
      //          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message, "Exception" );
      //          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "Stack trace" );
      //          m_strValue = null;
      //        }
      //        catch( Exception ex )
      //        {
      //          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message, "Exception" );
      //          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "Stack trace" );
      //          throw;
      //        }
      //      }
      //      else
      //      {
      //        m_strValue = null;
      //      }
    }

    #endregion

    #region INativePTG methods
    /// <summary>
    /// Gets ptg of current range.
    /// </summary>
    /// <returns>Returns native ptg.</returns>
    public Ptg[] GetNativePtg()
    {
      Ptg[] result = new Ptg[ 1 ];

      int supBookIndex = m_book.ExternWorkbooks.InsertSelfSupbook();
      int refIndex = m_book.AddSheetReference( supBookIndex
        , DEF_NAME_SHEET_INDEX, DEF_NAME_SHEET_INDEX );

      result[ 0 ] = FormulaUtil.CreatePtg( FormulaToken.tNameX1, refIndex, Index );

      return result;
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      NameImpl result = ( NameImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_name = ( NameRecord )CloneUtils.CloneCloneable( m_name );

      int iSheetIndex = m_name.IndexOrGlobal;

      if( iSheetIndex != 0 )
      {
        iSheetIndex--;
        WorksheetImpl sheet = ( WorksheetImpl )result.m_book.Objects[ iSheetIndex ];
        sheet.InnerNames.AddLocal( result );
        result.m_worksheet = sheet;
      }
      
      return result;
    }

    #endregion

    #region IEnumerable Members

    public IEnumerator GetEnumerator()
    {
        return this.RefersToRange.GetEnumerator();
    }

    #endregion

    #region ICombinedRange Members

    public string GetNewAddress( Dictionary<string, string> names, out string strSheetName )
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public IRange Clone( object parent, Dictionary<string, string> hashNewNames, WorkbookImpl book )
    {
        if (Worksheet !=null )
        {
            string strWorkSheetName = Worksheet.Name;

            if (hashNewNames != null && hashNewNames.ContainsKey(strWorkSheetName))
            {
                strWorkSheetName = hashNewNames[strWorkSheetName];
            }

            WorksheetImpl sheet = (WorksheetImpl)book.Worksheets[strWorkSheetName];
            IRange cloneRange = null;
            if (sheet != null)
            {
                int name = sheet.Names.Count;
                cloneRange = sheet.Names[this.Name] as NameImpl;
            }
            else
                cloneRange = Worksheet.Names[this.Name] as NameImpl;

            return cloneRange;
        }
        else
        {
            return book.Names[this.Name] as NameImpl;
           
        }
    }

    public void ClearConditionalFormats()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public Rectangle[] GetRectangles()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public int GetRectanglesCount()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    public int CellsCount
    {
      get
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }

    public string AddressGlobal2007
    {
      get
      {
        return ( m_worksheet != null ) ?
          string.Format( "'{0}'!{1}", m_worksheet.Name, Name ) :
          "[0]!" + Name;
      }
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
        internal void ClearAll()
    {
        m_name.ClearData();
        m_name = null;
        Dispose();
    }
 #region IDisposable Members

    void IDisposable.Dispose()
    {
        GC.SuppressFinalize(this);
    }

    #endregion
  }

  /// <summary>
  /// Event arguments for NameIndexChanged event.
  /// </summary>
  public class NameIndexChangedEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Old index.
    /// </summary>
    private int m_OldIndex;
    /// <summary>
    /// New index.
    /// </summary>
    private int m_NewIndex;
    #endregion

    #region Class constructors
    /// <summary>
    /// To prevent creation without parameters.
    /// </summary>
    private NameIndexChangedEventArgs()
    {
    }
    /// <summary>
    /// Creates new instance of the event arguments.
    /// </summary>
    /// <param name="oldIndex">Old index.</param>
    /// <param name="newIndex">New index.</param>
    public NameIndexChangedEventArgs( int oldIndex, int newIndex )
    {
      m_OldIndex = oldIndex;
      m_NewIndex = newIndex;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns old index. Read-only.
    /// </summary>
    public int NewIndex
    {
      get
      {
        return m_NewIndex;
      }
    }
    /// <summary>
    /// Returns new index. Read-only.
    /// </summary>
    public int OldIndex
    {
      get
      {
        return m_OldIndex;
      }
    }
    #endregion
  }
}
