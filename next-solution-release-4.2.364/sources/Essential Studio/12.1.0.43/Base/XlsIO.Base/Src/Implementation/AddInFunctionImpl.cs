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
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Summary description for AddInFunctionImpl.
  /// </summary>
  public class AddInFunctionImpl
    : CommonObject
    , IAddInFunction
    , ICloneParent
  {
    #region Class members
    /// <summary>
    /// Index to extern workbook.
    /// </summary>
    private int m_iBookIndex;
    /// <summary>
    /// Name index in the extern workbook.
    /// </summary>
    private int m_iNameIndex;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="iBookIndex">Book index.</param>
    /// <param name="iNameIndex">Name index.</param>
    public AddInFunctionImpl( IApplication application, object parent, int iBookIndex, int iNameIndex )
      : base( application, parent )
    {
      m_iBookIndex = iBookIndex;
      m_iNameIndex = iNameIndex;
      SetParents();
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
    /// Gets / sets book index.
    /// </summary>
    public int BookIndex
    {
      get
      {
        return m_iBookIndex;
      }
      set
      {
        m_iBookIndex = value;
      }
    }
    /// <summary>
    /// Name index.
    /// </summary>
    public int NameIndex
    {
      get
      {
        return m_iNameIndex;
      }
      set
      {
        m_iNameIndex = value;
      }
    }
    /// <summary>
    /// Returns name of add-in function.
    /// </summary>
    public string Name
    {
      get
      {
        string strResult = null;

        if( BookIndex == AddInFunctionsCollection.DEF_LOCAL_BOOK_INDEX )
        {
          strResult = m_book.Names[ NameIndex ].Name;
        }
        else
        {
          ExternWorkbookImpl externBook = m_book.ExternWorkbooks[ BookIndex ];
          strResult = externBook.ExternNames[ NameIndex ].Name;
        }

        return strResult;
      }
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
      AddInFunctionImpl result = ( AddInFunctionImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      return result;
    }

    #endregion
  }
}
