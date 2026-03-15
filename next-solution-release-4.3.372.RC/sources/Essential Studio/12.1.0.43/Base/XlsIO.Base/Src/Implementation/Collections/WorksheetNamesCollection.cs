#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for NamesCollection.
  /// </summary>
  public class WorksheetNamesCollection
    : CollectionBaseEx<IName>
    , INames
  {
    #region Class members
    /// <summary>
    /// Dictionary Name-to-IName
    /// </summary>
    private Dictionary<string, IName> m_hashNameToIName = new Dictionary<string, IName>();
    /// <summary>
    /// Parent workbook for the collection.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Parent worksheet for the collection.
    /// </summary>
    private WorksheetImpl m_worksheet;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new empty collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the new collection.</param>
    public WorksheetNamesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region INames Properties
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
        return ( IWorksheet )m_worksheet;
      }
    }
    #endregion

    #region INames methods
    /// <summary>
    /// Renames name in Name-to-IName hashtable.
    /// </summary>
    /// <param name="name">New name implementation.</param>
    /// <param name="strOldName">Old name.</param>
    public void Rename( IName name, string strOldName )
    {
      if( Contains( strOldName )  )
      {
        m_hashNameToIName.Remove( strOldName );
        m_hashNameToIName.Add( name.Name, name );
      }
    }
    /// <summary>
    /// Adds new named range to the collection.
    /// </summary>
    /// <param name="name">Name of the new named range.</param>
    /// <returns>Newly added named range.</returns>
    public IName Add( string name )
    {
      IName result = new NameImpl( Application, this, name, Count, true );
      Add( result );
      return result;
    }
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name for the new Name object.</param>
    /// <param name="namedRange">Range that will be associated with the Name.</param>
    /// <returns>Newly created Name object.</returns>
    public IName Add( string name, IRange namedRange )
    {
      NameImpl result = new NameImpl( Application, this, name, namedRange, Count, true );
      Add( result );
      return result;
    }
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name object to add.</param>
    /// <returns>Added Name object.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If name argument is NULL.
    /// </exception>
    public IName Add( IName name )
    {
      return Add( name, true );
    }
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">Name object to add.</param>
    /// <param name="bAddInGlobalNamesHash">Indicates is adds in global names hash.</param>
    /// <returns>Added Name object.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If name argument is NULL.
    /// </exception>
    public IName Add( IName name, bool bAddInGlobalNamesHash )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      base.Add( name );
      
      WorkbookNamesCollection names = ( WorkbookNamesCollection )m_book.Names;
      names.AddLocal( name, bAddInGlobalNamesHash );

      return name;
    }
    /// <summary>
    /// Removes Name object from the collection.
    /// </summary>
    /// <param name="name">Name of the object to remove.</param>
    public void Remove( string name )
    {
      IName toDel;
      
      if( m_hashNameToIName.TryGetValue( name, out toDel ) )
      {
        m_hashNameToIName.Remove( name );
        m_book.Names.RemoveAt( toDel.Index );
        base.Remove( toDel );
      }
    }
    /// <summary>
    /// Clear collection.
    /// </summary>
    new public void Clear()
    {
      for( int i = Count - 1; i >= 0; i-- )
      {
        Remove( this[ i ].Name );
      }
    }
    /// <summary>
    /// Checks whether collection contains named range.
    /// </summary>
    /// <param name="name">Name of the named range to search.</param>
    /// <returns>True if collection contains such named range; otherwise returns False.</returns>
    public bool Contains( string name )
    {
      return m_hashNameToIName.ContainsKey( name );
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// Adds a new name only to this collection (without registering it in the workbook names collection).
    /// </summary>
    /// <param name="name">Name object to add.</param>
    /// <returns>Added Name object.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If name argument is NULL.
    /// </exception>
    public IName AddLocal( IName name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      base.Add( name );
      return name;
    }
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">NameRecord containing info for new Name object.</param>
    /// <returns>Name object that was added to the collection.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If specified NameRecord is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public IName Add( NameRecord name )
    {
      return Add( name, true );
    }
    /// <summary>
    /// Defines a new name.
    /// </summary>
    /// <param name="name">NameRecord containing info for new Name object.</param>
    /// <param name="bAddInGlobalNamesHash">Indicates is adds in global names hash.</param>
    /// <returns>Name object that was added to the collection.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If specified NameRecord is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public IName Add( NameRecord name, bool bAddInGlobalNamesHash )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      // We can't use constructor with NameRecord argument because excel
      // can contain named ranges with references to itself. So we should
      // add this name to the collection and then try to parse it.
      NameImpl result = new NameImpl( Application, this, name.Name, Count );
      result.Parse( name );
      ( ( IParseable )result ).Parse();
      Add( result, bAddInGlobalNamesHash );

      return result;
    }
    /// <summary>
    /// Defines range of new names.
    /// </summary>
    /// <param name="names">
    /// Array of NameRecords containing info for new Name objects.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// If array of NameRecords is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void AddRange( NameRecord[] names )
    {
      if( names == null )
        throw new ArgumentNullException( "names" );

      string strSheetName = "'" + m_worksheet.Name + "'";
      FormulaUtil formulaParser = m_book.FormulaUtil;

      for( int i = 0, len = names.Length; i < len; i++ )
      {
        NameRecord name = names[ i ];
        string strRangeName = formulaParser.ParsePtgArray( name.FormulaTokens, 0, 0, false, false );

        // Let's check if this name record belongs to the parent worksheet.
        if( strRangeName.StartsWith( strSheetName ) )
        {
          Add( name );
        }
      }
    }
    /// <summary>
    /// Fills Names.
    /// </summary>
    /// <param name="sourceNames">Source names collection.</param>
    /// <param name="hashNewWorksheetNames">Dictionary with modified worksheet names.</param>
    /// <param name="option">Flags for filling.</param>
    /// <param name="dicNewNameIndexes">Dictionary, key - old name index, value - new name index.</param>
    /// <param name="hashExternSheetIndexes">Represents hash table with new extern sheet indexes.</param>
    internal void FillFrom( WorksheetNamesCollection sourceNames,
      IDictionary hashNewWorksheetNames, Dictionary<int, int> dicNewNameIndexes,
      ExcelNamesMergeOptions option, Dictionary<int, int> hashExternSheetIndexes )
    {
      if( sourceNames == null )
        throw new ArgumentNullException( "sourceNames" );

      if( hashExternSheetIndexes == null )
        throw new ArgumentNullException( "hashExternSheetIndexes" );

      WorkbookImpl oldBook = sourceNames.m_book;

      for( int i = 0, len = sourceNames.Count; i < len; i++ )
      {
        NameImpl curName = ( NameImpl )sourceNames[ i ];
        NameRecord newRecord = ( NameRecord )curName.Record.Clone();
        newRecord.IndexOrGlobal = ( ushort )( m_worksheet.RealIndex + 1 );
        UpdateReferenceIndexes( newRecord, oldBook, hashNewWorksheetNames,
          hashExternSheetIndexes, m_book );
        //Ptg[] arrTokens = newRecord.FormulaTokens;

        //for( int j = 0, lenJ = arrTokens.Length; j < lenJ; j++ )
        //{
        //  IReference reference = arrTokens[ j ] as IReference;

        //  if( reference != null )
        //  {
        //    int iOldRefIndex = reference.RefIndex;
        //    string strOldSheetName = oldBook.GetSheetNameByReference( iOldRefIndex );

        //    if( strOldSheetName != null && hashNewWorksheetNames.Contains( strOldSheetName ) )
        //    {
        //      strOldSheetName = ( string )hashNewWorksheetNames[ strOldSheetName ];
        //    }

        //    if( strOldSheetName == WorkbookImpl.DEF_BAD_SHEET_NAME )
        //    {
        //      if( hashExternSheetIndexes.Contains( iOldRefIndex ) )
        //      {
        //        int refIndex = ( int )hashExternSheetIndexes[ iOldRefIndex ];
        //        reference.RefIndex = ( ushort )refIndex;
        //      }
        //    }
        //    else
        //    {
        //      iOldRefIndex = m_book.AddSheetReference( strOldSheetName );

        //      reference.RefIndex = ( ushort )iOldRefIndex;
        //    }
        //  }
        //}

        IName newName = Add( newRecord );
        dicNewNameIndexes.Add( curName.Index, newName.Index );
      }
    }
    /// <summary>
    /// Updates reference indexes in the name record.
    /// </summary>
    /// <param name="name">Name record to update.</param>
    /// <param name="oldBook">Old workbook object.</param>
    /// <param name="hashNewWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="hashExternSheetIndexes">Dictionary with changed extern sheet indexes.</param>
    /// <param name="newBook">New workbook object.</param>
    internal static void UpdateReferenceIndexes( NameRecord name, WorkbookImpl oldBook,
      IDictionary hashNewWorksheetNames, Dictionary<int, int> hashExternSheetIndexes, WorkbookImpl newBook )
    {
      if( hashExternSheetIndexes == null )
        return;

      Ptg[] arrTokens = name.FormulaTokens;

      if( arrTokens == null || arrTokens.Length == 0 )
        return;

      if( oldBook == null )
        throw new ArgumentException( "oldBook" );

      if( newBook == null )
        throw new ArgumentNullException( "newBook" );

      for( int j = 0, lenJ = arrTokens.Length; j < lenJ; j++ )
      {
        IReference reference = arrTokens[ j ] as IReference;

        if( reference != null )
        {
          int iOldRefIndex = reference.RefIndex;
          string strOldSheetName = oldBook.GetSheetNameByReference( iOldRefIndex );
		  
		  if (strOldSheetName == null)
             strOldSheetName = WorkbookImpl.DEF_BAD_SHEET_NAME;

          if( strOldSheetName != null && hashNewWorksheetNames.Contains( strOldSheetName ) )
          {
            strOldSheetName = ( string )hashNewWorksheetNames[ strOldSheetName ];
          }

          if( strOldSheetName == WorkbookImpl.DEF_BAD_SHEET_NAME )
          {
            if( hashExternSheetIndexes.ContainsKey( iOldRefIndex ) )
            {
              int refIndex = hashExternSheetIndexes[ iOldRefIndex ];
              reference.RefIndex = ( ushort )refIndex;
            }
          }
          else
          {
            iOldRefIndex = newBook.AddSheetReference( strOldSheetName );

            reference.RefIndex = ( ushort )iOldRefIndex;
          }
        }
      }
    }
    /// <summary>
    /// Sets sheet index.
    /// </summary>
    /// <param name="iSheetIndex">Sheet index to set.</param>
    public void SetSheetIndex( int iSheetIndex )
    {
      for( int i = Count - 1; i >= 0; i-- )
      {
        NameImpl name = ( NameImpl )InnerList[ i ];
        name.SetSheetIndex( iSheetIndex );
      }
    }
    /// <summary>
    /// Returns existing or creates new name.
    /// </summary>
    /// <param name="strName">Name to create.</param>
    /// <returns>Required name.</returns>
    public NameImpl GetOrCreateName( string strName )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty." );

      NameImpl name = this[ strName ] as NameImpl;

      if( name == null )
      {
        name = ( NameImpl )Add( strName );
      }

      return name;
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

    #region Class methods
    /// <summary>
    /// Sets parent workbook and worksheet for the collection.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If the parent workbook or worksheet cannot be found.
    /// </exception>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_worksheet == null )
        throw new ArgumentNullException( "WorksheetNamesCollection has no parent Worksheet." );

      m_book = m_worksheet.ParentWorkbook;
    }
    #endregion

    #region Implementation of sync Name property
    /// <summary>
    /// Performs additional processes after inserting a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at the index.</param>
    protected override void OnInsertComplete( int index, IName value )
    {
      NameImpl name = ( NameImpl )value;

      string strName = name.Name;

      if( !m_book.Loading || !m_hashNameToIName.ContainsKey( strName ) )
        m_hashNameToIName[ strName ] = value;

      base.OnInsertComplete( index, value );
    }
    #endregion
  }
}
