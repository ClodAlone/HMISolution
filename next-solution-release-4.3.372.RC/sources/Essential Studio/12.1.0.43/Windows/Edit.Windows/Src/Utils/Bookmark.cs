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

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Windows.Forms.Edit;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Edit.Utils
{

	/// <summary>
	/// Bookmark with custom drawing.
	/// </summary>
	internal class CustomBookmark
		: Bookmark
		, ICustomBookmark
	{
		#region Class Private Members
		/// <summary>
		/// Specifies whether this bookmark can be found by BookmarkNext or BookmarkPrevious
		/// </summary>
		private bool m_bUseInSearch;
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when bookmark is to be draw.
		/// </summary>
		public event BookmarkPaintEventHandler DrawBookmark;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates bookmark.
		/// </summary>
		/// <param name="wrapper"></param>
		/// <param name="converter"></param>
		/// <param name="point">Point, bookmark is associated with.</param>
		/// <param name="endPoint"></param>
		public CustomBookmark( StreamsWrapper wrapper, IPositionConverter converter, IParsePoint point, IParsePoint endPoint )
			: base( wrapper, converter, point, endPoint )
		{ }
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets value indicating whether bookmark 
		/// can be found while searching for next/previous bookmark.
		/// </summary>
		public bool UseInBookmarkSearch
		{
			get
			{
				return m_bUseInSearch;
			}
			set
			{
				m_bUseInSearch = value;
			}
		}
		/// <summary>
		/// Gets value indicating whether the list of the customdrawing delegates is empty.
		/// </summary>
		public bool IsEventHandlerListEmpty
		{
			get
			{
				return ( null == DrawBookmark );
			}
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws bookmark.
		/// </summary>
		/// <param name="g">Graphics to be used for drawing.</param>
		/// <param name="rect"></param>
		/// <param name="wndHandle">Handle of window to draw at.</param>
		/// <param name="bUseXPStyle">Indicates whether XP style must be used.</param>
		/// <param name="bForPrint"></param>
		public override void PaintBookmark( Graphics g, Rectangle rect, IntPtr wndHandle, bool bUseXPStyle, bool bForPrint )
		{
			if( DrawBookmark != null )
			{
				PaintEventArgs arg = new PaintEventArgs( g, rect );
				DrawBookmark( this, new BookmarkPaintEventArgs( arg, this.Line ) );
			}
		}
		#endregion

     
        private object m_tag = null;
        /// <summary>
        /// Gets or Sets data about the bookmark
        /// </summary>
        public object tag
        {

            get
            {
                return m_tag;
            }

            set
            {
                m_tag = value;
            }

        }
    }

	/// <summary>
	/// Bookmark.
	/// </summary>
	public class Bookmark
		: IComparable
		, IBookmark
	{
		#region Class Constants
		/// <summary>
		/// Height of font of indexed bookmark.
		/// </summary>
		const int BOOKMARK_FONT_HEIGHT = 14;
		#endregion

		#region Class Static Members
		/// <summary>
		/// Default brush info.
		/// </summary>
		private static BrushInfo _brushDefault = new BrushInfo( Color.Cyan );
		/// <summary>
		/// Get or default BrushInfo
		/// </summary>
		public static BrushInfo BrushDefault
		{
			get
			{
				if( _brushDefault == null )
				{
					new BrushInfo( Color.Cyan );
				}

				return _brushDefault;
			}

			set
			{
				_brushDefault = value;
			}
		}
		/// <summary>
		/// Paint bookmark helper.
		/// </summary>
		/// <param name="g">Graphics.</param>
		/// <param name="rect">Rectangle for bookmark.</param>
		/// <param name="wndHandle">Handle of window.</param>
		/// <param name="bUseXPStyle">Indicates whether XP style should be used.</param>
		public static void PaintBookmarkHelper( Graphics g, Rectangle rect, IntPtr wndHandle, bool bUseXPStyle )
		{
			BrushInfo fakeXPBrushInfo = new BrushInfo( GradientStyle.BackwardDiagonal, new Color[] { Color.White, Color.LightBlue } );
			if( bUseXPStyle )
				PaintBookmarkHelper( g, rect, wndHandle, bUseXPStyle, false, -1, fakeXPBrushInfo, null, Color.Empty );
			else
				PaintBookmarkHelper( g, rect, wndHandle, bUseXPStyle, false, -1, fakeXPBrushInfo, _brushDefault, Color.Empty );
		}

		/// <summary>
		/// Paint bookmark helper.
		/// </summary>
		/// <param name="g">Graphics.</param>
		/// <param name="rect">Rectangle for bookmark.</param>
		/// <param name="wndHandle">Handle of window.</param>
		/// <param name="bUseXPStyle">Indicates whether XP style should be used.</param>
		/// <param name="bForPrint">Indicates whether drawing is being performed for printing.</param>
		/// <param name="index">Index of bookmark.</param>
		/// <param name="fakeXPBrushBookmarks">Brush for emulating XP style.</param>
		/// <param name="defaultBrushInfo">Default brush.</param>
		/// <param name="frameBorderColor">Color for border.</param>
		public static void PaintBookmarkHelper( Graphics g, Rectangle rect, IntPtr wndHandle, bool bUseXPStyle, bool bForPrint,
			int index, BrushInfo fakeXPBrushBookmarks, BrushInfo defaultBrushInfo, Color frameBorderColor )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );
			if( Rectangle.Empty == rect )
				throw new ArgumentOutOfRangeException( "rect" );
			if( IntPtr.Zero == wndHandle )
				throw new ArgumentOutOfRangeException( "wndHandle" );

			rect.X += 2;
			rect.Y += 3;
			rect.Width -= 4;
			rect.Height -= 5;

			if( bUseXPStyle && ( null == defaultBrushInfo ) && ( Color.Empty == frameBorderColor ) )
			{
				if( XPStyle.XPThemesEnabled() && !bForPrint )
				{
					Rectangle tempRect = rect;

					tempRect.Y += ( int )g.Transform.OffsetY - 2;
					tempRect.Height += 2;
					tempRect.X += ( int )g.Transform.OffsetX;
					XPStyle.Draw( wndHandle, g, tempRect, "Scrollbar", 3, 2 );
				}
				else
				{
					GraphicsUtils.DrawRoundedRect( g, rect, 4, fakeXPBrushBookmarks, Pens.Gray );
				}
			}
			else
			{
				Pen pen = new Pen( ( Color.Empty == frameBorderColor ) ? ( Color.Black ) : ( frameBorderColor ) );
				GraphicsUtils.DrawRoundedRect(
					g, rect, 4, ( null != defaultBrushInfo ) ? defaultBrushInfo : _brushDefault, pen );
				pen.Dispose();
			}

			rect.Width--; rect.Height--;
			;

			if( index >= 0 )
			{
				Font testFont = new Font( FontFamily.GenericMonospace, BOOKMARK_FONT_HEIGHT, GraphicsUnit.Pixel );
				g.DrawString( index.ToString(), testFont, Brushes.Black, rect.X, rect.Y + ( rect.Height - BOOKMARK_FONT_HEIGHT ) / 2 );
				testFont.Dispose();
			}
		}
		#endregion

		#region Class members
		/// <summary>
		/// ParsePoint, bookmark is connected to.
		/// </summary>
		private IParsePoint m_point;
		/// <summary>
		/// ParsePoint situated at the end of the bookmarked line.
		/// </summary>
		private IParsePoint m_endPoint;
		/// <summary>
		/// Fast access index of bookmark.
		/// </summary>
		private int m_index;
		/// <summary>
		/// Index of last unindexed bookmark.
		/// </summary>
		private static int m_lastIndex = -1;
		/// <summary>
		/// Parent
		/// </summary>
		private StreamsWrapper m_parent;
		/// <summary>
		/// Position converter, used to convert parsepoint to virtual coordinates.
		/// </summary>
		private IPositionConverter m_converter;
		/// <summary>
		/// BrushInfo object for bookmarks painting.
		/// </summary>
		private BrushInfo m_brushInfo;
		/// <summary>
		/// Brush used to draw bookmarks when XP style is used but there's no XP themes available.
		/// </summary>
		private BrushInfo m_brushBookmarks =
			new BrushInfo( GradientStyle.BackwardDiagonal, new Color[] { Color.White, Color.LightBlue } );
		/// <summary>
		/// Color of bookmark border.
		/// </summary>
		private Color m_clrBorder = Color.Empty;
		#endregion

		#region Class Properties
		/// <summary>
		/// Get BrushInfo where uses XP drawing
		/// </summary>
		public BrushInfo XPBrushInfo
		{
			get
			{
				return m_brushBookmarks;
			}
		}
		/// <summary>
		/// Gets or sets ParsePoint, bookmark is connected to.
		/// </summary>
		public IParsePoint Point
		{
			get
			{
				return m_point;
			}
			set
			{
				m_point = value;
			}
		}
		/// <summary>
		/// Gets or sets ParsePoint situated at the end of the bookmarked line.
		/// </summary>
		public IParsePoint EndPoint
		{
			get
			{
				return m_endPoint;
			}
			set
			{
				m_endPoint = value;
			}
		}
		/// <summary>
		/// GET fast access index of bookmark.
		/// </summary>
		public int Index
		{
			get
			{
				return m_index;
			}
		}
		/// <summary>
		/// Gets bookmark location in text.
		/// </summary>
		public int Line
		{
			get
			{
				return m_converter.PhysicalToVirtual( Point ).Y;
			}
		}
		/// <summary>
		/// Gets or sets BrushInfo object that is using for painting bookmarks.
		/// </summary>
		public BrushInfo BookmarkBrush
		{
			get
			{
				if( null == m_brushInfo )
				{
					m_brushInfo = ( BrushInfo )_brushDefault.Clone();
				}

				return m_brushInfo;
			}
			set
			{
				if( value != BookmarkBrush )
				{
					if( null == value )
						throw new ArgumentNullException( "BookmarkBrush", "Brush can not be set to null." );

					m_brushInfo = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets color of bookmark border.
		/// </summary>
		public Color BorderColor
		{
			get
			{
				return m_clrBorder;
			}
			set
			{
				if( value != m_clrBorder )
				{
					m_clrBorder = value;
				}
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Draws bookmark.
		/// </summary>
		/// <param name="g">Graphics to be used for drawing.</param>
		/// <param name="rect"></param>
		/// <param name="wndHandle">Handle of window to draw at.</param>
		/// <param name="bUseXPStyle">Indicates whether XP style must be used.</param>
		/// <param name="bForPrint">Indicates whether g is printer's graphics.</param>
		public virtual void PaintBookmark(
			Graphics g, Rectangle rect, IntPtr wndHandle, bool bUseXPStyle, bool bForPrint )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );
			if( Rectangle.Empty == rect )
				throw new ArgumentOutOfRangeException( "rect" );
			if( IntPtr.Zero == wndHandle )
				throw new ArgumentOutOfRangeException( "wndHandle" );

			rect.X += 2;
			rect.Y += 3;
			rect.Width -= 4;
			rect.Height -= 5;

			if( bUseXPStyle && ( null == m_brushInfo ) && ( Color.Empty == m_clrBorder ) )
			{
				if( XPStyle.XPThemesEnabled() && !bForPrint )
				{
					Rectangle tempRect = rect;

					tempRect.Y += ( int )g.Transform.OffsetY - 2;
					tempRect.Height += 2;
					tempRect.X += ( int )g.Transform.OffsetX;
					XPStyle.Draw( wndHandle, g, tempRect, "Scrollbar", 3, 2 );
				}
				else
				{
					GraphicsUtils.DrawRoundedRect( g, rect, 4, m_brushBookmarks, Pens.Gray );
				}
			}
			else
			{
				Pen pen = new Pen( ( Color.Empty == m_clrBorder ) ? ( Color.Black ) : ( m_clrBorder ) );
				GraphicsUtils.DrawRoundedRect(
					g, rect, 4, ( null != m_brushInfo ) ? m_brushInfo : _brushDefault, pen );
				pen.Dispose();
			}

			rect.Width--; rect.Height--;
			;

			if( Index >= 0 )
			{
				Font testFont = new Font( FontFamily.GenericMonospace, BOOKMARK_FONT_HEIGHT, GraphicsUnit.Pixel );
				g.DrawString( Index.ToString(), testFont, Brushes.Black, rect.X, rect.Y + ( rect.Height - BOOKMARK_FONT_HEIGHT ) / 2 );
				testFont.Dispose();
			}
		}
		/// <summary>
		/// Compares two bookmarks by offsets of ther parsepoints.
		/// </summary>
		/// <param name="obj">Bookmark object or IParsePoint.</param>
		/// <returns>Standard CompareTo result.</returns>
		public int CompareTo( object obj )
		{
			Bookmark mark = obj as Bookmark;

			IParsePoint p = ( this.Point != null ) ? ( this.Point ) : ( this.EndPoint );

			if( mark != null )
			{
				return p.Line.CompareTo( mark.Point.Line );
			}
			else
			{
				IParsePoint point = obj as IParsePoint;
				return p.Line.CompareTo( point.Line );
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates bookmark.
		/// </summary>
		/// <param name="wrapper">Underlying StreamsWrapper.</param>
		/// <param name="converter">IPositionConverter.</param>
		/// <param name="point">Point, bookmark is associated with.</param>
		/// <param name="endPoint">ParsePoint situated at the end of the bookmarked line.</param>
		public Bookmark( StreamsWrapper wrapper, IPositionConverter converter, IParsePoint point, IParsePoint endPoint )
			: this( wrapper, converter, point, endPoint, m_lastIndex-- )
		{
		}
		/// <summary>
		/// Creates bookmark.
		/// </summary>
		/// <param name="wrapper">Underlying StreamsWrapper.</param>
		/// <param name="converter">IPositionConverter.</param>
		/// <param name="point">Point, bookmark is associated with.</param>
		/// <param name="endPoint">ParsePoint situated at the end of the bookmarked line.</param>
		/// <param name="index">Fast access index of the bookmark.</param>
		public Bookmark( StreamsWrapper wrapper, IPositionConverter converter, IParsePoint point, IParsePoint endPoint, int index )
		{
			if( point == null )
				throw new ArgumentNullException( "point" );

			if( wrapper == null )
				throw new ArgumentNullException( "wrapper" );

			if( converter == null )
				throw new ArgumentNullException( "converter" );

			m_endPoint = endPoint;
			m_point = point;
			m_index = index;
			m_parent = wrapper;
			m_converter = converter;
		}
		#endregion

	}

	/// <summary>
	/// Comparer for Bookmarks to search by index.
	/// </summary>
	public class BookMarkIndexSearch : IComparer
	{
		/// <summary>
		/// Compare bookmarks
		/// </summary>
		/// <param name="x">First bookmark.</param>
		/// <param name="y">Second bookmark.</param>
		/// <returns>Standard comparing result.</returns>
		public int Compare( object x, object y )
		{
			Bookmark mark1 = x as Bookmark;
			Bookmark mark2 = y as Bookmark;
			int result = mark1.Index.CompareTo( ( mark2 != null ) ? mark2.Index : y );

			return result;
		}
	}
}
