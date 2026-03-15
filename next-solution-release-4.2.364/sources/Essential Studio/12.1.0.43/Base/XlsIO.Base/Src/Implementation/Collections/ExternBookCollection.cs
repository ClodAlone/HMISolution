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
using System.IO;

using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#else
#if !(WINRT ) && !ClientProfile
using System.Web.UI;
#endif
//using System.Drawing;
#endif


namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection of external workbooks in the worksheet.
  /// </summary>
  public class ExternBookCollection : CollectionBaseEx<ExternWorkbookImpl>
  {
    #region Class constants
    /// <summary>
    /// Default options of StdDocumnt extern name.
    /// </summary>
    private const int StdDocumentOptions = 32746;
    /// <summary>
    /// Sheet index for non-existing sheet.
    /// </summary>
    private const int DEF_NO_SHEET_INDEX = 65534;
    /// <summary>
    /// This URL is written by MS Excel 2003 when referenced file is closed before saving workbook.
    /// </summary>
    internal const string DEF_WRONG_URL_NAME = " ";
      #endregion

    #region Class members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Dictionary key - extern workbook url, value - corresponding extern workbook.
    /// </summary>
    private Dictionary<string, ExternWorkbookImpl> m_hashUrlToBook = new Dictionary<string, ExternWorkbookImpl>();
    /// <summary>
    /// Dictionary key - short name of the workbook, value - workbook.
    /// </summary>
    private Dictionary<string, ExternWorkbookImpl> m_hashShortNameToBook = new Dictionary<string, ExternWorkbookImpl>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection with specified Application and Parent.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public ExternBookCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single extern workbook from the collection.
    /// </summary>
    public ExternWorkbookImpl this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count" );

        return ( ExternWorkbookImpl )List[ index ];
      }
    }
    /// <summary>
    /// Returns single extern workbook from the collection.
    /// </summary>
    public ExternWorkbookImpl this[ string strUrl ]
    {
      get
      {
        if( strUrl == null || strUrl.Length == 0 )
          return null;

        ExternWorkbookImpl result;

        m_hashUrlToBook.TryGetValue( strUrl, out result );
        return result;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book;
      }
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parses array of biff records.
    /// </summary>
    /// <param name="arrData">Array to parse.</param>
    /// <param name="iOffset">Offset to collection data.</param>
    /// <returns>Offset value after parsing all workbooks.</returns>
    [ CLSCompliant( false ) ]
    public int Parse( BiffRecordRaw[] arrData, int iOffset )
    {
      if( arrData == null )
        throw new ArgumentNullException( "arrData" );

      if( iOffset < 0 || iOffset > arrData.Length - 1 )
        throw new ArgumentOutOfRangeException( "iOffset", "Value cannot be less than 0 and greater than arrData.Length - 1" );

      while( arrData[ iOffset ].TypeCode == TBIFFRecord.SupBook )
      {
        ExternWorkbookImpl book = new ExternWorkbookImpl( Application, this );
        book.Index = InnerList.Count;
        iOffset = book.Parse( arrData, iOffset );
        Add( book );
      }

      return iOffset;
    }
    /// <summary>
    /// Extracts extern workbooks from the BiffReader.
    /// </summary>
    /// <param name="reader">Reader to extract records from.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader, IDecryptor decryptor )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      TBIFFRecord code = reader.PeekRecordType();//PeekRecord();

      while( code == TBIFFRecord.SupBook )
      {
        ExternWorkbookImpl book = new ExternWorkbookImpl( Application, this );
        book.Parse( reader, decryptor );
        Add( book );
        //InnerList.Add( book );
        //record = reader.PeekRecord();
        code = reader.PeekRecordType();
      }
    }
    /// <summary>
    /// Serializes collection of external workbooks as biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        ( ( ExternWorkbookImpl ) List[ i ] ).Serialize( records );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds workbook into collection.
    /// </summary>
    /// <param name="book">Book to add.</param>
    /// <returns>Index of the new workbook.</returns>
    public int Add( ExternWorkbookImpl book )
    {
      book.Index = List.Count;
      base.Add( book );
      return Count - 1;
    }
    /// <summary>
    /// Adds new workbook into collection
    /// </summary>
    /// <param name="fileName">Workbook to add.</param>
    /// <returns>Index of the new workbook.</returns>
    public int Add( string fileName )
    {
      return Add( fileName, false );
    }
    /// <summary>
    /// Adds new workbook into collection
    /// </summary>
    /// <param name="fileName">Workbook to add.</param>
    /// <returns>Index of the new workbook.</returns>
    public int Add( string fileName, bool bAddInFunctions )
    {
//      if( fileName == null )
//        throw new ArgumentNullException( "fileName" );
//
//      if( fileName.Length == 0 )
//        throw new ArgumentException( "fileName - string cannot be empty" );

      ExternWorkbookImpl book = new ExternWorkbookImpl( Application, this );
      book.IsInternalReference = false;
      book.IsAddInFunctions = true;
#if ( WINRT )
        //TODO:WINRT
      book.URL = fileName;
#else
      book.URL = ( fileName != null )
        ? Path.GetFullPath( fileName )
        : fileName;
#endif   
      int iResult = Add( book );

      int iSheetCount = book.SheetNumber;

      int iFirstSheet = ( iSheetCount == 0 )
        ? DEF_NO_SHEET_INDEX
        : 0;

      int iLastSheet = ( iSheetCount == 0 )
        ? DEF_NO_SHEET_INDEX
        : 0;

      m_book.AddSheetReference( iResult, iFirstSheet, iLastSheet );

      return iResult;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <param name="sheets"></param>
    /// <param name="names"></param>
    /// <returns></returns>
    public int Add( string filePath, string fileName, List<string> sheets, string[] names )
    {
      if( fileName == null || fileName.Length == 0 )
        throw new ArgumentOutOfRangeException( "fileName" );

      ExternWorkbookImpl book = new ExternWorkbookImpl( Application, this );
      book.IsInternalReference = false;
      book.URL = ( filePath == null ) ? fileName : filePath + fileName;

      int iResult = Add( book );

      int iSheetsCount = ( sheets != null ) ? sheets.Count : 0;
      book.SheetNumber = iSheetsCount;
      book.AddWorksheets( sheets );
      book.AddNames( names );

      //ExternNamesCollection arrExternNames = book.ExternNames;
      //int iNameIndex = arrExternNames.Add( "StdDocument" );
      //ExternNameImpl externName = arrExternNames[ iNameIndex ];
      //externName.Record.Options = StdDocumentOptions;

      return iResult;
    }
    /// <summary>
    /// Adds new workbook into collection
    /// </summary>
    /// <param name="fileName">Workbook to add.</param>
    /// <returns>Index of the new workbook.</returns>
    public int AddDDEFile(string fileName)
    {
        ExternWorkbookImpl book = new ExternWorkbookImpl(Application, this);
        book.IsInternalReference = false;
        book.URL = fileName;

        int iResult = Add(book);

        book.SheetNumber = 0;
        int iFirstSheet = DEF_NO_SHEET_INDEX;
        int iLastSheet = DEF_NO_SHEET_INDEX;

        m_book.AddSheetReference(iResult, iFirstSheet, iLastSheet);

        ExternNamesCollection arrExternNames = book.ExternNames;
        int iNameIndex = arrExternNames.Add("StdDocument");
        ExternNameImpl externName = arrExternNames[iNameIndex];
        externName.Record.Options = StdDocumentOptions;

        return iResult;
    }
    /// <summary>
    /// Inserts SupbookRecord describing this workbook.
    /// </summary>
    /// <returns>
    /// Index to the SupBookRecord that describes current workbook.
    /// </returns>
    public int InsertSelfSupbook()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        //SupBookRecord supBook = m_arrSupBooks[ i ] as SupBookRecord;
        ExternWorkbookImpl book = this[ i ];

        if( book.IsInternalReference )
        {
          book.SheetNumber = ( ushort ) ( m_book.Worksheets.Count + m_book.Charts.Count );
          return i;
        }
      }

      ExternWorkbookImpl newBook = new ExternWorkbookImpl( Application, this );
      newBook.Index = List.Count;
      newBook.IsInternalReference = true;
      newBook.SheetNumber = ( ushort ) ( m_book.Worksheets.Count + m_book.Charts.Count );
      base.Add( newBook );
      
      return Count - 1;
    }
    /// <summary>
    /// Checks whether any of books in this collection contains extern name.
    /// </summary>
    /// <param name="strName">Name to search.</param>
    /// <returns>True if name was found; false otherwise.</returns>
    public bool ContainsExternName( string strName )
    {
      //return m_hashNames.Contains( strName );
      for( int i = 0, len = Count; i < len; i++ )
      {
        ExternWorkbookImpl externBook = this[ i ];

        if( externBook.ExternNames.Contains( strName ) )
          return true;
      }

      return false;
    }
    /// <summary>
    /// Checks whether any of books in this collection contains extern name.
    /// </summary>
    /// <param name="strName">Name to search.</param>
    /// <param name="iBookIndex">Output extern workbook index.</param>
    /// <param name="iNameIndex">Output name index.</param>
    /// <returns>True if name was found; false otherwise.</returns>
    public bool ContainsExternName( string strName, ref int iBookIndex, ref int iNameIndex )
    {
      //return m_hashNames.Contains( strName );
      for( int i = 0, len = Count; i < len; i++ )
      {
        ExternWorkbookImpl externBook = this[ i ];

        iNameIndex = externBook.ExternNames.GetNameIndex( strName );

        if( iNameIndex >= 0 )
        {
          //iBookIndex = i;
          iBookIndex = m_book.AddSheetReference( externBook.Index, DEF_NO_SHEET_INDEX, DEF_NO_SHEET_INDEX );
          return true;
        }
      }

      return false;
    }
    /// <summary>
    /// Returns index of the extern name.
    /// </summary>
    /// <param name="strName">Name to search.</param>
    /// <param name="iRefIndex">Reference index.</param>
    /// <returns>Returns index to extern workbook containing required name.</returns>
    public int GetNameIndexes( string strName, out int iRefIndex )
    {
      iRefIndex = -1;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ExternWorkbookImpl book = this[ i ];
        int index = book.ExternNames.GetNameIndex( strName );

        if( index != -1 )
        {
          iRefIndex = index;
          return i;
        }
      }

      return -1;
    }
    /// <summary>
    /// Returns extern workbook with specified short name.
    /// </summary>
    /// <param name="strShortName">Short name to find.</param>
    /// <returns>
    /// Extern workbook that corresponds to the specified short name;
    ///  or Null if there isn't such workbook.
    ///  </returns>
    public ExternWorkbookImpl GetBookByShortName( string strShortName )
    {
      if( strShortName == null )
        throw new ArgumentNullException( "strShortName" );

      if( strShortName.Length == 0 )
        throw new ArgumentException( "strShortName - string cannot be empty" );

      ExternWorkbookImpl result;
      m_hashShortNameToBook.TryGetValue( strShortName, out result );

      return result;
    }
    /// <summary>
    /// Sets all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "m_book" );
    }
    /// <summary>
    /// Returns first index of current book in collection.
    /// </summary>
    /// <returns>Returns first index of current book in collection.</returns>
    public int GetFirstInternalIndex()
    {
      for( int i = 0, ilen = List.Count; i < ilen; i++ )
      {
        ExternWorkbookImpl externBook = ( ExternWorkbookImpl )List[ i ];

        if( externBook.IsInternalReference )
          return i;
      }

      return -1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected override void OnInsertComplete(int index, ExternWorkbookImpl value)
    {
      base.OnInsertComplete (index, value);

      //ExternNamesCollection names = book.ExternNames;
      ExternWorkbookImpl book = ( ExternWorkbookImpl )value;
      book.Index = List.Count - 1;

      if( !book.IsInternalReference )
      {
        string strURL = book.URL;

        if( strURL != null && strURL != DEF_WRONG_URL_NAME )
        {
          // there can be different workbooks that are referencing the same document in result of decoding.
          if( !m_hashUrlToBook.ContainsKey( strURL ) || !m_book.Loading )
            m_hashUrlToBook.Add( strURL, book );

          string strShortName = book.ShortName;

          if( !m_hashShortNameToBook.ContainsKey( strShortName ) )
          {
            m_hashShortNameToBook.Add( strShortName, book );
          }
        }
      }
    }
    /// <summary>
    /// Copies subbook collection.
    /// </summary>
    /// <param name="subBooks">Represents parent subbook collection.</param>
    /// <returns>Returns sub book indexes hash. Key - old indexes; Value - new indexes.</returns>
    public Dictionary<int, int> AddCopy( ExternBookCollection subBooks )
    {
      if( subBooks == null )
        throw new ArgumentNullException( "subBooks" );

      //ExternBookCollection curSubBooks = ExternWorkbooks;
      Dictionary<int, int> result = new Dictionary<int, int>();
      // 1. Find self reference
      int iSelfReferenceIndex = GetFirstInternalIndex();

      for( int i = 0, iLen = subBooks.Count; i < iLen; i++ )
      {
        ExternWorkbookImpl externBook = subBooks[ i ];
        int iIndex = -1;
        ExternWorkbookImpl presentBook = this[ externBook.URL ];

        if( externBook.IsInternalReference && iSelfReferenceIndex >= 0 )
        {
          iIndex = iSelfReferenceIndex;
        }
        else if( presentBook == null )
        {
          externBook = ( ExternWorkbookImpl )externBook.Clone( this );
          iIndex = Add( externBook );
        }
        else
        {
          iIndex = presentBook.Index;
        }

        result.Add( i, iIndex );
      }

      return result;
    }
    /// <summary>
    /// Tries to find corresponding workbook or creates new if not found.
    /// </summary>
    /// <param name="strBook">Name of the workbook file.</param>
    /// <param name="strBookPath">Path to the workbook.</param>
    /// <returns>Found or created extern workbook.</returns>
    internal ExternWorkbookImpl FindOrAdd( string strBook, string strBookPath )
    {
      if( strBook == null || strBook.Length == 0 )
        throw new ArgumentOutOfRangeException( "strBook" );

      string strUrl = ( strBookPath == null ) ? strBook : strBookPath + strBook;
      ExternWorkbookImpl result = null;

      if( m_hashUrlToBook.ContainsKey( strUrl ) )
      {
        result = m_hashUrlToBook[ strUrl ];
      }
      else if( ( strBook == null || strBook.Length == 0 ) && m_hashShortNameToBook.ContainsKey( strBook ) )
      {
        result = m_hashShortNameToBook[ strBook ];
      }
      else
      {
        // Here we have to add new external workbook.
        result = this[ Add( strBookPath, strBook, null, null ) ];
      }

      return result;
    }
    /// <summary>
    /// Frees all allocated unmanaged resources.
    /// </summary>
    internal void Dispose()
    {
      for( int i = Count - 1; i >= 0; i-- )
      {
        ExternWorkbookImpl book = this[ i ];
        book.Dispose();
      }

      Clear();
    }
    internal int Add( string fileName, WorkbookImpl book, IRange sourceRange )
    {
      ExternWorkbookImpl newBook = new ExternWorkbookImpl( Application, this );
      newBook.IsInternalReference = false;
      newBook.IsAddInFunctions = false;
#if ( WINRT )
        if(fileName!=null)
            newBook.URL=fileName;
#else
      newBook.URL = ( fileName != null )
        ? Path.GetFullPath( fileName )
        : fileName;
#endif
      int iResult = Add( newBook );
      IWorksheets sheets = sourceRange.Worksheet.Workbook.Worksheets;
      ExternWorksheetImpl parentSheet = null;
      string parentSheetName = sourceRange.Worksheet.Name;

      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        IWorksheet sheet = sheets[ i ];
        string strSheetName = sheet.Name;
        ExternWorksheetImpl newSheet = newBook.AddWorksheet( strSheetName );

        if( strSheetName == parentSheetName )
        {
          parentSheet = newSheet;
        }
      }

      int iSheetCount = newBook.SheetNumber;

      int iFirstSheet = ( iSheetCount == 0 )
        ? DEF_NO_SHEET_INDEX
        : parentSheet.Index;

      int iLastSheet = ( iSheetCount == 0 )
        ? DEF_NO_SHEET_INDEX
        : parentSheet.Index;

      parentSheet.CacheValues( sourceRange );
      m_book.AddSheetReference( iResult, iFirstSheet, iLastSheet );

      return iResult;
    }
    #endregion
  }
}
