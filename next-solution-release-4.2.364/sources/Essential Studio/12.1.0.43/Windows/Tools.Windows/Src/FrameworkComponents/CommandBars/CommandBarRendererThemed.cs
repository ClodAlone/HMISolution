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
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using Syncfusion.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents themed renderer for docked CommandBar.
	/// </summary>
	internal class CommandBarRendererThemed : CommandBarRenderer
	{
		#region Class Members
		// using for draw control with themes
		protected ThemedControlDrawing m_tdRebar = null;
		protected ThemedControlDrawing m_tdToolbar = null;
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarRendererThemed( CommandBar commandBar ) : base( commandBar )
		{
			m_tdRebar = new ThemedControlDrawing( ThemedControls.REBAR );
			m_tdToolbar = new ThemedControlDrawing( ThemedControls.TOOLBAR );
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with themes.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Font textFont = this.CmdBar.Font;
			Color foreColor = this.CmdBar.ForeColor;
			CommandBarPainter.PaintDockedText( g, rect, text, textFont, foreColor, bRTL, bVertical );
		}
		/// <summary>
		/// Draws background of the CommandBar with themes.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Rectangle clientRect = rect;
			IntPtr hdc = g.GetHdc();
            try
            {
                Syncfusion.Runtime.InteropServices.NativeMethods.RECT rc = new Syncfusion.Runtime.InteropServices.NativeMethods.RECT(rect);
                Syncfusion.Runtime.InteropServices.NativeMethods.DrawThemeParentBackground(this.CmdBar.Handle, hdc, ref rc);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }

			// Draw a border along the trailing edge of the bar
			Pen pn1 = new Pen( ControlPaint.LightLight(SystemColors.ControlDark), 1 );
			Pen pn2 = new Pen( SystemColors.ControlLightLight, 1 );

			if( bVertical )
			{
				if( bRTL )
				{
					g.DrawLine(pn1, clientRect.Left+2, clientRect.Top+1, clientRect.Right-3, clientRect.Top+1);
					g.DrawLine(pn2, clientRect.Left+2, clientRect.Top,   clientRect.Right-3, clientRect.Top  );
				}
				else
				{
					g.DrawLine(pn1, clientRect.Left+2, clientRect.Bottom-2, clientRect.Right-3, clientRect.Bottom-2);
					g.DrawLine(pn2, clientRect.Left+2, clientRect.Bottom-1, clientRect.Right-3, clientRect.Bottom-1);
				}
			}
			else
			{
				if( bRTL )
				{
					g.DrawLine(pn1, clientRect.Left+1, clientRect.Top+2, clientRect.Left+1, clientRect.Bottom-3);
					g.DrawLine(pn2, clientRect.Left+0, clientRect.Top+2, clientRect.Left+0, clientRect.Bottom-3);
				}
				else
				{
					g.DrawLine(pn1, clientRect.Right-2, clientRect.Top+2, clientRect.Right-2, clientRect.Bottom-3);
					g.DrawLine(pn2, clientRect.Right-1, clientRect.Top+2, clientRect.Right-1, clientRect.Bottom-3);
				}
			}

			pn1.Dispose();
			pn2.Dispose();
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with themes.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			// draw dropdown
			int iThemeStates = ThemeStates.TS_NORMAL;

			if( state == CBButtonState.Hot )
			{
				iThemeStates = ThemeStates.TS_HOT;
			}
			else if( state == CBButtonState.Pressed )
			{
				iThemeStates = ThemeStates.TS_PRESSED;
			}

			this.m_tdToolbar.DrawThemeBackground( g, ThemeParts.TP_BUTTON, iThemeStates, rect );

			// draw dropdown arrow and chevron arrow
			Color chevronColor = this.CmdBar.ChevronColor;
			CommandBarPainter.PaintDropDownArrowsThemed( g, rect, bRTL, bVertical, bShowChevron, chevronColor, bShowArrow );
		}

		/// <summary>
		/// Draws gripper of the CommandBar with themes.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			int nGprXOff = 2;
			int iWidth = this.CmdBar.Width;
			int iHeight = this.CmdBar.Height;

			if( bRTL )
			{
				if( bVertical )
				{
					m_tdRebar.DrawThemeBackground( g, ThemeParts.RP_GRIPPERVERT, 0,
						new Rectangle( 2, rect.Bottom - 8, iWidth - 4, 6) );
				}
				else
				{
					m_tdRebar.DrawThemeBackground( g, ThemeParts.RP_GRIPPER, 0,
						new Rectangle( rect.Right - 8, 2, 6, iHeight - 4 ) );
					
				}
			}
			else
			{
				if( bVertical )
				{
					m_tdRebar.DrawThemeBackground( g, ThemeParts.RP_GRIPPERVERT, 0,
						new Rectangle( 2, nGprXOff, iWidth - 4, 6 ) );
				}
				else
				{
					m_tdRebar.DrawThemeBackground( g, ThemeParts.RP_GRIPPER, 0, 
						new Rectangle( nGprXOff, 2, 6, iHeight - 4 ) );
				}
					
			}
		}

		/// <summary>
		/// Gets rectangle of the DropDown button.
		/// </summary>
		protected override Rectangle GetDropDownRect()
		{
			Rectangle rect = this.CmdBar.DropDownRect;

			if( IsVertical() )
			{
				rect.Y -= 2;
			}
			else
			{
				rect.X += ( this.CmdBar.IsRTL ) ? 1 : 2;
			}

			return rect;
		}


		protected override void OnDispose( bool diposing )
		{
			if( diposing )
			{
				m_tdRebar.Dispose();
				m_tdToolbar.Dispose();
			}

			base.OnDispose( diposing );
		}

		#endregion
	}

	/// <summary>
	/// Represents themed renderer for floating CommandBar.
	/// </summary>
	internal class CommandBarFloatingRendereThemed : CommandBarFloatingRenderer
	{
		#region Class Members
		// using for draw control with themes
		protected ThemedControlDrawing m_tdWindow = null;
		#endregion

		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRendereThemed( CommandBar commandBar ) : base( commandBar )
		{
			m_tdWindow = new ThemedControlDrawing( ThemedControls.WINDOW );
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with themes.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Color foreColor = GetThemedCaptionColor();
			Font textFont = GetThemedCaptionFont();
			CommandBarPainter.PaintFloatingText( g, rect, text, textFont, foreColor, bRTL );
		}
		/// <summary>
		/// Draws background of the CommandBar with themes.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			// Leave background painting to the parent frame
			Region initialcliprgn = g.Clip;
            using (Region rgnbkgnd = new Region(this.CmdBar.ClientRectangle))
            {
                rgnbkgnd.Exclude(this.CmdBar.CaptionRect);
                g.ExcludeClip(rgnbkgnd);
            }

			IntPtr hdc = g.GetHdc();
            try
            {
                Syncfusion.Runtime.InteropServices.NativeMethods.RECT rc = new Syncfusion.Runtime.InteropServices.NativeMethods.RECT(this.CmdBar.ClientRectangle);
                Syncfusion.Runtime.InteropServices.NativeMethods.DrawThemeParentBackground(this.CmdBar.Handle, hdc, ref rc);
            }
            finally
            {
                g.ReleaseHdc(hdc);
            }
			g.Clip = initialcliprgn;

			// draw background
			Rectangle captionRect = this.GetTextRectangle();
			captionRect.X = rect.X + 1;
			captionRect.Width = rect.Width - 2;
			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_SMALLCAPTION, ThemeStates.CS_INACTIVE, captionRect );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with themes.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			int iState = ThemeStates.MINBS_NORMAL;
			Rectangle buttonRect = rect;

			if( this.CmdBar.CmdBarHilight == CBButtons.DropDown )
			{
				iState = ThemeStates.MINBS_HOT;
			}
			else if( this.CmdBar.CmdBarHilight == ( CBButtons.DropDown 
				| CBButtons.Pressed ) )
			{
				iState = ThemeStates.MINBS_PUSHED;
			}

			// draw background for dropdown arrow
			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_MINBUTTON, iState,
				Rectangle.Inflate( buttonRect, -1, -1 ) );
			RectangleF rcclip = g.ClipBounds;
			g.SetClip( Rectangle.Inflate( buttonRect, -3, -3 ) );
			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_MINBUTTON, iState, 
				new Rectangle( buttonRect.Left, buttonRect.Top, buttonRect.Width, buttonRect.Height + 10 ) );
			g.SetClip( rcclip );

			// draw dropdown arrow
			CommandBarPainter.PaintFloatingDropDownArrowsThemed( g, rect, bRTL );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with themes.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			// doesn't need draw gripper
		}
		/// <summary>
		/// Draws close button of the floating CommandBar with themes.
		/// </summary>
		protected override void DrawCloseButton( Graphics g, Rectangle rect, CBButtonState state )
		{
			Rectangle buttonRect = this.CmdBar.CloseButtonRect;
			int iState = ThemeStates.CBS_NORMAL;

			if( this.CmdBar.CmdBarHilight == CBButtons.Close )
			{
				iState = ThemeStates.CBS_HOT;
			}
			else if(this.CmdBar.CmdBarHilight == ( CBButtons.Close
				| CBButtons.Pressed ) )
			{
				iState = ThemeStates.CBS_PUSHED;
			}

			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_SMALLCLOSEBUTTON, iState, 
				Rectangle.Inflate( buttonRect, -1, -1 ) );
		}

		protected override void OnDispose( bool diposing )
		{
			if( diposing )
			{
				m_tdWindow.Dispose();
			}
				
			base.OnDispose( diposing );
		}

		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Gets color for caption text.
		/// </summary>
		private Color GetThemedCaptionColor()
		{
			ulong ucolor = 0;
			NativeMethods.GetThemeColor( m_tdWindow.HTheme, ThemeParts.WP_MINCAPTION, 
				ThemeStates.CS_INACTIVE, NativeMethods.TMT_TEXTCOLOR, ref ucolor );

			int rgb = NativeMethods.COLORREFToRGB( ( int )ucolor );
			Color captionColor = Color.FromArgb(NativeMethods.GetRValue( rgb ),
				NativeMethods.GetGValue( rgb ),
				NativeMethods.GetBValue( rgb ) );

			if( captionColor == Color.Empty )
			{
				captionColor = this.CmdBar.ForeColor;
			}

			return captionColor; 
		}

		/// <summary>
		/// Gets font for caption text.
		/// </summary>
		private Font GetThemedCaptionFont()
		{
			NativeMethods.LOGFONT lFont = new NativeMethods.LOGFONT();				
			NativeMethods.GetThemeSysFont( m_tdWindow.HTheme, NativeMethods.TMT_CAPTIONFONT, ref lFont);
        
			Font captionFont = Font.FromLogFont(lFont);	

			if( captionFont == null )
			{
				captionFont = this.CmdBar.GetFloatinCaptionFont();
			}

			return captionFont;
		}
		#endregion
	}

	/// <summary>
	/// Represents themed renderer for docked ControlBar.
	/// </summary>
	internal class ControlBarRendererThemed : ControlBarRenderer
	{
		#region Class Members
		// using for draw control with themes
		protected ThemedControlDrawing m_tdRebar = null;
		protected ThemedControlDrawing m_tdWindow = null;
		#endregion

		#region Class Initialize/Finalize Methods
		public ControlBarRendererThemed( ControlBar controlBar ) : base( controlBar )
		{
			m_tdRebar = new ThemedControlDrawing( ThemedControls.REBAR );
			m_tdWindow = new ThemedControlDrawing( ThemedControls.WINDOW );
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the ControlBar with themes.
		/// </summary>
		protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			string text = this.ControlBar.Text;
			Color foreColor = this.ControlBar.ForeColor;
			Font textFont = this.ControlBar.Font;
			CommandBarPainter.PaintControlBarText( g, rect, text, textFont, foreColor, bRTL );
		}

		/// <summary>
		/// Draws background of the ControlBar with themes.
		/// </summary>
		protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
		{
			Rectangle clientRect = rect;

			using( CMirroredDrawer mirrorDrawer = new CMirroredDrawer( g, clientRect, bRTL ) )
			{
				Graphics gphVirtual = mirrorDrawer.VirtualGfx;
				IntPtr hdc = gphVirtual.GetHdc();
                try
                {
                    NativeMethods.RECT rc = new NativeMethods.RECT(mirrorDrawer.VirtualBounds);
                    NativeMethods.DrawThemeParentBackground(this.CmdBar.Handle, hdc, ref rc);
                }
                finally
                {
                    gphVirtual.ReleaseHdc(hdc);
                }
			}

			if( this.ControlBar.ClientControl != null
				&& this.ControlBar.ClientControl.ContainsFocus == true )
			{
				Rectangle captionRect = this.CmdBar.CaptionRect;					
				Brush cptnbrush = cptnbrush = new SolidBrush( SystemColors.ActiveCaption );
				g.FillRectangle( cptnbrush, captionRect );
				cptnbrush.Dispose();
			}
		}

		/// <summary>
		/// Draws DropBownButton of the ControlBar with themes.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			// draw background of the DropDown area
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = MenuColors.SelBorderColor;
					fillColor = MenuColors.SelColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = MenuColors.DropDownBorderColor;
					fillColor = this.CmdBar.BackColor;
					break;
				}
			}

			CommandBarPainter.PaintControlBarDropDownThemed( g, rect, borderColor, fillColor );

			// draw background for dropdown button
			int iCaptionHeight = CommandBar.CaptionHeight;
			Rectangle buttonRect = new Rectangle( bRTL ? rect.Left : 
				rect.Right - iCaptionHeight, rect.Top, iCaptionHeight, iCaptionHeight);

			int iState = ThemeStates.MINBS_NORMAL;

			if( this.CmdBar.CmdBarHilight == CBButtons.DropDown )
			{
				iState = ThemeStates.MINBS_HOT;
			}
			else if( this.CmdBar.CmdBarHilight == ( CBButtons.DropDown 
				| CBButtons.Pressed ) )
			{
				iState = ThemeStates.MINBS_PUSHED;
			}
			
			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_MINBUTTON, iState,
				Rectangle.Inflate( buttonRect, -1, -1 ) );
			RectangleF rcclip = g.ClipBounds;
			g.SetClip( Rectangle.Inflate( buttonRect, -3, -3 ) );
			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_MINBUTTON, iState, 
				new Rectangle( buttonRect.Left, buttonRect.Top, buttonRect.Width, buttonRect.Height + 10 ) );
			g.SetClip( rcclip );

			// draw dropdown arrow
			if( bShowArrow )
			{
				CommandBarPainter.PaintFloatingDropDownArrowsThemed( g, buttonRect, bRTL );
			}
		}

		/// <summary>
		/// Draws close button of the ControlBar.
		/// </summary>
		protected override void DrawCloseButton(Graphics g, Rectangle rect, Syncfusion.Windows.Forms.Tools.CBButtonState state)
		{
			Rectangle buttonRect = this.ControlBar.CloseButtonRect;
			
			// draw background of the close button
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				fillColor = MenuColors.SelBorderColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				fillColor = Office2003Colors.PressedSelColor;
			}

			CommandBarPainter.PaintControlBarDropDownThemed( g, rect, fillColor, fillColor );

			// draw close buton
			int iState = ThemeStates.CBS_NORMAL;

			if( this.ControlBar.CmdBarHilight == CBButtons.Close )
			{
				iState = ThemeStates.CBS_HOT;
			}
			else if(this.ControlBar.CmdBarHilight == ( CBButtons.Close
				| CBButtons.Pressed ) )
			{
				iState = ThemeStates.CBS_PUSHED;
			}

			m_tdWindow.DrawThemeBackground( g, ThemeParts.WP_SMALLCLOSEBUTTON, iState, 
				Rectangle.Inflate( buttonRect, -1, -1 ) );
		}
		/// <summary>
		/// Draws gripper of the ControlBar with themes.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Rectangle clientRect = this.CmdBar.ClientRectangle;
			int iClientRight = clientRect.Right;
			int iGprXOff = 2;
			m_tdRebar.DrawMirrored = bRTL;

			m_tdRebar.DrawThemeBackground( g, ThemeParts.RP_GRIPPER, 0,
				new Rectangle( bRTL ? iClientRight - iGprXOff - 2 - 6 : iGprXOff + 2,
				2, 6, this.ControlBar.ControlBarCaptionHeight - 3 ) );
		}

		protected override void OnDispose( bool diposing )
		{
			if( diposing )
			{
				m_tdRebar.Dispose();
				m_tdWindow.Dispose();
			}

			base.OnDispose( diposing );
		}
		#endregion
	}
}
