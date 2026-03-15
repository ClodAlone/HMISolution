#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Shapes;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// A collection of all the Worksheet objects in the specified or
  /// active workbook. Each Worksheet object represents a worksheet.
  /// </summary>
  public class WorksheetsCollection
    : CollectionBaseEx<IWorksheet>
    , IWorksheets
    , ICloneParent
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    /// Creates a new worksheet, chart, or macro sheet. The new worksheet
    /// becomes the active sheet.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    /// <param name="Count"></param>
    /// <param name="Type"></param>
    /// <returns></returns>
    public IWorksheet Add( object Before, object After, object Count, object Type )
    {
      // TODO:  Add WorksheetsCollection.Add implementation.
      throw new NotImplementedException();
    }
    /// <summary>
    /// Copies the sheet to another location in the workbook.
    /// </summary>
    /// <param name="Before"></param>
    /// <param name="After"></param>
    public void Copy( object Before, object After )
    {
      // TODO:  Add WorksheetsCollection.Copy implementation
      throw new NotImplementedException();
    }
    /// <summary>
    /// Deletes the object.
    /// </summary>
    public void Delete()
    {
      // TODO:  Add WorksheetsCollection.Delete implementation.
      throw new NotImplementedException();
    }
    /// <summary>
    /// Selects the object.
    /// </summary>
    /// <param name="Replace"></param>
    public void Select( object Replace )
    {
      // TODO:  Add WorksheetsCollection.Select implementation.
      throw new NotImplementedException();
    }
#endif
    #endregion

    #region Class members
    /// <summary>
    /// Hash table that contains all worksheets and gives access
    /// to them by worksheet name.
    /// </summary>
    private Dictionary<string, IWorksheet> m_list = new Dictionary<string, IWorksheet>( System.StringComparer.CurrentCultureIgnoreCase );
//    /// <summary>
//    /// Collection of horizontal page breaks on the sheet.
//    /// </summary>
//    private HPageBreaksCollection m_hPages;
//    /// <summary>
//    /// Collection of vertical page breaks on the sheet
//    /// </summary>
//    private VPageBreaksCollection m_vPages;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Indicates whether to use hash for worksheet look up.
    /// </summary>
    private bool m_bUseHash = true;
    /// <summary>
    /// Sheet Name Validator
    /// </summary>
    private char[] m_sheetNameValidators = { '<', '>', '*', '?', '"', '|', ':', '/', '[', ']' };
    #endregion

    #region Class initialize/finilize methods
    /// <summary>
    /// Creates a collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public WorksheetsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;
      
      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );

      m_book.Objects.TabSheetMoved += new TabSheetMovedEventHandler( Objects_TabSheetMoved );
    }
    #endregion

    #region IWorksheets Members
    /// <summary>
    /// Returns a single object from a collection. Read-only.
    /// </summary>
    public IWorksheet this[ int Index ]
    {
      get
      {
        return ( IWorksheet )InnerList[ Index ];
      }
    }
    /// <summary>
    /// Returns a single object from a collection. Read-only.
    /// </summary>
    public IWorksheet this[ string sheetName ]
    {
      get
      {
        IWorksheet result = null;

        if( m_bUseHash )
        {
          m_list.TryGetValue( sheetName, out result );
        }
        else
        {
          List<IWorksheet> list = InnerList;
          System.StringComparer comparer = System.StringComparer.CurrentCultureIgnoreCase;

          for( int i = 0, len = list.Count; i < len; i++ )
          {
            IWorksheet sheet = list[ i ];

            if( comparer.Compare( sheet.Name, sheetName ) == 0 )
            {
              result = sheet;
              break;
            }
          }
        }

        return result;
      }
    }
