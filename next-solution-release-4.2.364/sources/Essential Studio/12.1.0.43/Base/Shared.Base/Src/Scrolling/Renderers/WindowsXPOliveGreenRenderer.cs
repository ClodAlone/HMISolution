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
	/// <summary>WindowsXP OliveGreen Style renderer implementation.</summary>
	public class WindowsXPOliveGreenRenderer :
		WindowsXPRenderer
	{
		#region Class members
		/// <summary>
		/// Blend for the default and selected arrow button.
		/// </summary>
		private Blend m_blendArrowButton = null;
		/// <summary>
		/// Blend for the default and selected arrow button.
		/// </summary>
		private Blend m_blendInternalArrowButton = null;
		/// <summary>
		/// Blend for the thumb.
		/// </summary>
		private Blend m_blendThumb = null;
		/// <summary>
		/// Blend for the background.
		/// </summary>
		private Blend m_blendBackGround = null;
		/// <summary></summary>
		private Color m_defaultBorderColor = Color.FromArgb( 142, 153, 125 );
		/// <summary></summary>
		private Color m_selectedBorderColor = Color.FromArgb( 157, 171, 125 );
		/// <summary></summary>
		private Color m_defaultBackColor = Color.FromArgb( 195, 200, 184 );
		/// <summary></summary>
		private Color m_arrowColor = Color.White;
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
		private Color m_arrowButtonShadowColor = Color.FromArgb( 131, 171, 90 );
		/// <summary></summary>
		private Color m_defaultArrowButtonEndColor = Color.FromArgb( 149, 167, 117 );
		/// <summary></summary>
		private Color m_selectedArrowButtonStartColor = Color.FromArgb( 201, 213, 170 );
		/// <summary></summary>
		private Color m_selectedArrowButtonEndColor = Color.FromArgb( 195, 208, 150 );
		/// <summary></summary>
		private Color m_selectedArrowButtonBorderColor = Color.FromArgb( 157, 171, 119 );
		/// <summary></summary>
		private Color m_selectedArrowButtonInternalColor = Color.FromArgb( 218, 232, 185 );
		/// <summary></summary>
		private Color m_pushedArrowButtonStartColor = Color.FromArgb( 149, 167, 125 );
		/// <summary></summary>
		private Color m_pushedArrowButtonEndColor = Color.FromArgb( 149, 170, 114 );
		/// <summary></summary>
		private Color m_pushedArrowButtonBackColor = Color.FromArgb( 155, 174, 126 );
		/// <summary></summary>
		private Color m_pushedArrowButtonBorderColor = Color.FromArgb( 118, 131, 97 );
		/// <summary></summary>
		private Color m_thumbShadowColor = Color.FromArgb( 130, 144, 97 );
		/// <summary></summary>
		private Color m_defaultThumbWhiteLineColor = Color.FromArgb( 208, 223, 172 );
		/// <summary></summary>
		private Color m_defaultThumbGreenLineColor = Color.FromArgb( 140, 157, 115 );
		/// <summary></summary>
		private Color m_defaultThumbStartColor = Color.FromArgb( 165, 183, 142 );
		/// <summary></summary>
		private Color m_defaultThumbEndColor = Color.FromArgb( 149, 167, 117 );
		/// <summary></summary>
		private Color m_selectedThumbWhiteLineColor = Color.FromArgb( 235, 245, 212 );
		/// <summary></summary>
		private Color m_selectedThumbGreenLineColor = Color.FromArgb( 182, 198, 142 );
		/// <summary></summary>
		private Color m_selectedThumbBorderColor = Color.FromArgb( 189, 203, 150 );
		/// <summary></summary>
		private Color m_selectedThumbStartColor = Color.FromArgb( 203, 217, 169 );
		/// <summary></summary>
		private Color m_selectedThumbEndColor = Color.FromArgb( 195, 208, 150 );
		/// <summary></summary>
		private Color m_pushedThumbWhiteLineColor = Color.FromArgb( 185, 208, 151 );
		/// <summary></summary>
		private Color m_pushedThumbGreenLineColor = Color.FromArgb( 122, 139, 99 );
		/// <summary></summary>
		private Color m_pushedThumbStartColor = Color.FromArgb( 155, 173, 130 );
		/// <summary></summary>
		private Color m_pushedThumbEndColor = Color.FromArgb( 149, 170, 114 );
		#endregion

		#region class initialize\finalize methods
		/// <summary>
		/// Initialize new instance of WindowsXPRenderer
		/// </summary>
		protected internal WindowsXPOliveGreenRenderer( bool isVerticalScrollBar ) : 
			base( isVerticalScrollBar )
		{
			IsVerticalScrollBar = isVerticalScrollBar;

			m_blendArrowButton = new Blend();
			m_blendArrowButton.Positions = new float[ ]{0.0F, 0.3F, 0.3F, 0.45F, 0.5F, 0.8F, 0.8F, 1.0F};
			m_blendArrowButton.Factors = new float[ ]{0.0F, 0.25F, 0.4F, 0.45F, 0.6F, 0.7F, 0.8F, 1.0F};

			m_blendInternalArrowButton = new Blend();
			m_blendInternalArrowButton.Positions = new float[ ]{0.0F, 0.1F, 0.1F, 0.7F, 0.7F, 1.0F};
			m_blendInternalArrowButton.Factors = new float[ ]{0.0F, 0.2F, 0.3F, 0.6F, 0.6F, 0.7F};

			m_blendThumb = new Blend();
			m_blendThumb.Positions = new float[ ]{0.0F, 0.23F, 0.23F, 0.55F, 0.55F, 0.75F, 0.75F, 1.0F};
			m_blendThumb.Factors = new float[ ]{0.25F, 0.35F, 0.0F, 0.25F, 0.45F, 0.7F, 0.9F, 1.0F};

			m_blendBackGround = new Blend();
			m_blendBackGround.Positions = new float[ ]{0.0F, 0.55F, 1.0F};
			m_blendBackGround.Factors = new float[ ]{0.0F, 0.9F, 1.0F};
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
				if( IsVerticalScrollBar )
				{
					rc = new Rectangle( bounds.X, bounds.Y - 1, bounds.Width, bounds.Height + 1 );
				}
				else
				{
					rc = new Rectangle( bounds.X - 1, bounds.Y, bounds.Width + 1, bounds.Height );
				}

				DrawShadowForArrowButton( g, rc, m_arrowButtonShadowColor, m_defaultBackGroundStartColor, m_defaultBackGroundEndColor );

				rc.Inflate( -1, -1 );
                using (Pen pen = new Pen(Color.White))
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

				DrawArrows( g, bounds, type, m_arrowColor );
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
				if( IsVerticalScrollBar )
				{
					rc = new Rectangle( bounds.X, bounds.Y, bounds.Width, bounds.Height - 1 );
					rc.Inflate( -1, 0 );
				}
				else
				{
					rc = new Rectangle( bounds.X, bounds.Y, bounds.Width - 1, bounds.Height );
					rc.Inflate( 0, -1 );
				}

				DrawShadowForThumb( g, bounds, m_thumbShadowColor );
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
		/// Fills background with specified colors and gradient.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the background.</param>
		/// <param name="startColor">Start color of the gradient.</param>
		/// <param name="endColor">End color of the gradient.</param>
		protected void FillBackground( Graphics g, Rectangle bounds, Color startColor, Color endColor )
		{
			if( IsVerticalScrollBar )
			{
				bounds.Inflate( -1, 0 );
				using( LinearGradientBrush bgBrush = GetVerticalBrush( bounds, startColor, endColor ) )
				{
					bgBrush.Blend = m_blendBackGround;
					g.FillRectangle( bgBrush, bounds );
				}
			}
			else
			{
				bounds.Inflate( 0, -1 );
				using( LinearGradientBrush bgBrush = GetHorizontalBrush( bounds, startColor, endColor ) )
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
            using (Pen pen = new Pen(m_defaultBackColor))
			g.DrawRectangle( pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1 );

			DrawBorder( g, rc, m_defaultBorderColor );

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, Color.FromArgb( 125, m_defaultArrowButtonEndColor ), m_defaultArrowButtonEndColor ) )
			{
				bgBrush.Blend = m_blendArrowButton;
				using( Pen pen = new Pen( bgBrush ) )
				{
					g.FillRectangle( bgBrush, rc );
				}
			}

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, Color.FromArgb( 100, m_defaultArrowButtonEndColor ), m_defaultArrowButtonEndColor ) )
			{
				bgBrush.Blend = m_blendInternalArrowButton;
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
            using (Pen pen = new Pen(m_defaultBackColor))
                g.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1);
			DrawBorder( g, rc, m_selectedArrowButtonBorderColor );

			rc.Inflate( -1, -1 );
			rc = new Rectangle( rc.X, rc.Y, rc.Width - 1, rc.Height - 1 );

			using( Pen pen = new Pen( m_selectedArrowButtonInternalColor ) )
			{
				g.DrawRectangle( pen, rc );
			}

			rc.Inflate( -1, -1 );
			rc = new Rectangle( rc.X, rc.Y, rc.Width + 1, rc.Height + 1 );
			using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_selectedArrowButtonStartColor, m_selectedArrowButtonEndColor ) )
			{
				g.FillRectangle( bgBrush, rc );
			}

			using( Pen pen = new Pen( m_selectedArrowButtonEndColor ) )
			{
				g.DrawLine( pen, rc.Left - 1, rc.Bottom, rc.Right, rc.Bottom );
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

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_pushedArrowButtonBackColor))
                g.DrawRectangle(pen, rc.Left, rc.Top, rc.Width - 1, rc.Height - 1);
			DrawBorder( g, rc, m_pushedArrowButtonBorderColor );

			rc.Inflate( -1, -1 );
			using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_pushedArrowButtonStartColor, m_pushedArrowButtonEndColor ) )
			{
				g.FillRectangle( bgBrush, rc );
			}

			using( Pen pen = new Pen( Color.FromArgb( 125, m_pushedArrowButtonBorderColor ) ) )
			{
				g.DrawLine( pen, rc.Left, rc.Top, rc.Left, rc.Bottom - 1 );
				g.DrawLine( pen, rc.Right - 1, rc.Top, rc.Right - 1, rc.Bottom - 1 );
			}

			rc.Inflate( -1, -1 );
			using( Pen pen = new Pen( Color.FromArgb( 50, m_pushedArrowButtonBorderColor ) ) )
			{
				g.DrawLine( pen, rc.Left, rc.Top - 1, rc.Left, rc.Bottom );
				g.DrawLine( pen, rc.Right - 1, rc.Top - 1, rc.Right - 1, rc.Bottom );
				g.DrawLine( pen, rc.Left, rc.Top - 1, rc.Right - 1, rc.Top - 1 );
			}
		}

		/// <summary>
		/// Draws default thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawDefaultThumb( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(m_defaultBackColor))
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
			DrawBorder( g, rc, m_defaultBorderColor );

			rc.Inflate( -1, -1 );

			if( IsVerticalScrollBar )
			{
				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
				                                                       m_defaultThumbStartColor, m_defaultThumbEndColor ) )
				{
					bgBrush.Blend = m_blendThumb;
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_defaultThumbStartColor, m_defaultThumbEndColor ) )
				{
					bgBrush.Blend = m_blendThumb;
					g.FillRectangle( bgBrush, rc );
				}
			}

			DrawMiddleLinesForThumb( g, bounds, m_defaultThumbWhiteLineColor, m_defaultThumbGreenLineColor );
		}

		/// <summary>
		/// Draws selected thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawSelectedThumb( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen( Color.FromArgb( 175, m_defaultBackColor )))
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
			DrawBorder( g, rc, m_selectedThumbBorderColor );

			rc.Inflate( -1, -1 );

			if( IsVerticalScrollBar )
			{
				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
				                                                       m_selectedThumbStartColor, m_selectedThumbEndColor ) )
				{
					bgBrush.Blend = m_blendThumb;
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_selectedThumbStartColor, m_selectedThumbEndColor ) )
				{
					bgBrush.Blend = m_blendThumb;
					g.FillRectangle( bgBrush, rc );
				}
			}

			DrawMiddleLinesForThumb( g, bounds, m_selectedThumbWhiteLineColor, m_selectedThumbGreenLineColor );
		}

		/// <summary>
		/// Draws pushed thumb.
		/// </summary>
		/// <param name="g">Graphics object to use.</param>
		/// <param name="bounds">Bounds of the thumb.</param>
		private void DrawPushedThumb( Graphics g, Rectangle bounds )
		{
			Rectangle rc = bounds;

			rc.Inflate( -1, -1 );
            using (Pen pen = new Pen(Color.FromArgb(175, m_defaultBackColor)))
                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
			DrawBorder( g, rc, m_defaultBorderColor );

			rc.Inflate( -1, -1 );

			if( IsVerticalScrollBar )
			{
				using( LinearGradientBrush bgBrush = GetVerticalBrush( new Rectangle( rc.X - 1, rc.Y, rc.Width + 1, rc.Height ),
				                                                       m_pushedThumbStartColor, m_pushedThumbEndColor ) )
				{
					g.FillRectangle( bgBrush, rc );
				}
			}
			else
			{
				using( LinearGradientBrush bgBrush = GetHorizontalBrush( rc, m_pushedThumbStartColor, m_pushedThumbEndColor ) )
				{
					g.FillRectangle( bgBrush, rc );
				}
			}

			using( Pen pen = new Pen( Color.FromArgb( 75, m_defaultBorderColor ) ) )
			{
				g.DrawLine( pen, rc.Left, rc.Top, rc.Right, rc.Top );
				g.DrawLine( pen, rc.Left, rc.Top, rc.Left, rc.Bottom );
			}

			DrawMiddleLinesForThumb( g, bounds, m_pushedThumbWhiteLineColor, m_pushedThumbGreenLineColor );
		}

		/// <summary>
		/// Draws rounded path for the rectangle of arrow button with specified color.
		/// </summary>
		/// <param name="g">The graphics object to use.</param>
		/// <param name="bounds">Bounds of the arrow button.</param>
		/// <param name="color">Color to draw.</param>
		private void DrawBorder( Graphics g, RectangleF bounds, Color color )
		{
			using( GraphicsPath path = GetRoundedPath( bounds, DEF_BORDERS_RADIUS ) )
			{
                using (Pen pen = new Pen(color))
                    g.DrawPath(pen, path);
			}
		}
		#endregion
	}
}