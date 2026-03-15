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

using Syncfusion.Windows.Forms.Renderers;
#endregion

namespace Syncfusion.Windows.Forms.Scrolling.Renderers
{
	/// <summary></summary>
	public class WindowsXPRenderer : ClassicRenderer
	{
		#region Class constants
		/// <summary>
		/// Angle for horizontal gradient brush.
		/// </summary>
		protected const float DEF_HORIZONTAL_BRUSH_ANGLE = 90F;
		/// <summary>
		/// Angle for vertical gradient brush.
		/// </summary>
		protected const float DEF_VERTICAL_BRUSH_ANGLE = 0F;
		/// <summary>
		/// Angle for vertical gradient brush.
		/// </summary>
		protected const float DEF_SLOPING_BRUSH_ANGLE = 53F;
		/// <summary>
		/// Width for brush.
		/// </summary>
		protected const int DEF_WIDTH_BRUSH = 1;
		/// <summary>
		/// Height for brush.
		/// </summary>
		protected const int DEF_HEIGHT_BRUSH = 1;
		/// <summary>
		/// Default radius truncation corners.
		/// </summary>
		protected const int DEF_BORDERS_RADIUS = 1;
		/// <summary>
		/// Count of lines on the thumb.
		/// </summary>
		protected const int DEF_COUNT_OF_LINES = 8;
		/// <summary>
		/// Height of lines on the thumb.
		/// </summary>
		protected const int DEF_HEIGHT_OF_LINES = 5;
		#endregion

		#region Class members
		/// <summary></summary>
		protected float m_iLeft;
		/// <summary></summary>
		protected float m_iTop;
		/// <summary></summary>
		protected float m_iRight;
		/// <summary></summary>
		protected float m_iBottom;
		/// <summary></summary>
		private Color m_disabledColor = Color.FromArgb( 230, 230, 220 );
		/// <summary></summary>
		private Color m_disabledShadowColor = Color.FromArgb( 196, 196, 175 );
		/// <summary></summary>
		private Color m_disabledArrowColor = Color.FromArgb( 201, 201, 194 );
		/// <summary></summary>
		private Color m_disabledBackgroundColor = Color.FromArgb( 254, 254, 251 );
		/// <summary>
		/// The color scheme that the renderer will render. 
		/// </summary>
		private WindowsXPColorsScheme colorScheme = WindowsXPColorsScheme.DefaultBlue;
		/// <summary></summary>
		private WindowsXPRenderer renderer = null;
		#endregion

		#region Class properties
		/// <summary>
		///  The color scheme that the renderer will render. 
		/// </summary>
		public WindowsXPColorsScheme ColorScheme
		{
			get
			{
				return this.colorScheme;
			}

			set
			{
				if( this.colorScheme != value )
				{
					this.colorScheme = value;
				}
			}
		}
		#endregion

		#region class initialize\finalize methods
		/// <summary>
		/// Initialize new instance of WindowsXPRenderer
		/// </summary>
		/// <param name="isVerticalScrollBar"></param>
		protected internal WindowsXPRenderer( bool isVerticalScrollBar )
			: base( isVerticalScrollBar )
		{
		}

		/// <summary>
		/// Initialize new instance of WindowsXPRenderer
		/// </summary>
		/// <param name="parent"></param>
		public WindowsXPRenderer( ScrollBarCustomDraw parent )
			: base( parent )
		{
			SetColorScheme();

			bool isVerticalScrollBar = false;
			if( parent is VScrollBarCustomDraw )
			{
				isVerticalScrollBar = true;
			}

			switch( ColorScheme )
			{
				case WindowsXPColorsScheme.DefaultBlue:
					renderer = new WindowsXPBlueRenderer( isVerticalScrollBar );
					break;

				case WindowsXPColorsScheme.OliveGreen:
					renderer = new WindowsXPOliveGreenRenderer( isVerticalScrollBar );
					break;

				case WindowsXPColorsScheme.Silver:
					renderer = new WindowsXPSilverRenderer( isVerticalScrollBar );
					break;
			}
		}
		#endregion

