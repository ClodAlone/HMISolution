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
using System.Collections.Generic;
using System.Diagnostics;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for ExternNamesCollection.
  /// </summary>
  public class ExternNamesCollection : CollectionBaseEx<ExternNameImpl>
  {
    #region Class members
    /// <summary>
    /// Parent extern workbook.
    /// </summary>
    private ExternWorkbookImpl m_externBook;
    /// <summary>
    /// Hash table with all names.
    /// </summary>
    private List<ExternNameImpl> m_hashNames = new List<ExternNameImpl>();
    /// <summary>
    /// Sometimes different applications (not MS Excel) can create duplicated
    /// extern names, we have to remember all of them and after parsing remove it.
    /// </summary>
    private SortedList<int, object> m_lstToRemove = new SortedList<int, object>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates collection with specified Application and Parent.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public ExternNamesCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      Removed += new CollectionChange(ExternNamesCollection_Removed);
      Inserted += new CollectionChange(ExternNamesCollection_Inserted);
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_externBook = FindParent( typeof( ExternWorkbookImpl ) ) as ExternWorkbookImpl;

      if( m_externBook == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns extern name. Read-only.
    /// </summary>
    public ExternNameImpl this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count" );

        return ( ExternNameImpl )List[ index ];
      }
    }
    /// <summary>
    /// Returns extern name. Read-only.
    /// </summary>
    public ExternNameImpl this[ string name ]
    {
      get
      {
        int index = GetNameIndex( name );

        return ( index < 0 || index > Count ) ?
          null :
          ( ExternNameImpl )List[ index ];
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public ExternWorkbookImpl ParentWorkbook
    {
      get
      {
        return m_externBook;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds new name to the collection.
    /// </summary>
    /// <param name="name">Name to add.</param>
    /// <returns>Index of added extern name.</returns>
    [ CLSCompliant( false ) ]
    public int Add( ExternNameRecord name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      WorkbookImpl book = m_externBook.Workbook;

      //if( Contains( name.Name ) )
      //{
      //  if( book.Loading )
      //  {
      //    int iCurIndex = List.Count + m_lstToRemove.Count;
      //    m_lstToRemove.Add( iCurIndex, null );
      //    book.HasDuplicatedNames = true;
      //    return -1;
      //  }

      //  throw new ApplicationException( "Duplicate names" );
      //}

      ExternNameImpl nameImpl = new ExternNameImpl( Application, this, name, List.Count );
      
      base.Add( nameImpl );
      if (!m_hashNames.Contains(nameImpl)) 
          m_hashNames.Add( nameImpl);
      return Count - 1;
    }
    /// <summary>
    /// Adds new name to the collection.
    /// </summary>
    /// <param name="name">Name to add.</param>
    /// <returns>Index of added extern name.</returns>
    public int Add( string name )
    {
      if( name == null )
        throw new ArgumentNullException( "name" );

      if( name.Length == 0 )
        throw new ArgumentException( "name - string cannot be empty" );

      ExternNameRecord externName = ( ExternNameRecord )BiffRecordFactory.GetRecord(
        TBIFFRecord.ExternName );

      externName.Name = name;
      //externName.FormulaData

      int iResult = Add( externName );
      return iResult;
    }
    /// <summary>
    /// Adds new name to the collection.
    /// </summary>
    /// <param name="name">Name to add.</param>
    /// <returns>Index of added extern name.</returns>
    public int Add(string name,bool isAddIn)
    {
        if (name == null)
            throw new ArgumentNullException("name");

        if (name.Length == 0)
            throw new ArgumentException("name - string cannot be empty");

        ExternNameRecord externName = (ExternNameRecord)BiffRecordFactory.GetRecord(
          TBIFFRecord.ExternName);

        externName.Name = name;
        //externName.FormulaData
        if (isAddIn)
            externName.IsAddIn = true;

        int iResult = Add(externName);
        return iResult;
    }
    /// <summary>
    /// Checks if collection contains extern name with specified name.
    /// </summary>
    /// <param name="name">Name to find.</param>
    /// <returns>Boolean value indicating whether collection contains extern name.</returns>
    public bool Contains( string name )
    {
        for (int i = 0; i < m_hashNames.Count; i++)
        {
            if (m_hashNames[i].Name == name)
                return true ;
        }
        return false;
      //return m_hashNames.ContainsKey( name );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        this[ i ].Serialize( records );
      }
    }
    /// <summary>
    /// Return index to the extern name.
    /// </summary>
    /// <param name="strName">Name to locate.</param>
    /// <returns>Index to the extern name, or -1 if name was not found.</returns>
    public int GetNameIndex( string strName )
    {
        for (int i = 0; i < m_hashNames.Count; i++)
        {
            if (m_hashNames[i].Name == strName)
              return  m_hashNames[i].Index;
        }
        return -1;
    }
    /// <summary>
    /// Get new index for extern name (to remove duplicated extern names).
    /// </summary>
    /// <param name="iNameIndex">Name index.</param>
    /// <returns>Updated name index.</returns>
    public int GetNewIndex( int iNameIndex )
    {
      //return m_externNames.GetNewIndex( iNewIndex );
      int iIndex = m_lstToRemove.IndexOfKey( iNameIndex );

      if( iIndex != -1 )
      {
        return iNameIndex - iIndex - 1;
      }

      return iNameIndex;
    }
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns></returns>
    public override object Clone( object parent )
    {
      ExternNamesCollection result = ( ExternNamesCollection )base.Clone( parent );
      IList<int> keys = m_lstToRemove.Keys;

      for( int i = 0, len = m_lstToRemove.Count; i < len; i++ )
      {
        int key = keys[ i ];
        m_lstToRemove.Add( key, null );
      }

//      for( int i = 0, len = Count; i < len; i++ )
//      {
//        ExternNameImpl name = this[ i ];
//        name = name.Clone( result );
//        result.Add( name );
//      }

      return result;
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="name">Name to add.</param>
    /// <returns>Index of the added item.</returns>
    private int Add( ExternNameImpl name )
    {
      base.Add( name );
      return Count - 1;
    }
    #endregion

    #region Class event handlers
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ExternNamesCollection_Removed(object sender, CollectionChangeEventArgs<ExternNameImpl> args)
    {
      // reset indexes
      for( int i = args.Index, len = Count; i < len; i++ )
      {
        this[ i ].Index = i;
      }

      ExternNameImpl name = ( ExternNameImpl )args.Value;

      if( !name.Record.NeedDataArray )
      {
        // TODO: here we have to look for same names.
        m_hashNames.Remove( name );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void ExternNamesCollection_Inserted(object sender, CollectionChangeEventArgs<ExternNameImpl> args)
    {
      ExternNameImpl name = ( ExternNameImpl )args.Value;

      if( !name.Record.NeedDataArray )
      {
        m_hashNames.Add(name );
      }
    }
    #endregion
  }
}
