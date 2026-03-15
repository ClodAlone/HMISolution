#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for ArrayListEx.
  /// </summary>
  public class ArrayListEx
  {
    #region Class members
    /// <summary>
    /// Represents item storage.
    /// </summary>
    private RowStorage[] m_items;
    /// <summary>
    /// Represents count of elements.
    /// </summary>
    private int m_iCount;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new instance current collection.
    /// </summary>
    public ArrayListEx()
//      : this( 16 )
    {
    }
    /// <summary>
    /// Creates new instance of current collection.
    /// </summary>
    /// <param name="iCount">Represents count.</param>
    public ArrayListEx( int iCount )
    {
      if( iCount <= 0 )
        throw new ArgumentOutOfRangeException( "iCount" );

      m_iCount = iCount;
      m_items = new RowStorage[ m_iCount ];
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets value by index.
    /// </summary>
    public RowStorage this[ int index ]
    {
      get
      {
        return ( index < 0 || index >= m_iCount ) ?
          null :
          m_items[ index ];
      }
      set
      {
        if( m_iCount <= index )
        {
          UpdateSize( index + 1 );
        }

        m_items[ index ] =  value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Updates count of storage array.
    /// </summary>
    /// <param name="iCount">Represents count of array to update.</param>
    public void UpdateSize( int iCount )
    {
      if( iCount > m_iCount )
      {
        int iBufCount = m_iCount * 2;

        m_iCount = ( iCount >= iBufCount )
          ? iCount
          : iBufCount;

        RowStorage[] arr = new RowStorage[ m_iCount ];

        if( m_items != null )
          m_items.CopyTo( arr, 0 );

        m_items = arr;
      }
    }
    /// <summary>
    /// Reduces size of the internal array if necessary.
    /// </summary>
    /// <param name="iCount">Maximum size of the internal array.</param>
    public void ReduceSizeIfNecessary( int iCount )
    {
      if( iCount < 0 )
        throw new ArgumentOutOfRangeException( "iCount" );

      if( iCount < m_iCount )
      {
        RowStorage[] arrNewStorage = new RowStorage[ iCount ];
        Array.Copy( m_items, 0, arrNewStorage, 0, iCount );
        m_items = arrNewStorage;
        m_iCount = iCount;
      }
    }
    /// <summary>
    /// Inserts specified number of null RowStorages into specified position.
    /// </summary>
    /// <param name="index">Index to insert into.</param>
    /// <param name="count">Number of items to insert.</param>
    /// <param name="length">Number of items after index position to preserve.</param>
    public void Insert( int index, int count, int length )
    {
      Array.Copy( m_items, index, m_items, index + count, length );

      for( int i = index, len = index + count; i < len; i++ )
      {
        m_items[ i ] = null;
      }
    }
    /// <summary>
    /// Get Maximum Row Count
    /// </summary>
    /// <returns></returns>
    internal int GetCount()
    {
        return m_iCount;
    }
    #endregion

    /// <summary>
    /// Get Row Index in Row Storage Based on the row number
    /// </summary>
    /// <param name="row">RowNumber</param>
    /// <param name="arrIndex">RowIndex in RowStorage</param>
    /// <returns></returns>
    internal bool GetRowIndex(int row, out int arrIndex)
    {
        if (m_iCount == 0)
        {
            arrIndex = 0;
            return false;
        }
        int num3 = 0;
        int num = 0;
        int num2 = m_iCount - 1;
        if (num2 == row)
        {
            arrIndex = row;
            return true;
        }
        if (num2 < row)
        {
            arrIndex = num2 + 1;
            return false;
        }
        while (num <= num2)
        {
            num3 = (num + num2) / 2;
            int num4 = row - num3;
            if (num4 == 0)
            {
                arrIndex = num3;
                return true;
            }
            if (num4 < 0)
            {
                num2 = num3 - 1;
            }
            else
            {
                num = num3 + 1;
            }
        }
        if (row > num3)
        {
            arrIndex = num3 + 1;
        }
        else
        {
            arrIndex = num3;
        }
        return false;

    }
  }
}
