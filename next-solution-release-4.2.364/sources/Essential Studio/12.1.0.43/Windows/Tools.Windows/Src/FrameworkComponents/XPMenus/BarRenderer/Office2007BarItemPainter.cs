#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File Using
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// This class allows users to draw BarItem and CommandBar for Office2007 visual style.
	/// </summary>
	public class Office2007BarItemPainter
	{
		#region Class Initialize/Finalize Methods

		static Office2007BarItemPainter()
		{
			m_blButtonHorizontalShadow = new Blend();
			m_blButtonHorizontalShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
			m_blButtonHorizontalShadow.Factors = new float[] { 0.0f, 0.3f, 0.6f, 0.8f, 0.95f, 1.0f };

			m_blButtonVerticalShadow = new Blend();
			m_blButtonVerticalShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
			m_blButtonVerticalShadow.Factors = new float[] { 0.0f, 0.2f, 0.5f, 0.9f, 0.95f, 1.0f };

			m_blButtonFlash = new Blend();
			m_blButtonFlash.Positions = new float[] { 0f, 0.4f, 1f };
			m_blButtonFlash.Factors = new float[] { 0f, 0.6f, 1f };

			m_htBitmaps = new Hashtable();
		}


		#endregion

		#region Class Static Constants
		/// <summary>
		/// Radius for rounded polygon.
		/// </summary>
		private static readonly int s_iItemRadius = 1;
		/// <summary>
		/// Default size for flash image.
		/// </summary>
		private static readonly Size s_flashSize = new Size( 20, 20 );
		#endregion

		#region Class Static Members
		/// <summary>
		/// Blend for horizontal shadow item.
		/// </summary>
		private static  Blend m_blButtonHorizontalShadow = null;
		/// <summary>
		/// Blend for vertical shadow item.
		/// </summary>
		private static  Blend m_blButtonVerticalShadow = null;
		/// <summary>
		/// Blend for flash.
		/// </summary>
		private static Blend m_blButtonFlash = null;
		/// <summary>
		/// Hashtable for flash bitmaps.
		/// </summary>
		protected static Hashtable m_htBitmaps;
		/// <summary>
		/// Color table for Office2007 visual style.
		/// </summary>
		private static Office2007Colors m_colorTable = null;
		#endregion

		#region Class Static Properties
		
		/// <summary>
		/// Gets bitmap for checked flash.
		/// </summary>
		protected static Bitmap CheckedFlashImage
		{
			get
			{
				Bitmap bitmap = m_htBitmaps[ EBITMAP.ebCheckedFlash ] as Bitmap;

				if( bitmap == null )
				{
					bitmap = GetFlashImage( ColorTable.BarItemCheckFlashColor, s_flashSize );
					m_htBitmaps[ EBITMAP.ebCheckedFlash ] = bitmap;
				}

				return bitmap;
			}
		}

		/// <summary>
		/// Gets bitmap for selected flash.
		/// </summary>
		protected static Bitmap SelectedFlashImage
		{
			get
			{
				Bitmap bitmap = m_htBitmaps[ EBITMAP.ebSelectedFlash ] as Bitmap;

				if( bitmap == null )
				{
					bitmap = GetFlashImage( ColorTable.BarItemSelectFlashColor, s_flashSize );
					m_htBitmaps[ EBITMAP.ebSelectedFlash ] = bitmap;
				}

				return bitmap;
			}
		}

		/// <summary>
		/// Gets bitmap for pressed flash.
		/// </summary>
		protected static Bitmap PressedFlashImage
		{
			get
			{
				Bitmap bitmap = m_htBitmaps[ EBITMAP.ebPressedFlash ] as Bitmap;

				if( bitmap == null )
				{
					bitmap = GetFlashImage( ColorTable.BarItemPressFlashColor, s_flashSize );
					m_htBitmaps[ EBITMAP.ebPressedFlash ] = bitmap;
				}

				return bitmap;
			}
		}
		/// <summary>
		/// Gets or sets color table for Office2007 visual style.
		/// </summary>
		public static Office2007Colors ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( m_colorTable == null ) ? Office2007Colors.Default :
					m_colorTable;

				return colorTable;
			}
			set
			{
				if( value != m_colorTable )
				{
					m_colorTable = value;
				}
			}
		}
		#endregion

		#region Class Statc Utility Methods

		#region Utility
		/// <summary>
		/// Gets rectangle for background of the BarItem
		/// </summary>
		private static Rectangle GetButtonBackgroundRect( Rectangle rc )
		{
			Rectangle rcResult = rc;
			rcResult.Inflate( -1, -1 );

			return rcResult;
		}
		/// <summary>
		/// Gets rounded polygon.
		/// </summary>
		public static Point[] GetRoundedPolygon( Rectangle rc, int iRadius )
		{
			int iLeft = rc.X;
			int iTop = rc.Y;
			int iRight = rc.Right - 1;
			int iBottom = rc.Bottom - 1;

			Point[] points = new Point[]
				{
					new Point(iLeft, iTop+iRadius),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight-iRadius, iTop),
					new Point(iRight, iTop+iRadius),
					new Point(iRight, iBottom-iRadius),
					new Point(iRight-iRadius, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
			};

			return points;
		}

		/// <summary>
		/// Gets modified color.
		/// </summary>
		public static Color GetAlphaBlendedColor(Color src, Color dest, int alpha)
		{
			int R = ((src.R * alpha) + ((0xff - alpha) * dest.R)) / 0xff;
			int G = ((src.G * alpha) + ((0xff - alpha) * dest.G)) / 0xff;
			int B = ((src.B * alpha) + ((0xff - alpha) * dest.B)) / 0xff;
			int A = ((src.A * alpha) + ((0xff - alpha) * dest.A)) / 0xff;

			return Color.FromArgb(A, R, G, B);
		}

		/// <summary>
		/// Gets vertical linear gradient brush.
		/// </summary>
		protected static LinearGradientBrush GetVerticalBrush( Rectangle rc, Color cl1, Color cl2 )
		{
			return GetVerticalBrush( rc.Top, rc.Height, cl1, cl2 );
		}
		
		/// <summary>
		/// Gets vertical linear gradient brush.
		/// </summary>
		protected static LinearGradientBrush GetVerticalBrush( int top, int height, Color cl1, Color cl2 )
		{
			Rectangle rcBrush = new Rectangle( 0, top, 1, height );
			return new LinearGradientBrush( rcBrush, cl1, cl2, LinearGradientMode.Vertical );
		}

		/// <summary>
		/// Gets horizontal linear gradient brush.
		/// </summary>
		protected static LinearGradientBrush GetHorizontalBrush( Rectangle rc, Color cl1, Color cl2 )
		{
			return GetHorizontalBrush( rc.Left, rc.Width, cl1, cl2 );
		}

		/// <summary>
		/// Gets horizontal linear gradient brush.
		/// </summary>
		protected static LinearGradientBrush GetHorizontalBrush( int left, int width, Color cl1, Color cl2 )
		{
			Rectangle rcBrush = new Rectangle( left - 1, 0, width + 1, 1 );
			return new LinearGradientBrush( rcBrush, cl1, cl2, LinearGradientMode.Horizontal );
		}

		/// <summary>
		/// Draws background.
		/// </summary>
		private static void PaintGradientSelected( Graphics g, Rectangle rc, Color clBegin, Color clEnd,
			bool bHorizontal )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				Color clMedium = GetAlphaBlendedColor( clBegin, clEnd, 128 );

				int topHeight = ( bHorizontal ) ? ( int )( rc.Height * 0.38 ) : 
					( int )( rc.Width * 0.38 );
				int bottomHeight = ( bHorizontal ) ? rc.Height - topHeight :
					rc.Width - topHeight;

				// draw top ( right - for vertical alignment ) part of the gradient
				if ( topHeight > 0 )
				{
					Rectangle rcTop = ( bHorizontal ) ? new Rectangle( rc.X, rc.Y, rc.Width, topHeight + 1 ) :
						new Rectangle( rc.X + bottomHeight, rc.Y, topHeight, rc.Height );

					using( LinearGradientBrush brush = ( bHorizontal ) ?
							   GetVerticalBrush( rcTop, clBegin, clMedium ) : 
							   GetHorizontalBrush( rcTop, clMedium, clBegin ) )
					{
						brush.WrapMode = WrapMode.TileFlipY;
						g.FillRectangle( brush, rcTop );
					}
				}

				// draw bottom ( left - for vertical alignment ) part of the gradient
				Rectangle rcBottom = ( bHorizontal ) ? new Rectangle( rc.X, rc.Y + topHeight, rc.Width, bottomHeight ) :
					new Rectangle( rc.X - 1, rc.Y, bottomHeight + 1, rc.Height );

				using( LinearGradientBrush brush = ( bHorizontal ) ?
						   GetVerticalBrush( rcBottom, clEnd, clMedium ) : 
						   GetHorizontalBrush( rcBottom, clMedium, clEnd ) )
				{
					brush.WrapMode = WrapMode.TileFlipY;
					g.FillRectangle( brush, rcBottom );
				}
			}
		}

		/// <summary>
		/// Draws outside border for pressed state.
		/// </summary>
		private static void PainButtonPressedOutsideBorder(  Graphics g, Rectangle rc, Color clHighlightBorder, 
			bool bDropDown, bool bHorizontal )
		{
			int iOffset = ( bDropDown ) ? 1 : 0;
			Color clBorderBegin = ColorTable.BarItemPressBorderColor;
			Color clBorderEnd = GetAlphaBlendedColor( clBorderBegin, Color.White, 128 );

			if( bHorizontal )
			{
				using( LinearGradientBrush brush = GetVerticalBrush( rc.Top, rc.Height, clBorderBegin, clBorderEnd ) )
				{
					brush.TranslateTransform( 2 * ( rc.Width + 2 * rc.X ),
						rc.Height, MatrixOrder.Append );

					using( Pen pen = new Pen( brush ) )
					{
						Point[] points = new Point[]
							{
								new Point( rc.Left, rc.Bottom - 2 + iOffset ),
								new Point( rc.Left, rc.Top + 1 ),
								new Point( rc.Left + 1, rc.Top ),
								new Point( rc.Right - 2, rc.Top ),
								new Point( rc.Right - 1, rc.Top + 1 ),
								new Point( rc.Right - 1, rc.Bottom - 2 + iOffset ),
						};

						g.DrawLines( pen, points );
					}
				}
			}
			else
			{
				using( LinearGradientBrush brush = GetHorizontalBrush( rc.Left, rc.Width, clBorderEnd, clBorderBegin ) )
				{
					using( Pen pen = new Pen( brush ) )
					{
						Point[] points = new Point[]
							{
								new Point( rc.Left + 1, rc.Top ),
								new Point( rc.Right - 2, rc.Top ),
								new Point( rc.Right - 1, rc.Top + 1 ),
								new Point( rc.Right - 1, rc.Bottom - 2 ),
								new Point( rc.Right - 2, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Bottom - 1 )
							};

						g.DrawLines( pen, points );
					}
				}
			}
		}
		
		/// <summary>
		/// Draws inside border for pressed state.
		/// </summary>
		private static void PainButtonPressedInsideBorder(  Graphics g, Rectangle rc, Color clHighlightBorder, 
			bool bDropDown, bool bHorizontal )
		{
			Color clHighlightBegin = Color.FromArgb( 32, clHighlightBorder );
			Color clHighlightEnd = clHighlightBorder;

			if( bHorizontal )
			{
				using( LinearGradientBrush brush = GetVerticalBrush(rc.Top + 1, rc.Height - 1, 
						   clHighlightBegin, clHighlightEnd ) )
				{
					using( Pen pen = new Pen( brush ) )
					{
						Point[] points = new Point[]
							{
								new Point( rc.Right - 2, rc.Y + 2 ),
								new Point( rc.Right - 2, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Y + 2 )
							};

						g.DrawLines( pen, points );
					}
				}
			}
			else
			{
				using( LinearGradientBrush brush = GetHorizontalBrush(rc.Left + 1, rc.Width - 1, 
						   clHighlightEnd, clHighlightBegin ) )
				{
					using ( Pen pen = new Pen( brush ) )
					{
						Point[] points = new Point[]
							{
								new Point( rc.Right - 3, rc.Y + 1 ),
								new Point( rc.X, rc.Y + 1 ),
								new Point( rc.X, rc.Bottom - 2 ),
								new Point( rc.Right - 3, rc.Bottom - 2 )
							};

						g.DrawLines( pen, points );
					}
				}
			}
		}

		/// <summary>
		/// Draws border for pressed state.
		/// </summary>
		private static void PaintButtonPressedBorder( Graphics g, Rectangle rc, Color clHighlightBorder, 
			bool bDropDown, bool bHorizontal )
		{
			// draw outside border
			PainButtonPressedOutsideBorder( g, rc, clHighlightBorder, bDropDown, bHorizontal );

			// draw inside border
			PainButtonPressedInsideBorder( g, rc, clHighlightBorder, bDropDown, bHorizontal );
		}
		
		/// <summary>
		/// Draw highlighted border.
		/// </summary>
		private static void PaintButtonSelectedBorder( Graphics g, Rectangle rc )
		{
			if( rc.Width > 0 && rc.Height > 0 )
			{
				GraphicsState gState = g.Save();
				g.SmoothingMode = SmoothingMode.AntiAlias;

				using( Pen selectedPen = new Pen( ColorTable.BarItemHighlightBorderColor ) )
				{
					g.DrawPolygon( selectedPen, GetRoundedPolygon( rc, s_iItemRadius ) );
				}
				using( Pen highlightPen = new Pen( GetAlphaBlendedColor( Color.Transparent, Color.White, 128 ) ) )
				{
					g.DrawRectangle( highlightPen, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3 );
				}

				g.Restore( gState );
			}
		}
		/// <summary>
		/// Gets flash bitmap.
		/// </summary>
		protected static Bitmap GetFlashImage( Color colorBase, Size size )
		{
			Rectangle flashRect = new Rectangle( 0, 0, size.Width, size.Height );
			Bitmap bitmap = new Bitmap( flashRect.Width, flashRect.Height );

			using( Graphics g = Graphics.FromImage( bitmap ) )
			{
				GraphicsPath path = new GraphicsPath();
				path.AddEllipse( flashRect );

				using( PathGradientBrush flashBrush = new PathGradientBrush( path ) )
				{
					flashBrush.Blend = m_blButtonFlash;
					flashBrush.CenterColor = colorBase;
					flashBrush.SurroundColors = new Color[] { Color.Transparent };

					g.FillRectangle( flashBrush, flashRect );
				}
			}

			return bitmap;
		}
		/// <summary>
		/// Draws shadow for BarItem.
		/// </summary>
		private static void PaintBarItemShadow( Graphics g, Rectangle rect, Color color, bool bHorizontal )
		{
			if( rect.Width > 0 && rect.Height > 0 )
			{
				Color clBegin = Color.FromArgb( 160, color ); 
				Color clEnd = Color.Transparent;
				int iOffset = ( bHorizontal ) ? Math.Max( rect.Height / 10, 2 ) :
					Math.Max( rect.Width / 10, 2 );
				Rectangle destRect = ( bHorizontal ) ?
					new Rectangle( rect.X, rect.Y, rect.Width, iOffset ) :
					new Rectangle( rect.Right - iOffset, rect.Y, iOffset, rect.Height );

				LinearGradientBrush brush = ( bHorizontal ) ?
					GetVerticalBrush( destRect, clBegin, clEnd ) :
					GetHorizontalBrush( destRect, clEnd, clBegin );
				
				if( bHorizontal )
				{
					brush.TranslateTransform( 2 * ( rect.Width + 2 * rect.X ),
						destRect.Height, MatrixOrder.Append );
				}

				using( brush )
				{
					brush.Blend = ( bHorizontal ) ? m_blButtonHorizontalShadow : m_blButtonVerticalShadow;
					g.FillRectangle( brush, destRect );
				}
			}
		}


		#endregion
		
		#region Draw BarItem
		/// <summary>
		/// Draws flash for pressed state of the BarItem.
		/// </summary>
		private static void PaintFlashPress( Graphics g, Rectangle rect, bool bHorizontal )
		{
			g.SmoothingMode = SmoothingMode.None;
			Rectangle clipRect = ( bHorizontal ) ? 
				new Rectangle( rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2 ) :
				new Rectangle( rect.X, rect.Y, rect.Width / 2, rect.Height );
			g.SetClip( clipRect );

			Rectangle destRect = ( bHorizontal ) ?
				new Rectangle( rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height ) :
				new Rectangle( rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height );

			g.DrawImage( PressedFlashImage, destRect );
			g.ResetClip();
		}

		/// <summary>
		/// Draws flash for selected state of the BarItem.
		/// </summary>
		private static void PaintFlashSelected( Graphics g, Rectangle rect, bool bHorizontal )
		{
			g.SmoothingMode = SmoothingMode.None;
			Rectangle clipRect = ( bHorizontal ) ? 
				new Rectangle( rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2 ) :
				new Rectangle( rect.X, rect.Y, rect.Width / 2, rect.Height );
			g.SetClip( clipRect );

			Rectangle destRect = ( bHorizontal ) ?
				new Rectangle( rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height ) :
				new Rectangle( rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height );

			g.DrawImage( SelectedFlashImage, destRect );
			g.ResetClip();
		}

		/// <summary>
		/// Draws flash for checked state of the BarItem.
		/// </summary>
		private static void PaintFlashCheck( Graphics g, Rectangle rect, bool bHorizontal )
		{
			g.SmoothingMode = SmoothingMode.None;
			Rectangle clipRect = ( bHorizontal ) ? 
				new Rectangle( rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2 ) :
				new Rectangle( rect.X, rect.Y, rect.Width / 2, rect.Height );
			g.SetClip( clipRect );

			Rectangle destRect = ( bHorizontal ) ?
				new Rectangle( rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height ) :
				new Rectangle( rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height );

			g.DrawImage( CheckedFlashImage, destRect );
			g.ResetClip();
		}

		/// <summary>
		/// Draws highlighted background for BarItem.
		/// </summary>
		private static void DrawBarItemBackgroundHighlight( Graphics g, Rectangle rect, bool bHorizontal )
		{
			// draw background
			Color clBegin = ColorTable.MenuItemLightColor;
			Color clEnd = ColorTable.MenuItemDarkColor;
			Rectangle backRect = GetButtonBackgroundRect( rect );
			PaintGradientSelected( g, backRect, clBegin, clEnd, bHorizontal );

			// draw flash
			PaintFlashSelected( g, backRect, bHorizontal );

			// draw border
			PaintButtonSelectedBorder( g, rect );
		}

		/// <summary>
		/// Draws pressed background for BarItem.
		/// </summary>
		private static void DrawBarItemBackgroundPress( Graphics g, Rectangle rect, bool bHorizontal )
		{
			// draw background gradient
			Color clBegin = ColorTable.BarItemPressLightColor;
			Color clEnd = ColorTable.BarItemPressDarkColor;
			Rectangle backRect = GetButtonBackgroundRect( rect );
			PaintGradientSelected( g, backRect, clBegin, clEnd, bHorizontal );

			// draw shadow
			Color shadowColor = ColorTable.BarItemPressBorderColor;
			PaintBarItemShadow( g, backRect, shadowColor, bHorizontal );
			
			// draw flash
			PaintFlashPress( g, backRect, bHorizontal );

			// draw border
			PaintButtonPressedBorder( g, rect, clEnd, false, bHorizontal );
		}

		/// <summary>
		/// Draws checked background for BarItem.
		/// </summary>
		private static void DrawBarItemBackgroundCheck( Graphics g, Rectangle rect, bool bHorizontal )
		{
			// draw background gradient
			Color clBegin = ColorTable.DropDownBarItemLightColor;
			Color clEnd = ColorTable.DropDownBarItemDarkColor;
			Rectangle backRect = GetButtonBackgroundRect( rect );
			PaintGradientSelected( g, backRect, clBegin, clEnd, bHorizontal );

			// draw shadow
			Color shadowColor = ColorTable.DropDownBarItemBorderColor;
			PaintBarItemShadow( g, backRect, shadowColor, bHorizontal );
			
			// draw flash
			PaintFlashCheck( g, backRect, bHorizontal );

			// draw border
			PaintButtonPressedBorder( g, rect, clEnd, false, bHorizontal );
		}

		/// <summary>
		/// Draws check highlighted background for BarItem.
		/// </summary>
		private static void DrawBarItemBackgroundCheckHighlight( Graphics g, Rectangle rect, bool bHorizontal )
		{
			// draw background gradient
			Color clBegin = ColorTable.BarItemCheckLightColor;
			Color clEnd = ColorTable.BarItemCheckDarkColor;
			Rectangle backRect = GetButtonBackgroundRect( rect );
			PaintGradientSelected( g, backRect, clBegin, clEnd, bHorizontal );

			// draw shadow
			Color shadowColor = ColorTable.BarItemCheckBorderColor;
			PaintBarItemShadow( g, backRect, shadowColor, bHorizontal );
			
			// draw flash
			PaintFlashCheck( g, backRect, bHorizontal );

			// draw border
			PaintButtonPressedBorder( g, rect, clEnd, false, bHorizontal );
		}

		/// <summary>
		/// Draws background for BarItem.
		/// </summary>
		private static void DrawBarItemBackground( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			switch( state )
			{
				case ItemState.Selected :
				{
					DrawBarItemBackgroundHighlight( g, rect, bHorizontal );
					break;
				}
				case ItemState.Pressed : 
				{
					DrawBarItemBackgroundPress( g, rect, bHorizontal );
					break;
				}
				case ItemState.Checked :
				{
					DrawBarItemBackgroundCheck( g, rect, bHorizontal );
					break;
				}
				case ItemState.Collapsed :
				{
					DrawBarItemBackgroundCheckHighlight( g, rect, bHorizontal );
					break;
				}
			}
		}

		#endregion

		#region Draw DropDown

		/// <summary>
		/// Draws background for DropDownBarItem.
		/// </summary>
		private static void DrawDropDownBarItemBackground( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			switch( state )
			{
				case ItemState.Selected :
				{
					DrawBarItemBackgroundHighlight( g, rect, bHorizontal );
					break;
				}
				case ItemState.Collapsed : 
				{
					DrawBarItemBackgroundDropDown( g, rect, bHorizontal );
					break;
				}
                case ItemState.Checked:
                {
                    DrawBarItemBackgroundCheck(g, rect, bHorizontal);
                    break;
                }
      
			}
		}

		
		/// <summary>
		/// Draws dropdown background for BarItem.
		/// </summary>
		private static void DrawBarItemBackgroundDropDown( Graphics g, Rectangle rect, bool bHorizontal )
		{
			// draw background gradient
			Color clBegin = ColorTable.DropDownBarItemLightColor;
			Color clEnd = ColorTable.DropDownBarItemDarkColor;
			Rectangle backRect = GetButtonBackgroundRect( rect );
			PaintGradientSelected( g, backRect, clBegin, clEnd, bHorizontal );

			// draw shadow
			Color shadowColor = ColorTable.DropDownBarItemBorderColor;
			PaintBarItemShadow( g, backRect, shadowColor, bHorizontal );
			
			// draw flash
			PaintFlashSelected( g, backRect, bHorizontal );

			// draw border
			PaintButtonPressedBorder( g, rect, clEnd, true, bHorizontal );
		}

		#endregion

		#region Draw TextBoxBarItem

		/// <summary>
		/// Draws background for TextBoxBarItem.
		/// </summary>
		private static void DrawTextBoxBarItemBackground( Graphics g, Rectangle rect, ItemState state )
		{
            using(SolidBrush brush = new SolidBrush(ColorTable.TextBarItemBackColor))
            {
                g.FillRectangle(brush, rect);
            }
		}

        /// <summary>
        /// Draws border for TextBoxBarItem.
        /// </summary>
        private static void DrawTextBoxBarItemBorder( Graphics g, Rectangle rect, ItemState state )
        {
            if (state == ItemState.Normal)
            {
                using (Pen borderPen = new Pen(ColorTable.TextBarItemBorderColor))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }
            else if (state == ItemState.Selected)
            {
                using (Pen borderPen = new Pen(ColorTable.TextBarItemBorderHighlightColor))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }
        }

		#endregion

		#region Draw ComboBoxBarItem
		/// <summary>
		/// Draws background for ComboButton of the ComboBoxBarItem.
		/// </summary>
		private static void DrawComboButtonBackground( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			switch( state )
			{
				case ItemState.Selected :
				{
					DrawButtonSelected( g, rect, bHorizontal );
					break;
				}
				case ItemState.Pressed :
				{
					DrawButtonPressed( g, rect, bHorizontal );
					break;
				}
				default : 
				{
					DrawButtonNormal( g, rect, bHorizontal );
					break;
				}
			}
		}

		/// <summary>
		/// Draws arrow for ComboButton.
		/// </summary>
		private static void PaintArrow( Graphics g, Rectangle rect )
		{
			int x = rect.X + rect.Width / 2;
			int y = rect.Y + rect.Height / 2;

			Point[] points = new Point[]
				{
					new Point( x - 2, y - 1 ), 
					new Point( x + 3, y - 1 ),
					new Point( x, y + 2 )
				};

			g.FillPolygon( SystemBrushes.ControlText, points );
		}

		/// <summary>
		/// Draws background for normal state of the ComboButton.
		/// </summary>
		private static void DrawButtonNormal( Graphics g, Rectangle rect, bool bHorizontal )
		{
			if( rect.Width > 0 && rect.Height > 0 )
			{
				Color clBegin = ColorTable.ComboButtonLightColor;
				Color clEnd = ColorTable.ComboButtonDarkColor;

				PaintGradientSelected( g, rect, clBegin, clEnd, bHorizontal );

				using( Pen pen = new Pen( ColorTable.ComboButtonBorder ) )
				{
					g.DrawRectangle( pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
				}

				PaintArrow( g, rect );
			}
		}

		/// <summary>
		/// Draws background for highlighted state of the ComboButton.
		/// </summary>
		private static void DrawButtonSelected( Graphics g, Rectangle rect, bool bHorizontal )
		{
			if( rect.Width > 0 && rect.Height > 0 )
			{
				Color clBegin = ColorTable.ComboButtonHighlightLightColor;
				Color clEnd = ColorTable.ComboButtonHighlightDarkColor;

				PaintGradientSelected( g, rect, clBegin, clEnd, bHorizontal );

				using( Pen pen = new Pen( ColorTable.ComboButtonHighlightBorder ) )
				{
					g.DrawRectangle( pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
				}

				using( Pen pen = new Pen( Color.FromArgb( 127, Color.White ) ) )
				{
					g.DrawRectangle( pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3 );
				}

				PaintArrow( g, rect );
			}
		}

		/// <summary>
		/// Draws background for pressed state of the ComboButton.
		/// </summary>
		private static void DrawButtonPressed( Graphics g, Rectangle rect, bool bHorizontal )
		{
			if (rect.Width > 0 && rect.Height > 0)
			{
				Color clBegin = ColorTable.ComboButtonPressLightColor;
				Color clEnd = ColorTable.ComboButtonPressDarkColor;
				
				PaintGradientSelected( g, rect, clBegin, clEnd, bHorizontal );

				using( Pen pen = new Pen( ColorTable.ComboButtonPressBorder ) )
				{
					g.DrawRectangle( pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1 );
				}

				using( Pen pen = new Pen( Color.FromArgb( 127, Color.White ) ) )
				{
					g.DrawRectangle( pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3 );
				}

				PaintArrow( g, rect );
			}
		}

		/// <summary>
		/// Draws background for ComboBoxBarItem.
		/// </summary>
		private static void DrawComboBoxBarItemBackground( Graphics g, Rectangle rect, ItemState state )
		{
			using( Pen pen = new Pen( ColorTable.ComboButtonBorder ) )
			{
				g.DrawRectangle( pen, rect );
			}
		}

		#endregion

		#endregion

		#region Class Static Public Methods

		/// <summary>
		/// Draws separator.
		/// </summary>
		public static void DrawSeparator( Graphics g, RectangleF rect, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			float top = rect.Y + 3.0f;
			float bottom = rect.Bottom - 5.0f;
			float right = bRTL ? ( rect.Right + 3 ) : ( rect.Left - 3 );

			using( Pen pen = new Pen( ColorTable.BarItemSeparatorColor ) )
			{
				g.DrawLine( pen, right - 1, top, right - 1, bottom );
			}

			using( Pen pen = new Pen( Color.FromArgb( 200, Color.White ) ) )
			{
				g.DrawLine( pen, right, top, right, bottom + 1 );
			}
		}

		/// <summary>
		/// Draws BarItem.
		/// </summary>
		public static void DrawBarItem( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			DrawBarItemBackground( g, rect, state, bHorizontal );
		}
	
		/// <summary>
		/// Draws DropDownBarItem.
		/// </summary>
		public static void DrawDropDownBarItem( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			DrawDropDownBarItemBackground( g, rect, state, bHorizontal );
		}
	
		/// <summary>
		/// Draws TextBoxBarItem.
		/// </summary>
		public static void DrawTextBoxBarItem( Graphics g, Rectangle rect, ItemState state, bool bHorizontal )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			DrawTextBoxBarItemBackground( g, rect, state );
            DrawTextBoxBarItemBorder( g, rect, state );
		}

		/// <summary>
		/// Draws ComboButton for ComboBoxBarItem.
		/// </summary>
		public static void DrawComboButton( Graphics g, Rectangle rect, 
			ItemState state, bool bHorizontal )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			DrawComboButtonBackground( g, rect, state, bHorizontal );
		}
		/// <summary>
		/// Draws background for ComboBoxBarItem.
		/// </summary>
		public static void DrawComboBoxBarItem( Graphics g, Rectangle rect, 
			ItemState state )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			DrawComboBoxBarItemBackground( g, rect, state );
		}
		#endregion
	}
    public class Office2010BarItemPainter
    {
        #region Class Initialize/Finalize Methods

        static Office2010BarItemPainter()
        {
            m_blButtonHorizontalShadow = new Blend();
            m_blButtonHorizontalShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
            m_blButtonHorizontalShadow.Factors = new float[] { 0.0f, 0.3f, 0.6f, 0.8f, 0.95f, 1.0f };

            m_blButtonVerticalShadow = new Blend();
            m_blButtonVerticalShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
            m_blButtonVerticalShadow.Factors = new float[] { 0.0f, 0.2f, 0.5f, 0.9f, 0.95f, 1.0f };

            m_blButtonFlash = new Blend();
            m_blButtonFlash.Positions = new float[] { 0f, 0.4f, 1f };
            m_blButtonFlash.Factors = new float[] { 0f, 0.6f, 1f };

            m_htBitmaps = new Hashtable();
        }


        #endregion

        #region Class Static Constants
        /// <summary>
        /// Radius for rounded polygon.
        /// </summary>
        private static readonly int s_iItemRadius = 1;
        /// <summary>
        /// Default size for flash image.
        /// </summary>
        private static readonly Size s_flashSize = new Size(20, 20);
        #endregion

        #region Class Static Members
        /// <summary>
        /// Blend for horizontal shadow item.
        /// </summary>
        private static Blend m_blButtonHorizontalShadow = null;
        /// <summary>
        /// Blend for vertical shadow item.
        /// </summary>
        private static Blend m_blButtonVerticalShadow = null;
        /// <summary>
        /// Blend for flash.
        /// </summary>
        private static Blend m_blButtonFlash = null;
        /// <summary>
        /// Hashtable for flash bitmaps.
        /// </summary>
        protected static Hashtable m_htBitmaps;
        /// <summary>
        /// Color table for Office2010 visual style.
        /// </summary>
        private static Office2010Colors m_colorTable = null;
        #endregion

        #region Class Static Properties

        /// <summary>
        /// Gets bitmap for checked flash.
        /// </summary>
        protected static Bitmap CheckedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebCheckedFlash] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.BarItemCheckFlashColor, s_flashSize);
                    m_htBitmaps[EBITMAP.ebCheckedFlash] = bitmap;
                }

                return bitmap;
            }
        }

        /// <summary>
        /// Gets bitmap for selected flash.
        /// </summary>
        protected static Bitmap SelectedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebSelectedFlash] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.BarItemSelectFlashColor, s_flashSize);
                    m_htBitmaps[EBITMAP.ebSelectedFlash] = bitmap;
                }

                return bitmap;
            }
        }

        /// <summary>
        /// Gets bitmap for pressed flash.
        /// </summary>
        protected static Bitmap PressedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebPressedFlash] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.BarItemPressFlashColor, s_flashSize);
                    m_htBitmaps[EBITMAP.ebPressedFlash] = bitmap;
                }

                return bitmap;
            }
        }
        /// <summary>
        /// Gets or sets color table for Office2010 visual style.
        /// </summary>
        public static Office2010Colors ColorTable
        {
            get
            {
                Office2010Colors colorTable = (m_colorTable == null) ? Office2010Colors.Default :
                    m_colorTable;

                return colorTable;
            }
            set
            {
                if (value != m_colorTable)
                {
                    m_colorTable = value;
                }
            }
        }
        #endregion

        #region Class Statc Utility Methods

        #region Utility
        /// <summary>
        /// Gets rectangle for background of the BarItem
        /// </summary>
        private static Rectangle GetButtonBackgroundRect(Rectangle rc)
        {
            Rectangle rcResult = rc;
            rcResult.Inflate(-1, -1);

            return rcResult;
        }
        /// <summary>
        /// Gets rounded polygon.
        /// </summary>
        public static Point[] GetRoundedPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point(iLeft, iTop+iRadius),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight-iRadius, iTop),
					new Point(iRight, iTop+iRadius),
					new Point(iRight, iBottom-iRadius),
					new Point(iRight-iRadius, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
			};

            return points;
        }

        /// <summary>
        /// Gets modified color.
        /// </summary>
        public static Color GetAlphaBlendedColor(Color src, Color dest, int alpha)
        {
            int R = ((src.R * alpha) + ((0xff - alpha) * dest.R)) / 0xff;
            int G = ((src.G * alpha) + ((0xff - alpha) * dest.G)) / 0xff;
            int B = ((src.B * alpha) + ((0xff - alpha) * dest.B)) / 0xff;
            int A = ((src.A * alpha) + ((0xff - alpha) * dest.A)) / 0xff;

            return Color.FromArgb(A, R, G, B);
        }

        /// <summary>
        /// Gets vertical linear gradient brush.
        /// </summary>
        protected static LinearGradientBrush GetVerticalBrush(Rectangle rc, Color cl1, Color cl2)
        {
            return GetVerticalBrush(rc.Top, rc.Height, cl1, cl2);
        }

        /// <summary>
        /// Gets vertical linear gradient brush.
        /// </summary>
        protected static LinearGradientBrush GetVerticalBrush(int top, int height, Color cl1, Color cl2)
        {
            Rectangle rcBrush = new Rectangle(0, top, 1, height);
            return new LinearGradientBrush(rcBrush, cl1, cl2, LinearGradientMode.Vertical);
        }

        /// <summary>
        /// Gets horizontal linear gradient brush.
        /// </summary>
        protected static LinearGradientBrush GetHorizontalBrush(Rectangle rc, Color cl1, Color cl2)
        {
            return GetHorizontalBrush(rc.Left, rc.Width, cl1, cl2);
        }

        /// <summary>
        /// Gets horizontal linear gradient brush.
        /// </summary>
        protected static LinearGradientBrush GetHorizontalBrush(int left, int width, Color cl1, Color cl2)
        {
            Rectangle rcBrush = new Rectangle(left - 1, 0, width + 1, 1);
            return new LinearGradientBrush(rcBrush, cl1, cl2, LinearGradientMode.Horizontal);
        }

        /// <summary>
        /// Draws background.
        /// </summary>
        private static void PaintGradientSelected(Graphics g, Rectangle rc, Color clBegin, Color clEnd,
            bool bHorizontal)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color clMedium = GetAlphaBlendedColor(clBegin, clEnd, 128);

                int topHeight = (bHorizontal) ? (int)(rc.Height * 0.38) :
                    (int)(rc.Width * 0.38);
                int bottomHeight = (bHorizontal) ? rc.Height - topHeight :
                    rc.Width - topHeight;

                // draw top ( right - for vertical alignment ) part of the gradient
                if (topHeight > 0)
                {
                    Rectangle rcTop = (bHorizontal) ? new Rectangle(rc.X, rc.Y, rc.Width, topHeight + 1) :
                        new Rectangle(rc.X + bottomHeight, rc.Y, topHeight, rc.Height);

                    using (LinearGradientBrush brush = (bHorizontal) ?
                               GetVerticalBrush(rcTop, clBegin, clMedium) :
                               GetHorizontalBrush(rcTop, clMedium, clBegin))
                    {
                        brush.WrapMode = WrapMode.TileFlipY;
                        g.FillRectangle(brush, rcTop);
                    }
                }

                // draw bottom ( left - for vertical alignment ) part of the gradient
                Rectangle rcBottom = (bHorizontal) ? new Rectangle(rc.X, rc.Y + topHeight, rc.Width, bottomHeight) :
                    new Rectangle(rc.X - 1, rc.Y, bottomHeight + 1, rc.Height);

                using (LinearGradientBrush brush = (bHorizontal) ?
                           GetVerticalBrush(rcBottom, clEnd, clMedium) :
                           GetHorizontalBrush(rcBottom, clMedium, clEnd))
                {
                    brush.WrapMode = WrapMode.TileFlipY;
                    g.FillRectangle(brush, rcBottom);
                }
            }
        }

        /// <summary>
        /// Draws outside border for pressed state.
        /// </summary>
        private static void PainButtonPressedOutsideBorder(Graphics g, Rectangle rc, Color clHighlightBorder,
            bool bDropDown, bool bHorizontal)
        {
            int iOffset = (bDropDown) ? 1 : 0;
            Color clBorderBegin = ColorTable.BarItemPressBorderColor;
            Color clBorderEnd = GetAlphaBlendedColor(clBorderBegin, Color.White, 128);

            if (bHorizontal)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(rc.Top, rc.Height, clBorderBegin, clBorderEnd))
                {
                    brush.TranslateTransform(2 * (rc.Width + 2 * rc.X),
                        rc.Height, MatrixOrder.Append);

                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point( rc.Left, rc.Bottom - 2 + iOffset ),
								new Point( rc.Left, rc.Top + 1 ),
								new Point( rc.Left + 1, rc.Top ),
								new Point( rc.Right - 2, rc.Top ),
								new Point( rc.Right - 1, rc.Top + 1 ),
								new Point( rc.Right - 1, rc.Bottom - 2 + iOffset ),
						};

                        g.DrawLines(pen, points);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = GetHorizontalBrush(rc.Left, rc.Width, clBorderEnd, clBorderBegin))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point( rc.Left + 1, rc.Top ),
								new Point( rc.Right - 2, rc.Top ),
								new Point( rc.Right - 1, rc.Top + 1 ),
								new Point( rc.Right - 1, rc.Bottom - 2 ),
								new Point( rc.Right - 2, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Bottom - 1 )
							};

                        g.DrawLines(pen, points);
                    }
                }
            }
        }

        /// <summary>
        /// Draws inside border for pressed state.
        /// </summary>
        private static void PainButtonPressedInsideBorder(Graphics g, Rectangle rc, Color clHighlightBorder,
            bool bDropDown, bool bHorizontal)
        {
            Color clHighlightBegin = Color.FromArgb(32, clHighlightBorder);
            Color clHighlightEnd = clHighlightBorder;

            if (bHorizontal)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(rc.Top + 1, rc.Height - 1,
                           clHighlightBegin, clHighlightEnd))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point( rc.Right - 2, rc.Y + 2 ),
								new Point( rc.Right - 2, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Bottom - 1 ),
								new Point( rc.Left + 1, rc.Y + 2 )
							};

                        g.DrawLines(pen, points);
                    }
                }
            }
            else
            {
                using (LinearGradientBrush brush = GetHorizontalBrush(rc.Left + 1, rc.Width - 1,
                           clHighlightEnd, clHighlightBegin))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point( rc.Right - 3, rc.Y + 1 ),
								new Point( rc.X, rc.Y + 1 ),
								new Point( rc.X, rc.Bottom - 2 ),
								new Point( rc.Right - 3, rc.Bottom - 2 )
							};

                        g.DrawLines(pen, points);
                    }
                }
            }
        }

        /// <summary>
        /// Draws border for pressed state.
        /// </summary>
        private static void PaintButtonPressedBorder(Graphics g, Rectangle rc, Color clHighlightBorder,
            bool bDropDown, bool bHorizontal)
        {
            // draw outside border
            PainButtonPressedOutsideBorder(g, rc, clHighlightBorder, bDropDown, bHorizontal);

            // draw inside border
            PainButtonPressedInsideBorder(g, rc, clHighlightBorder, bDropDown, bHorizontal);
        }

        /// <summary>
        /// Draw highlighted border.
        /// </summary>
        private static void PaintButtonSelectedBorder(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                GraphicsState gState = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen selectedPen = new Pen(ColorTable.BarItemHighlightBorderColor))
                {
                    g.DrawPolygon(selectedPen, GetRoundedPolygon(rc, s_iItemRadius));
                }
                using (Pen highlightPen = new Pen(GetAlphaBlendedColor(Color.Transparent, Color.White, 128)))
                {
                    g.DrawRectangle(highlightPen, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                }

                g.Restore(gState);
            }
        }
        /// <summary>
        /// Gets flash bitmap.
        /// </summary>
        protected static Bitmap GetFlashImage(Color colorBase, Size size)
        {
            Rectangle flashRect = new Rectangle(0, 0, size.Width, size.Height);
            Bitmap bitmap = new Bitmap(flashRect.Width, flashRect.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(flashRect);

                using (PathGradientBrush flashBrush = new PathGradientBrush(path))
                {
                    flashBrush.Blend = m_blButtonFlash;
                    flashBrush.CenterColor = colorBase;
                    flashBrush.SurroundColors = new Color[] { Color.Transparent };

                    g.FillRectangle(flashBrush, flashRect);
                }
            }

            return bitmap;
        }
        /// <summary>
        /// Draws shadow for BarItem.
        /// </summary>
        private static void PaintBarItemShadow(Graphics g, Rectangle rect, Color color, bool bHorizontal)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Color clBegin = Color.FromArgb(160, color);
                Color clEnd = Color.Transparent;
                int iOffset = (bHorizontal) ? Math.Max(rect.Height / 10, 2) :
                    Math.Max(rect.Width / 10, 2);
                Rectangle destRect = (bHorizontal) ?
                    new Rectangle(rect.X, rect.Y, rect.Width, iOffset) :
                    new Rectangle(rect.Right - iOffset, rect.Y, iOffset, rect.Height);

                LinearGradientBrush brush = (bHorizontal) ?
                    GetVerticalBrush(destRect, clBegin, clEnd) :
                    GetHorizontalBrush(destRect, clEnd, clBegin);

                if (bHorizontal)
                {
                    brush.TranslateTransform(2 * (rect.Width + 2 * rect.X),
                        destRect.Height, MatrixOrder.Append);
                }

                using (brush)
                {
                    brush.Blend = (bHorizontal) ? m_blButtonHorizontalShadow : m_blButtonVerticalShadow;
                    g.FillRectangle(brush, destRect);
                }
            }
        }


        #endregion

        #region Draw BarItem
        /// <summary>
        /// Draws flash for pressed state of the BarItem.
        /// </summary>
        private static void PaintFlashPress(Graphics g, Rectangle rect, bool bHorizontal)
        {
            g.SmoothingMode = SmoothingMode.None;
            Rectangle clipRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2) :
                new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
            g.SetClip(clipRect);

            Rectangle destRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height) :
                new Rectangle(rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height);

            g.DrawImage(PressedFlashImage, destRect);
            g.ResetClip();
        }

        /// <summary>
        /// Draws flash for selected state of the BarItem.
        /// </summary>
        private static void PaintFlashSelected(Graphics g, Rectangle rect, bool bHorizontal)
        {
            g.SmoothingMode = SmoothingMode.None;
            Rectangle clipRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2) :
                new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
            g.SetClip(clipRect);

            Rectangle destRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height) :
                new Rectangle(rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height);

            g.DrawImage(SelectedFlashImage, destRect);
            g.ResetClip();
        }

        /// <summary>
        /// Draws flash for checked state of the BarItem.
        /// </summary>
        private static void PaintFlashCheck(Graphics g, Rectangle rect, bool bHorizontal)
        {
            g.SmoothingMode = SmoothingMode.None;
            Rectangle clipRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2 + 1, rect.Width, rect.Height / 2) :
                new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height);
            g.SetClip(clipRect);

            Rectangle destRect = (bHorizontal) ?
                new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height) :
                new Rectangle(rect.X - rect.Width / 2, rect.Y, rect.Width, rect.Height);

            g.DrawImage(CheckedFlashImage, destRect);
            g.ResetClip();
        }

        /// <summary>
        /// Draws highlighted background for BarItem.
        /// </summary>
        private static void DrawBarItemBackgroundHighlight(Graphics g, Rectangle rect, bool bHorizontal)
        {
            // draw background
            Color clBegin = ColorTable.MenuItemLightColor;
            Color clEnd = ColorTable.MenuItemDarkColor;
            Rectangle backRect = GetButtonBackgroundRect(rect);
            PaintGradientSelected(g, backRect, clBegin, clEnd, bHorizontal);

            // draw flash
            PaintFlashSelected(g, backRect, bHorizontal);

            // draw border
            PaintButtonSelectedBorder(g, rect);
        }

        /// <summary>
        /// Draws pressed background for BarItem.
        /// </summary>
        private static void DrawBarItemBackgroundPress(Graphics g, Rectangle rect, bool bHorizontal)
        {
            // draw background gradient
            Color clBegin = ColorTable.BarItemPressLightColor;
            Color clEnd = ColorTable.BarItemPressDarkColor;
            Rectangle backRect = GetButtonBackgroundRect(rect);
            PaintGradientSelected(g, backRect, clBegin, clEnd, bHorizontal);

            // draw shadow
            Color shadowColor = ColorTable.BarItemPressBorderColor;
            PaintBarItemShadow(g, backRect, shadowColor, bHorizontal);

            // draw flash
            PaintFlashPress(g, backRect, bHorizontal);

            // draw border
            PaintButtonPressedBorder(g, rect, clEnd, false, bHorizontal);
        }

        /// <summary>
        /// Draws checked background for BarItem.
        /// </summary>
        private static void DrawBarItemBackgroundCheck(Graphics g, Rectangle rect, bool bHorizontal)
        {
            // draw background gradient
            Color clBegin = ColorTable.DropDownBarItemLightColor;
            Color clEnd = ColorTable.DropDownBarItemDarkColor;
            Rectangle backRect = GetButtonBackgroundRect(rect);
            PaintGradientSelected(g, backRect, clBegin, clEnd, bHorizontal);

            // draw shadow
            Color shadowColor = ColorTable.DropDownBarItemBorderColor;
            PaintBarItemShadow(g, backRect, shadowColor, bHorizontal);

            // draw flash
            PaintFlashCheck(g, backRect, bHorizontal);

            // draw border
            PaintButtonPressedBorder(g, rect, clEnd, false, bHorizontal);
        }

        /// <summary>
        /// Draws check highlighted background for BarItem.
        /// </summary>
        private static void DrawBarItemBackgroundCheckHighlight(Graphics g, Rectangle rect, bool bHorizontal)
        {
            // draw background gradient
            Color clBegin = ColorTable.BarItemCheckLightColor;
            Color clEnd = ColorTable.BarItemCheckDarkColor;
            Rectangle backRect = GetButtonBackgroundRect(rect);
            PaintGradientSelected(g, backRect, clBegin, clEnd, bHorizontal);

            // draw shadow
            Color shadowColor = ColorTable.BarItemCheckBorderColor;
            PaintBarItemShadow(g, backRect, shadowColor, bHorizontal);

            // draw flash
            PaintFlashCheck(g, backRect, bHorizontal);

            // draw border
            PaintButtonPressedBorder(g, rect, clEnd, false, bHorizontal);
        }

        /// <summary>
        /// Draws background for BarItem.
        /// </summary>
        private static void DrawBarItemBackground(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            switch (state)
            {
                case ItemState.Selected:
                    {
                        DrawBarItemBackgroundHighlight(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Pressed:
                    {
                        DrawBarItemBackgroundPress(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Checked:
                    {
                        DrawBarItemBackgroundCheck(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Collapsed:
                    {
                        DrawBarItemBackgroundCheckHighlight(g, rect, bHorizontal);
                        break;
                    }
            }
        }

        #endregion

        #region Draw DropDown

        /// <summary>
        /// Draws background for DropDownBarItem.
        /// </summary>
        private static void DrawDropDownBarItemBackground(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            switch (state)
            {
                case ItemState.Selected:
                    {
                        DrawBarItemBackgroundHighlight(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Collapsed:
                    {
                        DrawBarItemBackgroundDropDown(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Checked:
                    {
                        DrawBarItemBackgroundCheck(g, rect, bHorizontal);
                        break;
                    }

            }
        }


        /// <summary>
        /// Draws dropdown background for BarItem.
        /// </summary>
        private static void DrawBarItemBackgroundDropDown(Graphics g, Rectangle rect, bool bHorizontal)
        {
            // draw background gradient
            Color clBegin = ColorTable.DropDownBarItemLightColor;
            Color clEnd = ColorTable.DropDownBarItemDarkColor;
            Rectangle backRect = GetButtonBackgroundRect(rect);
            PaintGradientSelected(g, backRect, clBegin, clEnd, bHorizontal);

            // draw shadow
            Color shadowColor = ColorTable.DropDownBarItemBorderColor;
            PaintBarItemShadow(g, backRect, shadowColor, bHorizontal);

            // draw flash
            PaintFlashSelected(g, backRect, bHorizontal);

            // draw border
            PaintButtonPressedBorder(g, rect, clEnd, true, bHorizontal);
        }

        #endregion

        #region Draw TextBoxBarItem

        /// <summary>
        /// Draws background for TextBoxBarItem.
        /// </summary>
        private static void DrawTextBoxBarItemBackground(Graphics g, Rectangle rect, ItemState state)
        {
            using (SolidBrush brush = new SolidBrush(ColorTable.TextBarItemBackColor))
            {
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws border for TextBoxBarItem.
        /// </summary>
        private static void DrawTextBoxBarItemBorder(Graphics g, Rectangle rect, ItemState state)
        {
            if (state == ItemState.Normal)
            {
                using (Pen borderPen = new Pen(ColorTable.TextBarItemBorderColor))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }
            else if (state == ItemState.Selected)
            {
                using (Pen borderPen = new Pen(ColorTable.TextBarItemBorderHighlightColor))
                {
                    g.DrawRectangle(borderPen, rect);
                }
            }
        }

        #endregion

        #region Draw ComboBoxBarItem
        /// <summary>
        /// Draws background for ComboButton of the ComboBoxBarItem.
        /// </summary>
        private static void DrawComboButtonBackground(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            switch (state)
            {
                case ItemState.Selected:
                    {
                        DrawButtonSelected(g, rect, bHorizontal);
                        break;
                    }
                case ItemState.Pressed:
                    {
                        DrawButtonPressed(g, rect, bHorizontal);
                        break;
                    }
                default:
                    {
                        DrawButtonNormal(g, rect, bHorizontal);
                        break;
                    }
            }
        }

        /// <summary>
        /// Draws arrow for ComboButton.
        /// </summary>
        private static void PaintArrow(Graphics g, Rectangle rect)
        {
            int x = rect.X + rect.Width / 2;
            int y = rect.Y + rect.Height / 2;

            Point[] points = new Point[]
				{
					new Point( x - 2, y - 1 ), 
					new Point( x + 3, y - 1 ),
					new Point( x, y + 2 )
				};

            g.FillPolygon(SystemBrushes.ControlText, points);
        }

        /// <summary>
        /// Draws background for normal state of the ComboButton.
        /// </summary>
        private static void DrawButtonNormal(Graphics g, Rectangle rect, bool bHorizontal)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Color clBegin = ColorTable.ComboButtonLightColor;
                Color clEnd = ColorTable.ComboButtonDarkColor;

                PaintGradientSelected(g, rect, clBegin, clEnd, bHorizontal);

                using (Pen pen = new Pen(ColorTable.ComboButtonBorder))
                {
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }

                PaintArrow(g, rect);
            }
        }

        /// <summary>
        /// Draws background for highlighted state of the ComboButton.
        /// </summary>
        private static void DrawButtonSelected(Graphics g, Rectangle rect, bool bHorizontal)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Color clBegin = ColorTable.ComboButtonHighlightLightColor;
                Color clEnd = ColorTable.ComboButtonHighlightDarkColor;

                PaintGradientSelected(g, rect, clBegin, clEnd, bHorizontal);

                using (Pen pen = new Pen(ColorTable.ComboButtonHighlightBorder))
                {
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }

                using (Pen pen = new Pen(Color.FromArgb(127, Color.White)))
                {
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
                }

                PaintArrow(g, rect);
            }
        }

        /// <summary>
        /// Draws background for pressed state of the ComboButton.
        /// </summary>
        private static void DrawButtonPressed(Graphics g, Rectangle rect, bool bHorizontal)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Color clBegin = ColorTable.ComboButtonPressLightColor;
                Color clEnd = ColorTable.ComboButtonPressDarkColor;

                PaintGradientSelected(g, rect, clBegin, clEnd, bHorizontal);

                using (Pen pen = new Pen(ColorTable.ComboButtonPressBorder))
                {
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }

                using (Pen pen = new Pen(Color.FromArgb(127, Color.White)))
                {
                    g.DrawRectangle(pen, rect.X + 1, rect.Y + 1, rect.Width - 3, rect.Height - 3);
                }

                PaintArrow(g, rect);
            }
        }

        /// <summary>
        /// Draws background for ComboBoxBarItem.
        /// </summary>
        private static void DrawComboBoxBarItemBackground(Graphics g, Rectangle rect, ItemState state)
        {
            using (Pen pen = new Pen(ColorTable.ComboButtonBorder))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        #endregion

        #endregion

        #region Class Static Public Methods

        /// <summary>
        /// Draws separator.
        /// </summary>
        public static void DrawSeparator(Graphics g, RectangleF rect, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            float top = rect.Y + 3.0f;
            float bottom = rect.Bottom - 5.0f;
            float right = bRTL ? (rect.Right + 3) : (rect.Left - 3);

            using (Pen pen = new Pen(ColorTable.BarItemSeparatorColor))
            {
                g.DrawLine(pen, right - 1, top, right - 1, bottom);
            }

            using (Pen pen = new Pen(Color.FromArgb(200, Color.White)))
            {
                g.DrawLine(pen, right, top, right, bottom + 1);
            }
        }

        /// <summary>
        /// Draws BarItem.
        /// </summary>
        public static void DrawBarItem(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            DrawBarItemBackground(g, rect, state, bHorizontal);
        }

        /// <summary>
        /// Draws DropDownBarItem.
        /// </summary>
        public static void DrawDropDownBarItem(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            DrawDropDownBarItemBackground(g, rect, state, bHorizontal);
        }

        /// <summary>
        /// Draws TextBoxBarItem.
        /// </summary>
        public static void DrawTextBoxBarItem(Graphics g, Rectangle rect, ItemState state, bool bHorizontal)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            DrawTextBoxBarItemBackground(g, rect, state);
            DrawTextBoxBarItemBorder(g, rect, state);
        }

        /// <summary>
        /// Draws ComboButton for ComboBoxBarItem.
        /// </summary>
        public static void DrawComboButton(Graphics g, Rectangle rect,
            ItemState state, bool bHorizontal)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            DrawComboButtonBackground(g, rect, state, bHorizontal);
        }
        /// <summary>
        /// Draws background for ComboBoxBarItem.
        /// </summary>
        public static void DrawComboBoxBarItem(Graphics g, Rectangle rect,
            ItemState state)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            DrawComboBoxBarItemBackground(g, rect, state);
        }
        #endregion
    }
	/// <summary>
	/// Visual state of the BarItem.
	/// </summary>
	public enum ItemState
	{
		Normal = 0,
		Pressed,
		Selected,
		Checked,
		Collapsed,
		MAX			
	};

	public enum EBITMAP
	{
		ebSelectedFlash = 0,
		ebPressedFlash,
		ebCheckedFlash,
		MAX
	};
}
