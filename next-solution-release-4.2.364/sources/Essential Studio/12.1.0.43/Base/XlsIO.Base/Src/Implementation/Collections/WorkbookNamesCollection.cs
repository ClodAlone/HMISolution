#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
#endif

#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection of the Workbook names (NamedRanges).
  /// </summary>
  public class WorkbookNamesCollection
    : CollectionBaseEx<IName>
    , INames
  {
    #region Class Constants

      char[] SpecialChars = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
                                '-', '=', '+', ']', '}', '[', '{', ';', ':', '/', '.', '>', '<' };

    #endregion

    #region Class members
    /// <summary>
    /// Dictionary Name-to-IName.
    /// </summary>
    private Dictionary<string, IName> m_hashNameToIName = new Dictionary<string, IName>();
    /// <summary>
    /// Parent workbook for the collection.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Flags indicating whether WorkbookNamesCollection changed.
    /// </summary>
    private bool m_bWorkNamesChanged;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates an empty collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public WorkbookNamesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParent();
    }
    #endregion

    #region INames Properties
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    public IName this[ int index ]
    {
      get
      {
        if( index < 0 || index >= List.Count )
          throw new ArgumentOutOfRangeException( 
            string.Format( "index is {0}, Count is {1}", index, List.Count ) );

        return ( IName )List[ index ];
      }
    }
    /// <summary>
    /// Returns a single Name object from a Names collection.
    /// </summary>
    public IName this[ string name ]
    {
      get
      {
        IName result;

        m_hashNameToIName.TryGetValue( name, out result );
        return result;
      }
    }
    /// <summary>
    /// Returns parent worksheet of the collection.
    /// </summary>
    public IWorksheet ParentWorksheet
    {
      get
      {
        return null;
      }
    }
      /// <summary>
      /// Represent the known named ranges count
      /// The name in the formula which has no reference is 
      /// unknown named ranges
      /// </summary>
    int INames.Count
    {
        get
        {
            return GetKnownNamedCount();
        }
    }
      /// <summary>
      /// Represents the known and unknown named
      /// ranges Count
      /// </summary>
    public int Count
    {
        get
        {
            return List.Count;
        }
    }
    #endregion

    #region INames methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IName Add( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length == 0 )
        throw new ArgumentException( "name" );

      bool bResult = IsValidName(name, m_book);
      if (!bResult || char.IsNumber(name [0]))
          throw new ArgumentException("This is not a valid name. Name should not be same as the cell name.");
      
      NameImpl newName = new NameImpl( Application, m_book, 
        name, List.Count );

      Add( newName );

      return newName;
    }

    /// <summary>
    /// Checks whether name of named range contains invalid characters.
    /// </summary>
    /// <param name="name"></param>
    private void CheckInvalidCharacters(string name)
    {
        string questionMark = "?";
        int indexOf = name.IndexOfAny(SpecialChars);

        if (indexOf != -1 || name.Contains("\"") || name.StartsWith(questionMark))
        {
            throw new ArgumentException("Contains invalid characters");
        }

        char number = name[0];

        if (char.IsNumber(number))
        {
            throw new ArgumentException("Contains invalid characters");
        }

        int charCount = 0;
        int numberCount = 0;
        bool bfinalChar = false;
        int length = name.Length;

        foreach (char c in name)
        {
            if (char.IsLetter(c))
            {
                charCount++;
            }
            else if (char.IsNumber(c))
                numberCount++;


        }
        if (char.IsLetter(name[length - 1]) || name.EndsWith(questionMark))
            bfinalChar = true;
        if (charCount <= 3 && numberCount > 0 && !bfinalChar)
            throw new ArgumentException("Contains invalid characters");
    }

      protected override void OnClearComplete()
    {
        m_hashNameToIName.Clear();
    }
    /// <summary>
    /// Defines a new name. Returns a Name object.
    /// </summary>
    /// <param name="name">Name of the new collection entry.</param>
    /// <param name="namedRange">Range that will be associated with the name.</param>
    /// <returns>Created IName object.</returns>
    /// <exception cref="System.ArgumentException">
    /// If the collection already contains object with the same name
    /// or if specified name string is empty.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// When one of the arguments is NULL.
    /// </exception>
    public IName Add( string name, IRange namedRange )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( namedRange == null )
        throw new ArgumentNullException( "namedRange" );

      if( name.Length == 0 )
        throw new ArgumentException( "name" );
        
      bool bResult = IsValidName(name,m_book);
      if (!bResult)
          throw new ArgumentException("This is not a valid name. Name should not be same as the cell name.");

      NameImpl newName = new NameImpl( Application, this,
        name, namedRange, List.Count );

      Add( newName );

      return newName;
    }
      /// <summary>
      /// Checks the namedRange's name is a valid name based on the 
      /// Excel version.
      /// </summary>
      /// <param name="name">name to check.</param>
      /// <param name="book">Active workbook.</param>
      /// <returns>True, if the name is valid.</returns>
    internal static bool IsValidName(string name,WorkbookImpl book)
    {
        string row, col;
        bool bIsR1C1 = FormulaUtil.IsR1C1(name);
        bool bResult= FormulaUtil.IsCell(name, bIsR1C1, out row, out col);
        if (row == null || col == null)
            return true;

        int iColumn=RangeImpl.GetColumnIndex(col);
        int iRow = Convert.ToInt32(row);
        if (bResult && iRow < book.MaxRowCount && iColumn < book.MaxColumnCount && iRow !=0 )
            bResult = true;
        else
            bResult = false;
        return !bResult;
    }
    internal void Validate()
    {
        foreach (NameImpl nameImpl in this)
        {
            if (!IsValidName(nameImpl.Name, m_book))
                throw new Exception("Named Range "+nameImpl.Name+" is not supported in this version");
        }
    }
    /// <summary>
    /// Defines a new name. Returns a Name object.
    /// </summary>
    /// <param name="name">IName object that must be added to the collection.</param>
    /// <returns>Created IName object.</returns>
    /// <exception cref="System.ArgumentException">
    /// If the collection already contains object with the same name.
    /// </exception>
    public IName Add( IName name )
    {
      NameImpl nameImpl = name as NameImpl;
      bool bExternName = nameImpl.IsExternName;

      if( !m_book.Loading && !bExternName && !nameImpl.IsLocal &&
        m_hashNameToIName.ContainsKey( name.Name ) )
      {
        throw new ArgumentException( "Name of the Name object must be unique." );
      }

      AddLocal( name );

//      if( !( nameImpl.IsBuiltIn || bExternName || nameImpl.IsLocal ) )
//        m_hashNameToIName.Add( name.Name, name );

      IsWorkbookNamesChanged = true;

      return name;
    }
    /// <summary>
    /// Removes specified name from the collection.
    /// </summary>
    /// <param name="name">Name of the object to remove.</param>
    public void Remove( string name )
    {
      IName toDel;

      if( m_hashNameToIName.TryGetValue( name, out toDel ) )
      {
        RemoveAt( toDel.Index );
      }
    }
    /// <summary>
    /// Removes specified name from the collection.
    /// </summary>
    /// <param name="index">Name of the object to remove.</param>
    new public void RemoveAt( int index )
    {
      if( index < 0 || index > Count - 1 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1." );

      IName name = this[ index ];
      if (m_hashNameToIName.ContainsValue(name))
          m_hashNameToIName.Remove( name.Name );

      IList<IName> arrNames = List;
      arrNames.RemoveAt( index );

      IsWorkbookNamesChanged = true;

      Dictionary<int, int> hash = new Dictionary<int,int>();
      for( int i = index, iCount = arrNames.Count; i < iCount; i++ )
      {
        NameImpl namedRange = ( NameImpl )arrNames[ i ];
        hash.Add( namedRange.Index, i );
        namedRange.SetIndex( i );
      }

      m_book.UpdateNamedRangeIndexes( hash );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrIndexes"></param>
    public void Remove( int[] arrIndexes )
    {
      List<int> lstIndexes = new List<int>( arrIndexes );
      lstIndexes.Sort();
      int iCount = List.Count;

      for( int i = lstIndexes.Count - 1; i >= 0; i-- )
      {
        int index = lstIndexes[ i ];

        if( index < 0 || index >= iCount )
        {
          throw new ArgumentOutOfRangeException( "index" );
        }

        IName name = ( IName )List[ index ];
        m_hashNameToIName.Remove( name.Name );
        base.RemoveAt( index );
        IsWorkbookNamesChanged = true;
      }

      Dictionary<int, int> hash = new Dictionary<int,int>();

      for( int i = ( int )lstIndexes[ 0 ], len = Count; i < len; i++ )
      {
        NameImpl name = ( NameImpl )List[ i ];
        hash.Add( name.Index, i );
        name.SetIndex( i );
      }

      m_book.UpdateNamedRangeIndexes( hash );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains( string name )
    {
      return m_hashNameToIName.ContainsKey( name );
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// Updates named range when inserting row.
    /// </summary>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <param name="iRowCount">Represents row count.</param>
    /// <param name="strSheetName">Represents sheet for update.</param>
    public void InsertRow( int iRowIndex, int iRowCount, string strSheetName )
    {
      InsertRemoveRowColumn( strSheetName, iRowIndex, false, true, iRowCount );
    }
    /// <summary>
    /// Updates named range when deleting row.
    /// </summary>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <param name="strSheetName">Represents sheet for update.</param>
    public void RemoveRow( int iRowIndex, string strSheetName )
    {
      InsertRemoveRowColumn( strSheetName, iRowIndex, true, true, 1 );
    }
    /// <summary>
    /// Updates named range when deleting row.
    /// </summary>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <param name="strSheetName">Represents sheet for update.</param>
    /// <param name="count">Number of rows to remove.</param>
    public void RemoveRow( int iRowIndex, string strSheetName, int count )
    {
      InsertRemoveRowColumn( strSheetName, iRowIndex, true, true, count );
    }
    /// <summary>
    /// Updates named ranges after column insertion.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <param name="strSheetName">Represents reference sheet name.</param>
    public void InsertColumn( int iColumnIndex, int iCount, string strSheetName )
    {
      InsertRemoveRowColumn( strSheetName, iColumnIndex, false, false, iCount );
    }
    /// <summary>
    /// Updates named ranges after column removal.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="strSheetName">Represents reference sheet name.</param>
    public void RemoveColumn( int iColumnIndex, string strSheetName )
    {
      RemoveColumn( iColumnIndex, strSheetName, 1 );
    }
    /// <summary>
    /// Updates named ranges after column removal.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    /// <param name="strSheetName">Represents reference sheet name.</param>
    /// <param name="count">Number of columns to remove.</param>
    public void RemoveColumn( int iColumnIndex, string strSheetName, int count )
    {
      InsertRemoveRowColumn( strSheetName, iColumnIndex, true, false, count );
    }
    /// <summary>
    /// Defines a new name. Returns a Name object.
    /// </summary>
    /// <param name="name">NameRecord describing new Name object.</param>
    /// <returns>Newly created Name object.</returns>
    /// <exception cref="System.ArgumentException">
    /// If the collection already contains object with the same name.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// When specified NameRecord is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public IName Add( NameRecord name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      // We can't use constructor with NameRecord argument because excel 
      // can contain named ranges with references to itself. So we should
      // add this name to the collection and then try to parse it.
      NameImpl result = new NameImpl( Application, this, name.Name, List.Count );

      Add( result );
      result.Parse( name );

      return result;
    }
    /// <summary>
    /// Defines multiple names.
    /// </summary>
    /// <param name="names">
    /// Array of Name Records that describes new Name objects.
    /// </param>
    /// <exception cref="System.ArgumentException">
    /// If the collection already contains object with the same name.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// When array of NameRecords is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void AddRange( NameRecord[] names )
    {
      if( names == null )
        throw new ArgumentNullException( "names" );

      for( int i = 0, len = names.Length; i < len; i++ )
      {
        Add( names[ i ] );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, i, "Parsed name" );
      }
    }
    /// <summary>
    /// Saves all NameRecords to the specified OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all NameRecords.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// If specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( Count == 0 ) return;

      if( records == null )
        throw new ArgumentNullException( "records" );

      SortForSerialization();
      for( int i = 0, len = Count; i < len; i++ )
      {
        NameImpl name = ( NameImpl )InnerList[ i ];
        name.Serialize( records );
      }
    }
    /// <summary>
    /// Adds into list and hashtable, for local named ranges.
    /// </summary>
    /// <param name="name">Name to add.</param>
    public void AddLocal( IName name )
    {
      AddLocal( name, true );
    }
    /// <summary>
    /// Adds into list and hashtable, for local named ranges.
    /// </summary>
    /// <param name="name">Name to add.</param>
    /// <param name="bAddInGlobalNamesHash">Indicates is adds in global names hash.</param>
    public void AddLocal( IName name, bool bAddInGlobalNamesHash )
    {
      ( ( NameImpl )name ).SetIndex( Count );

      if( bAddInGlobalNamesHash )
      {
        base.Add( name );
      }
      else
      {
        InnerList.Add( name );
      }

      IsWorkbookNamesChanged = true;
    }
    /// <summary>
    /// Sorts named range in the order which they must be serialized.
    /// </summary>
    internal void SortForSerialization()
    {
      // MS Excel stores local named ranges with same name sorted by
      // worksheet name and global name should be placed after local names
      // otherwise there could be problems with opening in MS Excel.
      // So we have to change indexes of our named ranges according to this rule.
      if( m_bWorkNamesChanged )
      {
        // Sort worksheets by name.
        SortedList<string, object> list = GetSortedWorksheets();

        // Sort names with same name by worksheet name.
        int[] arrNewIndex = GetNewIndexes( list );
        // Update indexes of the names.
        UpdateIndexes( arrNewIndex );
        SetIndexesWithoutEvent();
        IsWorkbookNamesChanged = false;
      }
    }
    /// <summary>
    /// Returns sorted list with worksheets names.
    /// </summary>
    /// <returns>Sorted list with worksheets names.</returns>
    private SortedList<string, object> GetSortedWorksheets()
    {
      IWorksheets worksheets = m_book.Worksheets;
      int iCount = worksheets.Count;
      SortedList<string, object> list = new SortedList<string, object>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        list.Add( worksheets[ i ].Name, null );
      }

      return list;
    }
    /// <summary>
    /// Sorts names by worksheet name.
    /// </summary>
    /// <param name="list">List with worksheets names.</param>
    /// <returns>Array of new indexes for names.</returns>
    private int[] GetNewIndexes( SortedList<string, object> list )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      int iCount = Count;
      List<IName> arrCopy = new List<IName>( InnerList );
      int[] arrNewIndex = new int[ iCount ];
      int iCurIndex = 0;
      bool[] arrFilled = new bool[ iCount ];
      int iSheetsCount = list.Count;
      string strMaxSheetName = list.Keys[ iSheetsCount - 1 ];

      // Create name larger than maximum name (for global named ranges).
      string strGlobalName = strMaxSheetName + "_1";

      for( int i = 0; i < iCount; i++ )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, i, "Processing name" );

        if( arrFilled[ i ] )
        {
          continue;
        }

        NameImpl name = ( NameImpl )arrCopy[ i ];
        IWorksheet sheet = name.Worksheet;
        SortedList<string, NameImpl> listSameNames = FindSameNames( name, strGlobalName );

        int iLen = listSameNames.Count;
        IList<NameImpl> values = listSameNames.Values;

        for( int j = 0; j < iLen; j++ )
        {
          NameImpl nameToSet = values[ j ];
          
          if( iCurIndex < InnerList.Count )
          {
            InnerList[ iCurIndex ] = nameToSet;
          }
          else
          {
#if ( WINRT )
            throw new ApplicationException("");
#else
              throw new ApplicationException();
#endif
            //InnerList.Add( nameToSet );
          }

          int iNameIndex = nameToSet.Index;

          if( arrFilled[ iNameIndex ] ) continue;

          arrNewIndex[ iNameIndex ] = iCurIndex;
          arrFilled[ iNameIndex ] = true;
          iCurIndex++;
        }
      }
      
      return arrNewIndex;
    }
    /// <summary>
    /// Searches for named ranges with same name in other worksheets.
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <param name="strGlobalName">Key for global named ranges.</param>
    /// <returns>SortedListEx with found names.</returns>
    private SortedList<string, NameImpl> FindSameNames( NameImpl name, string strGlobalName )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      SortedList<string, NameImpl> list = new SortedList<string, NameImpl>();

      string strName = name.Name;
      IWorksheets sheets = m_book.Worksheets;
      int index = name.Record.IndexOrGlobal;
      IWorksheet sheetSource = name.Worksheet;

      for( int i = 0, iLen = sheets.Count; i < iLen; i++ )
      {
        IWorksheet sheet = sheets[ i ];

        if( sheetSource != sheet )
        {
          AddNameToList( list, sheet.Names, strName, sheet.Name );
        }
      }

      if( index != 0 )
      {
        AddNameToList( list, m_book.Names, strName, strGlobalName );
        strGlobalName = sheetSource.Name;
      }

      list.Add( strGlobalName, name );

      return list;
    }
    /// <summary>
    /// Adds name to the list.
    /// </summary>
    /// <param name="list">List to insert name into.</param>
    /// <param name="names">Names collection to search for name.</param>
    /// <param name="strName">Name to find.</param>
    /// <param name="strSheetName">Parent worksheet name (used as key in the list).</param>
    private void AddNameToList( SortedList<string, NameImpl> list, INames names, string strName, string strSheetName )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      if( names == null )
        throw new ArgumentNullException( "names" );

      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty" );

      NameImpl nameFound = ( NameImpl )names[ strName ];

      if( nameFound != null )
      {
        list.Add( strSheetName, nameFound );
      }
    }
    /// <summary>
    /// Updates names indexes in formulas.
    /// </summary>
    /// <param name="arrNewIndex">New indexes of the named ranges.</param>
    private void UpdateIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      m_book.UpdateNamedRangeIndexes( arrNewIndex );
    }
    /// <summary>
    /// Sets index of all named ranges without event raising.
    /// </summary>
    private void SetIndexesWithoutEvent()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        NameImpl name = ( NameImpl )InnerList[ i ];
        name.SetIndex( i, false );
      }
    }
    /// <summary>
    /// Forces all named ranges to parse its data.
    /// </summary>
    public void ParseNames()
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        IParseable toParse = ( IParseable )this[ i ];
        toParse.Parse();
      }
    }
    /// <summary>
    /// Adds local user-defined function.
    /// </summary>
    /// <param name="strFunctionName">Function to add.</param>
    /// <returns>Name index of the added function.</returns>
    public int AddFunctions( string strFunctionName )
    {
      NameRecord name = ( NameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Name );
      name.IsFunctionOrCommandMacro = true;
      name.IsNameFunction = true;
      name.IsNameCommand = true;
      name.Name = strFunctionName;
      IName result = Add( name );
      return result.Index;
    }
    /// <summary>
    /// Gets name record by index.
    /// </summary>
    /// <param name="index">Index of name object in collection.</param>
    /// <returns>Returns named record.</returns>
    [ CLSCompliant( false ) ]
    public NameRecord GetNameRecordByIndex( int index )
    {
      if( index < 0 || index >= Count )
        throw new ArgumentOutOfRangeException( "index" );

      NameImpl name = ( NameImpl )this[ index ];

      return name.Record;
    }
    /// <summary>
    /// Adds copy of global name during worksheet copy.
    /// </summary>
    /// <param name="nameToCopy">Name to copy.</param>
    /// <param name="destSheet">Destination worksheet.</param>
    /// <param name="hashExternSheetIndexes">Dictionary with new extern worksheet names.</param>
    /// <param name="hashNewWorksheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Added name.</returns>
    public IName AddCopy( IName nameToCopy, IWorksheet destSheet, Dictionary<int, int> hashExternSheetIndexes,
      IDictionary hashNewWorksheetNames )
    {
      if( nameToCopy == null )
        throw new ArgumentNullException( "nameToCopy" );

      if( destSheet == null )
        throw new ArgumentNullException( "destSheet" );

      string strName = nameToCopy.Name;
      IName result = null;
      int iRefIndex = m_book.AddSheetReference( destSheet );
      NameImpl nameImpl = ( NameImpl )nameToCopy;
      NameRecord nameRecord = nameImpl.Record.Clone() as NameRecord;
      WorkbookImpl oldBook = nameImpl.Workbook;
      //SetReferenceIndex( nameRecord, iRefIndex );

      WorksheetNamesCollection.UpdateReferenceIndexes( nameRecord, oldBook, hashNewWorksheetNames,
        hashExternSheetIndexes, m_book );

      if( Contains( strName ) )
      {
        // We have try to add it as local name.
        nameRecord.IndexOrGlobal = ( ushort )( destSheet.Index + 1 );
        WorksheetNamesCollection sheetNames = ( WorksheetNamesCollection )destSheet.Names;
        result = sheetNames.Add( nameRecord, false );
      }
      else
      {
        result = Add( nameRecord );
      }

      return result;
    }
    /// <summary>
    /// Sets reference index in the all tokens.
    /// </summary>
    /// <param name="name">Name record to update.</param>
    /// <param name="iRefIndex">New reference index.</param>
    private void SetReferenceIndex( NameRecord name, int iRefIndex )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      Ptg[] arrTokens = name.FormulaTokens;

      for( int i = 0, len = arrTokens.Length; i < len; i++ )
      {
        Ptg token = arrTokens[ i ];
        IReference reference = token as IReference;

        if( reference != null )
        {
          // TODO: maybe we should check whether this reference is local
          // (somewhere in the current workbook) reference.
          reference.RefIndex = ( ushort )iRefIndex;
        }
      }
    }
    /// <summary>
    /// Creates a copy of the current collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>A copy of the current collection.</returns>
    public override object Clone(object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      WorkbookNamesCollection result = ( WorkbookNamesCollection )base.Clone( parent );
      result.m_bWorkNamesChanged = m_bWorkNamesChanged;
      return result;
    }
    /// <summary>
    /// Performs additional processes after inserting a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at the index.</param>
    protected override void OnInsertComplete( int index, IName value )
    {
      NameImpl name = ( NameImpl )value;
      base.OnInsertComplete( index, value );

      if( !( name.IsBuiltIn || name.IsExternName || name.IsLocal ) )
        m_hashNameToIName[ name.Name ] = name;
    }
    /// <summary>
    /// Converts full row or column tokens between versions.
    /// </summary>
    /// <param name="version">Version to convert into.</param>
    public void ConvertFullRowColumnNames( ExcelVersion version )
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        NameImpl name = ( NameImpl )this[ i ];
        name.ConvertFullRowColumnName( version );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// set Flags m_bWorkNamesChanged
    /// </summary>
    public bool IsWorkbookNamesChanged
    {
      get
      {
        return m_bWorkNamesChanged;
      }
      set
      {
        if( !m_book.Loading )
        {
          m_bWorkNamesChanged = value;
        }
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Sets parent workbook value.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// When  parent workbook cannot be found.
    /// </exception>
    private void SetParent()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "NamesCollection has no parent Workbook." );
    }
    /// <summary>
    /// Inserts/removes rows/columns from all named ranges in the collection.
    /// </summary>
    /// <param name="strSheetName">Represents sheet name.</param>
    /// <param name="index">Index of row/column to insert/remove.</param>
    /// <param name="bIsRemove">Indicates whether we should remove rows or columns.</param>
    /// <param name="bIsRow">Indicates whether we are operating with rows.</param>
    /// <param name="iCount">Number of rows/columns to insert/remove.</param>
    private void InsertRemoveRowColumn( string strSheetName, int index, bool bIsRemove, bool bIsRow, int iCount )
    {
      int iMaxValue = bIsRow
        ? m_book.MaxRowCount
        : m_book.MaxColumnCount;
      
      if( index < 1 || index > iMaxValue )
        throw new ArgumentOutOfRangeException( "index" );

      if( strSheetName == null )
        throw new ArgumentNullException( "strSheetName" );

      if( strSheetName.Length == 0 )
        throw new ArgumentException( "strSheetName" );

      List<IName> arrNames = InnerList;
      int iNamesCount = arrNames.Count;

      if( iCount == 0 ) return;

      IsWorkbookNamesChanged = true;

      // Let's make index zero-based.
      index--;

      for( int i = 0, len = iNamesCount; i < len; i++ )
      {
        NameImpl name = ( NameImpl )arrNames[ i ];
        //bool bIsSkipName = true;
        //string strValue = name.Value;

        //if( strValue == null )
        //  continue;

        //int iIndex = strValue.IndexOf( "!" );

        //if( iIndex >= 0 )
        //{
        //  string strSheetValidName = strValue.Substring( 1, iIndex - 1 );
          
        //  if( strSheetValidName.IndexOf( strSheetName ) == 0 && strSheetValidName.Length == strSheetName.Length + 1 )
        //    bIsSkipName = false;
        //}

        //if( bIsSkipName )
        //  continue;

        //IRange range = name.RefersToRange;
        NameRecord record = name.Record;
        Ptg[] arrTokens = record.FormulaTokens;

        if( arrTokens != null )
        {
          for( int j = 0, lenJ = arrTokens.Length; j < lenJ; j++ )
          {
            IRangeGetterToken token = arrTokens[ j ] as IRangeGetterToken;

            if( token != null )
            {
              IReference reference = token as IReference;

              if( reference == null || !m_book.IsExternalReference( reference.RefIndex ) )
              {
                  if (name != null && name.Worksheet != null && (name.Worksheet.ParseDataOnDemand || name.Worksheet.ParseOnDemand))
                  {
                      name.Worksheet.ParseData(null);
                  }

                Ptg result = InsertRemoveRow( token, strSheetName, index,
                  bIsRemove, bIsRow, iCount, name.Worksheet );

                if( result == null )
                  result = token.ConvertToError();

                // TODO: what if it is null?
                arrTokens[ j ] = result;
              }
            }
          }

          record.FormulaTokens = arrTokens;
        }
      }
    }
    /// <summary>
    /// Inserts/removes row from all named ranges in the collection.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="strSheetName"></param>
    /// <param name="index"></param>
    /// <param name="bIsRemove"></param>
    /// <param name="bIsRow"></param>
    /// <param name="iCount"></param>
    /// <param name="sheet"></param>
    /// <returns></returns>
    private Ptg InsertRemoveRow( IRangeGetterToken token, string strSheetName, int index,
      bool bIsRemove, bool bIsRow, int iCount, IWorksheet sheet )
    {
      if( token == null )
        throw new ArgumentNullException( "token" );

      MergeRegion newRegion = null;
      Ptg result = ( Ptg )token;

      IRange range = token.GetRange( m_book, sheet );
      if( range != null && range.Worksheet.Name == strSheetName )
      {
        Rectangle rect = token.GetRectangle();
        MergeRegion nameRegion = new MergeRegion( rect.Top, rect.Bottom, rect.Left, rect.Right );
        result = null;

        newRegion = bIsRow
          ? MergeCellsImpl.InsertRemoveRow( nameRegion, index, bIsRemove, iCount, m_book )
          : MergeCellsImpl.InsertRemoveColumn( nameRegion, index, bIsRemove, iCount, m_book );

        result = ( newRegion != null ) ?
          result = token.UpdateRectangle( newRegion.GetRectangle() )
          : null;
      }

      return result;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<IName> list = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        NameImpl name = ( NameImpl )list[ i ];
        NameRecord record = name.Record;
        Ptg[] tokens = record.FormulaTokens;
        FormulaUtil.MarkUsedReferences( tokens, usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<IName> list = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        NameImpl name = ( NameImpl )list[ i ];
        NameRecord record = name.Record;
        Ptg[] tokens = record.FormulaTokens;

        if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
        {
          record.FormulaTokens = tokens;
        }
      }
    }
    #endregion

    #region Helper Methods
    private int GetKnownNamedCount()
    {
        int iCount = 0;
        foreach (IName name in List)
            if (name.RefersToRange != null)
                iCount++;
        return iCount;
    }
    #endregion

  }
}