		#region class overrides
		/// <summary>
		/// Draws arrow button of scroll. If theme is disabled than draw classic scroll. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="type"></param>
		/// <param name="state"></param>
		public override void DrawArrowButton( Graphics g, Rectangle bounds, ScrollButton type, ButtonState state )
		{
			if( !m_parent.ThemeEnabled )
			{
				base.DrawArrowButton( g, bounds, type, state );
			}
			else
			{
				if( state == ButtonState.Inactive )
				{
					DrawArrowButtonDisabled( g, bounds, type );
				}
				else
				{
					renderer.DrawArrowButton( g, bounds, type, state );
				}
			}
		}

		/// <summary>
		/// Draws background of scroll. If theme is disabled than draw classic scroll. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"></param>
		public override void DrawBackground( Graphics g, Rectangle bounds, ButtonState state )
		{
			if( !m_parent.ThemeEnabled )
			{
				base.DrawBackground( g, bounds, state );
			}
			else
			{
				if( state == ButtonState.Inactive )
				{
					DrawBackgroundDisabled( g, bounds );
				}
				else
				{
					renderer.DrawBackground( g, bounds, state );
				}
			}

		}

		/// <summary>
		/// Draws thumb for scroll. If theme is disabled than draw classic scroll. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="state"></param>
		public override void DrawThumb( Graphics g, Rectangle bounds, ButtonState state )
		{
			if( !m_parent.ThemeEnabled )
			{
				base.DrawThumb( g, bounds, state );
			}
			else
			{
				if( state == ButtonState.Inactive )
				{
					DrawThumbDisabled( g, bounds );
				}
				else
				{
					renderer.DrawThumb( g, bounds, state );
				}
			}
		}
		#endregion

		#region Class utility methods
		/// <summary>
		/// Sets WindowsXP color scheme for the control.
		/// </summary>
		/// <param name="colorScheme"/>
		public void SetColorScheme( WindowsXPColorsScheme colorScheme )
		{
			ColorScheme = colorScheme;
		}

		/// <summary>
		/// Sets the color scheme for the button based on the current XP Scheme.
		/// </summary>
		public void SetColorScheme()
		{
			if( XPThemes.IsSilverThemeOn )
			{
				ColorScheme = WindowsXPColorsScheme.Silver;
			}
			else if( XPThemes.IsOliveGreenThemeOn )
			{
				ColorScheme = WindowsXPColorsScheme.OliveGreen;
			}
			else
			{
				ColorScheme = WindowsXPColorsScheme.DefaultBlue;
			}
		}

		/// <summary>
		/// Initializes edges of specified rectangle.
		/// </summary>
		/// <param name="bounds">Bounds of the rectangle.</param>
		protected void InitializeRectangleEdges( RectangleF bounds )
		{
			m_iLeft = bounds.X;
			m_iTop = bounds.Y;
			m_iRight = bounds.Right - 1;
			m_iBottom = bounds.Bottom - 1;
		}

		/// <summary></summary>
		/// <param name="rc"></param>
		protected void ValidateRectangle( ref RectangleF rc )
		{
			if( rc.Width <= 0 )
			{
				rc.Width = 1;
			}
			if( rc.Height <= 0 )
			{
				rc.Height = 1;
			}
		}

		/// <summary>
		///  Gets rounded path with specified radius for the rectangle.
		/// </summary>
		/// <param name="bounds"></param>
		/// <param name="iRadius"></param>
		/// <returns></returns>
		protected GraphicsPath GetRoundedPath( RectangleF bounds, int iRadius )
		{
			ValidateRectangle( ref bounds );
			InitializeRectangleEdges( bounds );

			GraphicsPath path = new GraphicsPath();

			path.AddLine( m_iLeft, m_iBottom - iRadius, m_iLeft, m_iTop + iRadius );
			path.AddLine( m_iLeft, m_iTop + iRadius, m_iLeft + iRadius, m_iTop );
			path.AddLine( m_iLeft + iRadius, m_iTop, m_iRight - iRadius, m_iTop );
			path.AddLine( m_iRight - iRadius, m_iTop, m_iRight, m_iTop + iRadius );
			path.AddLine( m_iRight, m_iTop + iRadius, m_iRight, m_iBottom - iRadius );
			path.AddLine( m_iRight, m_iBottom - iRadius, m_iRight - iRadius, m_iBottom );
			path.AddLine( m_iRight - iRadius, m_iBottom, m_iLeft + iRadius, m_iBottom );
			path.AddLine( m_iLeft + iRadius, m_iBottom, m_iLeft, m_iBottom - iRadius );

			return path;
		}

