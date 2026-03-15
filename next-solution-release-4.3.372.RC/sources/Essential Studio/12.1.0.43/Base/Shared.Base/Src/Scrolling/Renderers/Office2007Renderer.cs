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
	public class Office2007Renderer : 
		ClassicRenderer
	{
		#region Constants
		/// <summary></summary>
		private const int MIN_THUMB_LENGTH_FOR_LINES = 15;
		/// <summary></summary>
		private const int THUMB_LINE_LENGTH = 8;
		#endregion

		#region Enums
		
		#endregion

		#region Fields
		/// <summary></summary>
		private ColorTableOffice2007 m_colorTable;
		/// <summary></summary>
		private static Blend m_blBackGround;
		/// <summary></summary>
		private static Blend m_blBackGroundBlack;
		/// <summary></summary>
		private static Blend m_blArrowButton;
		/// <summary></summary>
		private static Blend m_blArrowButtonSelected;
		/// <summary></summary>
		private static Blend m_blArrowButtonSelectedSilverBlack;
		/// <summary></summary>
		private static Blend m_blThumbBackGround;
		/// <summary></summary>
		private Bitmaps m_hImages;
		#endregion

		#region Initialization
		/// <summary></summary>
		static Office2007Renderer()
		{
			m_blBackGround = new Blend();
			m_blBackGround.Positions = new float[ ]{0.0F, 0.18F, 0.18F, 1.0F};
			m_blBackGround.Factors = new float[ ]{0.0F, 0.7F, 1.0F, 0.7F};

			m_blBackGroundBlack = new Blend();
			m_blBackGroundBlack.Positions = new float[ ]{0.0F, 0.72F, 0.72F, 1.0F};
			m_blBackGroundBlack.Factors = new float[ ]{0.0F, 0.3F, 0.7F, 1.0F};

			m_blArrowButton = new Blend();
			m_blArrowButton.Positions = new float[ ]{0.0F, 0.25F, 0.45F, 0.45F, 0.75F, 1.0F};
			m_blArrowButton.Factors = new float[ ]{0.0F, 0.2F, 0.0F, 1.0F, 0.6F, 1.0F};

			m_blArrowButtonSelected = new Blend();
			m_blArrowButtonSelected.Positions = new float[ ]{0.0F, 0.45F, 0.45F, 0.7F, 0.7F, 1.0F};
			m_blArrowButtonSelected.Factors = new float[ ]{0.7F, 0.1F, 1.0F, 0.9F, 0.6F, 0.0F};

			m_blArrowButtonSelectedSilverBlack = new Blend();
			m_blArrowButtonSelectedSilverBlack.Positions = new float[ ]{0.0F, 0.45F, 0.45F, 1.0F};
			m_blArrowButtonSelectedSilverBlack.Factors = new float[ ]{0.0F, 0.3F, 0.7F, 1.0F};

			m_blThumbBackGround = new Blend();
			m_blThumbBackGround.Positions = new float[ ]{0.0F, 0.45F, 0.45F, 1.0F};
			m_blThumbBackGround.Factors = new float[ ]{1.0F, 0.0F, 0.0F, 1.0F};
		}

		/// <summary>
		/// Initialize new instance of Office2007Renderer
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="colorTable"/>
		public Office2007Renderer( ScrollBarCustomDraw parent, ColorTableOffice2007 colorTable ) : 
			base( parent )
		{
			m_colorTable = colorTable;
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
				throw new ArgumentNullException( "g" );

			if( !m_parent.ThemeEnabled )
			{
				base.DrawArrowButton( g, rcArrow, type, state );
			}
			else
			{
				DrawArrowBackground( g, rcArrow, type, state );
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
				throw new ArgumentNullException( "g" );

			if( !m_parent.ThemeEnabled )
			{
				base.DrawBackground( g, rcBackground, state );
			}
			else
			{
				if( rcBackground.Width > 0 && rcBackground.Height > 0 )
				{
					if( state == ButtonState.Normal )
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

		/// <summary>
		/// Draws thumb for scroll. If theme is disabled than draw classic scroll. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="rcThumb"></param>
		/// <param name="state"></param>
		public override void DrawThumb( Graphics g, Rectangle rcThumb, ButtonState state )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( !m_parent.ThemeEnabled )
			{
				base.DrawThumb( g, rcThumb, state );
			}
			else
			{
				DrawThumbBackground( g, rcThumb, state );
			}
		}
		#endregion

		#region Implementation
		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawBackground( Graphics g, Rectangle rc )
		{
			Color clBegin = m_colorTable.ScrollerGradientBegin;
			Color clEnd = m_colorTable.ScrollerGradientEnd;

			using( LinearGradientBrush brush = IsVerticalScrollBar ?
				GetHorizontalBrush( ref rc, clBegin, clEnd ) :
				GetVerticalBrush( ref rc, clBegin, clEnd ) )
			{
				if( m_colorTable is ColorTableOffice2007Black )
				{
					brush.Blend = m_blBackGroundBlack;
				}
				else
				{
					brush.Blend = m_blBackGround;
				}
				g.FillRectangle( brush, rc );
			}

			using( Pen penLight = new Pen( m_colorTable.ScrollerBorderBegin ) )
			{
				using( Pen penDark = new Pen( m_colorTable.ScrollerBorderEnd ) )
				{
					if( this.IsVerticalScrollBar )
					{
						g.DrawLine( penLight, rc.X, rc.Y, rc.X, rc.Y + rc.Bottom );
						g.DrawLine( penDark, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom );
					}
					else
					{
						g.DrawLine( penLight, rc.X, rc.Y, rc.Right, rc.Y );
						g.DrawLine( penDark, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1 );
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		private void DrawPushedBackground( Graphics g, Rectangle rc )
		{
			Color clBegin = m_colorTable.ThumbPressedBackgroundGradientBegin;
			Color clEnd = m_colorTable.ThumbPressedBackgroundGradientEnd;

			using( LinearGradientBrush brush = IsVerticalScrollBar ?
				GetHorizontalBrush( ref rc, clBegin, clEnd ) :
				GetVerticalBrush( ref rc, clBegin, clEnd ) )
			{
				brush.Blend = m_blThumbBackGround;
				g.FillRectangle( brush, rc );
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="type"/>
		/// <param name="state"/>
		private void DrawArrowBackground( Graphics g, Rectangle rc, ScrollButton type, ButtonState state )
		{
			bool bDrawBorder = true;
			Color clBegin = Color.Empty;
			Color clEnd = Color.Empty;
			Color clBorderDark = Color.Empty;
			Color clBorderLight = Color.Empty;
			Blend blend = new Blend();

			switch( state )
			{
				case ButtonState.Normal:
					clBegin = m_colorTable.ScrollerGradientBegin;
					clEnd = m_colorTable.ScrollerGradientEnd;
					blend = m_blBackGround;
					bDrawBorder = false;

					break;

				case ButtonState.Inactive:
					clBegin = m_colorTable.ArrowButtonGradientBegin;
					clEnd = m_colorTable.ArrowButtonGradientEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderDark;
					clBorderLight = m_colorTable.ArrowButtonBorderLight;
					blend = m_blArrowButton;

					break;

				case ButtonState.Checked:
					clBegin = m_colorTable.ArrowButtonGradientSelectedBegin;
					clEnd = m_colorTable.ArrowButtonGradientSelectedEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderSelectedDark;
					clBorderLight = m_colorTable.ArrowButtonBorderSelectedLight;

					if( m_colorTable is ColorTableOffice2007Silver || m_colorTable is ColorTableOffice2007Black )
					{
						blend = m_blArrowButtonSelectedSilverBlack;
					}
					else
					{
						blend = m_blArrowButtonSelected;
					}

					break;

				case ButtonState.Pushed:
					clBegin = m_colorTable.ArrowButtonGradientPressedBegin;
					clEnd = m_colorTable.ArrowButtonGradientPressedEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderPressedDark;
					clBorderLight = m_colorTable.ArrowButtonBorderPressedLight;
					blend = m_blArrowButtonSelected;

					if( m_colorTable is ColorTableOffice2007Silver || m_colorTable is ColorTableOffice2007Black )
					{
						blend = m_blArrowButtonSelectedSilverBlack;
					}
					else
					{
						blend = m_blArrowButtonSelected;
					}

					break;

				default:
					clBegin = m_colorTable.ScrollerGradientBegin;
					clEnd = m_colorTable.ScrollerGradientEnd;
					blend = m_blBackGround;
					bDrawBorder = false;

					break;
			}

			using( LinearGradientBrush brush = IsVerticalScrollBar ?
				GetHorizontalBrush( ref rc, clBegin, clEnd ) :
				GetVerticalBrush( ref rc, clBegin, clEnd ) )
			{
				brush.Blend = blend;
				g.FillRectangle( brush, rc );
			}

			DrawArrow( g, rc, type, state );

			if( bDrawBorder )
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;

				using( Pen pen = new Pen( clBorderLight ) )
				{
					g.DrawPolygon( pen, GetRoundedPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) );
				}

				using( Pen pen = new Pen( clBorderDark ) )
				{
					g.DrawPolygon( pen, GetRoundedPolygon( rc, 1 ) );
				}
			}
			else
			{
				using( Pen penLight = new Pen( m_colorTable.ScrollerBorderBegin ) )
				{
					using( Pen penDark = new Pen( m_colorTable.ScrollerBorderEnd ) )
					{
						if( this.IsVerticalScrollBar )
						{
							g.DrawLine( penLight, rc.X, rc.Y, rc.X, rc.Y + rc.Bottom );
							g.DrawLine( penDark, rc.Right - 1, rc.Y, rc.Right - 1, rc.Bottom );
						}
						else
						{
							g.DrawLine( penLight, rc.X, rc.Y, rc.Right, rc.Y );
							g.DrawLine( penDark, rc.X, rc.Bottom - 1, rc.Right, rc.Bottom - 1 );
						}
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="state"/>
		private void DrawThumbBackground( Graphics g, Rectangle rc, ButtonState state )
		{
			bool bDrawBorder = true;
			Color clBegin = Color.Empty;
			Color clEnd = Color.Empty;
			Color clBorderDark = Color.Empty;
			Color clBorderLight = Color.Empty;
			Blend blend = new Blend();

			switch( state )
			{
				case ButtonState.Normal:
					clBegin = m_colorTable.ArrowButtonGradientBegin;
					clEnd = m_colorTable.ArrowButtonGradientEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderDark;
					clBorderLight = m_colorTable.ArrowButtonBorderLight;
					blend = m_blArrowButton;

					break;

				case ButtonState.Checked:
					clBegin = m_colorTable.ArrowButtonGradientSelectedBegin;
					clEnd = m_colorTable.ArrowButtonGradientSelectedEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderSelectedDark;
					clBorderLight = m_colorTable.ArrowButtonBorderSelectedLight;

					if( m_colorTable is ColorTableOffice2007Silver || m_colorTable is ColorTableOffice2007Black )
					{
						blend = m_blArrowButtonSelectedSilverBlack;
					}
					else
					{
						blend = m_blArrowButtonSelected;
					}

					break;

				case ButtonState.Pushed:
					clBegin = m_colorTable.ArrowButtonGradientPressedBegin;
					clEnd = m_colorTable.ArrowButtonGradientPressedEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderPressedDark;
					clBorderLight = m_colorTable.ArrowButtonBorderPressedLight;

					if( m_colorTable is ColorTableOffice2007Silver || m_colorTable is ColorTableOffice2007Black )
					{
						blend = m_blArrowButtonSelectedSilverBlack;
					}
					else
					{
						blend = m_blArrowButtonSelected;
					}

					break;

				default:
					clBegin = m_colorTable.ArrowButtonGradientBegin;
					clEnd = m_colorTable.ArrowButtonGradientEnd;
					clBorderDark = m_colorTable.ArrowButtonBorderDark;
					clBorderLight = m_colorTable.ArrowButtonBorderLight;
					blend = m_blArrowButton;

					break;
			}

			using( LinearGradientBrush brush = IsVerticalScrollBar ?
				GetHorizontalBrush( ref rc, clBegin, clEnd ) :
				GetVerticalBrush( ref rc, clBegin, clEnd ) )
			{
				brush.Blend = blend;
				g.FillRectangle( brush, rc );
			}

			DrawLines( g, rc, this.IsVerticalScrollBar );

			if( bDrawBorder )
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;

				using( Pen pen = new Pen( clBorderLight ) )
				{
					g.DrawPolygon( pen, GetRoundedPolygon( Rectangle.Inflate( rc, -1, -1 ), 1 ) );
				}

				using( Pen pen = new Pen( clBorderDark ) )
				{
					g.DrawPolygon( pen, GetRoundedPolygon( rc, 1 ) );
				}
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="type"></param>
		/// <param name="state"/>
		private void DrawArrow( Graphics g, Rectangle rc, ScrollButton type, ButtonState state )
		{
			int iImageWidth = rc.Width;
			int iImageHeight = rc.Height;
			int iArrowHeight = 0;
			int iArrowWidth = 0;
			Point p = Point.Empty;
			Bitmap bmpArrow;

			switch( type )
			{
				case ScrollButton.Down:
				case ScrollButton.Up:
					iArrowWidth = ( int )( ( iImageHeight ) / 2 ) + 1;
					iArrowHeight = ( int )( iArrowWidth / 2 ) + 1;

					bmpArrow = ( type == ScrollButton.Down ) ?
						state == ButtonState.Normal ?
							GetOfficeDownArrowNormal( iArrowWidth, iArrowHeight ) :
							GetOfficeDownArrow( iArrowWidth, iArrowHeight ) :
						state == ButtonState.Normal ?
							GetOfficeUpArrowNormal( iArrowWidth, iArrowHeight ) :
							GetOfficeUpArrow( iArrowWidth, iArrowHeight );

					p.X = rc.Left + ( iImageWidth - iArrowWidth ) / 2;
					p.Y = rc.Top + ( iImageHeight - iArrowHeight ) / 2;

					g.DrawImage( bmpArrow, p );
					break;

				case ScrollButton.Right:
				case ScrollButton.Left:
					iArrowHeight = ( int )( iImageWidth / 2 ) + 1;
					iArrowWidth = ( int )( iArrowHeight / 2 ) + 1;

					bmpArrow = ( type == ScrollButton.Right ) ?
						state == ButtonState.Normal ?
							GetOfficeRightArrowNormal( iArrowWidth, iArrowHeight ) :
							GetOfficeRightArrow( iArrowWidth, iArrowHeight ) :
						state == ButtonState.Normal ?
							GetOfficeLeftArrowNormal( iArrowWidth, iArrowHeight ) :
							GetOfficeLeftArrow( iArrowWidth, iArrowHeight );

					p.X = rc.Left + ( iImageWidth - iArrowWidth ) / 2;
					p.Y = rc.Top + ( iImageHeight - iArrowHeight ) / 2;

					g.DrawImage( bmpArrow, p );
					break;
			}
		}

		/// <summary></summary>
		/// <param name="g"></param>
		/// <param name="rc"></param>
		/// <param name="isVerticalScrollBar"></param>
		private void DrawLines( Graphics g, Rectangle rc, bool isVerticalScrollBar )
		{
			Point pt = Point.Empty;
			Bitmap bmpThumbLines;

			if( rc.Width > MIN_THUMB_LENGTH_FOR_LINES && rc.Height > MIN_THUMB_LENGTH_FOR_LINES )
			{
				if( isVerticalScrollBar )
				{
					bmpThumbLines = GetOfficeThumbLinesVertical();
					pt.X = rc.Left + ( rc.Width - THUMB_LINE_LENGTH ) / 2;
					pt.Y = rc.Top + ( rc.Height - THUMB_LINE_LENGTH ) / 2 + 1;

					g.DrawImage( bmpThumbLines, pt );
				}
				else
				{
					bmpThumbLines = GetOfficeThumbLinesHorizontal();
					pt.X = rc.Left + ( rc.Width - THUMB_LINE_LENGTH ) / 2;
					pt.Y = rc.Top + ( rc.Height - THUMB_LINE_LENGTH ) / 2 + 1;

					g.DrawImage( bmpThumbLines, pt );
				}
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="clBegin"/>
		/// <param name="clEnd"/>
		private Brush GetBackgroundBrushHorizontal( int width, Color clBegin, Color clEnd )
		{
			LinearGradientBrush brush = new LinearGradientBrush( new Rectangle( 0, 0, width, 1 ), clBegin, clEnd, LinearGradientMode.Horizontal );

			return brush;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="height"/>
		/// <param name="clBegin"/>
		/// <param name="clEnd"/>
		private Brush GetBackgroundBrushVertical( int height, Color clBegin, Color clEnd )
		{
			LinearGradientBrush brush = new LinearGradientBrush( new Rectangle( 0, 0, 1, height ), clBegin, clEnd, LinearGradientMode.Vertical );

			return brush;
		}

		/// <summary></summary>
		/// <param name="rc"></param>
		/// <param name="cl1"></param>
		/// <param name="cl2"></param>
		/// <returns></returns>
		protected LinearGradientBrush GetVerticalBrush( ref Rectangle rc, Color cl1, Color cl2 )
		{
			return GetVerticalBrush( rc.Top, rc.Height, cl1, cl2 );
		}

		/// <summary></summary>
		/// <param name="top"></param>
		/// <param name="height"></param>
		/// <param name="cl1"></param>
		/// <param name="cl2"></param>
		/// <returns></returns>
		protected LinearGradientBrush GetVerticalBrush( int top, int height, Color cl1, Color cl2 )
		{
			Rectangle rcBrush = new Rectangle( 0, top, 1, height );

			return new LinearGradientBrush( rcBrush, cl1, cl2, 90 );
		}

		/// <summary></summary>
		/// <param name="rc"></param>
		/// <param name="cl1"></param>
		/// <param name="cl2"></param>
		/// <returns></returns>
		protected LinearGradientBrush GetHorizontalBrush( ref Rectangle rc, Color cl1, Color cl2 )
		{
			Rectangle rcBrush = new Rectangle( rc.Left, rc.Top, rc.Width, 1 );

			return new LinearGradientBrush( rcBrush, cl1, cl2, 0F );
		}

		/// <summary></summary>
		/// <param name="rc"></param>
		/// <param name="iRadius"></param>
		/// <returns></returns>
		public static Point[ ] GetRoundedPolygon( Rectangle rc, int iRadius )
		{
			int iLeft = rc.X;
			int iTop = rc.Y;
			int iRight = rc.Right - 1;
			int iBottom = rc.Bottom - 1;

			Point[ ] points = new Point[ ]
				{
					new Point( iLeft, iTop + iRadius ),
					new Point( iLeft + iRadius, iTop ),
					new Point( iRight - iRadius, iTop ),
					new Point( iRight, iTop + iRadius ),
					new Point( iRight, iBottom - iRadius ),
					new Point( iRight - iRadius, iBottom ),
					new Point( iLeft + iRadius, iBottom ),
					new Point( iLeft, iBottom - iRadius ),
				};

			return points;
		}

		/// <summary> Right office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected override Bitmap GetOfficeRightArrow( int width, int height )
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

						Color clBegin = m_colorTable.ArrowGradientBegin;
						Color clEnd = m_colorTable.ArrowGradientEnd;

						using( Brush brush = GetBackgroundBrushHorizontal( width, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
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
		protected override Bitmap GetOfficeLeftArrow( int width, int height )
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

						Color clBegin = m_colorTable.ArrowGradientBegin;
						Color clEnd = m_colorTable.ArrowGradientEnd;

						using( Brush brush = GetBackgroundBrushHorizontal( width, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
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
		protected override Bitmap GetOfficeDownArrow( int width, int height )
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

						Color clBegin = m_colorTable.ArrowGradientBegin;
						Color clEnd = m_colorTable.ArrowGradientEnd;

						using( Brush brush = GetBackgroundBrushVertical( height, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
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
		protected override Bitmap GetOfficeUpArrow( int width, int height )
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

						Color clBegin = m_colorTable.ArrowGradientBegin;
						Color clEnd = m_colorTable.ArrowGradientEnd;

						using( Brush brush = GetBackgroundBrushVertical( height, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
					}
				}

				m_hImages[ EIMAGE.eiUpArrow ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Right Normal office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected Bitmap GetOfficeRightArrowNormal( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiRightArrowNormal ] as Bitmap;

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

						Color clBegin = m_colorTable.ArrowGradientNormalBegin;
						Color clEnd = m_colorTable.ArrowGradientNormalEnd;

						using( Brush brush = GetBackgroundBrushHorizontal( width, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
					}
				}

				m_hImages[ EIMAGE.eiRightArrowNormal ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Left Normal office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected Bitmap GetOfficeLeftArrowNormal( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiLeftArrowNormal ] as Bitmap;

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

						Color clBegin = m_colorTable.ArrowGradientNormalBegin;
						Color clEnd = m_colorTable.ArrowGradientNormalEnd;

						using( Brush brush = GetBackgroundBrushHorizontal( width, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
					}
				}

				m_hImages[ EIMAGE.eiLeftArrowNormal ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Down Normal office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected Bitmap GetOfficeDownArrowNormal( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiDownArrowNormal ] as Bitmap;

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

						Color clBegin = m_colorTable.ArrowGradientNormalBegin;
						Color clEnd = m_colorTable.ArrowGradientNormalEnd;

						using( Brush brush = GetBackgroundBrushVertical( height, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
					}
				}

				m_hImages[ EIMAGE.eiDownArrowNormal ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Up Normal office arrow. </summary>
		/// <returns></returns>
		/// <param name="width"/>
		/// <param name="height"/>
		protected Bitmap GetOfficeUpArrowNormal( int width, int height )
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiUpArrowNormal ] as Bitmap;

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

						Color clBegin = m_colorTable.ArrowGradientNormalBegin;
						Color clEnd = m_colorTable.ArrowGradientNormalEnd;

						using( Brush brush = GetBackgroundBrushVertical( height, clBegin, clEnd ) )
						{
							g.FillRegion( brush, rg );
						}
					}
				}

				m_hImages[ EIMAGE.eiUpArrowNormal ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Vertical thumb lines. </summary>
		/// <returns></returns>
		protected Bitmap GetOfficeThumbLinesVertical()
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiThumbLinesVertical ] as Bitmap;

			if( bitmap == null )
			{
				bitmap = new Bitmap( THUMB_LINE_LENGTH, THUMB_LINE_LENGTH );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Point ptBegin = Point.Empty;
					Point ptEnd = new Point( THUMB_LINE_LENGTH - 1, 0 );

					Color clBegin = m_colorTable.ThumbLinesGradientBegin;
					Color clEnd = m_colorTable.ThumbLinesGradientEnd;

					using( Pen pen = new Pen( GetBackgroundBrushVertical( THUMB_LINE_LENGTH, clBegin, clEnd ) ) )
					{
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.Y += 2;
						ptEnd.Y += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.Y += 2;
						ptEnd.Y += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.Y += 2;
						ptEnd.Y += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
					}
				}

				m_hImages[ EIMAGE.eiThumbLinesVertical ] = bitmap;
			}

			return bitmap;
		}

		/// <summary> Horizontal thumb lines. </summary>
		/// <returns></returns>
		protected Bitmap GetOfficeThumbLinesHorizontal()
		{
			Bitmap bitmap = m_hImages[ EIMAGE.eiThumbLinesHorizontal ] as Bitmap;

			if( bitmap == null )
			{
				bitmap = new Bitmap( THUMB_LINE_LENGTH, THUMB_LINE_LENGTH );

				using( Graphics g = Graphics.FromImage( bitmap ) )
				{
					g.Clear( Color.Transparent );

					Point ptBegin = Point.Empty;
					Point ptEnd = new Point( 0, THUMB_LINE_LENGTH - 1 );

					Color clBegin = m_colorTable.ThumbLinesGradientBegin;
					Color clEnd = m_colorTable.ThumbLinesGradientEnd;

					using( Pen pen = new Pen( GetBackgroundBrushHorizontal( THUMB_LINE_LENGTH, clBegin, clEnd ) ) )
					{
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.X += 2;
						ptEnd.X += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.X += 2;
						ptEnd.X += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
						ptBegin.X += 2;
						ptEnd.X += 2;
						g.DrawLine( pen, ptBegin, ptEnd );
					}
				}

				m_hImages[ EIMAGE.eiThumbLinesHorizontal ] = bitmap;
			}

			return bitmap;
		}
		#endregion
	}

    /// <summary></summary>
    internal enum EIMAGE
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
        eiDownArrowNormal,
        /// <summary></summary>
        eiRightArrowNormal,
        /// <summary></summary>
        eiLeftArrowNormal,
        /// <summary></summary>
        eiUpArrowNormal,
        /// <summary></summary>
        eiThumbLinesVertical,
        /// <summary></summary>
        eiThumbLinesHorizontal,
        /// <summary></summary>
        MAX,
    }
}