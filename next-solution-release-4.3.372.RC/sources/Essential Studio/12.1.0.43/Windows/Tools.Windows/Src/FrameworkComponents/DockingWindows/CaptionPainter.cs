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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Tools.Renderers;
using Syncfusion.Drawing;
namespace Syncfusion.Windows.Forms.Tools
{
	// The CaptionPainter class is a helper class that abstracts the caption paint implementation from the
	// dockhost and the floating form.
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CaptionPainter : Syncfusion.ComponentModel.Disposable
	{
		#region Enums

		public enum CaptionHitTest
		{
			None = 0,
			CaptionRect = 1,
			ButtonRect,
			ButtonDown,
			ButtonUp
		}

		#endregion

		#region Constants

		protected const int	nTextOffset = 2;
		protected const int DEF_BUTTON_TOP_INDENT = 4;
		protected const int DEF_BUTTON_SIDE_INDENT = 3;
		protected const int DEF_BUTTON_SPACE_BETWEEN = 1;
		protected const int DEF_BUTTON_THEMED_SIZE = 20;
		protected const int DEF_ICON_SIZE = 16;
		protected const int DEF_ICON_INDENT = 2;
        protected const int DEF_CAPTION_TOP_INDENT = 2;
        protected const int DEF_BORDER_WIDTH = 1;

		#endregion

		#region Protected members

		protected DockHostController dhcOwner = null;
		protected CaptionButton btnHover = null;
		protected CaptionButton btnPressed = null;
		protected int nCaptionButtonIndex = -1;
		protected CaptionButtonsCollection m_buttons = new CaptionButtonsCollection();

		protected DockLabelAlignmentStyle labelAlignment = DockLabelAlignmentStyle.Default;

		// XPTheme drawing
		protected ThemedControlDrawing tdExplorerBar = null;
		protected ThemedControlDrawing tdWindow = null;
		
		protected Font ftCaption = SystemInformation.MenuFont;
		
		protected ToolTip toolTip = new ToolTip();

		#endregion

		#region Properties

		internal DockLabelAlignmentStyle LabelAlignment
		{
			get { return labelAlignment; }
			set { labelAlignment = value; }
		}

		public CaptionButtonsCollection Buttons
		{
			get
			{
				return m_buttons;
			}
		}

        protected internal int CaptionTopIndent
        {
            get { return DEF_CAPTION_TOP_INDENT; }
        }

        public int BorderWidth
        {
            get { return DEF_BORDER_WIDTH; }
        }

		public Rectangle CaptionRect
		{
			get
			{
				if(this.dhcOwner.HideCaption == false)
					return new Rectangle(0, CaptionTopIndent, this.dhcOwner.HostControl.Width, CaptionPainter.CaptionHeight);
				else
				{
					int captionht = CaptionPainter.CaptionHeight;
					if(XPThemes.IsThemedOS && XPThemes.IsThemeActive)
						captionht += 4;
					return new Rectangle(0, CaptionTopIndent - captionht, this.dhcOwner.HostControl.Width, captionht);
				}
			}
		}

		protected Rectangle TextRect
		{
			get
			{
				Rectangle rc = this.CaptionRect;

				int nLeft;
				int nWidth;
				int nImageOffset = 0;
				int nBtnsVisible = 0;
				bool bRestoreButtonExists = dhcOwner.DockingManager.CaptionButtons.ContainsButtonType(CaptionButtonType.Restore);
				for( int i = 0; i < Buttons.Count; i++ )
				{
					if( Buttons[i].Type == CaptionButtonType.Close 
						&& dhcOwner.CloseButtonVisibility ||
						Buttons[i].Type == CaptionButtonType.Pin 
						&& dhcOwner.AutoHideButtonVisibility ||
						Buttons[i].Type == CaptionButtonType.Menu 
						&& dhcOwner.MenuButtonVisiblity ||
						bRestoreButtonExists &&	!Buttons[i].Modified && dhcOwner.MaximizeButtonVisibility
						&& (Buttons[i].Type == CaptionButtonType.Maximize || Buttons[i].Type == CaptionButtonType.Restore) ||
						!bRestoreButtonExists && Buttons[i].Type == CaptionButtonType.Maximize && dhcOwner.MaximizeButtonVisibility ||
						Buttons[i].Type == CaptionButtonType.Custom)
					nBtnsVisible++;
				}
				if( dhcOwner.ImageIndex >= 0 && dhcOwner.DockingManager.ImageList != null 
					&& dhcOwner.ImageIndex <= dhcOwner.DockingManager.ImageList.Images.Count 
					&& dhcOwner.DockingManager.ShowCaptionImages ) 
				{
					nImageOffset = DEF_ICON_SIZE + DEF_ICON_INDENT;
				}
				if (this.dhcOwner.DockingManager.IsMirrored)
				{
					nLeft = 5 + (nBtnsVisible * CaptionPainter.CaptionButtonWidth);
					nWidth = rc.Width - nLeft - nImageOffset - 5; 
				}
				else
				{
					nLeft = rc.X + 2 + nImageOffset;
					nWidth = rc.Width - 5 - (nBtnsVisible*CaptionPainter.CaptionButtonWidth) - nImageOffset;
				}

				return new Rectangle(nLeft, rc.Y, nWidth, rc.Height);
			}
		}

		protected Rectangle ImageRect
		{
			get
			{
				if( !dhcOwner.DockingManager.ShowCaptionImages || dhcOwner.ImageIndex < 0 ||
					dhcOwner.DockingManager.ImageList == null ||
					dhcOwner.ImageIndex >= dhcOwner.DockingManager.ImageList.Images.Count )
				{
					return Rectangle.Empty;
				}
                Rectangle rcbutton = GetCaptionButtonBounds(Math.Max(Buttons.Count - 1, 0));
                if (rcbutton == Rectangle.Empty)
                {
                    return Rectangle.Empty;
                }

				if( dhcOwner.IsMirrored )
				{
					if( rcbutton.Right > CaptionRect.Width - DEF_ICON_INDENT - DEF_ICON_SIZE )
					{
						return Rectangle.Empty;
					}
					else
					{
						return new Rectangle( 
							CaptionRect.Width - DEF_ICON_INDENT - DEF_ICON_SIZE, 
							DEF_ICON_INDENT, 
							DEF_ICON_SIZE, 
							DEF_ICON_SIZE );
					}
				}
				else
				{
					if( rcbutton.Left <	DEF_ICON_INDENT + DEF_ICON_SIZE )
					{
						return Rectangle.Empty;
					}
					else
					{
						return new Rectangle( 
							DEF_ICON_INDENT, 
							DEF_ICON_INDENT, 
							DEF_ICON_SIZE, 
							DEF_ICON_SIZE );
					}
				}
			}
		}

		public static int CaptionHeight
		{
			get { return (DockingManager.ftSysInfoMenuFont.Height+3); }
		}

		public static int CaptionButtonHeight
		{
			get { return CaptionPainter.CaptionHeight-4; }
		}

		public static int CaptionButtonWidth
		{
			get { return CaptionPainter.CaptionHeight-2; }
		}


		#endregion

		#region Constructors

		public CaptionPainter(DockHostController dhc)
		{
			this.dhcOwner = dhc;

			DockingManager dmgr = dhc.DockingManager;
			if( dmgr != null )
			{
				this.labelAlignment = dmgr.DockLabelAlignment;
			}

			if(XPThemes.IsThemedOS)
			{
				this.tdExplorerBar = new ThemedControlDrawing(ThemedControls.EXPLORERBAR);
				this.tdWindow = new ThemedControlDrawing(ThemedControls.WINDOW);
			}

			toolTip.AutoPopDelay = 5000;
			toolTip.InitialDelay = 1000;
			toolTip.ReshowDelay = 500;
			toolTip.ShowAlways = true;

			dhc.HostControl.SystemColorsChanged += new EventHandler(this.Owner_SystemColorsChanged);
		}


		#endregion

		#region Public methods

		public virtual CaptionHitTest HitTest(MouseAction action, Point pt)
		{
			// Ignore caption button state updates in design mode
			if( (this.dhcOwner.DockingManager.DesignMode == true) && (CaptionRect.Contains(pt) == true) )
				return CaptionHitTest.CaptionRect;

			CaptionHitTest retval = CaptionHitTest.None;

			if(CaptionRect.Contains(pt) == true)
				retval = CaptionHitTest.CaptionRect;

			switch(action)
			{
				case MouseAction.LBtnDown:
				{
					nCaptionButtonIndex = -1;
					for( int i = 0; i < Buttons.Count; i++ )
					{
						Rectangle rect = GetCaptionButtonBounds(i);
						if( rect.Contains(pt) )
						{
							retval = CaptionHitTest.ButtonDown;
							nCaptionButtonIndex = i;
							btnPressed = Buttons[i];
							dhcOwner.HostControl.Invalidate( CaptionRect );
							break;
						}
					}

					break;
				}

				case MouseAction.LBtnUp:
				{
					nCaptionButtonIndex = -1;
					for( int i = 0; i < Buttons.Count; i++ )
					{
						Rectangle rect = GetCaptionButtonBounds(i);
						if( rect.Contains(pt) )
						{
							retval = CaptionHitTest.ButtonUp;
							nCaptionButtonIndex = i;
							btnPressed = null;
							dhcOwner.HostControl.Invalidate( CaptionRect );
							break;
						}
					}
					break;
				}

				case MouseAction.LBtnDblClk:
				{
					if(CaptionRect.Contains(pt))
						retval = CaptionHitTest.CaptionRect;
					break;
				}

				case MouseAction.RBtnUp:
				{
					if(CaptionRect.Contains(pt))
						retval = CaptionHitTest.CaptionRect;
					break;
				}

				case MouseAction.MouseMove:
				{
					nCaptionButtonIndex = -1;
					for( int i = 0; i < Buttons.Count; i++ )
					{
						Rectangle rect = GetCaptionButtonBounds(i);
						if( rect.Contains(pt) )
						{
							if( btnPressed != Buttons[i] )
							{
								btnHover = Buttons[i];
								nCaptionButtonIndex = i;
								retval = CaptionHitTest.ButtonRect;
								dhcOwner.HostControl.Invalidate( CaptionRect );
								break;
							}
						}
					}
					if( nCaptionButtonIndex < 0 )
					{
						btnHover = null;
						btnPressed = null;
						dhcOwner.HostControl.Invalidate( CaptionRect );
						retval = CaptionRect.Contains( pt )? CaptionHitTest.CaptionRect: CaptionHitTest.None;
					}
					break;
				}

				case MouseAction.MouseLeave:
				{
					if(this.btnHover != null)
					{
						this.dhcOwner.HostControl.Invalidate( CaptionRect );
						this.btnHover = null;
					}
					break;
				}
				default:
					break;
			}

			return retval;
		}

		public virtual Rectangle GetButtonImageBounds(CaptionButtonType buttonType)
		{
			if (dhcOwner.DockingManager.ThemesEnabled &&
				(buttonType == CaptionButtonType.Menu || buttonType == CaptionButtonType.Pin))
			{
				return new Rectangle(0, 0, DEF_BUTTON_THEMED_SIZE, DEF_BUTTON_THEMED_SIZE);
			}
			else
			{
				return new Rectangle(0, 0, CaptionButtonWidth, CaptionButtonHeight);
			}
		}

		public virtual Rectangle GetCaptionButtonBounds( int index )
		{
			int nLeft;
			nLeft = index * ( CaptionButtonWidth + DEF_BUTTON_SPACE_BETWEEN ) + DEF_BUTTON_SIDE_INDENT;
			if( !dhcOwner.IsMirrored )
			{
				nLeft = CaptionRect.Width - CaptionButtonWidth - nLeft;
			}
            Rectangle rcbounds = 
                new Rectangle(nLeft, DEF_BUTTON_TOP_INDENT, CaptionButtonWidth, CaptionPainter.CaptionButtonHeight);

            if (CaptionRect.Contains(rcbounds))
            {
                return rcbounds;
            }
            else
            {
                return Rectangle.Empty;
            }
		}

		protected virtual void SetCaptionButtonIndex( Point pt )
		{
			for( int i = 0; i < Buttons.Count; i++ )
			{
				if( GetCaptionButtonBounds(i).Contains( pt ) )
				{
					nCaptionButtonIndex = i;
				}
			}
			nCaptionButtonIndex = -1;
		}

		protected internal void ResetCaptonButtonsHitTest()
		{
			this.btnHover = null;
			this.btnPressed = null;
		}

		protected internal void RefreshCaptionButtonsCollection()
		{
			m_buttons.Clear();
			if( dhcOwner.HostControl != null && dhcOwner.HostControl.Controls.Count > 0 )
			{
				CaptionButtonsCollection captionButtons = dhcOwner.DockingManager.GetCustomCaptionButtons(dhcOwner.HostControl.Controls[0]);
				if (captionButtons != null)
				{
					bool bRestoreButtonExists = captionButtons.ContainsButtonType(CaptionButtonType.Restore);
					for (int i = 0; i < captionButtons.Count; i++)
					{
						CaptionButton button = captionButtons[i];
						switch (button.Type)
						{
							case CaptionButtonType.Close:
								if (dhcOwner.CloseButtonVisibility)
								{
									m_buttons.Add(button);
								}
								break;

							case CaptionButtonType.Pin:
								if (!dhcOwner.Floating && dhcOwner.AutoHideButtonVisibility)
								{
									m_buttons.Add(button);
								}
								break;

							case CaptionButtonType.Menu:
								if (dhcOwner.MenuButtonVisiblity)
								{
									m_buttons.Add(button);
								}
								break;

							case CaptionButtonType.Maximize:
								if (dhcOwner.MaximizeButtonVisibility)
								{
									if (!bRestoreButtonExists || (bRestoreButtonExists && !dhcOwner.Maximized))
									{
										m_buttons.Add(button);
									}
								}
								break;

							case CaptionButtonType.Restore:
								if (dhcOwner.MaximizeButtonVisibility)
								{
									if (bRestoreButtonExists && dhcOwner.Maximized)
									{
										m_buttons.Add(button);
									}
								}
								break;
							case CaptionButtonType.Custom:
								m_buttons.Add(button);
								break;

						}
					}
				}
			}
		}

		public virtual int GetCaptionButtonIndex()
		{
			return nCaptionButtonIndex;
		}

		public virtual CaptionButton GetHitButton()
		{
			if( nCaptionButtonIndex >= 0 && nCaptionButtonIndex < Buttons.Count )
			{
				return Buttons[nCaptionButtonIndex];
			}
			else
			{
				return null;
			}
		}

		public virtual CaptionButton GetButtonAt( Point pt )
		{
			for( int i = 0; i < Buttons.Count; i++ )
			{
				if( GetCaptionButtonBounds(i).Contains(pt) )
				{
					return Buttons[i];
				}
			}
			return null;
		}

		public virtual void PaintCaption(Graphics gph, Pen pen)
		{
			bool bhilight = this.DrawWithHighlight();
			if(bhilight == true)
			{
				if(this.dhcOwner.DockingManager.DHCInFocus != this.dhcOwner)
					this.dhcOwner.DockingManager.DHCInFocus = this.dhcOwner;
			}

			Rectangle rccaption = this.CaptionRect;

			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.dhcOwner.DockingManager.ThemesEnabled)
			{
				int csstate = ThemeStates.CS_INACTIVE;
				if(bhilight == true)
					csstate = ThemeStates.CS_ACTIVE;

				this.tdWindow.DrawThemeBackground(gph, ThemeParts.WP_SMALLCAPTION, csstate, rccaption);

				DrawImage( gph );

				int flags = 0x00008000|0x00000004|0x00000020; /*DT_SND_ELLIPSIS|DT_VCENTER|DY_SINGLELINE*/
				if( this.labelAlignment == DockLabelAlignmentStyle.Default )
				{
					flags |= ( this.dhcOwner.IsMirrored )? 0x00020000 | 0x00000002: 0x0000000;
				}
				else
				{
					if( this.dhcOwner.IsMirrored )
						flags |= 0x00020000; /*DT_RTLREADING*/
					switch (this.labelAlignment)
					{
						case DockLabelAlignmentStyle.Left:
							flags |= 0x0000000; /*DT_LEFT*/
							break;
						case DockLabelAlignmentStyle.Right:
							flags |= 0x0000002; /*DT_RIGHT*/
							break;
						case DockLabelAlignmentStyle.Center:
							flags |= 0x0000001; /*DT_CENTER*/
							break;
					}
				}
				this.tdWindow.DrawThemeText( gph, ThemeParts.WP_SMALLCAPTION, csstate, this.dhcOwner.HostControl.Text, TextRect, flags, 0 );

				for( int i = 0; i < Buttons.Count; i++ )
				{
					CaptionButton button = Buttons[i];
					Rectangle rcButtonBounds = GetCaptionButtonBounds(i);

					if( rcButtonBounds == Rectangle.Empty )
					{
						continue;
					}

					Image buttonImage = null;
					if( button.ImageIndex >= 0 &&  dhcOwner.DockingManager.ImageList != null 
						&& button.ImageIndex < dhcOwner.DockingManager.ImageList.Images.Count )
					{
						buttonImage = dhcOwner.DockingManager.ImageList.Images[button.ImageIndex];
					}
					if( buttonImage != null )
					{
						Bitmap bmp = buttonImage as Bitmap;
						if( bmp != null && button.TransparentImageColor != Color.Transparent )
						{
							bmp.MakeTransparent( button.TransparentImageColor );
						}
						Rectangle rc = GetButtonImageBounds(button.Type);
						gph.DrawImage( buttonImage, rcButtonBounds, rc, GraphicsUnit.Pixel );
					}
					else
					{
						int themestate = 0;
						Rectangle rcimg = GetButtonImageBounds(button.Type);
						Bitmap bitmap = new Bitmap(rcimg.Width, rcimg.Height);
						Graphics bmpg = Graphics.FromImage(bitmap);
						int tp = 0;
						switch( button.Type )
						{
							case CaptionButtonType.Close:
								if( !dhcOwner.CloseButtonVisibility )
								{
									continue;
								}
								themestate = ThemeStates.CBS_NORMAL;
								if(this.btnPressed == button)
									themestate = ThemeStates.CBS_PUSHED;
								else if(this.btnHover == button)
									themestate = ThemeStates.CBS_HOT;
								tp = ThemeParts.WP_SMALLCLOSEBUTTON;
								break;

							case CaptionButtonType.Pin:
								if( dhcOwner.Floating || !dhcOwner.AutoHideButtonVisibility )
								{
									continue;
								}
								if(this.dhcOwner.AutoHideMode == true)
								{
									themestate = ThemeStates.EBHP_NORMAL;
									if(this.btnPressed == button)
										themestate = ThemeStates.EBHP_PRESSED;
									else if(this.btnHover == button)
										themestate = ThemeStates.EBHP_HOT;
								}
								else
								{
									themestate = ThemeStates.EBHP_SELECTEDNORMAL;
									if(this.btnPressed == button)
										themestate = ThemeStates.EBHP_SELECTEDPRESSED;
									else if(this.btnHover == button)
										themestate = ThemeStates.EBHP_SELECTEDHOT;
								}
								tp = ThemeParts.EBP_HEADERPIN;
								break;

							case CaptionButtonType.Menu:
								if( !dhcOwner.MenuButtonVisiblity )
								{
									continue;
								}
								themestate = ThemeStates.CBS_NORMAL;	
								if(this.btnPressed == button)
									themestate = ThemeStates.CBS_PUSHED;
								else if(this.btnHover == button)
									themestate = ThemeStates.CBS_HOT;
								tp = ThemeParts.EBP_NORMALGROUPEXPAND;
								break;

							case CaptionButtonType.Maximize:
							case CaptionButtonType.Restore:
								bool bRestoreButtonExists = dhcOwner.DockingManager.CaptionButtons.ContainsButtonType(CaptionButtonType.Restore);
								if( !dhcOwner.MaximizeButtonVisibility || button.Modified && bRestoreButtonExists)
								{
									continue;
								}
								themestate = ThemeStates.CBS_NORMAL;
								if( this.btnPressed == button )
									themestate = ThemeStates.CBS_PUSHED;
								else if( this.btnHover == button )
									themestate = ThemeStates.CBS_HOT;
								if( this.dhcOwner.Maximized && !bRestoreButtonExists || button.Type == CaptionButtonType.Restore )
									tp = ThemeParts.WP_RESTOREBUTTON;
								else
									tp = ThemeParts.WP_MAXBUTTON;
								break;
							case CaptionButtonType.Custom:
								if (button.ImageIndex < 0 || dhcOwner.DockingManager.ImageList == null
									|| button.ImageIndex >= dhcOwner.DockingManager.ImageList.Images.Count)
								{
									return;
								}
								break;
						}
						if (button.Type == CaptionButtonType.Close
							|| button.Type == CaptionButtonType.Maximize
							|| button.Type == CaptionButtonType.Restore)
						{
							tdWindow.DrawThemeBackground(bmpg, tp, themestate, rcimg);
						}
						else
						{
							tdExplorerBar.DrawThemeBackground(bmpg, tp, themestate, rcimg);
						}
						gph.DrawImage(bitmap, rcButtonBounds, rcimg, GraphicsUnit.Pixel);
						bmpg.Dispose();
						bitmap.Dispose();
					}
				}
			}
			else
			{
				Brush brbackground = null;
				Color clrforeground = Color.Empty;
				Font ftcaption = this.ftCaption;
				if(bhilight == true)
				{
					brbackground = BrushConverter.GetBrush( rccaption,  dhcOwner.DockingManager.ActiveCaptionBackground);
					clrforeground = dhcOwner.DockingManager.ActiveCaptionForeGround ;
					ftcaption = dhcOwner.DockingManager.ActiveCaptionFont;
				}
				else
				{
					brbackground = BrushConverter.GetBrush( rccaption,dhcOwner.DockingManager.InActiveCaptionBackground);
					clrforeground = dhcOwner.DockingManager.InActiveCaptionForeGround;
					ftcaption = dhcOwner.DockingManager.InActiveCaptionFont;
				}


				if (this.dhcOwner.HostControl.Controls.Count > 0)
				{
				// Create and initialize a ProvideGraphicsItemsEventArgs instance with the default graphics objects
					ProvideGraphicsItemsEventArgs pgiargs = new ProvideGraphicsItemsEventArgs(this.dhcOwner.HostControl.Controls[0], rccaption, bhilight);
					pgiargs.CaptionBackground = brbackground;
					pgiargs.CaptionForeground = clrforeground;
					pgiargs.CaptionFont = ftcaption;

					// Fire the ProvideGraphicsItems event to allow changes to be made to the default settings
					this.dhcOwner.DockingManager.FireProvideGraphicsItemsEvent(pgiargs);

					if ((pgiargs.CaptionBackground != null) && (pgiargs.CaptionBackground != brbackground))
					{
						brbackground = pgiargs.CaptionBackground;
					}
					if ((pgiargs.CaptionForeground != Color.Empty) && (pgiargs.CaptionForeground != clrforeground))
						clrforeground = pgiargs.CaptionForeground;
					if ((pgiargs.CaptionFont != null) && (pgiargs.CaptionFont != ftcaption))
						ftcaption = pgiargs.CaptionFont;
				}
					Brush brforeground = new SolidBrush(clrforeground);
				gph.FillRectangle(brbackground, rccaption);

				DrawImage( gph );

				if (dhcOwner.DockingManager.PaintBorders)
				{
					ControlPaint.DrawBorder(gph, rccaption, pen.Color, ButtonBorderStyle.Solid);
				}
				else
				{
					ControlPaint.DrawBorder(gph, rccaption, SystemColors.ControlDark, ButtonBorderStyle.Solid);
				}
				float firstCharWidth;
				if (dhcOwner.HostControl.Text.Trim() == String.Empty)
				{
					//Temproary fix, if we set empty string as docklabel, it throws exception
					firstCharWidth = gph.MeasureString("A", ftcaption).Width;
				}
				else
				{
					// Paint text if the width of TextRect is at least the width of the first character
					firstCharWidth = gph.MeasureString(dhcOwner.HostControl.Text.Substring(0, 1), ftcaption).Width;
				}
				if( this.TextRect.Width > firstCharWidth)
				{
					StringFormat sf = new StringFormat();
					sf.FormatFlags = StringFormatFlags.NoWrap;
					if (this.dhcOwner.DockingManager.IsMirrored)
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}
					switch( this.dhcOwner.DockingManager.DockLabelAlignment )
					{
						case DockLabelAlignmentStyle.Left:
							sf.Alignment = (this.dhcOwner.DockingManager.IsMirrored)? 
								StringAlignment.Far: StringAlignment.Near;
							break;
						case DockLabelAlignmentStyle.Center:
							sf.Alignment = StringAlignment.Center;
							break;
						case DockLabelAlignmentStyle.Right:
							sf.Alignment = (this.dhcOwner.DockingManager.IsMirrored)?
								StringAlignment.Near: StringAlignment.Far;
							break;
						default: 
							sf.Alignment = StringAlignment.Near;
							break;
					}
					sf.LineAlignment = StringAlignment.Center;
					sf.Trimming = StringTrimming.EllipsisCharacter;
					gph.DrawString(this.dhcOwner.HostControl.Text, ftcaption, brforeground, TextRect, sf);
                    brforeground.Dispose();
                    sf.Dispose();
				}
                if (!bhilight)
                    clrforeground = this.dhcOwner.DockingManager.InActiveCaptionButtonForeColor;
                else
                    clrforeground = this.dhcOwner.DockingManager.ActiveCaptionButtonForeColor;
				for( int i = 0; i < Buttons.Count; i++ )
				{
					CaptionButton button = Buttons[i];
					Rectangle rcbounds = GetCaptionButtonBounds(i);

					if( rcbounds == Rectangle.Empty )
					{
						continue;
					}

					Image buttonImage = null;
					Rectangle rect = Rectangle.Empty;
					if( button.ImageIndex >= 0 &&  dhcOwner.DockingManager.ImageList != null 
						&& button.ImageIndex < dhcOwner.DockingManager.ImageList.Images.Count )
					{
						buttonImage = dhcOwner.DockingManager.ImageList.Images[button.ImageIndex];
						if( buttonImage != null )
						{
							Bitmap bmp = buttonImage as Bitmap;
							if( bmp != null && button.TransparentImageColor != Color.Transparent )
							{
								bmp.MakeTransparent( button.TransparentImageColor );
							}
							rect = GetButtonImageBounds(button.Type);
						}
					}

					if(this.btnPressed == button)
						this.DrawPressedBorder(gph, rcbounds);
					else if(this.btnHover == button)
						this.DrawHoverBorder(gph, rcbounds);

					switch( button.Type )
					{
						case CaptionButtonType.Close:
							if( !dhcOwner.CloseButtonVisibility )
							{
								continue;
							}
							if( buttonImage == null )
								this.DrawCloseButton(gph, rcbounds, clrforeground);
							else
								gph.DrawImage( buttonImage, rcbounds, rect, GraphicsUnit.Pixel );
							break;

						case CaptionButtonType.Pin:
							if( dhcOwner.Floating || !dhcOwner.AutoHideButtonVisibility )
							{
								continue;
							}
							if( buttonImage == null )
								this.DrawAutoHideButton(gph, rcbounds, clrforeground);
							else
								gph.DrawImage( buttonImage, rcbounds, rect, GraphicsUnit.Pixel );
							break;

						case CaptionButtonType.Menu:
							if( !dhcOwner.MenuButtonVisiblity )
							{
								continue;
							}
							if( buttonImage == null )
								this.DrawMenuButton(gph, rcbounds, clrforeground);
							else
								gph.DrawImage( buttonImage, rcbounds, rect, GraphicsUnit.Pixel );
							break;

						case CaptionButtonType.Maximize:
						case CaptionButtonType.Restore:
							bool bRestoreButtonExists = dhcOwner.DockingManager.CaptionButtons.ContainsButtonType(CaptionButtonType.Restore);
							if( !dhcOwner.MaximizeButtonVisibility || button.Modified && bRestoreButtonExists )
							{
								continue;
							}
							if( buttonImage == null )
							{
								if (this.dhcOwner.Maximized && !bRestoreButtonExists || button.Type == CaptionButtonType.Restore)
									this.DrawRestoreButton(gph, rcbounds, clrforeground);
								else
									this.DrawMaximizeButton(gph, rcbounds, clrforeground);
							}
							else
							{
								gph.DrawImage( buttonImage, rcbounds, rect, GraphicsUnit.Pixel );
							}
							break;
						case CaptionButtonType.Custom:
							if( buttonImage != null )
							{
								gph.DrawImage( buttonImage, rcbounds, rect, GraphicsUnit.Pixel );
							}
							break;
					}
				}
			}
		}


		#endregion

		#region Protected methods

		protected void Owner_SystemColorsChanged(object sender, EventArgs e)
		{
			this.ftCaption = SystemInformation.MenuFont;
		}

		protected internal void ProcessMouseMove(MouseEventArgs e)
		{
			if( dhcOwner.DockingManager.ShowToolTips )
			{
				String toolTipText = "";
				CaptionButton button = null;
				#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				ToolTipInfo tooltipInfo = null;
				#endif

				bool bRestoreButtonExists = dhcOwner.DockingManager.CaptionButtons.ContainsButtonType(CaptionButtonType.Restore);

				if( dhcOwner.RendererStyle == VisualStyle.Default )
				{
					button = GetButtonAt( new Point(e.X, e.Y) );
					if( button != null )
					{
						if( button.ToolTip == "" 
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
							&& !this.dhcOwner.DockingManager.EnableSuperToolTip
#endif
							)
						{
							switch( button.Type )
							{
								case CaptionButtonType.Close:
									toolTipText = dhcOwner.DockingManager.GetCloseButtonToolTip();
									break;
								case CaptionButtonType.Pin:
									toolTipText = dhcOwner.DockingManager.GetAutoHideButtonToolTip();
									break;
								case CaptionButtonType.Menu:
									toolTipText = dhcOwner.DockingManager.GetMenuButtonToolTip();
									break;
								case CaptionButtonType.Maximize:
								case CaptionButtonType.Restore:
									if (dhcOwner.Maximized && !bRestoreButtonExists || button.Type == CaptionButtonType.Restore)
										toolTipText = dhcOwner.DockingManager.GetRestoreButtonToolTip();
									else
										toolTipText = dhcOwner.DockingManager.GetMaximizeButtonToolTip();
									break;
							}
						}
						else
						{
							if( button.Type == CaptionButtonType.Maximize && !bRestoreButtonExists && dhcOwner.Maximized || button.Type == CaptionButtonType.Restore )
								toolTipText = dhcOwner.DockingManager.GetRestoreButtonToolTip();
							else
								toolTipText = button.ToolTip;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
							if( this.dhcOwner.DockingManager.EnableSuperToolTip )
							{
								if (button.Type == CaptionButtonType.Maximize && !bRestoreButtonExists && dhcOwner.Maximized || button.Type == CaptionButtonType.Restore)
								{
									tooltipInfo = new ToolTipInfo( button.SuperToolTipInfo );
									tooltipInfo.Body.Text = toolTipText;
									tooltipInfo.Footer.Text = string.Empty;
									tooltipInfo.Header.Text = string.Empty;
								}
								else
									tooltipInfo = button.SuperToolTipInfo;
							}
#endif
						}
					}
				}
				else
				{
					HitTestArea area = dhcOwner.Renderer.HitTest(MouseButtons.None, new	Point(e.X, e.Y));

					if( area == HitTestArea.Button )
					{
						button = dhcOwner.Renderer.GetHitButton();
						if( button != null )
						{
							if( button.ToolTip == ""
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
								&& !dhcOwner.DockingManager.EnableSuperToolTip 
#endif
								)
							{
								switch( button.Type )
								{
									case CaptionButtonType.Close:
										toolTipText = dhcOwner.DockingManager.GetCloseButtonToolTip();
										break;
									case CaptionButtonType.Pin:
										toolTipText = dhcOwner.DockingManager.GetAutoHideButtonToolTip();
										break;
									case CaptionButtonType.Menu:
										toolTipText = dhcOwner.DockingManager.GetMenuButtonToolTip();
										break;
									case CaptionButtonType.Maximize:
										if(!bRestoreButtonExists && dhcOwner.Maximized )
											toolTipText = dhcOwner.DockingManager.GetRestoreButtonToolTip();
										else
											toolTipText = dhcOwner.DockingManager.GetMaximizeButtonToolTip();
										break;
									case CaptionButtonType.Restore:
										toolTipText = dhcOwner.DockingManager.GetRestoreButtonToolTip();
										break;

								}
							}
							else
							{
								if (button.Type == CaptionButtonType.Maximize && !bRestoreButtonExists && dhcOwner.Maximized || button.Type == CaptionButtonType.Restore)
									toolTipText = dhcOwner.DockingManager.GetRestoreButtonToolTip();
								else
									toolTipText = button.ToolTip;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
								if( this.dhcOwner.DockingManager.EnableSuperToolTip )
								{
									if (button.Type == CaptionButtonType.Maximize && !bRestoreButtonExists && dhcOwner.Maximized || button.Type == CaptionButtonType.Restore)
									{
										tooltipInfo = new ToolTipInfo( button.SuperToolTipInfo );
										tooltipInfo.Body.Text = toolTipText;
										tooltipInfo.Footer.Text = string.Empty;
										tooltipInfo.Header.Text = string.Empty;
									}
									else
										tooltipInfo = button.SuperToolTipInfo;
								}
#endif
							}
						}
					}
				}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				if( this.dhcOwner.DockingManager.EnableSuperToolTip )
				{
					SuperToolTip dockingToolTip = this.dhcOwner.DockingManager.SuperToolTip;

					if( dockingToolTip != null )
					{
                        dockingToolTip.ToolTipDuration = this.dhcOwner.DockingManager.ToolTipInterval/1000;
                        if (this.dhcOwner.DockingManager.UseBalloonStyleToolTip)
                        {
                            dockingToolTip.Style = SuperToolTip.SuperToolTipStyle.Balloon;
                        }
                        else
                        {
                            dockingToolTip.Style = SuperToolTip.SuperToolTipStyle.Normal;
                        }
						if( button != null )
						{
							bool setRestore = false;
							if (button.Type == CaptionButtonType.Maximize && !bRestoreButtonExists && dhcOwner.Maximized || button.Type == CaptionButtonType.Restore)
							{ 
								ToolTipInfo tti = dockingToolTip.GetToolTip( dhcOwner.HostControl );
								if( tti != null && toolTipText == tti.Body.Text )
									setRestore = true;
							}
							if( tooltipInfo == null )
								button.SuperToolTipInfo = tooltipInfo = new ToolTipInfo();
							if( tooltipInfo.Body.Text == null && tooltipInfo.Header.Text == null && tooltipInfo.Footer.Text == null )
							{
								if( toolTipText == "" )
								{
									dockingToolTip.SetToolTip( dhcOwner.HostControl, null );
									return;
								}

								tooltipInfo.Body.Text = toolTipText;
							}

							if( dockingToolTip.GetToolTip( dhcOwner.HostControl ) != tooltipInfo && !setRestore)
							{
								toolTip.SetToolTip( dhcOwner.HostControl, string.Empty );
								dockingToolTip.Hide();
								dockingToolTip.SetToolTip( dhcOwner.HostControl, tooltipInfo );
							}
						}
						else
							dockingToolTip.SetToolTip( dhcOwner.HostControl, null );
					}
				} 
				else
#endif
				if( toolTipText != toolTip.GetToolTip(dhcOwner.HostControl) )
				{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					SuperToolTip dockingToolTip = this.dhcOwner.DockingManager.SuperToolTip;

					if( dockingToolTip != null )
						dockingToolTip.SetToolTip( dhcOwner.HostControl, null );
#endif
                    if (this.toolTip.Active)
                        this.toolTip.Active = false;
                    this.toolTip.IsBalloon = this.dhcOwner.DockingManager.UseBalloonStyleToolTip;
                    this.toolTip.AutoPopDelay = this.dhcOwner.DockingManager.ToolTipInterval;
					toolTip.SetToolTip(dhcOwner.HostControl, toolTipText);
                    this.toolTip.Active = true;
				}
			}
		}

		protected internal bool DrawWithHighlight()
		{
			bool bhilight = this.dhcOwner.HostControl.ContainsFocus;
			if((this.dhcOwner.DockingManager.DesignMode == true) && (bhilight == false))
			{
				// If the owner control is the PrimarySelected control then set bhilight to be true.
				// We need to do this because controls are normally not accorded focus in design time.
				ISelectionService iss = this.dhcOwner.DockingManager.GetServiceFromMgr(typeof(ISelectionService)) as ISelectionService;
				if((iss != null) && (iss.PrimarySelection != null))
				{
					Object primary = iss.PrimarySelection;
					if(primary is Control && dhcOwner.HostControl.Controls.Count > 0)
					{
						if(this.dhcOwner.HostControl.Controls[0] == primary)
							bhilight = true;
					}
				}
			}
			return bhilight;
		}

		protected virtual void DrawPressedBorder(Graphics gph, Rectangle rcbtn)
		{
			rcbtn.Inflate(-1,-2);
			ControlPaint.DrawBorder3D(gph, rcbtn, Border3DStyle.SunkenInner|Border3DStyle.Adjust, Border3DSide.Left|Border3DSide.Top);
			ControlPaint.DrawBorder3D(gph, rcbtn, Border3DStyle.SunkenInner|Border3DStyle.Adjust, Border3DSide.Right|Border3DSide.Bottom);
		}

		protected virtual void DrawHoverBorder(Graphics gph, Rectangle rcbtn)
		{
			rcbtn.Inflate(-1,-2);
			ControlPaint.DrawBorder3D(gph, rcbtn, (this.DrawWithHighlight()?Border3DStyle.RaisedOuter:Border3DStyle.RaisedInner)|Border3DStyle.Adjust,
				Border3DSide.Left|Border3DSide.Top);
			ControlPaint.DrawBorder3D(gph, rcbtn, Border3DStyle.RaisedOuter|Border3DStyle.Adjust, Border3DSide.Right|Border3DSide.Bottom);
		}

		protected virtual void DrawCloseButton(Graphics gph, Rectangle rcbtn, Color clrbtn)
		{
			Pen closepen = new Pen(clrbtn, 1);
			gph.DrawLine(closepen, new Point(rcbtn.Left+4,rcbtn.Top+3), new Point(rcbtn.Right-5,rcbtn.Bottom-4));
			gph.DrawLine(closepen, new Point(rcbtn.Left+4,rcbtn.Top+2), new Point(rcbtn.Right-4,rcbtn.Bottom-4));
			gph.DrawLine(closepen, new Point(rcbtn.Left+5,rcbtn.Top+2), new Point(rcbtn.Right-4,rcbtn.Bottom-5));
			gph.DrawLine(closepen, new Point(rcbtn.Left+4,rcbtn.Bottom-5), new Point(rcbtn.Right-5,rcbtn.Top+2));
			gph.DrawLine(closepen, new Point(rcbtn.Left+4,rcbtn.Bottom-4), new Point(rcbtn.Right-4,rcbtn.Top+2));
			gph.DrawLine(closepen, new Point(rcbtn.Left+5,rcbtn.Bottom-4), new Point(rcbtn.Right-4,rcbtn.Top+3));
			closepen.Dispose();
		}

		protected virtual void DrawAutoHideButton(Graphics gph, Rectangle rcbtn, Color clrbtn)
		{
			Pen autohidepen = new Pen(clrbtn, 1);
			Point ptcenter = new Point(rcbtn.Left+rcbtn.Width/2, rcbtn.Top+rcbtn.Height/2);

			if(this.dhcOwner.AutoHideMode == true)
			{
				Matrix rotatematrix = new Matrix();
				rotatematrix.RotateAt(90, ptcenter, MatrixOrder.Append);
				gph.Transform = rotatematrix;
			}

			gph.DrawLine(autohidepen, new Point(rcbtn.Left+5, ptcenter.Y+1), new Point(rcbtn.Right-4, ptcenter.Y+1));
			gph.DrawLine(autohidepen, new Point(rcbtn.Left+6, ptcenter.Y), new Point(rcbtn.Left+6, rcbtn.Top+2));
			gph.DrawLine(autohidepen, new Point(rcbtn.Left+6, rcbtn.Top+2), new Point(rcbtn.Right-5, rcbtn.Top+2));
			gph.DrawLine(autohidepen, new Point(rcbtn.Right-5, rcbtn.Top+2), new Point(rcbtn.Right-5, ptcenter.Y));
			gph.DrawLine(autohidepen, new Point(rcbtn.Right-6, rcbtn.Top+2), new Point(rcbtn.Right-6, ptcenter.Y));
			gph.DrawLine(autohidepen, new Point(ptcenter.X+1, ptcenter.Y+2), new Point(ptcenter.X+1, rcbtn.Bottom-3));

			if(this.dhcOwner.AutoHideMode == true)
				gph.ResetTransform();

			autohidepen.Dispose();
		}

		protected virtual void DrawMenuButton( Graphics gph, Rectangle rcBth, Color clBtn )
		{
			Pen btnPen = new Pen(clBtn, 1);
			int startx = rcBth.Left + 4;
			int starty = rcBth.Top + 6;
			for( int i = 0; i < 4; i++ )
			{
				gph.DrawLine(btnPen, startx + i, starty + i, startx + 7 - i, starty + i);
			}
			btnPen.Dispose();
		}

		protected virtual void DrawMaximizeButton( Graphics gph, Rectangle rcBth, Color clBtn )
		{
			Pen btnPen = new Pen(clBtn, 1);
			int borderOffset = 3;			

			Point topleft = new Point(rcBth.Left + 3, rcBth.Top + 3);

			int width = rcBth.Right - borderOffset - topleft.X;
			int height = rcBth.Bottom - borderOffset - topleft.Y;
			Point topright = new Point(topleft.X + width, topleft.Y);
			Rectangle window = new Rectangle(topleft, new Size(width, height));

			gph.DrawRectangle(btnPen, window);
			gph.DrawLine(btnPen, new Point(topleft.X, topleft.Y+1), new Point(topright.X, topright.Y+1));
			
			btnPen.Dispose();
		}

		protected virtual void DrawRestoreButton( Graphics g, Rectangle rcBth, Color clBtn )
		{
			Pen btnPen = new Pen(clBtn, 1);
			int borderOffset = 3;

			Point frontTopLeft = new Point(rcBth.Left + borderOffset, rcBth.Top + borderOffset);
			Point rearBottomRight = new Point(rcBth.Right - borderOffset, rcBth.Bottom - borderOffset);
			int width = ( int )( ( rearBottomRight.X - frontTopLeft.X ) / 3 * 2 );
			int height = ( int )( ( rearBottomRight.Y - frontTopLeft.Y ) / 7 * 5 );
			Point rearBottomLeft = new Point(rearBottomRight.X-width, rearBottomRight.Y);
			Point rearTopRight = new Point(rearBottomRight.X, rearBottomRight.Y - height);
			Point rearTopLeftB = new Point(rearBottomLeft.X, rearBottomLeft.Y - (int)(height*0.5));
			Point rearTopLeftR = new Point(rearTopRight.X - (int)(width/2), rearTopRight.Y);

			Rectangle front = new Rectangle(frontTopLeft, new Size(width, height));

			g.DrawRectangle(btnPen, front);
			g.DrawLine(btnPen, rearBottomRight, rearBottomLeft);
			g.DrawLine(btnPen, rearBottomRight, rearTopRight);
			g.DrawLine(btnPen, rearBottomLeft, rearTopLeftB);
			g.DrawLine(btnPen, rearTopRight, rearTopLeftR);

			btnPen.Dispose();
		}

		protected virtual void DrawImage( Graphics gph  )
		{
			Image image = null;
			if (dhcOwner.DockingManager.ImageList!=null&&dhcOwner.ImageIndex >= 0 && dhcOwner.DockingManager.ImageList.Images.Count > dhcOwner.ImageIndex)
			{
				image = dhcOwner.DockingManager.ImageList.Images[dhcOwner.ImageIndex];
			}
			if( image != null && ImageRect != Rectangle.Empty )
			{
				gph.DrawImage( image, ImageRect );
			}
		}

		protected override void Dispose(bool disposing)
		{
			if((disposing == true) && (this.dhcOwner != null))
			{
				this.dhcOwner.HostControl.SystemColorsChanged -= new EventHandler(this.Owner_SystemColorsChanged);

				if(this.tdExplorerBar != null)
					this.tdExplorerBar.Dispose();
				if(this.tdWindow != null)
					this.tdWindow.Dispose();
				this.dhcOwner = null;
				if( this.toolTip != null )
				{
					this.toolTip.Dispose();
					this.toolTip = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
	#endregion
	internal class BrushConverter
	{
		internal static Brush GetBrush(Rectangle r, BrushInfo brush)
		{
			switch (brush.Style)
			{
				case BrushStyle.None:

				case BrushStyle.Solid:
					return new SolidBrush(brush.BackColor);

				case BrushStyle.Pattern:
					if (brush.PatternStyle == PatternStyle.None)
						goto case BrushStyle.Solid;
					//g.FillRectangle(brush.PatternedBrush, r);
					return new HatchBrush((HatchStyle)(brush.PatternStyle - 1), 
						brush.ForeColor, brush.BackColor);

				case BrushStyle.Gradient:
					if (brush.GradientStyle == GradientStyle.None)
						goto case BrushStyle.Solid;
					return GetGradientBrush (r, brush.GradientStyle, (Color[])brush.GradientColors.ToArray(typeof(Color)));

				default :
					return SystemBrushes.Control;
			}
		}
		internal static Brush GetGradientBrush(Rectangle r, GradientStyle gradientStyle, Color[] colors)
		{
			Brush br = null;
			PathGradientBrush pbr = null;
			LinearGradientBrush lgbr = null;
			GraphicsPath path = null;
			switch (gradientStyle)
			{
				case GradientStyle.ForwardDiagonal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.ForwardDiagonal);
					lgbr.InterpolationColors =  GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.BackwardDiagonal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.BackwardDiagonal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.Horizontal:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.Horizontal);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.Vertical:
					lgbr = new LinearGradientBrush(r, Color.Empty, Color.Empty, LinearGradientMode.Vertical);
					lgbr.InterpolationColors = GetGenericColorBlend(colors);
					br = lgbr;
					break;

				case GradientStyle.PathRectangle:
					path = new GraphicsPath();
					path.AddRectangle(r);
					break;

				case GradientStyle.PathEllipse:
					path = new GraphicsPath();
					r.Inflate(r.Width / 4, r.Height / 4);
					path.AddEllipse(r);
					break;

					//				case GradientStyle.PathPie:
					//					path = new GraphicsPath();
					//					path.AddPie(r, -30, 200);
					//					break;
			}

			if (path != null && path.PointCount > 0)
			{
				pbr = new PathGradientBrush(path);
				pbr.CenterColor = colors[colors.Length - 1];
				Color[] scolors = new Color[colors.Length - 1];
				int j = 0;
				for (int i = colors.Length - 2; i >= 0; i--)
					scolors[j++] = colors[i];
				pbr.SurroundColors = scolors;
				//pbr.CenterPoint = new Point(r.X, r.Y+r.Height/2);
				br = pbr;
                path.Dispose();
                lgbr.Dispose();
                pbr.Dispose();
			}

			return br;
		}
		private static ColorBlend GetGenericColorBlend(Color[] colors)
		{
			ColorBlend blend = new ColorBlend(colors.Length);
			Array.Reverse(colors);
			blend.Colors = colors;
			float[] positions = new float[colors.Length];

			positions[0] = 0f;
			float position = 0f;
			for (int i = 1; i < colors.Length - 1; i++)
			{
				position += 1f / (colors.Length - 1f);
				positions[i] = position;
			}
			positions[positions.Length - 1] = 1f;
			blend.Positions = positions;
			return blend;
		}

	}