		/// <summary>
		/// Gets vertical gradient brush.
		/// </summary>
		/// <returns></returns>
		/// <param name="rc"/>
		/// <param name="cl1"/>
		/// <param name="cl2"/>
		protected LinearGradientBrush GetVerticalBrush( RectangleF rc, Color cl1, Color cl2 )
		{
			ValidateRectangle( ref rc );

			RectangleF rcBrush = new RectangleF( rc.Left, rc.Top, rc.Width, DEF_HEIGHT_BRUSH );
			return new LinearGradientBrush( rcBrush, cl1, cl2, DEF_VERTICAL_BRUSH_ANGLE );
		}

		/// <summary>
		/// Gets horizontal gradient brush.
		/// </summary>
		/// <returns></returns>
		/// <param name="rc"/>
		/// <param name="cl1"/>
		/// <param name="cl2"/>
		protected LinearGradientBrush GetHorizontalBrush( RectangleF rc, Color cl1, Color cl2 )
		{
			ValidateRectangle( ref rc );

			RectangleF rcBrush = new RectangleF( rc.Left, rc.Top, DEF_WIDTH_BRUSH, rc.Height );
			return new LinearGradientBrush( rcBrush, cl1, cl2, DEF_HORIZONTAL_BRUSH_ANGLE );
		}

		/// <summary>
		/// Gets gradient brush with 53 angle.
		/// </summary>
		/// <returns></returns>
		/// <param name="rc"/>
		/// <param name="cl1"/>
		/// <param name="cl2"/>
		protected LinearGradientBrush GetSlopingBrush( RectangleF rc, Color cl1, Color cl2 )
		{
			ValidateRectangle( ref rc );

			return new LinearGradientBrush( rc, cl1, cl2, DEF_SLOPING_BRUSH_ANGLE );
		}

		/// <summary>
		/// Draws background lines.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the background.</param>
		/// <param name="lineColor">Color of the lines.</param>
		protected void DrawBackgroundLines( Graphics g, Rectangle bounds, Color lineColor )
		{
			using( Pen borderPen = new Pen( lineColor ) )
			{
				if( IsVerticalScrollBar )
				{
					g.DrawLine( borderPen, bounds.X, bounds.Y, bounds.X, bounds.Y + bounds.Height - 1 );
					g.DrawLine( borderPen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1 );
				}
				else
				{
					g.DrawLine( borderPen, bounds.X, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y );
					g.DrawLine( borderPen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1 );
				}
			}
		}

		/// <summary>
		/// Draws shadow for the arrow button.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		/// <param name="shadowColor"/>
		/// <param name="startBackgroundColor"/>
		/// <param name="endBackgroundColor"/>
		protected void DrawShadowForArrowButton( Graphics g, RectangleF bounds, Color shadowColor, Color startBackgroundColor, Color endBackgroundColor )
		{
			int iRadius1 = DEF_BORDERS_RADIUS;
			int iRadius2 = 2;

			Blend shadowBlend = new Blend();
			shadowBlend = new Blend();
			shadowBlend.Positions = new float[ ]{0.0F, 0.2F, 0.8F, 1.0F};
			shadowBlend.Factors = new float[ ]{0.0F, 2.0F, 0.0F, 1.0F};

			bounds = new RectangleF( bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 1 );

			InitializeRectangleEdges( bounds );

			if( IsVerticalScrollBar )
			{
				using( LinearGradientBrush bgBrush = GetVerticalBrush( bounds, startBackgroundColor, endBackgroundColor ) )
				{
                    using (Pen pen = new Pen( bgBrush ))
                        g.DrawLine(pen, m_iRight - 1, m_iBottom, m_iLeft, m_iBottom);
				}
			}
			else
			{
				using( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, startBackgroundColor, startBackgroundColor ) )
				{
                    using (Pen pen = new Pen( bgBrush ))
                    g.DrawLine(pen, m_iRight, m_iTop, m_iRight, m_iBottom - 1);
				}
			}

