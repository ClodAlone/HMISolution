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

using Syncfusion.XlsIO.Implementation;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  SILVERLIGHT
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Each cell range address (called an ADDR structure) contains 4 16-bit values.
  /// Cell range address, BIFF8:
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public struct TAddr
  {
    #region Class members
    /// <summary>
    /// Index to first row.
    /// </summary>
    private int m_iFirstRow;
    /// <summary>
    /// Index to last row.
    /// </summary>
    private int m_iLastRow;
    /// <summary>
    /// Index to first column.
    /// </summary>
    private int m_iFirstCol;
    /// <summary>
    /// Index to last column.
    /// </summary>
    private int m_iLastCol;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="iFirstRow">First row.</param>
    /// <param name="iFirstCol">First column.</param>
    /// <param name="iLastRow">Last row.</param>
    /// <param name="iLastCol">Last column.</param>
    public TAddr( int iFirstRow, int iFirstCol, int iLastRow, int iLastCol )
    {
      m_iFirstRow = iFirstRow;
      m_iFirstCol = iFirstCol;
      m_iLastRow = iLastRow;
      m_iLastCol = iLastCol;
    }
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="iTopLeftIndex">Index of the top left cell.</param>
    /// <param name="iBottomRightIndex">Index of the bottom right cell.</param>
    public TAddr( int iTopLeftIndex, int iBottomRightIndex )
    {
      m_iFirstRow = RangeImpl.GetRowFromCellIndex( iTopLeftIndex );
      m_iFirstCol = RangeImpl.GetColumnFromCellIndex( iTopLeftIndex );
      m_iLastRow = RangeImpl.GetRowFromCellIndex( iBottomRightIndex );
      m_iLastCol = RangeImpl.GetColumnFromCellIndex( iBottomRightIndex );
    }
    /// <summary>
    /// Initializes new instance.
    /// </summary>
    /// <param name="rect">Rectangle containing initialization data.</param>
    public TAddr( Rectangle rect )
    {
      m_iFirstCol = rect.X;
      m_iFirstRow = rect.Y;
      m_iLastCol = rect.Right;
      m_iLastRow = rect.Bottom;
    }
    #endregion

    #region Class Properties
    /// <summary>
    /// Index to first column.
    /// </summary>
    public int FirstCol
    {
      get
      {
        return m_iFirstCol;
      }
      set
      {
        m_iFirstCol = value;
      }
    }
    /// <summary>
    /// Index to first row.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }
    /// <summary>
    /// Index to last column.
    /// </summary>
    public int LastCol
    {
      get
      {
        return m_iLastCol;
      }
      set
      {
        m_iLastCol = value;
      }
    }
    /// <summary>
    /// Index to last row.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Converts object to the string.
    /// </summary>
    public override string ToString()
    {
      return base.ToString () + " ( " + m_iFirstRow.ToString() + ", " 
        + m_iFirstCol.ToString() + " ) - ( " + m_iLastRow.ToString() + ", "
        + m_iLastCol.ToString() + " )";
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Converts structure to rectangle.
    /// </summary>
    /// <returns>Created rectangle.</returns>
    public Rectangle GetRectangle()
    {
      return Rectangle.FromLTRB( FirstCol, FirstRow, LastCol, LastRow );
    }
    #endregion
  }
}
