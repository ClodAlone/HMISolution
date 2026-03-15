#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;

using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#endif
#if (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// 
  /// </summary>
  public class RangesOperations
  {
    #region Class constants
    /// <summary>
    /// Maximum number of rectangles after split.
    /// </summary>
    private const int DEF_MAXIMUM_SPLIT_COUNT = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Cell ranges list.
    /// </summary>
    private List<Rectangle> m_arrCells;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public RangesOperations()
      : this( new List<Rectangle>() )
    {
    }
    /// <summary>
    /// Creates new instance of current class.
    /// </summary>
    /// <param name="arrCells">Cell ranges list.</param>
    public RangesOperations( List<Rectangle> arrCells )
    {
      m_arrCells = arrCells;
    }
    #endregion
    
    #region Class properties
    /// <summary>
    /// Gets / sets list of the cells.
    /// </summary>
    public virtual List<Rectangle> CellList
    {
      get
      {
        return m_arrCells;
      }
      set
      {
        m_arrCells = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Indicates whether collection contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to check.</param>
    /// <returns>True if collection contains all specified ranges.</returns>
    public bool Contains( Rectangle[] arrRanges )
    {
      return Contains( arrRanges, 0 );
    }
    /// <summary>
    /// Indicates whether collection contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to check.</param>
    /// <param name="iStartIndex">Start index in the internal ranges collection to search from.</param>
    /// <returns>True if collection contains all specified ranges.</returns>
    public bool Contains( Rectangle[] arrRanges, int iStartIndex )
    {
      if( arrRanges == null )
        return true;

      int iLength = arrRanges.Length;

      if( iLength == 0 )
        return true;

      if( iStartIndex < 0 )
        iStartIndex = 0;

      for( int i = 0; i < iLength; i++ )
      {
        if( !Contains( arrRanges[ i ], iStartIndex ) )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Indicates whether collection contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to check.</param>
    /// <returns>True if collection contains all specified ranges.</returns>
    public bool Contains( IList<Rectangle> arrRanges )
    {
      return Contains( arrRanges, 0 );
    }
    /// <summary>
    /// Indicates whether collection contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to check.</param>
    /// <param name="iStartIndex">Start index in the internal ranges collection to search from.</param>
    /// <returns>True if collection contains all specified ranges.</returns>
    public bool Contains( IList<Rectangle> arrRanges, int iStartIndex )
    {
      if( arrRanges == null )
        return true;

      int iLength = arrRanges.Count;

      if( iLength == 0 )
        return true;

      if( iStartIndex < 0 )
        iStartIndex = 0;

      for( int i = 0; i < iLength; i++ )
      {
        if( !Contains( arrRanges[ i ], iStartIndex ) )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Indicates whether collection contains specified range.
    /// </summary>
    /// <param name="range">Range to check.</param>
    /// <returns>True if collection contains specified range.</returns>
    public bool Contains( Rectangle range )
    {
      return Contains( range, 0 );
    }
    /// <summary>
    /// Indicates whether collection contains specified range.
    /// </summary>
    /// <param name="range">Range to check.</param>
    /// <param name="iStartIndex">Start index in the internal ranges collection to search from.</param>
    /// <returns>True if collection contains specified range.</returns>
    public bool Contains( Rectangle range, int iStartIndex )
    {
      List<Rectangle> arrRanges = CellList;

      for( int i = iStartIndex, len = arrRanges.Count; i < len; i++ )
      {
        Rectangle curRange = arrRanges[ i ];
        //        // TODO: this can be optimized if we will store all ranges in Rectangle instead of TAddr.
        //        Rectangle rect = curRange.GetRectangle();

        // Check whether range have intersection.
        IList<Rectangle> lstRects = SplitRectangle( range, curRange );

        if( lstRects != null )
          return Contains( lstRects, i );
      }

      return false;
    }
    /// <summary>
    /// Returns contains count for specified range.
    /// </summary>
    /// <param name="range">Range to check.</param>
    /// <returns>Contains count</returns>
    public int ContainsCount( Rectangle range )
    {
      List<Rectangle> arrRanges = CellList;
      int iCount = 0;

      for( int i = 0, len = arrRanges.Count; i < len; i++ )
      {
        Rectangle curRange = arrRanges[ i ];
        IList<Rectangle> lstRects = SplitRectangle( range, curRange );

        if( lstRects != null )
          iCount++;
      }

      return iCount;
    }
    /// <summary>
    /// Adds cells from the collection.
    /// </summary>
    /// <param name="arrCells">Cells to add to the collection.</param>
    public void AddCells( IList<Rectangle> arrCells )
    {
      if( arrCells == null )
        return;

      for( int i = 0, len = arrCells.Count; i < len; i++ )
      {
        Rectangle range = arrCells[ i ];
        AddRange( range );
      }
    }
    /// <summary>
    /// Adds cells from the collection.
    /// </summary>
    /// <param name="arrCells">Cells to add to the collection.</param>
    public void AddRectangles( IList<Rectangle> arrCells )
    {
      if( arrCells == null )
        return;

      for( int i = 0, len = arrCells.Count; i < len; i++ )
      {
        Rectangle range = arrCells[ i ];
        AddRange( range );
      }
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    public void AddRange( IRange range )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      //      TAddr addr = new TAddr( range.Row - 1, range.Column - 1, range.LastRow - 1, range.LastColumn - 1 );
      //      AddRange( addr );
      ICombinedRange combined = ( ICombinedRange )range;
      Rectangle[] arrRects = combined.GetRectangles();

      for( int i = 0, len = arrRects.Length; i < len; i++ )
      {
        AddRange( arrRects[ i ] );
      }
    }
    /// <summary>
    /// Adds new cell range to the collection.
    /// </summary>
    /// <param name="rect">Range to add.</param>
    public void AddRange( Rectangle rect )
    {
      List<Rectangle> lstRanges = CellList;

      for (int i = 0, len = lstRanges.Count; i < len / 2; i += 2)
      {
          Rectangle curRange = lstRanges[i];
          if (CheckAndAddRange(ref curRange, lstRanges[i + 1]))
          {
              lstRanges.RemoveAt(i);
              lstRanges.RemoveAt(i);
              lstRanges.Insert(i, curRange);
          }
      }

      for( int i = 0, len = lstRanges.Count; i < len; i++ )
      {
        Rectangle curRange = lstRanges[ i ];

        if( CheckAndAddRange( ref curRange, rect ) )
        {
          lstRanges[ i ] = curRange;
          // TODO: here we can also check whether it is possible to add meged range to another range.
          return;
        }
        else if( curRange.Contains( rect ) )
        {
          return;
        }
      }

      lstRanges.Add( rect );
    }
    /// <summary>
    /// Clears internal list.
    /// </summary>
    public void Clear()
    {
      m_arrCells.Clear();
    }
    /// <summary>
    /// Gets part of the cells.
    /// </summary>
    /// <param name="rect">Rectangle to get new collection for.</param>
    /// <param name="remove">Indicates whether to remove cells from the collection.</param>
    /// <param name="rowIncrement">Number of rows to add to each resulting rectangle.</param>
    /// <param name="columnIncrement">Number of columns to add to each resulting rectangle.</param>
    /// <returns></returns>
    public RangesOperations GetPart( Rectangle rect, bool remove, int rowIncrement, int columnIncrement )
    {
      RangesOperations result = new RangesOperations();
      //...

      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        Rectangle currentRect = m_arrCells[ i ];

        if( UtilityMethods.Intersects( currentRect, rect ) )
          
        {
          int x = Math.Max( currentRect.X, rect.X );
          int y = Math.Max( currentRect.Y, rect.Y );
          int right = Math.Min( currentRect.Right, rect.Right );
          int bottom = Math.Min( currentRect.Bottom, rect.Bottom );
          Rectangle rectToAdd = Rectangle.FromLTRB( x, y, right, bottom );
          rectToAdd.Offset( columnIncrement, rowIncrement );
          result.AddRange( rectToAdd );
        }
      }

      if( remove )
        Remove( rect );

      return ( result.m_arrCells.Count > 0 ) ? 
        result :
        null;
    }
    /// <summary>
    /// Checks whether ranges can be merged.
    /// </summary>
    /// <param name="curRange">Range to add to.</param>
    /// <param name="rangeToAdd">Range to add.</param>
    /// <returns>True if operation succeeded.</returns>
    private bool CheckAndAddRange( ref Rectangle curRange, Rectangle rangeToAdd )
    {
      bool bSameColumns = ( curRange.Left == rangeToAdd.Left
        && curRange.Right == rangeToAdd.Right );
      bool bResult = false;

      if( bSameColumns )
      {
        // check top side
        if( curRange.Bottom == rangeToAdd.Top - 1 )
        {
          curRange.Height = rangeToAdd.Bottom - curRange.Top;
          bResult = true;
        }
        else if( rangeToAdd.Bottom == curRange.Top - 1 )
        {
          curRange.Height = curRange.Top - rangeToAdd.Bottom + curRange.Height + rangeToAdd.Height;
          curRange.Y = rangeToAdd.Y;
          bResult = true;
        }
        // check bottom side
        else if( curRange.Top == rangeToAdd.Bottom + 1 )
        {
          curRange.Y = rangeToAdd.Top;
          bResult = true;
        }
      }

      if( !bResult )
      {
        bool bSameRows = ( curRange.Top == rangeToAdd.Top
          && curRange.Bottom == rangeToAdd.Bottom );

        if( bSameRows )
        {
          // check left side
          if( curRange.Right == rangeToAdd.Left - 1 )
          {
            curRange.Width = rangeToAdd.Right - curRange.Left;
            bResult = true;
          }
          else if( rangeToAdd.Right == curRange.Left - 1 )
          {
            curRange.Width = curRange.Left - rangeToAdd.Right + curRange.Width + rangeToAdd.Width;
            curRange.X = rangeToAdd.X;
            bResult = true;
          }
          // check right side
          else if( curRange.Left == rangeToAdd.Right + 1 )
          {
            curRange.X = rangeToAdd.Left;
            bResult = true;
          }
        }
      }

      return bResult;
    }
    /// <summary>
    /// Removes range from the collection of conditional formats.
    /// </summary>
    /// <param name="arrRanges">Array of ranges to remove.</param>
    public void Remove( Rectangle[] arrRanges )
    {
      if( arrRanges == null )
        return;

      int iLength = arrRanges.Length;

      for( int i = 0; i < iLength; i++ )
      {
        Rectangle rect = arrRanges[ i ];
        Remove( rect );
      }
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <returns>Copy of the current object.</returns>
    public RangesOperations Clone()
    {
      RangesOperations result = ( RangesOperations )MemberwiseClone();
      result.m_arrCells = new List<Rectangle>();

      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        result.m_arrCells.Add( m_arrCells[ i ] );
      }

      return result;
    }
    /// <summary>
    /// Removes rectangle from the collection.
    /// </summary>
    /// <param name="rect">Rectangle to remove.</param>
    /// <returns>Index after last element that should be checked.</returns>
    private int Remove( Rectangle rect )
    {
      List<Rectangle> arrRanges = CellList;
      int iRemoveCount = 0;
      int iStartLen = arrRanges.Count;
      List<Rectangle> arrNewParts = null;
      int len = iStartLen;

      for( int i = 0; i < len; i++ )
      {
        Rectangle currentRect = arrRanges[ i ];

        //if( currentRect.IntersectsWith( rect ) )
        if( UtilityMethods.Intersects( currentRect, rect ) )
        {
          // Swap with the last one.
          int iNotSwappedIndex = len - 1;
          Rectangle temp = arrRanges[ iNotSwappedIndex ];
          arrRanges[ iNotSwappedIndex ] = arrRanges[ i ];
          arrRanges[ i ] = temp;
          iRemoveCount++;
          len--;
          i--;

          IList<Rectangle> lstParts = SplitRectangle( currentRect, rect );

          if( lstParts != null )
          {
            if( arrNewParts == null )
            {
              arrNewParts = new List<Rectangle>( lstParts );
            }
            else
            {
              arrNewParts.AddRange( lstParts );
            }
          }

          //break;
        }
      }

      if( iRemoveCount > 0 )
        arrRanges.RemoveRange( iStartLen - iRemoveCount, iRemoveCount );

      if( arrNewParts != null )
        AddRectangles( arrNewParts );

      return len;
    }
    /// <summary>
    /// Splits rectangle into several parts after remove specified rectangle.
    /// </summary>
    /// <param name="rectSource">Rectangle to split.</param>
    /// <param name="rectRemove">Rectangle to remove.</param>
    /// <returns>List with splitted rectangle.</returns>
    private IList<Rectangle> SplitRectangle( Rectangle rectSource, Rectangle rectRemove )
    {
      //if( rectRemove.IntersectsWith( rectSource ) )
      if( UtilityMethods.Intersects( rectRemove, rectSource ) )
      {
        rectRemove.Intersect( rectSource );

        // Here we have to split original rectSource into up to 4 different rectSources.
        List<Rectangle> arrRemain = new List<Rectangle>( DEF_MAXIMUM_SPLIT_COUNT );

        if( rectSource.Top < rectRemove.Top )
        {
          Rectangle rectTop = Rectangle.FromLTRB( rectSource.Left, rectSource.Top, rectSource.Right, rectRemove.Top - 1 );
          arrRemain.Add( rectTop );
        }

        if( rectSource.Bottom > rectRemove.Bottom )
        {
          Rectangle rectBottom = Rectangle.FromLTRB( rectSource.Left, rectRemove.Bottom + 1, rectSource.Right, rectSource.Bottom );
          arrRemain.Add( rectBottom );
        }

        if( rectSource.Left < rectRemove.Left )
        {
          Rectangle rectLeft = Rectangle.FromLTRB( rectSource.Left, rectRemove.Top, rectRemove.Left - 1, rectRemove.Bottom );
          arrRemain.Add( rectLeft );
        }

        if( rectSource.Right > rectRemove.Right )
        {
          Rectangle rectRight = Rectangle.FromLTRB( rectRemove.Right + 1, rectRemove.Top, rectSource.Right, rectRemove.Bottom );
          arrRemain.Add( rectRight );
        }

        return arrRemain;
      }

      return null;
    }
    public void OptimizeStorage()
    {
      SortAndTryAdd( TopValueGetter, LeftValueGetter, CombineSameRowRectangles );
      SortAndTryAdd( LeftValueGetter, TopValueGetter, CombineSameColumnRectangles );

    }
    private void SortAndTryAdd( SortKeyGetter topLevelKeyGetter, SortKeyGetter lowLevelKeyGetter,
      CombineRectangles combine )
    {
      if( m_arrCells.Count > 1 )
      {
        int iStartCount;
        int iCurrentCount = m_arrCells.Count;

        do
        {
          iStartCount = iCurrentCount;
          // Sort existing ranges by row
          SortedDictionary<int, SortedList<int, Rectangle>> dictionary = SortBy( topLevelKeyGetter, lowLevelKeyGetter );

          //IList<int> lstKeys = dictionary.Keys;
          // Re-add all ranges to decrease their count if possible.
          Clear();

          //for( int i = 0, lenKeys = lstKeys.Count; i < lenKeys; i++ )
          OptimizeAndAdd( dictionary, combine );
          iCurrentCount = m_arrCells.Count;
        }
        while( iStartCount != iCurrentCount );
      }
    }
#if WP
    private void OptimizeAndAdd( SortedDictionary<int, SortedList<int, Rectangle>> dictionary,
      CombineRectangles combine )
#else
    private void OptimizeAndAdd( SortedDictionary<int, SortedList<int, Rectangle>> dictionary,
      CombineRectangles combine )
#endif
    {
      foreach( int iKey in dictionary.Keys )
      {
        IList<Rectangle> lstRects = dictionary[ iKey ].Values;

        lstRects = combine( lstRects );

        for( int j = 0, lenRects = lstRects.Count; j < lenRects; j++ )
        {
          AddRange( lstRects[ j ] );
        }
      }
    }
    private delegate int SortKeyGetter( Rectangle rect );
    private int TopValueGetter( Rectangle rect )
    {
      return rect.Top;
    }
    private int LeftValueGetter( Rectangle rect )
    {
      return rect.Left;
    }

    private SortedDictionary<int, SortedList<int, Rectangle>> SortBy( SortKeyGetter keyGetter,
      SortKeyGetter secondLevelKeyGetter )
    {
        SortedDictionary<int, SortedList<int, Rectangle>> dictionary = 
            new SortedDictionary<int, SortedList<int, Rectangle>>();

        for ( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        Rectangle rect = m_arrCells[ i ];

        SortedList<int, Rectangle> list;
        int key = keyGetter( rect );

        if( !dictionary.TryGetValue( key, out list ) )
        {
          list = new SortedList<int, Rectangle>();
          dictionary.Add( key, list );
        }

        if (!list.ContainsKey(secondLevelKeyGetter( rect )))
            list.Add( secondLevelKeyGetter( rect ), rect );
      }

      return dictionary;
    }

    private delegate IList<Rectangle> CombineRectangles( IList<Rectangle> lstRects );
    private IList<Rectangle> CombineSameRowRectangles( IList<Rectangle> lstRects )
    {
      if( lstRects == null || lstRects.Count == 0 )
        return lstRects;

      List<Rectangle> lstCombined = new List<Rectangle>();
      lstCombined.Add( lstRects[ 0 ] );

      for( int i = 1, len = lstRects.Count; i < len; i++ )
      {
        int iLastIndex = lstCombined.Count - 1;
        Rectangle lastRect = lstCombined[ iLastIndex ];
        Rectangle currentRect = lstRects[ i ];

        if( lastRect.Top == currentRect.Top && lastRect.Bottom == currentRect.Bottom &&
          lastRect.Right + 1 == currentRect.Left )
        {
          lastRect = Rectangle.FromLTRB( lastRect.Left, lastRect.Top, currentRect.Right, lastRect.Bottom );
          lstCombined[ iLastIndex ] = lastRect;
        }
        else
        {
          lstCombined.Add( currentRect );
        }
      }

      return lstCombined;
    }

    private IList<Rectangle> CombineSameColumnRectangles( IList<Rectangle> lstRects )
    {
      if( lstRects == null || lstRects.Count == 0 )
        return lstRects;

      List<Rectangle> lstCombined = new List<Rectangle>();
      lstCombined.Add( lstRects[ 0 ] );

      for( int i = 1, len = lstRects.Count; i < len; i++ )
      {
        int iLastIndex = lstCombined.Count - 1;
        Rectangle lastRect = lstCombined[ iLastIndex ];
        Rectangle currentRect = lstRects[ i ];

        if( lastRect.Left == currentRect.Left && lastRect.Right == currentRect.Right &&
          lastRect.Bottom + 1 == currentRect.Top )
        {
          lastRect = Rectangle.FromLTRB( lastRect.Left, lastRect.Top, currentRect.Right, currentRect.Bottom );
          lstCombined[ iLastIndex ] = lastRect;
        }
        else
        {
          lstCombined.Add( currentRect );
        }
      }

      return lstCombined;
    }

    internal void Offset( int iRowDelta, int iColumnDelta, WorkbookImpl book )
    {
      for( int i = 0, len = m_arrCells.Count; i < len; i++ )
      {
        Rectangle rect = m_arrCells[ i ];
        rect.Offset( iColumnDelta, iRowDelta );

        if( rect.Y >= book.MaxRowCount )
          rect.Y = book.MaxRowCount - 1;

        if( rect.Bottom >= book.MaxRowCount )
          rect.Height -= rect.Bottom - book.MaxRowCount + 1;

        if( rect.X >= book.MaxColumnCount )
          rect.X = book.MaxColumnCount - 1;

        if( rect.Right >= book.MaxColumnCount )
          rect.Width -= rect.Right - book.MaxColumnCount + 1;

        m_arrCells[ i ] = rect;
      }
    }
    public void SetLength( int maxLength )
    {
      if( m_arrCells.Count > maxLength )
      {
        m_arrCells.RemoveRange( maxLength, m_arrCells.Count - maxLength );
      }
    }
    #endregion
  }
}