			GraphicsPath path = new GraphicsPath();

			path.AddLine( m_iRight, m_iTop + iRadius1, m_iRight, m_iBottom - iRadius2 );
			path.AddLine( m_iRight, m_iBottom - iRadius2, m_iRight - iRadius2, m_iBottom );
			path.AddLine( m_iRight - iRadius2, m_iBottom, m_iLeft + iRadius2, m_iBottom );
			path.AddLine( m_iLeft + iRadius1, m_iBottom, m_iLeft, m_iBottom - iRadius1 );

			using( LinearGradientBrush bgBrush = GetSlopingBrush( bounds, shadowColor, Color.FromArgb( 125, shadowColor ) ) )
			{
				bgBrush.Blend = shadowBlend;
                using (Pen pen = new Pen(bgBrush))
                    g.DrawPath(pen, path);
			}
            path.Dispose();
		}

		/// <summary>
		/// Draws shadow for the thumb.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		/// <param name="shadowColor"/>
		protected void DrawShadowForThumb( Graphics g, RectangleF bounds, Color shadowColor )
		{
			InitializeRectangleEdges( bounds );

			int radius1 = DEF_BORDERS_RADIUS;
			int radius2 = 2;

			Pen shadowPen1 = new Pen( shadowColor );
			Pen shadowPen2 = new Pen( Color.FromArgb( 200, shadowColor ) );
			Pen shadowPen3 = new Pen( Color.FromArgb( 225, shadowColor ) );

			if( IsVerticalScrollBar )
			{
				g.DrawLine( shadowPen1, m_iRight - radius2 - radius1, m_iBottom, m_iLeft + radius2 + radius1, m_iBottom );
				g.DrawLine( shadowPen2, m_iRight, m_iTop + radius2, m_iRight, m_iBottom - radius2 );
				g.DrawLine( shadowPen2, m_iRight, m_iTop + radius2 + radius1, m_iRight, m_iBottom - radius2 - radius1 );
				g.DrawLine( shadowPen3, m_iLeft + radius2, m_iBottom, m_iLeft + radius1, m_iBottom - radius1 );
				g.DrawLine( shadowPen3, m_iRight - radius1, m_iBottom - radius1, m_iRight - radius2, m_iBottom );
			}
			else
			{
				g.DrawLine( shadowPen1, m_iRight, m_iTop + radius2 + radius1, m_iRight, m_iBottom - radius2 - radius1 );
				g.DrawLine( shadowPen2, m_iRight - radius2, m_iBottom, m_iLeft + radius2, m_iBottom );
				g.DrawLine( shadowPen2, m_iRight - radius2 - radius1, m_iBottom, m_iLeft + radius2 + radius1, m_iBottom );
				g.DrawLine( shadowPen3, m_iRight - radius1, m_iTop + radius1, m_iRight, m_iTop + radius2 );
				g.DrawLine( shadowPen3, m_iRight, m_iBottom - radius2, m_iRight - radius1, m_iBottom - radius1 );
			}
            shadowPen1.Dispose();
            shadowPen2.Dispose();
            shadowPen3.Dispose();
		}

