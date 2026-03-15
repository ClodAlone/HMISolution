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
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// This class allows users to draw menu for Office2007 visual style.
	/// </summary>
	public class Office2007MenuPainter
	{
		#region Class Initialize/Finalize Methods

		static Office2007MenuPainter()
		{
			m_blMenuItemHighlight = new Blend();
			m_blMenuItemHighlight.Positions = new float[] { 0.0F, 0.4F, 0.5F, 1.0F };
			m_blMenuItemHighlight.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

			m_blMenuComboButtonPushed1 = new Blend();
			m_blMenuComboButtonPushed1.Positions = new float[] { 0.0F, 0.5F, 1.0F };
			m_blMenuComboButtonPushed1.Factors = new float[] { 0.0F, 0.2F, 1.0F };

			m_blMenuComboButtonPushed2 = new Blend();
			m_blMenuComboButtonPushed2.Positions = new float[] { 0.0F, 0.2F, 1.0F };
			m_blMenuComboButtonPushed2.Factors = new float[] { 0.0F, 0.4F, 1.0F };
		}

		#endregion

		#region Class Static Constants

		/// <summary>
		/// Radius for rounded polygon.
		/// </summary>
		private static readonly int s_menuItemRadius = 1;

		/// <summary>
		/// Arrow height.
		/// </summary>
		private static readonly int s_arrowHeight = 7;

		/// <summary>
		/// Arrow width.
		/// </summary>
		private static readonly int s_arrowWidth = 4;

		/// <summary>
		/// Default offset for RightToLeft drawing.
		/// </summary>
		private static readonly int s_offsetRTL = 1;

		#endregion

		#region Class Static Members

		/// <summary>
		/// Blend for highlighted item of the menu.
		/// </summary>
		private static Blend m_blMenuItemHighlight = null;

		/// <summary>
		/// Blend for top part background pushed comboButton of the menu.
		/// </summary>
		private static Blend m_blMenuComboButtonPushed1 = null;

		/// <summary>
		/// Blend for bottom part background pushed comboButton of the menu.
		/// </summary>
		private static Blend m_blMenuComboButtonPushed2 = null;

		/// <summary>
		/// Color table for Office2007 visual style.
		/// </summary>
		private static Office2007Colors m_colorTable = null;
		#endregion

		#region Class Static Properties
		/// <summary>
		/// Gets or sets color table for Office2007 visual style.
		/// </summary>
		public static Office2007Colors ColorTable
		{
			get
			{
				Office2007Colors colorTable = (m_colorTable == null) ? Office2007Colors.Default :
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

		/// <summary>
		/// Gets modified color.
		/// </summary>
		public static Color GetAlphaBlendedColor( Color src, Color dest, int alpha )
		{
			int R = ((src.R * alpha) + ((0xff - alpha) * dest.R)) / 0xff;
			int G = ((src.G * alpha) + ((0xff - alpha) * dest.G)) / 0xff;
			int B = ((src.B * alpha) + ((0xff - alpha) * dest.B)) / 0xff;
			int A = ((src.A * alpha) + ((0xff - alpha) * dest.A)) / 0xff;

			return Color.FromArgb( A, R, G, B );
		}

		/// <summary>
		/// Gets rounded polygon.
		/// </summary>
		private static Point[] GetRoundedPolygon( Rectangle rect, int radius )
		{
			int iLeft = rect.X;
			int iTop = rect.Y;
			int iRight = rect.Right;
			int iBottom = rect.Bottom;

			Point[] points = new Point[]
				{
					new Point( iLeft, iTop + radius ),
					new Point( iLeft + radius, iTop ),
					new Point( iRight - radius, iTop ),
					new Point( iRight, iTop + radius ),
					new Point( iRight, iBottom - radius ),
					new Point( iRight - radius, iBottom ),
					new Point( iLeft + radius, iBottom ),
					new Point( iLeft, iBottom - radius )
				};

			return points;
		}


		/// <summary>
		/// Gets rounded polygon for menu.
		/// </summary>
		private static Point[] GetMenuPolygon( Rectangle rect, int radius, bool bParent, bool bRTL )
		{
			int iLeft = rect.X;
			int iTop = rect.Y;
			int iRight = rect.Right;
			int iBottom = rect.Bottom;

			Point[] points = null;

			if( bParent )
			{
				if( bRTL )
				{
					points = new Point[]
					{
						new Point( iLeft, iTop + radius ),
						new Point( iLeft + radius, iTop + radius ),
						new Point( iLeft + radius, iTop ),
						
						new Point( iRight, iTop ),
						
						new Point( iRight, iBottom - radius ),
						new Point( iRight - radius, iBottom - radius ),
						new Point( iRight - radius, iBottom) ,
						
						new Point( iLeft + radius, iBottom ),
						new Point( iLeft + radius, iBottom - radius ),
						new Point( iLeft, iBottom - radius )
					};
				}
				else
				{
					points = new Point[]
					{
						new Point( iLeft, iTop ),
						new Point( iRight - radius, iTop ),
						new Point( iRight - radius, iTop + radius  ),
						new Point( iRight, iTop + radius ),
						new Point( iRight, iBottom - radius ),
						new Point( iRight - radius, iBottom - radius ),
						new Point( iRight - radius, iBottom) ,
						new Point( iLeft + radius, iBottom ),
						new Point( iLeft + radius, iBottom - radius ),
						new Point( iLeft, iBottom - radius )
					};
				}
			}
			else
			{
				points = new Point[]
				{
					new Point( iLeft, iTop + radius ),
					new Point( iLeft + radius, iTop + radius ),
					new Point( iLeft + radius, iTop ),
					new Point( iRight - radius, iTop ),
					new Point( iRight - radius, iTop + radius  ),
					new Point( iRight, iTop + radius ),
					new Point( iRight, iBottom - radius ),
					new Point( iRight - radius, iBottom - radius ),
					new Point( iRight - radius, iBottom) ,
					new Point( iLeft + radius, iBottom ),
					new Point( iLeft + radius, iBottom - radius ),
					new Point( iLeft, iBottom - radius )
				};
			}

			return points;
		}


		/// <summary>
		/// Gets rectangle for arrow.
		/// </summary>
		private static Rectangle GetArrowRect( Rectangle bounds )
		{
			int offsetY = 1;
			int offsetSize = 1;

			Rectangle arrowRect = new Rectangle( bounds.Left + (bounds.Width - s_arrowWidth) / 2,
				bounds.Top + (bounds.Height - s_arrowHeight) / 2 - offsetY,
				s_arrowWidth, s_arrowHeight + offsetSize );

			return arrowRect;
		}


		/// <summary>
		/// Gets arrow polygon.
		/// </summary>
		private static Point[] GetArrowPolygon( Rectangle rect, bool bRTL )
		{
			int senterY = rect.Top + (rect.Bottom - rect.Top) / 2;
			Point[] points = null;

			if( bRTL )
			{
				points = new Point[]
					{
						new Point( rect.Right, rect.Top ),
						new Point( rect.Right, rect.Bottom ),
						new Point( rect.Left, senterY )
					};
			}
			else
			{
				points = new Point[]
					{
						new Point( rect.Left, rect.Top ),
						new Point( rect.Left, rect.Bottom ),
						new Point( rect.Right, senterY )
					};
			}

			return points;
		}


		/// <summary>
		/// Draws arrow for ComboButton.
		/// </summary>
		private static void DrawComboButtonArrow( Graphics g, Rectangle rect )
		{
			Point[] points = ComboBoxItemRenderer.GetDropDownBorderBounds( rect );

			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );
			bool disabled = false;
			Color arrowColor = ColorTable.MenuComboButtonArrowColor;

			using( Brush brush = new SolidBrush( disabled ?
					   Color.FromArgb( 125, arrowColor ) : arrowColor ) )
			{
				Region region = new Region( path );
				g.FillRegion( brush, region );
				region.Dispose();
				path.Dispose();
			}
		}


		/// <summary>
		/// Draws arrow.
		/// </summary>
		private static void DrawMenuArrow( Graphics g, Rectangle rect,
			Color lightColor, Color darkColor, bool bRTL )
		{
			Rectangle arrowRect = GetArrowRect( rect );

			using( Brush brush = new LinearGradientBrush( arrowRect, lightColor, darkColor, LinearGradientMode.Horizontal ) )
			{
				g.FillPolygon( brush, GetArrowPolygon( arrowRect, bRTL ) );
			}
		}


		/// <summary>
		/// Draws highlight ComboButton.
		/// </summary>
		private static void DrawComboButtonHighlight( Graphics g, Rectangle rect, bool bRTL )
		{
			using( Pen darkPen = new Pen( ColorTable.MenuItemBorderColor ) )
			{
				// draw separator line
				if( bRTL )
				{
					g.DrawLine( darkPen, rect.Right + s_offsetRTL, rect.Top,
						rect.Right + s_offsetRTL, rect.Bottom );
				}
				else
				{
					g.DrawLine( darkPen, rect.Left, rect.Top, rect.Left, rect.Bottom );
				}
			}

			rect.X += 1;
			rect.Height += 1;

			using( LinearGradientBrush brush = new LinearGradientBrush( rect,
					   ColorTable.MenuItemLightColor,
					   ColorTable.MenuItemDarkColor,
					   LinearGradientMode.Vertical ) )
			{
				// draw background
				brush.Blend = m_blMenuItemHighlight;
				g.FillRectangle( brush, rect );
			}
		}


		/// <summary>
		/// Draws ComboButton when ComboBox is highlighted.
		/// </summary>
		private static void DrawComboButtonSelected( Graphics g, Rectangle rect, bool bRTL )
		{
			using( Pen pen = new Pen( ColorTable.MenuTextBoxBorderColor ) )
			{
				if( bRTL )
				{
					g.DrawLine( pen, rect.Right + s_offsetRTL, rect.Top,
						rect.Right + s_offsetRTL, rect.Bottom );
				}
				else
				{
					g.DrawLine( pen, rect.Left, rect.Top, rect.Left, rect.Bottom );
				}
			}

			rect.X += 1;
			rect.Height += 1;

			using( LinearGradientBrush brush = new LinearGradientBrush( rect,
					   ColorTable.MenuComboButtonHighlightLightColor,
					   ColorTable.MenuComboButtonHighlightDarkColor,
					   LinearGradientMode.Vertical ) )
			{
				brush.Blend = m_blMenuItemHighlight;
				g.FillRectangle( brush, rect );
			}
		}


		/// <summary>
		/// Draws ComboButton when ComboBox popup menu is visible.
		/// </summary>
		private static void DrawComboButtonPopup( Graphics g, Rectangle rect, bool bRTL )
		{
			using( Pen darkPen = new Pen( ColorTable.MenuItemBorderColor ) )
			{
				if( bRTL )
				{
					g.DrawLine( darkPen, rect.Right + s_offsetRTL, rect.Top,
						rect.Right + s_offsetRTL, rect.Bottom );
				}
				else
				{
					g.DrawLine( darkPen, rect.Left, rect.Top, rect.Left, rect.Bottom );
				}
			}

			rect.X += 1;
			rect.Height += 1;
			Rectangle topRect = new Rectangle( rect.X, rect.Y, rect.Width, rect.Height / 2 );

			using( LinearGradientBrush brush1 = new LinearGradientBrush( topRect,
					   ColorTable.MenuComboButtonPushed1Color,
					   ColorTable.MenuComboButtonPushed2Color,
					   LinearGradientMode.Vertical ) )
			{
				brush1.Blend = m_blMenuComboButtonPushed1;
				g.FillRectangle( brush1, topRect );
			}

			Rectangle bottomRect = new Rectangle( rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2 );

			using( LinearGradientBrush brush2 = new LinearGradientBrush( bottomRect,
					   ColorTable.MenuComboButtonPushed3Color,
					   ColorTable.MenuComboButtonPushed4Color,
					   LinearGradientMode.Vertical ) )
			{
				brush2.Blend = m_blMenuComboButtonPushed2;
				g.FillRectangle( brush2, bottomRect );
			}
		}

		#endregion

		#region Class Static Public Methods

		/// <summary>
		/// Draws TextBox item of the menu.
		/// </summary>
		public static void DrawMenuTextBoxItem( Graphics g, Rectangle rect, bool selected )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			rect.Inflate( -1, -1 );
			rect.Width -= 1;
			rect.Height -= 1;

			Color backColor = (selected) ? Color.White : ColorTable.MenuTextBoxBackColor;

			using( Brush fillBrush = new SolidBrush( backColor ) )
			{
				g.FillRectangle( fillBrush, rect );
			}

			using( Pen borderPen = new Pen( ColorTable.MenuTextBoxBorderColor ) )
			{
				g.DrawRectangle( borderPen, rect );
			}
		}


		/// <summary>
		/// Draws ComboButton for ComboBox of the menu.
		/// </summary>
		public static void DrawComboButton( Graphics g, Rectangle rect, ButtonState buttonState,
			bool bDroppedDown, bool bSelected, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			Rectangle bounds = rect;
			bounds.Inflate( 1, 1 );
			bounds.Width -= 1;
			bounds.Height -= 1;
			bool drawBorder = true;

			if( bDroppedDown ) // draw button when ComboBox Popup menu is visible.
			{
				DrawComboButtonPopup( g, bounds, bRTL );
			}
			else if( bSelected && buttonState != ButtonState.Normal ) // draw button when ComboBox is highlighted
			{
				DrawComboButtonSelected( g, bounds, bRTL );
				drawBorder = false;
			}
			else if( buttonState == ButtonState.Flat ) // braw simple button
			{
				drawBorder = false;
			}
			else if( buttonState == ButtonState.Normal )  // draw highlight button
			{
				DrawComboButtonHighlight( g, bounds, bRTL );
			}

			if( drawBorder ) // draw border
			{
				Color borderColor = GetAlphaBlendedColor( Color.Transparent, Color.White, 128 );

				using( Pen lightPen = new Pen( borderColor ) )
				{
					bounds.X += 1;
					bounds.Width -= 1;
					g.DrawRectangle( lightPen, bounds );
				}
			}

			// draw arrow
			DrawComboButtonArrow( g, rect );
		}


		/// <summary>
		/// Draws check mark for menu.
		/// </summary>
		public static void DrawMenuChecked( Graphics g, Rectangle rect, bool bDrawCheckmark )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			Rectangle bounds = rect;
			bounds.Inflate( -1, -1 );
			bounds.Width -= 2;
			bounds.Height -= 1;

			using( Brush fillBrush = new SolidBrush( ColorTable.MenuCheckedFillColor ) )
			{
				// draw background
				g.FillRectangle( fillBrush, bounds );
			}

			SmoothingMode saveSmoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			using( Pen borderPen = new Pen( ColorTable.MenuCheckedBorderColor ) )
			{
				// draw border
				g.DrawPolygon( borderPen, GetRoundedPolygon( bounds, s_menuItemRadius ) );
			}

			if( bDrawCheckmark )
			{
				// calculate location of the check mark
				float left1 = ((float)bounds.Width / 4f + (float)bounds.Width / 10f);
				float left2 = left1 + ((float)bounds.Width / 10f);
				float left3 = left2 + ((float)bounds.Width / 5f);

				left1 = (int)left1;
				left3 = (float)Math.Round( left3 );

				float top1 = ((float)bounds.Height / 3f + (float)bounds.Height / 5f);
				float top2 = top1 + ((float)bounds.Height / 4f);
				float top3 = top2 - ((float)bounds.Height / 2f);

				top1 = (int)top1;
				top2 = (float)Math.Round( top2 );
				top3 = (int)top3;

				int thickness = (int)((float)bounds.Width / 10f);

				using( Pen checkedPen = new Pen( ColorTable.MenuCheckedColor ) )
				{
					// draw check mark
					for( int i = 0; i < thickness; i++ )
					{
						g.DrawLine( checkedPen, bounds.Left + left1, bounds.Top + top1 + i, bounds.Left + left2, bounds.Top + top2 + i );
						g.DrawLine( checkedPen, bounds.Left + left2, bounds.Top + top2 + i, bounds.Left + left3, bounds.Top + top3 + i );
					}
				}
			}

			g.SmoothingMode = saveSmoothingMode;
		}


		/// <summary>
		/// Draws background for highlighted item of the menu.
		/// </summary>
		public static void DrawMenuItemHighlightBackground( Graphics g, Rectangle rect )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			using( LinearGradientBrush brush = new LinearGradientBrush( rect,
					   ColorTable.MenuItemLightColor, ColorTable.MenuItemDarkColor,
					   LinearGradientMode.Vertical ) )
			{
				brush.Blend = m_blMenuItemHighlight;
				g.FillRectangle( brush, rect );
			}
		}


		/// <summary>
		/// Draws border for highlighted item of the menu.
		/// </summary>
		public static void DrawMenuItemHighlightBorder( Graphics g, Rectangle rect )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			g.DrawLine( Pens.White, rect.X, rect.Y, rect.Right, rect.Y );
			g.DrawLine( Pens.White, rect.X, rect.Bottom, rect.Right, rect.Bottom );

			using( Pen darkPen = new Pen( ColorTable.MenuItemBorderColor ) )
			{
				// draw outside border
				g.DrawPolygon( darkPen, GetRoundedPolygon( rect, s_menuItemRadius ) );
			}

			Color borderColor = GetAlphaBlendedColor( Color.Transparent, Color.White, 128 );

			using( Pen lightPen = new Pen( borderColor ) )
			{
				// draw inside border
				rect.Inflate( -1, -1 );
				g.DrawRectangle( lightPen, rect );
			}
		}


		/// <summary>
		/// Draws border for menu.
		/// </summary>
		public static void DrawMenuBorder( Graphics g, Rectangle rect, bool bParent, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			SmoothingMode saveSmoothingMode = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			using( Pen outsidePen = new Pen( ColorTable.MenuBorderColor ) )
			{
				// draw outside border
				g.DrawPolygon( outsidePen, GetMenuPolygon( rect, s_menuItemRadius, bParent, bRTL ) );
			}

			Color lightColor = GetAlphaBlendedColor( ColorTable.MenuColumnColor, Color.White, 128 );

			using( Pen insidePen = new Pen( lightColor ) )
			{
				// draw inside border
				rect.Inflate( -1, -1 );
				g.DrawPolygon( insidePen, GetMenuPolygon( rect, s_menuItemRadius, bParent, bRTL ) );
			}

			g.SmoothingMode = saveSmoothingMode;
		}


		/// <summary>
		/// Draws column background of the menu.
		/// </summary>
		public static void DrawMenuColumn( Graphics g, Rectangle rect, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			using( Brush backBrush = new SolidBrush( ColorTable.MenuColumnColor ) )
			{
				// draw background
				g.FillRectangle( backBrush, rect );
			}

			using( Pen darkPen = new Pen( ColorTable.MenuColumnSeparatorColor ) )
			{
				// draw dark line
				if( bRTL )
				{
					g.DrawLine( darkPen, rect.Left + 1, rect.Top, rect.Left + 1, rect.Bottom );
				}
				else
				{
					g.DrawLine( darkPen, rect.Right - 2, rect.Top, rect.Right - 2, rect.Bottom );
				}
			}

			Color lightColor = GetAlphaBlendedColor( Color.Transparent, Color.White, 128 );

			using( Pen lightPen = new Pen( lightColor ) )
			{
				// draw ligth line
				if( bRTL )
				{
					g.DrawLine( lightPen, rect.Left, rect.Top, rect.Left, rect.Bottom );
				}
				else
				{
					g.DrawLine( lightPen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom );
				}
			}
		}


		/// <summary>
		/// Draws button for DropDownBarItem of the menu.
		/// </summary>
		public static void DrawMenuDropDownBarItem( Graphics g, Rectangle rect, bool selected, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			int offsetX = 0;

			if( bRTL )
			{
				offsetX = (selected) ? -1 : 1;
			}
			else
			{
				offsetX = (selected) ? 1 : -1;
			}

			int offsetY = (selected) ? 0 : 4;

			// draw dark separator lines
			using( Pen darkPen = new Pen( ColorTable.MenuItemBorderColor ) )
			{
				if( bRTL )
				{
					g.DrawLine( darkPen, rect.Right - s_offsetRTL, rect.Top + offsetY,
						rect.Right - s_offsetRTL, rect.Bottom - offsetY );
				}
				else
				{
					g.DrawLine( darkPen, rect.Left, rect.Top + offsetY,
						rect.Left, rect.Bottom - offsetY );
				}
			}

			// draw light separator lines
			Color lightColor = GetAlphaBlendedColor( Color.Transparent, Color.White, 128 );

			using( Pen lightPen = new Pen( lightColor ) )
			{
				if( bRTL )
				{
					g.DrawLine( lightPen, rect.Right - s_offsetRTL + offsetX, rect.Top + offsetY,
						rect.Right - s_offsetRTL + offsetX, rect.Bottom - offsetY );
				}
				else
				{
					g.DrawLine( lightPen, rect.Left + offsetX, rect.Top + offsetY,
						rect.Left + offsetX, rect.Bottom - offsetY );
				}
			}

			// draw arrow
			DrawMenuArrow( g, rect, ColorTable.MenuItemArrowLightColor,
				ColorTable.MenuItemArrowDarkColor, bRTL );
		}


		/// <summary>
		/// Draws button for parentBarItem of the menu.
		/// </summary>
		public static void DrawMenuParentBarItem( Graphics g, Rectangle rect, bool bRTL )
		{
			if( g == null || rect == Rectangle.Empty )
				throw new NullReferenceException();

			// draw arrow
			DrawMenuArrow( g, rect, ColorTable.MenuItemArrowLightColor,
				ColorTable.MenuItemArrowDarkColor, bRTL );
		}

		/// <summary>
		/// Gets rounded region for menu.
		/// </summary>
		public static Region GetMenuRegion( Rectangle rect, bool bParent, bool bRTL )
		{
			if( bRTL )
			{
				rect.X -= 1;
				rect.Width += 1;
			}

			int iRadius = s_menuItemRadius;
			Point[] points = GetMenuPolygon( rect, iRadius, bParent, bRTL );
			GraphicsPath path = new GraphicsPath();
			path.AddLines( points );
			Region region = new Region( path );
            path.Dispose();
			return region;
		}

		#endregion
	}

    /// <summary>
    /// This class allows users to draw menu for Office2010 visual style.
    /// </summary>
    public class Office2010MenuPainter
    {
        #region Class Initialize/Finalize Methods

        static Office2010MenuPainter()
        {
            m_blMenuItemHighlight = new Blend();
            m_blMenuItemHighlight.Positions = new float[] { 0.0F, 0.4F, 0.5F, 1.0F };
            m_blMenuItemHighlight.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blMenuComboButtonPushed1 = new Blend();
            m_blMenuComboButtonPushed1.Positions = new float[] { 0.0F, 0.5F, 1.0F };
            m_blMenuComboButtonPushed1.Factors = new float[] { 0.0F, 0.2F, 1.0F };

            m_blMenuComboButtonPushed2 = new Blend();
            m_blMenuComboButtonPushed2.Positions = new float[] { 0.0F, 0.2F, 1.0F };
            m_blMenuComboButtonPushed2.Factors = new float[] { 0.0F, 0.4F, 1.0F };
        }

        #endregion

        #region Class Static Constants

        /// <summary>
        /// Radius for rounded polygon.
        /// </summary>
        private static readonly int s_menuItemRadius = 1;

        /// <summary>
        /// Arrow height.
        /// </summary>
        private static readonly int s_arrowHeight = 7;

        /// <summary>
        /// Arrow width.
        /// </summary>
        private static readonly int s_arrowWidth = 4;

        /// <summary>
        /// Default offset for RightToLeft drawing.
        /// </summary>
        private static readonly int s_offsetRTL = 1;

        #endregion

        #region Class Static Members

        /// <summary>
        /// Blend for highlighted item of the menu.
        /// </summary>
        private static Blend m_blMenuItemHighlight = null;

        /// <summary>
        /// Blend for top part background pushed comboButton of the menu.
        /// </summary>
        private static Blend m_blMenuComboButtonPushed1 = null;

        /// <summary>
        /// Blend for bottom part background pushed comboButton of the menu.
        /// </summary>
        private static Blend m_blMenuComboButtonPushed2 = null;

        /// <summary>
        /// Color table for Office2010 visual style.
        /// </summary>
        private static Office2010Colors m_colorTable = null;
        #endregion

        #region Class Static Properties
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
        /// Gets rounded polygon.
        /// </summary>
        private static Point[] GetRoundedPolygon(Rectangle rect, int radius)
        {
            int iLeft = rect.X;
            int iTop = rect.Y;
            int iRight = rect.Right;
            int iBottom = rect.Bottom;

            Point[] points = new Point[]
				{
					new Point( iLeft, iTop + radius ),
					new Point( iLeft + radius, iTop ),
					new Point( iRight - radius, iTop ),
					new Point( iRight, iTop + radius ),
					new Point( iRight, iBottom - radius ),
					new Point( iRight - radius, iBottom ),
					new Point( iLeft + radius, iBottom ),
					new Point( iLeft, iBottom - radius )
				};

            return points;
        }


        /// <summary>
        /// Gets rounded polygon for menu.
        /// </summary>
        private static Point[] GetMenuPolygon(Rectangle rect, int radius, bool bParent, bool bRTL)
        {
            int iLeft = rect.X;
            int iTop = rect.Y;
            int iRight = rect.Right;
            int iBottom = rect.Bottom;

            Point[] points = null;

            if (bParent)
            {
                if (bRTL)
                {
                    points = new Point[]
					{
						new Point( iLeft, iTop + radius ),
						new Point( iLeft + radius, iTop + radius ),
						new Point( iLeft + radius, iTop ),
						
						new Point( iRight, iTop ),
						
						new Point( iRight, iBottom - radius ),
						new Point( iRight - radius, iBottom - radius ),
						new Point( iRight - radius, iBottom) ,
						
						new Point( iLeft + radius, iBottom ),
						new Point( iLeft + radius, iBottom - radius ),
						new Point( iLeft, iBottom - radius )
					};
                }
                else
                {
                    points = new Point[]
					{
						new Point( iLeft, iTop ),
						new Point( iRight - radius, iTop ),
						new Point( iRight - radius, iTop + radius  ),
						new Point( iRight, iTop + radius ),
						new Point( iRight, iBottom - radius ),
						new Point( iRight - radius, iBottom - radius ),
						new Point( iRight - radius, iBottom) ,
						new Point( iLeft + radius, iBottom ),
						new Point( iLeft + radius, iBottom - radius ),
						new Point( iLeft, iBottom - radius )
					};
                }
            }
            else
            {
                points = new Point[]
				{
					new Point( iLeft, iTop + radius ),
					new Point( iLeft + radius, iTop + radius ),
					new Point( iLeft + radius, iTop ),
					new Point( iRight - radius, iTop ),
					new Point( iRight - radius, iTop + radius  ),
					new Point( iRight, iTop + radius ),
					new Point( iRight, iBottom - radius ),
					new Point( iRight - radius, iBottom - radius ),
					new Point( iRight - radius, iBottom) ,
					new Point( iLeft + radius, iBottom ),
					new Point( iLeft + radius, iBottom - radius ),
					new Point( iLeft, iBottom - radius )
				};
            }

            return points;
        }


        /// <summary>
        /// Gets rectangle for arrow.
        /// </summary>
        private static Rectangle GetArrowRect(Rectangle bounds)
        {
            int offsetY = 1;
            int offsetSize = 1;

            Rectangle arrowRect = new Rectangle(bounds.Left + (bounds.Width - s_arrowWidth) / 2,
                bounds.Top + (bounds.Height - s_arrowHeight) / 2 - offsetY,
                s_arrowWidth, s_arrowHeight + offsetSize);

            return arrowRect;
        }


        /// <summary>
        /// Gets arrow polygon.
        /// </summary>
        private static Point[] GetArrowPolygon(Rectangle rect, bool bRTL)
        {
            int senterY = rect.Top + (rect.Bottom - rect.Top) / 2;
            Point[] points = null;

            if (bRTL)
            {
                points = new Point[]
					{
						new Point( rect.Right, rect.Top ),
						new Point( rect.Right, rect.Bottom ),
						new Point( rect.Left, senterY )
					};
            }
            else
            {
                points = new Point[]
					{
						new Point( rect.Left, rect.Top ),
						new Point( rect.Left, rect.Bottom ),
						new Point( rect.Right, senterY )
					};
            }

            return points;
        }


        /// <summary>
        /// Draws arrow for ComboButton.
        /// </summary>
        private static void DrawComboButtonArrow(Graphics g, Rectangle rect)
        {
            Point[] points = ComboBoxItemRenderer.GetDropDownBorderBounds(rect);

            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            bool disabled = false;
            Color arrowColor = ColorTable.MenuComboButtonArrowColor;

            using (Brush brush = new SolidBrush(disabled ?
                       Color.FromArgb(125, arrowColor) : arrowColor))
            {
                Region region = new Region(path);
                g.FillRegion(brush, region);
                region.Dispose();
                path.Dispose();
            }
        }


        /// <summary>
        /// Draws arrow.
        /// </summary>
        private static void DrawMenuArrow(Graphics g, Rectangle rect,
            Color lightColor, Color darkColor, bool bRTL)
        {
            Rectangle arrowRect = GetArrowRect(rect);

            using (Brush brush = new LinearGradientBrush(arrowRect, lightColor, darkColor, LinearGradientMode.Horizontal))
            {
                g.FillPolygon(brush, GetArrowPolygon(arrowRect, bRTL));
            }
        }


        /// <summary>
        /// Draws highlight ComboButton.
        /// </summary>
        private static void DrawComboButtonHighlight(Graphics g, Rectangle rect, bool bRTL)
        {
            using (Pen darkPen = new Pen(ColorTable.MenuItemBorderColor))
            {
                // draw separator line
                if (bRTL)
                {
                    g.DrawLine(darkPen, rect.Right + s_offsetRTL, rect.Top,
                        rect.Right + s_offsetRTL, rect.Bottom);
                }
                else
                {
                    g.DrawLine(darkPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                }
            }

            rect.X += 1;
            rect.Height += 1;

            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                       ColorTable.MenuItemLightColor,
                       ColorTable.MenuItemDarkColor,
                       LinearGradientMode.Vertical))
            {
                // draw background
                brush.Blend = m_blMenuItemHighlight;
                g.FillRectangle(brush, rect);
            }
        }


        /// <summary>
        /// Draws ComboButton when ComboBox is highlighted.
        /// </summary>
        private static void DrawComboButtonSelected(Graphics g, Rectangle rect, bool bRTL)
        {
            using (Pen pen = new Pen(ColorTable.MenuTextBoxBorderColor))
            {
                if (bRTL)
                {
                    g.DrawLine(pen, rect.Right + s_offsetRTL, rect.Top,
                        rect.Right + s_offsetRTL, rect.Bottom);
                }
                else
                {
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                }
            }

            rect.X += 1;
            rect.Height += 1;

            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                       ColorTable.MenuComboButtonHighlightLightColor,
                       ColorTable.MenuComboButtonHighlightDarkColor,
                       LinearGradientMode.Vertical))
            {
                brush.Blend = m_blMenuItemHighlight;
                g.FillRectangle(brush, rect);
            }
        }


        /// <summary>
        /// Draws ComboButton when ComboBox popup menu is visible.
        /// </summary>
        private static void DrawComboButtonPopup(Graphics g, Rectangle rect, bool bRTL)
        {
            using (Pen darkPen = new Pen(ColorTable.MenuItemBorderColor))
            {
                if (bRTL)
                {
                    g.DrawLine(darkPen, rect.Right + s_offsetRTL, rect.Top,
                        rect.Right + s_offsetRTL, rect.Bottom);
                }
                else
                {
                    g.DrawLine(darkPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                }
            }

            rect.X += 1;
            rect.Height += 1;
            Rectangle topRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);

            using (LinearGradientBrush brush1 = new LinearGradientBrush(topRect,
                       ColorTable.MenuComboButtonPushed1Color,
                       ColorTable.MenuComboButtonPushed2Color,
                       LinearGradientMode.Vertical))
            {
                brush1.Blend = m_blMenuComboButtonPushed1;
                g.FillRectangle(brush1, topRect);
            }

            Rectangle bottomRect = new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2);

            using (LinearGradientBrush brush2 = new LinearGradientBrush(bottomRect,
                       ColorTable.MenuComboButtonPushed3Color,
                       ColorTable.MenuComboButtonPushed4Color,
                       LinearGradientMode.Vertical))
            {
                brush2.Blend = m_blMenuComboButtonPushed2;
                g.FillRectangle(brush2, bottomRect);
            }
        }

        #endregion

        #region Class Static Public Methods

        /// <summary>
        /// Draws TextBox item of the menu.
        /// </summary>
        public static void DrawMenuTextBoxItem(Graphics g, Rectangle rect, bool selected)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            rect.Inflate(-1, -1);
            rect.Width -= 1;
            rect.Height -= 1;

            Color backColor = (selected) ? Color.White : ColorTable.MenuTextBoxBackColor;

            using (Brush fillBrush = new SolidBrush(backColor))
            {
                g.FillRectangle(fillBrush, rect);
            }

            using (Pen borderPen = new Pen(ColorTable.MenuTextBoxBorderColor))
            {
                g.DrawRectangle(borderPen, rect);
            }
        }


        /// <summary>
        /// Draws ComboButton for ComboBox of the menu.
        /// </summary>
        public static void DrawComboButton(Graphics g, Rectangle rect, ButtonState buttonState,
            bool bDroppedDown, bool bSelected, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            Rectangle bounds = rect;
            bounds.Inflate(1, 1);
            bounds.Width -= 1;
            bounds.Height -= 1;
            bool drawBorder = true;

            if (bDroppedDown) // draw button when ComboBox Popup menu is visible.
            {
                DrawComboButtonPopup(g, bounds, bRTL);
            }
            else if (bSelected && buttonState != ButtonState.Normal) // draw button when ComboBox is highlighted
            {
                DrawComboButtonSelected(g, bounds, bRTL);
                drawBorder = false;
            }
            else if (buttonState == ButtonState.Flat) // braw simple button
            {
                drawBorder = false;
            }
            else if (buttonState == ButtonState.Normal)  // draw highlight button
            {
                DrawComboButtonHighlight(g, bounds, bRTL);
            }

            if (drawBorder) // draw border
            {
                Color borderColor = GetAlphaBlendedColor(Color.Transparent, Color.White, 128);

                using (Pen lightPen = new Pen(borderColor))
                {
                    bounds.X += 1;
                    bounds.Width -= 1;
                    g.DrawRectangle(lightPen, bounds);
                }
            }

            // draw arrow
            DrawComboButtonArrow(g, rect);
        }


        /// <summary>
        /// Draws check mark for menu.
        /// </summary>
        public static void DrawMenuChecked(Graphics g, Rectangle rect, bool bDrawCheckmark)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            Rectangle bounds = rect;
            bounds.Inflate(-1, -1);
            bounds.Width -= 2;
            bounds.Height -= 1;

            using (Brush fillBrush = new SolidBrush(ColorTable.MenuCheckedFillColor))
            {
                // draw background
                g.FillRectangle(fillBrush, bounds);
            }

            SmoothingMode saveSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen borderPen = new Pen(ColorTable.MenuCheckedBorderColor))
            {
                // draw border
                g.DrawPolygon(borderPen, GetRoundedPolygon(bounds, s_menuItemRadius));
            }

            if (bDrawCheckmark)
            {
                // calculate location of the check mark
                float left1 = ((float)bounds.Width / 4f + (float)bounds.Width / 10f);
                float left2 = left1 + ((float)bounds.Width / 10f);
                float left3 = left2 + ((float)bounds.Width / 5f);

                left1 = (int)left1;
                left3 = (float)Math.Round(left3);

                float top1 = ((float)bounds.Height / 3f + (float)bounds.Height / 5f);
                float top2 = top1 + ((float)bounds.Height / 4f);
                float top3 = top2 - ((float)bounds.Height / 2f);

                top1 = (int)top1;
                top2 = (float)Math.Round(top2);
                top3 = (int)top3;

                int thickness = (int)((float)bounds.Width / 10f);

                using (Pen checkedPen = new Pen(ColorTable.MenuCheckedColor))
                {
                    // draw check mark
                    for (int i = 0; i < thickness; i++)
                    {
                        g.DrawLine(checkedPen, bounds.Left + left1, bounds.Top + top1 + i, bounds.Left + left2, bounds.Top + top2 + i);
                        g.DrawLine(checkedPen, bounds.Left + left2, bounds.Top + top2 + i, bounds.Left + left3, bounds.Top + top3 + i);
                    }
                }
            }

            g.SmoothingMode = saveSmoothingMode;
        }


        /// <summary>
        /// Draws background for highlighted item of the menu.
        /// </summary>
        public static void DrawMenuItemHighlightBackground(Graphics g, Rectangle rect)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                       ColorTable.MenuItemLightColor, ColorTable.MenuItemDarkColor,
                       LinearGradientMode.Vertical))
            {
                brush.Blend = m_blMenuItemHighlight;
                g.FillRectangle(brush, rect);
            }
        }


        /// <summary>
        /// Draws border for highlighted item of the menu.
        /// </summary>
        public static void DrawMenuItemHighlightBorder(Graphics g, Rectangle rect)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            g.DrawLine(Pens.White, rect.X, rect.Y, rect.Right, rect.Y);
            g.DrawLine(Pens.White, rect.X, rect.Bottom, rect.Right, rect.Bottom);

            using (Pen darkPen = new Pen(ColorTable.MenuItemBorderColor))
            {
                // draw outside border
                g.DrawPolygon(darkPen, GetRoundedPolygon(rect, s_menuItemRadius));
            }

            Color borderColor = GetAlphaBlendedColor(Color.Transparent, Color.White, 128);

            using (Pen lightPen = new Pen(borderColor))
            {
                // draw inside border
                rect.Inflate(-1, -1);
                g.DrawRectangle(lightPen, rect);
            }
        }


        /// <summary>
        /// Draws border for menu.
        /// </summary>
        public static void DrawMenuBorder(Graphics g, Rectangle rect, bool bParent, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            SmoothingMode saveSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen outsidePen = new Pen(ColorTable.MenuBorderColor))
            {
                // draw outside border
                g.DrawPolygon(outsidePen, GetMenuPolygon(rect, s_menuItemRadius, bParent, bRTL));
            }

            Color lightColor = GetAlphaBlendedColor(ColorTable.MenuColumnColor, Color.White, 128);

            using (Pen insidePen = new Pen(lightColor))
            {
                // draw inside border
                rect.Inflate(-1, -1);
                g.DrawPolygon(insidePen, GetMenuPolygon(rect, s_menuItemRadius, bParent, bRTL));
            }

            g.SmoothingMode = saveSmoothingMode;
        }


        /// <summary>
        /// Draws column background of the menu.
        /// </summary>
        public static void DrawMenuColumn(Graphics g, Rectangle rect, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            using (Brush backBrush = new SolidBrush(ColorTable.MenuColumnColor))
            {
                // draw background
                g.FillRectangle(backBrush, rect);
            }

            using (Pen darkPen = new Pen(ColorTable.MenuColumnSeparatorColor))
            {
                // draw dark line
                if (bRTL)
                {
                    g.DrawLine(darkPen, rect.Left + 1, rect.Top, rect.Left + 1, rect.Bottom);
                }
                else
                {
                    g.DrawLine(darkPen, rect.Right - 2, rect.Top, rect.Right - 2, rect.Bottom);
                }
            }

            Color lightColor = GetAlphaBlendedColor(Color.Transparent, Color.White, 128);

            using (Pen lightPen = new Pen(lightColor))
            {
                // draw ligth line
                if (bRTL)
                {
                    g.DrawLine(lightPen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                }
                else
                {
                    g.DrawLine(lightPen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                }
            }
        }


        /// <summary>
        /// Draws button for DropDownBarItem of the menu.
        /// </summary>
        public static void DrawMenuDropDownBarItem(Graphics g, Rectangle rect, bool selected, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            int offsetX = 0;

            if (bRTL)
            {
                offsetX = (selected) ? -1 : 1;
            }
            else
            {
                offsetX = (selected) ? 1 : -1;
            }

            int offsetY = (selected) ? 0 : 4;

            // draw dark separator lines
            using (Pen darkPen = new Pen(ColorTable.MenuItemBorderColor))
            {
                if (bRTL)
                {
                    g.DrawLine(darkPen, rect.Right - s_offsetRTL, rect.Top + offsetY,
                        rect.Right - s_offsetRTL, rect.Bottom - offsetY);
                }
                else
                {
                    g.DrawLine(darkPen, rect.Left, rect.Top + offsetY,
                        rect.Left, rect.Bottom - offsetY);
                }
            }

            // draw light separator lines
            Color lightColor = GetAlphaBlendedColor(Color.Transparent, Color.White, 128);

            using (Pen lightPen = new Pen(lightColor))
            {
                if (bRTL)
                {
                    g.DrawLine(lightPen, rect.Right - s_offsetRTL + offsetX, rect.Top + offsetY,
                        rect.Right - s_offsetRTL + offsetX, rect.Bottom - offsetY);
                }
                else
                {
                    g.DrawLine(lightPen, rect.Left + offsetX, rect.Top + offsetY,
                        rect.Left + offsetX, rect.Bottom - offsetY);
                }
            }

            // draw arrow
            DrawMenuArrow(g, rect, ColorTable.MenuItemArrowLightColor,
                ColorTable.MenuItemArrowDarkColor, bRTL);
        }


        /// <summary>
        /// Draws button for parentBarItem of the menu.
        /// </summary>
        public static void DrawMenuParentBarItem(Graphics g, Rectangle rect, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            // draw arrow
            DrawMenuArrow(g, rect, ColorTable.MenuItemArrowLightColor,
                ColorTable.MenuItemArrowDarkColor, bRTL);
        }

        /// <summary>
        /// Gets rounded region for menu.
        /// </summary>
        public static Region GetMenuRegion(Rectangle rect, bool bParent, bool bRTL)
        {
            if (bRTL)
            {
                rect.X -= 1;
                rect.Width += 1;
            }

            int iRadius = s_menuItemRadius;
            Point[] points = GetMenuPolygon(rect, iRadius, bParent, bRTL);
            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            Region region = new Region(path);
            path.Dispose();
            return region;
        }

        #endregion
    }
    public class MetroMenuPainter 
    {
        private static Color metroColor;
        private static Color metroBackColor = Color.White;
        private static readonly int s_arrowWidth = 4;
        private static readonly int s_arrowHeight = 7;
        public static void GetMetroColor(Color metrocolor)
        {
            metroColor = metrocolor;
           
        }
        /// <summary>
        /// To Set ParentBarItem BackColor
        /// </summary>
        /// <param name="BackColor">ParentBarItem BackColor</param>
        internal static void GetBarItemBackColor(Color BackColor)
        {
            metroBackColor = BackColor;
        }

        public static void DrawMenuParentBarItem(Graphics g, Rectangle rect, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            // draw arrow
            DrawMenuArrow(g, rect, metroColor, bRTL);
        }

        public static void DrawMenuItemHighlightBackground(Graphics g, Rectangle rect)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            SolidBrush brush = new SolidBrush(metroColor);
            g.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public static void DrawMetroMenuColumn(Graphics g, Rectangle rect, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            using (Brush backBrush = new SolidBrush(metroBackColor))
            {
                // draw background
                g.FillRectangle(backBrush, rect);
                Rectangle rc = rect;
            }
        }
        public static void DrawMenuTextBoxItem(Graphics g, Rectangle rect, bool selected)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            rect.Inflate(-1, -1);
            rect.Width -= 1;
            rect.Height -= 1;

            SolidBrush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, rect);
            Pen pen = new Pen(Color.LightGray);
            g.DrawRectangle(pen, rect);
            brush.Dispose();
            pen.Dispose();
            
        }
        public static void DrawComboButton(Graphics g, Rectangle rect, ButtonState buttonState,
            bool bDroppedDown, bool bSelected, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();
            Rectangle bounds = rect;
            bounds.Inflate(1, 1);
            bounds.Width -= 1;
            bounds.Height -= 1;
            bool drawBorder = true;
            if (bDroppedDown) // draw button when ComboBox Popup menu is visible.
            {
                DrawComboButtonPopup(g, bounds, bRTL);
                DrawComboButtonArrow(g, rect , Color.Black);
            }
            else if (bSelected && buttonState != ButtonState.Normal) // draw button when ComboBox is highlighted
            {
                DrawComboButtonSelected(g, bounds, bRTL);
                drawBorder = false;
                DrawComboButtonArrow(g, rect , Color.White);
            }
            else if (buttonState == ButtonState.Inactive)
            {
                DrawComboButtonSelected(g, bounds, bRTL);
                drawBorder = false;
                DrawComboButtonArrow(g, rect, Color.White);
            }
            else if (buttonState == ButtonState.Flat) // braw simple button
            {
                DrawComboButtonArrow(g, rect, Color.Gray);
                drawBorder = true;
            }
            else if (buttonState == ButtonState.Normal)  // draw highlight button
            {
                DrawComboButtonHighlight(g, bounds, bRTL);
                DrawComboButtonArrow(g, rect, Color.Gray);
                drawBorder = false;
            }

            if (drawBorder) // draw border
            {
                Pen pen = new Pen(Color.LightGray);
                bounds.X += 1;
                bounds.Width -= 1;
                g.DrawRectangle(pen, bounds);
                pen.Dispose();
            }

        }
        private static void DrawComboButtonPopup(Graphics g, Rectangle rect, bool bRTL)
        {
            rect.X += 2;
            rect.Height += 1;
            Rectangle topRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
            SolidBrush brush1 = new SolidBrush(Color.LightGray);
            g.FillRectangle(brush1, topRect);
            Rectangle bottomRect = new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height / 2);
            SolidBrush brush2 = new SolidBrush(Color.LightGray);
            g.FillRectangle(brush2, bottomRect);
            brush1.Dispose();
            brush2.Dispose();
        }
        private static void DrawComboButtonSelected(Graphics g, Rectangle rect, bool bRTL)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb(70, metroColor));
            g.FillRectangle(brush, rect);
            brush.Dispose();
        }
        private static void DrawComboButtonHighlight(Graphics g, Rectangle rect, bool bRTL)
        {
                // draw background
                SolidBrush brush = new SolidBrush(Color.LightGray);
                g.FillRectangle(brush, rect);
                brush.Dispose();
            
        }
        private static void DrawComboButtonArrow(Graphics g, Rectangle rect , Color color)
        {
            Point[] points = ComboBoxItemRenderer.GetDropDownBorderBounds(rect);
            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            bool disabled = false;
            Color arrowColor = color;
            using (Brush brush = new SolidBrush(disabled ?
                       Color.FromArgb(125, arrowColor) : arrowColor))
            {
                Region region = new Region(path);
                g.FillRegion(brush, region);
                region.Dispose();
                path.Dispose();
            }
        }
        private static void DrawComboPussedButtonHighlight(Graphics g, Rectangle rect, bool bRTL)
        {
            rect.X += 1;
            rect.Height += 1;
            SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
            g.FillRectangle(brush, rect);
            brush.Dispose();
            
        }
        public static void DrawMenuBorder(Graphics g, Rectangle rect, bool bParent, bool bRTL)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();
            Pen pen = new Pen(Color.FromArgb(80 , Color.LightBlue));
            rect.Inflate(-1, -1);
            rect.Width -= 1;
            rect.Height-= 1;
            g.DrawRectangle(pen, rect);
            pen.Dispose();
        }
        public static void DrawMenuChecked(Graphics g, Rectangle rect, bool bDrawCheckmark)
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            Rectangle bounds = rect;
            bounds.Inflate(-1, -1);
            bounds.Width -= 2;
            bounds.Height -= 1;
            SmoothingMode saveSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (bDrawCheckmark)
            {
                // calculate location of the check mark
                float left1 = ((float)bounds.Width / 4f + (float)bounds.Width / 10f);
                float left2 = left1 + ((float)bounds.Width / 10f);
                float left3 = left2 + ((float)bounds.Width / 5f);

                left1 = (int)left1;
                left3 = (float)Math.Round(left3);

                float top1 = ((float)bounds.Height / 3f + (float)bounds.Height / 5f);
                float top2 = top1 + ((float)bounds.Height / 4f);
                float top3 = top2 - ((float)bounds.Height / 2f);

                top1 = (int)top1;
                top2 = (float)Math.Round(top2);
                top3 = (int)top3;

                int thickness = (int)((float)bounds.Width / 10f);

                using (Pen checkedPen = new Pen(Color.Black))
                {
                    // draw check mark
                    for (int i = 0; i < thickness; i++)
                    {
                            g.DrawLine(checkedPen, bounds.Left + left1 +1, bounds.Top + top1 + i, bounds.Left + left2, bounds.Top + top2 + i);
                            g.DrawLine(checkedPen, bounds.Left + left2, bounds.Top + top2 + i, bounds.Left + left3, bounds.Top + top3 + i);

                    }

                    g.SmoothingMode = saveSmoothingMode;
                }
            }
        }
        public static void DrawMenuDropDownBarItem(Graphics g, Rectangle rect, bool selected, bool bRTL )
        {
            if (g == null || rect == Rectangle.Empty)
                throw new NullReferenceException();

            int offsetX = 0;

            if (bRTL)
            {
                offsetX = (selected) ? -1 : 1;
            }
            else
            {
                offsetX = (selected) ? 1 : -1;
            }

            int offsetY = (selected) ? 0 : 4;

            // draw dark separator lines
            using (Pen darkPen = new Pen(Color.LightBlue))
            {
                if (!bRTL)
                {
                    g.DrawLine(darkPen, rect.Left, rect.Top + offsetY,
                        rect.Left, rect.Bottom - offsetY);
                }
            }

            DrawMenuArrow(g, rect, Color.Black, bRTL);
        }
        private static void DrawMenuArrow(Graphics g, Rectangle rect,
            Color metroColor, bool bRTL)
        {
            Rectangle arrowRect = GetArrowRect(rect);
            using(SolidBrush brush = new SolidBrush(Color.Black))
            g.FillPolygon(brush, GetArrowPolygon(arrowRect, bRTL));
        }
        private static Rectangle GetArrowRect(Rectangle bounds)
        {
            int offsetY = 1;
            int offsetSize = 1;

            Rectangle arrowRect = new Rectangle(bounds.Left + (bounds.Width - s_arrowWidth) / 2,
                bounds.Top + (bounds.Height - s_arrowHeight) / 2 - offsetY,
                s_arrowWidth, s_arrowHeight + offsetSize);

            return arrowRect;
        }
        private static Point[] GetArrowPolygon(Rectangle rect, bool bRTL)
        {
            int senterY = rect.Top + (rect.Bottom - rect.Top) / 2;
            Point[] points = null;

            if (bRTL)
            {
                points = new Point[]
					{
						new Point( rect.Right, rect.Top ),
						new Point( rect.Right, rect.Bottom ),
						new Point( rect.Left, senterY )
					};
            }
            else
            {
                points = new Point[]
					{
						new Point( rect.Left, rect.Top ),
						new Point( rect.Left, rect.Bottom ),
						new Point( rect.Right, senterY )
					};
            }

            return points;
        }
    }
}
