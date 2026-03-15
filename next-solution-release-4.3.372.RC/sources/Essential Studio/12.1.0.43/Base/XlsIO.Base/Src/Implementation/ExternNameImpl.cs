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

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for ExternNameImpl.
  /// </summary>
  public class ExternNameImpl
    : CommonObject
    , INameIndexChangedEventProvider
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Extern name record with information about this name.
    /// </summary>
    private ExternNameRecord m_name;
    /// <summary>
    /// Index of the extern name in extern names collection.
    /// </summary>
    private int m_iIndex;
    /// <summary>
    /// Parent extern workbook.
    /// </summary>
    private ExternWorkbookImpl m_externBook;
    /// <summary>
    /// Represents the refersTo attribute of the definedName tag
    /// </summary>
    private string m_refersTo;
    public int sheetId;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="index"></param>
    [ CLSCompliant( false ) ]
    public ExternNameImpl( IApplication application, object parent, ExternNameRecord name, int index )
      : base( application, parent )
    {
      m_name = name;
      m_iIndex = index;
      SetParents();
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_externBook = FindParent( typeof( ExternWorkbookImpl ) ) as ExternWorkbookImpl;

      if( m_externBook == null )
        throw new ArgumentNullException( "Can't find parent extern workbook" );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Index of the extern name in extern names collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        if( value != m_iIndex  )
        {
          int iOldIndex = m_iIndex;
          m_iIndex = value;
          NameIndexChangedEventArgs args = new NameIndexChangedEventArgs( iOldIndex, m_iIndex );
          RaiseIndexChangedEvent( args );
        }
      }
    }
    /// <summary>
    /// Returns name of this extern name. Read-only.
    /// </summary>
    public string Name
    {
      get
      {
        return m_name.Name;
      }
    }
    /// <summary>
    /// Returns index of the parent extern workbook. Read-only.
    /// </summary>
    public int BookIndex
    {
      get
      {
        return m_externBook.Index;
      }
    }
    /// <summary>
    /// Returns internal record that stores all data. Read-only.
    /// </summary>
    internal ExternNameRecord Record
    {
      get
      {
        return m_name;
      }
    }
    /// <summary>
    /// Represents the refersTo attribute of the definedName tag
    /// </summary>
    internal string RefersTo
    {
        get
        {
            return m_refersTo;
        }
        set
        {
            m_refersTo = value;
        }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    private void RaiseIndexChangedEvent( NameIndexChangedEventArgs args )
    {
      if( NameIndexChanged != null )
      {
        NameIndexChanged( this, args );
      }
    }
    /// <summary>
    /// Saves class into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that would receive class data.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_name );
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>Copy of the current object.</returns>
    public object Clone( object parent )
    {
      ExternNameImpl result = ( ExternNameImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      m_name = ( ExternNameRecord )CloneUtils.CloneCloneable( m_name );

      return result;
    }
    #endregion

    #region Class events
    /// <summary>
    /// 
    /// </summary>
    public event NameImpl.NameIndexChangedEventHandler NameIndexChanged;
    #endregion
  }
}