		/// <summary>
		/// Draws middle lines on the thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		/// <param name="firstColor"/>
		/// <param name="secondColor"/>
		protected void DrawMiddleLinesForThumb( Graphics g, RectangleF bounds, Color firstColor, Color secondColor )
		{
			float posX;
			float posY;
			Pen whitePen = new Pen( firstColor );
			Pen bluePen = new Pen( secondColor );

			if( IsVerticalScrollBar )
			{
				if( bounds.Height > 17 && bounds.Width > 8 )
				{
					posY = bounds.Y + ( bounds.Height - DEF_COUNT_OF_LINES ) / 2;
					posX = bounds.X + ( float )Math.Floor( ( bounds.Width - ( DEF_HEIGHT_OF_LINES + 1 ) ) / 2 );

					g.DrawLine( whitePen, posX, posY, posX + DEF_HEIGHT_OF_LINES, posY );
					g.DrawLine( bluePen, posX + 1, posY + 1, posX + 1 + DEF_HEIGHT_OF_LINES, posY + 1 );
					g.DrawLine( whitePen, posX, posY + 2, posX + DEF_HEIGHT_OF_LINES, posY + 2 );
					g.DrawLine( bluePen, posX + 1, posY + 3, posX + 1 + DEF_HEIGHT_OF_LINES, posY + 3 );
					g.DrawLine( whitePen, posX, posY + 4, posX + DEF_HEIGHT_OF_LINES, posY + 4 );
					g.DrawLine( bluePen, posX + 1, posY + 5, posX + 1 + DEF_HEIGHT_OF_LINES, posY + 5 );
					g.DrawLine( whitePen, posX, posY + 6, posX + DEF_HEIGHT_OF_LINES, posY + 6 );
					g.DrawLine( bluePen, posX + 1, posY + 7, posX + 1 + DEF_HEIGHT_OF_LINES, posY + 7 );
				}
			}
			else
			{
				if( bounds.Width > 17 && bounds.Height > 8 )
				{
					posX = bounds.X + ( bounds.Width - DEF_COUNT_OF_LINES ) / 2;
					posY = bounds.Y + ( float )Math.Floor( ( bounds.Height - ( DEF_HEIGHT_OF_LINES + 1 ) ) / 2 );

					g.DrawLine( whitePen, posX, posY, posX, posY + DEF_HEIGHT_OF_LINES );
					g.DrawLine( bluePen, posX + 1, posY + 1, posX + 1, posY + 1 + DEF_HEIGHT_OF_LINES );
					g.DrawLine( whitePen, posX + 2, posY, posX + 2, posY + DEF_HEIGHT_OF_LINES );
					g.DrawLine( bluePen, posX + 3, posY + 1, posX + 3, posY + 1 + DEF_HEIGHT_OF_LINES );
					g.DrawLine( whitePen, posX + 4, posY, posX + 4, posY + DEF_HEIGHT_OF_LINES );
					g.DrawLine( bluePen, posX + 5, posY + 1, posX + 5, posY + 1 + DEF_HEIGHT_OF_LINES );
					g.DrawLine( whitePen, posX + 6, posY, posX + 6, posY + DEF_HEIGHT_OF_LINES );
					g.DrawLine( bluePen, posX + 7, posY + 1, posX + 7, posY + 1 + DEF_HEIGHT_OF_LINES );
				}
			}
            whitePen.Dispose();
            bluePen.Dispose();
		}

		/// <summary>
		/// Fill Rectangle with ControlLightLight color.
		/// </summary>
		/// <param name="g">Graphics objects to use.</param>
		/// <param name="bounds">Bounds of the rectangle.</param>
		private void FillRectangleWhite( Graphics g, Rectangle bounds )
		{
			using( SolidBrush brush = new SolidBrush( SystemColors.ControlLightLight ) )
			{
				g.FillRectangle( brush, bounds );
			}
		}

