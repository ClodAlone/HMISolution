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
#endregion

namespace Syncfusion.GridExcelConverter
{
  /// <exclude/>
  public class IndexRange
  {
    #region Class members
    /// <summary>
    /// First index of the range.
    /// </summary>
    private int m_iFirstIndex;
    /// <summary>
    /// Last index of the range.
    /// </summary>
    private int m_iLastIndex;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constuctor was made private to prevent user
    /// from creating instances without arguments.
    /// </summary>
    private IndexRange()
    {
    }
    /// <summary>
    /// Initializes new instance of the IndexRange and sets its first index and last index.
    /// </summary>
    /// <param name="iFirstIndex">First index of the range.</param>
    /// <param name="iLastIndex">Last index of the range.</param>
    public IndexRange( int iFirstIndex, int iLastIndex )
    {
      if( iFirstIndex < 0 )
        throw new ArgumentOutOfRangeException( "iFirstIndex" );

      if( iLastIndex < iFirstIndex )
        throw new ArgumentOutOfRangeException( "iLastIndex" );

      m_iFirstIndex = iFirstIndex;
      m_iLastIndex = iLastIndex;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// First index of the range. Read-only.
    /// </summary>
    public int FirstIndex
    {
      get
      {
        return m_iFirstIndex;
      }
    }
    /// <summary>
    /// Last index of the range. Read-only.
    /// </summary>
    public int LastIndex
    {
      get
      {
        return m_iLastIndex;
      }
    }
    #endregion
  }
}
