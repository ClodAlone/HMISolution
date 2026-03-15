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
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Represents renderer of the Office2003 visual style for docked CommandBar.
	/// </summary>
	internal class CommandBarRendererOffice2003 : CommandBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarRendererOffice2003( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Font textFont = this.CmdBar.Font;
			Color foreColor = this.CmdBar.ForeColor;
			CommandBarPainter.PaintDockedText( g, rect, text, textFont, foreColor, bRTL, bVertical );
		}
		/// <summary>
		/// Draws background of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			if( !IsMainCommandBar() )
			{
				CommandBarPainter.PaintDockedBackground( g, bRTL, bVertical, rect,
					Office2003Colors.MenuMarginColorLight,
					Office2003Colors.MenuMarginColorDark,
					Office2003Colors.ControlBorderColorDark );
			}
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color chevronColor = this.CmdBar.ChevronColor;
			Color startColor = Color.Empty;
			Color endColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					startColor = ( bVertical && bRTL ) ? Office2003Colors.MenuItemHotColorDark : 
						Office2003Colors.MenuItemHotColorLight;
					endColor = ( bVertical && bRTL ) ? Office2003Colors.MenuItemHotColorLight : 
						Office2003Colors.MenuItemHotColorDark;
					break;
				}
				case CBButtonState.Pressed :
				{
					startColor = ( bVertical && bRTL ) ? Office2003Colors.MenuItemPressedColorLight : 
						Office2003Colors.MenuItemPressedColorDark;
					endColor = ( bVertical && bRTL ) ? Office2003Colors.MenuItemPressedColorDark : 
						Office2003Colors.MenuItemPressedColorLight;
					break;
				}
				default : 
				{
					startColor = ( bVertical && bRTL ) ? Office2003Colors.CommandBarDropDownColorDark : 
						Office2003Colors.CommandBarDropDownColorLight;
					endColor = ( bVertical && bRTL ) ? Office2003Colors.CommandBarDropDownColorLight : 
						Office2003Colors.CommandBarDropDownColorDark;
					break;
				}
			}

			CommandBarPainter.PaintDropDownButton( g, rect, bRTL, bVertical, bShowChevron,
				startColor, endColor, chevronColor, bShowArrow );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, bVertical, 
				Color.White, Office2003Colors.ControlGripperColor );
		}

		#endregion
	}

	/// <summary>
	/// Represents renderer of the Office2003 visual style for floating CommandBar.
	/// </summary>
	internal class CommandBarFloatingRendererOffice2003 : CommandBarFloatingRenderer
	{
		#region Class Initialize/Finalize Methods
		public CommandBarFloatingRendererOffice2003( CommandBar commandBar ) : base( commandBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawText( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			string text = this.CmdBar.Text;
			Color foreColor = Color.White;
			Font textFont = this.CmdBar.GetFloatinCaptionFont();
			CommandBarPainter.PaintFloatingText( g, rect, text, textFont, foreColor, bRTL );
		}
		/// <summary>
		/// Draws background of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawBackground( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			Color backColor = Office2003Colors.FloatingCommandBarCaptionColor;
			Color startColor = Office2003Colors.MenuMarginColorLight;
			Color endColor = Office2003Colors.MenuMarginColorDark;
			Color borderColor = Office2003Colors.MenuMarginColorLight;

			Rectangle captionRect = this.CmdBar.CaptionRect;
			Rectangle barRect = new Rectangle( rect.Left + 1, 
				rect.Top + captionRect.Height + 1, 
				rect.Width - 2, rect.Height - 2 );

			CommandBarPainter.PaintFloatingBackground( g, rect, barRect, 
				backColor, borderColor, startColor, endColor );
		}

		/// <summary>
		/// Draws DropBownButton of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawDropDown( Graphics g, Rectangle rect, bool bRTL, bool bVertical,
			CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					if( Office2003Colors.UseThemedColors )
					{
						borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
						fillColor = Office2003Colors.SelColor;
						buttonColor = SystemColors.ControlText;
					}
					else
					{
						borderColor = MenuColors.SelBorderColor;
						fillColor = MenuColors.SelColor;
						buttonColor = SystemColors.ControlText;
					}
					break;
				}
				case CBButtonState.Pressed :
				{
					if( Office2003Colors.UseThemedColors )
					{
						borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
						fillColor = Office2003Colors.FloatingCommandBarItemPressedColor;
						buttonColor = SystemColors.ControlText;
					}
					else
					{
						borderColor = MenuColors.DropDownBorderColor;
						fillColor = SystemColors.Control;
						buttonColor = SystemColors.ControlText;
					}
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingDropDownButton( g, rect, borderColor, fillColor, buttonColor );
		}
		/// <summary>
		/// Draws gripper of the CommandBar with Office2003 visual style.
		/// </summary>
		protected override void DrawGripper( Graphics g, Rectangle rect, bool bRTL, bool bVertical )
		{
			// doesn't need draw gripper
		}
		/// <summary>
		/// Draws close button of the floating CommandBar.
		/// </summary>
		protected override void DrawCloseButton( Graphics g, Rectangle rect, CBButtonState state )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;
			Color buttonColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
					fillColor = Office2003Colors.SelColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = Office2003Colors.ControlBorderColorDark;
					fillColor = Office2003Colors.PressedSelColor;
					buttonColor = SystemColors.ControlText;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					buttonColor = Color.White;
					break;
				}
			}

			CommandBarPainter.PaintFloatingCloseButton( g, rect, borderColor, fillColor, buttonColor );
		}
		#endregion
	}

	/// <summary>
	/// Represents renderer of the Office2003 visual style for docked ControlBar.
	/// </summary>
	internal class ControlBarRendererOffice2003 : ControlBarRenderer
	{
		#region Class Initialize/Finalize Methods
		public ControlBarRendererOffice2003( ControlBar controlBar ) : base( controlBar )
		{
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Draws text of the ControlBar with Office2003 visual style.
		/// </summary>
		protected override void DrawText(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			string text = this.ControlBar.Text;
			Color foreColor = this.ControlBar.ForeColor;
			Font textFont = this.ControlBar.Font;
			CommandBarPainter.PaintControlBarText( g, rect, text, textFont, foreColor, bRTL );
		}

		/// <summary>
		/// Draws background of the ControlBar with Office2003 visual style.
		/// </summary>
		protected override void DrawBackground(Graphics g, Rectangle rect, bool bRTL, bool bVerticalt)
		{
			Color borderColor = Office2003Colors.DockBarColorLight;
			Color lightColor = Office2003Colors.MenuMarginColorLight;
			Color darkColor = Office2003Colors.MenuMarginColorDark;
			Color activeLightColor = Office2003Colors.MenuItemHotColorLight;
			Color activeDarkColor = Office2003Colors.MenuItemHotColorDark;

			Rectangle captionRect = this.ControlBar.CaptionRect;
			bool bContainFocus = ( this.ControlBar.ClientControl != null ) 
				&& ( this.ControlBar.ClientControl.ContainsFocus == true );

			CommandBarPainter.PaintControlBarBackground( g, rect, captionRect, bRTL, bContainFocus,
				borderColor, lightColor, darkColor, activeLightColor, activeDarkColor );
		}

		/// <summary>
		/// Draws DropBownButton of the ControlBar with Office2003 visual style.
		/// </summary>
		protected override void DrawDropDown(
			Graphics g, Rectangle rect, bool bRTL, bool bVertical, Syncfusion.Windows.Forms.Tools.CBButtonState state, bool bShowChevron, bool bShowArrow )
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			if( state == CBButtonState.Hot )
			{
				borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
				fillColor = Office2003Colors.SelColor;
			}
			else if( state == CBButtonState.Pressed )
			{
				borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
				fillColor = Office2003Colors.FloatingCommandBarItemPressedColor;
			}

			CommandBarPainter.PaintControlBarDropDown( g, rect, bRTL, borderColor, fillColor);
		}

		/// <summary>
		/// Draws close button of the ControlBar.
		/// </summary>
		protected override void DrawCloseButton(Graphics g, Rectangle rect, Syncfusion.Windows.Forms.Tools.CBButtonState state)
		{
			Color borderColor = Color.Empty;
			Color fillColor = Color.Empty;

			switch( state )
			{
				case CBButtonState.Hot :
				{
					borderColor = Office2003Colors.FloatingCommandBarCaptionColor;
					fillColor = Office2003Colors.SelColor;
					break;
				}
				case CBButtonState.Pressed :
				{
					borderColor = Office2003Colors.ControlBorderColorDark;
					fillColor = Office2003Colors.PressedSelColor;
					break;
				}
				default :
				{
					borderColor = Color.Empty;
					fillColor = Color.Empty;
					break;
				}
			}

			CommandBarPainter.PaintControlBarCloseButton( g, rect, borderColor, fillColor );
		}
		/// <summary>
		/// Draws gripper of the ControlBar with Office2003 visual style.
		/// </summary>
		protected override void DrawGripper(Graphics g, Rectangle rect, bool bRTL, bool bVertical)
		{
			CommandBarPainter.PaintGripper( g, rect, bRTL, false, 
				Color.White, Office2003Colors.ControlGripperColor );
		}

		#endregion
	}
}