		/// <summary>
		/// Draws disabled arrowButton.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the button.</param>
		/// <param name="type">Type of the button.</param>
		private void DrawArrowButtonDisabled( Graphics g, Rectangle bounds, ScrollButton type )
		{
			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				FillRectangleWhite( g, bounds );

				DrawBackgroundLines( g, bounds, Color.FromArgb( 175, m_disabledColor ) );

				Rectangle rc;
				if( IsVerticalScrollBar )
				{
					rc = new Rectangle( bounds.X, bounds.Y - 1, bounds.Width, bounds.Height + 1 );
				}
				else
				{
					rc = new Rectangle( bounds.X - 1, bounds.Y, bounds.Width + 1, bounds.Height );
				}

				DrawShadowForArrowButton( g, rc, m_disabledShadowColor, Color.FromArgb( 100, m_disabledColor ),
					Color.FromArgb( 100, m_disabledColor ) );

				rc.Inflate( -2, -2 );
                using (Pen pen = new Pen(Color.FromArgb(100, m_disabledColor)))
                    g.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1);

				using( GraphicsPath path = GetRoundedPath( rc, DEF_BORDERS_RADIUS ) )
				{
                    using (Pen pen = new Pen(m_disabledColor))
                        g.DrawPath(pen, path);
				}

				rc.Inflate( -1, -1 );
				using( LinearGradientBrush bgBrush = GetSlopingBrush( rc, Color.FromArgb( 100, m_disabledColor ),
								 Color.FromArgb( 240, m_disabledColor ) ) )
				{
					g.FillRectangle( bgBrush, rc );
				}

