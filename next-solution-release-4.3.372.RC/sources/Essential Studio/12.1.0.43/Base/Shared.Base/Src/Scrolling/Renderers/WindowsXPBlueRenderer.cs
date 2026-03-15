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
using Syncfusion.Windows.Forms.Scrolling.Renderers;
#endregion

namespace Syncfusion.Windows.Forms.Renderers
{
	/// <summary>WindowsXP Blue Style renderer implementation.</summary>
	public class WindowsXPBlueRenderer : 
		WindowsXPRenderer
	{
		#region Class members
		/// <summary>
		/// Blend for the default and selected arrow button.
		/// </summary>
		private Blend m_blendArrowButtonDefault = null;
		/// <summary>
		/// Blend for the pushed arrow button.
		/// </summary>
		private Blend m_blendArrowButtonPushed = null;
		/// <summary>
		/// Blend for the thumb.
		/// </summary>
		private Blend m_blendThumb = null;
		/// <summary>
		/// Blend for the thumb with height &lt; 17.
		/// </summary>
		private Blend m_blendSmallThumb = null;
		/// <summary>
		/// Blend for the border of arrow button.
		/// </summary>
		private Blend m_blendArrowButtonBorder = null;
		/// <summary>
		/// Blend for the background.
		/// </summary>
		private Blend m_blendBackGround = null;
		/// <summary></summary>
		private Color m_defaultBackGroundBorderColor = Color.FromArgb( 238, 237, 229 );
		/// <summary></summary>
		private Color m_defaultBackGroundStartColor = Color.FromArgb( 243, 241, 236 );
		/// <summary></summary>
		private Color m_defaultBackGroundEndColor = Color.FromArgb( 254, 254, 251 );
		/// <summary></summary>
		private Color m_pushedBackGroundBorderColor = Color.FromArgb( 215, 213, 194 );
		/// <summary></summary>
		private Color m_pushedBackGroundStartColor = Color.FromArgb( 227, 222, 211 );
		/// <summary></summary>
		private Color m_pushedBackGroundEndColor = Color.FromArgb( 253, 253, 251 );
		/// <summary></summary>
		private Color m_shadowColor = Color.FromArgb( 124, 159, 211 );
		/// <summary></summary>
		private Color m_defaultArrowButtonStartColor = Color.FromArgb( 225, 234, 254 );
		/// <summary></summary>
		private Color m_defaultArrowButtonEndColor = Color.FromArgb( 174, 200, 247 );
		/// <summary></summary>
		private Color m_defaultArrowButtonBackColor = Color.FromArgb( 225, 234, 254 );
		/// <summary></summary>
		private Color m_defaultArrowButtonBorderColor = Color.FromArgb( 185, 201, 243 );
		/// <summary></summary>
		private Color m_selectedArrowButtonStartColor = Color.FromArgb( 253, 255, 255 );
		/// <summary></summary>
		private Color m_selectedArrowButtonEndColor = Color.FromArgb( 185, 218, 251 );
		/// <summary></summary>
		private Color m_selectedArrowButtonBackColor = Color.FromArgb( 195, 205, 231 );
		/// <summary></summary>
		private Color m_selectedArrowButtonBorderColor = Color.FromArgb( 144, 171, 224 );
		/// <summary></summary>
		private Color m_pushedArrowButtonStartColor = Color.FromArgb( 110, 142, 241 );
		/// <summary></summary>
		private Color m_pushedArrowButtonEndColor = Color.FromArgb( 210, 222, 235 );
		/// <summary></summary>
		private Color m_pushedArrowButtonBackColor = Color.FromArgb( 187, 194, 220 );
		/// <summary></summary>
		private Color m_pushedArrowButtonBorderColor = Color.FromArgb( 114, 128, 216 );
		/// <summary></summary>
		private Color m_arrowColor = Color.FromArgb( 77, 97, 133 );
		/// <summary></summary>
		private Color m_defaultThumbWhiteLineColor = Color.FromArgb( 238, 244, 254 );
		/// <summary></summary>
		private Color m_defaultThumbBlueLineColor = Color.FromArgb( 140, 176, 248 );
		/// <summary></summary>
		private Color m_defaultThumbBackColor = Color.FromArgb( 219, 227, 248 );
		/// <summary></summary>
		private Color m_defaultThumbBorderColor = Color.FromArgb( 184, 203, 246 );
		/// <summary></summary>
		private Color m_defaultThumbStartColor = Color.FromArgb( 200, 214, 251 );
		/// <summary></summary>
		private Color m_defaultThumbEndColor = Color.FromArgb( 181, 205, 250 );
		/// <summary></summary>
		private Color m_selectedThumbWhiteLineColor = Color.FromArgb( 252, 253, 255 );
		/// <summary></summary>
		private Color m_selectedThumbBlueLineColor = Color.FromArgb( 156, 197, 255 );
		/// <summary></summary>
		private Color m_selectedThumbBackColor = Color.FromArgb( 209, 228, 255 );
		/// <summary></summary>
		private Color m_selectedThumbBorderColor = Color.FromArgb( 172, 206, 255 );
		/// <summary></summary>
		private Color m_selectedThumbStartColor = Color.FromArgb( 216, 232, 255 );
		/// <summary></summary>
		private Color m_selectedThumbEndColor = Color.FromArgb( 204, 225, 255 );
		/// <summary></summary>
		private Color m_pushedThumbWhiteLineColor = Color.FromArgb( 207, 221, 253 );
		/// <summary></summary>
		private Color m_pushedThumbBlueLineColor = Color.FromArgb( 131, 158, 216 );
		/// <summary></summary>
		private Color m_pushedThumbBackColor = Color.FromArgb( 164, 191, 252 );
		/// <summary></summary>
		private Color m_pushedThumbBorderColor = Color.FromArgb( 129, 153, 212 );
		/// <summary></summary>
		private Color m_pushedThumbStartColor = Color.FromArgb( 171, 193, 250 );
		/// <summary></summary>
		private Color m_pushedThumbEndColor = Color.FromArgb( 148, 183, 250 );
		#endregion

		#region class initialize\finalize methods

		/// <summary>
		/// Initialize new instance of WindowsXPRenderer
		/// </summary>
		/// <param name="isVerticalScrollBar"/>
		protected internal WindowsXPBlueRenderer( bool isVerticalScrollBar )
			: base( isVerticalScrollBar )
		{
			IsVerticalScrollBar = isVerticalScrollBar;

			m_blendArrowButtonDefault = new Blend();
			m_blendArrowButtonDefault.Positions = new float[ ]{0.0F, 0.4F, 1.0F};
			m_blendArrowButtonDefault.Factors = new float[ ]{0.1F, 0.75F, 1.0F};

			m_blendArrowButtonPushed = new Blend();
			m_blendArrowButtonPushed.Positions = new float[ ]{0.0F, 0.1F, 0.6F, 1.0F};
			m_blendArrowButtonPushed.Factors = new float[ ]{0.1F, 0.1F, 0.4F, 1.0F};

			m_blendArrowButtonBorder = new Blend();
			m_blendArrowButtonBorder.Positions = new float[ ]{0.0F, 0.3F, 1.0F};
			m_blendArrowButtonBorder.Factors = new float[ ]{0.0F, 1.0F, 1.0F};

			m_blendThumb = new Blend();
			m_blendThumb.Positions = new float[ ]{0.0F, 0.65F, 0.76F, 1.0F};
			m_blendThumb.Factors = new float[ ]{0.0F, 0.3F, 1.0F, 1.0F};

			m_blendSmallThumb = new Blend();
			m_blendSmallThumb.Positions = new float[ ]{0.0F, 0.5F, 0.65F, 1.0F};
			m_blendSmallThumb.Factors = new float[ ]{0.0F, 0.2F, 1.0F, 1.0F};

			m_blendBackGround = new Blend();
			m_blendBackGround.Positions = new float[] { 0.0F, 0.55F, 1.0F };
			m_blendBackGround.Factors = new float[] { 0.0F, 0.9F, 1.0F };
		}

		#endregion

		#region Class overrides
		/// <summary>
		/// Draws arrow button of scroll. If theme is disabled than draw classic scroll. 
		/// </summary>
		/// <param name="g"></param>
		/// <param name="bounds"></param>
		/// <param name="type"></param>
		/// <param name="state"></param>
		public override void DrawArrowButton( Graphics g, Rectangle bounds, ScrollButton type, ButtonState state )
		{
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				DrawBackgroundLines( g, bounds, m_defaultBackGroundBorderColor );

				Rectangle rc;
				if ( IsVerticalScrollBar )
				{
					rc = new Rectangle( bounds.X, bounds.Y - 1, bounds.Width, bounds.Height + 1 );
				}
				else
				{
					rc = new Rectangle( bounds.X - 1, bounds.Y, bounds.Width + 1, bounds.Height );
				}

				DrawShadowForArrowButton( g, rc, m_shadowColor, m_defaultBackGroundStartColor, m_defaultBackGroundEndColor );

				rc.Inflate( -1, -1 );
                using (Pen pen = new Pen( Color.White ))
                    g.DrawPath(pen, GetRoundedPath(rc, DEF_BORDERS_RADIUS));

				switch( state )
				{
					case ButtonState.Normal:
						DrawDefaultArrowButton( g, rc );
						break;

					case ButtonState.Pushed:
						DrawPushedArrowButton( g, rc );
						break;

					case ButtonState.Checked:
						DrawSelectedArrowButton( g, rc );
						break;
				}

				DrawArrows(g, bounds, type, m_arrowColor);
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
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				if( state == ButtonState.Normal )
				{
					DrawDefaultBackground( g, bounds );
				}
				else if( state == ButtonState.Pushed )
				{
					DrawPushedBackground( g, bounds );
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
			if( null == g )
				throw new ArgumentNullException( "g" );

			if( bounds.Width > 0 && bounds.Height > 0 )
			{
				DrawBackground( g, bounds, ButtonState.Normal );

				Rectangle rc;
				if ( IsVerticalScrollBar )
				{
					rc = new Rectangle( bounds.X, bounds.Y, bounds.Width, bounds.Height - 1 );
					rc.Inflate( -1, 0 );
				}
				else
				{
					rc = new Rectangle( bounds.X, bounds.Y, bounds.Width - 1, bounds.Height );
					rc.Inflate( 0, -1 );
				}

				DrawShadowForThumb( g, bounds, m_shadowColor );
                using (Pen pen = new Pen(Color.White))
                    g.DrawPath(pen, GetRoundedPath(rc, DEF_BORDERS_RADIUS));

				switch( state )
				{
					case ButtonState.Normal:
						DrawDefaultThumb( g, rc );
						break;

					case ButtonState.Pushed:
						DrawPushedThumb( g, rc );
						break;

					case ButtonState.Checked:
						DrawSelectedThumb( g, rc );
						break;
				}
			}
		}
		#endregion

		#region Class Utility methods

		/// <summary>
		/// Gets path that represents left and top lines.
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		/// <param name="iRadius"/>
		private GraphicsPath GetTopRoundedPath( RectangleF bounds, int iRadius )
		{
			InitializeRectangleEdges( bounds );

			GraphicsPath path = new GraphicsPath();

			path.AddLine( m_iLeft, m_iBottom - iRadius, m_iLeft, m_iTop + iRadius );
			path.AddLine( m_iLeft, m_iTop + iRadius, m_iLeft + iRadius, m_iTop );
			path.AddLine( m_iLeft + iRadius, m_iTop, m_iRight - iRadius, m_iTop );

			return path;
		}

		/// <summary>
		/// Gets path that represents bottom and right lines.
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		/// <param name="iRadius"/>
		private GraphicsPath GetBottomRoundedPath( RectangleF bounds, int iRadius )
		{
			InitializeRectangleEdges( bounds );

			GraphicsPath path = new GraphicsPath();

			path.AddLine( m_iRight, m_iTop + iRadius, m_iRight, m_iBottom - iRadius );
			path.AddLine( m_iRight, m_iBottom - iRadius, m_iRight - iRadius, m_iBottom );
			path.AddLine( m_iRight - iRadius, m_iBottom, m_iLeft + iRadius, m_iBottom );

			return path;
		}
                
		/// <summary>
		/// Fills background with specified colors and gradient.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the background.</param>
		/// <param name="startColor">Start color of the gradient.</param>
		/// <param name="endColor">End color of the gradient.</param>
		protected void FillBackground( Graphics g, Rectangle bounds, Color startColor, Color endColor )
		{
			if ( IsVerticalScrollBar )
			{
				bounds.Inflate( -1, 0 );
				using ( LinearGradientBrush bgBrush = GetVerticalBrush(bounds, startColor, endColor ) )
				{
					bgBrush.Blend = m_blendBackGround;
					g.FillRectangle( bgBrush, bounds );
				}
			}
			else
			{
				bounds.Inflate( 0, -1 );
				using ( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, startColor, endColor ) )
				{
					bgBrush.Blend = m_blendBackGround;
					g.FillRectangle( bgBrush, bounds );
				}
			}
		}

		/// <summary>
		/// Draws default background.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of background.</param>
		private void DrawDefaultBackground( Graphics g, Rectangle bounds )
		{
			DrawBackgroundLines( g, bounds, m_defaultBackGroundBorderColor );
			FillBackground( g, bounds, m_defaultBackGroundStartColor, m_defaultBackGroundEndColor );
		}

		/// <summary>
		/// Draws pushed background.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of background.</param>
		private void DrawPushedBackground( Graphics g, Rectangle bounds )
		{
			DrawBackgroundLines( g, bounds, m_pushedBackGroundBorderColor );
			FillBackground( g, bounds, m_pushedBackGroundStartColor, m_pushedBackGroundEndColor );
		}

		/// <summary>
		/// Draws default arrow button.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		private void DrawDefaultArrowButton( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_defaultArrowButtonBackColor))
                g.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1);

