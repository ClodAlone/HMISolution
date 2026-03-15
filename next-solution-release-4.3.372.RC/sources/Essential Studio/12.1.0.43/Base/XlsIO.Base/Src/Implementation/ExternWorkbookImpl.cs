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

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
#endif


namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Describes external workbook.
  /// </summary>
  public class ExternWorkbookImpl
    : CommonObject
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Array of all worksheets in this workbook. Key - index, Value - ExternWorksheet.
    /// </summary>
    private SortedList<int, ExternWorksheetImpl> m_arrSheets = new SortedList<int, ExternWorksheetImpl>();
    /// <summary>
    /// Dictionary key - worksheet name, value - corresponding worksheet.
    /// </summary>
    private Dictionary<string, ExternWorksheetImpl> m_hashNameToSheet = new Dictionary<string, ExternWorksheetImpl>();
    /// <summary>
    /// Array of all extern names in this workbook.
    /// </summary>
    private ExternNamesCollection m_externNames;
    /// <summary>
    /// Corresponding SupBookRecord.
    /// </summary>
    private SupBookRecord m_supBook;
    /// <summary>
    /// Book index.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Workbook's short name.
    /// </summary>
    private string m_strShortName;
    /// <summary>
    /// Program id for ole object links.
    /// </summary>
    private string m_strProgramId;
    #endregion

    #region Class initialize / finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ExternWorkbookImpl( IApplication application, object parent )
      : base( application, parent )
    {
      InitializeVariables();
    }
    /// <summary>
    /// Initializes internal variables.
    /// </summary>
    private void InitializeVariables()
    {
      FindParents();
      m_externNames = new ExternNamesCollection( Application, this );
      m_supBook = ( SupBookRecord )BiffRecordFactory.GetRecord( TBIFFRecord.SupBook );
      m_supBook.SheetNames = new List<string>();

      InitShortName();
    }
    /// <summary>
    /// Inserts default worksheet.
    /// </summary>
    public void InsertDefaultWorksheet()
    {
      m_supBook.SheetNames.Add( "Sheet1" );
      //m_supBook.SheetNumber = 1;

      ExternWorksheetImpl sheet = new ExternWorksheetImpl( Application, this );
      sheet.Index = 0;
      m_arrSheets.Add( 0, sheet );
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "m_book" );

      m_supBook = ( SupBookRecord )CloneUtils.CloneCloneable( m_supBook );
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Parses extern workbook.
    /// </summary>
    /// <param name="arrData">Records array that contains workbook data.</param>
    /// <param name="iOffset">Offset to the workbook data.</param>
    /// <returns>Offset after reading all necessary data.</returns>
    [ CLSCompliant( false ) ]
    public int Parse( BiffRecordRaw[] arrData, int iOffset )
    {
      throw new NotImplementedException();
//      if( arrData == null )
//        throw new ArgumentNullException( "arrData" );
//
//      if( iOffset < 0 || iOffset > arrData.Length - 1 )
//        throw new ArgumentOutOfRangeException( "iOffset", iOffset, "Value cannot be less than 0 and greater than arrData.Length - 1" );
//
//      m_arrSheets.Clear();
//      m_externNames.Clear();
//
//      BiffRecordRaw record = arrData[ iOffset ];
//      record.CheckTypeCode( TBIFFRecord.SupBook );
//
//      m_supBook = ( SupBookRecord )arrData[ iOffset++ ];
//
//      int iSheets = m_supBook.SheetNumber;
//      int iNameIndex = 0;
//
//      while( arrData[ iOffset ].TypeCode == TBIFFRecord.ExternName )
//      {
//        ExternNameRecord name = ( ExternNameRecord )arrData[ iOffset ];
//        m_externNames.Add( name );
//        
//        if( name.FormulaSize == 0 )
//        {
//          m_book.InnerAddInFunctions.Add( m_iIndex, iNameIndex );
//        }
//
//        iOffset++;
//        iNameIndex++;
//      }
//
//      while( arrData[ iOffset ].TypeCode == TBIFFRecord.ExternName )
//      {
//        ExternWorksheetImpl sheet = new ExternWorksheetImpl( Application, this );
//        iOffset = sheet.Parse( arrData, iOffset );
//        m_arrSheets.Add( sheet.Index, sheet );
//      }
//
//      return iOffset;
    }
    /// <summary>
    /// Parses extern workbook.
    /// </summary>
    /// <param name="reader">Reader with workbook data.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public void Parse( BiffReader reader, IDecryptor decryptor )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      m_arrSheets.Clear();
      m_externNames.Clear();

      BiffRecordRaw record = reader.GetRecord( decryptor );//reader.PeekRecord();
      record.CheckTypeCode( TBIFFRecord.SupBook );

      m_supBook = ( SupBookRecord )record;

      string strUrl = m_supBook.URL;

      if( !m_supBook.IsInternalReference && strUrl != null )
      {
        m_supBook.URL = m_book.DecodeName( strUrl );
      }

      //reader.GetRecord();

      int iSheets = m_supBook.SheetNumber;
      TBIFFRecord recordCode = reader.PeekRecordType();
      int iNameIndex = 0;

      while( recordCode == TBIFFRecord.ExternName )
      {
        record = reader.GetRecord( decryptor );
        ExternNameRecord name = ( ExternNameRecord )record;
        m_externNames.Add( name );

        if( name.FormulaSize == 0 || m_supBook.IsAddInFunctions )
        {
          m_book.InnerAddInFunctions.Add( m_iIndex, iNameIndex );
        }

        iNameIndex++;
        recordCode = reader.PeekRecordType();
      }

      // Create all worksheets based on their names.
      List<string> sheetNames = m_supBook.SheetNames;

      if( m_arrSheets.Count == 0 && iSheets > 0 && sheetNames != null )
      {
        for( int i = 0, len = sheetNames.Count; i < len; i++ )
        {
          ExternWorksheetImpl sheet = new ExternWorksheetImpl( Application, this );
          sheet.Name = sheetNames[ i ];
          sheet.Index = i;
          AddExternSheet( sheet );
        }
      }

      // Extract worksheet's preserved data.
      while( recordCode == TBIFFRecord.XCT )
      {
        ExternWorksheetImpl sheet = new ExternWorksheetImpl( Application, this );
        sheet.Parse( reader, decryptor );
        AddExternSheet( sheet );

        recordCode = reader.PeekRecordType();
      }

      InitShortName();
    }
    /// <summary>
    /// Saves extern workbook as biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      SupBookRecord supbook = m_supBook;

      if( !m_supBook.IsInternalReference && supbook.URL != null )
      {
        supbook = ( SupBookRecord )m_supBook.Clone();
        supbook.URL = m_book.EncodeName( m_supBook.URL );
      }

      records.Add( supbook );
      m_externNames.Serialize( records );

      if( !IsInternalReference )
      {
        IList<ExternWorksheetImpl> arrSheets = m_arrSheets.Values;

        for( int i = 0, len = m_arrSheets.Count; i < len; i++ )
        {
          ExternWorksheetImpl sheet = arrSheets[ i ];
          sheet.Serialize( records );
        }
      }
    }
    /// <summary>
    /// Adds extern worksheet.
    /// </summary>
    /// <param name="sheet">Sheet to add.</param>
    private void AddExternSheet( ExternWorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      int iIndex = sheet.Index;
      m_arrSheets[ iIndex ] = sheet;

      List<string> arrNames = m_supBook.SheetNames;
      int iSheetNameCount = arrNames.Count;

      if( iIndex < iSheetNameCount )
      {
        string strSheetName = m_supBook.SheetNames[ iIndex ];
        m_hashNameToSheet[ strSheetName ] = sheet;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns collection of extern names. Read-only.
    /// </summary>
    public ExternNamesCollection ExternNames
    {
      get
      {
        return m_externNames;
      }
    }
    /// <summary>
    /// Indicates whether this is workbook is used for internal reference.
    /// </summary>
    public bool IsInternalReference
    {
      get
      {
        return m_supBook.IsInternalReference;
      }
      set
      {
        m_supBook.IsInternalReference = value;
      }
    }
    public bool IsOleLink
    {
      get
      {
        return ( m_externNames != null && m_externNames.Count == 1 && m_externNames[ 0 ].Record.OleLink );
      }
    }
    /// <summary>
    /// Number of sheet names (if external references) or
    /// number of sheets in this document (if internal references).
    /// </summary>
    public int SheetNumber
    {
      get
      {
        return m_supBook.SheetNumber;
      }
      set
      {
        m_supBook.SheetNumber = ( ushort )value;
      }
    }
    /// <summary>
    /// Encoded URL without sheet name (for external references).
    /// </summary>
    public string URL
    {
      get
      {
        return m_supBook.URL;
      }
      set
      {
//        if( value == null )
//          throw new ArgumentNullException( "URL" );

        m_supBook.URL = value;
        InitShortName();

        if( value == null )
        {
          m_arrSheets.Clear();
          m_hashNameToSheet.Clear();
          m_supBook.SheetNames = null;
        }
      }
    }
    /// <summary>
    /// Gets / sets index of the workbook.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Returns short name of the workbook. Read-only.
    /// </summary>
    public string ShortName
    {
      get
      {
        return m_strShortName;
      }
    }
    /// <summary>
    /// Indicates whether add-in function names are stored 
    /// in EXTERNNAME records following this SUPBOOK record.
    /// </summary>
    public bool IsAddInFunctions
    {
      get
      {
        return m_supBook.IsAddInFunctions;
      }
      set
      {
        m_supBook.IsAddInFunctions = value;
      }
    }
    /// <summary>
    /// Returns worksheets collection sorted by index. Read-only.
    /// </summary>
    public SortedList<int, ExternWorksheetImpl> Worksheets
    {
      get
      {
        return m_arrSheets;
      }
    }
    /// <summary>
    /// Gets or sets program id for the ole object.
    /// </summary>
    public string ProgramId
    {
      get
      {
        return m_strProgramId;
      }
      set
      {
        m_strProgramId = value;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Returns index of worksheet in this workbook.
    /// </summary>
    /// <param name="strSheetName">Sheet name to search for.</param>
    /// <returns>Index of the worksheet in the workbook; -1 if worksheet was not found.</returns>
    public int IndexOf( string strSheetName )
    {
      if( strSheetName == null || strSheetName.Length == 0 ) return -1;

      ExternWorksheetImpl sheet;

      return ( m_hashNameToSheet.TryGetValue( strSheetName, out sheet ) ) ?
        sheet.Index :
        -1;
    }

  public void saveAsHtml(string FileName)
  {

  }
    /// <summary>
    /// Get new index for extern name (to remove duplicated extern names).
    /// </summary>
    /// <param name="iNameIndex">Name index.</param>
    /// <returns>Updated name index.</returns>
    public int GetNewIndex( int iNameIndex )
    {
      return m_externNames.GetNewIndex( iNameIndex );
    }
    /// <summary>
    /// Creates copy of the current extern workbook.
    /// </summary>
    /// <param name="parent">Parent for the copy of this extern workbook.</param>
    /// <returns>Copy of the current extern workbook.</returns>
    public object Clone( object parent )
    {
      ExternWorkbookImpl result = ( ExternWorkbookImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();
      result.m_arrSheets = new SortedList<int, ExternWorksheetImpl>();
      IList<int> keys = m_arrSheets.Keys;
      IList<ExternWorksheetImpl> values = m_arrSheets.Values;

      for( int i = 0, len = m_arrSheets.Count; i < len; i++ )
      {
        int iIndex = keys[ i ];
        ExternWorksheetImpl sheet = values[ i ];
        sheet = ( ExternWorksheetImpl )sheet.Clone( result );
        result.AddExternSheet( sheet );
      }

      result.m_externNames = ( ExternNamesCollection )m_externNames.Clone( this );
      return result;
    }
    /// <summary>
    /// Gets name of the sheet at specified index.
    /// </summary>
    /// <param name="index">Index of the desired worksheet.</param>
    /// <returns>Name of the sheet.</returns>
    public string GetSheetName( int index )
    {
        if (index == WorkbookImpl.DEF_REMOVED_SHEET_INDEX)
            return WorkbookImpl.DEF_BAD_SHEET_NAME;
        else
        {
            if (index == WorkbookImpl.DEF_REMOVED_SHEET_INDEX - 1)
            {
                index = 0;
            }
            return m_supBook.SheetNames[index];
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Initializes ShortName property.
    /// </summary>
    private void InitShortName()
    {
      string strURL = m_supBook.URL;

      m_strShortName = ( strURL != null )
        ? GetFileName( m_supBook.URL )
        : null;
    }
    /// <summary>
    /// Gets file name with extension from the path.
    /// </summary>
    /// <param name="strUrl">Url to get name from.</param>
    /// <returns>File name without extension from the path.</returns>
    private static string GetFileName( string strUrl )
    {
      if( strUrl == null || strUrl.Length == 0 ) return strUrl;

      int iLastSlash = strUrl.LastIndexOf( '\\' );
      int iEndIndex = strUrl.Length;
      int iStartIndex = 0;

      if( iLastSlash > 0 )
      {
        iStartIndex = iLastSlash + 1;
      }

      return strUrl.Substring( iStartIndex, iEndIndex - iStartIndex );
    }
    /// <summary>
    /// Gets file name without extension from the path.
    /// </summary>
    /// <param name="strUrl">Url to get name from.</param>
    /// <returns>File name without extension from the path.</returns>
    private static string GetFileNameWithoutExtension( string strUrl )
    {
      if( strUrl == null || strUrl.Length == 0 ) return strUrl;

      int iLastSlash = strUrl.LastIndexOf( '\\' );
      int iLastDot = strUrl.LastIndexOf( '.' );

      int iStartIndex = 0;
      int iEndIndex = strUrl.Length;

      if( iLastSlash > 0 )
      {
        iStartIndex = iLastSlash + 1;
      }

      if( iLastDot > iStartIndex )
      {
        iEndIndex = iLastDot;
      }

      return strUrl.Substring( iStartIndex, iEndIndex - iStartIndex );
    }
    /// <summary>
    /// Adds new worksheets to the extern workbook.
    /// </summary>
    /// <param name="sheets">Array that contains worksheet names to add.</param>
    public void AddWorksheets( List<string> sheets )
    {
      //throw new Exception( "The method or operation is not implemented." );
      int iCount = ( sheets != null ) ? sheets.Count : 0;

      if( iCount == 0 )
        return;

      //ExternWorksheetImpl arrSheets = new ExternWorksheetImpl[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        AddWorksheet( sheets[ i ] );
      }
    }
    /// <summary>
    /// Adds new worksheets to the extern workbook.
    /// </summary>
    /// <param name="sheets">Array that contains worksheet names to add.</param>
    public void AddWorksheets( string[] sheets )
    {
      //throw new Exception( "The method or operation is not implemented." );
      int iCount = ( sheets != null ) ? sheets.Length : 0;

      if( iCount == 0 )
        return;

      //ExternWorksheetImpl arrSheets = new ExternWorksheetImpl[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        AddWorksheet( sheets[ i ] );
      }
    }
    /// <summary>
    /// Adds new worksheet to the collection of worksheets.
    /// </summary>
    /// <param name="sheetName">Name of the worksheet to add.</param>
    /// <returns>Create worksheet.</returns>
    public ExternWorksheetImpl AddWorksheet( string sheetName )
    {
        if (sheetName == null)
            throw new ArgumentOutOfRangeException("sheetName");

      ExternWorksheetImpl sheet = new ExternWorksheetImpl( Application, this );
      int iIndex = m_arrSheets.Count;
      sheet.Index = iIndex;
      sheet.Name = sheetName;
      m_arrSheets.Add( iIndex, sheet );
      m_hashNameToSheet.Add( sheetName, sheet );
      m_supBook.SheetNames.Add( sheetName );

      return sheet;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="names"></param>
    public void AddNames( string[] names )
    {
      //throw new Exception( "The method or operation is not implemented." );
      int iCount = ( names != null ) ? names.Length : 0;

      for( int i = 0; i < iCount; i++ )
      {
        AddName( names[ i ] );
      }
    }
    /// <summary>
    /// Adds external name object to the workbook.
    /// </summary>
    /// <param name="name">Name of the named range to add.</param>
    public void AddName( string name )
    {
      m_externNames.Add( name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    internal int FindOrAddSheet( string sheetName )
    {
      if( sheetName == null || sheetName.Length == 0 )
        throw new ArgumentOutOfRangeException( "sheetName" );

      ExternWorksheetImpl result;

      if( !m_hashNameToSheet.TryGetValue( sheetName, out result ) )
      {
        result = AddWorksheet( sheetName );
      }

      return result.Index;
    }
    /// <summary>
    /// This method is called during dispose operation.
    /// </summary>
    protected override void OnDispose()
    {
      if( !m_bIsDisposed )
      {
        if( m_arrSheets != null )
        {
          foreach( ExternWorksheetImpl sheet in m_arrSheets.Values )
          {
            sheet.Dispose();
          }

          m_arrSheets.Clear();
          m_arrSheets = null;
        }

        base.OnDispose();
      }
    }
    #endregion
  }
}