				DrawArrows( g, bounds, type, m_disabledArrowColor );

			}
		}

		/// <summary>
		/// Draws disabled background.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the button.</param>
		private void DrawBackgroundDisabled( Graphics g, Rectangle bounds )
		{
			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				FillRectangleWhite( g, bounds );

				DrawBackgroundLines( g, bounds, Color.FromArgb( 175, m_disabledColor ) );

				using( SolidBrush brush = new SolidBrush( m_disabledBackgroundColor ) )
				{
					if( IsVerticalScrollBar )
					{
						g.FillRectangle( brush, new Rectangle( bounds.Left + 1, bounds.Top, bounds.Width - 2, bounds.Height ) );
					}
					else
					{
						g.FillRectangle( brush, new Rectangle( bounds.Left, bounds.Top + 1, bounds.Width, bounds.Height - 2 ) );
					}
				}
			}
		}

		/// <summary>
		/// Draws disabled thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawThumbDisabled( Graphics g, Rectangle bounds )
		{
			DrawBackgroundDisabled( g, bounds );
		}

		/// <summary>
		/// Draws up arrow on the button.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the button.</param>
		/// <param name="type"/>
		/// <param name="color"/>
		protected void DrawArrows( Graphics g, Rectangle bounds, ScrollButton type, Color color )
		{
			int smallValue = ( bounds.Height > bounds.Width ) ? bounds.Width : bounds.Height;
			//int largeValue = ( bounds.Height > bounds.Width ) ? bounds.Height : bounds.Width;

			float oddWidthShift = 1;
			float oddHeightShift = 1;

			if( ( float )( bounds.Width ) / 2 - ( float )Math.Floor( ( float )( bounds.Width ) / 2 ) == 0.5F ) //odd width
			{
				oddWidthShift = 0;
			}
			if( ( float )( bounds.Height ) / 2 - ( float )Math.Floor( ( float )( bounds.Height ) / 2 ) == 0.5F ) //odd height
			{
				oddHeightShift = 0;
			}

			float shiftLine = 1;
			if( smallValue <= 14 )
			{
				shiftLine = 2;
			}

			float distHeight1 = ( float )Math.Floor( ( smallValue ) / 3.0F ); // 1/3 
			float distHeight2 = ( float )Math.Floor( ( smallValue ) * 2 / 3.0F ); // 2/3 

			float lengthOfFirstLine = ( float )Math.Abs( distHeight1 - distHeight2 + 1 );
			if( lengthOfFirstLine > 4 && smallValue < 28 )
			{
				lengthOfFirstLine = 4;
			}
			if( lengthOfFirstLine == 0 )
			{
				lengthOfFirstLine = 1;
			}

			float lengthOfSecondLine = lengthOfFirstLine - 1;
			float lengthOfThirdLine = lengthOfFirstLine - shiftLine;
			if( lengthOfThirdLine < 0 )
			{
				lengthOfThirdLine = 0;
			}

			float posX = bounds.X + ( float )Math.Floor( bounds.Width / 2.0F ) - oddWidthShift; //
			float posY = bounds.Y + ( float )Math.Floor( ( bounds.Height - lengthOfFirstLine ) / 2.0F ); //position of first point

			float centerX = posX;
			float centerY = bounds.Y + ( float )Math.Floor( bounds.Height / 2.0F );

			float fDegree = 0;

			switch( type )
			{
				case ScrollButton.Up:
					fDegree = 0;
					posY -= 1;
					break;

				case ScrollButton.Right:
					fDegree = 90;
					break;

				case ScrollButton.Down:
					fDegree = 180;
					posY += oddHeightShift;
					break;

				case ScrollButton.Left:
					fDegree = 270;
					posY -= 1;
					break;
			}

			PointF p1 = new PointF( posX, posY );
			PointF p2 = new PointF( p1.X - lengthOfFirstLine, p1.Y + lengthOfFirstLine );
			PointF p3 = new PointF( p1.X + lengthOfFirstLine, p1.Y + lengthOfFirstLine );
			PointF p4 = new PointF( p1.X, p1.Y + 1 );
			PointF p5 = new PointF( p4.X - lengthOfSecondLine, p4.Y + lengthOfSecondLine );
			PointF p6 = new PointF( p4.X + lengthOfSecondLine, p4.Y + lengthOfSecondLine );
			PointF p7 = new PointF( p1.X, p1.Y + 2 );
			PointF p8 = new PointF( p7.X - lengthOfThirdLine, p7.Y + lengthOfThirdLine );
			PointF p9 = new PointF( p7.X + lengthOfThirdLine, p7.Y + lengthOfThirdLine );

			PointF[ ] ps = new PointF[ ]{p1, p2, p3, p4, p5, p6, p7, p8, p9};

			float fRadianDegree = ( float )( fDegree / 180 * Math.PI );

			PointF centerPoint = new PointF( centerX, centerY );

			Rotate( ps, fRadianDegree, centerPoint );

			using( Pen pen = new Pen( color ) )
			{
				DrawArrowInternal( g, pen, ps[ 0 ], ps[ 1 ], ps[ 2 ] );
				DrawArrowInternal( g, pen, ps[ 3 ], ps[ 4 ], ps[ 5 ] );
				DrawArrowInternal( g, pen, ps[ 6 ], ps[ 7 ], ps[ 8 ] );
			}
		}

		/// <summary>
		/// Draws two lines in specified points.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="pen"></param>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		protected void DrawArrowInternal( Graphics g, Pen pen, PointF p1, PointF p2, PointF p3 )
		{
			g.DrawLine( pen, p1, p2 );
			g.DrawLine( pen, p1, p3 );
		}

		/// <summary>
		/// Rotate shape on specified angle around center p. 
		/// </summary>
		/// <param name="points"> Array of points that represents shape to rotate. </param>
		/// <param name="angle">Angle in radians to rotate.</param>
		/// <param name="p">Point to rotate around.</param>
		private void Rotate( PointF[ ] points, float angle, PointF p )
		{
			if( points == null )
			{
				throw new ArgumentNullException( "points" );
			}

			if( points.Length == 0 )
			{
				return;
			}

			float x = 0;
			float y = 0;

			for( int i = 0, len = points.Length ; i < len ; ++i )
			{
				x = points[ i ].X;
				y = points[ i ].Y;

				float transform = p.X;
				float n = p.Y;

				// move each point to new position
				points[ i ].X = ( float )( x * Math.Cos( angle ) - y * Math.Sin( angle ) -
					transform * ( Math.Cos( angle ) - 1 ) + n * Math.Sin( angle ) );

				points[ i ].Y = ( float )( x * Math.Sin( angle ) + y * Math.Cos( angle ) -
					n * ( Math.Cos( angle ) - 1 ) - transform * Math.Sin( angle ) );
			}
		}
		#endregion
	}
}