			DrawBorderForArrowButton( g, rc, m_defaultArrowButtonEndColor, m_defaultArrowButtonBorderColor );

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetSlopingBrush( rc, m_defaultArrowButtonStartColor, m_defaultArrowButtonEndColor ) )
			{
				bgBrush.Blend = m_blendArrowButtonDefault;
				g.FillRectangle( bgBrush, rc );
			}
		}

		/// <summary>
		/// Draws selected arrow button.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		private void DrawSelectedArrowButton( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_selectedArrowButtonBackColor))
                g.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1);

			DrawBorderForArrowButton( g, rc, m_selectedArrowButtonBorderColor, m_selectedArrowButtonBorderColor );

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetSlopingBrush( rc, m_selectedArrowButtonStartColor, m_selectedArrowButtonEndColor ) )
			{
				bgBrush.Blend = m_blendArrowButtonDefault;
				g.FillRectangle( bgBrush, rc );
			}
		}

		/// <summary>
		/// Draws pushed arrow button.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		private void DrawPushedArrowButton( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;
			Pen pushedPen = new Pen( m_pushedArrowButtonBackColor );
			Pen defaultPen = new Pen( m_defaultArrowButtonBackColor );

			rc.Inflate( -1, -1 );
			g.DrawLine( pushedPen, rc.Left, rc.Top, rc.Left + rc.Width - 1, rc.Top );
			g.DrawLine( pushedPen, rc.Left, rc.Top, rc.Left, rc.Top + rc.Height - 1 );
			g.DrawLine( defaultPen, rc.Left, rc.Top + rc.Height - 1, rc.Left + rc.Width - 1, rc.Top + rc.Height - 1 );

			DrawBorderForArrowButton( g, rc, m_pushedArrowButtonBorderColor, m_defaultArrowButtonBorderColor );

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetSlopingBrush( rc, m_pushedArrowButtonStartColor, m_pushedArrowButtonEndColor ) )
			{
				bgBrush.Blend = m_blendArrowButtonPushed;
				g.FillRectangle( bgBrush, rc );
			}
            pushedPen.Dispose();
            defaultPen.Dispose();
		}

		/// <summary>
		/// Draws default thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawDefaultThumb( Graphics g, Rectangle bounds )
		{
			Blend blend = new Blend();
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_defaultThumbBackColor))
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
            using (Pen pen = new Pen(m_defaultThumbBorderColor))
                g.DrawPath(pen, GetRoundedPath(rc, DEF_BORDERS_RADIUS));

			rc.Inflate( -1, -1 );
			blend = m_blendThumb;

			if ( IsVerticalScrollBar )
			{
				if( bounds.Width <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
								 m_defaultThumbStartColor, m_defaultThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				if( bounds.Height <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_defaultThumbStartColor, m_defaultThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}

			DrawMiddleLinesForThumb( g, bounds, m_defaultThumbWhiteLineColor, m_defaultThumbBlueLineColor );
		}

		/// <summary>
		/// Draws selected thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawSelectedThumb( Graphics g, Rectangle bounds )
		{
			Blend blend = new Blend();
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_defaultThumbBorderColor))
            {
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                g.DrawPath(pen, GetRoundedPath(rc, DEF_BORDERS_RADIUS));
            }

			rc.Inflate( -1, -1 );
			blend = m_blendThumb;

			if ( IsVerticalScrollBar )
			{
				if( bounds.Width <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
								 m_selectedThumbStartColor, m_selectedThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				if( bounds.Height <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_selectedThumbStartColor, m_selectedThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}

			DrawMiddleLinesForThumb( g, bounds, m_selectedThumbWhiteLineColor, m_selectedThumbBlueLineColor );
		}

		/// <summary>
		/// Draws pushed thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawPushedThumb( Graphics g, Rectangle bounds )
		{
			Blend blend = new Blend();
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_pushedThumbBackColor))
            {
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
            }

			DrawBorderForThumb( g, rc, m_pushedThumbBorderColor, Color.FromArgb( 100, m_pushedThumbBorderColor ) );

			rc.Inflate( -1, -1 );
			blend = m_blendThumb;

			if ( IsVerticalScrollBar )
			{
				if( bounds.Width <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
								 m_pushedThumbStartColor, m_pushedThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				if( bounds.Height <= 13 )
				{
					blend = m_blendSmallThumb;
				}

				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_pushedThumbStartColor, m_pushedThumbEndColor ) )
				{
					bgBrush.Blend = blend;
					g.FillRectangle( bgBrush, rc );
				}
			}
            using (Pen pen = new Pen(Color.FromArgb(50, m_shadowColor)))
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
			DrawMiddleLinesForThumb( g, bounds, m_pushedThumbWhiteLineColor, m_pushedThumbBlueLineColor );
		}

		/// <summary>
		/// Draws rounded path for the rectangle of thumb with specified color.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		/// <param name="color1">Color to draw.</param>
		/// <param name="color2">Color to draw.</param>
		private void DrawBorderForThumb( Graphics g, RectangleF bounds, Color color1, Color color2 )
		{
			using( GraphicsPath path = GetTopRoundedPath( bounds, DEF_BORDERS_RADIUS ) )
			{
                using (Pen pen = new Pen(color1))
                    g.DrawPath(pen, path);
			}

			using( GraphicsPath path = GetBottomRoundedPath( bounds, DEF_BORDERS_RADIUS ) )
			{
                using (Pen pen = new Pen(color2))
                    g.DrawPath(pen, path);
			}
		}

		/// <summary>
		/// Draws rounded path for the rectangle of arrow button with specified color.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		/// <param name="color1">Color to draw.</param>
		/// <param name="color2">Color to draw.</param>
		private void DrawBorderForArrowButton( Graphics g, RectangleF bounds, Color color1, Color color2 )
		{
			using( GraphicsPath path = GetTopRoundedPath( bounds, DEF_BORDERS_RADIUS ) )
			{
				using( LinearGradientBrush bgBrush = GetSlopingBrush( bounds, Color.FromArgb( 75, color1 ), color1 ) )
				{
					bgBrush.Blend = m_blendArrowButtonBorder;
                    using (Pen pen = new Pen(bgBrush))
                        g.DrawPath(pen, path);
				}
			}

			using( GraphicsPath path = GetBottomRoundedPath( bounds, DEF_BORDERS_RADIUS ) )
			{
                using (Pen pen = new Pen(color2))
                    g.DrawPath(pen, path);
			}
		}

		#endregion
	}
}