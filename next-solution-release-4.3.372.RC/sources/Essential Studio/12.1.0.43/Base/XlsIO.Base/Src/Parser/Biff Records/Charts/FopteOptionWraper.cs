#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Collections;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
  /// <summary>
  /// Represents wrapper for gel frame record.
  /// </summary>
  public class FopteOptionWrapper
    : IFopteOptionWrapper
  {
    #region Class methods
    /// <summary>
    /// Represents option storage.
    /// </summary>
    private List<FOPTE> m_list;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    public FopteOptionWrapper()
    {
      m_list = new List<FOPTE>();
    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="list">Represents option storage.</param>
    [ CLSCompliant( false ) ]
    public FopteOptionWrapper( List<FOPTE> list )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      m_list = list;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns option list. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public List<FOPTE> OptionList
    {
      get
      {
        return m_list;
      }
    }
    #endregion

    #region IFopteOptionWrapper methods
    /// <summary>
    /// Replaces option with specified value.
    /// </summary>
    /// <param name="option">Option to set.</param>
    [ CLSCompliant( false ) ]
    public void AddOptionSorted( FOPTE option )
    {
      int i = 0;
      int iCount = m_list.Count;
      
      for( int len = iCount; i < len; i++ )
      {
        if( m_list[ i ].Id >= option.Id )
          break;
      }

      if( i < iCount )
      {
        FOPTE curOption = m_list[ i ];

        if( curOption.Id == option.Id )
        {
          m_list[ i ] = option;
        }
        else
        {
          m_list.Insert( i, option );
        }
      }
      else
      {
        m_list.Add( option );
      }
    }
    /// <summary>
    /// Removes current option by id.
    /// </summary>
    /// <param name="index">Represents option id to remove.</param>
    public void RemoveOption( int index )
    {
      for( int i = 0, iLen = m_list.Count; i < iLen; i++ )
      {
        FOPTE opt = m_list[ i ];

        if( ( int )opt.Id == index )
        {
          m_list.RemoveAt( i );
          
          break;
        }
      }
    }
    #endregion
  }

  /// <summary>
  /// Represents interface, that implement mso option and fopte option wrapper classes.
  /// </summary>
  [ CLSCompliant( false ) ]
  public interface IFopteOptionWrapper
  {
    /// <summary>
    /// Replaces option with specified value.
    /// </summary>
    /// <param name="option">Option to set.</param>
    void AddOptionSorted( FOPTE option );
    /// <summary>
    /// Removes current option by id.
    /// </summary>
    /// <param name="index">Represents option id to remove.</param>
    void RemoveOption( int index );
  }
}
