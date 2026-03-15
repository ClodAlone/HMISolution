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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Collection stores all comments in the worksheet.
  /// </summary>
  public class CommentsCollection
    : CollectionBaseEx<CommentShapeImpl>
    , IComments
  {
    #region Class constants
    /// <summary>
    /// Default comment width.
    /// </summary>
    private const int DEFAULT_WIDTH = 200;
    /// <summary>
    /// Default comment height.
    /// </summary>
    private const int DEFAULT_HEIGHT = 100;
    /// <summary>
    /// Columns count in new comment - 1.
    /// </summary>
    private const int DEF_COLUMNS_COUNT = 1;
    /// <summary>
    /// Rows count in new comment - 1.
    /// </summary>
    private const int DEF_ROWS_COUNT = 3;
    #endregion

    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Dictionary with all comments,
    /// key - cell index (one-based),
    /// value - corresponding comment object.
    /// </summary>
    private Dictionary< long, ICommentShape > m_hashComments = new Dictionary<long, ICommentShape>();
    /// <summary>
    /// 
    /// </summary>
    private bool m_bReRegister;
    #endregion

    #region IComments Members
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public ICommentShape this[ int index ]
    {
      get
      {
        return InnerList[ index ] as ICommentShape;
      }
    }
    /// <summary>
    /// Returns single entry from the collection by row and column one-based indexes. Read-only.
    /// </summary>
    public ICommentShape this[ int iRow, int iColumn ]
    {
      get
      {
        long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
        ICommentShape result;
        HashComments.TryGetValue( lCellIndex, out result );
        return result;
      }
    }
    /// <summary>
    /// Gets single item from the collection.
    /// </summary>
    /// <param name="name">Name of the item to get.</param>
    /// <returns>Single item from the collection.</returns>
    public ICommentShape this[ string name ]
    {
      get
      {
        ICommentShape result = null;

        for( int i = 0, len = Count; i < len; i++ )
        {
          ICommentShape currentShape = this[ i ];

          if( currentShape.Name == name )
          {
            result = currentShape;
            break;
          }
        }

        return result;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// This property indicates whether internal hashtable is invalid and must be filled
    /// once again before accessing.
    /// </summary>
    public bool ReRegisterOnAccess
    {
      get
      {
        return m_bReRegister;
      }
      set
      {
        m_bReRegister = value;
      }
    }
    /// <summary>
    /// Dictionary with all comments,
    /// key - cell index (one-based),
    /// value - corresponding comment object.
    /// </summary>
    private Dictionary<long, ICommentShape> HashComments
    {
      get
      {
        if( m_bReRegister )
        {
          ReRegisterComments();
        }

        return m_hashComments;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the collection and sets its parent and application properties.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public CommentsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// Adds comment to the specified range.
    /// </summary>
    /// <param name="parentRange">Range that should be commented.</param>
    /// <returns>Newly created comment shape.</returns>
    public ICommentShape AddComment( IRange parentRange )
    {
      if( parentRange == null )
        throw new ArgumentNullException( "parentRange" );

      if( ( ( RangeImpl )parentRange ).IsSingleCell )
      {
        return AddComment( parentRange.Row, parentRange.Column );
      }

      return null;
    }
    /// <summary>
    /// Adds comment to the specified range.
    /// </summary>
    /// <param name="iRow">Row of the cell to add comment to.</param>
    /// <param name="iColumn">Column of the cell to add comment to.</param>
    /// <returns>Newly created comment shape.</returns>
    public ICommentShape AddComment( int iRow, int iColumn )
    {
      return AddComment( iRow, iColumn, true );
    }
    /// <summary>
    /// Adds comment to the specified range.
    /// </summary>
    /// <param name="iRow">Row of the cell to add comment to.</param>
    /// <param name="iColumn">Column of the cell to add comment to.</param>
    /// <param name="bIsParseOptions">Indicates is parse comment fill line options.</param>
    /// <returns>Newly created comment shape.</returns>
    public ICommentShape AddComment( int iRow, int iColumn, bool bIsParseOptions )
    {
      CommentShapeImpl comment = m_sheet.Shapes.AddComment( string.Empty, bIsParseOptions ) as CommentShapeImpl;
      
      comment.Column  = iColumn;
      comment.Row     = iRow;
        
      MsofbtClientAnchor clientAnchor = comment.ClientAnchor;
        
      clientAnchor.LeftColumn   = iColumn - 1; // make it zero-based
      clientAnchor.TopRow       = iRow - 1;    // make it zero-based
      clientAnchor.RightColumn  = iColumn + DEF_COLUMNS_COUNT;
      clientAnchor.BottomRow    = iRow + DEF_ROWS_COUNT;
      clientAnchor.LeftOffset   = CommentShapeImpl.DEF_OFFSET;
      clientAnchor.RightOffset  = CommentShapeImpl.DEF_OFFSET;
      clientAnchor.TopOffset    = CommentShapeImpl.DEF_OFFSET;
      clientAnchor.BottomOffset = CommentShapeImpl.DEF_OFFSET;
      int iMaxColumn = m_sheet.Workbook.MaxColumnCount - 1;

      if( clientAnchor.RightColumn > iMaxColumn )
      {
        int iDelta = clientAnchor.RightColumn - iMaxColumn;
        clientAnchor.RightColumn = iMaxColumn;
        clientAnchor.LeftColumn -= iDelta + 1;
      }

      int iMaxRowCount = m_sheet.Workbook.MaxRowCount;

      if( clientAnchor.BottomRow >= iMaxRowCount )
      {
        int iDelta = clientAnchor.BottomRow - iMaxRowCount + 1;
        clientAnchor.BottomRow = iMaxRowCount - 1;
        clientAnchor.TopRow -= iDelta + 1;
      }

      comment.UpdateWidth();
      comment.UpdateHeight();

      base.Add( comment );
      return comment;
    }
    /// <summary>
    /// Adds comment to this collection only.
    /// Should be called from Shapes collection only.
    /// </summary>
    /// <param name="comment">Comment to add.</param>
    internal void AddComment( ICommentShape comment )
    {
      base.Add( comment as CommentShapeImpl );
      //( m_sheet.Range[ comment.Row, comment.Column ] as RangeImpl ).SetComment( comment );
    }

    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If can't find parent worksheet.
    /// </exception>
    private void SetParents()
    {
      m_sheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet" );
    }
    /// <summary>
    /// Removes comment from the collection.
    /// </summary>
    /// <param name="comment">Comment to remove from the collection.</param>
    public void Remove( ICommentShape comment )
    {
      base.Remove( comment as CommentShapeImpl );
      ( ( ShapesCollection )m_sheet.Shapes ).InnerRemoveComment( comment );
    }
    /// <summary>
    /// Removes specified comment only from internal storages (without removing it from shapes collection).
    /// </summary>
    /// <param name="comment">Comment to remove.</param>
    internal void InnerRemove( ICommentShape comment )
    {
      long lCellIndex = RangeImpl.GetCellIndex( comment.Column, comment.Row );
      m_hashComments.Remove( lCellIndex );
      Remove( comment );
    }
    /// <summary>
    /// 
    /// </summary>
    internal void ReRegisterComments()
    {
      m_hashComments.Clear();
      List<CommentShapeImpl> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        CommentShapeImpl comment = list[ i ];
        long lCellIndex = RangeImpl.GetCellIndex( comment.Column, comment.Row );
        m_hashComments.Add( lCellIndex, comment );
      }
    }
    /// <summary>
    /// OnClear is invoked before Clear behavior.
    /// </summary>
    protected override void OnClear()
    {
      base.OnClear();

      for( int i = Count - 1; i >= 0; i-- )
      {
        ICommentShape comment = this[ i ];
        ( ( ShapesCollection )m_sheet.Shapes ).InnerRemoveComment( comment );
      }
    }
    #endregion

    #region Class synchonization methods
    /// <summary>
    /// Performs additional processes after inserting
    /// a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected override void OnInsertComplete( int index, CommentShapeImpl value )
    {
      base.OnInsertComplete( index, value );

      //ICommentShape comment = ( ICommentShape )value;
      int iRow = value.Row;
      int iColumn = value.Column;
      long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );

      HashComments[ lCellIndex ] = value;
    }

    /// <summary>
    /// Performs additional processes after removing 
    /// an element from the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which the value can be found.</param>
    /// <param name="value">The value of the element to remove from index.</param>
    protected override void OnRemoveComplete( int index, CommentShapeImpl value )
    {
      base.OnRemoveComplete( index, value );

      int iRow = value.Row;
      int iColumn = value.Column;
      long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );

      m_hashComments.Remove( lCellIndex );
    }

    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      m_hashComments.Clear();
    }

    /// <summary>
    /// Performs additional processes after setting a value in the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at the index.</param>
    protected override void OnSetComplete( int index, CommentShapeImpl oldValue, CommentShapeImpl newValue )
    {
      base.OnSetComplete( index, oldValue, newValue );

      int iRow = newValue.Row;
      int iColumn = newValue.Column;
      long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );

      HashComments[ lCellIndex ] = newValue;
    }
    #endregion
  }
}
