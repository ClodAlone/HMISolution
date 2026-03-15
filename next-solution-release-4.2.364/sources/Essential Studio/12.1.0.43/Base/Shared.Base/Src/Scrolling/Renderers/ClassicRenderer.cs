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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
	/// <summary></summary>
	public class ClassicRenderer :
		BasicRenderer
	{
		#region Constants
		/// <summary></summary>
		private int BORDER_WIDTH = 2;
		#endregion

		#region Enums
		/// <summary></summary>
		private enum EIMAGE
		{
			/// <summary></summary>
			eiDownArrow,
			/// <summary></summary>
			eiRightArrow,
			/// <summary></summary>
			eiLeftArrow,
			/// <summary></summary>
			eiUpArrow,
			/// <summary></summary>
			MAX,
		}
		#endregion

		#region Fields
		/// <summary></summary>
		private Bitmaps m_hImages;
		#endregion

		#region Initialization
		/// <summary></summary>
		/// <param name="isVerticalScrollBar"/>
		protected internal ClassicRenderer( bool isVerticalScrollBar )
			: base( isVerticalScrollBar )
		{
		}

		/// <summary></summary>
		/// <param name="parent"/>
		public ClassicRenderer( ScrollBarCustomDraw parent )
			: base( parent )
		{
			m_hImages = new Bitmaps( ( int )EIMAGE.MAX );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Draws arrow button of scroll. If theme is disabled than draw classic scroll.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rcArrow"></param>
		/// <param name="type"></param>
		/// <param name="state"></param>
		public override void DrawArrowButton( Graphics g, Rectangle rcArrow, ScrollButton type, ButtonState state )
		{
			if( null == g )
			{
				throw new ArgumentNullException( "g" );
			}

            if( rcArrow.Width > 0 && rcArrow.Height > 0 )			
            {
                if( !m_parent.ThemeEnabled )
			    {
			    	base.DrawArrowButton( g, rcArrow, type, state );
			    }
			    else
			    {
				    if( m_parent.Enabled )
				    {
					    if( state == ButtonState.Pushed )
					    {
						    DrawArrowPushedBackground( g, rcArrow );
						    DrawArrow( g, rcArrow, type, state );
					    }
					    else
					    {
						    ControlPaint.DrawScrollButton( g, rcArrow, type, ButtonState.Normal );
					    }
				    }
				    else
				    {
					    ControlPaint.DrawScrollButton( g, rcArrow, type, ButtonState.Inactive );
				    }
			    }
            }
		}

		/// <summary>
		/// Draws background of scroll. If theme is disabled than draw classic scroll.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rcBackground"></param>
		/// <param name="state"></param>
		public override void DrawBackground( Graphics g, Rectangle rcBackground, ButtonState state )
		{
			if( null == g )
			{
				throw new ArgumentNullException( "g" );
			}

            if( rcBackground.Width > 0 && rcBackground.Height > 0 )			
            {
			    if( !m_parent.ThemeEnabled )
			    {
				    base.DrawBackground( g, rcBackground, state );
			    }
			    else
			    {
				    if( rcBackground.Width > 0 && rcBackground.Height > 0 )
				    {
					    if( state == ButtonState.Normal || state == ButtonState.Inactive )
					    {
						    DrawBackground( g, rcBackground );
					    }
					    else if( state == ButtonState.Pushed )
					    {
						    DrawPushedBackground( g, rcBackground );
					    }
				    }
			    }
            }
		}

		/// <summary>
		/// Draws thumb for scroll. If theme is disabled than draw classic scroll.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rcThumb"></param>
		/// <param name="state"></param>
		public override void DrawThumb( Graphics g, Rectangle rcThumb, ButtonState state )
		{
			if( null == g )
			{
				throw new ArgumentNullException( "g" );
			}

            if( rcThumb.Width > 0 && rcThumb.Height > 0 )			
            {
			    if( !m_parent.ThemeEnabled )
			    {
				    base.DrawThumb( g, rcThumb, state );
			    }
			    else
			    {
				    if( !m_parent.Enabled )
				    {
					    DrawBackground( g, rcThumb );
				    }
				    else
				    {
					    DrawThumbArrowBackground( g, rcThumb );
				    }
			    }
            }
		}
		#endregion

		#region Implementation
		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="type"></param>
		/// <param name="state"/>
		private void DrawArrow( Graphics g, Rectangle rc, ScrollButton type, ButtonState state )
		{
			int iImageWidth = rc.Width - 2 * BORDER_WIDTH;
			int iImageHeight = rc.Height - 2 * BORDER_WIDTH;
			int iArrowHeight = 0;
			int iArrowWidth = 0;
			int offsetX = 0;
			int offsetY = 0;
			Point p = Point.Empty;
			Bitmap bmpArrow;

			if( state == ButtonState.Pushed )
			{
				if( type == ScrollButton.Up || type == ScrollButton.Left )
				{
					offsetX = 1;
					offsetY = 1;
				}
				else if( type == ScrollButton.Down )
				{
					offsetX = 1;
					offsetY = 2;
				}
				else
				{
					offsetX = 2;
					offsetY = 1;
				}
			}

			switch( type )
			{
				case ScrollButton.Down:
				case ScrollButton.Up:
					iArrowWidth = ( int )( ( iImageHeight ) / 2 ) + 1;
					iArrowHeight = ( int )( iArrowWidth / 2 ) + 1;

					bmpArrow = ( type == ScrollButton.Down ) ?
						GetOfficeDownArrow( iArrowWidth, iArrowHeight ) :
						GetOfficeUpArrow( iArrowWidth, iArrowHeight );

					p.X = rc.Left + BORDER_WIDTH + ( iImageWidth - iArrowWidth ) / 2 + offsetX;
					p.Y = rc.Top + BORDER_WIDTH + ( iImageHeight - iArrowHeight ) / 2 + offsetY;

					g.DrawImage( bmpArrow, p );
					break;

				case ScrollButton.Right:
				case ScrollButton.Left:
					iArrowHeight = ( int )( iImageWidth / 2 ) + 1;
					iArrowWidth = ( int )( iArrowHeight / 2 ) + 1;

					bmpArrow = ( type == ScrollButton.Right ) ?
						GetOfficeRightArrow( iArrowWidth, iArrowHeight ) :
						GetOfficeLeftArrow( iArrowWidth, iArrowHeight );

					p.X = rc.Left + BORDER_WIDTH + ( iImageWidth - iArrowWidth ) / 2 + offsetX;
					p.Y = rc.Top + BORDER_WIDTH + ( iImageHeight - iArrowHeight ) / 2 + offsetY;

					g.DrawImage( bmpArrow, p );
					break;
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rcArrow"></param>
		private void DrawArrowPushedBackground( Graphics g, Rectangle rcArrow )
		{
			// Fill rectangle.
			g.FillRectangle( SystemBrushes.Control, rcArrow );

			// Draw border.
			rcArrow.Width -= 1;
			rcArrow.Height -= 1;
			g.DrawRectangle( SystemPens.ControlDark, rcArrow );
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rcBackground"></param>
		private void DrawPushedBackground( Graphics g, Rectangle rcBackground )
		{
			using( TextureBrush brush = new TextureBrush( GetBackgroundPressedImage(), WrapMode.Tile ) )
			{
				g.FillRectangle( brush, rcBackground );
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rcBackground"></param>
		private void DrawBackground( Graphics g, Rectangle rcBackground )
		{
			using( TextureBrush brush = new TextureBrush( GetBackgroundImage(), WrapMode.Tile ) )
			{
				g.FillRectangle( brush, rcBackground );
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rcThumb"></param>
		private void DrawThumbArrowBackground( Graphics g, Rectangle rcThumb )
		{
			// Fill rectangle.
			g.FillRectangle( SystemBrushes.Control, rcThumb );

			// Draw lines around.
			g.DrawLine( SystemPens.ControlLight, rcThumb.X, rcThumb.Y, rcThumb.Right - 2, rcThumb.Y );
			g.DrawLine( SystemPens.ControlLight, rcThumb.X, rcThumb.Y, rcThumb.X, rcThumb.Bottom - 2 );

			g.DrawLine( SystemPens.ControlDarkDark, rcThumb.X, rcThumb.Bottom - 1, rcThumb.Right - 1, rcThumb.Bottom - 1 );
			g.DrawLine( SystemPens.ControlDarkDark, rcThumb.Right - 1, rcThumb.Y, rcThumb.Right - 1, rcThumb.Bottom - 1 );

			g.DrawLine( SystemPens.ControlLightLight, rcThumb.X + 1, rcThumb.Y + 1, rcThumb.Right - 3, rcThumb.Y + 1 );
			g.DrawLine( SystemPens.ControlLightLight, rcThumb.X + 1, rcThumb.Y + 1, rcThumb.X + 1, rcThumb.Bottom - 3 );

			g.DrawLine( SystemPens.ControlDark, rcThumb.X + 1, rcThumb.Bottom - 2, rcThumb.Right - 2, rcThumb.Bottom - 2 );
			g.DrawLine( SystemPens.ControlDark, rcThumb.Right - 2, rcThumb.Y + 1, rcThumb.Right - 2, rcThumb.Bottom - 2 );
		}

		/// <summary></summary>
		/// <returns></returns>
		private Bitmap GetBackgroundImage()
		{
			Bitmap bmp = new Bitmap( 2, 2 );

			using( Graphics g = Graphics.FromImage( bmp ) )
			{
				g.FillRectangle( SystemBrushes.ControlLightLight, 1, 0, 1, 1 );
				g.FillRectangle( SystemBrushes.ControlLightLight, 0, 1, 1, 1 );
				g.FillRectangle( SystemBrushes.ControlLight, 0, 0, 1, 1 );
				g.FillRectangle( SystemBrushes.ControlLight, 1, 1, 1, 1 );
			}

			return bmp;
		}

		/// <summary></summary>
		/// <returns></returns>
		private Bitmap GetBackgroundPressedImage()
		{
			Bitmap bmp = new Bitmap( 2, 2 );

			using( Graphics g = Graphics.FromImage( bmp ) )
			{
                using (Brush brush = new SolidBrush(Color.FromArgb(43, 47, 55)))
                {
                    g.FillRectangle(brush, 1, 0, 1, 1);
                    g.FillRectangle(brush, 0, 1, 1, 1);
                }
				g.FillRectangle( SystemBrushes.ControlText, 0, 0, 1, 1 );
				g.FillRectangle( SystemBrushes.ControlText, 1, 1, 1, 1 );
			}

			return bmp;
		}

		/// <summary> Right office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected virtual Bitmap GetOfficeRightArrow( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiRightArrow ] as Bitmap;

			if( bitmap == null || bitmap.Width != width || bitmap.Height != height )
			{
				bitmap = new Bitmap( width, height );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Rectangle rcRightArrow = new Rectangle( 0, 0, 1, height );

					using( Region rg = new Region( rcRightArrow ) )
					{
						rg.Union( rcRightArrow );

						for( int i = 0 ; i < width ; i++ )
						{
							rcRightArrow.Inflate( 0, -1 );
							rcRightArrow.X += 1;
							rg.Union( rcRightArrow );
						}

						g.FillRegion( SystemBrushes.ControlText, rg );
					}
				}

				m_hImages[ EIMAGE.eiRightArrow ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Left office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected virtual Bitmap GetOfficeLeftArrow( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiLeftArrow ] as Bitmap;

			if( bitmap == null || bitmap.Width != width || bitmap.Height != height )
			{
				bitmap = new Bitmap( width, height );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Rectangle rcLeftArrow = new Rectangle( width - 1, 0, 1, height );

					using( Region rg = new Region( rcLeftArrow ) )
					{
						rg.Union( rcLeftArrow );

						for( int i = 0 ; i < width ; i++ )
						{
							rcLeftArrow.Inflate( 0, -1 );
							rcLeftArrow.X -= 1;
							rg.Union( rcLeftArrow );
						}

						g.FillRegion( SystemBrushes.ControlText, rg );
					}
				}

				m_hImages[ EIMAGE.eiLeftArrow ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Down office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected virtual Bitmap GetOfficeDownArrow( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiDownArrow ] as Bitmap;

			if( bitmap == null || bitmap.Width != width || bitmap.Height != height )
			{
				bitmap = new Bitmap( width, height );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Rectangle rcUpArrow = new Rectangle( 0, 0, width, 1 );

					using( Region rg = new Region( rcUpArrow ) )
					{
						rg.Union( rcUpArrow );

						for( int i = 0 ; i < height ; i++ )
						{
							rcUpArrow.Inflate( -1, 0 );
							rcUpArrow.Y += 1;
							rg.Union( rcUpArrow );
						}

						g.FillRegion( SystemBrushes.ControlText, rg );
					}
				}

				m_hImages[ EIMAGE.eiDownArrow ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Up office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected virtual Bitmap GetOfficeUpArrow( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiUpArrow ] as Bitmap;

			if( bitmap == null || bitmap.Width != width || bitmap.Height != height )
			{
				bitmap = new Bitmap( width, height );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Rectangle rcDownArrow = new Rectangle( 0, height - 1, width, 1 );

					using( Region rg = new Region( rcDownArrow ) )
					{
						rg.Union( rcDownArrow );

						for( int i = 0 ; i < height ; i++ )
						{
							rcDownArrow.Inflate( -1, 0 );
							rcDownArrow.Y -= 1;
							rg.Union( rcDownArrow );
						}

						g.FillRegion( SystemBrushes.ControlText, rg );
					}
				}

				m_hImages[ EIMAGE.eiUpArrow ] = bitmap;
			}

			return bitmap;
		}
		#endregion
	}
}