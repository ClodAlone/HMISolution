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

using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Shared.Utils.KeyBinding.Implementation;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Class theat implements bookmarks support for the editcontrol.
	/// </summary>
	public class BookmarksHelper
	{
		#region Fields
		/// <summary>
		/// List of bookmarks.
		/// </summary>
		private ArrayList m_bookmarks = new ArrayList();
		/// <summary>
		/// List of custom bookmarks.
		/// </summary>
		private ArrayList m_customMarks = new ArrayList();
		/// <summary>
		/// Parent editcontrol.
		/// </summary>
		private StreamEditControl m_editcontrol;
		/// <summary>
		/// Invoker for UpdateBookmarksPoints method.
		/// </summary>
		private MethodInvoker m_updateBookmarksInvoker;
		#endregion

		#region Properties
		/// <summary>
		/// Gets readonly copy of the bookmarks collection.
		/// </summary>
		public BookmarksCollection Bookmarks
		{
			get
			{
				return new BookmarksCollection( m_bookmarks );
			}
		}
		/// <summary>
		/// Gets readonly copy of the custom bookmarks collection.
		/// </summary>
		public CustomBookmarksCollection CustomBookmarks
		{
			get
			{
				return new CustomBookmarksCollection( m_customMarks );
			}
		}
		/// <summary>
		/// Gets current line index.
		/// </summary>
		protected int CurrentLine
		{
			get
			{
				return m_editcontrol.CurrentLine;
			}
		}
		/// <summary>
		/// Parent control's parser.
		/// </summary>
		protected RenderableLexemParser Parser
		{
			get
			{
				return m_editcontrol.Parser;
			}
		}
		/// <summary>
		/// Parent control's parser basestream.
		/// </summary>
		protected StreamsWrapper BaseStream
		{
			get
			{
				return Parser.BaseStream;
			}
		}
		/// <summary>
		/// Parent control's cursor manager.
		/// </summary>
		protected CursorManager CursorManager
		{
			get
			{
				return m_editcontrol.CursorManager;
			}
		}
		/// <summary>
		/// Parent control's position converter.
		/// </summary>
		protected IPositionConverter PositionConverter
		{
			get
			{
				return CursorManager.PositionConverter;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Event that is raised when line mark should be drawn.
		/// </summary>
		public event DrawLineMarkEventHandler DrawLineMark;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="control">StreamEditControl.</param>
		public BookmarksHelper( StreamEditControl control )
		{
			if( null == control ) throw new ArgumentNullException( "control" );

			m_editcontrol = control;
			m_updateBookmarksInvoker = new MethodInvoker( UpdateBookmarksPoints );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Draws rectangle with plus or minus sign in the collapsers area if line supports collapsing.
		/// </summary>
		/// <param name="line">Line, to draw sign for.</param>
		/// <param name="g">Graphics object, where sign can be drawn.</param>
		/// <param name="wndHandle">Handle of window to draw at.</param>
		/// <param name="bUseXPStyle">Indicates whether XP style must be used.</param>
		/// <param name="bForPrint">Indicates whether g is printer's graphics.</param>
		public void DrawBookmark( RenderedLine line, Graphics g, IntPtr wndHandle, bool bUseXPStyle, bool bForPrint )
		{
			if( line == null ) throw new ArgumentNullException( "line" );
			if( g == null ) throw new ArgumentNullException( "g" );

			Rectangle markRect = m_editcontrol.GetLineIndicatorRectangle( line );
			int bookmarkIndex = m_bookmarks.BinarySearch( line.LineStartPoint );
			if( bookmarkIndex >= 0 )
			{
				Bookmark mark = m_bookmarks[ bookmarkIndex ] as Bookmark;

				DrawLineMarkEventArgs drawArgs = new DrawLineMarkEventArgs( g, markRect, line.LineIndex, line.LineStartPoint.Line );

				if( DrawLineMark != null )
				{
					DrawLineMark( this, drawArgs );
				}

				if( !drawArgs.CustomDraw )
				{
					mark.PaintBookmark( g, markRect, wndHandle, bUseXPStyle, bForPrint );
				}

			}

			bookmarkIndex = m_customMarks.BinarySearch( line.LineStartPoint );
			if( bookmarkIndex >= 0 )
			{
				Bookmark mark = m_customMarks[ bookmarkIndex ] as Bookmark;
				mark.PaintBookmark( g, markRect, wndHandle, bUseXPStyle, bForPrint );
			}
		}
		/// <summary>
		/// Get bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <returns>IBookmark.</returns>
		public IBookmark BookmarkGet( int line )
		{
			IBookmark result = null;

			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, line ) );
			if( point != null )
			{
				int index = m_bookmarks.BinarySearch( point );
				if( index >= 0 )
				{
					result = ( IBookmark )m_bookmarks[ index ];
				}
			}

			return result;
		}
		/// <summary>
		/// Sets bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		/// <returns>IBookmark.</returns>
		public IBookmark BookmarkAdd( int line )
		{
			IBookmark result = null;

			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, line ) );
			ILexemLine lineInstance = m_editcontrol.GetLine( line );
			IParsePoint endPoint = ( lineInstance.LineLength > 0 ) ?
				( PositionConverter.VirtualToPhysical( new Point( lineInstance.LineLength, line ) ) ) : ( point );

			if( point != null && endPoint != null )
			{
				int index = m_bookmarks.BinarySearch( point );
				if( index < 0 )
				{
					point.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );
					endPoint.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );
					Bookmark bookmark = new Bookmark( BaseStream, m_editcontrol.Parser, point, endPoint );
					m_bookmarks.Add( bookmark );
					m_bookmarks.Sort();
					m_editcontrol.InvalidateAll();
					result = bookmark;
				}
				else
				{
					result = ( IBookmark )m_bookmarks[ index ];
				}
			}

			return result;
		}
		/// <summary>
		/// Removes bookmark at the specified line.
		/// </summary>
		/// <param name="line">Line index.</param>
		public void BookmarkRemove( int line )
		{
			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, line ) );
			if( point != null )
			{
				int index = m_bookmarks.BinarySearch( point );
				if( index >= 0 )
				{
					point.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );
					m_bookmarks.RemoveAt( index );
				}

				m_bookmarks.Sort();
				InvalidateAll();
			}
		}
		/// <summary>
		/// Gets custom bookmark for the line.
		/// </summary>
		/// <param name="iLine">Number of the line to be checked for the bookmark.</param>
		/// <returns>ICustomBookmark interface to the line's bookmarks or null if there is no custom bookmarks on a line.</returns>
		public ICustomBookmark GetCustomBookmark( int iLine )
		{
			IParsePoint point = BaseStream.GetParsePoint( iLine, 1, true );
			int markIndex = m_customMarks.BinarySearch( point );

			CustomBookmark mark = null;
			if( 0 <= markIndex )
			{
				mark = m_customMarks[ markIndex ] as CustomBookmark;
			}

			return mark;
		}
		/// <summary>
		/// Creates custom bookmark for the specified line.
		/// </summary>
		/// <param name="iLine">Physical line index.</param>
		/// <param name="painter">Paint handler.</param>
		public ICustomBookmark SetCustomBookmark( int iLine, BookmarkPaintEventHandler painter )
		{			
			if( iLine < 1 || iLine > BaseStream.LinesCount ) throw new ArgumentOutOfRangeException(
				"iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_70 );
			if( painter == null ) throw new ArgumentNullException( "painter" );

			lock( this )
			{
                IParsePoint point = BaseStream.GetParsePoint(iLine, 1, true);
                IParsePoint endPoint = point; 
				int markIndex = m_customMarks.BinarySearch( point );
				CustomBookmark mark = null;
				if( markIndex < 0 )
				{
					point.Deleted += new ParsePointDeletedEventHandler( CustomMarkPointDeleted );
					endPoint.Deleted += new ParsePointDeletedEventHandler( CustomMarkPointDeleted );
					mark = new CustomBookmark( BaseStream, m_editcontrol.Parser, point, endPoint );
					m_customMarks.Insert( ~markIndex, mark );
				}
				else
				{
					mark = m_customMarks[ markIndex ] as CustomBookmark;
				}

				mark.DrawBookmark += painter;
				m_editcontrol.InvalidateAll();
				return mark;
			}
		}
		/// <summary>
		/// Removes custom bookmark from the specified line.
		/// </summary>
		/// <param name="iLine">Physical line index.</param>
		/// <param name="painter">Paint handler.</param>
		public void RemoveCustomBookmark( int iLine, BookmarkPaintEventHandler painter )
		{
			if( iLine < 1 || iLine > BaseStream.LinesCount ) throw new ArgumentOutOfRangeException(
				"iLine", iLine, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_70 );
			if( painter == null ) throw new ArgumentNullException( "painter" );

			lock( this )
			{
				IParsePoint point = BaseStream.GetParsePoint( iLine, 1, true );
				int markIndex = m_customMarks.BinarySearch( point );
				CustomBookmark mark = null;
				if( markIndex >= 0 )
				{
					mark = m_customMarks[ markIndex ] as CustomBookmark;
					mark.DrawBookmark -= painter;
					if( mark.IsEventHandlerListEmpty )
					{
						m_customMarks.RemoveAt( markIndex );
					}
				}
			}

			InvalidateAll();
		}
		/// <summary>
		/// Clears all bookmarks.
		/// </summary>
		public virtual void BookmarkClear( bool clearCustom )
		{
			CancelEditorSelection();
			for( int i = m_bookmarks.Count - 1; i >= 0; i-- )
			{
				( m_bookmarks[ i ] as Bookmark ).Point.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );
			}
			m_bookmarks.Clear();

			if( clearCustom )
			{
				m_customMarks.Clear();
			}

			InvalidateAll();
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Removes bookmark, assigned to the deleted parsepoint.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void BookmarkPointDeleted( ParsePoint point, long lNewOffset )
		{
			point.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );
			UpdateBookmark( m_bookmarks, point );
		}
		/// <summary>
		/// Deletes custom bookmark.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void CustomMarkPointDeleted( ParsePoint point, long lNewOffset )
		{
			point.Deleted -= new ParsePointDeletedEventHandler( CustomMarkPointDeleted );
			UpdateBookmark( m_customMarks, point );
		}
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Invalidates parent control.
		/// </summary>
		private void InvalidateAll()
		{
			m_editcontrol.InvalidateAll();
		}
		/// <summary>
		/// Cancels parent control's selection.
		/// </summary>
		private void CancelEditorSelection()
		{
			m_editcontrol.StopSelection();
			m_editcontrol.SelectionCancel();
		}

		/// <summary>
		/// Toggles bookmark on the current line.
		/// </summary>
		protected virtual void BookmarkToggleInternal()
		{
			CancelEditorSelection();

			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, CurrentLine ) );

			IParsePoint endPoint =
				( m_editcontrol.CurrentLineInstance.LineLength > 0 )
				? ( PositionConverter.VirtualToPhysical( new Point( m_editcontrol.CurrentLineInstance.LineLength, CurrentLine ) ) )
				: ( point );

			if( point == null || endPoint == null )
				return;

			int index = m_bookmarks.BinarySearch( point );

			if( index < 0 )
			{
				point.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );
				endPoint.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );
				m_bookmarks.Add( new Bookmark( BaseStream, m_editcontrol.Parser, point, endPoint ) );
			}
			else
			{
				point.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );
				m_bookmarks.RemoveAt( index );
			}

			m_bookmarks.Sort();

			m_editcontrol.InvalidateAll();
		}
		/// <summary>
		/// Jumps to the next bookmark.
		/// </summary>
		protected virtual void BookmarkNextInternal()
		{
			CancelEditorSelection();

			IParsePoint point =
				PositionConverter.
				VirtualToPhysical( new Point( 1, CurrentLine ) );

			if( point == null )
				return;

			int index = m_bookmarks.BinarySearch( point );
			int indexCustom = GetNextSearchableCustomBookmark( point );

			if( index < 0 )
				index = ~index;
			else
				index++;

			if( index >= m_bookmarks.Count )
				index = 0;

			IParsePoint pointBookmark = ( index >= 0 && index < m_bookmarks.Count )
				? ( m_bookmarks[ index ] as Bookmark ).Point
				: null;

			IParsePoint pointCustomMark = ( indexCustom >= 0 && indexCustom < m_customMarks.Count )
				? ( m_customMarks[ indexCustom ] as Bookmark ).Point
				: null;

			if( null != pointBookmark && null != pointCustomMark )
			{
				if( ( pointBookmark.Offset < point.Offset && pointCustomMark.Offset > point.Offset )
					|| ( pointBookmark.Offset > point.Offset && pointCustomMark.Offset < point.Offset ) )
				{
					point = ( pointBookmark.Offset > pointCustomMark.Offset )
						? ( pointBookmark )
						: ( pointCustomMark );
				}
				else
				{
					if( pointBookmark.Offset != point.Offset && pointCustomMark.Offset != point.Offset )
					{
						point = ( pointBookmark.Offset < pointCustomMark.Offset )
							? ( pointBookmark )
							: ( pointCustomMark );
					}
					else
					{
						point = ( pointCustomMark.Offset == point.Offset )
							? ( pointBookmark )
							: ( pointCustomMark );
					}
				}
			}
			else
			{
				point = ( null != pointBookmark )
					? pointBookmark
					: pointCustomMark;
			}

			if( null != point )
			{
				m_editcontrol.Parser.EnsureVisibility( point );
				CursorManager.CursorPhysicalCoordinates.Position = point;
				m_editcontrol.UpdateScrollInfo();
			}
		}
		/// <summary>
		/// Jumps to the previous bookmark.
		/// </summary>
		protected virtual void BookmarkPreviousInternal()
		{
			CancelEditorSelection();

			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, CurrentLine ) );

			if( point == null )
				return;

			int index = m_bookmarks.BinarySearch( point );
			int indexCustom = GetPreviousSearchableCustomBookmark( point );

			if( index < 0 )
				index = ~index - 1;
			else
				index--;

			if( index < 0 )
				index = m_bookmarks.Count - 1;

			IParsePoint pointBookmark = ( index >= 0 && index < m_bookmarks.Count )
				? ( m_bookmarks[ index ] as Bookmark ).Point
				: null;
			IParsePoint pointCustomMark = ( indexCustom >= 0 && indexCustom < m_customMarks.Count )
				? ( m_customMarks[ indexCustom ] as Bookmark ).Point
				: null;

			if( null != pointBookmark && null != pointCustomMark )
			{
				if( ( pointBookmark.Offset < point.Offset && pointCustomMark.Offset > point.Offset )
					|| ( pointBookmark.Offset > point.Offset && pointCustomMark.Offset < point.Offset ) )
				{
					point = ( pointBookmark.Offset < pointCustomMark.Offset )
						? ( pointBookmark )
						: ( pointCustomMark );
				}
				else
				{
					if( pointBookmark.Offset != point.Offset && pointCustomMark.Offset != point.Offset )
					{
						point = ( pointBookmark.Offset > pointCustomMark.Offset )
							? ( pointBookmark )
							: ( pointCustomMark );
					}
					else
					{
						point = ( pointCustomMark.Offset == point.Offset )
							? ( pointBookmark )
							: ( pointCustomMark );
					}
				}
			}
			else
			{
				point = ( null != pointBookmark )
					? pointBookmark
					: pointCustomMark;
			}

			if( null != point )
			{
				Parser.EnsureVisibility( point );
				CursorManager.CursorPhysicalCoordinates.Position = point;

				InvalidateAll();
			}
		}
		/// <summary>
		/// Toggles indexed bookmark in current line.
		/// </summary>
		/// <param name="iIndex"></param>
		protected void ToggleIndexedBookmark( int iIndex )
		{
			CancelEditorSelection();

			IParsePoint point = PositionConverter.VirtualToPhysical( new Point( 1, CurrentLine ) );

			if( point == null )
				return;

			ArrayList list = new ArrayList( m_bookmarks );
			list.Sort( new BookMarkIndexSearch() );

			int index = m_bookmarks.BinarySearch( point );
			int index_SearchByIndex = list.BinarySearch( iIndex, new BookMarkIndexSearch() );
			IParsePoint foundPoint = ( index_SearchByIndex >= 0 ) ?
				( list[ index_SearchByIndex ] as Bookmark ).Point : null;
			index_SearchByIndex = ( index_SearchByIndex >= 0 ) ? m_bookmarks.BinarySearch( foundPoint ) : -1;
			bool bAdd = ( index_SearchByIndex < 0 && index < 0 ) || ( index_SearchByIndex != index );

			if( index_SearchByIndex >= 0 )
			{
				IParsePoint pointToDelete =
					( m_bookmarks[ index_SearchByIndex ] as Bookmark ).Point;
				pointToDelete.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );

				m_bookmarks.RemoveAt( index_SearchByIndex );
				index = m_bookmarks.BinarySearch( point );
			}

			if( index >= 0 )
			{
				( m_bookmarks[ index ] as Bookmark ).Point.Deleted -= new ParsePointDeletedEventHandler( BookmarkPointDeleted );
				m_bookmarks.RemoveAt( index );
			}

			if( bAdd )
			{
				point.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );

				IParsePoint endPoint = ( m_editcontrol.CurrentLineInstance.LineLength > 0 )
				? ( PositionConverter.VirtualToPhysical( new Point( m_editcontrol.CurrentLineInstance.LineLength, CurrentLine ) ) )
				: ( point );

				endPoint.Deleted += new ParsePointDeletedEventHandler( BookmarkPointDeleted );

				m_bookmarks.Add( new Bookmark( BaseStream, m_editcontrol.Parser, point, endPoint, iIndex ) );
			}

			m_bookmarks.Sort();

			m_editcontrol.InvalidateAll();
		}
		/// <summary>
		/// Gets previous searchable custom bookmark.
		/// </summary>
		/// <param name="point">Start point for search.</param>
		/// <returns>Index of found bookmark.</returns>
		protected int GetPreviousSearchableCustomBookmark( IParsePoint point )
		{
			if( 0 == m_customMarks.Count )
				return -1;

			int indexCustom = m_customMarks.BinarySearch( point );

			if( indexCustom < 0 )
				indexCustom = ~indexCustom - 1;
			else
				indexCustom--;

			if( indexCustom < 0 )
				indexCustom = m_customMarks.Count - 1;

			int fisrtIndex = indexCustom;

			while( !( ( CustomBookmark )m_customMarks[ indexCustom ] ).UseInBookmarkSearch )
			{
				indexCustom--;

				if( indexCustom < 0 )
					indexCustom = m_customMarks.Count - 1;

				if( indexCustom == fisrtIndex )
					break;
			}

			if( indexCustom == fisrtIndex
				&& !( ( CustomBookmark )m_customMarks[ indexCustom ] ).UseInBookmarkSearch )
				return -1;

			return indexCustom;
		}
		/// <summary>
		/// Gets next searchable custom bookmark.
		/// </summary>
		/// <param name="point">Start point for search.</param>
		/// <returns>Index of found bookmark.</returns>
		protected int GetNextSearchableCustomBookmark( IParsePoint point )
		{
			if( 0 == m_customMarks.Count )
				return -1;

			int indexCustom = m_customMarks.BinarySearch( point );

			if( indexCustom < 0 )
				indexCustom = ~indexCustom;
			else
				indexCustom++;

			if( indexCustom >= m_customMarks.Count )
				indexCustom = 0;

			int fisrtIndex = indexCustom;

			while( !( ( CustomBookmark )m_customMarks[ indexCustom ] ).UseInBookmarkSearch )
			{
				indexCustom++;

				if( indexCustom >= m_customMarks.Count )
					indexCustom = 0;

				if( indexCustom == fisrtIndex )
					break;
			}

			if( indexCustom == fisrtIndex
				&& !( ( CustomBookmark )m_customMarks[ indexCustom ] ).UseInBookmarkSearch )
				return -1;

			return indexCustom;
		}
		/// <summary>
		/// Switches indexed bookmark in current line.
		/// </summary>
		/// <param name="iIndex">Index of bookmark to switch.</param>
		protected void SwitchIndexedBookmark( int iIndex )
		{
			CancelEditorSelection();

			ArrayList list = new ArrayList( m_bookmarks );
			list.Sort( new BookMarkIndexSearch() );

			int index_SearchByIndex = list.BinarySearch( iIndex, new BookMarkIndexSearch() );

			if( index_SearchByIndex >= 0 )
			{
				IParsePoint point =
					( list[ index_SearchByIndex ] as Bookmark ).Point;
				Parser.EnsureVisibility( point );

				CursorManager.CursorPhysicalCoordinates.Position = point;
				InvalidateAll();
			}
		}

		/// <summary>
		/// Updates start or end point of the bookmark when one of edges is deleted.
		/// </summary>
		/// <param name="bookmarks">Collection of bookmarks to search.</param>
		/// <param name="point">Deleted parse point.</param>
		private void UpdateBookmark( ArrayList bookmarks, IParsePoint point )
		{
			int index = bookmarks.BinarySearch( point );

			if( index >= 0 )
			{
				Bookmark bookmark = ( Bookmark )bookmarks[ index ];

				if( bookmark.Point != null && point.Offset == bookmark.Point.Offset )
				{
					bookmark.Point = null;

					if( bookmark.EndPoint == null )
					{
						bookmarks.Remove( bookmark );
					}
				}
				else if( bookmark.EndPoint != null && point.Offset == bookmark.EndPoint.Offset )
				{
					bookmark.EndPoint = null;

					if( bookmark.Point == null )
					{
						bookmarks.Remove( bookmark );
					}
				}

				StreamsWrapper stream = m_editcontrol.Parser.BaseStream;
				if( !stream.AfterUpdateListContains( m_updateBookmarksInvoker ) )
					stream.AddDelegateToAfterUpdateInvokeList( m_updateBookmarksInvoker );
			}
		}
		/// <summary>
		/// Updates start and end points of bookmarked line.
		/// </summary>
		private void UpdateBookmarksPoints()
		{
			UpdateBookmarksCollection( m_bookmarks, new ParsePointDeletedEventHandler( BookmarkPointDeleted ) );
			UpdateBookmarksCollection( m_customMarks, new ParsePointDeletedEventHandler( CustomMarkPointDeleted ) );
		}
		/// <summary>
		/// Updates collection of bookmarks.
		/// </summary>
		/// <param name="bookmarks">Collection of bookmarks to be updated.</param>
		/// <param name="deletedhandler">handler for ParsePoint's deleted event.</param>
		private void UpdateBookmarksCollection( ArrayList bookmarks, ParsePointDeletedEventHandler deletedhandler )
		{
			for( int i = 0; i < bookmarks.Count; i++ )
			{
				Bookmark bookmark = ( Bookmark )bookmarks[ i ];

				if( bookmark.Point == null && bookmark.EndPoint == null )
				{
					bookmarks.Remove( bookmark );
					i--;
					continue;
				}

				if( bookmark.Point == null || !bookmark.Point.IsValid )
				{
					IParsePoint point = m_editcontrol.Parser.GetParsePoint( bookmark.EndPoint.Line, 1 );
					point.Deleted += deletedhandler;
					bookmark.Point = point;
				}

				if( bookmark.EndPoint == null || !bookmark.EndPoint.IsValid )
				{
					IParsePoint point = m_editcontrol.Parser.GetParsePoint(
						bookmark.Point.Line, m_editcontrol.GetLine( bookmark.Point.Line ).LineLength );
					point.Deleted += deletedhandler;
					bookmark.EndPoint = m_editcontrol.Parser.GetParsePoint(
						bookmark.Point.Line, m_editcontrol.GetLine( bookmark.Point.Line ).LineLength );
				}
			}

			bookmarks.Sort();

			for( int i = 0; i < bookmarks.Count - 1; i++ )
			{
				Bookmark b1 = ( Bookmark )bookmarks[ i ];
				Bookmark b2 = ( Bookmark )bookmarks[ i + 1 ];

				if( b1.Point.Line == b2.Point.Line )
				{
					bookmarks.Remove( b1 );
					i--;
				}
			}
		}
		#endregion

		#region Class KeyBinding Support
		/// <summary>
		/// Clears all bookmarks.
		/// </summary>
		[Command( "Edit.Bookmarks.Clear" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.F2 )]
		public virtual void BookmarkClear()
		{
			BookmarkClear( false );
		}
		/// <summary>
		/// Goes to the next bookmark.
		/// </summary>
		[Command( "Edit.Bookmarks.Next" )
	 , KeysBinding( Keys.F2 )
	 , KeysBinding( Keys.Control | Keys.K, Keys.Control | Keys.N )
	 ]
		public void BookmarkNext()
		{
			BookmarkNextInternal();
		}
		/// <summary>
		/// Goes to the next bookmark.
		/// </summary>
		[Command( "Edit.Bookmarks.Previous" )
	 , KeysBinding( Keys.Shift | Keys.F2 )
	 , KeysBinding( Keys.Control | Keys.K, Keys.Control | Keys.P )
	 ]
		public void BookmarkPrevious()
		{
			bool bAllowShiftSelectionOld = m_editcontrol.AllowShiftSelectionOld;
			m_editcontrol.AllowShiftSelectionOld = false;

			BookmarkPreviousInternal();

			m_editcontrol.AllowShiftSelectionOld = bAllowShiftSelectionOld;
		}
		/// <summary>
		/// Sets bookmark to the current line.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle" )
	 , KeysBinding( Keys.Control | Keys.F2 )
	 , KeysBinding( Keys.Control | Keys.K, Keys.Control | Keys.K )
	 ]
		public void BookmarkToggle()
		{
			BookmarkToggleInternal();
		}
		/// <summary>
		/// Toggles bookmark with index 1.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle1" )
	 , KeysBinding( Keys.Control | Keys.D1 )]
		public virtual void ToggleIndexedBookmark1()
		{
			ToggleIndexedBookmark( 1 );
		}
		/// <summary>
		/// Toggles bookmark with index 2.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle2" )
	 , KeysBinding( Keys.Control | Keys.D2 )]
		public virtual void ToggleIndexedBookmark2()
		{
			ToggleIndexedBookmark( 2 );
		}
		/// <summary>
		/// Toggles bookmark with index 3.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle3" )
	 , KeysBinding( Keys.Control | Keys.D3 )]
		public virtual void ToggleIndexedBookmark3()
		{
			ToggleIndexedBookmark( 3 );
		}
		/// <summary>
		/// Toggles bookmark with index 4.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle4" )
	 , KeysBinding( Keys.Control | Keys.D4 )]
		public virtual void ToggleIndexedBookmark4()
		{
			ToggleIndexedBookmark( 4 );
		}
		/// <summary>
		/// Toggles bookmark with index 5.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle5" )
	 , KeysBinding( Keys.Control | Keys.D5 )]
		public virtual void ToggleIndexedBookmark5()
		{
			ToggleIndexedBookmark( 5 );
		}
		/// <summary>
		/// Toggles bookmark with index 6.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle6" )
	 , KeysBinding( Keys.Control | Keys.D6 )]
		public virtual void ToggleIndexedBookmark6()
		{
			ToggleIndexedBookmark( 6 );
		}
		/// <summary>
		/// Toggles bookmark with index 1.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle7" )
	 , KeysBinding( Keys.Control | Keys.D7 )]
		public virtual void ToggleIndexedBookmark7()
		{
			ToggleIndexedBookmark( 7 );
		}
		/// <summary>
		/// Toggles bookmark with index 8.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle8" )
	 , KeysBinding( Keys.Control | Keys.D8 )]
		public virtual void ToggleIndexedBookmark8()
		{
			ToggleIndexedBookmark( 8 );
		}
		/// <summary>
		/// Toggles bookmark with index 9.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle9" )
	 , KeysBinding( Keys.Control | Keys.D9 )]
		public virtual void ToggleIndexedBookmark9()
		{
			ToggleIndexedBookmark( 9 );
		}
		/// <summary>
		/// Toggles bookmark with index 0.
		/// </summary>
		[Command( "Edit.Bookmarks.Toggle0" )
	 , KeysBinding( Keys.Control | Keys.D0 )]
		public virtual void ToggleIndexedBookmark0()
		{
			ToggleIndexedBookmark( 0 );
		}

		/// <summary>
		/// Switchs bookmark with index 1.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch1" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D1 )]
		public virtual void SwitchIndexedBookmark1()
		{
			SwitchIndexedBookmark( 1 );
		}
		/// <summary>
		/// Switchs bookmark with index 2.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch2" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D2 )]
		public virtual void SwitchIndexedBookmark2()
		{
			SwitchIndexedBookmark( 2 );
		}
		/// <summary>
		/// Switchs bookmark with index 3.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch3" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D3 )]
		public virtual void SwitchIndexedBookmark3()
		{
			SwitchIndexedBookmark( 3 );
		}
		/// <summary>
		/// Switchs bookmark with index 4.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch4" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D4 )]
		public virtual void SwitchIndexedBookmark4()
		{
			SwitchIndexedBookmark( 4 );
		}
		/// <summary>
		/// Switchs bookmark with index 5.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch5" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D5 )]
		public virtual void SwitchIndexedBookmark5()
		{
			SwitchIndexedBookmark( 5 );
		}
		/// <summary>
		/// Switchs bookmark with index 6.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch6" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D6 )]
		public virtual void SwitchIndexedBookmark6()
		{
			SwitchIndexedBookmark( 6 );
		}
		/// <summary>
		/// Switchs bookmark with index 1.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch7" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D7 )]
		public virtual void SwitchIndexedBookmark7()
		{
			SwitchIndexedBookmark( 7 );
		}
		/// <summary>
		/// Switchs bookmark with index 8.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch8" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D8 )]
		public virtual void SwitchIndexedBookmark8()
		{
			SwitchIndexedBookmark( 8 );
		}
		/// <summary>
		/// Switchs bookmark with index 9.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch9" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D9 )]
		public virtual void SwitchIndexedBookmark9()
		{
			SwitchIndexedBookmark( 9 );
		}
		/// <summary>
		/// Switchs bookmark with index 0.
		/// </summary>
		[Command( "Edit.Bookmarks.Switch0" )
	 , KeysBinding( Keys.Control | Keys.Shift | Keys.D0 )]
		public virtual void SwitchIndexedBookmark0()
		{
			SwitchIndexedBookmark( 0 );
		}
		#endregion
	}
}