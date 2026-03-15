#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
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


namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// Class used for Range Builder.
  /// </summary>
  public class RangeBuilder
  {
    #region Class members
    /// <summary>
    /// List with Rectangles used for range creation.
    /// </summary>
    private List<Rectangle> m_arrRanges = new List<Rectangle>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the RangeBuilder class.
    /// </summary>
    public RangeBuilder()
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the collection by its index. Read-only.
    /// </summary>
    public Rectangle this[ int index ]
    {
      get
      {
        return m_arrRanges[ index ];
      }
    }
    /// <summary>
    /// Gets number of elements in the collection. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_arrRanges.Count;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Adds cell to the collection.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to add.</param>
    /// <param name="iColumn">One-based column index of the cell to add.</param>
    public void Add( int iRow, int iColumn )
    {
      // TODO: optimization is required here.
      int iCount = Count;
      bool bNew = true;
      Rectangle rectLast = Rectangle.Empty;

      if( iCount > 0 )
      {
        rectLast = this[ iCount - 1 ];

        if( rectLast.Width == 0 && rectLast.Left == iColumn )
        {
          bNew = false;
          if( rectLast.Top - 1 == iRow )
          {
            rectLast.Height++;
            rectLast.Y--;
          }
          else if( rectLast.Bottom + 1 == iRow )
          {
            rectLast.Height++;
          }
          else
          {
            bNew = true;
          }
        }
        else if( rectLast.Height == 0 )
        {
          bNew = false;
          if( rectLast.Left - 1 == iColumn )
          {
            rectLast.Width++;
            rectLast.X--;
          }
          else if( rectLast.Right + 1 == iColumn )
          {
            rectLast.Width++;
          }
          else
          {
            bNew = true;
          }
        }
      }

      if( bNew )
      {
        m_arrRanges.Add( GetRectangle( iRow, iColumn ) );
      }
      else
      {
        m_arrRanges[ iCount - 1 ] = rectLast;
      }
    }
    /// <summary>
    /// Clears collection.
    /// </summary>
    public void Clear()
    {
      m_arrRanges.Clear();
    }
    /// <summary>
    /// Returns range object that was built by this instance.
    /// </summary>
    /// <param name="parentWorksheet">Parent worksheet object.</param>
    /// <returns>Range object that was built by this instance.</returns>
    public IRange ToRange( IWorksheet parentWorksheet )
    {
      if( parentWorksheet == null )
        throw new ArgumentNullException( "parentWorksheet" );

      int iCount = m_arrRanges.Count;

      if( iCount == 0 )
      {
        return null;
      }
      else if( iCount == 1 )
      {
        Rectangle rect = this[ 0 ];
        return parentWorksheet[ rect.Top, rect.Left, rect.Bottom, rect.Right ];
      }
      else
      {
        IRanges arrResult = parentWorksheet.CreateRangesCollection();

        for( int i = 0; i < iCount; i++ )
        {
          Rectangle rect = this[ i ];
          arrResult.Add( parentWorksheet.Range[ rect.Top, rect.Left, rect.Bottom, rect.Right ] );
        }

        return arrResult;
      }
    }
    /// <summary>
    /// Creates rectangle for specified cell.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell.</param>
    /// <param name="iColumn">One-based column index of the cell.</param>
    /// <returns>Created rectangle.</returns>
    public static Rectangle GetRectangle( int iRow, int iColumn )
    {
      return Rectangle.FromLTRB( iColumn, iRow, iColumn, iRow );
    }
    #endregion
  }
}