//    /// <summary>
//    /// Returns a VPageBreaks collection that represents the vertical page
//    /// breaks on the sheet. Read-only.
//    /// </summary>
//    public IVPageBreaks VPageBreaks
//    {
//      get
//      {
//        return m_vPages;
//      }
//    }

    /// <summary>
    /// Indicates whether all created range objects should be cached.
    /// </summary>
    public bool UseRangesCache
    {
      get
      {
        if( Count == 0 ) return false;

        List<IWorksheet> list = InnerList;
        bool result = ( list[ 0 ] ).UseRangesCache;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          bool curValue = sheet.UseRangesCache;

          if( curValue != result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        List<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          IWorksheet sheet = list[ i ];
          sheet.UseRangesCache = value;
        }
      }
    }
    /// <summary>
    /// Toggles worksheet search algorithm when searching worksheet by name.
    /// </summary>
    public bool UseHashForWorksheetLookup
    {
      get
      {
        return m_bUseHash;
      }
      set
      {
        if( m_bUseHash != value )
        {
          m_bUseHash = value;

          if( value )
          {
            List<IWorksheet> list = InnerList;

            for( int i = 0, len = Count; i < len; i++ )
            {
              IWorksheet sheet = list[ i ];
              m_list.Add( sheet.Name, sheet );
            }
          }
          else
          {
            m_list.Clear();
          }
        }
      }
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Adds new worksheet into collection.
    /// </summary>
    /// <returns>Added worksheet.</returns>
    internal IWorksheet Add( IWorksheet sheet )
    {
      //( (WorksheetImpl) sheet ).RealIndex = m_book.ObjectCount;
      m_book.Objects.Add( ( ISerializableNamedObject )sheet );
      base.Add( sheet );

      return sheet;
    }
    /// <summary>
    /// Removes all references to worksheet from the current collection.
    /// </summary>
    /// <param name="name">Worksheet's name to remove.</param>
    internal protected void RemoveLocal( string name )
    {
      m_book.CheckParseOnDemand();
      IWorksheet sheet = this[ name ];
      base.Remove( sheet );
    }
    /// <summary>
    /// Moves worksheet inside this collection only.
    /// </summary>
    /// <param name="iOldIndex">Old index in the collection.</param>
    /// <param name="iNewIndex">New index in the collection.</param>
    public void Move( int iOldIndex, int iNewIndex )
    {
      if( iOldIndex == iNewIndex ) return;

      int iCount = InnerList.Count;

      if( iOldIndex < 0 || iOldIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iOldIndex" );

      if( iNewIndex < 0 || iNewIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iNewIndex" );
      m_book.Objects.Move(iOldIndex, iNewIndex);
      WorksheetImpl toMove = this[ iOldIndex ] as WorksheetImpl;
      InnerList.RemoveAt( iOldIndex );
      InnerList.Insert( iNewIndex, toMove );

      int iMin = Math.Min( iNewIndex, iOldIndex );
      int iMax = Math.Max( iNewIndex, iOldIndex );

      for( int i = iMin; i <= iMax; i++ )
      {
        toMove = this[ i ] as WorksheetImpl;
        toMove.Index = i;
      }
    }
    /// <summary>
    /// Updates sheet index after move/insert operation.
    /// </summary>
    /// <param name="sheet">Sheet that was changed.</param>
    /// <param name="iOldRealIndex">Old sheet index in the TabSheets collection.</param>
    public void UpdateSheetIndex( WorksheetImpl sheet, int iOldRealIndex )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int iNewRealIndex = sheet.RealIndex;
      int iIncrement = 0;
      int iStartIndex = -1;
      ITabSheets arrTabSheets = m_book.TabSheets;
      int iEndIndex = iOldRealIndex;

      if( iOldRealIndex > iNewRealIndex )
      {
        // we have to search starting from current position in the worksheets collection to the begging
        iStartIndex = iNewRealIndex + 1;
        iIncrement = 1;
      }
      else if( iOldRealIndex < iNewRealIndex )
      {
        // We have to search starting from the current position to the end of the worksheets collection.
        iStartIndex = iNewRealIndex - 1;
        iIncrement = -1;
      }
      else
      {
        throw new NotImplementedException( "Worksheet wasn't moved at all" );
      }

      ITabSheet minTabSheet = null;

      for( int i = iStartIndex; ; i += iIncrement )
      {
        ITabSheet tabSheet = arrTabSheets[ i ];

        if( tabSheet is WorksheetImpl )
        {
          minTabSheet = tabSheet;
          break;
        }

        if( i == iEndIndex ) break;
      }

      if( minTabSheet != null )
      {
        WorksheetImpl sheetLocated = ( WorksheetImpl )minTabSheet;
        int iOldSheetIndex = sheet.Index;
        int iNewSheetIndex = sheetLocated.Index;

        MoveInternal( iOldSheetIndex, iNewSheetIndex );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iOldSheetIndex"></param>
    /// <param name="iNewSheetIndex"></param>
    private void MoveInternal( int iOldSheetIndex, int iNewSheetIndex )
    {
      if( iOldSheetIndex == iNewSheetIndex ) return;

      int iCount = InnerList.Count;

      if( iOldSheetIndex < 0 || iOldSheetIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iOldIndex" );

      if( iNewSheetIndex < 0 || iNewSheetIndex >= iCount )
        throw new ArgumentOutOfRangeException( "iNewIndex" );

      WorksheetImpl toMove = this[ iOldSheetIndex ] as WorksheetImpl;
      InnerList.RemoveAt( iOldSheetIndex );
      InnerList.Insert( iNewSheetIndex, toMove );

      int iMin = Math.Min( iNewSheetIndex, iOldSheetIndex );
      int iMax = Math.Max( iNewSheetIndex, iOldSheetIndex );

      for( int i = iMin; i <= iMax; i++ )
      {
        toMove = this[ i ] as WorksheetImpl;
        toMove.Index = i;
      }
    }
    #endregion

    #region IWorksheets methods
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
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst(string findValue, ExcelFindType flags,ExcelFindOptions findOptions)
    {
        if (findValue == null) return null;

        bool bIsFormula = ((flags & ExcelFindType.Formula) == ExcelFindType.Formula);
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsFormulaStringValue = ((flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);

        if (!(bIsFormula || bIsText || bIsFormulaStringValue || bIsError))
            throw new ArgumentException("Parameter flag is not valid.", "flags");

        IList<IWorksheet> list = InnerList;

        for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++)
        {
            IRange cell = list[i].FindFirst(findValue, flags,findOptions);

            if (cell != null)
            {
                return cell;
            }
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
      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IRange cell = list[ i ].FindFirst( findValue, flags );

        if( cell != null )
        {
          return cell;
        }
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
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IRange cell = list[ i ].FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
      }

      return null;
    }
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IRange cell = list[ i ].FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
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
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IRange cell = list[ i ].FindFirst( findValue );

        if( cell != null )
        {
          return cell;
        }
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
        return FindAll(findValue, flags, ExcelFindOptions.None);
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search.</param>
    /// <returns>
    /// All found cells, or Null if value was not found.
    /// </returns>
    public IRange[] FindAll(string findValue, ExcelFindType flags,ExcelFindOptions findOptions)
    {
        if (findValue == null) return null;

        bool bIsFormula = ((flags & ExcelFindType.Formula) == ExcelFindType.Formula);
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsFormulaStringValue = ((flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);

        if (!(bIsFormula || bIsText || bIsFormulaStringValue || bIsError))
            throw new ArgumentException("Parameter flag is not valid.", "flags");

        List<IRange> cellsArray = new List<IRange>();
        IList<IWorksheet> list = InnerList;

        for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++)
        {
            IWorksheet sheet = list[i];
            IRange[] ranges = sheet.FindAll(findValue, flags,findOptions);

            if (ranges != null)
            {
                cellsArray.AddRange(ranges);
            }
        }

        return (cellsArray.Count != 0) ?
          cellsArray.ToArray() :
          null;
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
        throw new ArgumentException( "Parameter flag is not valid.", "flags" );

      List<IRange> cellsArray = new List<IRange>();
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IWorksheet sheet = list[ i ];
        IRange[] ranges  = sheet.FindAll( findValue, flags );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      return ( cellsArray.Count != 0 ) ?
        cellsArray.ToArray() :
        null;
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( bool findValue )
    {
      List<IRange> cellsArray = new List<IRange>();
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IWorksheet sheet = list[ i ];
        IRange[] ranges  = sheet.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      return ( cellsArray.Count != 0 ) ?
        cellsArray.ToArray() :
        null;
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      List<IRange> cellsArray = new List<IRange>();
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IWorksheet sheet = list[ i ];
        IRange[] ranges  = sheet.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      return ( cellsArray.Count != 0 ) ?
        cellsArray.ToArray() :
        null;
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      List<IRange> cellsArray = new List<IRange>();
      IList<IWorksheet> list = InnerList;

      for (int i = 0, sheetsCount = list.Count; i < sheetsCount; i++ )
      {
        IWorksheet sheet = list[ i ];
        IRange[] ranges  = sheet.FindAll( findValue );

        if( ranges != null )
        {
          cellsArray.AddRange( ranges );
        }
      }

      return ( cellsArray.Count != 0 ) ?
        cellsArray.ToArray() :
        null;
    }
    #endregion

    #region Worksheet recovery method
    /// <summary>
    /// Add worksheet from reader.
    /// </summary>
    /// <param name="reader">Reader which contains worksheet.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">
    /// Indicates whether worksheet should be parsed.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes used in ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <returns>Read worksheet.</returns>
    [ CLSCompliant( false ) ]
    public IWorksheet Add( Parser.BiffReader reader, ExcelParseOptions  options,
      bool bSkipParsing, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      IWorksheet sheet = AppImplementation.CreateWorksheet( this, reader, options,
        bSkipParsing, hashNewXFormatIndexes, decryptor );
      
      return Add( sheet );
    }
    /// <summary>
    /// Adds an empty worksheet.
    /// </summary>
    /// <param name="sheetName">Name of the newly created worksheet.</param>
    /// <returns>Newly created worksheet.</returns>
    public IWorksheet Add( string sheetName )
    {
      IWorksheet sheet = AppImplementation.CreateWorksheet( this );
      ( (WorksheetImpl) sheet ).RealIndex = m_book.ObjectCount;
      sheet.Name = sheetName;

      //base.Add( sheet );
      //return sheet;
      return Add( sheet );
    }
    #endregion

    #region Worksheet copy methods
    /// <summary>
    /// Add a copy of the specified worksheet to the worksheet collection.
    /// </summary>
    /// <param name="sheetIndex">Index of the workbook that should be copied</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopy( int sheetIndex )
    {
      return AddCopy( sheetIndex, ExcelWorksheetCopyFlags.CopyAll );
    }
    /// <summary>
    /// Add a copy of the specified worksheet to the worksheet collection.
    /// </summary>
    /// <param name="sheetIndex">Index of the workbook that should be copied</param>
    /// <param name="flags">Represents copy options flags.</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopy( int sheetIndex, ExcelWorksheetCopyFlags flags )
    {
      return AddCopy( this[ sheetIndex ], flags, true );
    }
    /// <summary>
    /// Adds copy of worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to copy.</param>
    /// <returns>Copy of worksheet that was added.</returns>
    public IWorksheet AddCopy( IWorksheet sheet )
    {
      return AddCopy( sheet, ExcelWorksheetCopyFlags.CopyAll );
    }
    /// <summary>
    /// Adds copy of worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to copy.</param>
    /// <param name="flags">Represents copy flags.</param>
    /// <returns>Copy of worksheet that was added.</returns>
    public IWorksheet AddCopy( IWorksheet sheet, ExcelWorksheetCopyFlags flags )
    {
      return AddCopy( sheet, flags, false );
    }
    /// <summary>
    /// Adds copy of worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to copy.</param>
    /// <param name="flags">Flags that allows to choose what kind of data should be copied.</param>
    /// <param name="isLocal">Indicates is current sheet is local.</param>
    /// <returns>Copy of worksheet that was added.</returns>
    private IWorksheet AddCopy( IWorksheet sheet, ExcelWorksheetCopyFlags flags, bool isLocal )
    {
      WorksheetImpl oldSheet = ( WorksheetImpl )sheet;

      if (oldSheet.ParseOnDemand || oldSheet.ParseDataOnDemand)
          oldSheet.CheckParseOnDemand();

      WorksheetVisibility oldSheetVisibility = oldSheet.Visibility;
      Dictionary<string, string> hashWorksheetNames;
      WorksheetImpl newSheet;

      // Check if it is local worksheet.
      if( isLocal || sheet.Workbook.Worksheets == this )
      {
        newSheet = AppImplementation.CreateWorksheet( this );

        hashWorksheetNames = new Dictionary<string, string>( 1 );
        newSheet.Name = GenerateDefaultName( List, oldSheet.Name + "_" );
        hashWorksheetNames.Add( oldSheet.Name, newSheet.Name );

        this.Add( newSheet );
        newSheet.CopyFrom( oldSheet, new Dictionary<string, string>(), hashWorksheetNames, null, flags );

        return newSheet;
      }

      // It is a worksheet from another workbook,
      // so we have to copy styles, names, comments, and other data.

      ExcelVersion sourceVersion = sheet.Workbook.Version;

      if( sourceVersion != m_book.Version &&
        ( int )sourceVersion > ( int )m_book.Version )
        throw new InvalidOperationException("Operation is not valid due to a mismatch in the document version");
      
      ExcelVersion currentVersion = oldSheet.Version;
      if ((oldSheet.Workbook as WorkbookImpl).IsConverted &&
          m_book.IsCreated && !m_book.IsConverted)
          oldSheet.Workbook.Version = ExcelVersion.Excel97to2003;

      Dictionary<int, int> dicFontIndexes;
      Dictionary<int, int> hashExtFormatIndexes;

      Dictionary<string, string> hashNewNames = m_book.InnerStyles.Merge( sheet.Workbook,
        ExcelStyleMergeOptions.CreateDiffName, out dicFontIndexes, out hashExtFormatIndexes );

      WorkbookImpl book = ( WorkbookImpl )oldSheet.Workbook;

      if( ( flags & ExcelWorksheetCopyFlags.CopyPalette ) != 0 )
        book.CopyPaletteColorTo( m_book );

      if (book.DefaultThemeVersion != null)
          m_book.DefaultThemeVersion = book.DefaultThemeVersion;

      hashWorksheetNames = new Dictionary<string, string>();
      Dictionary<int, int> hashNameIndexes = new Dictionary<int, int>();
      Dictionary<int, int> hashSubBooks = m_book.ExternWorkbooks.AddCopy( book.ExternWorkbooks );
      Dictionary<int, int> hashExternSheets = m_book.CopyExternSheets( book.ExternSheet, hashSubBooks );

      newSheet = AddWorksheet( sheet.Name, hashWorksheetNames );

      newSheet.CopyFrom( oldSheet, hashNewNames, hashWorksheetNames, dicFontIndexes
        , flags, hashExtFormatIndexes, hashNameIndexes, hashExternSheets );

      if (flags == ExcelWorksheetCopyFlags.CopyShapes)
          CopyControlsData(oldSheet, flags);

#if !SILVERLIGHT && !WINRT && !WP
      CopyOleObjects(book, newSheet.Workbook as WorkbookImpl);
#endif

      ExcelVersion sheetVersion = newSheet.Version;
      ExcelVersion bookVersion = m_book.Version;

      if( sheetVersion != bookVersion )
        newSheet.Version = bookVersion;

      if (oldSheetVisibility != newSheet.Visibility)
          newSheet.Visibility = oldSheetVisibility;

      if (sheet.Workbook.Version != currentVersion)
          sheet.Workbook.Version = currentVersion;

      return newSheet;
    }
#if !SILVERLIGHT && !WINRT && !WP
    private static void CopyOleObjects(WorkbookImpl sourceBook, WorkbookImpl destBook)
    {
        if (sourceBook.HasOleObjects)
        {
            OleStorageCollection sourceCollection = sourceBook.OleStorageCollection;
            OleStorageCollection destCollection = destBook.OleStorageCollection;

            foreach (string StorageName in sourceCollection.OleStoragesNames)
            {
                if (!destCollection.OleStoragesNames.Contains(StorageName))
                {
                    OleStorage storage = sourceCollection.OpenStorage(StorageName);
                    destCollection.Add(storage);
                    destBook.HasOleObjects = true;
                    destBook.IsOleObjectCopied = true;
                }
            }
            foreach (string streamName in sourceCollection.ArrayStreamNames)
            {
                if (!destCollection.ArrayStreamNames.Contains(streamName))
                {
                    MemoryStream stream = sourceCollection.OpenStream(streamName);
                    destCollection.Add(streamName, stream);
                }
            }
        }
    }
#endif
    private void CopyControlsData( WorksheetImpl oldSheet, ExcelWorksheetCopyFlags flags )
    {
      if( ( flags & ExcelWorksheetCopyFlags.CopyShapes ) != 0 )
      {
        WorkbookImpl oldBook = oldSheet.Workbook as WorkbookImpl;

        if(ContainsActiveX(oldSheet) )
        {
          Stream controls = oldBook.ControlsStream;

          if( controls != null )
          {
            Stream destinationStream = m_book.ControlsStream;

            if( destinationStream != null )
            {
              controls.Position = 0;
              destinationStream.Position = destinationStream.Length;
              UtilityMethods.CopyStreamTo( controls, destinationStream );
            }
            else
            {
              m_book.ControlsStream = controls;
            }
          }
        }
      }
    }
      /// <summary>
      /// Returns true, if the sheet contains the ActiveX Control.
      /// </summary>
    /// <param name="sheet">sheet to check ActiveX control.</param>
      /// <returns>true, if sheet contains ActiveX control.</returns>
    private bool ContainsActiveX(IWorksheet sheet)
    {
        ShapesCollection shapes = sheet.Shapes as ShapesCollection;
        foreach (ShapeImpl shape in shapes)
        {
            if (shape.IsActiveX)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Adding worksheets collection to current workbook.
    /// </summary>
    /// <param name="worksheets">Source worksheets collection.</param>
    public void AddCopy( IWorksheets worksheets )
    {
      AddCopy( worksheets, ExcelWorksheetCopyFlags.CopyAll );
    }
    /// <summary>
    /// Adding worksheets collection to current workbook.
    /// </summary>
    /// <param name="worksheets">Source worksheets collection.</param>
    /// <param name="flags">Represents copy option flags.</param>
    public void AddCopy( IWorksheets worksheets, ExcelWorksheetCopyFlags flags )
    {
      if( worksheets == null )
        throw new ArgumentNullException( "worksheets" );

      int iCount = worksheets.Count;
      WorkbookImpl book = ( ( WorksheetsCollection )worksheets ).m_book;

      if( worksheets == this )
      {
        for( int i = 0; i < iCount; i++ ) AddCopy( i );
      }
      else
      {
        WorksheetImpl[] arrSheets = new WorksheetImpl[ iCount ];
        Dictionary<int, int> hashExtFormatIndexes;
        Dictionary<int, int> dicFontIndexes;
        Dictionary<string, string> hashWorksheetNames = new Dictionary<string, string>();
        Dictionary<int, int> hashNameIndexes = new Dictionary<int, int>();
        Dictionary<int, int> hashSubBooks = m_book.ExternWorkbooks.AddCopy( book.ExternWorkbooks );
        Dictionary<int, int> hashExternSheets = m_book.CopyExternSheets( book.ExternSheet, hashSubBooks );

        Dictionary<string, string> hashStyleNames = m_book.InnerStyles.Merge( worksheets[ 0 ].Workbook,
          ExcelStyleMergeOptions.CreateDiffName, out dicFontIndexes, out hashExtFormatIndexes );
        
        for( int i = 0; i < iCount; i++ )
          arrSheets[ i ] = AddWorksheet( worksheets[ i ].Name, hashWorksheetNames );

        if( ( flags & ExcelWorksheetCopyFlags.CopyNames ) != 0 )
        {
          for( int i = 0; i < iCount; i++ )
          {
            arrSheets[ i ].CopyFrom( ( WorksheetImpl )worksheets[ i ], hashStyleNames, hashWorksheetNames
              , dicFontIndexes, ExcelWorksheetCopyFlags.CopyNames,
              hashExtFormatIndexes, hashNameIndexes, hashExternSheets );
          }

          flags &= ~ExcelWorksheetCopyFlags.CopyNames;
        }

        for( int i = 0; i < iCount; i++ )
        {
          arrSheets[ i ].CopyFrom( ( WorksheetImpl )worksheets[ i ], hashStyleNames, hashWorksheetNames
            , dicFontIndexes, flags, hashExtFormatIndexes, hashNameIndexes );
        }
#if !SILVERLIGHT && !WINRT && !WP
        CopyOleObjects(book, m_book);
#endif
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="strSuggestedName"></param>
    /// <param name="hashWorksheetNames"></param>
    private WorksheetImpl AddWorksheet( string strSuggestedName, Dictionary<string, string> hashWorksheetNames )
    {
      WorksheetImpl result = AppImplementation.CreateWorksheet( this );
      string strName = strSuggestedName;

      if( this[ strSuggestedName ] != null )
      {
        if( hashWorksheetNames == null )
          hashWorksheetNames = new Dictionary<string, string>();

        strName = GenerateDefaultName( List, strName + "_" );
        hashWorksheetNames.Add( strSuggestedName, strName );
      }

      result.Name = strName;
      Add( result );
      return result;
    }
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopyBefore( IWorksheet toCopy )
    {
      return AddCopyBefore( toCopy, toCopy );
    }
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <param name="sheetAfter">Represents sheet that, in collection must be after copied sheet.</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopyBefore( IWorksheet toCopy, IWorksheet sheetAfter )
    {
      if( toCopy == null )
        throw new ArgumentNullException( "toCopy" );

      if( sheetAfter == null )
        throw new ArgumentNullException( "sheetAfter" );

      int iIndex = sheetAfter.Index;
      IWorksheet sheet = AddCopy( toCopy );

      sheet.Move( iIndex );
      sheet.Activate();

      return sheet;
    }
    /// <summary>
    /// Adds copy of sheet to collection after chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopyAfter( IWorksheet toCopy )
    {
      return AddCopyAfter( toCopy, toCopy );
    }
    /// <summary>
    /// Adds copy of sheet to collection before chosen sheet.
    /// </summary>
    /// <param name="toCopy">Represents worksheet to copy.</param>
    /// <param name="sheetBefore">Represents sheet that, in collection must be before copied sheet.</param>
    /// <returns>Returns copied sheet.</returns>
    public IWorksheet AddCopyAfter( IWorksheet toCopy, IWorksheet sheetBefore )
    {
      if( toCopy == null )
        throw new ArgumentNullException( "toCopy" );

      if( sheetBefore == null )
        throw new ArgumentNullException( "sheetBefore" );

      int iIndex = sheetBefore.Index;
      IWorksheet sheet = AddCopy( toCopy );

      sheet.Move( iIndex + 1 );
      sheet.Activate();

      return sheet;
    }
//    /// <summary>
//    /// Creates copy of this collection.
//    /// </summary>
//    /// <param name="parent">Parent object for new collection.</param>
//    /// <returns>Copy of this collection.</returns>
//    public override object Clone( object parent )
//    {
//      if( parent == null )
//        throw new ArgumentNullException( "parent" );
//
//      WorksheetsCollection result = ( WorksheetsCollection )base.Clone( parent );
//
////      result.m_hPages = ( HPageBreaksCollection )m_hPages.Clone( result );
////      result.m_vPages = ( VPageBreaksCollection )m_vPages.Clone( result );
//
//      return result;
//    }
    #endregion

    #region Implementation of sync Name property
    /// <summary>
    /// Performs additional processes after inserting a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at the index.</param>
    protected override void OnInsertComplete( int index, IWorksheet value )
    {
      ( value as WorksheetImpl ).NameChanged += new ValueChangedEventHandler( sheet_NameChanged );

      if( m_bUseHash )
        m_list[ value.Name ] = value;

      base.OnInsertComplete( index, value );
    }

    /// <summary>
    /// Performs additional processes after setting a value in the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at the index.</param>
    protected override void OnSetComplete( int index, IWorksheet oldValue, IWorksheet newValue )
    {

      ( oldValue as WorksheetImpl ).NameChanged -= new ValueChangedEventHandler( sheet_NameChanged );

      if( m_bUseHash )
      {
        m_list.Remove( oldValue.Name );
        m_list[ newValue.Name ] = newValue;
      }

      base.OnSetComplete( index, oldValue, newValue );
    }

    /// <summary>
    /// Performs additional processes after removing an element from the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which value can be found.</param>
    /// <param name="value">The value of the element to remove from the index.</param>
    protected override void OnRemoveComplete( int index, IWorksheet value )
    {
      ( value as WorksheetImpl ).NameChanged -= new ValueChangedEventHandler( sheet_NameChanged );

      if( m_bUseHash )
        m_list.Remove( value.Name );

      base.OnRemoveComplete( index, value );
    }

    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      m_list.Clear();
    }

    /// <summary>
    /// This method is called when sheet name was changed.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    /// <exception cref="System.ArgumentException">
    /// When workbook already contains worksheet with specified name.
    /// </exception>
    private void sheet_NameChanged( object sender, ValueChangedEventArgs e )
    {
      if( m_bUseHash )
      {
        if( m_list.ContainsKey( ( string )e.newValue ) )
          throw new ArgumentException( "Name of worksheet must be unique in a workbook." );

        m_list.Remove( ( string )e.oldValue );
        m_list[ ( string )e.newValue ] = ( IWorksheet )sender;
      }
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Creates empty worksheet with specified name.
    /// </summary>
    /// <param name="name">New name of worksheet. Must be unique for collection.</param>
    /// <returns>Reference on created worksheet.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When specified name is NULL.
    /// </exception>
    public IWorksheet Create( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if (name.Length < 1 || SheetNameValidator(name))
          throw new ArgumentException("Sheet Name is InValid");

      IWorksheet sheet = AppImplementation.CreateWorksheet( this );
      sheet.Name = name;

      return Add( sheet );
      //base.Add( sheet );
      //return sheet;
    }
    /// <summary>
    /// Creates empty worksheet with automatically generated name.
    /// </summary>
    /// <returns>Reference on created worksheet.</returns>
    public IWorksheet Create()
    {
      IWorksheet sheet = AppImplementation.CreateWorksheet( this );

      int i = InnerList.Count;
      string sheetName = "Sheet" + i;

      // TODO: this can be optimized
      while( this[ sheetName ] != null )
      {
        i++;
        sheetName = "Sheet" + i;
      }

      sheet.Name = sheetName;
      return Add( sheet );
      //base.Add( sheet );
      //return sheet;
    }
    /// <summary>
    /// Remove specified worksheet from workbook collection.
    /// </summary>
    /// <param name="sheet">Reference on worksheet to remove.</param>
    /// <exception cref="System.ArgumentException">
    /// When specified sheet is last sheet in the workbook.
    /// </exception>
    public void Remove( IWorksheet sheet )
    {
      if( !InnerList.Contains( sheet ) )
        throw new ArgumentOutOfRangeException( "Worksheets collection does not contain specified worksheet." );

      if( m_book.Objects.Count == 1 )
        throw new ArgumentException( "Workbook must contains at least one worksheet. You cannot remove last worksheet.", "sheet" );

      m_book.CheckParseOnDemand();

      sheet.Remove();
    }
    /// <summary>
    /// Removes specified worksheet from the collection.
    /// </summary>
    /// <param name="sheetName">Name of the sheet to remove.</param>
    public void Remove( string sheetName )
    {
      m_book.CheckParseOnDemand();
      Remove( this[ sheetName ] );
    }
    /// <summary>
    /// Removes specified worksheet from the collection.
    /// </summary>
    /// <param name="index">Index of the sheet to remove.</param>
    public void Remove( int index )
    {
      m_book.CheckParseOnDemand();
      Remove( this[ index ] );
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      List<IWorksheet> arrSheets = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )arrSheets[ i ];
        sheet.UpdateStringIndexes( arrNewIndexes );
      }
    }
    /// <summary>
    /// Removes specified worksheet from the collection.
    /// </summary>
    /// <param name="index">Index of the sheet to remove.</param>
    public void InnerRemove( int index )
    {
      int iCount = Count;

      if( index < 0 || index > iCount - 1 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1." );

      if( m_book.Objects.Count == 1 )//InnerList.Count == 1 )
        throw new ArgumentException( "Workbook at least must contains one worksheet. You cannot remove last worksheet.", "sheet" );

      IWorksheet sheet = this[ index ];
      int iRealIndex = ( ( ISerializableNamedObject )sheet ).RealIndex;
      base.RemoveAt( index );

      WorkbookObjectsCollection objects = m_book.Objects;
      objects.RemoveAt( iRealIndex );

      for( int i = iRealIndex, len = objects.Count; i < len; i++ )
      {
        objects[ i ].RealIndex = i;
      }

      if( m_book.ActiveSheet == sheet )
      {
        m_book.SetActiveWorksheet( this[ 0 ] as WorksheetBaseImpl );
      }

      for( int i = index + 1; i < iCount; i++ )
      {
        ( ( WorksheetImpl )this[ i - 1 ] ).Index = ( i - 1 );
      }
    }
    /// <summary>
    /// Adds worksheet into internal collection.
    /// </summary>
    /// <param name="sheet">Worksheet to add.</param>
    public void InnerAdd( IWorksheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      base.Add( sheet );
    }
    private bool SheetNameValidator(string sheetName)
    {
        for (int count = 0; count < m_sheetNameValidators.Length; count++)
        {
            if (sheetName.Contains(m_sheetNameValidators[count].ToString()))
                return true;
        }
            return false;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        List<IWorksheet> list = InnerList;
        bool result = list[ 0 ].IsRightToLeft;

        for( int i = 1, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          bool curValue = sheet.IsRightToLeft;

          if( curValue != result || !result )
          {
            return false;
          }
        }

        return result;
      }
      set
      {
        List<IWorksheet> list = InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          ITabSheet sheet = list[ i ];
          sheet.IsRightToLeft = value;
        }
      }
    }  
    #endregion

    #region Class event handlers
    private void Objects_TabSheetMoved(object sender, TabSheetMovedEventArgs args)
    {
      ITabSheets tabSheets = ( ITabSheets )sender;
      int iNewIndex = args.NewIndex;

      WorksheetImpl sheet = tabSheets[ iNewIndex ] as WorksheetImpl;

      if( sheet != null )
      {
        int iOldIndex = args.OldIndex;
        UpdateSheetIndex( sheet, iOldIndex );
      }
    }
    #endregion
  }
}
