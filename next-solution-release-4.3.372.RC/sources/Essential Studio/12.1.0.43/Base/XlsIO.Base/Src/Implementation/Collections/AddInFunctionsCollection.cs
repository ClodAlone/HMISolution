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
using System.IO;
using System.Collections.Generic;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for AddInFunctionsCollection.
  /// </summary>
  public class AddInFunctionsCollection
    : CollectionBaseEx<AddInFunctionImpl>
    , IAddInFunctions
  {
    #region Class constants
    /// <summary>
    /// Beginning of the file name with add-in function.
    /// </summary>
    private const string DEF_FILE_NAME_START = "\x1";
    /// <summary>
    /// Local workbook index.
    /// </summary>
    public const int DEF_LOCAL_BOOK_INDEX = -1;
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with all file names with extern functions.
    /// </summary>
    private Dictionary<string, int> m_hashFileNames = new Dictionary<string, int>();
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Extern workbook with unknown functions.
    /// </summary>
    private ExternWorkbookImpl m_unknownBook;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates a collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public AddInFunctionsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      //Inserted += new CollectionChange( AddInFunctionsCollection_Inserted );
      m_book.ExternWorkbooks.Inserted += new CollectionBaseEx<ExternWorkbookImpl>.CollectionChange( ExternWorkbooks_Inserted );
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "m_book" );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    public IAddInFunction this[ int index ]
    {
      get
      {
        return ( IAddInFunction )List[ index ];
      }
    }
    #endregion

    #region Class methods
#if !(WINRT )
    /// <summary>
    /// Adds new add-in function to the collection.
    /// </summary>
    /// <param name="fileName">File with add-in function.</param>
    /// <param name="functionName">Function name.</param>
    /// <returns>Index of the new function.</returns>
    public int Add( string fileName, string functionName )
    {
//      if( fileName == null )
//        throw new ArgumentNullException( "fileName" );
//
//      if( fileName.Length == 0 )
//        throw new ArgumentException( "fileName - string cannot be empty" );

      if( functionName == null )
        throw new ArgumentNullException( "functionName" );

      if( functionName.Length == 0 )
        throw new ArgumentException( "functionName - string cannot be empty" );

      if( fileName != null )
        fileName = Path.GetFullPath( fileName );

      int iWorkbookIndex;

      ExternBookCollection externWorkbooks = m_book.ExternWorkbooks;

      if( Contains( fileName ) )
      {
        iWorkbookIndex = ( fileName != null )
          ? m_hashFileNames[ fileName ]
          : m_unknownBook.Index;
      }
      else
      {
        // TODO: Add event to add in m_hashFileNames.
        iWorkbookIndex = externWorkbooks.Add( fileName, true );//AddAddInFileName( fileName );
      }

      ExternWorkbookImpl book = externWorkbooks[ iWorkbookIndex ];

      if( fileName == null )
        book.IsAddInFunctions = true;

      if( book.ExternNames.Contains( functionName ) )
        throw new ApplicationException( "Already contains same function" );

      int index = book.ExternNames.Add(functionName, true);

      base.Add( new AddInFunctionImpl( Application, this, iWorkbookIndex, index ) );
      return Count - 1;
    }
#endif
    /// <summary>
    /// Adds new local function to the collection.
    /// </summary>
    /// <param name="strFunctionName">Function to add.</param>
    /// <returns>Index of the added function.</returns>
    public int Add( string strFunctionName )
    {
      if( strFunctionName == null )
        throw new ArgumentNullException( "strFunctionName" );

      if( strFunctionName.Length == 0 )
        throw new ArgumentException( "strFunctionName - string cannot be empty" );

      int iNameIndex = m_book.InnerNamesColection.AddFunctions( strFunctionName );
      AddInFunctionImpl function = new AddInFunctionImpl( Application, this,
        DEF_LOCAL_BOOK_INDEX, iNameIndex );
      base.Add( function );
      return Count - 1;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iExternBookIndex"></param>
    /// <param name="iNameIndex"></param>
    public void Add( int iExternBookIndex, int iNameIndex )
    {
      AddInFunctionImpl function = new AddInFunctionImpl( Application, this,
        iExternBookIndex, iNameIndex );

      base.Add( function );
    }
    /// <summary>
    /// Removes add-in collection with specified index.
    /// </summary>
    /// <param name="index">Item to remove.</param>
    new public void RemoveAt( int index )
    {
      if( index < 0 || index > Count - 1 )
        throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1" );

      AddInFunctionImpl function = ( AddInFunctionImpl )List[ index ];
      ExternWorkbookImpl externBook = m_book.ExternWorkbooks[ function.BookIndex ];
      externBook.ExternNames.RemoveAt( function.NameIndex );
    }
    /// <summary>
    /// Indicates whether collection contains workbook with specified name.
    /// </summary>
    /// <param name="strBookName">Name to check.</param>
    /// <returns>True if collections contains book with specified name.</returns>
    public bool Contains( string strBookName )
    {
      return ( strBookName != null )
        ? m_hashFileNames.ContainsKey( strBookName )
        : m_unknownBook != null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="addinFunctions"></param>
    public void CopyFrom( AddInFunctionsCollection addinFunctions )
    {
      if( addinFunctions.m_unknownBook != null )
      {
        int iUnknownIndex = addinFunctions.m_unknownBook.Index;
        m_unknownBook = m_book.ExternWorkbooks[ iUnknownIndex ];;
      }

      List<AddInFunctionImpl> listSource = addinFunctions.InnerList;
      List<AddInFunctionImpl> listDest = InnerList;

      for( int i = 0, len = listSource.Count; i < len; i++ )
      {
        AddInFunctionImpl function = listSource[ i ];
        function = ( AddInFunctionImpl )function.Clone( this );
        listDest.Add( function );
      }
    }
    #endregion

    #region Event handlers
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ExternWorkbooks_Inserted(object sender, CollectionChangeEventArgs<ExternWorkbookImpl> args)
    {
      ExternWorkbookImpl book = args.Value;
      
      if( !book.IsInternalReference && book.IsAddInFunctions )
      {
        string strUrl = book.URL;

        if( strUrl == null )
        {
          m_unknownBook = book;
        }
        else
        {
          if( strUrl != ExternBookCollection.DEF_WRONG_URL_NAME )
            m_hashFileNames.Add( strUrl, book.Index );
        }
      }
    }
    #endregion
  }
}
