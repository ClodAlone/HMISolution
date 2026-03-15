#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Class used for optimized row height evaluation.
  /// </summary>
  public class ItemSizeHelper
  {
    #region Members
    /// <summary>
    /// List with height of the rows.
    /// </summary>
    private List<int> m_arrSizeSum = new List<int>();
    /// <summary>
    /// Method used to get size of the measured items.
    /// </summary>
    private SizeGetter m_getter;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the class.
    /// </summary>
    /// <param name="sizeGetter">Delegate used to get size of the items.</param>
    public ItemSizeHelper( SizeGetter sizeGetter )
    {
      if( sizeGetter == null )
        throw new ArgumentNullException( "sizeGetter" );

      m_getter = sizeGetter;
      m_arrSizeSum.Add( 0 );
    }
    /// <summary>
    /// Returns height of all rows starting from the first one and finishing specified row.
    /// </summary>
    /// <param name="rowIndex">One-based row index.</param>
    /// <returns>Sum of height of all rows till rowIndex (included).</returns>
    public int GetTotal( int rowIndex )
    {
      int iCount = m_arrSizeSum.Count;

      if( iCount <= rowIndex )
      {
        m_arrSizeSum.Capacity = Math.Max( m_arrSizeSum.Capacity, rowIndex );

        int iCurrentValue = 0;
        iCurrentValue = m_arrSizeSum[ iCount - 1 ];

        for( int i = iCount; i <= rowIndex; i++ )
        {
          iCurrentValue += m_getter( i );
          m_arrSizeSum.Add( iCurrentValue );
        }
      }

      return m_arrSizeSum[ rowIndex ];
    }
    /// <summary>
    /// Gets size starting from rowStart and ending rowEnd (both included).
    /// </summary>
    /// <param name="rowStart">Index of the first item to measure.</param>
    /// <param name="rowEnd">Index of the last item to measure.</param>
    /// <returns>Total size starting from rowStart and ending rowEnd (both included).</returns>
    public int GetTotal( int rowStart, int rowEnd )
    {
      // TODO: check indexes.
      return ( rowStart <= rowEnd ) ?
        GetTotal( rowEnd ) - GetTotal( rowStart - 1 ) :
        0;
    }
    /// <summary>
    /// Evaluates size of the specified item.
    /// </summary>
    /// <param name="itemIndex">Item's index.</param>
    /// <returns>Size of the specified item.</returns>
    public int GetSize( int itemIndex )
    {
      return GetTotal( itemIndex ) - GetTotal( itemIndex - 1 );
    }
    #endregion

    #region Delegates
    /// <summary>
    /// Delegate used to get size of the single object by its index.
    /// </summary>
    /// <param name="index">Item's index to get size for.</param>
    /// <returns>Size of the specified item.</returns>
    public delegate int SizeGetter( int index );
    #endregion
  }
}
